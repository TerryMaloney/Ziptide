from __future__ import annotations

import json
import re
import sys
import tempfile
import unittest
from pathlib import Path

TOOLS = Path(__file__).resolve().parents[1]
if str(TOOLS) not in sys.path:
    sys.path.insert(0, str(TOOLS))

from recovery_input_contract_scan import scan_repository, scan_text


class RecoveryInputContractScanTests(unittest.TestCase):
    def test_pairs_action_with_binding_and_finds_menu_chord(self) -> None:
        text = '''
namespace Ziptide.Gameplay
{
    public class QuickSwap
    {
        private InputAction _swap;
        void Start()
        {
            _swap = new InputAction("ZiptideQuickSwap", InputActionType.Button);
            _swap.AddBinding("<XRController>{RightHand}/secondaryButton"); // B
            // Y+B = dev menu chord
        }
    }
}
'''
        bindings, chords = scan_text("QuickSwap.cs", text)
        self.assertEqual(1, len(bindings))
        self.assertEqual("ZiptideQuickSwap", bindings[0].action_name)
        self.assertEqual("<XRController>{RightHand}/secondaryButton", bindings[0].binding)
        self.assertEqual(1, len(chords))

    def test_unresolved_field_binding_is_preserved(self) -> None:
        bindings, _ = scan_text("Owner.cs", '_fire.AddBinding("<XRController>/trigger");')
        self.assertEqual("<unresolved>", bindings[0].action_name)

    def test_repository_reports_cross_owner_collision(self) -> None:
        with tempfile.TemporaryDirectory() as temp:
            root = Path(temp)
            source = root / "src"
            source.mkdir()
            (source / "A.cs").write_text(
                'class A { InputAction _a; void X(){ _a = new InputAction("A"); _a.AddBinding("<XRController>/trigger"); }}',
                encoding="utf-8",
            )
            (source / "B.cs").write_text(
                'class B { InputAction _b; void X(){ _b = new InputAction("B"); _b.AddBinding("<XRController>/trigger"); }}',
                encoding="utf-8",
            )
            report = scan_repository(root, ("src",))
            self.assertEqual(1, report["collisionControlCount"])
            self.assertIn("<XRController>/trigger", report["controlCollisions"])

    def test_virtual_xr_layout_registration_is_test_only(self) -> None:
        root = TOOLS.parent
        bootstrap = (
            root
            / "Ziptide"
            / "Assets"
            / "Ziptide"
            / "Tests"
            / "PlayMode"
            / "RecoveryVirtualXrLayoutBootstrap.cs"
        )
        asmdef = bootstrap.parent / "Ziptide.Tests.PlayMode.asmdef"

        self.assertTrue(bootstrap.exists(), "Recovery virtual XR layout bootstrap is missing.")
        self.assertTrue(asmdef.exists(), "Recovery PlayMode test assembly definition is missing.")
        assembly = json.loads(asmdef.read_text(encoding="utf-8"))
        self.assertIn("UNITY_INCLUDE_TESTS", assembly.get("defineConstraints", []))
        self.assertFalse(assembly.get("autoReferenced", True))
        self.assertIn("InputSystem.RegisterLayoutOverride", bootstrap.read_text(encoding="utf-8"))

        registration = re.compile(r"\bInputSystem\s*\.\s*RegisterLayout\w*\s*(?:<|\()")
        offenders: list[str] = []
        for relative_root in (
            Path("Ziptide/Assets/Ziptide"),
            Path("Ziptide/Assets/ZiptideNet"),
        ):
            source_root = root / relative_root
            if not source_root.exists():
                continue
            for path in source_root.rglob("*.cs"):
                relative = path.relative_to(root)
                if "Tests" in relative.parts or "Editor" in relative.parts:
                    continue
                text = path.read_text(encoding="utf-8", errors="replace")
                if registration.search(text):
                    offenders.append(str(relative).replace("\\", "/"))

        self.assertEqual(
            [],
            sorted(offenders),
            "Runtime/editor-independent source registered an Input System layout; "
            "the recovery virtual device must remain test-only.",
        )


if __name__ == "__main__":
    unittest.main()
