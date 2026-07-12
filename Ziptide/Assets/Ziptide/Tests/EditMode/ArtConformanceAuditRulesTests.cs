#if UNITY_EDITOR
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Editor.Audit;
using Ziptide.Visuals;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// FORGE III F3.9 — the conformance ratchet's pure logic: provenance classification, the
    /// whitelist's WHY-required parser, and the lock decision. These prove the machine that keeps
    /// art quality EVEN across lanes behaves exactly as designed (and can never over-block).
    /// </summary>
    public class ArtConformanceAuditRulesTests
    {
        private readonly List<GameObject> _spawned = new List<GameObject>();

        private GameObject New(string name, Transform parent = null)
        {
            var go = new GameObject(name);
            if (parent != null) go.transform.SetParent(parent);
            _spawned.Add(go);
            return go;
        }

        [TearDown]
        public void Cleanup()
        {
            foreach (var go in _spawned)
                if (go != null) Object.DestroyImmediate(go);
            _spawned.Clear();
        }

        // ── provenance classification ──────────────────────────────────────────────────────────

        [Test]
        public void IsConformed_TrueUnderForgeVisualChild()
        {
            var root = New("Wall");
            var forge = New(ForgeVisualApplier.VisualChildName, root.transform);
            var mesh = New("bakedMesh", forge.transform);
            Assert.IsTrue(ArtConformanceAuditRules.IsConformed(mesh.transform, null, "W002"));
        }

        [Test]
        public void IsConformed_TrueUnderMarkerComponent()
        {
            var module = New("KitModule");
            module.AddComponent<ForgeModuleLook>(); // Awake does not fire in EditMode; safe no-op even if it did
            var child = New("primitive", module.transform);
            Assert.IsTrue(ArtConformanceAuditRules.IsConformed(child.transform, null, "W002"));
        }

        [Test]
        public void IsConformed_FalseForBareRenderer()
        {
            var group = New("Misc");
            var bare = New("StrayCube", group.transform);
            Assert.IsFalse(ArtConformanceAuditRules.IsConformed(bare.transform, null, "W002"));
        }

        [Test]
        public void IsConformed_TrueForWhitelistedNameInThatScene_ButNotAnother()
        {
            var gizmo = New("DebugGizmo");
            var keys = new HashSet<string> { "W002:DebugGizmo" };
            Assert.IsTrue(ArtConformanceAuditRules.IsConformed(gizmo.transform, keys, "W002"),
                "whitelisted in W002");
            Assert.IsFalse(ArtConformanceAuditRules.IsConformed(gizmo.transform, keys, "W005"),
                "the same object name is NOT exempt in a different scene");
        }

        // ── whitelist parser: WHY is mandatory ─────────────────────────────────────────────────

        [Test]
        public void ParseWhitelist_AcceptsEntryWithWhy_RejectsBareEntry()
        {
            var lines = new[]
            {
                "# a header comment",
                "",
                "W002:DebugGizmo   # ships as a dev aid, intentionally un-styled",
                "W002:SneakyCube", // no WHY → rejected
            };
            var keys = ArtConformanceAuditRules.ParseWhitelistKeys(lines, out var rejected);

            Assert.IsTrue(keys.Contains("W002:DebugGizmo"));
            Assert.IsFalse(keys.Contains("W002:SneakyCube"));
            CollectionAssert.Contains(rejected, "W002:SneakyCube");
        }

        [Test]
        public void ParseWhitelist_RejectsEmptyWhyOrMissingColon()
        {
            var lines = new[]
            {
                "W002:EmptyWhy   #   ",  // WHY is blank → rejected
                "NoColonHere   # has a why but no scene:object key",
            };
            var keys = ArtConformanceAuditRules.ParseWhitelistKeys(lines, out var rejected);

            Assert.AreEqual(0, keys.Count, "neither malformed line should become a key");
            Assert.AreEqual(2, rejected.Count);
        }

        // ── the ratchet decision ───────────────────────────────────────────────────────────────

        [Test]
        public void ShouldBlock_OnlyWhenLockedWorldRegresses()
        {
            Assert.IsFalse(ArtConformanceAuditRules.ShouldBlock(0, false), "clean unlocked = fine");
            Assert.IsFalse(ArtConformanceAuditRules.ShouldBlock(3, false), "debt in unlocked = WARN, not block");
            Assert.IsFalse(ArtConformanceAuditRules.ShouldBlock(0, true),  "clean locked = fine");
            Assert.IsTrue(ArtConformanceAuditRules.ShouldBlock(3, true),   "regression in a locked world BLOCKS");
        }

        [Test]
        public void IsWorldLocked_MatchesInjectedList()
        {
            var locked = new[] { "W002" };
            Assert.IsTrue(ArtConformanceAuditRules.IsWorldLocked("W002", locked));
            Assert.IsFalse(ArtConformanceAuditRules.IsWorldLocked("W005", locked));
            Assert.IsFalse(ArtConformanceAuditRules.IsWorldLocked("W002", null));
        }

        [Test]
        public void LockedWorlds_StartsEmpty_SoNothingCanBlockYet()
        {
            Assert.AreEqual(0, ArtConformanceAuditRules.ConformanceLockedWorlds.Length,
                "the ratchet starts fully open — the rule is WARN-only for every world until a world " +
                "is proven at 0 and explicitly added to the locked list.");
        }
    }
}
#endif
