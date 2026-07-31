#!/usr/bin/env bash
set -euo pipefail

release_id="${1:-}"
archive="${2:-}"
release_root="/usr/share/card-diy-ai/releases"
current_link="/usr/share/card-diy-ai/current"
upload_root="/var/lib/card-deploy/uploads"

glibc_version="$(getconf GNU_LIBC_VERSION | awk '{print $2}')"
if ! printf '%s\n%s\n' '2.27' "$glibc_version" | sort --version-sort --check=quiet; then
    echo ".NET 10 linux-x64 requires glibc 2.27 or newer; found $glibc_version" >&2
    exit 2
fi

if [[ ! "$release_id" =~ ^[0-9a-f]{40}$ ]]; then
    echo "release id must be a full Git commit SHA" >&2
    exit 2
fi

expected_archive="$upload_root/gwent-diy-ai-$release_id.tar.gz"
if [[ "$archive" != "$expected_archive" || ! -f "$archive" ]]; then
    echo "archive must be $expected_archive" >&2
    exit 2
fi

release_dir="$release_root/$release_id"
staging_dir="$release_root/.staging-$release_id"
previous_release=""
if [[ -L "$current_link" ]]; then
    previous_release="$(readlink -f "$current_link")"
fi

cleanup_staging() {
    if [[ -d "$staging_dir" ]]; then
        rm -rf -- "$staging_dir"
    fi
}
trap cleanup_staging EXIT

if [[ ! -d "$release_dir" ]]; then
    install -d -o card-diy-ai -g card-diy-ai -m 0755 "$staging_dir"
    tar -xzf "$archive" -C "$staging_dir"
    test -f "$staging_dir/Cynthia.Card.Server.dll"
    test -x "$staging_dir/Cynthia.Card.Server"
    grep -q '"tfm": "net10.0"' "$staging_dir/Cynthia.Card.Server.runtimeconfig.json"
    chown -R card-diy-ai:card-diy-ai "$staging_dir"
    chmod -R u=rwX,g=rX,o= "$staging_dir"
    mv "$staging_dir" "$release_dir"
fi

next_link="/usr/share/card-diy-ai/.current-$release_id"
ln -s "$release_dir" "$next_link"
mv -Tf "$next_link" "$current_link"

systemctl enable card-diy-ai.service >/dev/null
healthy=false
if systemctl restart card-diy-ai.service; then
    for _ in $(seq 1 30); do
        if curl --silent --show-error --fail --max-time 2 http://127.0.0.1:5010/healthz >/dev/null; then
            healthy=true
            break
        fi
        sleep 2
    done
else
    echo "DIY-AI candidate service restart failed" >&2
fi

if [[ "$healthy" != true ]]; then
    echo "DIY-AI health check failed; rolling back" >&2
    if [[ -n "$previous_release" && -d "$previous_release" ]]; then
        rollback_link="/usr/share/card-diy-ai/.rollback-$release_id"
        ln -s "$previous_release" "$rollback_link"
        mv -Tf "$rollback_link" "$current_link"
        rollback_healthy=false
        if systemctl restart card-diy-ai.service; then
            for _ in $(seq 1 30); do
                if curl --silent --show-error --fail --max-time 2 http://127.0.0.1:5010/healthz >/dev/null; then
                    rollback_healthy=true
                    break
                fi
                sleep 2
            done
        else
            echo "DIY-AI rollback service restart failed" >&2
        fi
        if [[ "$rollback_healthy" != true ]]; then
            echo "DIY-AI rollback release also failed its health check; stopping the service" >&2
            systemctl stop card-diy-ai.service || true
        else
            echo "DIY-AI rollback release is healthy: $previous_release" >&2
        fi
    else
        systemctl stop card-diy-ai.service || true
    fi
    journalctl -u card-diy-ai.service --no-pager -n 80 >&2 || true
    exit 1
fi

rm -f -- "$archive"

mapfile -t old_releases < <(
    find "$release_root" -mindepth 1 -maxdepth 1 -type d -printf '%T@ %p\n' \
        | sort -rn \
        | tail -n +6 \
        | cut -d' ' -f2-
)
for old_release in "${old_releases[@]}"; do
    case "$old_release" in
        "$release_root"/*) rm -rf -- "$old_release" ;;
    esac
done

echo "DIY-AI $release_id is healthy on port 5010"
