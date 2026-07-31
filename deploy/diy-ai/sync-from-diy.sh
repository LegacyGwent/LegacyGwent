#!/usr/bin/env bash
set -Eeuo pipefail

if [[ "${1:-}" != "--execute" ]]; then
    echo "usage: $0 --execute" >&2
    echo "Copies an online snapshot of DIY MongoDB 28020 over DIY-AI MongoDB 28021." >&2
    exit 2
fi
if [[ "$(id -u)" -ne 0 ]]; then
    echo "run this script as root" >&2
    exit 1
fi

source_host="127.0.0.1"
source_port="28020"
target_host="127.0.0.1"
target_port="28021"
databases=(gwentdiy Web)
backup_root="/var/backups/legacy-gwent/diy-ai-sync"
lock_file="/run/lock/card-diy-ai-db-sync.lock"
run_dir=""
target_service_stopped=false
target_mutated=false
completed=false

for tool in mongo mongodump mongorestore curl flock gzip mktemp; do
    command -v "$tool" >/dev/null || {
        echo "missing required tool: $tool" >&2
        exit 1
    }
done

umask 077
install -d -o root -g root -m 0700 "$backup_root"
exec 9>"$lock_file"
flock -n 9 || {
    echo "another DIY-AI database sync is already running" >&2
    exit 1
}

run_dir="$(mktemp -d "$backup_root/$(date -u +%Y%m%dT%H%M%SZ).XXXXXX")"
printf 'state=preparing\n' >"$run_dir/status"

database_state() {
    local port="$1"
    local database="$2"
    local output
    local result

    if ! output="$(mongo --quiet --host 127.0.0.1 --port "$port" --eval \
        "db.adminCommand({listDatabases:1,nameOnly:true}).databases.some(function(x){return x.name=='$database'})")"; then
        echo "failed to inspect database $database on port $port" >&2
        return 1
    fi
    result="$(printf '%s\n' "$output" | tail -n 1 | tr -d '\r')"
    case "$result" in
        true) printf 'present\n' ;;
        false) printf 'absent\n' ;;
        *)
            echo "unexpected database inspection result for $database on port $port: $result" >&2
            return 1
            ;;
    esac
}

write_counts() {
    local port="$1"
    local database="$2"
    local output="$3"
    mongo --quiet --host 127.0.0.1 --port "$port" "$database" --eval \
        'db.getCollectionNames().sort().forEach(function(n){print(n+"|"+db.getCollection(n).count())})' \
        >"$output"
}

wait_for_health() {
    local attempts="${1:-30}"
    local delay_seconds="${2:-2}"
    local attempt
    for attempt in $(seq 1 "$attempts"); do
        if curl --silent --show-error --fail --max-time 2 \
            http://127.0.0.1:5010/healthz >/dev/null; then
            return 0
        fi
        sleep "$delay_seconds"
    done
    return 1
}

restore_previous_target() {
    local original_status="$?"
    local rollback_status=0
    local can_restore=true
    local database

    trap - EXIT
    trap '' INT TERM
    set +e
    if [[ "$completed" == true ]]; then
        return
    fi

    if [[ "$target_mutated" == true ]]; then
        echo "sync failed; restoring the previous DIY-AI databases" >&2
        systemctl stop card-diy-ai.service || true
        if systemctl is-active --quiet card-diy-ai.service; then
            echo "refusing to restore while card-diy-ai.service is still active" >&2
            rollback_status=1
            can_restore=false
        fi
        if [[ "$can_restore" == true ]]; then
            for database in "${databases[@]}"; do
                mongo --quiet --host "$target_host" --port "$target_port" "$database" \
                    --eval 'db.dropDatabase()' >/dev/null || rollback_status=1
                if [[ -f "$run_dir/target-before-$database.ready" ]]; then
                    mongorestore --quiet --host "$target_host" --port "$target_port" \
                        --archive="$run_dir/target-before-$database.archive.gz" --gzip \
                        || rollback_status=1
                elif [[ ! -f "$run_dir/target-before-$database.absent" ]]; then
                    echo "missing rollback state for database: $database" >&2
                    rollback_status=1
                fi
            done
        fi
    fi

    if [[ "$target_service_stopped" == true ]]; then
        systemctl start card-diy-ai.service || rollback_status=1
        if ! wait_for_health 15 2; then
            echo "DIY-AI failed its health check after rollback" >&2
            rollback_status=1
        fi
    fi

    if [[ "$rollback_status" -eq 0 ]]; then
        printf 'state=rolled-back\n' >"$run_dir/status"
        echo "previous DIY-AI state restored" >&2
    else
        printf 'state=rollback-failed\n' >"$run_dir/status"
        echo "WARNING: DIY-AI rollback did not complete cleanly" >&2
    fi
    echo "recovery artifacts: $run_dir" >&2
    if [[ "$original_status" -eq 0 ]]; then
        original_status=1
    fi
    exit "$original_status"
}

