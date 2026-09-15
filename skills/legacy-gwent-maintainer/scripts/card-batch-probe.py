#!/usr/bin/env python3
"""Create committed card snapshots and verify a deployed DIY-AI release read-only."""

from __future__ import annotations

import argparse
import base64
import json
import os
from pathlib import Path
import re
import subprocess
import sys
import tempfile
from typing import Any, Iterable


PROPERTIES = [
    "CardId",
    "Name",
    "Strength",
    "Group",
    "Faction",
    "CardType",
    "CardUseInfo",
    "Info",
    "Categories",
    "IsDerive",
    "IsCountdown",
    "Countdown",
    "LinkedCards",
    "CardArtsId",
]
REPO_ROOT = Path(__file__).resolve().parents[3]
PROJECT = REPO_ROOT / "tools" / "CardBatchProbe" / "CardBatchProbe.csproj"
SHA_PATTERN = re.compile(r"^[0-9a-f]{40}$")
VERSION_PATTERN = re.compile(r"^[0-9]+(?:\.[0-9]+){3}$")
HOST_PATTERN = re.compile(r"^[A-Za-z0-9_.-]+@[A-Za-z0-9.-]+$")


class ProbeError(RuntimeError):
    pass


class JsonArgumentParser(argparse.ArgumentParser):
    def error(self, message: str) -> None:
        print(json.dumps({"passed": False, "error": message}, separators=(",", ":")), file=sys.stderr)
        raise SystemExit(2)


def parse_card_ids(value: str) -> list[str]:
    ids = [part.strip() for part in value.split(",")]
    if not ids or any(not card_id or not card_id.isascii() or not card_id.isdigit() for card_id in ids):
        raise argparse.ArgumentTypeError("--cards must be a comma-separated list of numeric card IDs")
    duplicates = sorted({card_id for card_id in ids if ids.count(card_id) > 1})
    if duplicates:
        raise argparse.ArgumentTypeError("duplicate card IDs: " + ",".join(duplicates))
    return ids


def _run(command: list[str], *, cwd: Path, timeout: int = 120, input_text: str | None = None) -> subprocess.CompletedProcess[str]:
    try:
        return subprocess.run(
            command,
            cwd=cwd,
            input=input_text,
            capture_output=True,
            text=True,
            encoding="utf-8",
            timeout=timeout,
            check=True,
            shell=False,
        )
    except subprocess.TimeoutExpired as error:
        raise ProbeError(f"command timed out after {timeout}s: {command[0]}") from error
    except subprocess.CalledProcessError as error:
        detail = (error.stderr or error.stdout or "").strip()
        raise ProbeError(f"{command[0]} failed: {detail}") from error


def _git_head() -> str:
    head = _run(["git", "rev-parse", "--verify", "HEAD"], cwd=REPO_ROOT).stdout.strip().lower()
    if not SHA_PATTERN.fullmatch(head):
        raise ProbeError(f"unexpected Git HEAD: {head!r}")
    return head


def _git_dirty() -> bool:
    return bool(_run(["git", "status", "--porcelain", "--untracked-files=all"], cwd=REPO_ROOT).stdout)


def _pairs_without_duplicates(pairs: Iterable[tuple[str, Any]]) -> dict[str, Any]:
    result: dict[str, Any] = {}
    for key, value in pairs:
        if key in result:
            raise ProbeError(f"duplicate JSON key: {key}")
        result[key] = value
    return result


def _read_json(path: Path) -> Any:
    try:
        return json.loads(path.read_text(encoding="utf-8"), object_pairs_hook=_pairs_without_duplicates)
    except (OSError, UnicodeError, json.JSONDecodeError) as error:
        raise ProbeError(f"cannot read snapshot {path}: {error}") from error


