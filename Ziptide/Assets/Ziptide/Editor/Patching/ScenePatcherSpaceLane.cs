#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Core;
using Ziptide.Gameplay;
using Ziptide.Ship;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// P4b — THE FLIGHT TRIAL scene (SPACEFLIGHT_PHYSICS "what v1 should do NOW"): a bounded space
    /// lane with a dock pad, an open cockpit frame, a helm running <see cref="ShipFlightRuntime"/>,
    /// and a ring course inside a LaneContent root (the thing that moves — the rig never does).
    /// Ships as a world pack in the story Packs folder, so every berthed ship's helm and the dev
    /// warp list "Flight Trial" with zero CityBuilder edits. Idempotent; standard patcher shell.
    /// 🔧 Terry: run Ziptide → Worlds → Build Space Lane (Flight Trial) once, commit scene + assets.
    /// </summary>
    public static class ScenePatcherSpaceLane
    {
        public const string SceneName = "SpaceLane_Trial";
        private const string ScenePath = "Assets/Ziptide/Scenes/" + SceneName + ".unity";
        private const string PackFolder = "Assets/Ziptide/Content/Worlds/Packs";

        private static readonly Vector3 HelmPos = new Vector3(0f, 0.05f, 0f);
        private static readonly Vector3 PlayerSpawn = new Vector3(0f, 0.1f, -2.5f);

        // Lane-space ring course: gentle climb + two turns, all reachable inside the pitch clamp
        // and snap-yaw comfort rails. Pass radius on the runtime is forgiving on purpose.
        private static readonly Vector3[] Rings =
        {
            new Vector3(0f, 3f, 60f),
            new Vector3(14f, 7f, 130f),
            new Vector3(-8f, 12f, 210f),
            new Vector3(-28f, 6f, 295f),
            new Vector3(0f, 10f, 380f),
        };
        private const float RingVisualRadius = 6f;
        // The pass test IS the bore. It used to be 7 m against a 6 m opening, which meant a run
        // could be credited on a line that also clips the truss — the ring saying "cleared" while
        // the bounds ladder says "you wore the rim". One number, no argument.
        private const float RingPassRadius = RingVisualRadius;

        /// <summary>The swept corridor as a polyline: the pilot's seat, then every ring. Rings are
        /// SQUARED to this path (FlightBoundsCore.PathAxis) — a line of rings only reads as a
        /// trajectory if each hoop actually faces along it, and the bounds ladder measures the bore
        /// with the same function so the hole you aim at is the hole the game checks.</summary>
        private static Vector3[] CorridorPath()
        {
            var path = new Vector3[Rings.Length + 1];
            path[0] = HelmPos + Vector3.up * 0.1f;
            for (int i = 0; i < Rings.Length; i++) path[i + 1] = Rings[i];
            return path;
        }

        /// <summary>
        /// Ensure the space lane exists and ships. Called by BuildAndroid, exactly as ToxicCity is.
        ///
        /// ⚠ THIS IS WHY THE SPACE LEG DID NOT EXIST. The patcher below was complete and correct for
        /// months, but it was reachable ONLY from a menu item, and its header asked Terry to run it
        /// once by hand. Nobody did, so SpaceLane_Trial.unity was never created, never entered Build
        /// Settings, and the first level's whole middle -- flight, salvage, approach, reentry -- had
        /// no scene to happen in. A world that only a human can generate is a world that does not
        /// exist. Every other shipped scene self-generates in the build; now this one does too.
        /// </summary>
        public static void EnsureInBuildSettings()
        {
            string normalized = ScenePath.Replace('\\', '/');
            if (!File.Exists(ScenePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
                var created = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                EditorSceneManager.SaveScene(created, ScenePath);
                Debug.Log("[Ziptide] Created empty space lane at " + normalized
                    + " (populated by the per-scene pass below).");
            }
            EnsureSceneEnabled();
        }

        /// <summary>No-op unless the active scene is the space lane. Called in the per-scene loop.</summary>
        public static void PatchActiveScene()
        {
            if (EditorSceneManager.GetActiveScene().name != SceneName) return;
            Populate();
        }

        [MenuItem("Ziptide/Worlds/Build Space Lane (Flight Trial)")]
        public static void Build()
        {
            Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
            var scene = File.Exists(ScenePath)
                ? EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single)
                : EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            Populate();

            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            EnsureSceneEnabled();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Ziptide] Space lane built: " + ScenePath + " (" + Rings.Length + " rings)");
        }

        private static void Populate()
        {
            var old = GameObject.Find("__SPACELANE_ROOT");
            if (old != null) Object.DestroyImmediate(old);
            var root = new GameObject("__SPACELANE_ROOT").transform;

            BuildDock(root);
            var lane = BuildLaneContent(root);

            EnsureLighting();
            EnsureEventSystem();

            // Deep-space dressing through the standard pipeline: near-black sky, a big cold planet.
            var theme = ThemeAuthor.EnsureThemeAsset(SceneName,
                new Color(0.02f, 0.03f, 0.06f), new Color(0.01f, 0.01f, 0.03f),
                new Color(0.05f, 0.06f, 0.08f),
                planetVisible: true,
                new Color(0.35f, 0.45f, 0.60f), new Color(0.55f, 0.35f, 0.45f), 14f);
            var profile = ThemeAuthor.EnsureWorldProfileAsset(SceneName, 0f, theme);
            EnsureWorldRuntime(profile);

            // SPACE IS A PLACE: seed + assign the Moss-orbit vista here rather than leaving it to
            // the Android build hook, so Terry's single "Build Space Lane" menu item produces the
            // giant, the sibling moon, the sun and the starfield instead of the placeholder void.
            // Both calls are create-only/idempotent (existing assets are never overwritten).
            SkyVistaLibrary.EnsureAllAuthored();
            SkyVistaAuthor.AssignAll();

            EnsureSpawn("player", PlayerSpawn);
            EnsureFlightRuntime(lane);
            EnsureRingLights(lane);
            EnsureCompassWiring();
            EnsureWorldPack();
            EnsureOnwardLeg();
            EnsureTheFind(lane);
        }

        // ── Dock: a small pad + open cockpit frame around the helm (interior stays static) ─────────
        private static void BuildDock(Transform root)
        {
            Cube(root, "DockPad", new Vector3(0f, -0.25f, 0f), new Vector3(8f, 0.5f, 10f),
                new Color(0.16f, 0.18f, 0.22f));

            // Open cockpit silhouette: floor rails + two side struts + a canopy bar. No walls —
            // the view of the moving lane IS the game; geometry just anchors the "inside" feel.
            var frame = new GameObject("CockpitFrame").transform;
            frame.SetParent(root, false);
            Cube(frame, "Strut_L", new Vector3(-1.4f, 1.1f, 1.2f), new Vector3(0.15f, 2.2f, 0.15f),
                new Color(0.25f, 0.28f, 0.34f));
            Cube(frame, "Strut_R", new Vector3(1.4f, 1.1f, 1.2f), new Vector3(0.15f, 2.2f, 0.15f),
                new Color(0.25f, 0.28f, 0.34f));
            Cube(frame, "CanopyBar", new Vector3(0f, 2.2f, 1.2f), new Vector3(3f, 0.15f, 0.15f),
                new Color(0.25f, 0.28f, 0.34f));

            // THE COMPASS RIBBON on the canopy bar: open space has no landmarks, so without this a
            // pilot who turns away from the course has nothing to steer back by. An instrument
            // bolted to the frame — never a HUD welded to the face.
            var compass = new GameObject("HelmCompass");
            compass.transform.SetParent(frame, false);
            compass.transform.localPosition = new Vector3(0f, 1.95f, 1.18f);
            compass.AddComponent<HelmCompassRuntime>();
        }

        // ── The lane: everything that moves past the pilot lives under this one root ───────────────
        // Drone targets flank the course between rings 2 and 4 — off the racing line, so pacifists
        // still finish the rings; hunters detour, disable, and fly in for the salvage.
        private static readonly Vector3[] Drones =
        {
            new Vector3(26f, 9f, 170f),
            new Vector3(-22f, 14f, 250f),
            new Vector3(-38f, 8f, 330f),
        };

        private static Transform BuildLaneContent(Transform root)
        {
            var lane = new GameObject("LaneContent").transform;
            lane.SetParent(root, false);

            for (int i = 0; i < Rings.Length; i++)
                BuildRing(lane, i, Rings[i]);

            for (int i = 0; i < Drones.Length; i++)
                BuildDroneTarget(lane, i, Drones[i]);

            BuildDebrisField(lane);
            return lane;
        }

        // ── The debris field (measured_specs/the_catch_measured_spec.md §4) ─────────────────────
        //
        // This used to be 28 grey rocks. Rocks were the single worst thing in the space leg: a
        // MANUFACTURED graveyard is the entire reason the swept corridor is worth flying, and
        // asteroids say the opposite — that this is empty nature you happen to be crossing.
        //
        // Every piece below is a broken part of something else in this pack, so the field reads as
        // consequence: pods that missed the catch, rings that failed, drones that ran out.

        private enum DebrisKind
        {
            TrussSection, ArrestorUnit, HullPlate, PodEndCap,
            DroneArm, ConduitBundle, FreightContainer, RadiatorFin, CargoPod,
        }

        private static void BuildDebrisField(Transform lane)
        {
            var rng = new System.Random(777);
            var field = new GameObject("DebrisField").transform;
            field.SetParent(lane, false);

            for (int i = 0; i < 30; i++)
            {
                float t = (float)rng.NextDouble();
                Vector3 along = Vector3.Lerp(new Vector3(0f, 4f, 20f), new Vector3(-5f, 8f, 420f), t);
                Vector3 off = new Vector3(((float)rng.NextDouble() - 0.5f) * 90f,
                    ((float)rng.NextDouble() - 0.5f) * 40f, ((float)rng.NextDouble() - 0.5f) * 30f);
                // Keep the swept lane clear — the corridor being CLEAN is the point of the corridor.
                if (Mathf.Abs(off.x) < 14f) off.x = Mathf.Sign(off.x == 0f ? 1f : off.x) * 14f;

                var kind = (DebrisKind)rng.Next(System.Enum.GetValues(typeof(DebrisKind)).Length);
                var piece = new GameObject(kind + "_" + i).transform;
                piece.SetParent(field, false);
                piece.localPosition = along + off;
                piece.localRotation = Quaternion.Euler(rng.Next(360), rng.Next(360), rng.Next(360));
                BuildDebrisPiece(piece, kind, rng);
                StripColliders(piece);
                piece.gameObject.AddComponent<DriftTumbleRuntime>();
            }
        }

        private static readonly Color Alloy = new Color(0.42f, 0.40f, 0.36f);
        private static readonly Color Galvanised = new Color(0.56f, 0.55f, 0.52f);
        private static readonly Color Copper = new Color(0.62f, 0.36f, 0.18f);
        private static readonly Color TornEdge = new Color(0.72f, 0.70f, 0.66f);

        /// <summary>One typed junk piece. Torn faces get a bright raw-metal cap so every break reads
        /// as failure rather than as a design choice.</summary>
        private static void BuildDebrisPiece(Transform root, DebrisKind kind, System.Random rng)
        {
            switch (kind)
            {
                case DebrisKind.TrussSection:   // 1 — SECTION A-12
                    for (int s = 0; s < 6; s++)
                    {
                        Cube(root, "Chord_" + s, new Vector3(0f, 0f, -2.5f + s), new Vector3(2.2f, 0.14f, 0.14f), Alloy);
                        var diag = Cube(root, "Diag_" + s, new Vector3(0f, 0.5f, -2.5f + s), new Vector3(2.4f, 0.1f, 0.1f), Alloy * 0.85f);
                        diag.transform.localRotation = Quaternion.Euler(0f, 0f, s % 2 == 0 ? 38f : -38f);
                    }
                    Cube(root, "TearCap", new Vector3(0f, 0f, 3.1f), new Vector3(2.2f, 0.3f, 0.12f), TornEdge);
                    break;

                case DebrisKind.ArrestorUnit:   // 2 — a coil housing torn off a ring, windings spilling
                    Cube(root, "Casing", Vector3.zero, new Vector3(1.1f, 0.9f, 1.5f), new Color(0.33f, 0.34f, 0.36f));
                    for (int r = 0; r < 4; r++)
                        Cube(root, "Rib_" + r, new Vector3(0f, 0.5f, -0.5f + r * 0.35f), new Vector3(1.2f, 0.1f, 0.12f), Alloy);
                    for (int w = 0; w < 5; w++)   // copper is the "torn open" tell
                    {
                        var wind = Cube(root, "Winding_" + w,
                            new Vector3(-0.3f + w * 0.15f, -0.1f, 0.9f), new Vector3(0.07f, 0.07f, 0.7f), Copper);
                        wind.transform.localRotation = Quaternion.Euler(rng.Next(-30, 30), rng.Next(-30, 30), 0f);
                    }
                    break;

                case DebrisKind.HullPlate:      // 3 — HULL ZONE-3, split in two
                    for (int h = 0; h < 2; h++)
                    {
                        var half = Cube(root, "Half_" + h, new Vector3(h == 0 ? -0.8f : 0.85f, 0f, 0f),
                            new Vector3(1.4f, 0.08f, 3f), Galvanised);
                        half.transform.localRotation = Quaternion.Euler(0f, 0f, h == 0 ? 6f : -9f);
                    }
                    Cube(root, "Chevrons", new Vector3(0.85f, 0.06f, 1.1f), new Vector3(0.5f, 0.02f, 0.6f),
                        new Color(0.62f, 0.52f, 0.12f));
                    break;

                case DebrisKind.PodEndCap:      // 4 — crumpled dish
                    Cube(root, "Dish", Vector3.zero, new Vector3(1.2f, 0.18f, 1.2f), Galvanised);
                    Cube(root, "Hub", new Vector3(0f, 0.12f, 0f), new Vector3(0.5f, 0.14f, 0.5f), Alloy);
                    Cube(root, "CrushLip", new Vector3(0.4f, 0.02f, 0.35f), new Vector3(0.5f, 0.1f, 0.4f), TornEdge);
                    break;

                case DebrisKind.DroneArm:       // 5 — M-DRONE S/N 40, severed
                    Cube(root, "Upper", new Vector3(0f, 0f, 0f), new Vector3(0.18f, 0.18f, 0.9f), Alloy);
                    var fore = Cube(root, "Fore", new Vector3(0f, 0.25f, 0.75f), new Vector3(0.15f, 0.15f, 0.8f), Alloy * 0.9f);
                    fore.transform.localRotation = Quaternion.Euler(42f, 0f, 0f);
                    Cube(root, "Clamp_L", new Vector3(-0.1f, 0.75f, 1.25f), new Vector3(0.06f, 0.3f, 0.1f), Alloy);
                    Cube(root, "Clamp_R", new Vector3(0.1f, 0.75f, 1.25f), new Vector3(0.06f, 0.3f, 0.1f), Alloy);
                    Cube(root, "Wires", new Vector3(0f, -0.05f, -0.6f), new Vector3(0.1f, 0.1f, 0.35f), Copper);
                    break;

                case DebrisKind.ConduitBundle:  // 6 — tangle
                    for (int c = 0; c < 7; c++)
                    {
                        var tube = Cube(root, "Conduit_" + c, new Vector3(
                                ((float)rng.NextDouble() - 0.5f) * 0.7f, ((float)rng.NextDouble() - 0.5f) * 0.7f, 0f),
                            new Vector3(0.09f, 0.09f, 1.2f + (float)rng.NextDouble()),
                            c % 3 == 0 ? Copper : Alloy);
                        tube.transform.localRotation = Quaternion.Euler(rng.Next(360), rng.Next(360), rng.Next(360));
                    }
                    break;

                case DebrisKind.FreightContainer: // 7 — MINERAL FREIGHT, burst, ore spilling
                    Cube(root, "Box", Vector3.zero, new Vector3(2.4f, 2.4f, 6f), new Color(0.40f, 0.33f, 0.26f));
                    for (int rIdx = 0; rIdx < 6; rIdx++)
                        Cube(root, "Corrugation_" + rIdx, new Vector3(1.22f, 0f, -2.2f + rIdx * 0.9f),
                            new Vector3(0.06f, 2.3f, 0.3f), new Color(0.36f, 0.30f, 0.24f));
                    Cube(root, "Breach", new Vector3(-1.2f, -0.2f, 0.6f), new Vector3(0.2f, 1.4f, 1.8f), TornEdge);
                    for (int o = 0; o < 8; o++)   // the ore it was carrying, still leaving
                    {
                        var ore = Cube(root, "Ore_" + o, new Vector3(
                                -1.6f - (float)rng.NextDouble() * 1.8f,
                                -0.4f + ((float)rng.NextDouble() - 0.5f) * 1.2f,
                                0.2f + ((float)rng.NextDouble() - 0.5f) * 2f),
                            Vector3.one * (0.18f + (float)rng.NextDouble() * 0.22f),
                            new Color(0.13f, 0.12f, 0.11f));
                        ore.transform.localRotation = Quaternion.Euler(rng.Next(360), rng.Next(360), rng.Next(360));
                    }
                    break;

                case DebrisKind.RadiatorFin:    // 8 — buckled fin off a catch ring
                    for (int f = 0; f < 5; f++)
                    {
                        var slat = Cube(root, "Slat_" + f, new Vector3(-0.6f + f * 0.3f, Mathf.Sin(f * 1.1f) * 0.15f, 0f),
                            new Vector3(0.24f, 0.05f, 2.6f), new Color(0.38f, 0.36f, 0.34f));
                        slat.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(f * 0.9f) * 14f);
                    }
                    Cube(root, "TearCap", new Vector3(0f, 0f, 1.4f), new Vector3(1.6f, 0.12f, 0.1f), TornEdge);
                    break;

                default:                        // 9 — a whole pod that never got caught
                    BuildCargoPod(root, burst: rng.Next(3) == 0);
                    break;
            }
        }

        /// <summary>
        /// CARGO POD — the object the whole Catch exists to stop, and therefore the object that
        /// explains the corridor without a word of dialogue. Blunt double-tapered capsule with the
        /// TWIN ferrous drive bands the launch track and the arrestor rings grip. No engines, no
        /// windows: it is freight, it is thrown, it cannot fly.
        /// </summary>
        private static void BuildCargoPod(Transform root, bool burst)
        {
            Cube(root, "Nose", new Vector3(0f, 0f, 1.15f), new Vector3(0.72f, 0.72f, 0.5f), Galvanised * 0.94f);
            Cube(root, "ForeBody", new Vector3(0f, 0f, 0.5f), new Vector3(0.95f, 0.95f, 0.9f), Galvanised);
            // The twin drive bands — the keeper's signature and the tell that a machine grips this.
            Cube(root, "DriveBand_A", new Vector3(0f, 0f, 0.03f), new Vector3(1.02f, 1.02f, 0.14f), new Color(0.12f, 0.12f, 0.13f));
            Cube(root, "DriveBand_B", new Vector3(0f, 0f, -0.16f), new Vector3(1.02f, 1.02f, 0.14f), new Color(0.12f, 0.12f, 0.13f));
            Cube(root, "Transponder", new Vector3(0.42f, 0.28f, 0.85f), new Vector3(0.1f, 0.1f, 0.14f),
                new Color(0.95f, 0.62f, 0.2f));
            Cube(root, "Chevrons", new Vector3(0f, 0.49f, 0.55f), new Vector3(0.5f, 0.02f, 0.3f),
                new Color(0.62f, 0.52f, 0.12f));

            if (!burst)
            {
                Cube(root, "AftBody", new Vector3(0f, 0f, -0.75f), new Vector3(0.95f, 0.95f, 1f), Galvanised);
                Cube(root, "Tail", new Vector3(0f, 0f, -1.35f), new Vector3(0.7f, 0.7f, 0.4f), Galvanised * 0.94f);
                return;
            }

            // Burst: the aft half tore off at the waist and is tumbling away with its load.
            var aft = new GameObject("AftSection_Burst").transform;
            aft.SetParent(root, false);
            aft.localPosition = new Vector3(0.25f, -0.15f, -1.5f);
            aft.localRotation = Quaternion.Euler(18f, 26f, 34f);
            Cube(aft, "AftBody", Vector3.zero, new Vector3(0.9f, 0.9f, 1f), Galvanised * 0.9f);
            Cube(aft, "TearLip", new Vector3(0f, 0f, 0.55f), new Vector3(0.98f, 0.98f, 0.1f), TornEdge);
            for (int o = 0; o < 6; o++)
                Cube(aft, "Ore_" + o, new Vector3(Mathf.Sin(o * 1.7f) * 0.6f, Mathf.Cos(o * 1.3f) * 0.5f, 0.9f + o * 0.28f),
                    Vector3.one * 0.16f, new Color(0.13f, 0.12f, 0.11f));
        }

        /// <summary>
        /// SERVICER-9 — the ring-tender (measured_specs/the_catch_measured_spec.md §3).
        ///
        /// The keeper corrected my prompt in the way that matters: this is a soft-cornered box with
        /// ONE big eye and folded tool arms, not a hostile shape. It reads as issued equipment doing
        /// a job for an employer that stopped existing, which is exactly why it pushes rather than
        /// kills. The arms are TOOLS — clamps and a welder — and nothing on it is a weapon.
        ///
        /// Named parts matter downstream: SpaceTargetRuntime finds "Eye" to light on wake, and
        /// "Arm_L"/"Arm_R" to deploy. Keep those names.
        /// </summary>
        private static void BuildDroneTarget(Transform lane, int index, Vector3 center)
        {
            // Direct child of LaneContent — SpaceTargetRuntime's lane math assumes this frame.
            var drone = new GameObject("Drone_" + index);
            drone.transform.SetParent(lane, false);
            drone.transform.localPosition = center;

            var bone = new Color(0.62f, 0.60f, 0.55f);      // pale bone-grey body, per the keeper
            var darkAlloy = new Color(0.30f, 0.29f, 0.28f);

            // Rounded box body, built as a core plus chamfer slabs so the silhouette softens.
            Cube(drone.transform, "Hull", Vector3.zero, new Vector3(1.5f, 1.5f, 1.15f), bone);
            Cube(drone.transform, "Chamfer_V", Vector3.zero, new Vector3(1.62f, 1.25f, 1.05f), bone * 0.97f);
            Cube(drone.transform, "Chamfer_H", Vector3.zero, new Vector3(1.25f, 1.62f, 1.05f), bone * 0.97f);
            Cube(drone.transform, "PanelSeam", new Vector3(0f, 0.3f, -0.6f), new Vector3(1.3f, 0.04f, 0.05f), darkAlloy);

            // Side pods — reaction mass, and the reason the silhouette reads wide at a distance.
            for (int s = -1; s <= 1; s += 2)
            {
                Cube(drone.transform, s < 0 ? "Pod_L" : "Pod_R",
                    new Vector3(s * 0.82f, 0.15f, 0f), new Vector3(0.34f, 0.62f, 0.62f), darkAlloy);
                Cube(drone.transform, s < 0 ? "Nozzle_L" : "Nozzle_R",
                    new Vector3(s * 0.98f, -0.3f, 0f), new Vector3(0.16f, 0.16f, 0.16f), darkAlloy * 0.8f);
            }

            // THE EYE — one, large, centred. Dark until you are noticed.
            var eye = Cube(drone.transform, "Eye", new Vector3(0f, 0.18f, 0.6f),
                new Vector3(0.46f, 0.46f, 0.12f), new Color(0.55f, 0.16f, 0.10f));
            Cube(eye.transform, "Bezel", new Vector3(0f, 0f, -0.3f), new Vector3(1.25f, 1.25f, 0.5f), darkAlloy);
            for (int s = -1; s <= 1; s += 2)
                Cube(drone.transform, s < 0 ? "Indicator_L" : "Indicator_R",
                    new Vector3(s * 0.42f, 0.2f, 0.6f), new Vector3(0.1f, 0.06f, 0.08f),
                    new Color(0.9f, 0.6f, 0.2f));

            // Tool arms, stowed flat along the flanks. SpaceTargetRuntime swings them out on wake.
            for (int s = -1; s <= 1; s += 2)
            {
                var arm = new GameObject(s < 0 ? "Arm_L" : "Arm_R").transform;
                arm.SetParent(drone.transform, false);
                arm.localPosition = new Vector3(s * 0.78f, -0.15f, 0.15f);
                Cube(arm, "Upper", new Vector3(0f, -0.3f, 0f), new Vector3(0.13f, 0.62f, 0.13f), darkAlloy);
                Cube(arm, "Fore", new Vector3(0f, -0.72f, 0.14f), new Vector3(0.11f, 0.5f, 0.11f), darkAlloy * 0.9f);
                Cube(arm, "Clamp_A", new Vector3(-0.06f, -1.0f, 0.24f), new Vector3(0.05f, 0.22f, 0.06f), bone * 0.8f);
                Cube(arm, "Clamp_B", new Vector3(0.06f, -1.0f, 0.24f), new Vector3(0.05f, 0.22f, 0.06f), bone * 0.8f);
            }

            // The access panel that hangs open when it is disabled — the salvage read.
            var panel = new GameObject("AccessPanel").transform;
            panel.SetParent(drone.transform, false);
            panel.localPosition = new Vector3(0f, -0.2f, -0.62f);
            Cube(panel, "Hatch", Vector3.zero, new Vector3(0.85f, 0.7f, 0.06f), bone * 0.9f);
            Cube(panel, "Board", new Vector3(0f, 0f, 0.09f), new Vector3(0.55f, 0.4f, 0.03f),
                new Color(0.16f, 0.38f, 0.20f));    // the green board, visible only once it opens
            Cube(panel, "Loom", new Vector3(0.12f, -0.1f, 0.12f), new Vector3(0.3f, 0.06f, 0.05f), Copper);

            StripColliders(drone.transform);   // hits resolve in the aim cone, never in physics
            drone.AddComponent<SpaceTargetRuntime>(); // serialized defaults: 6 armor, "scrap" ×6
        }

        /// <summary>
        /// CATCH 3 is the dead one (docs/design/THE_CATCH.md §5): blown coil, welded patch, and no
        /// sequencer at all. The corridor visibly degrades without anybody having to say so.
        /// </summary>
        private const int DeadRingIndex = 2;

        /// <summary>
        /// A CATCH RING — an orbital cargo arrestor, not a race hoop (docs/design/THE_CATCH.md).
        ///
        /// The Moss throws cargo to orbit on a mass driver, so orbit needs something to CATCH it:
        /// a line of electromagnetic brake rings on the pods' arrival arc. Every part below is the
        /// answer to a question an engineer would ask — open truss because mass costs money, coil
        /// blocks because the braking has to happen somewhere, radiator fins because vacuum has
        /// nowhere to put the heat, a numeral collar because a pilot needs to know which gate this
        /// is, and rungs because somebody used to come out here and service it.
        ///
        /// THE LAMPS LIVE IN THEIR OWN CHILD. The chase used to repaint the entire ring green, which
        /// is most of why it read as a toy: machines do not change colour, their indicators do.
        /// RingCourseLightsRuntime paints Ring_&lt;i&gt;/Lamps and nothing else.
        /// </summary>
        private static void BuildRing(Transform lane, int index, Vector3 center)
        {
            var ring = new GameObject("Ring_" + index).transform;
            ring.SetParent(lane, false);
            ring.localPosition = center;
            ring.localRotation = Quaternion.LookRotation(
                FlightBoundsCore.PathAxis(CorridorPath(), index + 1), Vector3.up);
            bool dead = index == DeadRingIndex;

            var structure = new GameObject("Truss").transform;
            structure.SetParent(ring, false);

            // 1. The truss torus — girders with cross-bracing, so the silhouette reads as built
            //    rather than extruded.
            const int segments = 12;
            var alloy = new Color(0.42f, 0.40f, 0.36f);
            var primer = new Color(0.45f, 0.31f, 0.19f);
            for (int s = 0; s < segments; s++)
            {
                float a = (s / (float)segments) * Mathf.PI * 2f;
                Vector3 outward = new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0f);

                var girder = Cube(structure, "Girder_" + s, outward * RingVisualRadius,
                    new Vector3(1.35f, 0.42f, 0.42f), s % 4 == 0 ? primer : alloy);
                girder.transform.localRotation = Quaternion.Euler(0f, 0f, a * Mathf.Rad2Deg);

                // Cross-brace between this girder and the next, angled so the lattice reads.
                float aNext = ((s + 0.5f) / segments) * Mathf.PI * 2f;
                var brace = Cube(structure, "Brace_" + s,
                    new Vector3(Mathf.Cos(aNext), Mathf.Sin(aNext), 0f) * (RingVisualRadius + 0.35f),
                    new Vector3(0.9f, 0.12f, 0.12f), alloy * 0.85f);
                brace.transform.localRotation = Quaternion.Euler(0f, 0f, aNext * Mathf.Rad2Deg + 28f);

                // 2. Coil housings on the INNER face — the working part. Ribbed, heavy, bolted.
                bool blown = dead && s == 5;
                var coil = Cube(structure, blown ? "Coil_Blown_" + s : "Coil_" + s,
                    outward * (RingVisualRadius - 0.55f),
                    new Vector3(0.85f, 0.62f, 0.72f),
                    blown ? new Color(0.10f, 0.09f, 0.09f) : new Color(0.33f, 0.34f, 0.36f));
                coil.transform.localRotation = Quaternion.Euler(0f, 0f, a * Mathf.Rad2Deg);
                if (!blown)
                    Cube(coil.transform, "Ribs", new Vector3(0f, 0.62f, 0f),
                        new Vector3(1.05f, 0.18f, 1.15f), new Color(0.28f, 0.29f, 0.31f));

                // 3. Radiator fins standing off the OUTER rim — the honest tell that this does work.
                if (s % 2 == 0)
                {
                    var fin = Cube(structure, "Fin_" + s, outward * (RingVisualRadius + 1.35f),
                        new Vector3(0.08f, 1.9f, 1.15f), new Color(0.38f, 0.36f, 0.34f));
                    fin.transform.localRotation = Quaternion.Euler(0f, 0f, a * Mathf.Rad2Deg);
                }
            }

            // The welded patch plate over the blown housing — somebody tried, once.
            if (dead)
            {
                float a = (5f / segments) * Mathf.PI * 2f;
                var patch = Cube(structure, "PatchPlate",
                    new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0f) * (RingVisualRadius - 0.2f),
                    new Vector3(1.25f, 0.9f, 0.12f), new Color(0.30f, 0.26f, 0.22f));
                patch.transform.localRotation = Quaternion.Euler(0f, 0f, a * Mathf.Rad2Deg);
            }

            // 5. The numeral collar — CATCH 1..5, readable from a long way out, plus hazard chevrons.
            var collar = Cube(structure, "NumeralCollar",
                new Vector3(0f, -(RingVisualRadius + 1.1f), 0f),
                new Vector3(3.4f, 1.5f, 0.14f), new Color(0.52f, 0.50f, 0.46f));
            var label = new GameObject("CatchNumber");
            label.transform.SetParent(collar.transform, false);
            label.transform.localPosition = new Vector3(0f, 0f, -0.12f);
            label.transform.localScale = new Vector3(1f / 3.4f, 1f / 1.5f, 1f) * 2.2f;
            var text = label.AddComponent<TextMesh>();
            text.text = "CATCH " + (index + 1);
            text.characterSize = 0.5f;
            text.fontSize = 64;
            text.anchor = TextAnchor.MiddleCenter;
            text.alignment = TextAlignment.Center;
            text.color = dead ? new Color(0.35f, 0.33f, 0.30f) : new Color(0.88f, 0.84f, 0.74f);
            for (int c = 0; c < 3; c++)
                Cube(collar.transform, "Chevron_" + c, new Vector3(-1.1f + c * 1.1f, -0.62f, -0.1f),
                    new Vector3(0.24f, 0.12f, 0.6f), new Color(0.62f, 0.52f, 0.12f));

            // 6. Service spar with grab rungs and a drone perch — human scale, so the bore reads big.
            var spar = Cube(structure, "ServiceSpar",
                new Vector3(RingVisualRadius + 1.6f, 0.9f, 0f),
                new Vector3(2.2f, 0.22f, 0.22f), alloy);
            for (int r = 0; r < 4; r++)
                Cube(spar.transform, "Rung_" + r, new Vector3(-0.3f + r * 0.22f, 0.6f, 0f),
                    new Vector3(0.05f, 1.6f, 0.05f), new Color(0.55f, 0.45f, 0.20f));
            Cube(spar.transform, "DronePerch", new Vector3(0.42f, 1.4f, 0f),
                new Vector3(0.25f, 0.9f, 1.8f), new Color(0.34f, 0.33f, 0.32f));

            // 4. THE SEQUENCER — small amber fixtures on the inner rim, the chase's only moving part.
            //    The dead ring gets none at all, which is how the player learns the Catch is failing.
            var lamps = new GameObject("Lamps").transform;
            lamps.SetParent(ring, false);
            if (dead) { StripColliders(ring); return; }

            const int lampCount = 16;
            for (int l = 0; l < lampCount; l++)
            {
                float a = (l / (float)lampCount) * Mathf.PI * 2f;
                var lamp = Cube(lamps, "Lamp_" + l,
                    new Vector3(Mathf.Cos(a), Mathf.Sin(a), 0.28f) * (RingVisualRadius - 0.05f),
                    new Vector3(0.26f, 0.16f, 0.1f), new Color(0.9f, 0.55f, 0.2f));
                lamp.transform.localRotation = Quaternion.Euler(0f, 0f, a * Mathf.Rad2Deg);
            }

            StripColliders(ring);
        }

        /// <summary>Strip colliders from a built ring: the corridor is flown THROUGH, never into.</summary>
        private static void StripColliders(Transform root)
        {
            foreach (var c in root.GetComponentsInChildren<Collider>(true))
                Object.DestroyImmediate(c);
        }

        private static void EnsureFlightRuntime(Transform lane)
        {
            var go = PatcherUtil.EnsureRootObject("ShipFlightHelm", HelmPos);
            var flight = PatcherUtil.EnsureComponent<ShipFlightRuntime>(go);
            var so = new SerializedObject(flight);
            PatcherUtil.SetObjectRef(so, "laneContent", lane);
            PatcherUtil.SetString(so, "returnScene", "W000_DriftIn");
            PatcherUtil.SetFloat(so, "ringRadius", RingPassRadius);
            PatcherUtil.SetFloat(so, "ringBoreRadius", RingVisualRadius);
            // Orbit has no floor: the bounds ladder's Ground class stays off here, and the corridor,
            // hulls, gates and the outer sphere do the talking instead.
            PatcherUtil.SetBool(so, "boundsHasGround", false);
            PatcherUtil.SetFloat(so, "boundsGroundY", 0f);
            var list = so.FindProperty("ringPositions");
            list.arraySize = Rings.Length;
            for (int i = 0; i < Rings.Length; i++)
                list.GetArrayElementAtIndex(i).vector3Value = Rings[i];
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        // The lamp-chase (LEVEL1_SPATIAL_SCRIPT §2): the lights runtime rides the lane root and
        // reads course progress off the helm's flight runtime — next ring chases amber, passed
        // rings settle green. Lane is rebuilt every Populate, so AddComponent lands fresh.
        private static void EnsureRingLights(Transform lane)
        {
            var lights = PatcherUtil.EnsureComponent<RingCourseLightsRuntime>(lane.gameObject);
            var helm = GameObject.Find("ShipFlightHelm");
            var so = new SerializedObject(lights);
            PatcherUtil.SetObjectRef(so, "flight",
                helm != null ? helm.GetComponent<ShipFlightRuntime>() : null);
            PatcherUtil.SetObjectRef(so, "lane", lane);
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        // The compass is built with the dock (it is part of the cockpit frame) but the helm it
        // reads is created later, so the reference is serialized here once both exist.
        private static void EnsureCompassWiring()
        {
            var compass = Object.FindObjectOfType<HelmCompassRuntime>();
            var helm = GameObject.Find("ShipFlightHelm");
            if (compass == null || helm == null) return;
            var so = new SerializedObject(compass);
            PatcherUtil.SetObjectRef(so, "flight", helm.GetComponent<ShipFlightRuntime>());
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        /// <summary>
        /// THE FIND (FIRST_HOUR_DIRECTORS_CUT §2.1) — among the scrap, one object does not scan.
        ///
        /// It sits on the far wreck, past the last ring, so reaching it means actually flying the
        /// course rather than stepping off the dock and picking it up. It is an ordinary grabbable
        /// item, which means the holster contract carries it to Toxic City for free — the artifact
        /// needed no new inventory system, only a reason to exist.
        /// </summary>
        private static void EnsureTheFind(Transform lane)
        {
            const string Name = "__ARTIFACT_HALF_A";
            if (lane.Find(Name) != null) return;

            var cradle = new GameObject(Name);
            cradle.transform.SetParent(lane, false);
            // Just beyond the final ring (0, 10, 380) and off the racing line, so it reads as
            // salvage you went looking for rather than something dropped on the path.
            cradle.transform.localPosition = new Vector3(9f, 8f, 405f);

            // A wreck fragment to find it ON. Without something to pull it out of, an artifact
            // floating in open space reads as a pickup, not a discovery.
            var hulk = GameObject.CreatePrimitive(PrimitiveType.Cube);
            hulk.name = "WreckFragment";
            hulk.transform.SetParent(cradle.transform, false);
            hulk.transform.localScale = new Vector3(6f, 2.4f, 9f);
            hulk.transform.localRotation = Quaternion.Euler(14f, 32f, 8f);
            Object.DestroyImmediate(hulk.GetComponent<Collider>());
            Paint(hulk, new Color(0.24f, 0.25f, 0.28f));

            var marker = new GameObject("__SALVAGE_ARTIFACT_HALF_A");
            marker.transform.SetParent(cradle.transform, false);
            marker.transform.localPosition = new Vector3(0f, 1.7f, 0f);
            marker.AddComponent<SpaceSalvageItemRuntime>().Init("artifact_half_a");

            BuildFindShell(cradle.transform);
        }

        /// <summary>
        /// THE FIND'S DEBRIS SHELL (LEVEL1_SPATIAL_SCRIPT §2). "One object does not scan" only
        /// lands if there are OTHER objects — a lone artifact on a bare hulk reads as a quest
        /// marker. Nine tumbling scrap pieces drift in a 12 m shell around the cradle, deliberately
        /// ordinary, so the humming one is a discovery the player makes rather than a thing the
        /// level points at. Procedural stand-in per the stand-in law; Tripo re-skins later.
        /// </summary>
        private static void BuildFindShell(Transform cradle)
        {
            const int Pieces = 9;
            const float ShellRadius = 12f;
            for (int i = 0; i < Pieces; i++)
            {
                // Deterministic scatter (golden-angle spiral) — same wreck field every bake.
                float a = i * 2.39996f;
                float y = 1f - (i / (float)(Pieces - 1)) * 2f;
                float r = Mathf.Sqrt(Mathf.Max(0f, 1f - y * y));
                var dir = new Vector3(Mathf.Cos(a) * r, y * 0.55f, Mathf.Sin(a) * r);

                var scrap = GameObject.CreatePrimitive(
                    i % 3 == 0 ? PrimitiveType.Cube : PrimitiveType.Capsule);
                scrap.name = "Scrap_" + i;
                scrap.transform.SetParent(cradle, false);
                scrap.transform.localPosition = dir * ShellRadius * (0.55f + (i % 4) * 0.15f);
                scrap.transform.localRotation = Quaternion.Euler(i * 37f, i * 61f, i * 23f);
                float s = 0.5f + (i % 3) * 0.35f;
                scrap.transform.localScale = new Vector3(s, s * (0.6f + (i % 2) * 0.5f), s);
                Object.DestroyImmediate(scrap.GetComponent<Collider>()); // drift past, never bump
                Paint(scrap, new Color(0.26f + (i % 3) * 0.03f, 0.25f, 0.23f));
                scrap.AddComponent<DriftTumbleRuntime>();
            }
        }

        /// <summary>
        /// The salvage sortie is a LEG, not a destination. Canon (FIRST_HOUR_DIRECTORS_CUT §5):
        /// W000 -> lane (fly, stun, salvage, THE FIND) -> dock at Toxic City. Without this the lane
        /// was a cul-de-sac whose only exit was RETURN HOME, so the first hour could never continue
        /// through it and the whole middle of the level stayed unreachable.
        ///
        /// The station stands ON the dock pad, beside the helm, so it reads as "the next hop" rather
        /// than a door hidden somewhere in open space.
        /// </summary>
        private static void EnsureOnwardLeg()
        {
            var pack = AssetDatabase.LoadAssetAtPath<WorldPackDefinition>(
                ZiptideConstants.PathToxicCityWorldPack);
            if (pack == null)
            {
                Debug.LogWarning("[Ziptide] space lane: ToxicCity world pack missing — the onward leg "
                    + "was not placed, so the salvage sortie has no continuation.");
                return;
            }

            var go = PatcherUtil.EnsureRootObject(ZiptideConstants.GoWorldTravelStation,
                new Vector3(2.6f, 0.1f, -1.4f));
            var station = PatcherUtil.EnsureComponent<WorldTravelStation>(go);
            var so = new SerializedObject(station);
            var list = so.FindProperty("destinationPacks");
            if (list != null)
            {
                list.arraySize = 1;
                list.GetArrayElementAtIndex(0).objectReferenceValue = pack;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        // ── Standard shell (same as every patcher) ──────────────────────────────────────────────────
        /// <summary>
        /// THE SUN IS AT YOUR BACK (⚖ Terry, 2026-07-29 — measured spec §6).
        ///
        /// *"whatever the sun direction is, that's the direction the lighting should come from…
        /// the sun is essentially more or less at our back so the objects are lit up ahead of us,
        /// at least to a degree, maybe a little bit of shadow."*
        ///
        /// This light used to be hand-authored at Euler(35, −30, 0) — a direction with no
        /// relationship whatsoever to the sun drawn in the sky, which is how you end up flying a
        /// corridor of silhouettes with a sun blazing somewhere it clearly isn't. The key light is
        /// now DERIVED from the vista's sun bearing, so the two can never drift apart again: the
        /// sun sits up and behind the pilot's shoulder, and every catch ring ahead of them is lit
        /// on the face they are looking at.
        /// </summary>
        private static void EnsureLighting()
        {
            var go = PatcherUtil.EnsureRootObject("Directional Light", new Vector3(0f, 10f, 0f));
            var light = PatcherUtil.EnsureComponent<Light>(go);
            light.type = LightType.Directional;
            light.color = new Color(1f, 0.95f, 0.86f);   // one warm star
            light.intensity = 1.1f;                       // hard key — vacuum has no fill
            light.shadows = LightShadows.Soft;            // "maybe a little bit of shadow"

            // Light TRAVELS opposite the direction the sun sits in. SunBearing is the same constant
            // the Moss-orbit vista uses, so sky and lighting are one decision.
            go.transform.rotation = Quaternion.LookRotation(-SkyVistaLibrary.MossSunBearing.normalized);

            // A dim cool ambient so unlit faces read as unlit MATERIAL rather than as holes.
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(0.06f, 0.07f, 0.10f);
        }

        private static void EnsureEventSystem()
        {
            if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() != null) return;
            var go = PatcherUtil.EnsureRootObject("EventSystem", Vector3.zero);
            PatcherUtil.EnsureComponent<UnityEngine.EventSystems.EventSystem>(go);
            PatcherUtil.EnsureComponent<UnityEngine.XR.Interaction.Toolkit.UI.XRUIInputModule>(go);
        }

        private static void EnsureWorldRuntime(WorldProfile profile)
        {
            var go = PatcherUtil.EnsureRootObject("WorldRuntime", Vector3.zero);
            var wr = PatcherUtil.EnsureComponent<WorldRuntime>(go);
            if (profile != null)
            {
                var so = new SerializedObject(wr);
                PatcherUtil.SetObjectRef(so, "worldProfile", profile);
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private static void EnsureSpawn(string markerId, Vector3 pos)
        {
            string objName = markerId == "player" ? ZiptideConstants.GoSpawnPlayer : "__SPAWN_" + markerId;
            var go = PatcherUtil.EnsureRootObject(objName, pos);
            var marker = PatcherUtil.EnsureComponent<SpawnMarkerRuntime>(go);
            var so = new SerializedObject(marker);
            PatcherUtil.SetString(so, "markerId", markerId);
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        private static void EnsureWorldPack()
        {
            string path = PackFolder + "/" + SceneName + "_WorldPack.asset";
            var pack = AssetDatabase.LoadAssetAtPath<WorldPackDefinition>(path);
            if (pack == null)
            {
                Directory.CreateDirectory(PackFolder);
                pack = ScriptableObject.CreateInstance<WorldPackDefinition>();
                AssetDatabase.CreateAsset(pack, path);
            }
            pack.packId = "spacelane_trial";
            pack.displayName = "Flight Trial";
            pack.sceneName = SceneName;
            var player = pack.spawnMarkers.Find(m => m != null && m.markerId == "player");
            if (player == null)
                pack.spawnMarkers.Add(new SpawnMarkerDefinition { markerId = "player", localPosition = PlayerSpawn });
            else
                player.localPosition = PlayerSpawn;
            EditorUtility.SetDirty(pack);
        }

        private static void EnsureSceneEnabled()
        {
            var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            int idx = scenes.FindIndex(s => s.path == ScenePath);
            if (idx < 0) { scenes.Add(new EditorBuildSettingsScene(ScenePath, true)); EditorBuildSettings.scenes = scenes.ToArray(); }
            else if (!scenes[idx].enabled) { scenes[idx] = new EditorBuildSettingsScene(ScenePath, true); EditorBuildSettings.scenes = scenes.ToArray(); }
        }

        private static GameObject Cube(Transform parent, string name, Vector3 localPos, Vector3 size, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = size;
            Paint(go, color);
            return go;
        }

        private static void Paint(GameObject go, Color color)
        {
            var r = go.GetComponent<Renderer>();
            if (r == null) return;
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) return;
            var mat = new Material(shader);
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
            else if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);
            r.sharedMaterial = mat;
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }
}
#endif
