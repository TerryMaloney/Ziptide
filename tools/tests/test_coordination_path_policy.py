from __future__ import annotations

import sys
import tempfile
import unittest
from pathlib import Path

TOOLS_DIR = Path(__file__).resolve().parents[1]
if str(TOOLS_DIR) not in sys.path:
    sys.path.insert(0, str(TOOLS_DIR))

import coordination_path_policy


class CoordinationPathPolicyTests(unittest.TestCase):
    def setUp(self) -> None:
        self.temp_dir = tempfile.TemporaryDirectory()
        self.root = Path(self.temp_dir.name)
        workflows = self.root / ".github/workflows"
        workflows.mkdir(parents=True)
        (workflows / "ci.yml").write_text(
            "on:\n"
            "  push:\n"
            "    paths-ignore:\n"
            "      - docs/CI_VERDICT.md\n"
            "      - docs/recovery/generated/**\n"
            "      - docs/HANDOFF.md\n"
            "  pull_request:\n"
            "jobs:\n"
            "  record:\n"
            "    steps:\n"
            "      - run: |\n"
            "          case \"$path\" in\n"
            "            docs/recovery/generated/*|docs/CI_VERDICT.md|docs/HANDOFF.md)\n"
            "              ;;\n"
            "          esac\n"
            "          echo \"::notice::Only generated evidence, the verdict, or docs/HANDOFF.md advanced; recording the tested SHA is safe.\"\n",
            encoding="utf-8",
        )
        (workflows / "cancel-superseded-ci.yml").write_text(
            "on:\n"
            "  push:\n"
            "    paths-ignore:\n"
            "      - docs/CI_VERDICT.md\n"
            "      - docs/recovery/generated/**\n"
            "      - docs/HANDOFF.md\n"
            "  workflow_dispatch:\n",
            encoding="utf-8",
        )
        (workflows / "fast-preflight.yml").write_text(
            "on:\n"
            "  push:\n"
            "    branches:\n"
            "      - terry-local-wip\n"
            "  pull_request:\n"
            "jobs:\n"
            "  governance:\n"
            "    steps:\n"
            "      - name: Factory governance gate\n"
            "        run: python3 tools/factory_governance_gate.py\n",
            encoding="utf-8",
        )

    def tearDown(self) -> None:
        self.temp_dir.cleanup()

    def test_current_shape_fails_before_apply(self) -> None:
        findings = coordination_path_policy.check_policy(self.root)
        self.assertEqual(5, len(findings))

    def test_apply_makes_policy_pass(self) -> None:
        changed = coordination_path_policy.apply_policy(self.root)
        self.assertEqual(
            {
                ".github/workflows/ci.yml",
                ".github/workflows/cancel-superseded-ci.yml",
                ".github/workflows/fast-preflight.yml",
            },
            set(changed),
        )
        self.assertEqual((), coordination_path_policy.check_policy(self.root))

    def test_apply_is_idempotent(self) -> None:
        coordination_path_policy.apply_policy(self.root)
        self.assertEqual((), coordination_path_policy.apply_policy(self.root))
        self.assertEqual((), coordination_path_policy.check_policy(self.root))

    def test_missing_ci_anchor_is_rejected(self) -> None:
        path = self.root / ".github/workflows/ci.yml"
        path.write_text("name: CI\n", encoding="utf-8")
        with self.assertRaisesRegex(coordination_path_policy.PolicyError, "ci paths-ignore"):
            coordination_path_policy.apply_policy(self.root)

    def test_missing_fast_gate_anchor_is_rejected(self) -> None:
        path = self.root / ".github/workflows/fast-preflight.yml"
        path.write_text(
            "on:\n"
            "  push:\n"
            "    branches:\n"
            "      - terry-local-wip\n"
            "  pull_request:\n",
            encoding="utf-8",
        )
        with self.assertRaisesRegex(coordination_path_policy.PolicyError, "governance step"):
            coordination_path_policy.apply_policy(self.root)


if __name__ == "__main__":
    unittest.main()
