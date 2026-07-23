from __future__ import annotations

import json
import sys
import tempfile
import unittest
from pathlib import Path

TOOLS_DIR = Path(__file__).resolve().parents[1]
if str(TOOLS_DIR) not in sys.path:
    sys.path.insert(0, str(TOOLS_DIR))

import mk2_room_manifest_gate


class Mk2RoomManifestGateTests(unittest.TestCase):
    def setUp(self) -> None:
        self.repo_root = Path(__file__).resolve().parents[2]
        source = self.repo_root / "docs/project_art_plan/mk2_room_socket_manifest.json"
        self.manifest = json.loads(source.read_text(encoding="utf-8"))
        self.temp_dir = tempfile.TemporaryDirectory()
        self.path = Path(self.temp_dir.name) / "manifest.json"
        self._write()

    def tearDown(self) -> None:
        self.temp_dir.cleanup()

    def _write(self) -> None:
        self.path.write_text(json.dumps(self.manifest), encoding="utf-8")

    def _codes(self) -> set[str]:
        result = mk2_room_manifest_gate.validate_manifest(self.repo_root, self.path)
        return {finding.code for finding in result.findings}

    def test_repository_manifest_passes(self) -> None:
        result = mk2_room_manifest_gate.validate_manifest(self.repo_root, self.path)
        self.assertEqual((), result.findings)
        self.assertEqual(7, result.room_count)
        self.assertGreater(result.socket_count, 50)

    def test_room_order_cannot_drift(self) -> None:
        rooms = self.manifest["rooms"]
        rooms[1], rooms[2] = rooms[2], rooms[1]
        self._write()
        codes = self._codes()
        self.assertIn("ROOM_ORDER_INVALID", codes)
        self.assertIn("ROOM_SEQUENCE_INVALID", codes)

    def test_bedroom_cannot_disappear(self) -> None:
        self.manifest["rooms"] = [
            room for room in self.manifest["rooms"] if room["id"] != "sleeping_quarters"
        ]
        self._write()
        codes = self._codes()
        self.assertIn("ROOM_ORDER_INVALID", codes)
        self.assertIn("ROOM_COUNT_INVALID", codes)

    def test_bedroom_requires_four_bunks(self) -> None:
        room = next(room for room in self.manifest["rooms"] if room["id"] == "sleeping_quarters")
        room["requiredSockets"].remove("bunk_04")
        self._write()
        self.assertIn("FOUR_BUNKS_REQUIRED", self._codes())

    def test_gameplay_socket_stability_is_locked(self) -> None:
        self.manifest["skinContract"]["gameplaySocketsRemainStable"] = False
        self._write()
        self.assertIn("SKIN_INVARIANT_REQUIRED", self._codes())

    def test_fixed_identity_cannot_become_skinnable(self) -> None:
        room = next(room for room in self.manifest["rooms"] if room["id"] == "cockpit")
        room["skinnableZones"].append("oil_lantern")
        self._write()
        self.assertIn("SKIN_ZONE_OVERLAP", self._codes())

    def test_child_reach_cap_is_enforced(self) -> None:
        room = next(room for room in self.manifest["rooms"] if room["id"] == "cockpit")
        room["essentialControlReachMeters"]["max"] = 1.25
        self._write()
        self.assertIn("CHILD_REACH_INVALID", self._codes())

    def test_drive_core_keeps_key_and_coupler_sockets(self) -> None:
        room = next(room for room in self.manifest["rooms"] if room["id"] == "drive_core")
        room["requiredSockets"].remove("artifact_key_joined")
        self._write()
        self.assertIn("CANON_SOCKET_MISSING", self._codes())

    def test_keeper_path_must_exist(self) -> None:
        room = next(room for room in self.manifest["rooms"] if room["id"] == "cockpit")
        room["conceptPaths"] = ["concepts/ship_mk2_architect/missing.png"]
        self._write()
        self.assertIn("CONCEPT_MISSING", self._codes())


if __name__ == "__main__":
    unittest.main()
