"""Read actual two-player battle captures; never infer animation from IsPremium alone."""
import json, re, sys
from pathlib import Path
work = Path(__file__).resolve().parents[1] / "work" / "PremiumNetwork"
if len(sys.argv) > 1:
    if not re.fullmatch(r"[A-Za-z0-9_-]+", sys.argv[1]):
        raise ValueError("Run id must contain only letters, digits, underscore or hyphen")
    work = work / "runs" / sys.argv[1]
checks, details = [], []
def check(ok, label):
    checks.append({"passed": bool(ok), "label": label})
for round_no in (1, 2):
    fixtures = {s: json.loads((work / f"round{round_no}-{s}" / "prepared.json").read_text(encoding="utf-8-sig")) for s in ("a", "b")}
    for side in ("a", "b"):
        folder = work / f"round{round_no}-{side}"
        frames = json.loads((folder / "samples.json").read_text(encoding="utf-8-sig"))
        other = "b" if side == "a" else "a"
        prefix = f"round {round_no} / viewer {side}"
        for zone, owner in (("MyLeader", side), ("EnemyLeader", other)):
            expected = fixtures[owner]["deck"]["Leader"] in fixtures[owner]["account"]["SelectedCards"]
            entries = [c for f in frames for c in f["cards"] if f"/{zone}/" in c["path"] and c["view"] and c["view"]["active"]]
            hashes = {c["view"]["hash"] for c in entries if c["view"]["hash"]}
            check(bool(entries) and all(c["premium"] == expected for c in entries), f"{prefix}: {zone} receives owner's selected version")
            if expected:
                check(len(hashes) >= 3, f"{prefix}: {zone} rendered animation changes across frames")
            else:
                check(bool(entries) and all(not c["view"]["model"] and not c["view"]["allowed"] and not c["view"]["hash"] for c in entries),
                      f"{prefix}: {zone} has no animated model or render texture output")
            details.append({"viewer": side, "round": round_no, "zone": zone, "expectedPremium": expected,
                            "samples": len(entries), "distinctRenderedFrames": len(hashes)})
        for zone, owner in (("MyRow", side), ("EnemyRow", other)):
            entries = [c for f in frames for c in f["cards"] if re.search(r"/"+zone+r"[123]/", c["path"]) and c["view"] and c["view"]["active"] and not c["back"]]
            selected = fixtures[owner]["account"]["SelectedCards"]
            known = fixtures[owner]["deck"]["Deck"]
            entries = [c for c in entries if c["card"] in known]
            # Fixture deck's Cantarella (33004) is CardUseInfo.EnemyRow in GwentMap.cs.
            # Its original player's selection survives playing into the opponent's rows.
            opposite = "b" if owner == "a" else "a"
            def expected(c):
                source = opposite if c["card"] == "33004" else owner
                return c["card"] in fixtures[source]["account"]["SelectedCards"]
            check(bool(entries) and all(c["premium"] == expected(c) for c in entries), f"{prefix}: {zone} cards preserve original player's version after network play")
            spies = [c for c in entries if c["card"] == "33004"]
            if spies:
                # CardShowInfo.Reverse swaps the portrait at the 0.15s midpoint.
                # Validate binding after that face is actually displayed, while the
                # owner-state check above also covers the reveal transition itself.
                revealed = [c for c in spies if c["view"]["sprite"] == "16221000"]
                check(bool(revealed) and all(c["premium"] == expected(c) and c["view"]["allowed"] == expected(c) for c in revealed), f"{prefix}: {zone} revealed spy keeps the original player's version")
            premium = [c for c in entries if c["premium"]]
            standard = [c for c in entries if not c["premium"]]
            animated = {}
            for c in premium:
                if c["view"]["hash"]: animated.setdefault(c["card"], set()).add(c["view"]["hash"])
            check(any(len(v) >= 3 for v in animated.values()), f"{prefix}: {zone} includes a visibly animated premium unit")
            check(bool(standard) and all(not c["view"]["allowed"] and not c["view"]["model"] for c in standard), f"{prefix}: {zone} includes a static standard unit")
            details.append({"viewer": side, "round": round_no, "zone": zone, "cards": sorted(set(c["card"] for c in entries)),
                            "animatedCards": {k: len(v) for k,v in animated.items()}, "standardSamples": len(standard)})
        hidden = [c for f in frames for c in f["cards"] if "/EnemyHand/" in c["path"] and c["back"] and c["view"]]
        check(bool(hidden) and all(c["premium"] is not True and not c["view"]["model"] for c in hidden), f"{prefix}: concealed opponent hand does not reveal premiums")
result = {"passed": all(c["passed"] for c in checks), "count": len(checks), "checks": checks, "details": details}
(work / "verification.json").write_text(json.dumps(result, ensure_ascii=False, indent=2), encoding="utf-8")
print(json.dumps(result, ensure_ascii=False, indent=2))
raise SystemExit(0 if result["passed"] else 1)
