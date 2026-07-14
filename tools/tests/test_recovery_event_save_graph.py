from __future__ import annotations

import sys
import tempfile
import unittest
from pathlib import Path

TOOLS = Path(__file__).resolve().parents[1]
if str(TOOLS) not in sys.path:
    sys.path.insert(0, str(TOOLS))

from recovery_event_save_graph import scan_repository, scan_text


class RecoveryEventSaveGraphTests(unittest.TestCase):
    def test_scans_events_subscriptions_and_save_access(self) -> None:
        text = '''
namespace Ziptide.Test
{
    public class Owner
    {
        public static event System.Action<string> TravelCompleted;
        void OnEnable() { TravelCoordinator.TravelCompleted += OnTravel; }
        void OnDisable() { TravelCoordinator.TravelCompleted -= OnTravel; }
        void Done()
        {
            TravelCompleted?.Invoke("W001");
            SaveSystem.AutosaveNow("travel");
            var credits = SaveSystem.Instance.Profile.credits;
        }
        void OnTravel(string scene) {}
    }
}
'''
        evidence = scan_text("Owner.cs", text)
        kinds = {item.kind for item in evidence}
        self.assertIn("EVENT_DECLARE", kinds)
        self.assertIn("EVENT_SUBSCRIBE", kinds)
        self.assertIn("EVENT_UNSUBSCRIBE", kinds)
        self.assertIn("EVENT_INVOKE", kinds)
        self.assertIn("AUTOSAVE", kinds)
        self.assertIn("PROFILE_FIELD_ACCESS", kinds)

    def test_reports_unmatched_named_subscription(self) -> None:
        with tempfile.TemporaryDirectory() as temp:
            root = Path(temp)
            source = root / "src"
            source.mkdir()
            (source / "Owner.cs").write_text(
                "class Owner { void OnEnable(){ SceneManager.sceneLoaded += OnScene; } void OnScene(){} }",
                encoding="utf-8",
            )
            report = scan_repository(root, ("src",))
            self.assertEqual(1, report["unmatchedNamedSubscriptionCount"])

    def test_matching_unsubscribe_clears_review_target(self) -> None:
        with tempfile.TemporaryDirectory() as temp:
            root = Path(temp)
            source = root / "src"
            source.mkdir()
            (source / "Owner.cs").write_text(
                "class Owner { void A(){ SceneManager.sceneLoaded += OnScene; } void B(){ SceneManager.sceneLoaded -= OnScene; } void OnScene(){} }",
                encoding="utf-8",
            )
            report = scan_repository(root, ("src",))
            self.assertEqual(0, report["unmatchedNamedSubscriptionCount"])


if __name__ == "__main__":
    unittest.main()
