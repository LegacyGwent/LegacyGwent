#!/usr/bin/env bash
set -euo pipefail

release_dir="/usr/share/card-diy-ai/current"
self_contained_host="$release_dir/Cynthia.Card.Server"
framework_dll="$release_dir/Cynthia.Card.Server.dll"

if [[ -x "$self_contained_host" ]]; then
    exec "$self_contained_host"
fi

# Transitional rollback support for releases created before the .NET 10
# migration. Remove this branch after all retained releases are self-contained.
if [[ -f "$framework_dll" && -x /usr/bin/dotnet ]]; then
    exec /usr/bin/dotnet "$framework_dll"
fi

echo "No runnable DIY-AI server found under $release_dir" >&2
exit 126
