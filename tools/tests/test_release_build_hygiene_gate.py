import json
import tempfile
import unittest
from copy import deepcopy
from pathlib import Path

from tools import release_build_hygiene_gate as gate


CLEAN_PROJECT_SETTINGS = """PlayerSettings:
  bundleVersion: 1.0.0
  applicationIdentifier:
    Android: com.example.game
    Standalone: com.example.game
  AndroidBundleVersionCode: 7
  AndroidMinSdkVersion: 29
  AndroidTargetSdkVersion: 32
  AndroidTargetArchitectures: 2
  AndroidKeystoreName: keys/release.keystore
  AndroidKeyaliasName: release
  scriptingDefineSymbols:
    Android: SAFE_DEFINE
    Standalone: SAFE_DEFINE
  scriptingBackend:
    Android: 1
"""

CLEAN_BUILD_SCRIPT = """public static class BuildAndroid
{
    public static void APK() { Build(BuildOptions.None); }
    public static void ReleaseAPK() { Build(BuildOptions.None); }
}
"""


class ReleaseBuildHygieneGateTests(unittest.TestCase):
    def setUp(self):
        self.tmp = tempfile.TemporaryDirectory()
        self.root = Path(self.tmp.name)
        (self.root / "docs/release").mkdir(parents=True)
        (self.root / "Ziptide/ProjectSettings").mkdir(parents=True)
        (self.root / "Ziptide/Assets/Editor").mkdir(parents=True)
        (self.root / "Ziptide/Packages").mkdir(parents=True)

        self.settings_path = self.root / "Ziptide/ProjectSettings/ProjectSettings.asset"
        self.build_path = self.root / "Ziptide/Assets/Editor/BuildAndroid.cs"
        self.packages_path = self.root / "Ziptide/Packages/manifest.json"
        self.contract_path = self.root / "docs/release/release_build_contract.json"

        self.settings_path.write_text(CLEAN_PROJECT_SETTINGS, encoding="utf-8")
        self.build_path.write_text(CLEAN_BUILD_SCRIPT, encoding="utf-8")
        self.packages_path.write_text(json.dumps({"dependencies": {"com.unity.xr": "1.0"}}),
                                      encoding="utf-8")

        self.contract = {
            "schemaVersion": 1,
            "contractId": "test",
            "sources": {
                "projectSettings": "Ziptide/ProjectSettings/ProjectSettings.asset",
                "buildScript": "Ziptide/Assets/Editor/BuildAndroid.cs",
                "packagesManifest": "Ziptide/Packages/manifest.json",
                "photonServerSettings": "Ziptide/Assets/Photon/Resources/PhotonServerSettings.asset",
            },
            "expected": {
                "applicationIdentifierAndroid": "com.example.game",
                "scriptingBackendAndroid": "1",
                "androidTargetArchitectures": "2",
                "androidMinSdkVersionAtLeast": 29,
                "requireExplicitTargetSdk": True,
                "maxApkBytes": 1000,
                "releaseForbiddenDefinesAndroid": ["ZIPTIDE_PHOTON"],
                "devBuildFlagPatterns": ["BuildOptions.Development"],
                "releaseBuildMethodName": "ReleaseAPK",
                "demoContentRoots": ["Ziptide/Assets/Photon/Demos"],
                "unityPackagePrefix": "com.unity.",
                "allowedNonUnityPackages": [],
            },
            "manualChecks": [],
        }

    def tearDown(self):
        self.tmp.cleanup()

    def write_contract(self, payload=None):
        self.contract_path.write_text(json.dumps(payload or self.contract), encoding="utf-8")

    def run_gate(self, release=False):
        self.write_contract(self.contract)
        return gate.run_gate(self.root, self.contract_path, release)

    def codes(self, result):
        return {finding.code for finding in result.findings}

    def severity(self, result, code):
        return next(f.severity for f in result.findings if f.code == code)

    def test_clean_fixture_is_release_ready(self):
        result = self.run_gate(release=True)
        self.assertEqual(result.count("blocker"), 0, result.findings)
        self.assertEqual(result.count("hold"), 0, result.findings)

    def test_invalid_contract_schema_blocks(self):
        self.contract["schemaVersion"] = 99
        result = self.run_gate()
        self.assertIn("CONTRACT_SCHEMA_UNSUPPORTED", self.codes(result))
        self.assertGreater(result.count("blocker"), 0)

    def test_manual_verified_without_evidence_blocks(self):
        self.contract["manualChecks"] = [{
            "id": "x", "title": "t", "owner": "o", "nextAction": "n",
            "required": True, "status": "verified", "evidence": [],
        }]
        result = self.run_gate()
        self.assertIn("MANUAL_VERIFIED_WITHOUT_EVIDENCE", self.codes(result))

    def test_app_id_mismatch_blocks_in_both_modes(self):
        self.contract["expected"]["applicationIdentifierAndroid"] = "com.other.game"
        result = self.run_gate()
        self.assertIn("APP_ID_MISMATCH", self.codes(result))
        self.assertEqual(self.severity(result, "APP_ID_MISMATCH"), "blocker")

    def test_forbidden_define_holds_dev_blocks_release(self):
        self.settings_path.write_text(
            CLEAN_PROJECT_SETTINGS.replace("Android: SAFE_DEFINE", "Android: SAFE_DEFINE;ZIPTIDE_PHOTON"),
            encoding="utf-8")
        dev = self.run_gate(release=False)
        self.assertEqual(self.severity(dev, "FORBIDDEN_DEFINES_PRESENT"), "hold")
        self.assertEqual(dev.count("blocker"), 0)
        release = self.run_gate(release=True)
        self.assertEqual(self.severity(release, "FORBIDDEN_DEFINES_PRESENT"), "blocker")

    def test_dev_build_flags_detected(self):
        self.build_path.write_text(
            CLEAN_BUILD_SCRIPT.replace("public static void APK() { Build(BuildOptions.None); }",
                                       "public static void APK() { Build(BuildOptions.Development); }"),
            encoding="utf-8")
        result = self.run_gate()
        self.assertIn("DEV_BUILD_FLAGS_HARDCODED", self.codes(result))

    def test_missing_release_method_holds(self):
        self.build_path.write_text(
            CLEAN_BUILD_SCRIPT.replace("public static void ReleaseAPK() { Build(BuildOptions.None); }", ""),
            encoding="utf-8")
        result = self.run_gate()
        self.assertIn("RELEASE_METHOD_MISSING", self.codes(result))

    def test_keystore_unconfigured_holds(self):
        self.settings_path.write_text(
            CLEAN_PROJECT_SETTINGS
            .replace("AndroidKeystoreName: keys/release.keystore", "AndroidKeystoreName: ")
            .replace("AndroidKeyaliasName: release", "AndroidKeyaliasName: "),
            encoding="utf-8")
        result = self.run_gate()
        self.assertIn("KEYSTORE_UNCONFIGURED", self.codes(result))
        self.assertEqual(self.severity(result, "KEYSTORE_UNCONFIGURED"), "hold")

    def test_demo_content_and_photon_appid_hold(self):
        demos = self.root / "Ziptide/Assets/Photon/Demos"
        demos.mkdir(parents=True)
        (demos / "scene.unity").write_text("demo", encoding="utf-8")
        photon = self.root / "Ziptide/Assets/Photon/Resources"
        photon.mkdir(parents=True)
        (photon / "PhotonServerSettings.asset").write_text(
            "  AppIdRealtime: abc-123\n", encoding="utf-8")
        result = self.run_gate()
        self.assertIn("DEMO_CONTENT_PRESENT", self.codes(result))
        self.assertIn("PHOTON_APPID_SHIPS", self.codes(result))
        self.assertEqual(result.count("blocker"), 0)

    def test_rogue_package_blocks(self):
        self.packages_path.write_text(
            json.dumps({"dependencies": {"com.unity.xr": "1.0", "com.photon.pun": "2.55"}}),
            encoding="utf-8")
        result = self.run_gate()
        self.assertIn("NON_UNITY_PACKAGE_UNDECLARED", self.codes(result))
        self.assertEqual(self.severity(result, "NON_UNITY_PACKAGE_UNDECLARED"), "blocker")

    def test_apk_over_cap_blocks(self):
        apk = self.root / "Builds/Android"
        apk.mkdir(parents=True)
        (apk / "Ziptide.apk").write_bytes(b"x" * 2000)
        result = self.run_gate()
        self.assertIn("APK_OVER_SIZE_CAP", self.codes(result))

    def test_manual_hold_blocks_only_release_mode(self):
        self.contract["manualChecks"] = [{
            "id": "keystore-custody", "title": "t", "owner": "o", "nextAction": "n",
            "required": True, "status": "hold", "evidence": [],
        }]
        dev = self.run_gate(release=False)
        self.assertEqual(dev.count("blocker"), 0)
        release = self.run_gate(release=True)
        self.assertGreater(release.count("blocker"), 0)

    def test_facts_snapshot_recorded(self):
        result = self.run_gate()
        self.assertEqual(result.facts["applicationIdentifierAndroid"], "com.example.game")
        self.assertEqual(result.facts["scriptingBackendAndroid"], "1")
        self.assertTrue(result.facts["androidKeystoreConfigured"])


if __name__ == "__main__":
    unittest.main()
