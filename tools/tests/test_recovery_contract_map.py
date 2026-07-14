from __future__ import annotations

import unittest

from recovery_contract_map import build_maps


class RecoveryContractMapTests(unittest.TestCase):
    def test_groups_findings_by_symbol_and_category(self) -> None:
        scan = {
            "toolVersion": 1,
            "scannedFiles": 2,
            "findingCount": 3,
            "findings": [
                {
                    "category": "bootstrap",
                    "code": "RUNTIME_BOOTSTRAP",
                    "path": "A.cs",
                    "line": 10,
                    "symbol": "Game.A",
                    "snippet": "[RuntimeInitializeOnLoadMethod]",
                },
                {
                    "category": "persistence",
                    "code": "DONT_DESTROY_ON_LOAD",
                    "path": "A.cs",
                    "line": 20,
                    "symbol": "Game.A",
                    "snippet": "DontDestroyOnLoad(gameObject);",
                },
                {
                    "category": "scene_loading",
                    "code": "DIRECT_SCENE_LOAD",
                    "path": "B.cs",
                    "line": 5,
                    "symbol": "Game.B",
                    "snippet": "SceneManager.LoadScene(name);",
                },
            ],
        }
        inventory = {
            "systems": [
                {
                    "id": "A",
                    "exposure": "SUPPORT",
                    "currentProof": ["CORE"],
                    "canonicalCandidate": "OwnerA",
                }
            ]
        }
        result = build_maps(scan, inventory)
        self.assertEqual("Game.A", result["maps"]["bootstrapPersistence"][0]["owner"])
        self.assertEqual(2, result["maps"]["bootstrapPersistence"][0]["evidenceCount"])
        self.assertEqual("Game.B", result["maps"]["sceneLoading"][0]["owner"])

    def test_builds_exposure_and_unresolved_owner_ledgers(self) -> None:
        scan = {"toolVersion": 1, "scannedFiles": 0, "findingCount": 0, "findings": []}
        inventory = {
            "systems": [
                {
                    "id": "BOOT",
                    "exposure": "GOLDEN_PATH",
                    "currentProof": ["CORE", "PATCHED"],
                    "canonicalCandidate": "BootOwner",
                },
                {
                    "id": "MELEE",
                    "exposure": "PROTOTYPE_HIDDEN",
                    "currentProof": ["CORE"],
                    "canonicalCandidate": "Undecided during R0",
                },
            ]
        }
        result = build_maps(scan, inventory)
        self.assertEqual(["BOOT"], result["inventory"]["exposures"]["GOLDEN_PATH"])
        self.assertEqual("MELEE", result["inventory"]["unresolvedCanonicalOwners"][0]["systemId"])
        self.assertEqual(["BOOT", "MELEE"], result["inventory"]["proofs"]["CORE"])

    def test_carries_inventory_validation_findings(self) -> None:
        scan = {"toolVersion": 1, "scannedFiles": 0, "findingCount": 0, "findings": []}
        inventory = {"systems": []}
        validation = {"findings": [{"code": "SOURCE_PATH_MISSING", "system_id": "X"}]}
        result = build_maps(scan, inventory, validation)
        self.assertEqual("SOURCE_PATH_MISSING", result["validationFindings"][0]["code"])


if __name__ == "__main__":
    unittest.main()
