#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Ziptide.Visuals;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// Code-authored <see cref="SkyVistaDefinition"/> assets — the canon skyscape per world/arena
    /// (STORY_BIBLE: "every sky should make you stop"; the Shell grid reveal is invisible through W006,
    /// faint at W007, banded at W009, the full wall at W012; RILL's cyan is seeded in the W005 nebula).
    /// CREATE-ONLY, same contract as WorldLayoutLibrary: an existing asset is never overwritten — the
    /// asset is the live, editable truth; this library only seeds it the first time. Dome gradients
    /// reuse each world's authored layout palette so vista and fog stay one atmosphere.
    /// Assignment onto the generated <c>&lt;Scene&gt;_Theme</c> assets is SkyVistaAuthor's job.
    ///
    /// TO ADD/RETUNE A VISTA: edit the asset in Unity for one world, or edit the Build* method +
    /// delete the asset to reseed. The canon progression tests in SkyVistaTests pin the arc.
    /// </summary>
    public static class SkyVistaLibrary
    {
        public const string VistaFolder = "Assets/Ziptide/Content/Worlds/SkyVistas";

        /// <summary>RILL's signature color (RillCompanion StirringColor) — canon-seeded in early skies.</summary>
        public static readonly Color RillCyan = new Color(0.30f, 0.80f, 0.95f);

        /// <summary>
        /// ⚖ THE SUN IS AT YOUR BACK (Terry, 2026-07-29). The Moss's star sits up and BEHIND the
        /// pilot as they fly the Catch corridor (+Z), so the rings ahead are lit on the face the
        /// player is looking at instead of being silhouettes against a glare.
        ///
        /// Shared deliberately: the vista draws the sun here and ScenePatcherSpaceLane derives the
        /// scene's key light from the same vector, so the sky and the lighting are ONE decision and
        /// cannot drift apart. Pinned by SkyVistaTests.
        /// </summary>
        public static readonly Vector3 MossSunBearing = new Vector3(0.18f, 0.62f, -0.76f);

        [MenuItem("Ziptide/Art/Author Sky Vistas (missing only)")]
        public static void AuthorFromMenu()
        {
            int made = EnsureAllAuthored();
            EditorUtility.DisplayDialog("Sky Vista Library",
                made + " vista asset(s) created under " + VistaFolder + " (existing ones untouched).", "OK");
        }

        /// <summary>Scene name → vista spec builder, in canon (world-number) order. Pure; test-inspectable.</summary>
        public static List<KeyValuePair<string, System.Func<SkyVistaDefinition>>> Specs()
        {
            return new List<KeyValuePair<string, System.Func<SkyVistaDefinition>>>
            {
                Spec("ToxicCity", BuildW001ToxicVenice),          // reserved: assigned once ToxicCity gains a theme
                Spec("W002_DryCistern", BuildW002DryCistern),
                Spec("W003_GlassShelf", BuildW003GlassShelf),
                Spec("W004_BroadcastTomb", BuildW004BroadcastTomb),
                Spec("W005_OxidizedCanopy", BuildW005OxidizedCanopy),
                Spec("W006_MirrorFlats", BuildW006MirrorFlats),
                Spec("W007_SableStation", BuildW007SableStation),
                Spec("W008_SealedArchive", BuildW008SealedArchive),
                Spec("W009_Chitinwall", BuildW009Chitinwall),
                Spec("W010_TidalArray", BuildW010TidalArray),
                Spec("W011_TheHum", BuildW011TheHum),
                Spec("W012_MarasLastJump", BuildW012MarasLastJump),
                Spec("Arena_Cistern", BuildArenaCistern),
                Spec("Arena_Chitinwall", BuildArenaChitinwall),
                Spec("Arena_MirrorFlats", BuildArenaMirrorFlats),
                Spec("Arena_Tidal", BuildArenaTidal),
                Spec("Arena_Void", BuildArenaVoid),
                Spec("SpaceLane_Trial", BuildSpaceLaneMossOrbit),  // the space leg is a PLACE, not a void
            };
        }

        // ── The space leg (CELESTIAL_SYSTEM_CANON §3/§4 — Moss orbit) ─────────────────────────────

        /// <summary>
        /// SPACE OVER THE MOSS. The flight trial shipped against a near-black placeholder sky, which
        /// made the one scene that is literally about being in space the least space-like place in
        /// the game. Canon §4 says what you see from Moss orbit: the ringed giant it orbits, the
        /// sibling grey moon, the one warm sun, and the full starfield with its galactic band.
        ///
        /// Canon §3 governs how it relates to the ground sky: same BEARINGS ("if the giant was
        /// south-southwest at dusk, it's south-southwest from orbit"), barely-changed apparent size —
        /// what space adds is REVELATION, not scale. So the giant keeps W001's exact direction and
        /// grows only 12° → 14° (the horizon no longer cuts it), while the atmosphere's smog veto on
        /// stars lifts completely: density 0 on the ground, 0.9 here.
        ///
        /// ⚖ open for Terry: canon §4 also describes the giant as filling "~⅓ of sky", which is far
        /// larger than the 12° the shipped W001 ground vista authors. This vista deliberately matches
        /// the SHIPPED GROUND ASSET rather than the doc, because the player sees both skies minutes
        /// apart and continuity between them beats matching a proposed number. Retuning both together
        /// is a one-line change here + in BuildW001ToxicVenice.
        /// </summary>
        private static SkyVistaDefinition BuildSpaceLaneMossOrbit()
        {
            var d = NewVista("space_moss_orbit",
                new Color(0.02f, 0.03f, 0.05f), new Color(0.01f, 0.01f, 0.03f));

            // The parent body — same bearing as the ground sky, fully revealed, ringplane crisp.
            Body(d, SkyVistaDefinition.BodyType.BandedPlanet, 14f, new Vector3(0.5f, 0.22f, 0.84f),
                new Color(0.46f, 0.56f, 0.76f), new Color(0.28f, 0.38f, 0.54f), bands: 7, seed: 1, phase: 0.18f);
            // The sibling grey moon — a whole world you can see from here (the in-system hop tease).
            Body(d, SkyVistaDefinition.BodyType.Moon, 5f, new Vector3(-0.62f, 0.34f, 0.71f),
                new Color(0.72f, 0.73f, 0.76f), new Color(0.48f, 0.50f, 0.55f), bands: 5, seed: 51, phase: 0.35f);
            // One warm star, harsher without air to soften it — and deliberately BEHIND the pilot
            // (MossSunBearing) so the corridor ahead is lit rather than silhouetted.
            Body(d, SkyVistaDefinition.BodyType.SunDisc, 3f, MossSunBearing,
                new Color(1f, 0.94f, 0.80f), new Color(1f, 0.80f, 0.52f), bands: 1, seed: 52, phase: 0f);

            // No air, no horizon: the whole starfield, all the way down.
            d.stars.density = 0.9f;
            d.stars.seed = 50;
            d.stars.tint = new Color(0.92f, 0.94f, 1f);
            d.stars.horizonFade = 0f;

            // The faint galactic band the night keeper shows — shared across the Moss system.
            d.nebula.enabled = true;
            d.nebula.seed = 53;
            d.nebula.coverage = 0.26f;
            d.nebula.altitudeBias = 0.1f;
            d.nebula.colorA = new Color(0.16f, 0.18f, 0.30f);
            d.nebula.colorB = new Color(0.10f, 0.14f, 0.20f);

            d.shellGridIntensity = 0f;          // canon baseline for the Moss system

            // The sun is the key light out here — hard, warm, and unfiltered, against a dark ambient.
            d.directionalLightColor = new Color(1f, 0.95f, 0.86f);
            d.directionalLightIntensity = 1.1f;
            d.overrideAmbient = true;
            d.ambientColor = new Color(0.06f, 0.07f, 0.10f);
            return d;
        }

        /// <summary>Create any missing vista assets. Returns how many were created.</summary>
        public static int EnsureAllAuthored()
        {
            int made = 0;
            foreach (var spec in Specs())
            {
                string path = AssetPathFor(spec.Key);
                if (AssetDatabase.LoadAssetAtPath<SkyVistaDefinition>(path) != null) continue;
                Directory.CreateDirectory(VistaFolder);
                var def = spec.Value();
                AssetDatabase.CreateAsset(def, path);
                Debug.Log("[Ziptide] SkyVistaLibrary authored " + path);
                made++;
            }
            if (made > 0) { AssetDatabase.SaveAssets(); AssetDatabase.Refresh(); }
            return made;
        }

        public static string AssetPathFor(string sceneName) => VistaFolder + "/" + sceneName + "_Vista.asset";

        private static KeyValuePair<string, System.Func<SkyVistaDefinition>> Spec(
            string sceneName, System.Func<SkyVistaDefinition> builder)
            => new KeyValuePair<string, System.Func<SkyVistaDefinition>>(sceneName, builder);

        // ── Story worlds (gradients mirror each world's authored layout palette) ──────────────────

        // W001 Toxic Venice — smog amber over the canals; the banded giant sits low and dim (12°).
        private static SkyVistaDefinition BuildW001ToxicVenice()
        {
            var d = NewVista("w001_toxic_venice",
                new Color(0.55f, 0.42f, 0.30f), new Color(0.25f, 0.28f, 0.35f));
            Body(d, SkyVistaDefinition.BodyType.BandedPlanet, 12f, new Vector3(0.5f, 0.22f, 0.84f),
                new Color(0.40f, 0.50f, 0.70f), new Color(0.25f, 0.35f, 0.50f), bands: 6, seed: 1, phase: 0.3f);
            d.stars.density = 0f;              // smog: no stars over the city
            d.shellGridIntensity = 0f;          // canon: invisible at W001

            // SKYSCAPE_DESIGN §4.1 — the acid default, on the first planet the player stands on.
            // ToxicCity has no theme asset, so SkyVistaAuthor cannot reach it and the dome/body/light
            // above still come from the shipped spec fields. WorldAtmosphereBinder reads ONLY this
            // block, so the city gains air (haze band + green drift) without its look changing.
            // Intensity is under the Bloom reference (0.8): this is smog you work in all day, not a
            // set-piece, and the motes have to stay readable against machinery the player is repairing.
            d.atmosphere.enabled = true;
            d.atmosphere.hazardTag = "acid";
            d.atmosphere.intensity = 0.7f;
            d.atmosphere.bodyGlow = false;      // the legacy path owns the planet; nothing to glow behind
            // Pillar 4 is already satisfied here, just not by this asset: the city's authored fog is
            // green (0.18, 0.22, 0.16) and its palette carries a `toxic` green, so the air's colour is
            // on the ground. Declaring that is honest; setting light fields nothing reads would not be.
            d.atmosphere.groundCouplingOwner = "ToxicCity.spec.json fog + layout palette";
            return d;
        }

        // W002 The Dry Cistern — underground; the "sky" is cave-black with a breath of dust glow.
        private static SkyVistaDefinition BuildW002DryCistern()
        {
            var d = NewVista("w002_dry_cistern",
                new Color(0.05f, 0.05f, 0.06f), new Color(0.09f, 0.09f, 0.11f));
            d.nebula.enabled = true; d.nebula.seed = 22; d.nebula.coverage = 0.18f; d.nebula.altitudeBias = -0.8f;
            d.nebula.colorA = new Color(0.16f, 0.13f, 0.08f);  // lamp-dust haze at the cave "horizon"
            d.nebula.colorB = new Color(0.10f, 0.10f, 0.12f);
            return d;
        }

        // W003 Glass Shelf — clear windswept daylight, TWO moons, first Pattern shimmer at the zenith.
        private static SkyVistaDefinition BuildW003GlassShelf()
        {
            var d = NewVista("w003_glass_shelf",
                new Color(0.78f, 0.88f, 0.92f), new Color(0.22f, 0.38f, 0.62f));
            Body(d, SkyVistaDefinition.BodyType.Moon, 8f, new Vector3(0.35f, 0.5f, 0.79f),
                new Color(0.85f, 0.85f, 0.90f), new Color(0.70f, 0.75f, 0.85f), bands: 4, seed: 31, phase: 0.2f);
            Body(d, SkyVistaDefinition.BodyType.Moon, 4.5f, new Vector3(-0.55f, 0.62f, 0.56f),
                new Color(0.80f, 0.82f, 0.88f), new Color(0.62f, 0.68f, 0.80f), bands: 3, seed: 32, phase: 0.45f);
            d.zenithShimmer.enabled = true;
            d.zenithShimmer.color = new Color(0.75f, 0.92f, 1f);
            d.zenithShimmer.intensity = 0.22f;  // "a faint geometric shimmer at zenith" — the Pattern seed
            return d;
        }

        // W004 The Broadcast Tomb — sealed dead station; dread is darkness, not spectacle.
        private static SkyVistaDefinition BuildW004BroadcastTomb()
        {
            return NewVista("w004_broadcast_tomb",
                new Color(0.03f, 0.03f, 0.04f), new Color(0.05f, 0.05f, 0.07f));
        }

        // W005 Oxidized Canopy — rust dusk, spore haze, the giant closer now; RILL's cyan seeded in the nebula.
        // SIGNATURE TIER (skyscape v1, 2026-07-10): the game's own Prospect reference gets the full
        // SKYSCAPE_DESIGN stack — Bloom spore atmosphere (haze card ~40m + drifting motes 8–15m, REAL
        // stereo depths) + body edge glow + the mandatory light coupling ("the sky's color IS the
        // scene's color"). Create-only: delete the W005 vista asset + reseed for this to take (runbook).
        private static SkyVistaDefinition BuildW005OxidizedCanopy()
        {
            var d = NewVista("w005_oxidized_canopy",
                new Color(0.85f, 0.45f, 0.20f), new Color(0.35f, 0.18f, 0.14f));
            Body(d, SkyVistaDefinition.BodyType.BandedPlanet, 14f, new Vector3(-0.3f, 0.35f, 0.89f),
                new Color(0.55f, 0.45f, 0.60f), new Color(0.35f, 0.28f, 0.45f), bands: 7, seed: 51, phase: 0.25f);
            d.stars.density = 0.08f; d.stars.seed = 52; d.stars.tint = new Color(1f, 0.85f, 0.7f);
            d.nebula.enabled = true; d.nebula.seed = 53; d.nebula.coverage = 0.30f; d.nebula.altitudeBias = 0.4f;
            d.nebula.colorA = new Color(0.45f, 0.20f, 0.10f);
            d.nebula.colorB = RillCyan;         // canon: the color RILL has chased for 40,000 years
            d.atmosphere.enabled = true;
            d.atmosphere.hazardTag = "Bloom";   // §4.1 default: pollen-lit amber air, dense spore drift
            d.atmosphere.intensity = 0.8f;
            d.atmosphere.bodyGlow = true;       // the giant's edge scatters through canopy air
            d.directionalLightColor = new Color(1.0f, 0.72f, 0.45f);   // pillar 4: the amber reaches the ground
            d.directionalLightIntensity = 1.05f;
            d.overrideAmbient = true;
            d.ambientColor = new Color(0.42f, 0.30f, 0.22f);
            return d;
        }

        // W006 Mirror Flats — blinding glare; the doubled sky carries "a shape that isn't above you".
        private static SkyVistaDefinition BuildW006MirrorFlats()
        {
            var d = NewVista("w006_mirror_flats",
                new Color(0.93f, 0.91f, 0.86f), new Color(0.55f, 0.66f, 0.78f));
            Body(d, SkyVistaDefinition.BodyType.BandedPlanet, 14f, new Vector3(0.15f, 0.42f, 0.90f),
                new Color(0.75f, 0.78f, 0.85f), new Color(0.60f, 0.66f, 0.78f), bands: 5, seed: 61, phase: 0.1f);
            d.zenithShimmer.enabled = true;
            d.zenithShimmer.color = new Color(0.85f, 0.88f, 0.95f);
            d.zenithShimmer.intensity = 0.14f;
            return d;
        }

        // W007 Sable Station — raw space; dense stars, cold nebula, the giant HUGE, the Shell FAINT (first glimpse).
        // Retuned 2026-07-06 (depth pass 2): a second, smaller body for parallax (Terry's "interstellar" bar
        // wants more than one thing hanging in frame) + a touch more star density/nebula coverage. NOTE: this
        // is a create-only asset (SkyVistaLibrary header) — the existing W007_SableStation_Vista.asset needs
        // deleting + `Ziptide → Art → Author Sky Vistas` re-run in Unity before this change is visible
        // (queued in TERRY_RUNBOOK.md).
        private static SkyVistaDefinition BuildW007SableStation()
        {
            var d = NewVista("w007_sable_station",
                new Color(0.02f, 0.02f, 0.05f), new Color(0.01f, 0.01f, 0.03f));
            Body(d, SkyVistaDefinition.BodyType.BandedPlanet, 22f, new Vector3(0.2f, 0.30f, 0.93f),
                new Color(0.30f, 0.45f, 0.55f), new Color(0.15f, 0.60f, 0.70f), bands: 8, seed: 71, phase: 0.35f);
            Body(d, SkyVistaDefinition.BodyType.Moon, 5f, new Vector3(0.45f, 0.42f, 0.80f),
                new Color(0.55f, 0.56f, 0.60f), new Color(0.35f, 0.38f, 0.45f), bands: 3, seed: 74, phase: 0.55f);
            d.stars.density = 0.6f; d.stars.seed = 72; d.stars.horizonFade = 0.02f; // space: stars to the "floor"
            d.nebula.enabled = true; d.nebula.seed = 73; d.nebula.coverage = 0.42f; d.nebula.altitudeBias = 0.1f;
            d.nebula.colorA = new Color(0.20f, 0.15f, 0.35f);
            d.nebula.colorB = new Color(0.10f, 0.35f, 0.45f);
            d.shellGridIntensity = 0.15f;       // canon: "a world inside a faint hexagonal grid"
            d.shellGridColor = new Color(0.15f, 0.60f, 0.70f);
            return d;
        }

        // W008 The Sealed Archive — the walled-in star map: warm dark dome strewn with archive-stars.
        private static SkyVistaDefinition BuildW008SealedArchive()
        {
            var d = NewVista("w008_sealed_archive",
                new Color(0.06f, 0.05f, 0.04f), new Color(0.10f, 0.08f, 0.06f));
            d.stars.density = 0.35f; d.stars.seed = 81; d.stars.tint = new Color(1f, 0.92f, 0.78f);
            d.shellGridIntensity = 0.2f;        // the Architects drew the cage into their own maps
            d.shellGridColor = new Color(0.45f, 0.38f, 0.22f);
            return d;
        }

        // W009 Chitinwall — the insectile aurora churn, amber into violet; the giant clearly banded by the grid.
        private static SkyVistaDefinition BuildW009Chitinwall()
        {
            var d = NewVista("w009_chitinwall",
                new Color(0.45f, 0.30f, 0.12f), new Color(0.18f, 0.10f, 0.22f));
            Body(d, SkyVistaDefinition.BodyType.BandedPlanet, 16f, new Vector3(-0.4f, 0.38f, 0.83f),
                new Color(0.45f, 0.40f, 0.55f), new Color(0.20f, 0.55f, 0.60f), bands: 9, seed: 91, phase: 0.3f);
            d.stars.density = 0.15f; d.stars.seed = 92;
            d.nebula.enabled = true; d.nebula.seed = 93; d.nebula.coverage = 0.55f; d.nebula.altitudeBias = 0.2f;
            d.nebula.colorA = new Color(0.55f, 0.35f, 0.10f); // the churning aurora
            d.nebula.colorB = new Color(0.30f, 0.15f, 0.45f);
            d.shellGridIntensity = 0.5f;        // canon: "planet clearly banded by the grid"
            d.shellGridColor = new Color(0.20f, 0.55f, 0.60f);
            return d;
        }

        // W010 Tidal Array — storm light, the vast ringed tide-puller and its runner moon.
        private static SkyVistaDefinition BuildW010TidalArray()
        {
            var d = NewVista("w010_tidal_array",
                new Color(0.55f, 0.58f, 0.50f), new Color(0.20f, 0.26f, 0.34f));
            Body(d, SkyVistaDefinition.BodyType.BandedPlanet, 18f, new Vector3(0.6f, 0.30f, 0.74f),
                new Color(0.60f, 0.55f, 0.45f), new Color(0.80f, 0.75f, 0.60f), bands: 11, seed: 101, phase: 0.2f);
            Body(d, SkyVistaDefinition.BodyType.Moon, 3.5f, new Vector3(-0.5f, 0.55f, 0.67f),
                new Color(0.55f, 0.56f, 0.58f), new Color(0.40f, 0.42f, 0.46f), bands: 3, seed: 102, phase: 0.5f);
            d.stars.density = 0.05f; d.stars.seed = 103;
            d.shellGridIntensity = 0.55f;
            d.shellGridColor = new Color(0.30f, 0.50f, 0.55f);
            return d;
        }

        // W011 The Hum — resonant tunnels; near-black with a crystal-glow breath, the grid pressing dim overhead.
        private static SkyVistaDefinition BuildW011TheHum()
        {
            var d = NewVista("w011_the_hum",
                new Color(0.06f, 0.04f, 0.05f), new Color(0.10f, 0.07f, 0.09f));
            d.nebula.enabled = true; d.nebula.seed = 111; d.nebula.coverage = 0.22f; d.nebula.altitudeBias = -0.6f;
            d.nebula.colorA = new Color(0.20f, 0.08f, 0.16f); // crystal pulse glow low in the dark
            d.nebula.colorB = new Color(0.10f, 0.06f, 0.12f);
            d.shellGridIntensity = 0.6f;
            d.shellGridColor = new Color(0.10f, 0.25f, 0.30f); // dim — felt more than seen
            return d;
        }

        // W012 Mara's Last Jump — the raw void and THE WALL: the Shell at full intensity, unmissable.
        private static SkyVistaDefinition BuildW012MarasLastJump()
        {
            var d = NewVista("w012_maras_last_jump",
                new Color(0.01f, 0.01f, 0.03f), new Color(0.03f, 0.03f, 0.06f));
            Body(d, SkyVistaDefinition.BodyType.BandedPlanet, 30f, new Vector3(0f, 0.28f, 0.96f),
                new Color(0.10f, 0.20f, 0.26f), new Color(0.20f, 0.75f, 0.85f), bands: 10, seed: 121, phase: 0.4f);
            d.stars.density = 0.55f; d.stars.seed = 122; d.stars.horizonFade = 0.02f;
            d.nebula.enabled = true; d.nebula.seed = 123; d.nebula.coverage = 0.25f; d.nebula.altitudeBias = 0f;
            d.nebula.colorA = new Color(0.12f, 0.10f, 0.22f);
            d.nebula.colorB = new Color(0.08f, 0.22f, 0.28f);
            d.shellGridIntensity = 1.0f;        // canon: the containment revealed
            d.shellGridColor = new Color(0.20f, 0.75f, 0.85f);
            return d;
        }

        // ── Arenas (distinct per MP's distinct-skies contract; palettes mirror the arena layouts) ──

        private static SkyVistaDefinition BuildArenaCistern()
        {
            var d = NewVista("arena_cistern",
                new Color(0.05f, 0.05f, 0.06f), new Color(0.09f, 0.09f, 0.11f));
            d.nebula.enabled = true; d.nebula.seed = 201; d.nebula.coverage = 0.15f; d.nebula.altitudeBias = -0.8f;
            d.nebula.colorA = new Color(0.20f, 0.15f, 0.06f); // torchlit dust — reads different from W002
            d.nebula.colorB = new Color(0.08f, 0.09f, 0.11f);
            return d;
        }

        private static SkyVistaDefinition BuildArenaChitinwall()
        {
            var d = NewVista("arena_chitinwall",
                new Color(0.45f, 0.30f, 0.12f), new Color(0.18f, 0.10f, 0.22f));
            Body(d, SkyVistaDefinition.BodyType.BandedPlanet, 16f, new Vector3(0.45f, 0.35f, 0.82f),
                new Color(0.45f, 0.40f, 0.55f), new Color(0.20f, 0.55f, 0.60f), bands: 9, seed: 211, phase: 0.3f);
            d.nebula.enabled = true; d.nebula.seed = 212; d.nebula.coverage = 0.5f; d.nebula.altitudeBias = 0.2f;
            d.nebula.colorA = new Color(0.55f, 0.35f, 0.10f);
            d.nebula.colorB = new Color(0.30f, 0.15f, 0.45f);
            d.shellGridIntensity = 0.5f;
            d.shellGridColor = new Color(0.20f, 0.55f, 0.60f);
            return d;
        }

        private static SkyVistaDefinition BuildArenaMirrorFlats()
        {
            var d = NewVista("arena_mirrorflats",
                new Color(0.93f, 0.91f, 0.86f), new Color(0.55f, 0.66f, 0.78f));
            Body(d, SkyVistaDefinition.BodyType.SunDisc, 6f, new Vector3(-0.35f, 0.55f, 0.76f),
                new Color(1f, 0.95f, 0.85f), new Color(0.95f, 0.75f, 0.45f), bands: 1, seed: 221, phase: 0f);
            d.zenithShimmer.enabled = true;
            d.zenithShimmer.color = new Color(0.85f, 0.88f, 0.95f);
            d.zenithShimmer.intensity = 0.12f;
            return d;
        }

        private static SkyVistaDefinition BuildArenaTidal()
        {
            var d = NewVista("arena_tidal",
                new Color(0.55f, 0.58f, 0.50f), new Color(0.20f, 0.26f, 0.34f));
            Body(d, SkyVistaDefinition.BodyType.BandedPlanet, 18f, new Vector3(-0.55f, 0.32f, 0.77f),
                new Color(0.60f, 0.55f, 0.45f), new Color(0.80f, 0.75f, 0.60f), bands: 11, seed: 231, phase: 0.25f);
            d.stars.density = 0.05f; d.stars.seed = 232;
            d.shellGridIntensity = 0.4f;
            d.shellGridColor = new Color(0.30f, 0.50f, 0.55f);
            return d;
        }

        private static SkyVistaDefinition BuildArenaVoid()
        {
            var d = NewVista("arena_void",
                new Color(0.01f, 0.01f, 0.03f), new Color(0.03f, 0.03f, 0.06f));
            Body(d, SkyVistaDefinition.BodyType.BlackHole, 12f, new Vector3(0.1f, 0.40f, 0.91f),
                new Color(0.02f, 0.02f, 0.04f), new Color(1f, 0.72f, 0.35f), bands: 1, seed: 241, phase: 0f);
            d.stars.density = 0.5f; d.stars.seed = 242; d.stars.horizonFade = 0.02f;
            d.shellGridIntensity = 1.0f;        // the Shell Gate: fighting against the wall itself
            d.shellGridColor = new Color(0.20f, 0.75f, 0.85f);
            return d;
        }

        // ── Builders ───────────────────────────────────────────────────────────────────────────────

        private static SkyVistaDefinition NewVista(string id, Color horizon, Color top)
        {
            var d = ScriptableObject.CreateInstance<SkyVistaDefinition>();
            d.name = id;
            d.vistaId = id;
            var g = new Gradient();
            g.SetKeys(
                new[] { new GradientColorKey(horizon, 0f), new GradientColorKey(top, 1f) },
                new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) });
            d.skyGradient = g;
            return d;
        }

        private static void Body(SkyVistaDefinition d, SkyVistaDefinition.BodyType type, float sizeDeg,
            Vector3 dir, Color baseColor, Color accent, int bands, int seed, float phase)
        {
            d.bodies.Add(new SkyVistaDefinition.CelestialBodyDef
            {
                type = type,
                angularSizeDeg = sizeDeg,
                direction = dir.normalized,
                baseColor = baseColor,
                accentColor = accent,
                bandCount = bands,
                seed = seed,
                phase = phase,
                rotationSpeedDeg = type == SkyVistaDefinition.BodyType.SunDisc ? 0f : 0.25f
            });
        }
    }
}
#endif
