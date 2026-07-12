#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Visuals;

namespace Ziptide.Tests.EditMode
{
    public class VfxFactoryTests
    {
        [TearDown]
        public void TearDown()
        {
            foreach (var factory in Resources.FindObjectsOfTypeAll<VfxFactory>())
            {
                if (factory != null && factory.gameObject != null)
                    Object.DestroyImmediate(factory.gameObject);
            }
        }

        [Test]
        public void UnknownId_IsSafeAndDoesNotAllocateFactory()
        {
            Assert.IsNull(VfxFactory.Spawn("missing_vfx", Vector3.zero, Vector3.up));
            Assert.AreEqual(0, VfxFactory.ActiveCount);
            Assert.AreEqual(0, VfxFactory.PooledCount);
        }

        [Test]
        public void KnownRecipe_ConfiguresRealParticleSystemInsideRecipeRails()
        {
            VfxRecipeDefinition recipe = VfxLibrary.Get("impact_metal");
            ParticleSystem system = VfxFactory.Spawn("impact_metal", Vector3.one, Vector3.up);

            Assert.IsNotNull(system);
            Assert.AreEqual(Vector3.one, system.transform.position);

            var main = system.main;
            Assert.IsFalse(main.loop);
            Assert.AreEqual(ParticleSystemStopAction.Callback, main.stopAction);
            Assert.AreEqual(recipe.PeakParticles(), main.maxParticles);
            Assert.LessOrEqual(main.maxParticles, VfxRecipeDefinition.MaxParticles);

            var emission = system.emission;
            Assert.AreEqual(1, emission.burstCount);
            var bursts = new ParticleSystem.Burst[emission.burstCount];
            Assert.AreEqual(1, emission.GetBursts(bursts));
            Assert.AreEqual(recipe.burstCount, bursts[0].maxCount);

            Assert.IsTrue(system.colorOverLifetime.enabled);
            Assert.IsTrue(system.sizeOverLifetime.enabled);
            Assert.IsTrue(system.shape.enabled);

            var renderer = system.GetComponent<ParticleSystemRenderer>();
            Assert.IsNotNull(renderer);
            Assert.AreNotEqual(ParticleSystemRenderMode.Mesh, renderer.renderMode);
            Assert.IsFalse(renderer.receiveShadows);
            Assert.IsNotNull(renderer.sharedMaterial);
            Assert.IsNotNull(renderer.sharedMaterial.mainTexture);

            Assert.IsFalse(system.collision.enabled);
            Assert.IsFalse(system.trails.enabled);
            Assert.IsFalse(system.lights.enabled);
            Assert.IsFalse(system.subEmitters.enabled);
        }

        [Test]
        public void SurfaceNormal_DeterminesEmitterForward()
        {
            Vector3 normal = new Vector3(1f, 2f, -3f).normalized;
            ParticleSystem system = VfxFactory.Spawn("sparks_short", Vector3.zero, normal);

            Assert.IsNotNull(system);
            Assert.Less(Vector3.Angle(system.transform.forward, normal), 0.01f);
        }

        [Test]
        public void ExplicitStop_ReturnsLoopingSystemAndSameKindReusesIt()
        {
            ParticleSystem first = VfxFactory.Spawn("steam_vent", Vector3.zero, Vector3.up);
            Assert.IsNotNull(first);
            Assert.IsTrue(first.main.loop);
            Assert.AreEqual(1, VfxFactory.ActiveCount);
            Assert.AreEqual(1, VfxFactory.PooledCount);

            Assert.IsTrue(VfxFactory.Stop(first));
            Assert.AreEqual(0, VfxFactory.ActiveCount);
            Assert.IsFalse(first.gameObject.activeSelf);

            ParticleSystem second = VfxFactory.Spawn("steam_vent", Vector3.right, Vector3.up);
            Assert.AreSame(first, second);
            Assert.AreEqual(1, VfxFactory.ActiveCount);
            Assert.AreEqual(1, VfxFactory.PooledCount);
        }

