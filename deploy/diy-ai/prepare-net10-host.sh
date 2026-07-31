#!/usr/bin/env bash
set -euo pipefail

if [[ "$(id -u)" -ne 0 ]]; then
    echo "run this script as root" >&2
    exit 1
fi

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
glibc_version="$(getconf GNU_LIBC_VERSION | awk '{print $2}')"
if ! printf '%s\n%s\n' '2.27' "$glibc_version" | sort --version-sort --check=quiet; then
    echo ".NET 10 linux-x64 requires glibc 2.27 or newer; found $glibc_version" >&2
    exit 2
fi

systemctl is-active --quiet card-diy-ai.service
test -f /usr/share/card-diy-ai/current/Cynthia.Card.Server.dll

# Install the transition pieces without restarting the running service. The
# next normal deployment will restart through the rollback-compatible launcher.
install -o root -g root -m 0755 "$script_dir/deploy.sh" /usr/local/sbin/deploy-card-diy-ai
install -o root -g root -m 0755 "$script_dir/run-server.sh" /usr/local/sbin/run-card-diy-ai
install -o root -g root -m 0644 "$script_dir/card-diy-ai.service" /etc/systemd/system/card-diy-ai.service
systemctl daemon-reload

test -x /usr/local/sbin/deploy-card-diy-ai
test -x /usr/local/sbin/run-card-diy-ai
systemctl is-active --quiet card-diy-ai.service

echo "DIY-AI host is prepared for the first .NET 10 deployment; the running release was not restarted."
