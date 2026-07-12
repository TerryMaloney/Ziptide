#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Gameplay;
using Ziptide.Visuals;

namespace Ziptide.Tests.EditMode
{
    public class ReactivePropTests
    {
        [TearDown]
        public void TearDown()
        {
            WorldDebrisBudget.ClearAll();
            foreach (VfxFactory factory in Resources.FindObjectsOfTypeAll<VfxFactory>())
            {
                if (factory != null && factory.gameObject != null)
                    UnityEngine.Object.DestroyImmediate(factory.gameObject);
            }
            foreach (ReactiveProp prop in Resources.FindObjectsOfTypeAll<ReactiveProp>())
            {
                if (prop != null && prop.gameObject != null)
                    UnityEngine.Object.DestroyImmediate(prop.gameObject);
            }
        }

        [Test]
        public void OneShotState_ReactsExactlyOnceUntilReset()
        {
            var state = new ReactivePropState(oneShot: true, cooldownSeconds: 30.0);
            Assert.IsTrue(state.CanReact(0.0));
            Assert.IsTrue(state.TryReact(0.0));
            Assert.IsTrue(state.IsSpent);
            Assert.AreEqual(1, state.ReactionCount);
            Assert.IsFalse(state.TryReact(999.0));

            state.Reset();
            Assert.IsFalse(state.IsSpent);
            Assert.AreEqual(0, state.ReactionCount);
            Assert.IsTrue(state.TryReact(1000.0));
        }

        [Test]
        public void CooldownState_ReopensAtExactlyThirtySeconds()
        {
            var state = new ReactivePropState(oneShot: false, cooldownSeconds: 30.0);
            Assert.IsTrue(state.TryReact(10.0));
            Assert.AreEqual(40.0, state.ReadyAt, 1e-9);
            Assert.IsFalse(state.CanReact(39.999));
            Assert.IsFalse(state.TryReact(39.999));
            Assert.IsTrue(state.CanReact(40.0));
            Assert.IsTrue(state.TryReact(40.0));
            Assert.AreEqual(2, state.ReactionCount);
        }

        [Test]
        public void EveryReactionKind_MapsToExistingVfxRecipeAndLockedTimingLaw()
        {
            foreach (ReactionKind kind in Enum.GetValues(typeof(ReactionKind)))
            {
                string id = ReactivePropRules.VfxId(kind);
                Assert.IsNotNull(id, kind.ToString());
                Assert.IsNotNull(VfxLibrary.Get(id), kind + " -> " + id);
            }

            Assert.IsTrue(ReactivePropRules.IsOneShot(ReactionKind.LightFlickerOut));
            Assert.IsFalse(ReactivePropRules.IsOneShot(ReactionKind.SteamBurst));
            Assert.IsTrue(ReactivePropRules.IsOneShot(ReactionKind.SparkShower));
            Assert.IsTrue(ReactivePropRules.IsOneShot(ReactionKind.Shatter));
            Assert.AreEqual(3, ReactivePropRules.ShatterChunkCount);
            Assert.AreEqual(30.0, ReactivePropRules.DefaultCooldownSeconds, 1e-9);
        }

        [Test]
        public void LightReaction_KillsPracticalHaloPoolEmissionAndHeroLightTogether()
        {
            var host = new GameObject("ReactivePractical");
            var practical = host.AddComponent<PracticalLight>();
            practical.InitHalo(new Vector3(0f, 0.4f, 0f));
            practical.InitPool(Vector3.zero, Vector3.up, 1.2f);
            Light hero = host.AddComponent<Light>();
            hero.enabled = true;

            var prop = host.AddComponent<ReactiveProp>();
            prop.Configure(ReactionKind.LightFlickerOut);
            Assert.IsTrue(prop.TryReact(0.0, host.transform.position, Vector3.forward));

            Assert.AreEqual(-1, prop.PlayerIndex);
            Assert.IsFalse(practical.IsLit);
            Assert.IsFalse(hero.enabled);
            Assert.IsFalse(prop.IsAlive, "one-shot prop leaves the damage roster after reacting");
            foreach (Renderer renderer in host.GetComponentsInChildren<Renderer>(true)
                         .Where(r => r.name == "PracticalHalo" || r.name == "PracticalPool"))
                Assert.IsFalse(renderer.enabled, renderer.name);
        }

