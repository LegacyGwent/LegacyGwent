#!/usr/bin/env bash
set -Eeuo pipefail

mode="${1:---dry-run}"
preimage_archive="${2:-/var/backups/legacy-gwent/diy-ai-card-reset/20260915T162954Z.rxtB98/before.archive.gz}"
if [[ "$mode" != "--dry-run" && "$mode" != "--execute" ]]; then
    echo "usage: $0 [--dry-run|--execute] [preimage-archive]" >&2
    exit 2
fi
if [[ "$(id -u)" -ne 0 ]]; then
    echo "run this script as root" >&2
    exit 1
fi

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
mongo_host="127.0.0.1"
mongo_port="28021"
live_database="gwentdiy"
preimage_database="gwentdiy_sep15_pre"
service="card-diy-ai.service"
backup_root="/var/backups/legacy-gwent/diy-ai-draug-restore"
lock_file="/run/lock/card-diy-ai-draug-restore.lock"
run_dir=""
service_stopped=false
database_mutated=false
completed=false

for tool in mongo mongodump mongorestore gzip flock mktemp curl; do
    command -v "$tool" >/dev/null || { echo "missing required tool: $tool" >&2; exit 1; }
done
test -f "$script_dir/restore-sep15-draug.js"
test -s "$preimage_archive"

umask 077
exec 9>"$lock_file"
flock -n 9 || { echo "another DIY-AI Draug restore is already running" >&2; exit 1; }

drop_preimage() {
    mongo --quiet --host "$mongo_host" --port "$mongo_port" "$preimage_database" \
        --eval 'db.dropDatabase()' >/dev/null
}

load_preimage() {
    drop_preimage
    mongorestore --quiet --host "$mongo_host" --port "$mongo_port" \
        --archive="$preimage_archive" --gzip \
        --nsFrom="$live_database.*" --nsTo="$preimage_database.*"
}

run_restore() {
    local execute_value="$1"
    mongo --quiet --host "$mongo_host" --port "$mongo_port" "$live_database" \
        --eval "var EXECUTE=$execute_value;" "$script_dir/restore-sep15-draug.js" | tail -n 1
}

wait_for_health() {
    local attempt
    for attempt in $(seq 1 30); do
        if curl --silent --show-error --fail --max-time 2 http://127.0.0.1:5010/healthz >/dev/null; then
            return 0
        fi
        sleep 2
    done
    return 1
}

rollback() {
    local original_status="$?"
    local rollback_status=0
    trap - EXIT INT TERM
    set +e
    drop_preimage || true
    if [[ "$completed" == true ]]; then
        return
    fi
    if [[ "$database_mutated" == true ]]; then
        systemctl stop "$service" || true
        mongo --quiet --host "$mongo_host" --port "$mongo_port" "$live_database" \
            --eval 'db.dropDatabase()' >/dev/null || rollback_status=1
        mongorestore --quiet --host "$mongo_host" --port "$mongo_port" \
            --archive="$run_dir/before.archive.gz" --gzip || rollback_status=1
    fi
    if [[ "$service_stopped" == true ]]; then
        systemctl start "$service" || rollback_status=1
        wait_for_health || rollback_status=1
    fi
    if [[ -n "$run_dir" ]]; then
        printf 'state=%s\n' "$([[ "$rollback_status" -eq 0 ]] && echo rolled-back || echo rollback-failed)" >"$run_dir/status"
        echo "Draug restore failed; recovery artifacts: $run_dir" >&2
    fi
    exit "$original_status"
}

load_preimage
if [[ "$mode" == "--dry-run" ]]; then
    run_restore false
    drop_preimage
    exit 0
fi

systemctl is-active --quiet "$service"
wait_for_health
install -d -o root -g root -m 0700 "$backup_root"
run_dir="$(mktemp -d "$backup_root/$(date -u +%Y%m%dT%H%M%SZ).XXXXXX")"
trap rollback EXIT
trap 'exit 130' INT
trap 'exit 143' TERM

printf 'state=backing-up\n' >"$run_dir/status"
run_restore false | tee "$run_dir/dry-run.json"
grep -q '"changedDecksWithReplacement":0' "$run_dir/dry-run.json"
grep -q '"missingLiveUsers":0' "$run_dir/dry-run.json"
grep -q '"missingLiveDecks":0' "$run_dir/dry-run.json"
grep -q '"writeConflicts":0' "$run_dir/dry-run.json"
mongodump --quiet --host "$mongo_host" --port "$mongo_port" --db "$live_database" \
    --archive="$run_dir/before.archive.gz.partial" --gzip
test -s "$run_dir/before.archive.gz.partial"
gzip -t "$run_dir/before.archive.gz.partial"
mv "$run_dir/before.archive.gz.partial" "$run_dir/before.archive.gz"

service_stopped=true
systemctl stop "$service"
systemctl is-active --quiet "$service" && { echo "$service did not stop" >&2; exit 1; }
database_mutated=true
printf 'state=restoring\n' >"$run_dir/status"
run_restore true | tee "$run_dir/result.json"
run_restore false | tee "$run_dir/post-check.json"
grep -q '"exactMatches":0' "$run_dir/post-check.json"
grep -q '"changedDecksWithReplacement":0' "$run_dir/post-check.json"
grep -q '"missingLiveUsers":0' "$run_dir/post-check.json"
grep -q '"missingLiveDecks":0' "$run_dir/post-check.json"
grep -q '"writeConflicts":0' "$run_dir/post-check.json"

systemctl start "$service"
wait_for_health
service_stopped=false
database_mutated=false
completed=true
printf 'state=complete\n' >"$run_dir/status"
drop_preimage
trap - EXIT INT TERM
echo "DIY-AI Draug restore complete: $run_dir"
