from __future__ import annotations

import re
import unittest
from pathlib import Path


class DevMenuAccessContractTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls) -> None:
        cls.root = Path(__file__).resolve().parents[2]
        cls.menu = (cls.root / "Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevMenu.cs").read_text(encoding="utf-8")
        cls.gate = (cls.root / "Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools/DevAccessGate.cs").read_text(encoding="utf-8")
        cls.script = (cls.root / "tools/dev_menu_access.ps1").read_text(encoding="utf-8")
        cls.controls = (cls.root / "docs/design/CONTROL_SCHEME.md").read_text(encoding="utf-8")

    def test_dev_menu_has_no_controller_polling(self) -> None:
        forbidden = (
            "InputDevices",
            "XRNode",
            "CommonUsages",
            "secondaryButton",
            "primaryButton",
            "primary2DAxisClick",
        )
        for token in forbidden:
            self.assertNotIn(token, self.menu, f"DevMenu must reserve zero controller inputs: {token}")

    def test_editor_and_quest_access_paths_are_explicit(self) -> None:
        self.assertIn("f2Key.wasPressedThisFrame", self.menu)
        self.assertIn("DevAccessGate.TryConsumeOpenRequest()", self.menu)
        self.assertIn("#if UNITY_EDITOR || DEVELOPMENT_BUILD", self.menu)
        self.assertIn("#if UNITY_EDITOR || DEVELOPMENT_BUILD", self.gate)

    def test_marker_names_match_script(self) -> None:
        access = re.search(r'AccessMarkerName\s*=\s*"([^"]+)"', self.gate)
        open_request = re.search(r'OpenMarkerName\s*=\s*"([^"]+)"', self.gate)
        self.assertIsNotNone(access)
        self.assertIsNotNone(open_request)
        self.assertIn(access.group(1), self.script)
        self.assertIn(open_request.group(1), self.script)

    def test_script_supports_full_access_lifecycle(self) -> None:
        for action in ("Open", "Unlock", "Lock", "Status"):
            self.assertIn(f'"{action}"', self.script)
        self.assertIn("adb", self.script.lower())
        self.assertIn("android.intent.category.LAUNCHER", self.script)

    def test_control_scheme_keeps_gameplay_buttons_owned(self) -> None:
        self.assertIn("Quick-swap | B", self.controls)
        self.assertIn("Y reserved for the future player menu", self.controls)
        self.assertIn("consumes zero controller buttons", self.controls)


if __name__ == "__main__":
    unittest.main()
