#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Editor.Patching;
using Ziptide.Gameplay;
using Ziptide.Visuals;

namespace Ziptide.Tests.EditMode
{
    public class ReactivePropAuthorTests
    {
        private readonly List<GameObject> _roots = new List<GameObject>();

        [TearDown]
        public void TearDown()
        {
            foreach (GameObject root in _roots)
                if (root != null) UnityEngine.Object.DestroyImmediate(root);
            _roots.Clear();
        }

        [Test]
        public void ClosedRecipeTable_CoversAllFourReactionKinds()
        {
            var rows = new Dictionary<string, ReactionKind>
            {
                { "light_sconce_wall", ReactionKind.LightFlickerOut },
                { "light_street_pole", ReactionKind.LightFlickerOut },
                { "light_lantern_hang", ReactionKind.LightFlickerOut },
                { SignRecipeLibrary.HangingRecipeId, ReactionKind.SparkShower },
                { SignRecipeLibrary.WallPlateRecipeId, ReactionKind.SparkShower },
                { SignRecipeLibrary.ChevronRecipeId, ReactionKind.SparkShower },
                { "prop_pipe_cluster", ReactionKind.SteamBurst },
                { "prop_patched_crate", ReactionKind.Shatter },
            };

            foreach (var row in rows)
            {
                Assert.IsTrue(ReactivePropAuthor.TryProfile(row.Key, out ReactivePropAuthor.Profile profile), row.Key);
                Assert.AreEqual(row.Value, profile.kind, row.Key);
                Assert.Greater(profile.proxySize.x, 0f, row.Key);
                Assert.Greater(profile.proxySize.y, 0f, row.Key);
                Assert.Greater(profile.proxySize.z, 0f, row.Key);
            }

            foreach (ReactionKind kind in Enum.GetValues(typeof(ReactionKind)))
                CollectionAssert.Contains(rows.Values, kind, kind.ToString());

            Assert.IsFalse(ReactivePropAuthor.TryProfile("flora_reed_w001", out _));
            Assert.IsFalse(ReactivePropAuthor.TryProfile(null, out _));
        }

        [Test]
        public void Place_WiresPracticalSignPipeAndCrateWithoutNewPhysicsOwner()
        {
            GameObject root = Root("ReactiveAuthorRoot");

            GameObject practical = Child(root, "Lantern");
            practical.AddComponent<PracticalLight>();
            GameObject fixture = Child(practical, "LanternFixture");
            ForgeModuleLook lanternLook = Look(fixture, "light_lantern_hang");

            GameObject sign = Child(root, "Sign");
            ForgeModuleLook signLook = Look(sign, SignRecipeLibrary.WallPlateRecipeId);
            signLook.keepChildren = new[] { SignAuthor.GlyphChildName };

            GameObject pipe = Child(root, "Pipe");
            ForgeModuleLook pipeLook = Look(pipe, "prop_pipe_cluster");

            GameObject crate = Child(root, "Crate");
            ForgeModuleLook crateLook = Look(crate, "prop_patched_crate");

            GameObject flora = Child(root, "Flora");
            Look(flora, "flora_reed_w001");

            Assert.AreEqual(4, ReactivePropAuthor.Place(root.transform));

            AssertReaction(practical, lanternLook, ReactionKind.LightFlickerOut);
            AssertReaction(sign, signLook, ReactionKind.SparkShower);
            AssertReaction(pipe, pipeLook, ReactionKind.SteamBurst);
            AssertReaction(crate, crateLook, ReactionKind.Shatter);
            Assert.IsNull(flora.GetComponent<ReactiveProp>());

            CollectionAssert.Contains(signLook.keepChildren, SignAuthor.GlyphChildName);
            CollectionAssert.Contains(signLook.keepChildren, ReactivePropAuthor.HitProxyName);
            CollectionAssert.Contains(lanternLook.keepChildren, ReactivePropAuthor.HitProxyName);

            Assert.AreEqual(0, root.GetComponentsInChildren<Rigidbody>(true).Length,
                "the author adds static hit proxies, never a second physics owner");
            Assert.AreEqual(0, root.GetComponentsInChildren<Light>(true).Length,
                "reactivity consumes existing hero lights but never creates one");
        }

