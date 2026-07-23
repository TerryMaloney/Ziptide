from __future__ import annotations

import json
import sys
import tempfile
import unittest
from pathlib import Path

TOOLS_DIR = Path(__file__).resolve().parents[1]
if str(TOOLS_DIR) not in sys.path:
    sys.path.insert(0, str(TOOLS_DIR))

import celestial_system_gate


class CelestialSystemGateTests(unittest.TestCase):
    def setUp(self) -> None:
        repo_root = Path(__file__).resolve().parents[2]
        source = repo_root / "docs/design/celestial_system_catalog.json"
        self.catalog = json.loads(source.read_text(encoding="utf-8"))
        self.temp_dir = tempfile.TemporaryDirectory()
        self.path = Path(self.temp_dir.name) / "catalog.json"
        self._write()

    def tearDown(self) -> None:
        self.temp_dir.cleanup()

    def _write(self) -> None:
        self.path.write_text(json.dumps(self.catalog), encoding="utf-8")

    def _codes(self) -> set[str]:
        result = celestial_system_gate.validate_catalog(self.path)
        return {finding.code for finding in result.findings}

    def test_repository_catalog_passes(self) -> None:
        result = celestial_system_gate.validate_catalog(self.path)
        self.assertEqual((), result.findings)
        self.assertEqual(1, result.system_count)
        self.assertEqual(1, result.world_count)
        self.assertEqual(1, result.space_view_count)

    def test_world_cannot_reference_missing_system(self) -> None:
        self.catalog["worldPlacements"][0]["systemId"] = "missing"
        self._write()
        self.assertIn("WORLD_SYSTEM_MISSING", self._codes())

    def test_ground_view_must_be_derived(self) -> None:
        self.catalog["worldPlacements"][0]["groundVistaMode"] = "hand-painted-independent"
        self._write()
        self.assertIn("GROUND_VIEW_DERIVATION_REQUIRED", self._codes())

    def test_space_view_must_share_system_background(self) -> None:
        self.catalog["spaceViews"][0]["backgroundId"] = "different_starfield"
        self._write()
        self.assertIn("BACKGROUND_MISMATCH", self._codes())

    def test_moss_remains_moon_of_ringed_giant(self) -> None:
        self.catalog["worldPlacements"][0]["parentBody"] = "moss_sibling_grey_moon"
        self._write()
        self.assertIn("MOSS_PARENT_DRIFT", self._codes())

    def test_moss_retains_single_warm_star(self) -> None:
        star = dict(self.catalog["systems"][0]["stars"][0])
        star["id"] = "second_star"
        self.catalog["systems"][0]["stars"].append(star)
        self._write()
        self.assertIn("MOSS_SINGLE_STAR_REQUIRED", self._codes())

    def test_body_parent_must_exist(self) -> None:
        body = next(
            body
            for body in self.catalog["systems"][0]["bodies"]
            if body["id"] == "moss_world_body"
        )
        body["parentBody"] = "missing_giant"
        self._write()
        self.assertIn("BODY_PARENT_MISSING", self._codes())

    def test_shell_grid_intensity_stays_in_range(self) -> None:
        self.catalog["worldPlacements"][0]["shellGridIntensity"] = 1.5
        self._write()
        self.assertIn("SHELL_GRID_RANGE", self._codes())

    def test_space_object_palette_is_required(self) -> None:
        del self.catalog["systems"][0]["spaceObjectPalette"]
        self._write()
        self.assertIn("OBJECT_PALETTE_REQUIRED", self._codes())


if __name__ == "__main__":
    unittest.main()