trap restore_previous_target EXIT
trap 'exit 130' INT
trap 'exit 143' TERM

if ! systemctl is-active --quiet card-diy-ai.service; then
    echo "card-diy-ai.service must be healthy before synchronization" >&2
    exit 1
fi
if ! wait_for_health 1 0; then
    echo "card-diy-ai.service must be healthy before synchronization" >&2
    exit 1
fi

available_kb="$(df -Pk "$backup_root" | awk 'NR==2 {print $4}')"
if [[ "$available_kb" -lt 1048576 ]]; then
    echo "at least 1 GiB of free backup space is required" >&2
    exit 1
fi

echo "creating online source snapshots"
for database in "${databases[@]}"; do
    source_state="$(database_state "$source_port" "$database")"
    if [[ "$source_state" != "present" ]]; then
        echo "source database does not exist: $database" >&2
        exit 1
    fi
    write_counts "$source_port" "$database" "$run_dir/source-before-$database.counts"
    mongodump --quiet --host "$source_host" --port "$source_port" --db "$database" \
        --archive="$run_dir/source-$database.archive.gz.partial" --gzip
    test -s "$run_dir/source-$database.archive.gz.partial"
    gzip -t "$run_dir/source-$database.archive.gz.partial"
    mv "$run_dir/source-$database.archive.gz.partial" "$run_dir/source-$database.archive.gz"
    write_counts "$source_port" "$database" "$run_dir/source-after-$database.counts"
    if ! diff -u "$run_dir/source-before-$database.counts" \
        "$run_dir/source-after-$database.counts" >"$run_dir/source-drift-$database.diff"; then
        echo "warning: live source counts changed while dumping $database" >&2
    else
        rm -f "$run_dir/source-drift-$database.diff"
    fi
done

target_service_stopped=true
systemctl stop card-diy-ai.service

echo "backing up the previous DIY-AI databases"
for database in "${databases[@]}"; do
    target_state="$(database_state "$target_port" "$database")"
    if [[ "$target_state" == "present" ]]; then
        mongodump --quiet --host "$target_host" --port "$target_port" --db "$database" \
            --archive="$run_dir/target-before-$database.archive.gz.partial" --gzip
        test -s "$run_dir/target-before-$database.archive.gz.partial"
        gzip -t "$run_dir/target-before-$database.archive.gz.partial"
        mv "$run_dir/target-before-$database.archive.gz.partial" \
            "$run_dir/target-before-$database.archive.gz"
        touch "$run_dir/target-before-$database.ready"
    else
        touch "$run_dir/target-before-$database.absent"
    fi
done
touch "$run_dir/target-backups.complete"

for database in "${databases[@]}"; do
    if [[ ! -f "$run_dir/target-before-$database.ready" \
        && ! -f "$run_dir/target-before-$database.absent" ]]; then
        echo "target backup state is incomplete for database: $database" >&2
        exit 1
    fi
done

echo "replacing DIY-AI databases from the source snapshots"
target_mutated=true
printf 'state=restoring\n' >"$run_dir/status"
for database in "${databases[@]}"; do
    mongo --quiet --host "$target_host" --port "$target_port" "$database" \
        --eval 'db.dropDatabase()' >/dev/null
    mongorestore --quiet --host "$target_host" --port "$target_port" \
        --archive="$run_dir/source-$database.archive.gz" --gzip
    write_counts "$target_port" "$database" "$run_dir/target-after-$database.counts"
done

systemctl start card-diy-ai.service
if ! wait_for_health 30 2; then
    echo "DIY-AI failed its health check after database synchronization" >&2
    exit 1
fi

completed=true
trap - EXIT INT TERM
target_service_stopped=false
target_mutated=false
printf 'state=complete\n' >"$run_dir/status"
echo "DIY-AI database sync completed: $run_dir"
echo "The source remained online; this is a best-effort standalone MongoDB snapshot."
