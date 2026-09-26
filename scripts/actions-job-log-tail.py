#!/usr/bin/env python3
"""Read a bounded tail of an Actions job log, including a running job's snapshot."""
import argparse
import re
import subprocess
import sys
import urllib.error
import urllib.parse
import urllib.request


class NoRedirect(urllib.request.HTTPRedirectHandler):
    def redirect_request(self, req, fp, code, msg, headers, newurl):
        return None


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("job_id", type=int)
    parser.add_argument("--repo", default="LegacyGwent/LegacyGwent")
    parser.add_argument("--lines", type=int, default=30)
    args = parser.parse_args()
    if args.job_id <= 0 or not 1 <= args.lines <= 500 or not re.fullmatch(r"[\w.-]+/[\w.-]+", args.repo):
        parser.error("Expected a positive job ID, owner/repository and 1-500 lines")

    # Use the existing CLI login only for api.github.com; never forward this token
    # to the signed storage URL, log it, or persist it in the checkout.
    token = subprocess.check_output(["gh", "auth", "token", "--hostname", "github.com"], text=True).strip()
    request = urllib.request.Request(
        f"https://api.github.com/repos/{args.repo}/actions/jobs/{args.job_id}/logs",
        headers={"Authorization": "Bearer " + token, "Accept": "application/vnd.github+json"})
    try:
        response = urllib.request.build_opener(NoRedirect()).open(request, timeout=30)
    except urllib.error.HTTPError as error:
        if error.code != 302:
            raise
        location = error.headers["Location"]
        if urllib.parse.urlparse(location).scheme != "https":
            raise ValueError("Rejected non-HTTPS log location")
        with urllib.request.urlopen(urllib.request.Request(location, method="HEAD"), timeout=30) as metadata:
            size = int(metadata.headers.get("Content-Length", "0"))
        # Azure log storage needs an explicit start byte; suffix ranges may be
        # ignored and download the entire multi-megabyte Unity import log.
        response = urllib.request.urlopen(urllib.request.Request(location,
            headers={"Range": f"bytes={max(0, size - 65536)}-"}), timeout=30)
    tail = b""
    with response:
        while True:
            block = response.read(65536)
            if not block:
                break
            tail = (tail + block)[-65536:]
    for line in tail.decode("utf-8", errors="replace").splitlines()[-args.lines:]:
        if any(word in line.upper() for word in ("LICENSE", "PASSWORD", "KEYSTORE", "SERIAL", "TOKEN", "AUTHORIZATION")):
            print("[credential-related log line omitted]")
        else:
            print(line)


if __name__ == "__main__":
    try:
        main()
    except urllib.error.HTTPError as error:
        sys.exit(f"Log request failed: HTTP {error.code}")
    except (urllib.error.URLError, TimeoutError, ValueError, subprocess.CalledProcessError) as error:
        # Exception URLs may contain temporary signed storage credentials.
        sys.exit(f"Log request failed ({type(error).__name__}); check connectivity and gh authentication.")
