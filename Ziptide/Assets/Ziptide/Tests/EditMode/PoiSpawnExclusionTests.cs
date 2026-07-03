using NUnit.Framework;
using UnityEngine;
using Ziptide.Editor.Patching;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// The spawn-exclusion contract behind the SPAWN_OVERLAP_SOLID fix (diag run 28682909567):
    /// no POI center may sit closer to the player spawn than its own props can reach.
    /// </summary>
    public class PoiSpawnExclusionTests
    {
        [Test]
        public void PoiInsideTheRing_IsPushedExactlyToTheRing()
        {
            Vector2 spawn = new Vector2(10f, 20f);
            Vector2 poi = spawn + new Vector2(3f, 4f); // 5m away — inside
            Vector2 result = WorldPoiBuilder.ExcludeFromSpawn(poi, spawn);
            Assert.AreEqual(WorldPoiBuilder.SpawnExclusionRadius, Vector2.Distance(result, spawn), 0.001f);
            // Pushed along the original direction, not teleported.
            Vector2 dir = (result - spawn).normalized;
            Assert.Greater(Vector2.Dot(dir, new Vector2(0.6f, 0.8f)), 0.999f);
        }

        [Test]
        public void PoiOutsideTheRing_IsUntouched()
        {
            Vector2 spawn = Vector2.zero;
            Vector2 poi = new Vector2(30f, -14f);
            Assert.AreEqual(poi, WorldPoiBuilder.ExcludeFromSpawn(poi, spawn));
        }

        [Test]
        public void PoiExactlyOnSpawn_StillEscapes()
        {
            Vector2 spawn = new Vector2(-5f, 7f);
            Vector2 result = WorldPoiBuilder.ExcludeFromSpawn(spawn, spawn);
            Assert.AreEqual(WorldPoiBuilder.SpawnExclusionRadius, Vector2.Distance(result, spawn), 0.001f);
        }

        [Test]
        public void ExclusionCoversTheWidestPropReach()
        {
            // CombatCamp cover ring = 7m + mast half-width; the ring must clear it with margin.
            Assert.GreaterOrEqual(WorldPoiBuilder.SpawnExclusionRadius, 10f);
        }
    }
}
