from __future__ import annotations

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


if __name__ == "__main__":
    unittest.main()
