#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;
using Ziptide.Core;
using Ziptide.Gameplay;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// Builds the dedicated ToxicCity world — the reusable WORLD BLUEPRINT. A thin shell over
    /// <see cref="CityBuilder"/>: it ensures the scene + layout asset, resets the city root, runs the
    /// data-driven builder, then wires the world-level singletons (spawn, world pack, travel door,
    /// dispatch/board/job director). Self-generates the empty scene so the cloud build produces it with
    /// no Unity PC and no hand-edited YAML. Idempotent + menu-invoked + populated by the build pipeline.
    ///
    /// To clone a new world: author a new CityLayoutDefinition asset + copy this ~shell pointing at it.
    /// </summary>
    public static class ScenePatcherToxicCity
    {
        public const string SceneName = ZiptideConstants.SceneToxicCity;
        private const string ScenePath = ZiptideConstants.PathToxicCityScene;
        private const string LayoutPath = ZiptideConstants.PathToxicCityLayout;
        private const string WorldPackPath = ZiptideConstants.PathToxicCityWorldPack;
        private const string DefaultWorldProfilePath = ZiptideConstants.PathDefaultWorldProfileWorlds;

        [MenuItem("Ziptide/Worlds/Build Toxic City")]
        public static void BuildFromMenu()
        {
            var scene = OpenOrCreateScene();
            PopulateActiveToxicCity();
            EditorSceneManager.MarkSceneDirty(scene);
            EditorSceneManager.SaveScene(scene, ScenePath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Toxic City",
                "Built/updated " + SceneName + ".\n\nWarp to it via Ziptide > Dev, or it ships in the next build.", "OK");
        }

        /// <summary>Ensure ToxicCity is enabled in Build Settings (called by BuildAndroid). Idempotent.</summary>
        public static void EnsureInBuildSettings()
        {
            string normalized = ScenePath.Replace('\\', '/');
            if (!File.Exists(ScenePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
                var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                EditorSceneManager.SaveScene(scene, ScenePath);
                Debug.Log("[Ziptide] Created empty ToxicCity scene at " + normalized + " (will be populated by the build).");
            }
            var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            int idx = scenes.FindIndex(s => s.path == normalized);
            if (idx < 0) { scenes.Add(new EditorBuildSettingsScene(normalized, true)); EditorBuildSettings.scenes = scenes.ToArray(); }
            else if (!scenes[idx].enabled) { scenes[idx] = new EditorBuildSettingsScene(normalized, true); EditorBuildSettings.scenes = scenes.ToArray(); }
        }

        /// <summary>No-op unless the active scene is ToxicCity. Called in the BuildAndroid per-scene loop.</summary>
        public static void PatchActiveScene()
        {
            if (EditorSceneManager.GetActiveScene().name != SceneName) return;
            PopulateActiveToxicCity();
        }

        /// <summary>Build the whole city into the currently-open ToxicCity scene.</summary>
        public static void PopulateActiveToxicCity()
        {
            var kit = EnsureLayoutAsset();

            string rootName = "__" + kit.cityId.ToUpperInvariant() + "_ROOT";
            var root = ResetRoot(rootName);

            Random.InitState(kit.seed);
            CityBuilder.Build(root, kit);

            EnsureLighting();
            EnsureEventSystem();
            EnsureWorldRuntime(kit);

            // Spawn on the Dispatch district plaza (open, on solid ground at walkwayHeight).
            var dispatch = FindDistrict(kit, "Dispatch") ?? (kit.districts.Count > 0 ? kit.districts[0] : null);
            Vector3 spawnPos = dispatch != null
                ? dispatch.anchor + new Vector3(0f, kit.walkwayHeight + 0.1f, 0f)
                : new Vector3(0f, kit.walkwayHeight + 0.1f, 0f);
            EnsureSpawn("player", spawnPos);

            var pack = EnsureWorldPack(kit, spawnPos);
            EnsureTravelStation(kit);
            EnsureDispatchAndBoard(pack, spawnPos);
            SpawnStarterWeapons(root, spawnPos);
            EnsureFirstHourRoute(root, kit, spawnPos);
            EnsureReentryArrival();

            // THE EXPEDITION (⚖ Terry): half B lives outside the wall, so the site, its burn-off
            // column, and the drivable breach are part of the city bake — not a separate world.
            FlatsSiteAuthor.Build(root, kit);

            // THE COMPASS: the lantern route over the contract's walk, and the sightline triple
            // (relay strobe / north crown / berth floods) that makes "which way is home" a thing
            // you look at rather than a thing you are told. Last of the §3 wayfinding rows.
            CityWayfindingAuthor.Build(root, kit);

            // THE HANGAR WALK: berths 1-5 west of your own, empty. Runs after the shipyard exists
            // so it can measure off the real berth rather than the design doc's coordinates.
            QuayBerthAuthor.Build(root, kit);

            // THE FOREGROUND: junk within arm's reach on the walk off the ramp, plus the one
            // grabbable crate that teaches the grab verb without a line of dialogue. Runs after the
            // berths so it measures the same walk the player actually crosses.
            ShipyardApproachAuthor.Build(root, kit);

            // THE AIR: haze band + acid drift (SKYSCAPE_DESIGN §4.1). ToxicCity owns no theme, so
            // the vista path never reaches it and the first planet shipped with perfectly still,
            // perfectly clear air. Additive by construction — the binder drives only haze and motes.
            EnsureWorldAtmosphere(root);
        }

        /// <summary>
        /// Point a <see cref="WorldAtmosphereBinder"/> at ToxicCity's authored vista. The dome, the
        /// planet, the fog and the light all still come from the shipped spec fields; only the
        /// atmosphere block is read, so this cannot change the city's existing look.
        /// </summary>
        private static void EnsureWorldAtmosphere(Transform root)
        {
            var vista = AssetDatabase.LoadAssetAtPath<Ziptide.Visuals.SkyVistaDefinition>(
                SkyVistaLibrary.AssetPathFor(ZiptideConstants.SceneToxicCity));
            if (vista == null)
            {
                // EnsureAllAuthored runs earlier in the bake hook; if it did not, say so rather than
                // silently shipping still air again.
                Debug.LogWarning("ZIPTIDE: WORLD_ATMO_AUTHOR skipped cause=no_vista_asset path="
                    + SkyVistaLibrary.AssetPathFor(ZiptideConstants.SceneToxicCity));
                return;
            }

            var go = new GameObject("__WORLD_ATMOSPHERE");
            go.transform.SetParent(root, false);
            var binder = go.AddComponent<Ziptide.Gameplay.WorldAtmosphereBinder>();
            binder.Configure(vista);
            Debug.Log("ZIPTIDE: WORLD_ATMO_AUTHOR built vista=" + vista.vistaId
                + " hazard=" + vista.atmosphere.hazardTag);
        }

        // The reentry/landing handoff owner (product contract §4 — was "no canonical owner/beat").
        // Fires only on the space-leg → world route; every other way into this scene stays silent.
        private static void EnsureReentryArrival()
        {
            var go = PatcherUtil.EnsureRootObject("ReentryArrival", Vector3.zero);
            var runtime = PatcherUtil.EnsureComponent<ReentryArrivalRuntime>(go);
            var so = new SerializedObject(runtime);
            PatcherUtil.SetString(so, "expectedOriginScene", ZiptideConstants.SceneSpaceLane);
            PatcherUtil.SetString(so, "rillLineId", string.Empty);
            so.ApplyModifiedPropertiesWithoutUndo();
        }

        /// <summary>
        /// The two pieces of first-hour furniture the contract's beats need but no other author
        /// places: a safe discharge target beside the spawn, and the designated job zipline.
        ///
        /// Both used to exist only as CI-green runtime classes with nowhere to stand. FH_SHOOT_PRACTICE
        /// and FH_USE_JOB_ZIPLINE could therefore never fire in the shipped world — a whole verb the
        /// tutorial claims to teach had no object to teach it on.
        /// </summary>
        private static void EnsureFirstHourRoute(Transform root, CityLayoutDefinition kit, Vector3 spawnPos)
        {
            // ── Discharge practice: teach the verb BEFORE the drones apply pressure ──────────────
            const string TargetName = "__FIRST_HOUR_PRACTICE_TARGET";
            if (root.Find(TargetName) == null)
            {
                var target = GameObject.CreatePrimitive(PrimitiveType.Cube);
                target.name = TargetName;
                target.transform.SetParent(root, true);
                // Measured, not eyeballed: the first placement put the target at (2.4, -3.5),
                // which is INSIDE Hero_DispatchHall's footprint (x -4..4, z -5.5..1.5). West of the
                // spawn is the one open pocket clear of the hall and of both facade rows.
                target.transform.position = spawnPos + new Vector3(-4.5f, 1.25f, 1.5f);
                target.transform.localScale = new Vector3(0.45f, 0.45f, 0.08f);
                target.AddComponent<TargetRuntime>();
                target.AddComponent<JobTarget>();
                ItemFactory.ApplyURPColor(target, new Color(0.85f, 0.30f, 0.22f));

                var post = GameObject.CreatePrimitive(PrimitiveType.Cube);
                post.name = "PracticePost";
                post.transform.SetParent(target.transform.parent, true);
                post.transform.position = target.transform.position + Vector3.down * 0.75f;
                post.transform.localScale = new Vector3(0.08f, 1.5f, 0.08f);
                ItemFactory.ApplyURPColor(post, new Color(0.28f, 0.29f, 0.31f));
            }

            // ── The designated job zipline: Plaza (high) down to the CanalRow relay ──────────────
            // It runs along the actual contract route, so the traversal beat is on the way to the work
            // rather than a detour the player has no reason to take.
            const string ZipName = "__FIRST_HOUR_JOB_ZIPLINE";
            if (root.Find(ZipName) != null) return;

            DistrictDef plaza = FindDistrict(kit, "Plaza");
            DistrictDef canal = FindDistrict(kit, "CanalRow");
            if (plaza == null || canal == null) return;

            var zip = new GameObject(ZipName);
            zip.transform.SetParent(root, true);
            // ⚠ The start anchor carries the GRABBABLE HANDLE, so it has to be inside hand reach.
            // The first placement strung it 7 m up -- a perfectly good zipline nobody could ever
            // ride, because ToxicCity is flat and there is nothing to climb. Generated worlds get
            // away with +5.5 m because their terrain gives you the climb; a flat city does not.
            zip.AddComponent<ZiplineRuntime>().Init(
                new Vector3(plaza.anchor.x, kit.walkwayHeight + 2.2f, plaza.anchor.z),
                new Vector3(canal.anchor.x, kit.walkwayHeight + 1.0f, canal.anchor.z + 6f));
        }

        // A taser + gravity gun by the spawn so you can actually fight the drones without hauling one in.
        private static void SpawnStarterWeapons(Transform root, Vector3 spawnPos)
        {
            var taser = ItemFactory.Create("taser_dart_gun", spawnPos + new Vector3(-0.6f, 1.0f, 1.0f));
            if (taser != null) taser.transform.SetParent(root, true);
            var grav = ItemFactory.Create("gravity_gun", spawnPos + new Vector3(0.6f, 1.0f, 1.0f));
            if (grav != null) grav.transform.SetParent(root, true);
        }

        // ── Layout asset (self-bootstrapping default) ────────────────────────
        private static CityLayoutDefinition EnsureLayoutAsset()
        {
            var kit = AssetDatabase.LoadAssetAtPath<CityLayoutDefinition>(LayoutPath);
            if (kit != null) return kit;

            Directory.CreateDirectory(Path.GetDirectoryName(LayoutPath));
            kit = BuildDefaultToxicCity();
            AssetDatabase.CreateAsset(kit, LayoutPath);
            AssetDatabase.SaveAssets();
            Debug.Log("[Ziptide] Created default ToxicCityLayout at " + LayoutPath + " (edit it to re-layout the city).");
            return kit;
        }

        /// <summary>The hand-authored Toxic City — edit the generated asset to change it without code.</summary>
        private static CityLayoutDefinition BuildDefaultToxicCity()
        {
            var kit = ScriptableObject.CreateInstance<CityLayoutDefinition>();
            kit.cityId = "toxic_city";
            // sceneName was never set here because nothing read it — the committed asset carries it and
            // the code default only runs if that asset is missing. It is set now because EnsureWorldRuntime
            // authors the theme at "Themes/<sceneName>_Theme.asset", and an empty sceneName would quietly
            // write "_Theme.asset" instead of failing. A latent trap costs one line to remove.
            kit.sceneName = SceneName;
            kit.seed = 1337;
            kit.walkwayHeight = 0f;

            // Districts — a coherent dock-to-plaza city.
            kit.districts.Add(new DistrictDef
            {
                id = "Shipyard", anchor = new Vector3(0f, 0f, -30f), bounds = new Vector2(22f, 18f), heightTier = 1,
                // 4.5 m, not 2 m: at 16 m tall a 2 m mast is 8:1 and reads as a stick with no surface
                // to put a ruler on (LandmarkScaleCore.ReadsAsStick). Pulled in from x=8 to x=5 at the
                // same time — the wider footprint would otherwise intersect the east facade row.
                landmarks = { new LandmarkDef { name = "Crane", kind = LandmarkKind.Crane, localPos = new Vector3(5f, 0f, -5f), height = 16f, width = 4.5f } },
                heroBuildings = { new HeroBuildingDef { id = "ShipyardOffice", localPos = new Vector3(-7f, 0f, 2f), footprint = new Vector2(7f, 7f), height = 4f, interior = InteriorKind.JobGiver, doorLocalPos = new Vector3(0f, 0f, -3.5f), interiorMarkerId = "shipyard_office" } },
            });
            kit.districts.Add(new DistrictDef
            {
                id = "Dispatch", anchor = new Vector3(0f, 0f, -8f), bounds = new Vector2(20f, 18f), heightTier = 1,
                heroBuildings = { new HeroBuildingDef { id = "DispatchHall", localPos = new Vector3(0f, 0f, 6f), footprint = new Vector2(8f, 7f), height = 4.5f, interior = InteriorKind.JobGiver, doorLocalPos = new Vector3(0f, 0f, -3.5f), interiorMarkerId = "dispatch_inside" } },
            });
            kit.districts.Add(new DistrictDef
            {
                id = "Plaza", anchor = new Vector3(0f, 0f, 28f), bounds = new Vector2(24f, 22f), heightTier = 2,
                landmarks =
                {
                    new LandmarkDef { name = "OligarchTower", localPos = new Vector3(9f, 0f, 6f), height = 40f, width = 6f },
                    new LandmarkDef { name = "GardenTower", localPos = new Vector3(-9f, 0f, 6f), height = 18f, width = 5f },
                },
            });
            kit.districts.Add(new DistrictDef
            {
                id = "Market", anchor = new Vector3(26f, 0f, 8f), bounds = new Vector2(22f, 20f), heightTier = 1,
                props = { new PropPatchDef { kind = "Stall", center = Vector3.zero, size = new Vector2(12f, 12f), density = 0.5f } },
            });
            kit.districts.Add(new DistrictDef
            {
                id = "CanalRow", anchor = new Vector3(-26f, 0f, 8f), bounds = new Vector2(16f, 26f), heightTier = 1,
                heroBuildings = { new HeroBuildingDef { id = "RelayVault", localPos = new Vector3(0f, 0f, 0f), footprint = new Vector2(9f, 8f), height = 5f, interior = InteriorKind.Mission, doorLocalPos = new Vector3(0f, 0f, -4f), interiorMarkerId = "relay_node" } },
            });

            // Connections — main boulevard + side streets forming a loop (so it walks as a city).
            kit.connections.Add(new ConnectionDef { fromDistrictId = "Shipyard", toDistrictId = "Dispatch", kind = ConnectionKind.GroundStreet, width = 8f });
            kit.connections.Add(new ConnectionDef { fromDistrictId = "Dispatch", toDistrictId = "Plaza", kind = ConnectionKind.GroundStreet, width = 8f });
            kit.connections.Add(new ConnectionDef { fromDistrictId = "Plaza", toDistrictId = "Market", kind = ConnectionKind.GroundStreet, width = 6f });
            kit.connections.Add(new ConnectionDef { fromDistrictId = "Dispatch", toDistrictId = "Market", kind = ConnectionKind.GroundStreet, width = 6f });
            kit.connections.Add(new ConnectionDef { fromDistrictId = "Plaza", toDistrictId = "CanalRow", kind = ConnectionKind.ElevatedWalkway, width = 5f, tier = 1 });
            kit.connections.Add(new ConnectionDef { fromDistrictId = "Dispatch", toDistrictId = "CanalRow", kind = ConnectionKind.Bridge, width = 5f });
            kit.connections.Add(new ConnectionDef { fromDistrictId = "CanalRow", toDistrictId = "Market", kind = ConnectionKind.Ramp, width = 5f });

            // Canals — toxic sludge in the gaps the elevated walkway/bridge cross.
            kit.canals.Add(new CanalRegionDef { center = new Vector3(-14f, 0f, 16f), size = new Vector2(14f, 30f), depth = 2.2f });
            kit.canals.Add(new CanalRegionDef { center = new Vector3(0f, 0f, 10f), size = new Vector2(10f, 12f), depth = 2f });

            // Drone zones — NOT every district. Passive tutorial trio + two combat patrols.
            kit.droneZones.Add(new DroneZoneDef { id = "Tutorial_Dispatch", center = new Vector3(0f, 0f, -8f), radius = 4f, count = 3, respawnDelay = 0f, combat = false });
            kit.droneZones.Add(new DroneZoneDef { id = "Patrol_Market", center = new Vector3(26f, 0f, 8f), radius = 6f, count = 3, respawnDelay = 14f, combat = true });
            kit.droneZones.Add(new DroneZoneDef { id = "Patrol_Canal", center = new Vector3(-14f, 0f, 16f), radius = 4f, count = 2, respawnDelay = 16f, combat = true });

            // Shipyard berth + static ship, just past the shipyard district.
            kit.shipyard = new ShipyardBerthDef
            {
                enabled = true,
                berthCenter = new Vector3(0f, 0f, -48f),
                berthSize = new Vector2(20f, 16f),
                shipLocalPos = new Vector3(0f, 1.6f, 0f),
                shipSize = new Vector3(5f, 3f, 12f),
                shipRotationY = 0f,
            };
            return kit;
        }

        // ── Root reset (deterministic rebuild) ───────────────────────────────
        private static Transform ResetRoot(string name)
        {
            var existing = GameObject.Find(name);
            if (existing != null) Object.DestroyImmediate(existing);
            var go = new GameObject(name);
            return go.transform;
        }

        // ── World singletons (find-or-update; never duplicated) ──────────────
        private static void EnsureLighting()
        {
            var go = PatcherUtil.EnsureRootObject("Directional Light", new Vector3(0f, 10f, 0f));
            var light = PatcherUtil.EnsureComponent<Light>(go);
            light.type = LightType.Directional;
            light.intensity = 1.05f;
            go.transform.rotation = Quaternion.Euler(50f, -25f, 0f);
        }

        private static void EnsureEventSystem()
        {
            if (Object.FindObjectOfType<UnityEngine.EventSystems.EventSystem>() != null) return;
            var go = PatcherUtil.EnsureRootObject("EventSystem", Vector3.zero);
            PatcherUtil.EnsureComponent<UnityEngine.EventSystems.EventSystem>(go);
            PatcherUtil.EnsureComponent<UnityEngine.XR.Interaction.Toolkit.UI.XRUIInputModule>(go);
        }

        /// <summary>
        /// THE FIRST LEVEL GETS ITS OWN SKY.
        ///
        /// ToxicCity pointed at the SHARED `DefaultWorldProfile`, whose theme is a generic one. Which
        /// means the sky the layout has always authored — olive horizon, dark teal zenith, a 22°
        /// occluded body — <b>has never once been rendered.</b> Every other scene in the game
        /// (W000–W012 via `WorldStubGenerator`, five arenas, the space lane) authors a theme from its
        /// own layout. This one did not, and it is the level the game opens in.
        ///
        /// The knock-on was worse than the sky. `VisualThemeProfile.skyVista` is the seam the whole
        /// vista system rides, so with no theme there was nowhere to hang W001 Toxic Venice — and both
        /// `SkyVistaLibrary` and `SkyVistaAuthor` say so in their own comments ("reserved: assigned
        /// once ToxicCity gains a theme", "the vista asset waits"). It was written down twice and
        /// waited anyway. That is the failure mode this project keeps paying for: a TODO in a comment
        /// is not a task, because nothing ever asks it whether it is done.
        ///
        /// Authoring the theme here closes both at once, and `SkyVistaAuditRules`' SKY_VISTA_UNWIRED
        /// warning is the thing that will notice if it ever regresses.
        /// </summary>
        private static void EnsureWorldRuntime(CityLayoutDefinition kit)
        {
            var go = PatcherUtil.EnsureRootObject("WorldRuntime", Vector3.zero);
            var wr = PatcherUtil.EnsureComponent<WorldRuntime>(go);

            // Same pipeline the other thirteen scenes use: theme from the layout, profile from the
            // theme. Falls back to the shared default if authoring somehow yields nothing, because a
            // WorldRuntime with no profile is a world with no fall net.
            var theme = ThemeAuthor.EnsureThemeAsset(kit);
            var profile = ThemeAuthor.EnsureWorldProfileAsset(kit, theme);
            if (profile == null) profile = AssetDatabase.LoadAssetAtPath<WorldProfile>(DefaultWorldProfilePath);

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

        private static WorldPackDefinition EnsureWorldPack(CityLayoutDefinition kit, Vector3 spawnPos)
        {
            var pack = AssetDatabase.LoadAssetAtPath<WorldPackDefinition>(WorldPackPath);
            if (pack == null)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(WorldPackPath));
                pack = ScriptableObject.CreateInstance<WorldPackDefinition>();
                AssetDatabase.CreateAsset(pack, WorldPackPath);
            }
            pack.packId = kit.cityId;
            pack.displayName = "Toxic City";
            pack.sceneName = SceneName;
            pack.spawnMarkers.Clear();
            pack.spawnMarkers.Add(new SpawnMarkerDefinition { markerId = "player", localPosition = spawnPos });
            EnsureRelayMachine(pack, kit);
            EditorUtility.SetDirty(pack);
            return pack;
        }

        /// <summary>
        /// THE RELAY THE CONTRACT ASKS YOU TO REPAIR.
        ///
        /// Step 4 of the Dockmaster's Bounty is RepairMachine("signal_relay"), and the pack spawned
        /// no machines at all — JobDirector materialises repairables from the PACK, so there was
        /// nothing in the world with that id and the contract could never advance past it. Steps 5
        /// (the drive out to the flats for half B) and 6 (return to the berth) sat behind a step
        /// that could not complete, and `toxiccity_complete` — which gates W002 — could never be
        /// granted. `WorldPackValidator` already predicts this exact failure in words: "Repair 'X'
        /// but the pack spawns no such machine — likely un-completable".
        ///
        /// Placed at the RelayVault the contract's own relay_node marker lives in, read off the LIVE
        /// layout rather than typed coordinates, so re-laying the city moves the machine with it.
        /// The cell spawns a few metres off the machine: the fetch is the beat, per the schema's
        /// own note, and it repeats the coupler lesson W000 taught rather than inventing a new verb.
        /// </summary>
        private static void EnsureRelayMachine(WorldPackDefinition pack, CityLayoutDefinition kit)
        {
            if (pack.machines == null) pack.machines = new List<MachineSpawnDefinition>();
            pack.machines.RemoveAll(m => m == null ||
                m.machineId == ToxicCityContractBuilder.RelayMachineId);

            Vector3 relay = new Vector3(-26f, kit.walkwayHeight, 8f); // fallback: the authored CanalRow anchor
            var canalRow = FindDistrict(kit, "CanalRow");
            if (canalRow != null)
            {
                relay = canalRow.anchor + new Vector3(0f, kit.walkwayHeight, 0f);
                for (int i = 0; i < canalRow.heroBuildings.Count; i++)
                {
                    var hero = canalRow.heroBuildings[i];
                    if (hero == null || hero.interiorMarkerId != "relay_node") continue;
                    relay = canalRow.anchor + hero.localPos + new Vector3(0f, kit.walkwayHeight, 0f);
                    break;
                }
            }

            pack.machines.Add(new MachineSpawnDefinition
            {
                machineId = ToxicCityContractBuilder.RelayMachineId,
                displayName = "signal relay",
                localPosition = relay,
                partItemId = "relay_cell",
                partLocalPosition = relay + new Vector3(3.2f, 0f, -2.4f),
            });
        }

        private static void EnsureTravelStation(CityLayoutDefinition kit)
        {
            var exitPack = EnsureExitPackAsset();
            if (exitPack == null) return;

            // Place by the shipyard berth — the eventual "leave on your ship" spot.
            Vector3 pos = kit.shipyard != null
                ? kit.shipyard.berthCenter + new Vector3(0f, kit.walkwayHeight + 0.1f, 6f)
                : new Vector3(0f, kit.walkwayHeight + 0.1f, -40f);

            var go = PatcherUtil.EnsureRootObject(ZiptideConstants.GoWorldTravelStation, pos);
            var station = PatcherUtil.EnsureComponent<WorldTravelStation>(go);
            var so = new SerializedObject(station);
            var listProp = so.FindProperty("destinationPacks");
            if (listProp != null)
            {
                listProp.arraySize = 1;
                listProp.GetArrayElementAtIndex(0).objectReferenceValue = exitPack;
                so.ApplyModifiedPropertiesWithoutUndo();
            }
        }

        private static void EnsureDispatchAndBoard(WorldPackDefinition pack, Vector3 spawnPos)
        {
            var jdGo = PatcherUtil.EnsureRootObject("JobDirector", Vector3.zero);
            var director = PatcherUtil.EnsureComponent<JobDirector>(jdGo);
            var so = new SerializedObject(director);
            PatcherUtil.SetObjectRef(so, "worldPack", pack);
            so.ApplyModifiedPropertiesWithoutUndo();

            var kioskGo = PatcherUtil.EnsureRootObject("DispatchKiosk", spawnPos + new Vector3(1.5f, 1.2f, 1.5f));
            PatcherUtil.EnsureComponent<DispatchKiosk>(kioskGo);
            PatcherUtil.EnsureComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>(kioskGo);
            var col = kioskGo.GetComponent<Collider>();
            if (col == null) { col = kioskGo.AddComponent<BoxCollider>(); col.isTrigger = true; }

            var boardGo = PatcherUtil.EnsureRootObject("ObjectiveBoard", spawnPos + new Vector3(-1.5f, 1.6f, 1.5f));
            PatcherUtil.EnsureComponent<ObjectiveBoard>(boardGo);
        }

        // The exit door points at the first OTHER enabled build scene (guaranteed in-build, avoids
        // TRAVEL_DEST_NOT_IN_BUILD). Replaced by the ship once the flight system lands.
        private const string ExitPackPath = "Assets/Ziptide/Content/Worlds/Packs/ToxicCityExit_WorldPack.asset";

        private static WorldPackDefinition EnsureExitPackAsset()
        {
            string dest = FirstOtherBuildSceneName();
            var pack = AssetDatabase.LoadAssetAtPath<WorldPackDefinition>(ExitPackPath);
            if (pack == null)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(ExitPackPath));
                pack = ScriptableObject.CreateInstance<WorldPackDefinition>();
                AssetDatabase.CreateAsset(pack, ExitPackPath);
            }
            pack.packId = "toxic_city_exit";
            pack.displayName = "Leave";
            pack.sceneName = dest;
            EditorUtility.SetDirty(pack);
            return pack;
        }

        /// <summary>
        /// The Leave door's destination. It must NEVER resolve to MilestoneA_GrabCube: that is a
        /// test room, it is absent from the Golden APK, and QuestRetryRouteRegressionTests pins
        /// this field to W000 with the message "must not point at MilestoneA_GrabCube".
        ///
        /// The old rule was "first enabled scene that is not _Boot or ToxicCity" - and
        /// MilestoneA_GrabCube is build index 1, so that rule returned it EVERY TIME. Each bake
        /// silently re-broke the door and the fix had to be re-applied by hand afterwards, which
        /// is exactly what happened twice on 2026-08-05. Prefer the story home scene, and never
        /// fall back to the test room.
        /// </summary>
        private static string FirstOtherBuildSceneName()
        {
            string firstOther = null;
            bool w000InBuild = false;

            foreach (var s in EditorBuildSettings.scenes)
            {
                if (!s.enabled || string.IsNullOrEmpty(s.path)) continue;
                string n = Path.GetFileNameWithoutExtension(s.path);
                if (n == "_Boot" || n == SceneName || n == ZiptideConstants.SceneTestRoom) continue;
                if (n == ZiptideConstants.SceneW000) w000InBuild = true;
                if (firstOther == null) firstOther = n;
            }

            if (w000InBuild) return ZiptideConstants.SceneW000;
            return firstOther ?? ZiptideConstants.SceneW000;
        }

        private static DistrictDef FindDistrict(CityLayoutDefinition kit, string id)
        {
            foreach (var d in kit.districts)
                if (d != null && d.id == id) return d;
            return null;
        }

        private static Scene OpenOrCreateScene()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return EditorSceneManager.GetActiveScene();
            if (File.Exists(ScenePath))
                return EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);
            Directory.CreateDirectory(Path.GetDirectoryName(ScenePath));
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            EditorSceneManager.SaveScene(scene, ScenePath);
            return scene;
        }
    }
}
#endif
