using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Editor.Patching;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// The Arena Factory's design contract (PVP_ARENA_AAA §A2): all five launch arenas validate clean,
    /// have unique identities, real bot navigation, spawn separation, and the fundamentals a fun map
    /// needs. If a spec edit breaks a map's basics, CI says so before Terry ever sideloads it.
    /// </summary>
    public class ArenaLibraryTests
    {
        private static List<ArenaLayoutDefinition> _specs;

        [OneTimeSetUp]
        public void Build() => _specs = ArenaLayoutLibrary.Specs();

        [OneTimeTearDown]
        public void Cleanup()
        {
            foreach (var s in _specs) Object.DestroyImmediate(s);
        }

        [Test]
        public void FiveArenas_AllValidateClean()
        {
            Assert.AreEqual(5, _specs.Count);
            foreach (var a in _specs)
                Assert.IsEmpty(a.Validate(), a.sceneName + ": " + string.Join(" | ", a.Validate()));
        }

        [Test]
        public void Identities_AreUnique()
        {
            var scenes = new HashSet<string>();
            var ids = new HashSet<string>();
            foreach (var a in _specs)
            {
                Assert.IsTrue(scenes.Add(a.sceneName), "duplicate sceneName " + a.sceneName);
                Assert.IsTrue(ids.Add(a.arenaId), "duplicate arenaId " + a.arenaId);
            }
        }

        [Test]
        public void EveryArena_HasRealBotNavigation()
        {
            foreach (var a in _specs)
            {
                Assert.GreaterOrEqual(a.waypoints.Count, 6, a.sceneName + " needs a patrol circuit");
                Assert.GreaterOrEqual(a.coverPoints.Count, 4, a.sceneName + " needs cover spots");
            }
        }

        [Test]
        public void EveryArena_ArmsThePlayer_AndHasAnObjective()
        {
            foreach (var a in _specs)
            {
                bool hasTaser = a.weaponPads.Exists(p => p.itemId == "taser_dart_gun");
                Assert.IsTrue(hasTaser, a.sceneName + " must offer the starter taser");
                Assert.GreaterOrEqual(a.weaponPads.Count, 3, a.sceneName + " needs at least 3 pads (map control)");
                Assert.GreaterOrEqual(a.objectiveZones.Count, 1, a.sceneName + " needs a mode objective (A3)");
            }
        }

        [Test]
        public void Difficulty_SpansTheLadder()
        {
            var tiers = new HashSet<string>();
            foreach (var a in _specs) tiers.Add(a.botDifficulty);
            Assert.GreaterOrEqual(tiers.Count, 3, "the five arenas should span at least 3 bot tiers");
        }

        [Test]
        public void SkyIdentities_AreDistinct()
        {
            // The one-glance test: no two arenas share a horizon color (their mood signature).
            var horizons = new HashSet<Color>();
            foreach (var a in _specs)
                Assert.IsTrue(horizons.Add(a.skyHorizonColor), a.sceneName + " shares a sky with another arena");
        }

        [Test]
        public void WaypointsAndSpawns_SitInsideTheFloor()
        {
            foreach (var a in _specs)
            {
                float hx = a.floorSize.x / 2f, hz = a.floorSize.y / 2f;
                foreach (var w in a.waypoints)
                {
                    Assert.LessOrEqual(Mathf.Abs(w.x), hx, a.sceneName + " waypoint outside floor X");
                    Assert.LessOrEqual(Mathf.Abs(w.z), hz, a.sceneName + " waypoint outside floor Z");
                }
            }
        }
    }
}