def _validate_compiled_cards(requested: list[str], compiled: Any) -> dict[str, Any]:
    if not isinstance(compiled, dict) or set(compiled) != {"version", "properties", "cards"}:
        raise ProbeError("compiled payload has an unexpected shape")
    if not isinstance(compiled["version"], str) or not VERSION_PATTERN.fullmatch(compiled["version"]):
        raise ProbeError("compiled payload has an invalid CardMap version")
    if compiled["properties"] != PROPERTIES:
        raise ProbeError("compiled payload does not contain the required 14 fields")
    cards = compiled["cards"]
    if not isinstance(cards, dict) or list(cards) != requested:
        missing = [card_id for card_id in requested if not isinstance(cards, dict) or card_id not in cards]
        extra = [] if not isinstance(cards, dict) else [card_id for card_id in cards if card_id not in requested]
        raise ProbeError(f"compiled card IDs differ from the request; missing={missing}, extra={extra}")
    for card_id, card in cards.items():
        if not isinstance(card, dict):
            raise ProbeError(f"compiled card {card_id} is not an object")
        absent = [name for name in PROPERTIES if name not in card]
        if absent:
            raise ProbeError(f"compiled card {card_id} lacks fields: {','.join(absent)}")
    return compiled


def validate_snapshot(snapshot: Any) -> dict[str, Any]:
    if not isinstance(snapshot, dict) or set(snapshot) != {"schemaVersion", "source", "version", "properties", "cards"}:
        raise ProbeError("snapshot has an unexpected top-level shape")
    if snapshot["schemaVersion"] != 1:
        raise ProbeError("unsupported snapshot schemaVersion")
    source = snapshot["source"]
    if not isinstance(source, dict) or set(source) != {"headSha", "dirty"}:
        raise ProbeError("snapshot source has an unexpected shape")
    if not isinstance(source["headSha"], str) or not SHA_PATTERN.fullmatch(source["headSha"]):
        raise ProbeError("snapshot source.headSha must be a full lowercase Git SHA")
    if type(source["dirty"]) is not bool:
        raise ProbeError("snapshot source.dirty must be a boolean")
    if snapshot["properties"] != PROPERTIES:
        raise ProbeError("snapshot does not contain the required 14 fields in canonical order")
    cards = snapshot["cards"]
    if not isinstance(cards, dict) or not cards:
        raise ProbeError("snapshot cards must be a non-empty object")
    if any(not card_id.isascii() or not card_id.isdigit() for card_id in cards):
        raise ProbeError("snapshot card keys must be numeric card IDs")
    _validate_compiled_cards(list(cards), {key: snapshot[key] for key in ("version", "properties", "cards")})
    for card_id, card in cards.items():
        if card["CardId"] != card_id:
            raise ProbeError(f"snapshot card key {card_id} does not match CardId {card['CardId']!r}")
    return snapshot


def verify_committed_source(snapshot: dict[str, Any], repo_head: str) -> None:
    if snapshot["source"]["dirty"]:
        raise ProbeError("snapshot was generated from a dirty worktree")
    if repo_head != snapshot["source"]["headSha"]:
        raise ProbeError(f"current HEAD {repo_head} does not match snapshot {snapshot['source']['headSha']}")


def verify_remote_actual(snapshot: dict[str, Any], actual: Any) -> dict[str, Any]:
    if not isinstance(actual, dict) or set(actual) != {"releaseSha", "version", "cards", "chineseInfo"}:
        raise ProbeError("remote probe returned an unexpected shape")
    expected_sha = snapshot["source"]["headSha"]
    if actual["releaseSha"] != expected_sha:
        raise ProbeError(f"active release {actual['releaseSha']!r} does not match snapshot {expected_sha}")
    if actual["version"] != snapshot["version"]:
        raise ProbeError(f"remote CardMap version {actual['version']!r} does not match snapshot {snapshot['version']!r}")
    if not isinstance(actual["cards"], dict) or not isinstance(actual["chineseInfo"], dict):
        raise ProbeError("remote cards or Chinese locales are not objects")
    for card_id, expected_card in snapshot["cards"].items():
        if card_id not in actual["cards"]:
            raise ProbeError(f"remote CardMap lacks card {card_id}")
        remote_card = actual["cards"][card_id]
        if not isinstance(remote_card, dict):
            raise ProbeError(f"remote card {card_id} is not an object")
        for field in PROPERTIES:
            if field not in remote_card:
                raise ProbeError(f"remote card {card_id} lacks field {field}")
            if remote_card[field] != expected_card[field]:
                raise ProbeError(f"remote mismatch: {card_id}:{field}")
        if actual["chineseInfo"].get(card_id) != expected_card["Info"]:
            raise ProbeError(f"remote mismatch: {card_id}:cn.Info")
    return {
        "passed": True,
        "releaseSha": actual["releaseSha"],
        "version": actual["version"],
        "cards": list(snapshot["cards"]),
        "propertiesPerCard": len(PROPERTIES),
        "chineseLocales": "PASS",
    }