        [Test]
        public void FullPool_RecyclesInactiveSystemAcrossKindsWithoutGrowing()
        {
            var systems = new List<ParticleSystem>();
            for (int i = 0; i < VfxRecipeDefinition.MaxLiveSystems; i++)
            {
                ParticleSystem system = VfxFactory.Spawn(
                    "steam_vent",
                    new Vector3(i, 0f, 0f),
                    Vector3.up);
                Assert.IsNotNull(system);
                systems.Add(system);
            }

            ParticleSystem retired = systems[0];
            Assert.IsTrue(VfxFactory.Stop(retired));
            Assert.AreEqual(VfxRecipeDefinition.MaxLiveSystems - 1, VfxFactory.ActiveCount);

            ParticleSystem impact = VfxFactory.Spawn("impact_stone", Vector3.zero, Vector3.forward);
            Assert.AreSame(retired, impact);
            Assert.IsFalse(impact.main.loop);
            Assert.AreEqual(VfxRecipeDefinition.MaxLiveSystems, VfxFactory.PooledCount);
            Assert.AreEqual(VfxRecipeDefinition.MaxLiveSystems, VfxFactory.ActiveCount);

            Assert.IsTrue(VfxFactory.Stop(impact));
            for (int i = 1; i < systems.Count; i++)
                Assert.IsTrue(VfxFactory.Stop(systems[i]));
        }

        [Test]
        public void OneShotCallback_ReturnsSystemToPool()
        {
            ParticleSystem impact = VfxFactory.Spawn("impact_metal", Vector3.zero, Vector3.up);
            Assert.AreEqual(1, VfxFactory.ActiveCount);

            Component pooled = impact.GetComponent("VfxPooledInstance");
            Assert.IsNotNull(pooled);
            MethodInfo callback = pooled.GetType().GetMethod(
                "OnParticleSystemStopped",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(callback);
            callback.Invoke(pooled, null);

            Assert.AreEqual(0, VfxFactory.ActiveCount);
            Assert.IsFalse(impact.gameObject.activeSelf);
            Assert.AreEqual(1, VfxFactory.PooledCount);
        }

        [Test]
        public void SeventhConcurrentSpawn_IsDroppedAndPoolNeverExceedsSix()
        {
            var systems = new List<ParticleSystem>();
            for (int i = 0; i < VfxRecipeDefinition.MaxLiveSystems; i++)
            {
                ParticleSystem system = VfxFactory.Spawn(
                    "steam_vent",
                    new Vector3(i, 0f, 0f),
                    Vector3.up);
                Assert.IsNotNull(system, "spawn " + i);
                systems.Add(system);
            }

            Assert.AreEqual(VfxRecipeDefinition.MaxLiveSystems, VfxFactory.ActiveCount);
            Assert.AreEqual(VfxRecipeDefinition.MaxLiveSystems, VfxFactory.PooledCount);
            Assert.IsNull(VfxFactory.Spawn("motes_amber", Vector3.zero, Vector3.up));
            Assert.AreEqual(VfxRecipeDefinition.MaxLiveSystems, VfxFactory.ActiveCount);
            Assert.AreEqual(VfxRecipeDefinition.MaxLiveSystems, VfxFactory.PooledCount);

            foreach (ParticleSystem system in systems)
                Assert.IsTrue(VfxFactory.Stop(system));
            Assert.AreEqual(0, VfxFactory.ActiveCount);
            Assert.AreEqual(VfxRecipeDefinition.MaxLiveSystems, VfxFactory.PooledCount);
        }

        [Test]
        public void DestroyingSceneLocalFactory_ResetsStateAndSourceOwnsSharedCleanup()
        {
            ParticleSystem system = VfxFactory.Spawn("motes_spore", Vector3.zero, Vector3.up);
            Assert.IsNotNull(system);
            Assert.IsNotNull(system.GetComponent<ParticleSystemRenderer>().sharedMaterial);

            VfxFactory factory = Resources.FindObjectsOfTypeAll<VfxFactory>().Single();
            Object.DestroyImmediate(factory.gameObject);

            Assert.AreEqual(0, VfxFactory.ActiveCount);
            Assert.AreEqual(0, VfxFactory.PooledCount);

            string path = Path.Combine(
                Application.dataPath,
                "Ziptide",
                "Visuals",
                "Runtime",
                "Vfx",
                "VfxFactory.cs");
            Assert.IsTrue(File.Exists(path), path);
            string source = File.ReadAllText(path);
            StringAssert.Contains("DestroyObject(_sharedMaterial);", source);
            StringAssert.Contains("DestroyObject(_sharedTexture);", source);
            StringAssert.Contains("if (Application.isPlaying) Destroy(value);", source);
            StringAssert.Contains("else DestroyImmediate(value);", source);
        }
    }
}
#endif
