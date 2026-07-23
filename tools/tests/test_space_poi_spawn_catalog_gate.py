from __future__ import annotations

import json
import sys
import tempfile
import unittest
from pathlib import Path

TOOLS_DIR = Path(__file__).resolve().parents[1]
if str(TOOLS_DIR) not in sys.path:
    sys.path.insert(0, str(TOOLS_DIR))

import space_poi_spawn_catalog_gate


class SpacePoiSpawnCatalogGateTests(unittest.TestCase):
    def setUp(self) -> None:
        self.root = Path(__file__).resolve().parents[2]
        self.catalog = json.loads(
            (self.root / "docs/design/space_poi_spawn_catalog.json").read_text(encoding="utf-8")
        )
        self.poi_catalog = self.root / "docs/design/space_poi_catalog.json"
        self.mission_catalog = self.root / "docs/design/space_mission_catalog.json"
        self.temp_dir = tempfile.TemporaryDirectory()
        self.path = Path(self.temp_dir.name) / "spawn.json"
        self._write()

    def tearDown(self) -> None:
        self.temp_dir.cleanup()

    def _write(self) -> None:
        self.path.write_text(json.dumps(self.catalog), encoding="utf-8")

    def _result(self) -> space_poi_spawn_catalog_gate.Result:
        return space_poi_spawn_catalog_gate.validate_catalog(
            self.root, self.path, self.poi_catalog, self.mission_catalog
        )

    def _codes(self) -> set[str]:
        return {finding.code for finding in self._result().findings}

    def _packet(self, family_id: str) -> dict:
        return next(
            packet
            for packet in self.catalog["packets"]
            if packet["poiFamilyId"] == family_id
        )

    def test_repository_catalog_passes(self) -> None:
        result = self._result()
        self.assertEqual((), result.findings)
        self.assertEqual(6, result.packet_count)

    def test_runtime_spawning_stays_unauthorized(self) -> None:
        self.catalog["scope"]["runtimeSpawningAuthorized"] = True
        self._write()
        self.assertIn("SCOPE_LOCK_VIOLATION", self._codes())

    def test_seed_contract_rejects_time_or_network_entropy(self) -> None:
        self.catalog["scope"]["deviceTimeEntropyAllowed"] = True
        self.catalog["scope"]["networkEntropyAllowed"] = True
        self._write()
        self.assertIn("SCOPE_LOCK_VIOLATION", self._codes())

    def test_packet_fields_must_match_source_poi_catalog(self) -> None:
        packet = self._packet("prior_waker_smallcraft")
        packet["budgetClass"] = "P1_micro"
        packet["missionCompatibility"] = ["gate_run"]
        self._write()
        codes = self._codes()
        self.assertIn("SOURCE_FIELD_MISMATCH", codes)
        self.assertIn("SOURCE_LIST_MISMATCH", codes)

    def test_gate_is_one_authored_landmark_only(self) -> None:
        packet = self._packet("ziptide_gate_approach_lane")
        packet["spawnMode"] = "deterministic-sector"
        packet["perSceneCap"] = 2
        packet["seedInputs"] = ["systemSeed", "sectorId", "poiFamilyId", "slotIndex"]
        self._write()
        self.assertIn("GATE_LANDMARK_CONTRACT", self._codes())

    def test_deterministic_packet_requires_exact_seed_inputs(self) -> None:
        self._packet("debris_ribbon")["seedInputs"] = ["deviceTime"]
        self._write()
        self.assertIn("PACKET_SEED_INPUTS_INVALID", self._codes())

    def test_relay_keeps_signal_and_warden_sockets(self) -> None:
        packet = self._packet("orbital_repair_relay")
        packet["requiredSockets"].remove("signal_hook")
        self._write()
        codes = self._codes()
        self.assertIn("SOURCE_LIST_MISMATCH", codes)
        self.assertIn("RELAY_SOCKET_REQUIRED", codes)

    def test_prior_waker_keeps_ordered_lore(self) -> None:
        self._packet("prior_waker_smallcraft")["storyContract"]["orderedThread"] = False
        self._write()
        codes = self._codes()
        self.assertIn("STORY_CONTRACT_MISMATCH", codes)
        self.assertIn("WAKER_ORDER_REQUIRED", codes)

    def test_debris_cannot_overlap_gate_or_traversal(self) -> None:
        packet = self._packet("debris_ribbon")
        packet["spawnExclusions"].remove("traversal_plane")
        self._write()
        self.assertIn("DEBRIS_EXCLUSION_REQUIRED", self._codes())

    def test_unreviewed_packet_requires_amendment(self) -> None:
        extra = json.loads(json.dumps(self.catalog["packets"][0]))
        extra["id"] = "spawn_random_combat_arena_v1"
        extra["poiFamilyId"] = "random_combat_arena"
        self.catalog["packets"].append(extra)
        self._write()
        codes = self._codes()
        self.assertIn("SOURCE_POI_MISSING", codes)
        self.assertIn("UNREVIEWED_PACKETS_PRESENT", codes)
        self.assertIn("PACKET_COUNT_INVALID", codes)


if __name__ == "__main__":
    unittest.main()
