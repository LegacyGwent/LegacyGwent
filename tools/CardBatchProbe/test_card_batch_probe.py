import argparse
import contextlib
import importlib.util
import io
import json
from pathlib import Path
import tempfile
import unittest


SCRIPT = Path(__file__).resolve().parents[2] / "skills" / "legacy-gwent-maintainer" / "scripts" / "card-batch-probe.py"
SPEC = importlib.util.spec_from_file_location("card_batch_probe", SCRIPT)
probe = importlib.util.module_from_spec(SPEC)
assert SPEC.loader is not None
SPEC.loader.exec_module(probe)


def card(card_id="64004", info="说明"):
    return {field: (card_id if field == "CardId" else info if field == "Info" else None) for field in probe.PROPERTIES}


def snapshot(dirty=False):
    return {
        "schemaVersion": 1,
        "source": {"headSha": "a" * 40, "dirty": dirty},
        "version": "1.0.0.198",
        "properties": list(probe.PROPERTIES),
        "cards": {"64004": card()},
    }


class CardBatchProbeTests(unittest.TestCase):
    def test_card_arguments_reject_bad_and_duplicate_values(self):
        for value in ("", "64004,", "64004,abc", "64004,64004"):
            with self.subTest(value=value), self.assertRaises(argparse.ArgumentTypeError):
                probe.parse_card_ids(value)

    def test_cli_argument_failure_is_compact_json(self):
        stderr = io.StringIO()
        with contextlib.redirect_stderr(stderr), self.assertRaises(SystemExit):
            probe.build_parser().parse_args(["snapshot", "--cards", "64004,64004", "--output", "ignored.json"])
        self.assertEqual(
            json.loads(stderr.getvalue()),
            {"passed": False, "error": "argument --cards: duplicate card IDs: 64004"},
        )

    def test_compiled_payload_rejects_unknown_or_omitted_card(self):
        compiled = {"version": "1.0.0.198", "properties": list(probe.PROPERTIES), "cards": {"64004": card()}}
        with self.assertRaisesRegex(probe.ProbeError, "missing=.*70178"):
            probe._validate_compiled_cards(["64004", "70178"], compiled)

    def test_snapshot_reader_rejects_duplicate_keys(self):
        with tempfile.TemporaryDirectory() as directory:
            path = Path(directory) / "snapshot.json"
            path.write_text('{"schemaVersion":1,"schemaVersion":1}', encoding="utf-8")
            with self.assertRaisesRegex(probe.ProbeError, "duplicate JSON key"):
                probe._read_json(path)

    def test_snapshot_validation_rejects_malformed_shapes(self):
        malformed = snapshot()
        malformed["properties"] = malformed["properties"][:-1]
        with self.assertRaisesRegex(probe.ProbeError, "required 14 fields"):
            probe.validate_snapshot(malformed)

        malformed = snapshot()
        malformed["cards"]["64004"]["CardId"] = "70178"
        with self.assertRaisesRegex(probe.ProbeError, "does not match CardId"):
            probe.validate_snapshot(malformed)

    def test_committed_source_rejects_dirty_or_different_head(self):
        with self.assertRaisesRegex(probe.ProbeError, "dirty worktree"):
            probe.verify_committed_source(snapshot(dirty=True), "a" * 40)
        with self.assertRaisesRegex(probe.ProbeError, "does not match snapshot"):
            probe.verify_committed_source(snapshot(), "b" * 40)

    def test_remote_actual_rejects_release_and_field_mismatches(self):
        expected = snapshot()
        actual = {
            "releaseSha": "b" * 40,
            "version": expected["version"],
            "cards": expected["cards"],
            "chineseInfo": {"64004": "说明"},
        }
        with self.assertRaisesRegex(probe.ProbeError, "active release"):
            probe.verify_remote_actual(expected, actual)

        actual["releaseSha"] = "a" * 40
        actual["cards"] = {"64004": dict(expected["cards"]["64004"], Strength=99)}
        with self.assertRaisesRegex(probe.ProbeError, "64004:Strength"):
            probe.verify_remote_actual(expected, actual)

    def test_remote_actual_returns_compact_success_summary(self):
        expected = snapshot()
        actual = {
            "releaseSha": "a" * 40,
            "version": expected["version"],
            "cards": expected["cards"],
            "chineseInfo": {"64004": "说明"},
        }
        self.assertEqual(
            probe.verify_remote_actual(expected, actual),
            {
                "passed": True,
                "releaseSha": "a" * 40,
                "version": "1.0.0.198",
                "cards": ["64004"],
                "propertiesPerCard": 14,
                "chineseLocales": "PASS",
            },
        )

    def test_embedded_remote_probe_is_valid_python(self):
        compile(probe.REMOTE_SCRIPT.replace("__SNAPSHOT_B64__", "e30="), "<remote-card-probe>", "exec")


if __name__ == "__main__":
    unittest.main()