def _atomic_write(path: Path, text: str) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    handle, temporary_name = tempfile.mkstemp(prefix=path.name + ".", suffix=".tmp", dir=path.parent)
    try:
        with os.fdopen(handle, "w", encoding="utf-8", newline="\n") as temporary:
            temporary.write(text)
            temporary.write("\n")
        os.replace(temporary_name, path)
    except BaseException:
        try:
            os.unlink(temporary_name)
        except FileNotFoundError:
            pass
        raise


def snapshot_command(card_ids: list[str], output: Path) -> dict[str, Any]:
    before = (_git_head(), _git_dirty())
    with tempfile.TemporaryDirectory(prefix="card-batch-probe-") as temporary_directory:
        compiled_path = Path(temporary_directory) / "compiled.json"
        _run(
            [
                "dotnet",
                "run",
                "--project",
                str(PROJECT),
                "--configuration",
                "Release",
                "--verbosity",
                "quiet",
                "--",
                "--cards",
                ",".join(card_ids),
                "--output",
                str(compiled_path),
            ],
            cwd=REPO_ROOT,
        )
        compiled = _validate_compiled_cards(card_ids, _read_json(compiled_path))
    after = (_git_head(), _git_dirty())
    if before != after:
        raise ProbeError("repository HEAD or dirty state changed while generating the snapshot")
    snapshot = {
        "schemaVersion": 1,
        "source": {"headSha": after[0], "dirty": after[1]},
        "version": compiled["version"],
        "properties": compiled["properties"],
        "cards": compiled["cards"],
    }
    encoded = json.dumps(snapshot, ensure_ascii=False, separators=(",", ":"))
    _atomic_write(output.resolve(), encoded)
    return snapshot


