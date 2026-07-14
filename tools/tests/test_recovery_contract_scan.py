from __future__ import annotations

import json
import sys
import tempfile
import unittest
from pathlib import Path

TOOLS = Path(__file__).resolve().parents[1]
if str(TOOLS) not in sys.path:
    sys.path.insert(0, str(TOOLS))

import recovery_contract_scan as scan


class RecoveryContractScanTests(unittest.TestCase):
    def test_scan_text_reports_cross_cutting_contract_signals(self) -> None:
        text = """
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
namespace Ziptide.Test
{
    public class BootThing : MonoBehaviour
    {
        public static event System.Action Happened;

        [RuntimeInitializeOnLoadMethod]
        private static void Boot()
        {
            var go = new GameObject("__BootThing");
            DontDestroyOnLoad(go);
            SceneManager.LoadScene("W001");
            var tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tile.AddComponent<TextMesh>();
            var manager = FindObjectOfType<XRInteractionManager>();
            bool open = UnityEngine.InputSystem.Gamepad.current.buttonNorth.wasPressedThisFrame;
            SaveSystem.AutosaveNow("test");
            RenderSettings.fog = true;
            var material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
            // placeholder fallback
            Debug.Log("ZIPTIDE: BOOT_THING");
        }
    }
}
"""
        findings = scan.scan_text("Assets/BootThing.cs", text)
        codes = {finding.code for finding in findings}

        expected = {
            "RUNTIME_BOOTSTRAP",
            "NEW_GAME_OBJECT",
            "DONT_DESTROY_ON_LOAD",
            "DIRECT_SCENE_LOAD",
            "CREATE_PRIMITIVE",
            "TEXTMESH_COMPONENT",
            "XRI_MANAGER_LOOKUP",
            "INPUT_BUTTON_REFERENCE",
            "FRAME_BUTTON_POLL",
            "SAVE_SYSTEM_REFERENCE",
            "AUTOSAVE_CALL",
            "RENDER_SETTINGS_MUTATION",
            "RUNTIME_MATERIAL_CREATE",
            "SHADER_FIND",
            "FALLBACK_MARKER",
            "ZIPTIDE_LOG_TAG",
            "STATIC_EVENT_DECLARATION",
        }
        self.assertTrue(expected.issubset(codes))
        self.assertTrue(all(finding.symbol == "Ziptide.Test.BootThing" for finding in findings))
        self.assertTrue(all(finding.line > 0 for finding in findings))

    def test_fallback_markers_are_deduplicated_per_line(self) -> None:
        findings = scan.scan_text(
            "Assets/Fallback.cs",
            "// fallback placeholder interim stub plumbing skeleton graybox blockout\n",
        )
        matches = [finding for finding in findings if finding.code == "FALLBACK_MARKER"]
        self.assertEqual(1, len(matches))

    def test_repository_scan_ignores_library_and_is_deterministic(self) -> None:
        with tempfile.TemporaryDirectory() as tmp:
            root = Path(tmp)
            included = root / "Ziptide/Assets/Ziptide/Runtime"
            ignored = root / "Ziptide/Library"
            included.mkdir(parents=True)
            ignored.mkdir(parents=True)
            (included / "B.cs").write_text("class B { void X(){ new GameObject(); } }", encoding="utf-8")
            (included / "A.cs").write_text("class A { void X(){ DontDestroyOnLoad(this); } }", encoding="utf-8")
            (ignored / "Ignored.cs").write_text("class Ignored { }", encoding="utf-8")

            result = scan.scan_repository(root, ("Ziptide",))

            self.assertEqual(2, result.scanned_files)
            paths = [finding.path for finding in result.findings]
            self.assertEqual(sorted(paths), paths)
            self.assertFalse(any("Library" in path for path in paths))

    def test_reports_write_valid_json_and_markdown(self) -> None:
        with tempfile.TemporaryDirectory() as tmp:
            root = Path(tmp)
            source = root / "Ziptide/Assets/Ziptide/Test"
            source.mkdir(parents=True)
            (source / "Test.cs").write_text(
                "namespace N { class C { void X(){ new GameObject(); } } }",
                encoding="utf-8",
            )
            result = scan.scan_repository(root, scan.DEFAULT_SCAN_ROOTS)
            json_path = root / "Builds/Reports/scan.json"
            markdown_path = root / "Builds/Reports/scan.md"

            scan.write_json_report(result, json_path)
            scan.write_markdown_report(result, markdown_path)

            payload = json.loads(json_path.read_text(encoding="utf-8"))
            self.assertEqual("recovery_contract_scan", payload["tool"])
            self.assertEqual(1, payload["scannedFiles"])
            self.assertIn("# ZIPTIDE Recovery Contract Scan", markdown_path.read_text(encoding="utf-8"))

    def test_main_is_report_only(self) -> None:
        with tempfile.TemporaryDirectory() as tmp:
            root = Path(tmp)
            source = root / "Ziptide/Assets/Ziptide/Test"
            source.mkdir(parents=True)
            (source / "Test.cs").write_text(
                "[RuntimeInitializeOnLoadMethod] class C { }",
                encoding="utf-8",
            )
            exit_code = scan.main(["--root", str(root)])
            self.assertEqual(scan.EXIT_OK, exit_code)
            self.assertTrue((root / "Builds/Reports/recovery_contract_scan.json").exists())


if __name__ == "__main__":
    unittest.main()
