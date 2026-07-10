using NUnit.Framework;
using UnityEngine;
using Ziptide.Visuals;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// FORGE III F3.1 — the derivation law under test: the light rig computed from a vista must
    /// agree with that vista BY CONSTRUCTION (fog = horizon, ambient = the gradient, key light
    /// from the brightest body, elevation inside the sculpting band), stay deterministic, and
    /// hold for every authored vista in the library.
    /// </summary>
    public class SkyLightScriptTests
    {
        private static SkyVistaDefinition Vista(Color horizon, Color zenith, string id = "test_vista")
        {
            var v = ScriptableObject.CreateInstance<SkyVistaDefinition>();
            v.vistaId = id;
            v.skyGradient = new Gradient();
            v.skyGradient.SetKeys(
                new[] { new GradientColorKey(horizon, 0f), new GradientColorKey(zenith, 1f) },
                new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) });
            return v;
        }

        [Test]
        public void Derivation_IsDeterministic_AndSkyAgrees()
        {
            var horizon = new Color(0.7f, 0.45f, 0.3f);
            var zenith = new Color(0.1f, 0.15f, 0.3f);
            var v = Vista(horizon, zenith);
            var a = SkyLightScript.Derive(v);
            var b = SkyLightScript.Derive(v);

            Assert.AreEqual(a.keyDirection, b.keyDirection, "deterministic");
            Assert.AreEqual(a.fogColor, b.fogColor, "deterministic");

            // THE LAW: fog is the horizon, ambient is the gradient.
            Assert.AreEqual(horizon.r, a.fogColor.r, 1e-4f, "fog IS the horizon color");
            Assert.AreEqual(zenith.g, a.ambientSky.g, 1e-4f, "ambient sky IS the zenith");
            Assert.AreEqual(horizon.b, a.ambientEquator.b, 1e-4f, "ambient equator IS the horizon");
            Assert.Less(SkyLum(a.ambientGround), SkyLum(a.ambientEquator), "ground darker than equator");
            Object.DestroyImmediate(v);
        }

        [Test]
        public void KeyLight_ComesFromTheSun_ElevationClamped()
        {
            var v = Vista(Color.gray, Color.blue);
            v.bodies.Add(new SkyVistaDefinition.CelestialBodyDef
            {
                type = SkyVistaDefinition.BodyType.Moon, angularSizeDeg = 30f,
                direction = new Vector3(0f, 0.5f, 0.87f), baseColor = Color.red,
            });
            v.bodies.Add(new SkyVistaDefinition.CelestialBodyDef
            {
                type = SkyVistaDefinition.BodyType.SunDisc, angularSizeDeg = 6f,
                direction = new Vector3(1f, 0.05f, 0f), // grazing — must clamp UP to 20°
                baseColor = new Color(1f, 0.9f, 0.7f),
            });

            var d = SkyLightScript.Derive(v);
            Vector3 from = -d.keyDirection;
            Assert.GreaterOrEqual(from.y, 0.342f - 1e-3f, "elevation clamped to ≥20° (sculpting band)");
            Assert.LessOrEqual(from.y, 0.819f + 1e-3f, "elevation clamped to ≤55°");
            Assert.Greater(from.x, 0.5f, "azimuth preserved — the light still comes from the sun's side");
            Assert.Greater(d.keyColor.r, d.keyColor.b, "key color carries the sun's warmth");
            Object.DestroyImmediate(v);
        }

        [Test]
        public void BodilessSky_StaysInRange_AndSiblingsDiffer()
        {
            var a = SkyLightScript.Derive(Vista(Color.gray, Color.white, "w_alpha"));
            var b = SkyLightScript.Derive(Vista(Color.gray, Color.white, "w_beta"));
            Assert.GreaterOrEqual(a.keyIntensity, 0.7f);
            Assert.LessOrEqual(a.keyIntensity, 1.4f);
            Assert.AreEqual(1f, a.keyDirection.magnitude, 1e-3f, "key direction normalized");
            Assert.Greater(Vector3.Angle(a.keyDirection, b.keyDirection), 0.5f,
                "different vista ids must not share one sun azimuth (worlds would all shadow alike)");
        }

        [Test]
        public void HazardAtmosphere_ThickensTheAir()
        {
            var clear = Vista(Color.gray, Color.blue, "w_clear");
            var hazed = Vista(Color.gray, Color.blue, "w_hazed");
            hazed.atmosphere.enabled = true;
            hazed.atmosphere.intensity = 0.8f;
            Assert.Greater(SkyLightScript.Derive(hazed).fogDensity,
                SkyLightScript.Derive(clear).fogDensity, "hazard worlds are foggier");
            Assert.LessOrEqual(SkyLightScript.Derive(hazed).fogDensity, 0.02f, "density capped");
            Object.DestroyImmediate(clear);
            Object.DestroyImmediate(hazed);
        }

        [Test]
        public void EveryAuthoredVista_DerivesInRange()
        {
            foreach (var spec in Ziptide.Editor.Patching.SkyVistaLibrary.Specs())
            {
                var v = spec.Value();
                var d = SkyLightScript.Derive(v);
                Assert.IsTrue(Finite(d.keyDirection), spec.Key + " key direction finite");
                Assert.GreaterOrEqual(d.keyIntensity, 0.7f, spec.Key);
                Assert.LessOrEqual(d.keyIntensity, 1.4f, spec.Key);
                Assert.GreaterOrEqual(-d.keyDirection.y, 0.342f - 1e-3f, spec.Key + " elevation band");
                Assert.Greater(d.fogDensity, 0f, spec.Key);
                Assert.LessOrEqual(d.fogDensity, 0.02f, spec.Key);
                Object.DestroyImmediate(v);
            }
        }

        private static float SkyLum(Color c) => 0.2126f * c.r + 0.7152f * c.g + 0.0722f * c.b;
        private static bool Finite(Vector3 v) =>
            !float.IsNaN(v.x) && !float.IsNaN(v.y) && !float.IsNaN(v.z)
            && !float.IsInfinity(v.x) && !float.IsInfinity(v.y) && !float.IsInfinity(v.z);
    }
}