REMOTE_SCRIPT = r'''
import base64
import json
from pathlib import Path
import urllib.parse
import urllib.request

expected = json.loads(base64.b64decode("__SNAPSHOT_B64__").decode("utf-8"))
release_sha = Path("/usr/share/card-diy-ai/current").resolve(strict=True).name
if release_sha != expected["source"]["headSha"]:
    raise RuntimeError("active release does not match snapshot: " + release_sha)

opener = urllib.request.build_opener(urllib.request.ProxyHandler({}))
base = "http://127.0.0.1:5010/hub/gwent"
connection_url = None

def request(url, data=None, method=None):
    request_object = urllib.request.Request(url, data=data, method=method)
    if data is not None:
        request_object.add_header("Content-Type", "text/plain;charset=UTF-8")
    with opener.open(request_object, timeout=20) as response:
        return response.read().decode("utf-8")

try:
    negotiation = json.loads(request(base + "/negotiate?negotiateVersion=1", b"", "POST"))
    if not any(item["transport"] == "LongPolling" for item in negotiation["availableTransports"]):
        raise RuntimeError("server does not offer SignalR LongPolling")
    token = negotiation.get("connectionToken", negotiation["connectionId"])
    connection_url = base + "?id=" + urllib.parse.quote(token)
    request(connection_url)
    request(connection_url, b'{"protocol":"json","version":1}\x1e', "POST")
    handshake = [json.loads(part) for part in request(connection_url).split("\x1e") if part]
    if {} not in handshake:
        raise RuntimeError("SignalR handshake failed")

    calls = ["GetCardMapVersion", "GetCardMap", "GetGameLocales"]
    payload = "".join(
        json.dumps({"type": 1, "invocationId": str(index), "target": target, "arguments": []}) + "\x1e"
        for index, target in enumerate(calls)
    ).encode("utf-8")
    request(connection_url, payload, "POST")
    results = {}
    for unused in range(6):
        for part in request(connection_url).split("\x1e"):
            if not part:
                continue
            message = json.loads(part)
            if message.get("type") == 3:
                if "error" in message:
                    raise RuntimeError(message["error"])
                results[message["invocationId"]] = message["result"]
        if len(results) == 3:
            break
    if len(results) != 3:
        raise RuntimeError("missing SignalR invocation results")

    live_map = json.loads(results["1"])
    locales = json.loads(results["2"])
    chinese = next(locale for locale in locales if locale["Info"]["Filename"] == "cn")
    card_ids = list(expected["cards"])
    properties = expected["properties"]
    projected_cards = {
        card_id: {field: live_map[card_id][field] for field in properties}
        for card_id in card_ids
    }
    chinese_info = {
        card_id: chinese["CardLocales"][card_id]["Info"]
        for card_id in card_ids
    }
    print(json.dumps({
        "releaseSha": release_sha,
        "version": results["0"],
        "cards": projected_cards,
        "chineseInfo": chinese_info,
    }, ensure_ascii=False, separators=(",", ":")))
finally:
    if connection_url is not None:
        try:
            request(connection_url, method="DELETE")
        except Exception:
            pass
'''.lstrip()


def live_command(snapshot_path: Path, host: str) -> dict[str, Any]:
    snapshot = validate_snapshot(_read_json(snapshot_path))
    verify_committed_source(snapshot, _git_head())
    if not HOST_PATTERN.fullmatch(host):
        raise ProbeError("--host must use the user@hostname form")
    encoded_snapshot = base64.b64encode(
        json.dumps(snapshot, ensure_ascii=False, separators=(",", ":")).encode("utf-8")
    ).decode("ascii")
    remote_source = REMOTE_SCRIPT.replace("__SNAPSHOT_B64__", encoded_snapshot)
    completed = _run(
        ["ssh", "-o", "BatchMode=yes", "-o", "ConnectTimeout=10", host, "python3", "-"],
        cwd=REPO_ROOT,
        timeout=90,
        input_text=remote_source,
    )
    try:
        actual = json.loads(completed.stdout, object_pairs_hook=_pairs_without_duplicates)
    except json.JSONDecodeError as error:
        raise ProbeError(f"remote probe returned invalid JSON: {completed.stdout!r}") from error
    return verify_remote_actual(snapshot, actual)


def build_parser() -> argparse.ArgumentParser:
    parser = JsonArgumentParser(description=__doc__)
    commands = parser.add_subparsers(dest="command", required=True)
    snapshot_parser = commands.add_parser("snapshot", help="compile selected cards into a source-bound snapshot")
    snapshot_parser.add_argument("--cards", required=True, type=parse_card_ids)
    snapshot_parser.add_argument("--output", required=True, type=Path)
    live_parser = commands.add_parser("live", help="verify a deployed release through read-only SignalR calls")
    live_parser.add_argument("--snapshot", required=True, type=Path)
    live_parser.add_argument("--host", default="root@cynthia.ovyno.com")
    return parser


def main(argv: list[str] | None = None) -> int:
    args = build_parser().parse_args(argv)
    try:
        if args.command == "snapshot":
            result = snapshot_command(args.cards, args.output)
        else:
            result = live_command(args.snapshot, args.host)
        print(json.dumps(result, ensure_ascii=False, separators=(",", ":")))
        return 0
    except (ProbeError, OSError) as error:
        print(json.dumps({"passed": False, "error": str(error)}, ensure_ascii=False, separators=(",", ":")), file=sys.stderr)
        return 1


if __name__ == "__main__":
    raise SystemExit(main())
