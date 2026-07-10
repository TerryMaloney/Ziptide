#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Ziptide.Visuals;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// Code-authored <see cref="ForgeCreatureBody"/> assets — THE STUDIO'S CREATURE BOOK (FORGE II
    /// P3 genomes; the skinning/gait cores turn these into walking bodies). Asset ids MATCH
    /// CreatureDefinition ids (Resources/Enemies) on purpose: CreatureBehaviorBase asks
    /// ForgeCreatureVisualApplier by creatureId, so authoring a body here upgrades every spawn of
    /// that creature in every world with zero zone-data edits. CREATE-ONLY like ForgeRecipeLibrary:
    /// an existing asset under Resources/Forge/Bodies is the live, editable truth; delete it to
    /// reseed from code. TO ADD A CREATURE: write a Build* method (start from BuildSwarmBug),
    /// keep Validate() empty and bones ≤ 12 — the tests pin both.
    /// </summary>
    public static class ForgeBodyLibrary
    {
        public const string BodyFolder = "Assets/Ziptide/Resources/Forge/Bodies";

        [MenuItem("Ziptide/Art/Author Forge Creature Bodies (missing only)")]
        public static void AuthorFromMenu()
        {
            int made = EnsureAllAuthored();
            EditorUtility.DisplayDialog("Forge Body Library",
                made + " body asset(s) created under " + BodyFolder + " (existing ones untouched).", "OK");
        }

        /// <summary>creatureId → genome builder. Pure; test-inspectable.</summary>
        public static List<KeyValuePair<string, System.Func<ForgeCreatureBody>>> Specs()
        {
            return new List<KeyValuePair<string, System.Func<ForgeCreatureBody>>>
            {
                Spec("swarm_bug", BuildSwarmBug),
                Spec("light_grazer", BuildLightGrazer),
                Spec("tendril", BuildTendril),
                Spec("witness_mite", BuildWitnessMite),
                Spec("husk_molter", BuildHuskMolter),
                Spec("warden", BuildWarden),
                // tether_swarm is INTENTIONALLY not forged: its whole encounter IS procedural —
                // colliderless bug clusters weaving around the ONE hittable glowing node on the
                // cord (the lesson the player learns by shooting bugs uselessly). A single skinned
                // body would erase that read. The behavior is the art; do not "complete" this row.
            };
        }

        /// <summary>
        /// tendril (W003 glass shelf) — the wall-crawling creeper: a gnarled root-knot (P2 noised
        /// OrganicBlobs — the first genome to use the organic ops) topped by a pale spore bulb, with
        /// two long 3-segment grasping tendrils forward and two shorter anchors behind (Tentacle role —
        /// the propagating wave is the "it's ALIVE on that wall" read). One sickly yellow-green eye.
        /// Bones: 1 + (2×3 + 2×2 mirrored) = 11 ≤ 12.
        /// </summary>
        private static ForgeCreatureBody BuildTendril()
        {
            var b = ScriptableObject.CreateInstance<ForgeCreatureBody>();
            b.bodyId = "tendril";
            b.palette = new[]
            {
                new Color(0.20f, 0.14f, 0.22f), // 0 tendrils — dark violet-brown creeper
                new Color(0.22f, 0.28f, 0.20f), // 1 root-knot — mossy dark green
                new Color(0.72f, 0.70f, 0.52f), // 2 spore bulb — pale sick cream
                new Color(0.75f, 0.95f, 0.35f), // 3 eye — sickly yellow-green glow
            };
            b.slotStyles = new[]
            {
                new ForgeStyleSpec { style = ForgeStyle.Bark, grime = 0.55f, wear = 0.2f },
                new ForgeStyleSpec { style = ForgeStyle.Bark, grime = 0.6f, wear = 0.25f },
                new ForgeStyleSpec { style = ForgeStyle.Slime, grime = 0.3f },
                new ForgeStyleSpec { style = ForgeStyle.GlowPanel, emissive = new Color(0.75f, 0.95f, 0.35f), emissiveIntensity = 1.3f },
            };
            b.coreParts = new[]
            {
                new ForgePart { name = "RootKnot", op = ForgeOp.OrganicBlob, segments = 18, smooth = true,
                    size = new Vector3(0.30f, 0.22f, 0.30f), position = new Vector3(0f, 0.16f, 0f),
                    noiseAmplitude = 0.02f, noiseFrequency = 12f, noiseSeed = 9, paletteSlot = 1 },
                new ForgePart { name = "SporeBulb", op = ForgeOp.OrganicBlob, segments = 14, smooth = true,
                    size = new Vector3(0.16f, 0.14f, 0.16f), position = new Vector3(0f, 0.30f, -0.06f),
                    noiseAmplitude = 0.012f, noiseFrequency = 16f, noiseSeed = 21, paletteSlot = 2 },
            };
            b.limbs = new[]
            {
                new ForgeLimb
                {
                    name = "TendrilFront", attachLocal = new Vector3(0.10f, 0.14f, 0.10f),
                    chainDirection = new Vector3(0.6f, -0.5f, 0.5f), role = GaitRole.Tentacle, mirrorX = true,
                    segments = new[]
                    {
                        new ForgeLimbSegment { size = new Vector3(0.032f, 0.18f, 0.032f), paletteSlot = 0, rounded = true, taper = 0.2f },
                        new ForgeLimbSegment { size = new Vector3(0.024f, 0.16f, 0.024f), paletteSlot = 0, rounded = true, taper = 0.25f, bendDegrees = 35f },
                        new ForgeLimbSegment { size = new Vector3(0.016f, 0.14f, 0.016f), paletteSlot = 0, rounded = true, taper = 0.6f, bendDegrees = 45f },
                    }
                },
                new ForgeLimb
                {
                    name = "TendrilRear", attachLocal = new Vector3(0.10f, 0.14f, -0.10f),
                    chainDirection = new Vector3(0.65f, -0.5f, -0.45f), role = GaitRole.Tentacle, mirrorX = true,
                    segments = new[]
                    {
                        new ForgeLimbSegment { size = new Vector3(0.030f, 0.16f, 0.030f), paletteSlot = 0, rounded = true, taper = 0.25f },
                        new ForgeLimbSegment { size = new Vector3(0.020f, 0.14f, 0.020f), paletteSlot = 0, rounded = true, taper = 0.55f, bendDegrees = 40f },
                    }
                },
            };
            b.eyeLocal = new Vector3(0f, 0.22f, 0.16f);
            b.eyeRadius = 0.035f;
            b.eyePaletteSlot = 3;
            return b;
        }

        /// <summary>Create any missing body assets. Returns how many were created.</summary>
        public static int EnsureAllAuthored()
        {
            int made = 0;
            foreach (var spec in Specs())
            {
                string path = BodyFolder + "/" + spec.Key + ".asset";
                if (AssetDatabase.LoadAssetAtPath<ForgeCreatureBody>(path) != null) continue;
                Directory.CreateDirectory(BodyFolder);
                var body = spec.Value();
                AssetDatabase.CreateAsset(body, path);
                Debug.Log("[Ziptide] ForgeBodyLibrary authored " + path);
                made++;
            }
            if (made > 0) { AssetDatabase.SaveAssets(); AssetDatabase.Refresh(); }
            return made;
        }

        private static KeyValuePair<string, System.Func<ForgeCreatureBody>> Spec(
            string id, System.Func<ForgeCreatureBody> builder)
            => new KeyValuePair<string, System.Func<ForgeCreatureBody>>(id, builder);

        // ── Genomes ────────────────────────────────────────────────────────────────────────────────

        /// <summary>
        /// swarm_bug (W005 canopy / W009 chitin wall) — a squat six-legged chitin skitterer.
        /// Six 1-segment legs (no knee — the scuttle reads through the hip swing), a pair of
        /// antennae that sway in the air, amber carapace over dark legs, one hot amber eye.
        /// 1 + 6 + 2 = 9 bones.
        /// </summary>
        private static ForgeCreatureBody BuildSwarmBug()
        {
            var b = ScriptableObject.CreateInstance<ForgeCreatureBody>();
            b.bodyId = "swarm_bug";
            b.palette = new[]
            {
                new Color(0.16f, 0.12f, 0.08f), // 0 legs — dark chitin
                new Color(0.55f, 0.38f, 0.16f), // 1 carapace — amber
                new Color(0.35f, 0.24f, 0.12f), // 2 underside — worn amber-brown
                new Color(1.00f, 0.55f, 0.15f), // 3 eye — hot amber glow
            };
            b.slotStyles = new[]
            {
                new ForgeStyleSpec { style = ForgeStyle.Chitin, cellSize = 0.035f, grime = 0.55f, wear = 0.25f },
                new ForgeStyleSpec { style = ForgeStyle.Chitin, cellSize = 0.06f, grime = 0.4f, wear = 0.35f },
                new ForgeStyleSpec { style = ForgeStyle.Chitin, cellSize = 0.045f, grime = 0.6f },
                new ForgeStyleSpec { style = ForgeStyle.GlowPanel, emissive = new Color(1f, 0.55f, 0.15f), emissiveIntensity = 1.4f },
            };
            b.coreParts = new[]
            {
                new ForgePart { name = "Carapace", op = ForgeOp.OrganicBlob, segments = 22, smooth = true,
                    size = new Vector3(0.34f, 0.16f, 0.42f), position = new Vector3(0f, 0.18f, 0f),
                    noiseAmplitude = 0.010f, noiseFrequency = 13f, noiseSeed = 2, paletteSlot = 1 },
                new ForgePart { name = "Belly", op = ForgeOp.SphereSection, bevel = 1f, segments = 14,
                    smooth = true, size = new Vector3(0.26f, 0.10f, 0.32f), position = new Vector3(0f, 0.12f, 0f), paletteSlot = 2 },
                new ForgePart { name = "Head", op = ForgeOp.OrganicBlob, segments = 14, smooth = true,
                    size = new Vector3(0.16f, 0.12f, 0.16f), position = new Vector3(0f, 0.17f, 0.22f),
                    noiseAmplitude = 0.006f, noiseFrequency = 18f, noiseSeed = 4, paletteSlot = 1 },
                new ForgePart { name = "RidgeL", op = ForgeOp.Wedge,
                    size = new Vector3(0.025f, 0.035f, 0.26f), position = new Vector3(0.06f, 0.245f, -0.02f),
                    eulerRotation = new Vector3(-6f, 0f, -14f), paletteSlot = 0 },
                new ForgePart { name = "RidgeR", op = ForgeOp.Wedge,
                    size = new Vector3(0.025f, 0.035f, 0.26f), position = new Vector3(-0.06f, 0.245f, -0.02f),
                    eulerRotation = new Vector3(-6f, 0f, 14f), paletteSlot = 0 },
            };
            b.limbs = new[]
            {
                Leg("LegFront", new Vector3(0.13f, 0.15f, 0.14f), new Vector3(0.85f, -1f, 0.25f)),
                Leg("LegMid", new Vector3(0.15f, 0.15f, 0f), new Vector3(1f, -1f, 0f)),
                Leg("LegRear", new Vector3(0.13f, 0.15f, -0.14f), new Vector3(0.85f, -1f, -0.25f)),
                new ForgeLimb
                {
                    name = "Antenna", attachLocal = new Vector3(0.05f, 0.22f, 0.28f),
                    chainDirection = new Vector3(0.25f, 0.7f, 0.65f), role = GaitRole.Antenna, mirrorX = true,
                    segments = new[] { new ForgeLimbSegment { size = new Vector3(0.014f, 0.15f, 0.014f),
                        paletteSlot = 0, rounded = true, taper = 0.55f } }
                },
            };
            b.eyeLocal = new Vector3(0f, 0.19f, 0.30f);
            b.eyeRadius = 0.03f;
            b.eyePaletteSlot = 3;
            return b;
        }

        private static ForgeLimb Leg(string name, Vector3 attach, Vector3 dir) => new ForgeLimb
        {
            name = name, attachLocal = attach, chainDirection = dir, role = GaitRole.Leg, mirrorX = true,
            segments = new[] { new ForgeLimbSegment { size = new Vector3(0.028f, 0.20f, 0.032f),
                paletteSlot = 0, rounded = true, taper = 0.35f } }
        };

        /// <summary>
        /// light_grazer (W002 dry cistern) — the pale drifting grazer: a soft luminous bell with
        /// four 2-segment tentacles trailing under it (Tentacle role — the traveling wave does the
        /// "alive in the dark" read) and one large soft-green eye. 1 + 8 = 9 bones.
        /// </summary>
        private static ForgeCreatureBody BuildLightGrazer()
        {
            var b = ScriptableObject.CreateInstance<ForgeCreatureBody>();
            b.bodyId = "light_grazer";
            b.palette = new[]
            {
                new Color(0.30f, 0.42f, 0.32f), // 0 tentacles — muted moss
                new Color(0.62f, 0.78f, 0.58f), // 1 bell — pale luminous green
                new Color(0.45f, 0.60f, 0.45f), // 2 underbell
                new Color(0.55f, 1.00f, 0.60f), // 3 eye — soft green glow
            };
            b.slotStyles = new[]
            {
                // v4 loop: grime drives the new Slime subdermal mottle — lifted so the pale hide
                // reads as a body with something INSIDE it, not a smooth balloon.
                new ForgeStyleSpec { style = ForgeStyle.Slime, grime = 0.45f, wear = 0.15f },
                new ForgeStyleSpec { style = ForgeStyle.Slime, grime = 0.35f, wear = 0.1f },
                // v5 loop: it's called a LIGHT grazer — the underbell + skirt become the LANTERN
                // (soft green glow under the bell; the eye stays the brighter point above it).
                new ForgeStyleSpec { style = ForgeStyle.GlowPanel, emissive = new Color(0.5f, 0.9f, 0.55f), emissiveIntensity = 0.9f },
                new ForgeStyleSpec { style = ForgeStyle.GlowPanel, emissive = new Color(0.55f, 1f, 0.6f), emissiveIntensity = 1.3f },
            };
            b.coreParts = new[]
            {
                new ForgePart { name = "Bell", op = ForgeOp.OrganicBlob, segments = 24, smooth = true,
                    size = new Vector3(0.40f, 0.34f, 0.40f), position = new Vector3(0f, 0.34f, 0f),
                    noiseAmplitude = 0.016f, noiseFrequency = 9f, noiseSeed = 7, paletteSlot = 1 },
                new ForgePart { name = "Underbell", op = ForgeOp.SphereSection, bevel = 1f, segments = 16,
                    smooth = true, size = new Vector3(0.30f, 0.16f, 0.30f), position = new Vector3(0f, 0.20f, 0f), paletteSlot = 2 },
                new ForgePart { name = "Skirt", op = ForgeOp.Torus, segments = 20, smooth = true,
                    size = new Vector3(0.36f, 0.05f, 0.01f), position = new Vector3(0f, 0.23f, 0f), paletteSlot = 2 },
            };
            b.limbs = new[]
            {
                new ForgeLimb
                {
                    name = "TentacleFront", attachLocal = new Vector3(0.11f, 0.18f, 0.09f),
                    chainDirection = new Vector3(0.3f, -1f, 0.18f), role = GaitRole.Tentacle, mirrorX = true,
                    segments = new[]
                    {
                        new ForgeLimbSegment { size = new Vector3(0.038f, 0.16f, 0.038f), paletteSlot = 0, rounded = true, taper = 0.25f },
                        new ForgeLimbSegment { size = new Vector3(0.024f, 0.14f, 0.024f), paletteSlot = 0, rounded = true, taper = 0.5f, bendDegrees = 28f },
                    }
                },
                new ForgeLimb
                {
                    name = "TentacleRear", attachLocal = new Vector3(0.11f, 0.18f, -0.09f),
                    chainDirection = new Vector3(0.35f, -1f, -0.22f), role = GaitRole.Tentacle, mirrorX = true,
                    segments = new[]
                    {
                        new ForgeLimbSegment { size = new Vector3(0.038f, 0.16f, 0.038f), paletteSlot = 0, rounded = true, taper = 0.25f },
                        new ForgeLimbSegment { size = new Vector3(0.024f, 0.14f, 0.024f), paletteSlot = 0, rounded = true, taper = 0.5f, bendDegrees = 28f },
                    }
                },
            };
            b.eyeLocal = new Vector3(0f, 0.36f, 0.19f);
            b.eyeRadius = 0.045f;
            b.eyePaletteSlot = 3;
            return b;
        }

        /// <summary>
        /// witness_mite (W004 mirror flats) — the weeping-angel scuttler: a dusky-rose noised blob
        /// hunched behind a big pale lens (the thing that stares back). Six 1-segment legs for the
        /// between-glances skitter. Its gameplay tell (stone-freeze while observed) rides the
        /// ForgeBodyTell body-tint channel. 1 + 6 = 7 bones.
        /// </summary>
        private static ForgeCreatureBody BuildWitnessMite()
        {
            var b = ScriptableObject.CreateInstance<ForgeCreatureBody>();
            b.bodyId = "witness_mite";
            b.palette = new[]
            {
                new Color(0.30f, 0.16f, 0.24f), // 0 legs — dark dried-rose chitin
                new Color(0.65f, 0.30f, 0.45f), // 1 body — dusky rose (the behavior's MoveColor)
                new Color(0.45f, 0.22f, 0.33f), // 2 lens housing
                new Color(0.88f, 0.84f, 0.95f), // 3 eye — pale watching white
            };
            b.slotStyles = new[]
            {
                new ForgeStyleSpec { style = ForgeStyle.Chitin, cellSize = 0.03f, grime = 0.45f, wear = 0.2f },
                new ForgeStyleSpec { style = ForgeStyle.Slime, grime = 0.3f, wear = 0.15f },
                new ForgeStyleSpec { style = ForgeStyle.Slime, grime = 0.35f },
                new ForgeStyleSpec { style = ForgeStyle.GlowPanel, emissive = new Color(0.88f, 0.84f, 0.95f), emissiveIntensity = 1.5f },
            };
            b.coreParts = new[]
            {
                new ForgePart { name = "Body", op = ForgeOp.OrganicBlob, segments = 16, smooth = true,
                    size = new Vector3(0.30f, 0.20f, 0.36f), position = new Vector3(0f, 0.16f, 0f),
                    noiseAmplitude = 0.012f, noiseFrequency = 14f, noiseSeed = 5, paletteSlot = 1 },
                new ForgePart { name = "LensHousing", op = ForgeOp.SphereSection, bevel = 1f, segments = 14,
                    smooth = true, size = new Vector3(0.15f, 0.13f, 0.15f),
                    position = new Vector3(0f, 0.20f, 0.16f), paletteSlot = 2 },
                new ForgePart { name = "Spine1", op = ForgeOp.Capsule, segments = 8, smooth = true, taper = 0.7f,
                    size = new Vector3(0.030f, 0.12f, 0.030f), position = new Vector3(0f, 0.27f, -0.04f),
                    eulerRotation = new Vector3(-20f, 0f, 0f), paletteSlot = 0 },
                new ForgePart { name = "Spine2", op = ForgeOp.Capsule, segments = 8, smooth = true, taper = 0.7f,
                    size = new Vector3(0.024f, 0.10f, 0.024f), position = new Vector3(0.06f, 0.24f, -0.09f),
                    eulerRotation = new Vector3(-30f, 0f, 12f), paletteSlot = 0 },
                new ForgePart { name = "Spine3", op = ForgeOp.Capsule, segments = 8, smooth = true, taper = 0.7f,
                    size = new Vector3(0.024f, 0.10f, 0.024f), position = new Vector3(-0.06f, 0.24f, -0.09f),
                    eulerRotation = new Vector3(-30f, 0f, -12f), paletteSlot = 0 },
            };
            b.limbs = new[]
            {
                MiteLeg("LegFront", new Vector3(0.11f, 0.13f, 0.12f), new Vector3(0.85f, -1f, 0.3f)),
                MiteLeg("LegMid", new Vector3(0.13f, 0.13f, 0f), new Vector3(1f, -1f, 0f)),
                MiteLeg("LegRear", new Vector3(0.11f, 0.13f, -0.12f), new Vector3(0.85f, -1f, -0.3f)),
            };
            b.eyeLocal = new Vector3(0f, 0.21f, 0.24f);
            b.eyeRadius = 0.05f; // the big lens — being seen back IS the creature
            b.eyePaletteSlot = 3;
            return b;
        }

        private static ForgeLimb MiteLeg(string name, Vector3 attach, Vector3 dir) => new ForgeLimb
        {
            name = name, attachLocal = attach, chainDirection = dir, role = GaitRole.Leg, mirrorX = true,
            segments = new[] { new ForgeLimbSegment { size = new Vector3(0.024f, 0.16f, 0.028f),
                paletteSlot = 0, rounded = true, taper = 0.3f } }
        };

        /// <summary>
        /// husk_molter (W009 chitin wall) — the shed-skin trickster: a mossy noised carapace with a
        /// dark dorsal ridge fin and four 2-segment legs. Its molt decoy is a frozen grey clone of
        /// THIS body (ForgeBodyTell.TryCloneStatue) — the forge made the trick stronger, not weaker.
        /// 1 + 8 = 9 bones.
        /// </summary>
        private static ForgeCreatureBody BuildHuskMolter()
        {
            var b = ScriptableObject.CreateInstance<ForgeCreatureBody>();
            b.bodyId = "husk_molter";
            b.palette = new[]
            {
                new Color(0.20f, 0.26f, 0.17f), // 0 legs + ridge — dark moss chitin
                new Color(0.35f, 0.45f, 0.30f), // 1 carapace — mossy green (the behavior's BodyColor)
                new Color(0.55f, 0.58f, 0.45f), // 2 belly — pale shed-ready lining
                new Color(1.00f, 0.60f, 0.20f), // 3 eye — amber
            };
            b.slotStyles = new[]
            {
                new ForgeStyleSpec { style = ForgeStyle.Chitin, cellSize = 0.05f, grime = 0.55f, wear = 0.3f },
                new ForgeStyleSpec { style = ForgeStyle.Chitin, cellSize = 0.07f, grime = 0.65f, wear = 0.2f },
                new ForgeStyleSpec { style = ForgeStyle.Slime, grime = 0.4f },
                new ForgeStyleSpec { style = ForgeStyle.GlowPanel, emissive = new Color(1f, 0.6f, 0.2f), emissiveIntensity = 1.4f },
            };
            b.coreParts = new[]
            {
                new ForgePart { name = "Carapace", op = ForgeOp.OrganicBlob, segments = 16, smooth = true,
                    size = new Vector3(0.34f, 0.24f, 0.40f), position = new Vector3(0f, 0.18f, 0f),
                    noiseAmplitude = 0.015f, noiseFrequency = 10f, noiseSeed = 3, paletteSlot = 1 },
                new ForgePart { name = "Belly", op = ForgeOp.SphereSection, bevel = 1f, segments = 12,
                    smooth = true, size = new Vector3(0.26f, 0.12f, 0.32f),
                    position = new Vector3(0f, 0.12f, 0f), paletteSlot = 2 },
                new ForgePart { name = "RidgeFin", op = ForgeOp.Wedge, segments = 4,
                    size = new Vector3(0.05f, 0.13f, 0.34f), position = new Vector3(0f, 0.32f, -0.02f),
                    paletteSlot = 0 },
                new ForgePart { name = "RidgeFin2", op = ForgeOp.Wedge, segments = 4,
                    size = new Vector3(0.04f, 0.08f, 0.20f), position = new Vector3(0f, 0.30f, -0.16f),
                    eulerRotation = new Vector3(-15f, 0f, 0f), paletteSlot = 0 },
                new ForgePart { name = "PlateL", op = ForgeOp.BeveledBox, bevel = 0.01f,
                    size = new Vector3(0.05f, 0.10f, 0.26f), position = new Vector3(0.15f, 0.18f, 0f),
                    eulerRotation = new Vector3(0f, 0f, -25f), paletteSlot = 2 },
                new ForgePart { name = "PlateR", op = ForgeOp.BeveledBox, bevel = 0.01f,
                    size = new Vector3(0.05f, 0.10f, 0.26f), position = new Vector3(-0.15f, 0.18f, 0f),
                    eulerRotation = new Vector3(0f, 0f, 25f), paletteSlot = 2 },
            };
            b.limbs = new[]
            {
                new ForgeLimb
                {
                    name = "LegFront", attachLocal = new Vector3(0.14f, 0.15f, 0.13f),
                    chainDirection = new Vector3(0.9f, -1f, 0.25f), role = GaitRole.Leg, mirrorX = true,
                    segments = new[]
                    {
                        new ForgeLimbSegment { size = new Vector3(0.034f, 0.14f, 0.038f), paletteSlot = 0, rounded = true, taper = 0.2f, bendDegrees = -18f },
                        new ForgeLimbSegment { size = new Vector3(0.024f, 0.12f, 0.028f), paletteSlot = 0, rounded = true, taper = 0.5f, bendDegrees = 55f },
                    }
                },
                new ForgeLimb
                {
                    name = "LegRear", attachLocal = new Vector3(0.14f, 0.15f, -0.13f),
                    chainDirection = new Vector3(0.9f, -1f, -0.25f), role = GaitRole.Leg, mirrorX = true,
                    segments = new[]
                    {
                        new ForgeLimbSegment { size = new Vector3(0.034f, 0.14f, 0.038f), paletteSlot = 0, rounded = true, taper = 0.2f, bendDegrees = -18f },
                        new ForgeLimbSegment { size = new Vector3(0.024f, 0.12f, 0.028f), paletteSlot = 0, rounded = true, taper = 0.5f, bendDegrees = 55f },
                    }
                },
            };
            b.eyeLocal = new Vector3(0f, 0.24f, 0.21f);
            b.eyeRadius = 0.035f;
            b.eyePaletteSlot = 3;
            return b;
        }

        /// <summary>
        /// warden (all worlds, Signal-gated) — the Shell's lawful enforcer: a 2.2m sentinel that
        /// WIDENS toward the shoulders (Frustum torso — authority you read across a plaza), split
        /// angled pauldrons, dome head over a near-black visor slot, and one BIG eye whose color IS
        /// the law (WardenState tells ride ForgeBodyTell.TrySetEye: dormant slate → watch cyan →
        /// warn orange → pursue red → ally green). Two 2-segment legs + two 3-segment ARMS on
        /// GaitRole.Leg — the leg-ordinal phase offset makes them swing contralateral to the legs
        /// for free. 1 + 4 + 6 = 11 bones.
        /// </summary>
        private static ForgeCreatureBody BuildWarden()
        {
            var b = ScriptableObject.CreateInstance<ForgeCreatureBody>();
            b.bodyId = "warden";
            b.palette = new[]
            {
                // Photo-loop v4: the v3 palette baked NEAR-BLACK under booth light (PaintedMetal
                // grime multiplies down) — every slot lifted so panels/wear actually read.
                new Color(0.34f, 0.37f, 0.44f), // 0 torso + legs — slate (the behavior's pillar color)
                new Color(0.25f, 0.27f, 0.33f), // 1 pauldrons + head — darker gunmetal
                new Color(0.58f, 0.62f, 0.70f), // 2 trim seams + collar + fists — bright steel
                new Color(0.45f, 0.55f, 0.68f), // 3 eye — dormant slate, bright enough to READ as a lens
                                                //   (WardenBehavior drives the live colors via TrySetEye)
                new Color(0.04f, 0.04f, 0.06f), // 4 visor slot — near-black so the eye burns in a void
            };
            b.slotStyles = new[]
            {
                new ForgeStyleSpec { style = ForgeStyle.PaintedMetal, wear = 0.45f, grime = 0.22f, panelDensity = 2f },
                new ForgeStyleSpec { style = ForgeStyle.PaintedMetal, wear = 0.35f, grime = 0.18f, panelDensity = 1f },
                new ForgeStyleSpec { style = ForgeStyle.BareMetal, wear = 0.6f, grime = 0.2f },
                new ForgeStyleSpec { style = ForgeStyle.GlowPanel, emissive = new Color(0.45f, 0.55f, 0.68f), emissiveIntensity = 1.2f },
                new ForgeStyleSpec { style = ForgeStyle.BareMetal, wear = 0.2f, grime = 0.5f },
            };
            b.coreParts = new[]
            {
                // Photo-loop v4 (v3 still read "dark monolith with a lid"): the full-width pauldron
                // slab is SPLIT into two angled shoulder plates with a bright collar between them,
                // and the body finally gets ARMS — nothing says "not a bin" like hands.
                new ForgePart { name = "Torso", op = ForgeOp.Frustum, segments = 18, smooth = true,
                    size = new Vector3(0.50f, 1.05f, 0.66f), position = new Vector3(0f, 1.30f, 0f),
                    paletteSlot = 0 },
                new ForgePart { name = "PauldronL", op = ForgeOp.BeveledBox, bevel = 0.05f,
                    size = new Vector3(0.44f, 0.20f, 0.56f), position = new Vector3(0.33f, 1.92f, 0f),
                    eulerRotation = new Vector3(0f, 0f, -10f), paletteSlot = 1 },
                new ForgePart { name = "PauldronR", op = ForgeOp.BeveledBox, bevel = 0.05f,
                    size = new Vector3(0.44f, 0.20f, 0.56f), position = new Vector3(-0.33f, 1.92f, 0f),
                    eulerRotation = new Vector3(0f, 0f, 10f), paletteSlot = 1 },
                new ForgePart { name = "Collar", op = ForgeOp.BeveledBox, bevel = 0.02f,
                    size = new Vector3(0.34f, 0.10f, 0.36f), position = new Vector3(0f, 1.90f, 0f),
                    paletteSlot = 2 },
                new ForgePart { name = "Head", op = ForgeOp.SphereSection, bevel = 0.55f, segments = 18,
                    smooth = true, size = new Vector3(0.50f, 0.34f, 0.50f),
                    position = new Vector3(0f, 2.00f, 0f), paletteSlot = 1 },
                new ForgePart { name = "Visor", op = ForgeOp.BeveledBox, bevel = 0.015f,
                    size = new Vector3(0.40f, 0.11f, 0.12f), position = new Vector3(0f, 2.02f, 0.20f),
                    paletteSlot = 4 },
                new ForgePart { name = "SkirtL", op = ForgeOp.BeveledBox, bevel = 0.015f,
                    size = new Vector3(0.08f, 0.42f, 0.50f), position = new Vector3(0.28f, 0.95f, 0f),
                    eulerRotation = new Vector3(0f, 0f, -8f), paletteSlot = 1 },
                new ForgePart { name = "SkirtR", op = ForgeOp.BeveledBox, bevel = 0.015f,
                    size = new Vector3(0.08f, 0.42f, 0.50f), position = new Vector3(-0.28f, 0.95f, 0f),
                    eulerRotation = new Vector3(0f, 0f, 8f), paletteSlot = 1 },
                new ForgePart { name = "ChestSeam", op = ForgeOp.BeveledBox, bevel = 0.01f,
                    // v5 photo niggle: at 0.10 wide the bright seam read as a door GASKET — a thin line.
                    size = new Vector3(0.06f, 0.80f, 0.06f), position = new Vector3(0f, 1.35f, 0.32f),
                    paletteSlot = 2 },
            };
            b.limbs = new[]
            {
                new ForgeLimb
                {
                    name = "Leg", attachLocal = new Vector3(0.20f, 0.80f, 0f),
                    chainDirection = new Vector3(0.06f, -1f, 0f), role = GaitRole.Leg, mirrorX = true,
                    segments = new[]
                    {
                        new ForgeLimbSegment { size = new Vector3(0.16f, 0.42f, 0.17f), paletteSlot = 0, rounded = true, taper = 0.10f, bendDegrees = -10f },
                        new ForgeLimbSegment { size = new Vector3(0.13f, 0.38f, 0.14f), paletteSlot = 1, rounded = true, taper = 0.18f, bendDegrees = 18f },
                    }
                },
                new ForgeLimb
                {
                    // Leg role ON PURPOSE: the motor's per-limb ordinal offsets this chain half a
                    // cycle from the legs — left arm swings with right leg, a contralateral walk.
                    name = "Arm", attachLocal = new Vector3(0.40f, 1.82f, 0f),
                    chainDirection = new Vector3(0.12f, -1f, 0.02f), role = GaitRole.Leg, mirrorX = true,
                    segments = new[]
                    {
                        new ForgeLimbSegment { size = new Vector3(0.15f, 0.42f, 0.15f), paletteSlot = 1, rounded = true, taper = 0.08f, bendDegrees = -6f },
                        new ForgeLimbSegment { size = new Vector3(0.12f, 0.36f, 0.12f), paletteSlot = 0, rounded = true, taper = 0.12f, bendDegrees = 22f },
                        // v5 photo niggle: bright-steel fists read as WHITE GLOVES — slate like the torso.
                        new ForgeLimbSegment { size = new Vector3(0.17f, 0.16f, 0.17f), paletteSlot = 0, rounded = true, taper = 0.05f, bendDegrees = 8f },
                    }
                },
            };
            b.eyeLocal = new Vector3(0f, 2.02f, 0.27f); // burning INSIDE the visor slot
            b.eyeRadius = 0.085f;
            b.eyePaletteSlot = 3;
            return b;
        }
    }
}
#endif
