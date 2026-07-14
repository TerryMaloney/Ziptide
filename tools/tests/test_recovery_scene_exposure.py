from __future__ import annotations

import unittest

from recovery_scene_exposure import build_report, classify_scene, parse_build_settings


class RecoverySceneExposureTests(unittest.TestCase):
    def test_parses_enabled_and_disabled_scenes(self) -> None:
        text = """
  m_Scenes:
  - enabled: 1
    path: Assets/Ziptide/Scenes/_Boot.unity
    guid: a
  - enabled: 0
    path: Assets/Ziptide/Scenes/StarterWorld.unity
    guid: b
"""
        entries = parse_build_settings(text)
        self.assertEqual(2, len(entries))
        self.assertTrue(entries[0].enabled)
        self.assertFalse(entries[1].enabled)
        self.assertEqual("GOLDEN_PATH_SUPPORT", entries[0].exposure)

    def test_classifies_prototype_and_legacy_scenes(self) -> None:
        self.assertEqual(
            "PROTOTYPE_MULTIPLAYER_EXPOSED",
            classify_scene("Assets/Ziptide/Scenes/Arenas/Arena_Cistern.unity")[0],
        )
        self.assertEqual(
            "PROTOTYPE_WORLD_EXPOSED",
            classify_scene("Assets/Ziptide/Scenes/Generated/W009_Chitinwall.unity")[0],
        )
        self.assertEqual(
            "LEGACY_TEST_EXPOSED",
            classify_scene("Assets/Ziptide/Scenes/SandboxTestLab.unity")[0],
        )

    def test_report_counts_hidden_enabled_scenes(self) -> None:
        text = """
  - enabled: 1
    path: Assets/Ziptide/Scenes/_Boot.unity
  - enabled: 1
    path: Assets/Ziptide/Scenes/SandboxTestLab.unity
  - enabled: 1
    path: Assets/Ziptide/Scenes/Arenas/Arena_Cistern.unity
"""
        report = build_report(parse_build_settings(text))
        self.assertEqual(3, report["enabledCount"])
        self.assertEqual(2, report["hiddenButEnabledCount"])


if __name__ == "__main__":
    unittest.main()