        [Test]
        public void LanternChild_ResolvesReactionToPracticalRoot()
        {
            GameObject root = Root("LanternResolveRoot");
            GameObject practical = Child(root, "PracticalRoot");
            PracticalLight light = practical.AddComponent<PracticalLight>();
            GameObject fixture = Child(practical, "LanternFixture");
            ForgeModuleLook look = Look(fixture, "light_lantern_hang");

            Assert.AreSame(practical, ReactivePropAuthor.ResolveOwner(look));
            Assert.AreEqual(1, ReactivePropAuthor.Place(root.transform));
            Assert.IsNotNull(practical.GetComponent<ReactiveProp>());
            Assert.IsNull(fixture.GetComponent<ReactiveProp>());
            Assert.IsNotNull(light);
        }

        [Test]
        public void Place_IsIdempotentOneComponentAndOneProxyPerOwner()
        {
            GameObject root = Root("ReactiveIdempotentRoot");
            GameObject sign = Child(root, "Sign");
            ForgeModuleLook look = Look(sign, SignRecipeLibrary.HangingRecipeId);

            Assert.AreEqual(1, ReactivePropAuthor.Place(root.transform));
            Assert.AreEqual(1, ReactivePropAuthor.Place(root.transform));
            Assert.AreEqual(1, sign.GetComponents<ReactiveProp>().Length);
            Assert.AreEqual(1, look.transform.Cast<Transform>()
                .Count(child => child.name == ReactivePropAuthor.HitProxyName));
            Assert.AreEqual(1, look.keepChildren.Count(name => name == ReactivePropAuthor.HitProxyName));
        }

        [Test]
        public void AuthorSource_OwnsNoLootInputRigidbodyOrDamageFork()
        {
            string path = Path.Combine(
                Application.dataPath,
                "Ziptide",
                "Editor",
                "Patching",
                "ReactivePropAuthor.cs");
            Assert.IsTrue(File.Exists(path), path);
            string source = File.ReadAllText(path);

            StringAssert.DoesNotContain("Reward", source);
            StringAssert.DoesNotContain("Inventory", source);
            StringAssert.DoesNotContain("XRBaseInteractable", source);
            StringAssert.DoesNotContain("XRSimpleInteractable", source);
            StringAssert.DoesNotContain("AddComponent<Rigidbody>", source);
            StringAssert.DoesNotContain("AddComponent<Light>", source);
            StringAssert.DoesNotContain("interface I", source);
            StringAssert.Contains("owner.AddComponent<ReactiveProp>()", source);
            StringAssert.Contains("proxy.isTrigger = false", source);
        }

        private static void AssertReaction(
            GameObject owner,
            ForgeModuleLook look,
            ReactionKind expected)
        {
            ReactiveProp prop = owner.GetComponent<ReactiveProp>();
            Assert.IsNotNull(prop, owner.name);
            Assert.AreEqual(expected, prop.Kind, owner.name);

            Transform proxyTransform = look.transform.Find(ReactivePropAuthor.HitProxyName);
            Assert.IsNotNull(proxyTransform, owner.name);
            BoxCollider proxy = proxyTransform.GetComponent<BoxCollider>();
            Assert.IsNotNull(proxy, owner.name);
            Assert.IsFalse(proxy.isTrigger, owner.name);
            Assert.AreSame(proxy, prop.ProjectileHitProxy, owner.name);
            Assert.IsNull(proxyTransform.GetComponent<Rigidbody>(), owner.name);
        }

        private GameObject Root(string name)
        {
            var value = new GameObject(name);
            _roots.Add(value);
            return value;
        }

        private static GameObject Child(GameObject parent, string name)
        {
            var value = new GameObject(name);
            value.transform.SetParent(parent.transform, false);
            return value;
        }

        private static ForgeModuleLook Look(GameObject owner, string recipeId)
        {
            ForgeModuleLook look = owner.AddComponent<ForgeModuleLook>();
            look.recipeId = recipeId;
            look.keepChildren = new string[0];
            return look;
        }
    }
}
#endif
