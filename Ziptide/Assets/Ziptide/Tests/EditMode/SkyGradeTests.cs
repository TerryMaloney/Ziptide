using NUnit.Framework;
using UnityEngine;
using Ziptide.Visuals;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// FORGE III F3.2 — the grade clamps ARE the contract: no derivation may exceed post-exposure
    /// ±0.3, saturation −10..+15, an 8% color filter, or ±15 white balance — for ANY vista,
    /// including every authored one in the library. Plus the two derivation behaviors that carry
    /// meaning: haze desaturates, warmth follows the horizon.
    /// </summary>
    public class SkyGradeTests
    {
        private static SkyVistaDefinition Vista(Color horizon, Color zenith, string id = "grade_vista")
        {
            var v = ScriptableObject.CreateInstance<SkyVistaDefinition>();
            v.vistaId = id;
            v.skyGradient = new Gradient();
            v.skyGradient.SetKeys(
                new[] { new GradientColorKey(horizon, 0f), new GradientColorKey(zenith, 1f) },
                new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) });
            return v;
        }

        private static void AssertInContract(SkyGrade.GradeValues g, string tag)
        {
            Assert.GreaterOrEqual(g.postExposure, -0.3f, tag + " exposure floor");
            Assert.LessOrEqual(g.postExposure, 0.3f, tag + " exposure cap");
            Assert.GreaterOrEqual(g.saturation, -10f, tag + " saturation floor");
            Assert.LessOrEqual(g.saturation, 15f, tag + " saturation cap");
            Assert.GreaterOrEqual(g.temperature, -15f, tag + " temperature floor");
            Assert.LessOrEqual(g.temperature, 15f, tag + " temperature cap");
            Assert.LessOrEqual(Mathf.Abs(1f - g.colorFilter.r), 0.08f + 1e-4f, tag + " filter r ≤8%");
            Assert.LessOrEqual(Mathf.Abs(1f - g.colorFilter.g), 0.08f + 1e-4f, tag + " filter g ≤8%");
            Assert.LessOrEqual(Mathf.Abs(1f - g.colorFilter.b), 0.08f + 1e-4f, tag + " filter b ≤8%");
        }

        [Test]
        public void Derivation_Deterministic_AndInsideTheContract()
        {
            var v = Vista(new Color(0.8f, 0.5f, 0.3f), new Color(0.1f, 0.12f, 0.2f));
            var a = SkyGrade.Derive(v);
            var b = SkyGrade.Derive(v);
            Assert.AreEqual(a.postExposure, b.postExposure, "deterministic");
            Assert.AreEqual(a.colorFilter, b.colorFilter, "deterministic");
            AssertInContract(a, "warm-dark");
            AssertInContract(SkyGrade.Derive(Vista(Color.white, Color.white)), "blown-white");
            AssertInContract(SkyGrade.Derive(Vista(Color.black, Color.black)), "pitch-black");
            Object.DestroyImmediate(v);
        }

        [Test]
        public void HazardHaze_Desaturates()
        {
            var clear = Vista(Color.gray, Color.blue, "g_clear");
            var hazed = Vista(Color.gray, Color.blue, "g_hazed");
            hazed.atmosphere.enabled = true;
            hazed.atmosphere.intensity = 1f;
            Assert.Less(SkyGrade.Derive(hazed).saturation, SkyGrade.Derive(clear).saturation,
                "a hazed world reads through murk — less saturated");
            Object.DestroyImmediate(clear);
            Object.DestroyImmediate(hazed);
        }

        [Test]
        public void Warmth_FollowsTheHorizon()
        {
            var warm = SkyGrade.Derive(Vista(new Color(0.9f, 0.5f, 0.2f), Color.gray, "g_warm"));
            var cold = SkyGrade.Derive(Vista(new Color(0.2f, 0.4f, 0.9f), Color.gray, "g_cold"));
            Assert.Greater(warm.temperature, 0f, "amber horizon grades warm");
            Assert.Less(cold.temperature, 0f, "blue horizon grades cold");
        }

        [Test]
        public void EveryAuthoredVista_GradesInsideTheContract()
        {
            foreach (var spec in Ziptide.Editor.Patching.SkyVistaLibrary.Specs())
            {
                var v = spec.Value();
                AssertInContract(SkyGrade.Derive(v), spec.Key);
                Object.DestroyImmediate(v);
            }
        }
    }
}
