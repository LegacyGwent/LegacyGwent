import importlib.util
from pathlib import Path
import unittest

spec = importlib.util.spec_from_file_location('guard', Path(__file__).with_name('check-public-content.py'))
guard = importlib.util.module_from_spec(spec)
spec.loader.exec_module(guard)


class PublicationGuard(unittest.TestCase):
    def test_added_path_reports_location_not_value(self):
        private = 'C:' + '/Users/' + 'ExamplePerson/Documents/log.txt'
        result = guard.inspect_patch('+++ b/docs/example.md\n@@ -0,0 +3 @@\n+' + private)
        self.assertEqual(result, [('docs/example.md', 3, 'personal-profile-path')])
        self.assertNotIn('ExamplePerson', repr(result))

    def test_removed_sensitive_line_does_not_block_cleanup(self):
        private = 'C:' + '/Users/' + 'ExamplePerson/log.txt'
        self.assertEqual(guard.inspect_patch('+++ b/log.txt\n@@ -1 +1 @@\n-' + private + '\n+<repo>/log.txt'), [])

    def test_token_is_detected_without_echo(self):
        token = 'ghp_' + 'x' * 36
        result = guard.inspect_patch('+++ b/config.txt\n@@ -0,0 +1 @@\n+' + token)
        self.assertEqual(result, [('config.txt', 1, 'github-token')])
        self.assertNotIn(token, repr(result))

    def test_repository_paths_and_rule_source_are_safe(self):
        self.assertEqual(guard.inspect_patch('+++ b/docs/a.md\n@@ -0,0 +1 @@\n+<repo>/Assets/Card.png'), [])
        source = Path(guard.__file__).read_text(encoding='utf-8')
        patch = '+++ b/guard.py\n@@ -0,0 +1 @@\n' + '\n'.join('+' + line for line in source.splitlines())
        self.assertEqual(guard.inspect_patch(patch), [])


if __name__ == '__main__':
    unittest.main()
