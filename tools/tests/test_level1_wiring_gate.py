"""Tests for tools/level1_wiring_gate.py — the "is it actually hooked up" gate.

The gate's whole value is that it fails on code that LOOKS finished. These tests prove it does:
a class nobody instantiates, a core nobody calls, a spec that quietly lost an id, and a companion
cue nobody fires must each produce a distinct, named red.
"""

import json
import sys
import tempfile
import unittest
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1]))

import level1_wiring_gate as gate


def _write(path: Path, text: str) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text(text, encoding="utf-8")


class Level1WiringGateTests(unittest.TestCase):
    def setUp(self):
        self._tmp = tempfile.TemporaryDirectory()
        self.root = Path(self._tmp.name)
        self._stage()

    def tearDown(self):
        self._tmp.cleanup()

    def _stage(self):
        """A miniature project where every feature is present, created and driven."""
        assets = self.root / gate.ASSETS
        for feature, src, creator, driver in gate.FEATURES:
            _write(assets / src, f"// {feature} implementation\n")

        # One 'wiring' file that satisfies every creator/driver pattern at once. Real patterns are
        # regexes, so write literal call sites that match them.
        wiring = [
            "AddComponent<RingCourseLightsRuntime>(); RingLampChaseCore.Classify();",
            "AddComponent<SpaceTargetRuntime>(); t.ObservePilot(v);",
            "AddComponent<DriftTumbleRuntime>(); DriftTumbleRuntime.RateFor(v);",
            "SalvageTractorFx.Play(a, b); SpaceCombatCore.SalvageApproach01(a, b);",
            "AtmosphereVeilEffect.Play(leg); AtmosphereVeilCore.Intensity();",
            "EnsureComponent<ReentryArrivalRuntime>(go); ReentryArrivalCore.ShouldPlay(a,b,c);",
            "EnsureComponent<PortholeRuntime>(go); PortholeStarfieldCore.Bake(b,1,1,1,1f);",
            "AddComponent<HelmCompassRuntime>(); flight.TryGetCourseBearing(out var d);",
            "FlatsSiteAuthor.Build(r, k); FlatsSiteAuthor.BreachAzimuthDegrees(rings);",
            "ResonanceTellRuntime.PlayAt(p); ResonanceTellCore.Power(t);",
            "IResonanceSensitive x; y.SetInstrumentPower(1f);",
            "AddComponent<SkiffWaterLockRuntime>(); CanalWaterCore.IsNavigable(p, r, 1f, 1f);",
            "AddComponent<CanalStalkerBehavior>(); CanalStalkerCore.Stage(1, 1f, true, false);",
            "AddComponent<BeaconThreadRuntime>(); ZiptideFlags.ARTIFACT_JOINED;",
            "castOff.ConfigureKeyGate(true, \"\"); bool keyRequired = true;",
            "BuildTheThrow(root, kit); Cube(t, \"MuzzleGantry\", a, b, c, false);",
            'rill.SayById("CATCH_DEAD_RING"); rill.SayById("CATCH_OVERRUN"); rill.SayById("CATCH_THE_FIND");',
        ]
        _write(assets / "Gameplay" / "Wiring.cs", "\n".join(wiring))

        _write(self.root / gate.SPEC,
               json.dumps({"ids": list(gate.REQUIRED_SPEC_IDS)}))

    def _codes(self, result):
        return [f.code for f in result.findings]

    # ── the contract ──────────────────────────────────────────────────────────

    def test_fully_wired_project_passes(self):
        result = gate.run_gate(self.root)
        self.assertEqual("pass", result.status, msg=str(result.findings))
        self.assertEqual(len(gate.FEATURES), result.checked)

    def test_missing_source_is_a_red(self):
        (self.root / gate.ASSETS / gate.FEATURES[0][1]).unlink()
        result = gate.run_gate(self.root)
        self.assertIn("WIRING_SOURCE_MISSING", self._codes(result))
        self.assertEqual("fail", result.status)

    def test_class_nobody_instantiates_is_a_red(self):
        # This is the headline case: the file exists and compiles, and is never added to anything.
        wiring = self.root / gate.ASSETS / "Gameplay" / "Wiring.cs"
        wiring.write_text(wiring.read_text().replace("AddComponent<CanalStalkerBehavior>()", ""),
                          encoding="utf-8")
        result = gate.run_gate(self.root)
        self.assertIn("WIRING_NEVER_CREATED", self._codes(result))

    def test_core_nobody_calls_is_a_red(self):
        wiring = self.root / gate.ASSETS / "Gameplay" / "Wiring.cs"
        wiring.write_text(wiring.read_text().replace("CanalStalkerCore.Stage(1, 1f, true, false)", ""),
                          encoding="utf-8")
        result = gate.run_gate(self.root)
        self.assertIn("WIRING_NEVER_DRIVEN", self._codes(result))

    def test_spec_losing_an_id_is_a_red(self):
        _write(self.root / gate.SPEC, json.dumps({"ids": ["relay_node"]}))
        result = gate.run_gate(self.root)
        self.assertIn("WIRING_SPEC_ID_MISSING", self._codes(result))

    def test_missing_spec_is_a_red(self):
        (self.root / gate.SPEC).unlink()
        result = gate.run_gate(self.root)
        self.assertIn("WIRING_SPEC_MISSING", self._codes(result))

    def test_unfired_companion_cue_is_a_red(self):
        wiring = self.root / gate.ASSETS / "Gameplay" / "Wiring.cs"
        wiring.write_text(wiring.read_text().replace('rill.SayById("CATCH_OVERRUN");', ""),
                          encoding="utf-8")
        result = gate.run_gate(self.root)
        self.assertIn("WIRING_CUE_NEVER_FIRED", self._codes(result))

    def test_exit_codes_and_report(self):
        self.assertEqual(gate.EXIT_OK, gate.main(["--root", str(self.root)]))
        report = self.root / "report.json"
        gate.main(["--root", str(self.root), "--json-report", str(report)])
        payload = json.loads(report.read_text(encoding="utf-8"))
        self.assertEqual("level1_wiring_gate", payload["tool"])
        self.assertEqual("pass", payload["status"])

        (self.root / gate.SPEC).unlink()
        self.assertEqual(gate.EXIT_VALIDATION_FAILED, gate.main(["--root", str(self.root)]))

    def test_missing_root_is_an_operational_error(self):
        self.assertEqual(gate.EXIT_OPERATIONAL_ERROR,
                         gate.main(["--root", str(self.root / "nope")]))


if __name__ == "__main__":
    unittest.main()
