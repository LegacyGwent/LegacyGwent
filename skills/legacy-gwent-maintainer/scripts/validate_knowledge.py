#!/usr/bin/env python3
"""Validate the maintainer skill's progressive-disclosure knowledge catalog."""

from __future__ import annotations

import re
import sys
from pathlib import Path


SKILL_ROOT = Path(__file__).resolve().parents[1]
REFERENCE_ROOT = SKILL_ROOT / "references"
INDEX = REFERENCE_ROOT / "index.md"
MAX_LINES = 200
MAX_BYTES = 16 * 1024


def main() -> int:
    errors: list[str] = []
    index_text = INDEX.read_text(encoding="utf-8")
    linked = set(re.findall(r"\]\(([^/)]+\.md)\)", index_text))
    actual = {path.name for path in REFERENCE_ROOT.glob("*.md") if path.name != "index.md"}

    for missing in sorted(linked - actual):
        errors.append(f"index links missing reference: {missing}")
    for unlisted in sorted(actual - linked):
        errors.append(f"reference is not routed by index: {unlisted}")

    for path in sorted(REFERENCE_ROOT.glob("*.md")):
        raw = path.read_bytes()
        text = raw.decode("utf-8")
        line_count = len(text.splitlines())
        if line_count > MAX_LINES:
            errors.append(f"{path.name}: {line_count} lines exceeds {MAX_LINES}")
        if len(raw) > MAX_BYTES:
            errors.append(f"{path.name}: {len(raw)} bytes exceeds {MAX_BYTES}")
        if not re.search(r"^Last verified: \d{4}-\d{2}-\d{2}$", text, re.MULTILINE):
            errors.append(f"{path.name}: missing Last verified date")

    if errors:
        print("Knowledge validation failed:", file=sys.stderr)
        for error in errors:
            print(f"- {error}", file=sys.stderr)
        return 1

    print(f"Knowledge validation passed: {len(actual)} routed references")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
