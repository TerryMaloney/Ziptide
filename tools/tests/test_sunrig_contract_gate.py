from __future__ import annotations

import json
import sys
import tempfile
import unittest
from pathlib import Path

TOOLS_DIR = Path(__file__).resolve().parents[1]
if str(TOOLS_DIR) not in sys.path:
    sys.path.insert(0, str(TOOLS_DIR))

import sunrig_contract_gate


class SunRigContractGateTests(unittest.TestCase):
    def setUp(self) -> None:
        root = Path(__file__).resolve().parents[2]
        self.catalog = json.loads(
            (root / "docs/design/celestial_system_catalog.json").read_text(encoding="utf-8")
        )
        self.temp_dir = tempfile.TemporaryDirectory()
        self.path = Path(self.temp_dir.name) / "catalog.json"
        self._write()

    def tearDown(self) -> None:
        self.temp_dir.cleanup()

    def _write(self) -> None:
        self.path.write_text(json.dumps(self.catalog), encoding="utf-8")

    def _result(self) -> sunrig_contract_gate.Result:
        return sunrig_contract_gate.validate_catalog(self.path)

    def _codes(self, severity: str | None = None) -> set[str]:
        findings = self._result().findings
        if severity is not None:
            findings = tuple(item for item in findings if item.severity == severity)
        return {item.code for item in findings}

    def _primary(self) -> dict:
        return self.catalog["systems"][0]["stars"][0]

    def test_repository_catalog_has_no_structural_blockers(self) -> None:
        result = self._result()
        self.assertEqual((), result.blockers)
        self.assertGreater(len(result.warnings), 0)
        self.assertIn("AUTHOR_FIELD_UNRESOLVED", self._codes("warning"))
        self.assertIn("STAR_FIELD_NOT_YET_MIGRATED", self._codes("warning"))

    def test_system_requires_exactly_one_primary_star(self) -> None:
        self._primary()["countRole"] = "secondary"
        self._primary()["lightRole"] = "SunRig-secondary-fill"
        self._write()
        self.assertIn("PRIMARY_STAR_COUNT_INVALID", self._codes("blocker"))

    def test_system_allows_at_most_two_stars(self) -> None:
        base = self._primary()
        for index in range(2):
            extra = dict(base)
            extra["id"] = f"extra_{index}"
            extra["countRole"] = "secondary"
            extra["lightRole"] = "SunRig-secondary-fill"
            self.catalog["systems"][0]["stars"].append(extra)
        self._write()
        self.assertIn("STAR_COUNT_INVALID", self._codes("blocker"))

    def test_primary_light_and_disc_roles_are_locked(self) -> None:
        self._primary()["lightRole"] = "painted-sky-disc"
        self._primary()["discRole"] = "space-only"
        self._write()
        codes = self._codes("blocker")
        self.assertIn("PRIMARY_LIGHT_ROLE_INVALID", codes)
        self.assertIn("DISC_ROLE_INVALID", codes)

    def test_ground_and_space_must_reference_same_primary_star(self) -> None:
        self.catalog["spaceViews"][0]["primaryStar"] = "wrong_star"
        self._write()
        codes = self._codes("blocker")
        self.assertIn("SPACE_VIEW_PRIMARY_STAR_MISSING", codes)
        self.assertIn("GROUND_SPACE_STAR_MISMATCH", codes)

    def test_sun_bearing_parity_is_required(self) -> None:
        self.catalog["spaceViews"][0]["sunBearingMatchesGround"] = False
        self._write()
        self.assertIn("SUN_BEARING_PARITY_REQUIRED", self._codes("blocker"))

    def test_resolved_contract_can_be_warning_free(self) -> None:
        star = self._primary()
        star.update(
            {
                "exactColor": [1.0, 0.82, 0.62],
                "exactDiscAngularSize": 0.53,
                "direction": {"bearingDegrees": 250.0, "elevationDegrees": 8.0},
                "keyIntensity": 1.0,
                "flareProfile": "subtle-primary-v1",
                "groundHorizonWarm": 0.65,
            }
        )
        self.catalog["worldPlacements"][0]["sunDirection"] = {
            "bearingDegrees": 250.0,
            "elevationDegrees": 8.0,
        }
        self._write()
        result = self._result()
        self.assertEqual((), result.blockers)
        self.assertEqual((), result.warnings)
        self.assertEqual("pass", result.status)

    def test_missing_direction_without_world_fallback_blocks(self) -> None:
        self.catalog["worldPlacements"][0]["primaryStar"] = "missing_star"
        self._write()
        codes = self._codes("blocker")
        self.assertIn("STAR_DIRECTION_MISSING", codes)
        self.assertIn("PLACEMENT_PRIMARY_STAR_MISSING", codes)


if __name__ == "__main__":
    unittest.main()
