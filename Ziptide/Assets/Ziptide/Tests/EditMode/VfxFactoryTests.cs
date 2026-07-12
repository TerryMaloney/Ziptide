#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
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
        public void InactiveSystem_CanBeReconfiguredAcrossKindsWithoutGrowingPool()
        {
            ParticleSystem steam = VfxFactory.Spawn("steam_vent", Vector3.zero, Vector3.up);
            Assert.IsTrue(VfxFactory.Stop(steam));

            ParticleSystem impact = VfxFactory.Spawn("impact_stone", Vector3.zero, Vector3.forward);
            Assert.AreSame(steam, impact);
            Assert.IsFalse(impact.main.loop);
            Assert.AreEqual(1, VfxFactory.PooledCount);
        }

        [Test]
        public void OneShotCallback_ReturnsSystemToPool()
        {
            ParticleSystem impact = VfxFactory.Spawn("impact_metal", Vector3.zero, Vector3.up);
            Assert.AreEqual(1, VfxFactory.ActiveCount);

            impact.SendMessage("OnParticleSystemStopped", SendMessageOptions.DontRequireReceiver);

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
        public void DestroyingSceneLocalFactory_ReleasesSharedMaterialTextureAndStaticState()
        {
            ParticleSystem system = VfxFactory.Spawn("motes_spore", Vector3.zero, Vector3.up);
            Assert.IsNotNull(system);

            Material material = system.GetComponent<ParticleSystemRenderer>().sharedMaterial;
            Texture texture = material.mainTexture;
            VfxFactory factory = Resources.FindObjectsOfTypeAll<VfxFactory>().Single();

            Object.DestroyImmediate(factory.gameObject);

            Assert.IsTrue(material == null, "shared runtime material must be destroyed with the factory");
            Assert.IsTrue(texture == null, "shared runtime texture must be destroyed with the factory");
            Assert.AreEqual(0, VfxFactory.ActiveCount);
            Assert.AreEqual(0, VfxFactory.PooledCount);
        }
    }
}
#endif
