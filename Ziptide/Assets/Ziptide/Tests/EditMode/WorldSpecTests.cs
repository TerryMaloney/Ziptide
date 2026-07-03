using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Content;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// ARCHITECTURE V2 Q1 — the WorldSpec contract: JSON round-trip fidelity + the validator's
    /// actionable-error rules (each failure asserts on its stable CODE token).
    /// </summary>
    public class WorldSpecTests
    {
        // ── A minimal VALID spec every negative test mutates ─────────────────
        private static WorldSpec Valid()
        {
            var s = new WorldSpec
            {
                sceneName = "W099_TestWorld",
                cityId = "test_world",
                displayName = "Test World",
                seed = 4242,
            };
            s.experience.enabled = true;
            s.experience.worldRadius = 300f;
            s.experience.vista = VistaKind.GateSpire;
            s.experience.vistaDirection = new Vector3(0f, 0f, 1f);
            s.pois = new List<PoiDef>
            {
                new PoiDef { id = "camp", type = PoiType.CombatCamp, position = new Vector3(80, 0, 0) },
                new PoiDef { id = "grove", type = PoiType.HarvestGrove, position = new Vector3(-80, 0, 40) },
                new PoiDef { id = "site", type = PoiType.MachineSite, position = new Vector3(0, 0, 120) },
                new PoiDef { id = "ruin", type = PoiType.RuinCache, position = new Vector3(60, 0, -90) },
                new PoiDef { id = "story", type = PoiType.StoryAnchor, position = new Vector3(-40, 0, -130) },
            };
            s.districts = new List<DistrictDef>
            {
                new DistrictDef { id = "hub" },
                new DistrictDef { id = "works", anchor = new Vector3(60, 0, 0) },
            };
            s.connections = new List<ConnectionDef>
            {
                new ConnectionDef { fromDistrictId = "hub", toDistrictId = "works" },
            };
            s.flagsGranted = new List<string> { "W099_COMPLETE" };
            return s;
        }

        private static List<string> Check(WorldSpec s, WorldSpecRegistry reg = null)
            => WorldSpecValidator.Validate(s, reg ?? WorldSpecRegistry.Permissive());

        private static void AssertHas(List<string> issues, string code)
            => Assert.IsTrue(issues.Exists(i => i.StartsWith(code)),
                "expected " + code + " in: [" + string.Join(" | ", issues) + "]");

        // ── Round trip ────────────────────────────────────────────────────────
        [Test]
        public void Json_RoundTrip_IsLossless()
        {
            var a = Valid();
            a.hazards.Add(new HazardZoneDef { id = "wind", kind = HazardKind.Wind, strength = 2.5f });
            a.mines.Add(new MineSpawnDefinition { id = "m1", resourceId = "mineral", ratePerSecond = 0.1, storageCap = 40 });
            var b = WorldSpec.FromJson(a.ToJson());
            Assert.AreEqual(a.ToJson(), b.ToJson(), "spec → json → spec → json must be byte-identical");
            Assert.AreEqual(a.sceneName, b.sceneName);
            Assert.AreEqual(a.pois.Count, b.pois.Count);
            Assert.AreEqual(a.pois[4].type, b.pois[4].type);
            Assert.AreEqual(a.hazards[0].kind, b.hazards[0].kind);
            Assert.AreEqual(a.mines[0].ratePerSecond, b.mines[0].ratePerSecond);
            Assert.AreEqual(a.experience.worldRadius, b.experience.worldRadius);
        }

        [Test]
        public void ValidSpec_HasNoIssues()
        {
            var issues = Check(Valid());
            Assert.IsEmpty(issues, string.Join(" | ", issues));
        }

        // ── Identity rules ────────────────────────────────────────────────────
        [Test]
        public void MissingScene_Cityid_Seed_AreCaught()
        {
            var s = Valid();
            s.sceneName = ""; s.cityId = ""; s.seed = 0;
            var issues = Check(s);
            AssertHas(issues, "SPEC_SCENE_MISSING");
            AssertHas(issues, "SPEC_CITYID_MISSING");
            AssertHas(issues, "SPEC_SEED_ZERO");
        }

        // ── Experience rules ──────────────────────────────────────────────────
        [Test]
        public void TinyWorld_And_MissingVistaDirection_AreCaught()
        {
            var s = Valid();
            s.experience.worldRadius = 40f;
            s.experience.vistaDirection = Vector3.zero;
            var issues = Check(s);
            AssertHas(issues, "SPEC_EXPERIENCE_RADIUS");
            AssertHas(issues, "SPEC_VISTA_DIRECTION");
        }

        // ── POI rules ─────────────────────────────────────────────────────────
        [Test]
        public void PoiCount_And_VerbVariety_Gates_MirrorTheAudit()
        {
            var s = Valid();
            s.pois.RemoveRange(2, 3); // 2 left, 2 verbs
            var issues = Check(s);
            AssertHas(issues, "SPEC_POI_COUNT_LOW");
            AssertHas(issues, "SPEC_POI_VERBS_LOW");
        }

        [Test]
        public void DuplicateAndCrowdedPois_AreCaught()
        {
            var s = Valid();
            s.pois.Add(new PoiDef { id = "camp", type = PoiType.CaveSecret, position = new Vector3(82, 0, 5) }); // dup id + <25m from "camp"
            var issues = Check(s);
            AssertHas(issues, "SPEC_POI_DUPLICATE");
            AssertHas(issues, "SPEC_POI_TOO_CLOSE");
        }

        [Test]
        public void PoiOutsideWorldRadius_IsCaught()
        {
            var s = Valid();
            s.pois[0].position = new Vector3(500f, 0f, 0f);
            AssertHas(Check(s), "SPEC_POI_OUT_OF_BOUNDS");
        }

        [Test]
        public void FlagGrantingWorld_NeedsAStoryAnchor()
        {
            var s = Valid();
            s.pois.RemoveAll(p => p.type == PoiType.StoryAnchor);
            s.pois.Add(new PoiDef { id = "extra", type = PoiType.CaveSecret, position = new Vector3(140, 0, 60) });
            AssertHas(Check(s), "SPEC_STORY_ANCHOR_MISSING");
        }

        // ── Layout rules ──────────────────────────────────────────────────────
        [Test]
        public void UnknownConnection_And_SpawnDistrict_AreCaught()
        {
            var s = Valid();
            s.connections.Add(new ConnectionDef { fromDistrictId = "hub", toDistrictId = "nowhere" });
            s.spawnDistrictId = "alsonowhere";
            var issues = Check(s);
            AssertHas(issues, "SPEC_CONNECTION_UNKNOWN");
            AssertHas(issues, "SPEC_SPAWN_DISTRICT_UNKNOWN");
        }

        [Test]
        public void DuplicateDistrict_IsCaught()
        {
            var s = Valid();
            s.districts.Add(new DistrictDef { id = "hub" });
            AssertHas(Check(s), "SPEC_DISTRICT_DUPLICATE");
        }

        // ── Registry rules ────────────────────────────────────────────────────
        [Test]
        public void UnknownReferences_AreCaught_WithKnownListsInTheMessage()
        {
            var s = Valid();
            s.creatureZones.Add(new CreatureZoneDef { creatureId = "gorgon" });
            s.gardens.Add(new GardenSpawnDefinition { id = "g1", plantId = "triffid" });
            s.collectibles.Add(new CollectibleSpawnDefinition { itemId = "bfg9000" });
            var reg = new WorldSpecRegistry
            {
                CreatureIds = new HashSet<string> { "swarm_bug", "warden" },
                PlantIds = new HashSet<string> { "dew_bulb" },
                ItemIds = new HashSet<string> { "taser_dart_gun" },
            };
            var issues = Check(s, reg);
            AssertHas(issues, "SPEC_CREATURE_UNKNOWN");
            AssertHas(issues, "SPEC_PLANT_UNKNOWN");
            AssertHas(issues, "SPEC_ITEM_UNKNOWN");
            Assert.IsTrue(issues.Exists(i => i.Contains("swarm_bug")), "unknown-creature error must list known ids");
        }

        [Test]
        public void PermissiveRegistry_SkipsReferenceChecks()
        {
            var s = Valid();
            s.creatureZones.Add(new CreatureZoneDef { creatureId = "gorgon" });
            Assert.IsEmpty(Check(s)); // no registry sets → structural rules only
        }

        // ── Economy rules ─────────────────────────────────────────────────────
        [Test]
        public void BadMine_And_Socket_And_DuplicatePackIds_AreCaught()
        {
            var s = Valid();
            s.mines.Add(new MineSpawnDefinition { id = "m1", ratePerSecond = 0, storageCap = 0 });
            s.sockets.Add(new BuildSocketSpawnDefinition { id = "s1", buildCost = 0 });
            s.gardens.Add(new GardenSpawnDefinition { id = "g1" });
            s.gardens.Add(new GardenSpawnDefinition { id = "g1" });
            var issues = Check(s);
            AssertHas(issues, "SPEC_MINE_RATE");
            AssertHas(issues, "SPEC_SOCKET_COST");
            AssertHas(issues, "SPEC_PACK_ID_DUPLICATE");
        }

        // ── The proof shape: an exported world validates ─────────────────────
        [Test]
        public void SpecVersion_IsStamped()
        {
            Assert.AreEqual(1, Valid().specVersion);
            Assert.AreEqual(1, WorldSpec.FromJson(Valid().ToJson()).specVersion);
        }
    }
}
