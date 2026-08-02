using NUnit.Framework;
using UnityEngine;

using Ziptide.Editor.Audit;

namespace Ziptide.Tests.EditMode
{
    public sealed class InteractionReachAuditRulesTests
    {
        [TestCase(0.90f, 0.20f, true, ReachGateResult.Pass)]
        [TestCase(0.90f, 0.20f, false, ReachGateResult.NoSupport)]
        [TestCase(0.10f, 0.20f, true, ReachGateResult.TooLow)]
        [TestCase(1.80f, 0.20f, true, ReachGateResult.TooHigh)]
        [TestCase(0.90f, 0.02f, true, ReachGateResult.TooSmall)]
        public void Evaluate_EnforcesTheReachEnvelope(float height, float width, bool supported,
            ReachGateResult expected)
        {
            Assert.That(InteractionReachAuditRules.Evaluate(height, width, supported), Is.EqualTo(expected));
        }

        [Test]
        public void Evaluate_PowerSwitchUsesChildReachCeiling()
        {
            Assert.That(InteractionReachAuditRules.Evaluate(1.20f, 0.20f, true,
                InteractionReachAuditRules.PowerMaxCenterHeight), Is.EqualTo(ReachGateResult.TooHigh));
        }

        [TestCase("Tile_CONFIRM")]
        [TestCase("Dest_ToxicCity")]
        [TestCase("PowerSwitch")]
        [TestCase("PUNCH IT")]
        [TestCase("BoardPanel")]
        [TestCase("DisembarkPanel")]
        [TestCase("TravelButton")]
        public void IsCritical_RecognizesFixedGameplayControls(string name)
        {
            Assert.That(InteractionReachAuditRules.IsCritical(name), Is.True);
        }

        [Test]
        public void Run_DeliberatelyHighPowerSwitch_FiresBlocker()
        {
            GameObject floor = null;
            GameObject control = null;
            try
            {
                floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
                floor.name = "ReachTestFloor";
                floor.transform.position = new Vector3(0f, -0.05f, 0f);
                floor.transform.localScale = new Vector3(4f, 0.1f, 4f);

                control = GameObject.CreatePrimitive(PrimitiveType.Cube);
                control.name = "PowerSwitch";
                control.transform.position = new Vector3(0f, 2.0f, 0f);
                control.transform.localScale = new Vector3(0.20f, 0.20f, 0.08f);
                control.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();

                var report = new SceneAuditReport { sceneName = "PG4_BrokenFixture" };
                InteractionReachAuditRules.Run(report);

                Assert.That(report.findings.Exists(f => f.code == InteractionReachAuditRules.TooHigh),
                    Is.True, "A child-unreachable power switch must make the build red.");
            }
            finally
            {
                if (control != null) Object.DestroyImmediate(control);
                if (floor != null) Object.DestroyImmediate(floor);
            }
        }

        [Test]
        public void Run_ReachableTileOnSupportedFloor_Passes()
        {
            GameObject floor = null;
            GameObject control = null;
            try
            {
                floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
                floor.name = "ReachTestFloor";
                floor.transform.position = new Vector3(0f, -0.05f, 0f);
                floor.transform.localScale = new Vector3(4f, 0.1f, 4f);

                control = GameObject.CreatePrimitive(PrimitiveType.Cube);
                control.name = "Tile_CONFIRM";
                control.transform.position = new Vector3(0f, 0.90f, 0f);
                control.transform.localScale = new Vector3(0.20f, 0.16f, 0.08f);
                control.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();

                var report = new SceneAuditReport { sceneName = "PG4_ValidFixture" };
                InteractionReachAuditRules.Run(report);

                Assert.That(report.blockerCount, Is.Zero,
                    string.Join("\n", report.findings.ConvertAll(f => f.ToString())));
            }
            finally
            {
                if (control != null) Object.DestroyImmediate(control);
                if (floor != null) Object.DestroyImmediate(floor);
            }
        }
    }
}
