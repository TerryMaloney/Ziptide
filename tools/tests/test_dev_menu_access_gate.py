from __future__ import annotations

import re
import unittest
from pathlib import Path


class DevMenuAccessContractTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls) -> None:
        cls.root = Path(__file__).resolve().parents[2]
        devtools = cls.root / "Ziptide/Assets/Ziptide/Gameplay/Runtime/DevTools"
        cls.menu = (devtools / "DevMenu.cs").read_text(encoding="utf-8")
        cls.board = (devtools / "DevWarpBoard.cs").read_text(encoding="utf-8")
        cls.gesture = (devtools / "DevMenuGesture.cs").read_text(encoding="utf-8")
        cls.gate = (devtools / "DevAccessGate.cs").read_text(encoding="utf-8")
        cls.script = (cls.root / "tools/dev_menu_access.ps1").read_text(encoding="utf-8")
        cls.controls = (cls.root / "docs/design/CONTROL_SCHEME.md").read_text(encoding="utf-8")

    def test_dev_access_reads_pose_but_no_gameplay_buttons(self) -> None:
        combined = self.board + "\n" + self.menu + "\n" + self.gesture
        forbidden = (
            "secondaryButton",
            "primaryButton",
            "primary2DAxisClick",
            "triggerButton",
            "gripButton",
            "menuButton",
        )
        for token in forbidden:
            self.assertNotIn(token, combined, f"Dev access must reserve zero gameplay buttons: {token}")

        self.assertIn("CommonUsages.devicePosition", self.gesture)
        self.assertNotIn("CommonUsages.deviceRotation", self.gesture)

    def test_editor_headset_and_backup_paths_are_explicit(self) -> None:
        # DS-02 transferred all summon ownership from the retired TMP DevMenu to the
        # device-proven primitive DevWarpBoard. The gate follows the authority, not the old class name.
        self.assertIn("f2Key.wasPressedThisFrame", self.board)
        self.assertIn("_gesture.Tick(Time.unscaledDeltaTime)", self.board)
        self.assertIn("DevAccessGate.TryConsumeOpenRequest()", self.board)
        self.assertIn("#if UNITY_EDITOR || DEVELOPMENT_BUILD", self.board)
        self.assertIn("#if UNITY_EDITOR || DEVELOPMENT_BUILD", self.gesture)
        self.assertIn("#if UNITY_EDITOR || DEVELOPMENT_BUILD", self.gate)

        self.assertIn("RETIRED FROM RUNTIME", self.menu)
        self.assertNotIn("f2Key.wasPressedThisFrame", self.menu)
        self.assertNotIn("RuntimeInitializeOnLoadMethod", self.menu.replace(
            "Do not re-add a RuntimeInitializeOnLoadMethod here.", ""
        ))

    def test_gesture_requires_hold_and_release_latch(self) -> None:
        self.assertIn("HoldSeconds = 2f", self.gesture)
        self.assertIn("if (_latched) return false", self.gesture)
        self.assertIn("_latched = false", self.gesture)
        self.assertIn("IsSummonPose", self.gesture)

    def test_marker_names_match_script(self) -> None:
        access = re.search(r'AccessMarkerName\s*=\s*"([^"]+)"', self.gate)
        open_request = re.search(r'OpenMarkerName\s*=\s*"([^"]+)"', self.gate)
        self.assertIsNotNone(access)
        self.assertIsNotNone(open_request)
        self.assertIn(access.group(1), self.script)
        self.assertIn(open_request.group(1), self.script)

    def test_script_supports_optional_backup_lifecycle(self) -> None:
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
