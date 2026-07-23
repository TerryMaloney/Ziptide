import json
import tempfile
import unittest
from copy import deepcopy
from datetime import date
from pathlib import Path

from tools import meta_store_readiness_gate as gate


class MetaStoreReadinessGateTests(unittest.TestCase):
    def setUp(self):
        self.tmp = tempfile.TemporaryDirectory()
        self.root = Path(self.tmp.name)
        (self.root / "docs/store_readiness").mkdir(parents=True)
        (self.root / "docs").mkdir(exist_ok=True)
        (self.root / "docs/VRC_TEST_PLAN.md").write_text("vrc", encoding="utf-8")
        (self.root / "docs/evidence.md").write_text("evidence", encoding="utf-8")
        self.manifest = self.root / "docs/store_readiness/meta_store_release_contract.json"
        self.base = {
            "schemaVersion": 1,
            "contractId": "test-contract",
            "reviewedAgainst": {
                "document": "docs/VRC_TEST_PLAN.md",
                "officialSource": "https://example.invalid",
                "verifiedAt": "2026-07-23"
            },
            "policy": {
                "allowedStatuses": sorted(gate.ALLOWED_STATUSES)
            },
            "releaseCandidate": {
                "status": "hold",
                "reason": "testing",
                "candidateSha": gate.UNKNOWN
            },
            "requirements": [
                {
                    "id": "support",
                    "category": "publishing",
                    "title": "Support URL",
                    "required": True,
                    "owner": "terry",
                    "status": "owner-input-required",
                    "vrcIds": ["Publishing.2"],
                    "url": gate.UNKNOWN,
                    "evidence": [],
                    "nextAction": "Publish it."
                }
            ]
        }

    def tearDown(self):
        self.tmp.cleanup()

    def write(self, payload=None):
        self.manifest.write_text(json.dumps(payload or self.base), encoding="utf-8")

    def codes(self, result):
        return {finding.code for finding in result.findings}

    def test_development_mode_allows_explicit_release_hold(self):
        self.write()
        result = gate.validate(self.root, self.manifest)
        self.assertEqual("hold", result.status)
        self.assertEqual(1, result.release_hold_count)
        self.assertEqual(0, len(result.blockers))
        self.assertIn("RELEASE_REQUIREMENT_OPEN", self.codes(result))

    def test_release_mode_blocks_open_requirements(self):
        self.write()
        result = gate.validate(self.root, self.manifest, require_release_ready=True)
        self.assertEqual("fail", result.status)
        self.assertIn("RELEASE_REQUIREMENT_OPEN", self.codes(result))
        self.assertIn("CANDIDATE_ON_HOLD", self.codes(result))

    def test_false_ready_claim_is_blocked(self):
        payload = deepcopy(self.base)
        payload["releaseCandidate"]["status"] = "ready"
        self.write(payload)
        result = gate.validate(self.root, self.manifest)
        self.assertIn("FALSE_READY_CLAIM", self.codes(result))

    def test_verified_url_must_be_https_and_have_evidence(self):
        payload = deepcopy(self.base)
        item = payload["requirements"][0]
        item.update({"status": "verified", "url": "http://example.com", "verifiedAt": "2026-07-23"})
        self.write(payload)
        result = gate.validate(self.root, self.manifest)
        self.assertIn("VERIFIED_URL_INVALID", self.codes(result))
        self.assertIn("VERIFIED_WITHOUT_EVIDENCE", self.codes(result))

    def test_missing_evidence_path_is_blocked(self):
        payload = deepcopy(self.base)
        payload["requirements"][0]["evidence"] = ["docs/missing.md"]
        self.write(payload)
        result = gate.validate(self.root, self.manifest)
        self.assertIn("EVIDENCE_PATH_MISSING", self.codes(result))

    def test_verified_annual_renewal_must_be_future(self):
        payload = deepcopy(self.base)
        item = payload["requirements"][0]
        item.update({
            "status": "verified",
            "url": "https://example.com/support",
            "verifiedAt": "2026-01-01",
            "evidence": ["docs/evidence.md"],
            "renewal": {"cadence": "annual", "lastCompleted": "2025-01-01", "nextDue": "2026-01-01"}
        })
        payload["releaseCandidate"]["status"] = "ready"
        self.write(payload)
        result = gate.validate(self.root, self.manifest, today=date(2026, 7, 23))
        self.assertIn("RENEWAL_OVERDUE", self.codes(result))

    def test_waiver_requires_reason_and_approver(self):
        payload = deepcopy(self.base)
        payload["requirements"][0]["status"] = "waived"
        self.write(payload)
        result = gate.validate(self.root, self.manifest)
        self.assertIn("WAIVER_INCOMPLETE", self.codes(result))

    def test_fully_verified_contract_passes_release_mode(self):
        payload = deepcopy(self.base)
        item = payload["requirements"][0]
        item.update({
            "status": "verified",
            "url": "https://example.com/support",
            "verifiedAt": "2026-07-23",
            "evidence": ["docs/evidence.md"]
        })
        payload["releaseCandidate"]["status"] = "ready"
        payload["releaseCandidate"]["candidateSha"] = "abc1234"
        self.write(payload)
        result = gate.validate(self.root, self.manifest, require_release_ready=True)
        self.assertEqual("pass", result.status)
        self.assertEqual(0, len(result.blockers))
        self.assertEqual(0, result.release_hold_count)


if __name__ == "__main__":
    unittest.main()
