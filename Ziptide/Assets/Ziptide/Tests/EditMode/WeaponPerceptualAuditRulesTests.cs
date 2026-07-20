using NUnit.Framework;
using UnityEngine;
using Ziptide.Editor.Audit;

namespace Ziptide.Tests.EditMode
{
    public sealed class WeaponPerceptualAuditRulesTests
    {
        [Test]
        public void DeliberatelyTinyWeapon_FiresVisibleBoundsBlocker()
        {
            GameObject weapon = null;
            try
            {
                weapon = BuildWeapon(new Vector3(0.04f, 0.03f, 0.08f),
                    new Vector3(0f, 0f, 0.02f));
                var report = new SceneAuditReport { sceneName = "BrokenWeaponFixture" };

                WeaponPerceptualAuditRules.ValidateWeapon(weapon, report);

                Assert.That(report.HasCode(WeaponPerceptualAuditRules.TinyVisual), Is.True);
            }
            finally
            {
                if (weapon != null) Object.DestroyImmediate(weapon);
            }
        }

        [Test]
        public void DeliberatelySidewaysMuzzle_FiresAimAxisBlocker()
        {
            GameObject weapon = null;
            try
            {
                // The root has non-uniform world scale. A local X offset of 0.24 was not actually
                // sideways enough after scaling; 1.0 produces a true world-space lateral axis and
                // proves the same calculation the shipped gate uses.
                weapon = BuildWeapon(new Vector3(0.16f, 0.11f, 0.42f),
                    new Vector3(1.0f, 0f, 0f));
                var report = new SceneAuditReport { sceneName = "BrokenWeaponFixture" };

                WeaponPerceptualAuditRules.ValidateWeapon(weapon, report);

                Assert.That(report.HasCode(WeaponPerceptualAuditRules.BadAimAxis), Is.True);
            }
            finally
            {
                if (weapon != null) Object.DestroyImmediate(weapon);
            }
        }

        [Test]
        public void HumanScaleForwardWeapon_PassesPerceptualGate()
        {
            GameObject weapon = null;
            try
            {
                weapon = BuildWeapon(new Vector3(0.16f, 0.11f, 0.42f),
                    new Vector3(0f, 0f, 0.26f));
                var report = new SceneAuditReport { sceneName = "ValidWeaponFixture" };

                WeaponPerceptualAuditRules.ValidateWeapon(weapon, report);

                Assert.That(report.blockerCount, Is.Zero);
            }
            finally
            {
                if (weapon != null) Object.DestroyImmediate(weapon);
            }
        }

        private static GameObject BuildWeapon(Vector3 visualScale, Vector3 muzzlePosition)
        {
            GameObject weapon = GameObject.CreatePrimitive(PrimitiveType.Cube);
            weapon.name = "PerceptualWeapon";
            weapon.transform.localScale = visualScale;

            Transform grip = new GameObject("Grip").transform;
            grip.SetParent(weapon.transform, false);
            grip.localPosition = new Vector3(0f, 0f, -0.10f);
            grip.localRotation = Quaternion.identity;

            Transform muzzle = new GameObject("Muzzle").transform;
            muzzle.SetParent(weapon.transform, false);
            muzzle.localPosition = muzzlePosition;
            muzzle.localRotation = Quaternion.identity;
            return weapon;
        }
    }

    internal static class SceneAuditReportTestExtensions
    {
        public static bool HasCode(this SceneAuditReport report, string code)
        {
            for (int i = 0; i < report.findings.Count; i++)
                if (report.findings[i].code == code) return true;
            return false;
        }
    }
}
