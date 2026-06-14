using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Gameplay;
using Ziptide.Visuals;
using Ziptide.Editor.Validation;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Edit-mode smoke/contract tests. These run headless in CI (no device required) and guard
    /// the core wiring/data invariants described in docs/00_LOCKED_CONTRACTS and docs/06_SCHEMAS.
    /// </summary>
    public class ZiptideContractTests
    {
        [Test]
        public void DependencyValidator_Passes_OnRealAsmdefs()
        {
            // Locked contract: Ziptide.Core must never reference Ziptide.Visuals.
            bool ok = DependencyValidator.Validate(out string message);
            Assert.IsTrue(ok, "Dependency validation failed: " + message);
        }

        [Test]
        public void WorldProfile_Defaults_AreSane()
        {
            var profile = ScriptableObject.CreateInstance<WorldProfile>();
            try
            {
                Assert.AreEqual(new Vector2(4f, 4f), profile.playAreaSize);
                Assert.IsTrue(profile.respawnOnFall);
                Assert.Less(profile.fallYThreshold, 0f, "Fall threshold should be below the ground.");
                Assert.IsNotNull(profile.availableThemes);
            }
            finally
            {
                Object.DestroyImmediate(profile);
            }
        }

        [Test]
        public void VisualThemeProfile_PlanetDefaults_AreSane()
        {
            var theme = ScriptableObject.CreateInstance<VisualThemeProfile>();
            try
            {
                Assert.Greater(theme.planet.distance, 0f);
                Assert.GreaterOrEqual(theme.planet.angularSizeDegrees, 1f);
                Assert.LessOrEqual(theme.planet.angularSizeDegrees, 60f);
                Assert.IsTrue(theme.planet.followPlayer);
            }
            finally
            {
                Object.DestroyImmediate(theme);
            }
        }

        [Test]
        public void PlayAreaBounds_Build_CreatesFourWalls()
        {
            var go = new GameObject("PlayAreaBoundsTest");
            try
            {
                var bounds = go.AddComponent<PlayAreaBounds>();
                bounds.Build(new Vector2(4f, 4f), 0f);

                Assert.AreEqual(4, go.transform.childCount, "Expected four boundary walls.");
                for (int i = 0; i < go.transform.childCount; i++)
                {
                    var wall = go.transform.GetChild(i);
                    var col = wall.GetComponent<BoxCollider>();
                    Assert.IsNotNull(col, "Each wall should have a BoxCollider.");
                    Assert.IsFalse(col.isTrigger, "Boundary walls should be solid, not triggers.");
                }
            }
            finally
            {
                Object.DestroyImmediate(go);
            }
        }

        [Test]
        public void PlayAreaBounds_Build_IsIdempotent()
        {
            var go = new GameObject("PlayAreaBoundsRebuildTest");
            try
            {
                var bounds = go.AddComponent<PlayAreaBounds>();
                bounds.Build(new Vector2(4f, 4f), 0f);
                bounds.Build(new Vector2(6f, 6f), 0f);

                Assert.AreEqual(4, go.transform.childCount, "Rebuilding should not accumulate walls.");
            }
            finally
            {
                Object.DestroyImmediate(go);
            }
        }
    }
}
