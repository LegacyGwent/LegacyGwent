#!/usr/bin/env bash
set -Eeuo pipefail

mode="${1:---dry-run}"
if [[ "$mode" != "--dry-run" && "$mode" != "--execute" ]]; then
    echo "usage: $0 [--dry-run|--execute]" >&2
    exit 2
fi
if [[ "$(id -u)" -ne 0 ]]; then
    echo "run this script as root" >&2
    exit 1
fi

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
mongo_host="127.0.0.1"
mongo_port="28021"
database="gwentdiy"
service="card-diy-ai.service"
backup_root="/var/backups/legacy-gwent/diy-ai-card-reset"
lock_file="/run/lock/card-diy-ai-card-reset.lock"
run_dir=""
service_stopped=false
database_mutated=false
completed=false

for tool in mongo mongodump mongorestore gzip flock mktemp curl; do
    command -v "$tool" >/dev/null || { echo "missing required tool: $tool" >&2; exit 1; }
done
test -f "$script_dir/reset-card-pool.js"

umask 077
exec 9>"$lock_file"
flock -n 9 || { echo "another DIY-AI card reset is already running" >&2; exit 1; }

run_migration() {
    local execute_value="$1"
    mongo --quiet --host "$mongo_host" --port "$mongo_port" "$database" \
        --eval "var EXECUTE=$execute_value;" "$script_dir/reset-card-pool.js" | tail -n 1
}

if [[ "$mode" == "--dry-run" ]]; then
    run_migration false
    exit 0
fi

install -d -o root -g root -m 0700 "$backup_root"

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
    if [[ "$completed" == true ]]; then
        return
    fi
    if [[ "$database_mutated" == true ]]; then
        systemctl stop "$service" || true
        mongo --quiet --host "$mongo_host" --port "$mongo_port" "$database" \
            --eval 'db.dropDatabase()' >/dev/null || rollback_status=1
        mongorestore --quiet --host "$mongo_host" --port "$mongo_port" \
            --archive="$run_dir/before.archive.gz" --gzip || rollback_status=1
    fi
    if [[ "$service_stopped" == true ]]; then
        systemctl start "$service" || rollback_status=1
        wait_for_health || rollback_status=1
    fi
    printf 'state=%s\n' "$([[ "$rollback_status" -eq 0 ]] && echo rolled-back || echo rollback-failed)" >"$run_dir/status"
    echo "card reset failed; recovery artifacts: $run_dir" >&2
    exit "$original_status"
}
systemctl is-active --quiet "$service"
wait_for_health
run_dir="$(mktemp -d "$backup_root/$(date -u +%Y%m%dT%H%M%SZ).XXXXXX")"
trap rollback EXIT
trap 'exit 130' INT
trap 'exit 143' TERM
printf 'state=backing-up\n' >"$run_dir/status"
mongodump --quiet --host "$mongo_host" --port "$mongo_port" --db "$database" \
    --archive="$run_dir/before.archive.gz.partial" --gzip
test -s "$run_dir/before.archive.gz.partial"
gzip -t "$run_dir/before.archive.gz.partial"
mv "$run_dir/before.archive.gz.partial" "$run_dir/before.archive.gz"

service_stopped=true
systemctl stop "$service"
systemctl is-active --quiet "$service" && { echo "$service did not stop" >&2; exit 1; }
database_mutated=true
printf 'state=migrating\n' >"$run_dir/status"
run_migration true | tee "$run_dir/result.json"
run_migration false | tee "$run_dir/post-check.json"
grep -q '"removedDecks":0' "$run_dir/post-check.json"
grep -q '"removedBlacklistEntries":0' "$run_dir/post-check.json"

systemctl start "$service"
wait_for_health
service_stopped=false
database_mutated=false
completed=true
printf 'state=complete\n' >"$run_dir/status"
trap - EXIT INT TERM
echo "DIY-AI card pool reset complete: $run_dir"
