"""Validate every configured language, format argument and serialized UI binding.

Run with Python 3; no third-party packages or running server required.
"""
import json
import re
from pathlib import Path

ROOT = Path(__file__).resolve().parents[1]
CLIENT = ROOT / "src/Cynthia.Card.Unity/src/Cynthia.Unity.Card"
PACKS = [CLIENT / "Assets/Resources/Locales", CLIENT / "Assets/StreamingFile/Locales",
         ROOT / "src/Cynthia.Card/src/Cynthia.Card.Server/Locales"]


def unique_object(pairs):
    result = {}
    for key, value in pairs:
        assert key not in result, f"Duplicate JSON key: {key}"
        result[key] = value
    return result


def read(path):
    return json.loads(path.read_text(encoding="utf-8-sig"), object_pairs_hook=unique_object)


def placeholders(text):
    return sorted(re.findall(r"(?<!\{)\{(\d+)(?:,[^}:]+)?(?::[^}]+)?\}(?!\})", text))


def bound_keys(source):
    for call in re.finditer(r"LocalizedLabel\.Set\(", source):
        depth = 1
        tail = source[call.end():]
        for token in re.finditer(r'"(?:\\.|[^"\\])*"|[(),]', tail):
            if token[0] == "(":
                depth += 1
            elif token[0] == ")":
                depth -= 1
            elif token[0] == "," and depth == 1:
                key = re.match(r'\s*"([^"]+)"', tail[token.end():])
                if key:
                    yield key[1]
                break


def main():
    checks = []
    languages = [x["Filename"] for x in read(PACKS[0] / "config.json")]
    for folder in PACKS:
        packs = {lang: read(folder / f"{lang}.json") for lang in languages}
        for section in ["MenuLocales", "CardLocales"]:
            keys = set().union(*(set(pack[section]) for pack in packs.values()))
            for lang, pack in packs.items():
                assert keys == set(pack[section]), (folder, lang, section, keys - set(pack[section]))
                checks.append(f"{folder.relative_to(ROOT)} {lang} {section}: {len(keys)} keys")
        for lang, pack in packs.items():
            for key, value in pack["MenuLocales"].items():
                assert placeholders(value) == placeholders(packs["en"]["MenuLocales"][key]), (lang, key, "format arguments")
                assert value.strip() or key.endswith(("Description", "_Description")), (lang, key, "empty UI text")
            for card, text in pack["CardLocales"].items():
                assert text["Name"].strip() and text["Info"].strip(), (lang, card, "empty card content")
    # Server card balance descriptions may intentionally differ from bundled data;
    # require exact parity for UI text, while checking card completeness per source.
    for lang in languages:
        menus = [read(folder / f"{lang}.json")["MenuLocales"] for folder in PACKS]
        assert menus[0] == menus[1] == menus[2], (lang, "UI language-pack copies differ")
    menu_keys = set(read(PACKS[0] / "en.json")["MenuLocales"])
    assets = CLIENT / "Assets"
    for folder in [assets / "Resources/Scenes", assets / "Resources/Prefab"]:
        for path in folder.rglob("*"):
            if path.suffix not in [".unity", ".prefab"]:
                continue
            source = path.read_text(encoding="utf-8-sig")
            for key in re.findall(r"  - Id: (.+)", source):
                assert key in menu_keys, (path, key, "missing serialized UI translation")
    runtime = list(assets.glob("*.cs"))
    for folder in [assets / "Script", assets / "Code", assets / "DynamicCards/Runtime"]:
        runtime.extend(folder.rglob("*.cs"))
    for path in runtime:
        source = path.read_text(encoding="utf-8-sig")
        keys = re.findall(r'(?:GetText|LocalizedLabel.Get)\("([^"]+)"', source)
        keys += list(bound_keys(source))
        for key in keys:
            assert key in menu_keys, (path, key, "missing runtime translation")
    checks += ["All format arguments match English", "All serialized and literal runtime keys exist",
               "All 3 sets of UI language packs agree"]
    print(json.dumps({"passed": True, "languages": languages, "checks": checks}, ensure_ascii=False, indent=2))


if __name__ == "__main__":
    main()
