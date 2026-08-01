#!/usr/bin/env python3
"""Fail CI when a dotnet package audit reports known vulnerabilities."""

from __future__ import annotations

import json
import pathlib
import sys
from typing import Any


def iter_vulnerable_packages(report: dict[str, Any]):
    for project in report.get("projects", []):
        project_path = project.get("path", "<unknown project>")
        for framework in project.get("frameworks", []):
            framework_name = framework.get("framework", "<unknown framework>")
            for package_group in ("topLevelPackages", "transitivePackages"):
                for package in framework.get(package_group, []):
                    vulnerabilities = package.get("vulnerabilities") or []
                    if vulnerabilities:
                        yield project_path, framework_name, package, vulnerabilities


def main() -> int:
    if len(sys.argv) != 2:
        print("usage: check-vulnerable-packages.py <dotnet-list-json>", file=sys.stderr)
        return 2

    report_path = pathlib.Path(sys.argv[1])
    with report_path.open("r", encoding="utf-8-sig") as report_file:
        report = json.load(report_file)

    findings = list(iter_vulnerable_packages(report))
    if not findings:
        print("Dependency audit passed: no known vulnerable packages.")
        return 0

    print("Dependency audit failed:", file=sys.stderr)
    for project_path, framework_name, package, vulnerabilities in findings:
        package_id = package.get("id", "<unknown package>")
        version = package.get("resolvedVersion", "<unknown version>")
        for vulnerability in vulnerabilities:
            severity = vulnerability.get("severity", "unknown")
            advisory = vulnerability.get("advisoryurl", vulnerability.get("advisoryUrl", ""))
            print(
                f"- {project_path} [{framework_name}] {package_id} {version}: "
                f"{severity} {advisory}".rstrip(),
                file=sys.stderr,
            )
    return 1


if __name__ == "__main__":
    raise SystemExit(main())
