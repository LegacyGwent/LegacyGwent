#!/usr/bin/env bash
set -euo pipefail

if [[ "$(id -u)" -ne 0 ]]; then
    echo "run this script as root" >&2
    exit 1
fi

script_dir="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"

if ss -lnt | awk '{print $4}' | grep -Eq '(^|:)5010$'; then
    owner="$(ss -lntp | grep -E '(^|:)5010([[:space:]]|$)' || true)"
    if ! systemctl is-active --quiet card-diy-ai.service; then
        echo "port 5010 is already occupied: $owner" >&2
        exit 1
    fi
fi

getent passwd card-diy-ai >/dev/null || \
    useradd --system --home-dir /var/lib/card-diy-ai --shell /usr/sbin/nologin card-diy-ai
getent passwd card-deploy >/dev/null || \
    useradd --system --create-home --home-dir /var/lib/card-deploy --shell /bin/bash card-deploy
getent passwd mongodb >/dev/null

install -d -o card-diy-ai -g card-diy-ai -m 0755 \
    /usr/share/card-diy-ai /usr/share/card-diy-ai/releases /var/lib/card-diy-ai
install -d -o mongodb -g mongodb -m 0750 /var/lib/mongodb-diy-ai
install -d -o mongodb -g mongodb -m 0755 /var/log/mongodb
install -d -o card-deploy -g card-deploy -m 0700 \
    /var/lib/card-deploy/.ssh /var/lib/card-deploy/uploads

if [[ ! -f /etc/card-diy-ai.env ]]; then
    install -o root -g card-diy-ai -m 0640 \
        "$script_dir/card-diy-ai.env.example" /etc/card-diy-ai.env
fi
if ! grep -q '^DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=' /etc/card-diy-ai.env; then
    printf '%s\n' 'DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=1' >>/etc/card-diy-ai.env
fi

install -o root -g root -m 0644 "$script_dir/card-diy-ai.service" /etc/systemd/system/card-diy-ai.service
install -o root -g root -m 0644 "$script_dir/mongod-diy-ai.service" /etc/systemd/system/mongod-diy-ai.service
install -o root -g root -m 0755 "$script_dir/deploy.sh" /usr/local/sbin/deploy-card-diy-ai
install -o root -g root -m 0755 "$script_dir/sync-from-diy.sh" /usr/local/sbin/sync-card-diy-to-ai

cat >/etc/sudoers.d/card-diy-ai-deploy <<'EOF'
card-deploy ALL=(root) NOPASSWD: /usr/local/sbin/deploy-card-diy-ai *
EOF
chmod 0440 /etc/sudoers.d/card-diy-ai-deploy
visudo -cf /etc/sudoers.d/card-diy-ai-deploy >/dev/null

systemctl daemon-reload
systemctl enable mongod-diy-ai.service >/dev/null
systemctl restart mongod-diy-ai.service
systemctl enable card-diy-ai.service >/dev/null

echo "DIY-AI server environment is ready; upload the first release before starting card-diy-ai."