        [Test]
        public void SteamReaction_UsesCooldownAndReturnsLoopingVfxToSharedPool()
        {
            var host = new GameObject("ReactiveSteam");
            var prop = host.AddComponent<ReactiveProp>();
            prop.Configure(ReactionKind.SteamBurst);

            Assert.IsTrue(prop.TryReact(5.0, Vector3.zero, Vector3.up));
            Assert.AreEqual(1, VfxFactory.ActiveCount);
            Assert.IsFalse(prop.TryReact(34.999, Vector3.zero, Vector3.up));
            Assert.IsTrue(prop.StopTimedVfx());
            Assert.AreEqual(0, VfxFactory.ActiveCount);
            Assert.AreEqual(1, VfxFactory.PooledCount);
            Assert.IsTrue(prop.TryReact(35.0, Vector3.zero, Vector3.up));
        }

        [Test]
        public void Shatter_HidesLookDisablesOnlyProxyAndEmitsThreeSharedChunks()
        {
            var host = GameObject.CreatePrimitive(PrimitiveType.Cube);
            host.name = "ReactiveCrate";
            Collider proxy = host.GetComponent<Collider>();
            Renderer look = host.GetComponent<Renderer>();
            var prop = host.AddComponent<ReactiveProp>();
            prop.Configure(ReactionKind.Shatter, proxy);

            Assert.IsTrue(prop.TryReact(0.0, host.transform.position, Vector3.forward));
            Assert.IsFalse(look.enabled);
            Assert.IsFalse(proxy.enabled);
            Assert.AreEqual(ReactivePropRules.ShatterChunkCount, WorldDebrisBudget.LiveCount);
            Assert.IsTrue(Resources.FindObjectsOfTypeAll<WallChunkDebris>().Length >=
                          ReactivePropRules.ShatterChunkCount);
        }

        [Test]
        public void WallAndPropDebrisShareOneTwentyFourObjectRail()
        {
            for (int i = 0; i < WorldDebrisBudget.MaxLiveDebris + 7; i++)
            {
                WorldDebrisBudget.SpawnChunk(
                    new Vector3(i, 0f, 0f),
                    Quaternion.identity,
                    Vector3.one * 0.1f,
                    Vector3.zero,
                    Vector3.zero,
                    null,
                    1f);
            }

            Assert.AreEqual(24, WorldDebrisBudget.MaxLiveDebris);
            Assert.AreEqual(WorldDebrisBudget.MaxLiveDebris, WorldDebrisBudget.LiveCount);
        }

        [Test]
        public void BreakableWall_RegistersThroughSharedDebrisBudget()
        {
            string path = Path.Combine(
                Application.dataPath,
                "Ziptide",
                "Gameplay",
                "Runtime",
                "Pvp",
                "BreakableWall.cs");
            Assert.IsTrue(File.Exists(path), path);
            string source = File.ReadAllText(path);

            StringAssert.Contains("WorldDebrisBudget.Register(go);", source);
            StringAssert.DoesNotContain("Queue<GameObject> _liveDebris", source);
            StringAssert.DoesNotContain("MaxLiveDebris = 24", source);
        }

        [Test]
        public void ReactiveCore_OwnsNoLootSaveNavigationOrMaterialInstances()
        {
            string path = Path.Combine(
                Application.dataPath,
                "Ziptide",
                "Gameplay",
                "Runtime",
                "Pvp",
                "ReactiveProp.cs");
            Assert.IsTrue(File.Exists(path), path);
            string source = File.ReadAllText(path);

            StringAssert.DoesNotContain("Reward", source);
            StringAssert.DoesNotContain("Inventory", source);
            StringAssert.DoesNotContain("PlayerPrefs", source);
            StringAssert.DoesNotContain("NavMesh", source);
            StringAssert.DoesNotContain("new Material(", source);
            StringAssert.DoesNotContain("AddComponent<Light>", source);
            StringAssert.Contains("IPvpDamageable", source);
            StringAssert.Contains("projectileHitProxy.enabled = false", source);
        }
    }
}
#endif
