#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using Ziptide.Content;
using Ziptide.Core;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// Table-driven job/story authoring for generated worlds (specs from docs/storyboard/WORLD_DATA.md;
    /// generalizes ToxicCityContractBuilder). For a world it authors: the JobDefinition + real step
    /// assets, the reward + completionFlag, the pack's flagsRequired/flagsGranted (story gating +
    /// grants — incl. W004's first Transmission fragment), and pack spawnMarkers for every GoToMarker
    /// target (JobDirector materializes them as Marker_&lt;id&gt; at runtime — pure data, no scene objects).
    ///
    /// Called from WorldStubGenerator.Populate on every (re)generation. CODE IS THE SOURCE OF TRUTH for
    /// step order/flags of generated worlds — tune numbers on the assets; restructure a contract here.
    /// TO ADD A WORLD'S CONTRACT: copy a Spec block below, add it to SpecFor. Steps supported:
    /// Go(markerId, position) and Drones(count) — Collect/Deliver land when collectible spawning does.
    /// </summary>
    public static class WorldJobLibrary
    {
        private const string JobFolder = "Assets/Ziptide/Content/Jobs/Generated";

        // ── The specs ──────────────────────────────────────────────────────────────────────────────
        private class Spec
        {
            public string jobId, title, completionFlag;
            public List<(string kind, string markerId, Vector3 pos, int count, string label)> steps
                = new List<(string, string, Vector3, int, string)>();
            public List<(string resourceId, double amount)> reward = new List<(string, double)>();
            // Spawn entries carry an optional POI anchor (P1g): atPoi != null → localPosition is an
            // OFFSET from that POI's terrain pad, resolved by EnsureJobsFor.
            public List<(CollectibleSpawnDefinition def, string atPoi)> pickups
                = new List<(CollectibleSpawnDefinition, string)>();
            public List<(MachineSpawnDefinition def, string atPoi, string partAtPoi)> machines
                = new List<(MachineSpawnDefinition, string, string)>();
            public List<(MineSpawnDefinition def, string atPoi)> mineRigs
                = new List<(MineSpawnDefinition, string)>();
            public string[] flagsRequired = new string[0];
            public string[] flagsGranted = new string[0];

            public Spec Go(string markerId, Vector3 pos, string label = null)
            { steps.Add(("go", markerId, pos, 0, label)); return this; }
            // P1g: route the contract THROUGH the POI network. "poi_<id>" pack markers are synced from
            // kit.pois by WorldStubGenerator — no raw coordinates in the spec at all.
            public Spec GoPoi(string poiId, string label)
            { steps.Add(("go", "poi_" + poiId, Vector3.zero, 0, label)); return this; }
            public Spec Drones(int count) { steps.Add(("drones", null, Vector3.zero, count, null)); return this; }
            // Collect step: requires <count> pickups of <itemId> — pair with Pickup() entries below.
            public Spec Collect(string itemId, int count) { steps.Add(("collect", itemId, Vector3.zero, count, null)); return this; }
            // A physical pickup in the world (JobDirector spawns a CollectibleRuntime from pack data).
            public Spec Pickup(string itemId, Vector3 pos, string flagOnCollect = "", string label = "")
            {
                pickups.Add((new CollectibleSpawnDefinition
                {
                    itemId = itemId, localPosition = pos, flagOnCollect = flagOnCollect, displayName = label
                }, null));
                return this;
            }
            public Spec PickupAtPoi(string itemId, string poiId, Vector3 offset, string flagOnCollect = "", string label = "")
            {
                pickups.Add((new CollectibleSpawnDefinition
                {
                    itemId = itemId, localPosition = offset, flagOnCollect = flagOnCollect, displayName = label
                }, poiId));
                return this;
            }
            // Repair step: requires the named machine's hands-on fix — pair with a Machine() entry.
            public Spec Repair(string machineId) { steps.Add(("repair", machineId, Vector3.zero, 1, null)); return this; }
            // A repairable machine in the world (JobDirector spawns a RepairableMachine from pack data).
            public Spec Machine(string machineId, Vector3 pos, string partItemId, Vector3 partPos, string label = "")
            {
                machines.Add((new MachineSpawnDefinition
                {
                    machineId = machineId, localPosition = pos, partItemId = partItemId,
                    partLocalPosition = partPos, displayName = label
                }, null, null));
                return this;
            }
            // Machine at one POI, its missing part at ANOTHER — the walk between pockets is the job.
            public Spec MachineAtPoi(string machineId, string poiId, Vector3 offset, string partItemId,
                string partAtPoi, Vector3 partOffset, string label = "")
            {
                machines.Add((new MachineSpawnDefinition
                {
                    machineId = machineId, localPosition = offset, partItemId = partItemId,
                    partLocalPosition = partOffset, displayName = label
                }, poiId, partAtPoi));
                return this;
            }
            // A placed extractor (idle economy made visible; JobDirector spawns a MiningRigRuntime).
            public Spec Mine(string id, string resourceId, double rate, double cap, Vector3 pos)
            {
                mineRigs.Add((new MineSpawnDefinition
                {
                    id = id, resourceId = resourceId, ratePerSecond = rate, storageCap = cap, localPosition = pos
                }, null));
                return this;
            }
            public Spec MineAtPoi(string id, string resourceId, double rate, double cap, string poiId, Vector3 offset)
            {
                mineRigs.Add((new MineSpawnDefinition
                {
                    id = id, resourceId = resourceId, ratePerSecond = rate, storageCap = cap, localPosition = offset
                }, poiId));
                return this;
            }
            public Spec Reward(string id, double amt) { reward.Add((id, amt)); return this; }
        }

        /// <summary>Coverage gate hook (EXCELLENCE_MAP gap #3): does this scene carry authored
        /// jobs/beats? Additive — the story-beat coverage test asserts every shipped story world
        /// answers true, so a new world can't ship beat-less by accident.</summary>
        public static bool HasJobsFor(string sceneName) => SpecFor(sceneName) != null;

        private static Spec SpecFor(string sceneName)
        {
            switch (sceneName)
            {
                case "W000_DriftIn":
                    return new Spec
                    {
                        jobId = "w000_onboard",
                        title = "Cast Off",
                        // Ships today per the WORLD_DATA record: no payout, TUTORIAL_COMPLETE on finish.
                        completionFlag = ZiptideFlags.TUTORIAL_COMPLETE,
                        flagsRequired = new string[0], // the entry world
                        flagsGranted = new[] { ZiptideFlags.TUTORIAL_COMPLETE, ZiptideFlags.FIRST_TRAVEL,
                                               ZiptideFlags.C1_W001_RILL_BOOT },
                    }
                    // The record's helm→coupler→gate route, teaching each verb the game now HAS:
                    .Go("helm", new Vector3(0, 0.1f, 20))                       // move/look — walk to your ship
                    .Collect("guild_manifest", 1)                               // grab — take your papers
                    .Pickup("guild_manifest", new Vector3(-4, 0.1f, 2), label: "guild manifest")
                    .Machine("gate_coupler", new Vector3(6, 0.1f, 14), "coupler_cell",
                             new Vector3(-3, 0.1f, 16), "gate coupler")         // repair — prime the coupler
                    .Repair("gate_coupler")
                    .Reward("credits", 0);
                    // Then RILL points you at the ship: boarding + departing IS the travel lesson.

                case "W002_DryCistern":
                    return new Spec
                    {
                        jobId = "w002_pumps",
                        title = "Restart the Cistern Pumps",
                        completionFlag = ZiptideFlags.W002_COMPLETE,
                        // W001's contract gates entry (W000 tutorial is parked — see WORLD_DATA note).
                        flagsRequired = new[] { "toxiccity_complete" },
                        flagsGranted = new[] { ZiptideFlags.W002_COMPLETE },
                    }
                    // P1g: the contract now WALKS THE WORLD — every leg lands on a POI pocket.
                    .GoPoi("camp_a", "Reach the gallery camp")
                    .Drones(3)                                           // the gallery swarm (drone stand-in)
                    .Collect("mineral_sample", 3)                        // samples strewn across three pockets
                    .PickupAtPoi("mineral_sample", "ruin", new Vector3(2f, 0.1f, 1f), label: "mineral sample")
                    .PickupAtPoi("mineral_sample", "camp_a", new Vector3(-3f, 0.1f, 2f), label: "mineral sample")
                    .PickupAtPoi("mineral_sample", "grove", new Vector3(1f, 0.1f, -3f), label: "mineral sample")
                    .GoPoi("works", "Reach the pump works")
                    // THE M2 GATE LOOP at POI scale: the pump lives at the works, its valve waits back
                    // at the ruin cache — the walk between pockets IS the job.
                    .MachineAtPoi("cistern_pump", "works", new Vector3(4.5f, 0.1f, -2f),
                                  "pump_valve", "ruin", new Vector3(-2f, 0.1f, 1.5f), "cistern pump")
                    .Repair("cistern_pump")
                    // Idle economy made visible: an extractor at the works — it produces while you're
                    // away (ECON_RESOLVE) and pays out when you select the hopper.
                    .MineAtPoi("cistern_extractor", "mineral", 0.05, 40, "works", new Vector3(-4f, 0.1f, 3f))
                    .GoPoi("story", "Reach the old basin heart")
                    .Reward("credits", 60).Reward("mineral", 5);

                case "W003_GlassShelf":
                    return new Spec
                    {
                        jobId = "w003_baffles",
                        title = "Raise the Wind Baffles",
                        completionFlag = ZiptideFlags.W003_COMPLETE,
                        flagsRequired = new[] { ZiptideFlags.W002_COMPLETE },
                        flagsGranted = new[] { ZiptideFlags.W003_COMPLETE },
                    }
                    .GoPoi("camp_a", "Reach the mesa base camp")
                    .GoPoi("works", "Raise baffle relay one")
                    .GoPoi("ruin", "Raise baffle relay two")
                    .GoPoi("story", "Reach the shelf's edge shrine")
                    .Reward("credits", 70).Reward("crystal", 4);

                case "W004_BroadcastTomb":
                    return new Spec
                    {
                        jobId = "w004_broadcast",
                        title = "Restore the Broadcast Spine",
                        completionFlag = ZiptideFlags.W004_COMPLETE,
                        flagsRequired = new[] { ZiptideFlags.W003_COMPLETE },
                        // Ch.1 capstone: RILL's question + Signal threshold + THE FIRST TRANSMISSION
                        // FRAGMENT — granting FRAGMENT_T1_FOUND makes TransmissionProgress raise
                        // clarity tier 1 (the de-garble UI's first stage).
                        flagsGranted = new[]
                        {
                            ZiptideFlags.C1_WAKE_GUILD_INTRO,
                            ZiptideFlags.C1_W004_RILL_ASKED_CARGO,
                            ZiptideFlags.SIGNAL_THRESHOLD_1,
                            ZiptideFlags.FRAGMENT_T1_FOUND,
                            ZiptideFlags.W004_COMPLETE,
                        },
                    }
                    .GoPoi("ruin", "Trace the dead junction")
                    .GoPoi("cave", "Follow the spine underground")
                    .GoPoi("story", "Reach the broadcast core")
                    // THE FIRST TRANSMISSION FRAGMENT IS A PHYSICAL OBJECT: it waits on the story
                    // anchor's dais — FRAGMENT_T1_FOUND fires the moment you grab it (the pack's
                    // flagsGranted keeps it too as an idempotent completion backstop).
                    .Collect("transmission_fragment", 1)
                    .PickupAtPoi("transmission_fragment", "story", new Vector3(0f, 0.95f, 0f),
                            flagOnCollect: ZiptideFlags.FRAGMENT_T1_FOUND, label: "?? recording")
                    .Reward("credits", 90).Reward("memory_shard", 1);

                case "W005_OxidizedCanopy":
                    return new Spec
                    {
                        jobId = "w005_harvest",
                        title = "Scrub the Spores, Work the Canopy",   // Mara's first contract
                        completionFlag = ZiptideFlags.W005_COMPLETE,
                        flagsRequired = new[] { ZiptideFlags.W004_COMPLETE },
                        flagsGranted = new[] { ZiptideFlags.W005_COMPLETE, ZiptideFlags.C2_W005_JOB_COMPLETE },
                    }
                    .GoPoi("grove", "Reach the canopy grove")
                    .Drones(4)
                    .GoPoi("works", "Restart the spore scrubber")
                    .GoPoi("story", "Reach the overgrown gate")
                    .Reward("credits", 110).Reward("spore", 6);

                case "W006_MirrorFlats":
                    return new Spec
                    {
                        jobId = "w006_prisms",
                        title = "Align the Prism Towers",
                        completionFlag = ZiptideFlags.W006_COMPLETE,
                        flagsRequired = new[] { ZiptideFlags.W005_COMPLETE },
                        flagsGranted = new[] { ZiptideFlags.W006_COMPLETE },
                    }
                    .GoPoi("ruin", "Align prism tower one")
                    .GoPoi("works", "Align prism tower two")
                    .GoPoi("cave", "Find the buried reflector")
                    .GoPoi("story", "Reach the beam collector")
                    .Reward("credits", 100).Reward("prism", 3);

                case "W007_SableStation":
                    return new Spec
                    {
                        jobId = "w007_fuelrig",
                        title = "Repair Sable's Fuel Rig",
                        completionFlag = ZiptideFlags.W007_COMPLETE,
                        flagsRequired = new[] { ZiptideFlags.W006_COMPLETE },
                        // First Sable contact seeds the Ch.4 arc.
                        flagsGranted = new[] { ZiptideFlags.W007_COMPLETE, ZiptideFlags.C4_SABLE_INTRO },
                    }
                    .GoPoi("ruin", "Reach the crash perimeter")
                    .GoPoi("works", "Repair the fuel rig")
                    .GoPoi("story", "Reach the observation deck") // the Shell-grid viewport reveal
                    .Reward("credits", 120).Reward("fuel_cell", 1);

                case "W008_SealedArchive":
                    return new Spec
                    {
                        jobId = "w008_archive",
                        title = "Restore the Archive",
                        completionFlag = ZiptideFlags.W008_COMPLETE,
                        flagsRequired = new[] { ZiptideFlags.W007_COMPLETE },
                        // The Architects get NAMED here — Ch.2's lore turn.
                        flagsGranted = new[] { ZiptideFlags.W008_COMPLETE, ZiptideFlags.C2_ARCHITECTS_NAMED },
                    }
                    .GoPoi("ruin", "Find the vault door")
                    .GoPoi("works", "Restart the power core")
                    .GoPoi("story", "Enter the reader hall")
                    .Reward("credits", 120).Reward("data_chip", 2);

                case "W009_Chitinwall":
                    return new Spec
                    {
                        jobId = "w009_pylons",
                        title = "Raise the Swarm-Deterrent Pylons",
                        completionFlag = ZiptideFlags.W009_COMPLETE,
                        flagsRequired = new[] { ZiptideFlags.W008_COMPLETE },
                        // RILL's memory audibly glitches here — Ch.2's ★ beat.
                        flagsGranted = new[] { ZiptideFlags.W009_COMPLETE, ZiptideFlags.C2_W009_RILL_MISIDENTIFIED },
                    }
                    .GoPoi("camp_a", "Reach the wall gate camp")
                    .Drones(6)                                       // the signature swarm world
                    .GoPoi("camp_b", "Clear the second nest")
                    .GoPoi("story", "Raise the pylon array")
                    .Reward("credits", 130).Reward("carapace", 4);

                case "W010_TidalArray":
                    return new Spec
                    {
                        jobId = "w010_turbines",
                        title = "Restart the Tidal Turbines",
                        completionFlag = ZiptideFlags.W010_COMPLETE,
                        flagsRequired = new[] { ZiptideFlags.W009_COMPLETE },
                        flagsGranted = new[] { ZiptideFlags.W010_COMPLETE, ZiptideFlags.SIGNAL_THRESHOLD_2 },
                    }
                    .GoPoi("camp_a", "Reach the shore camp")
                    .GoPoi("ruin", "Restart turbine one")
                    .GoPoi("works", "Restart turbine two")
                    .GoPoi("story", "Reach the salt works")
                    .Reward("credits", 120).Reward("salt", 4);

                case "W011_TheHum":
                    return new Spec
                    {
                        jobId = "w011_resonators",
                        title = "Tune the Resonator Banks",
                        completionFlag = ZiptideFlags.W011_COMPLETE,
                        flagsRequired = new[] { ZiptideFlags.W010_COMPLETE },
                        flagsGranted = new[] { ZiptideFlags.W011_COMPLETE },
                    }
                    .GoPoi("cave", "Enter the tunnel mouth")
                    .GoPoi("ruin", "Tune resonator bank one")
                    .GoPoi("works", "Tune resonator bank two")
                    .GoPoi("story", "Reach the miners' camp")
                    .Reward("credits", 110).Reward("resonator", 3);

                case "W012_MarasLastJump":
                    return new Spec
                    {
                        jobId = "w012_stabilize",
                        title = "Stabilize the Failing Gate",
                        completionFlag = ZiptideFlags.C2_CONTAINMENT_REVEALED,
                        flagsRequired = new[] { ZiptideFlags.W011_COMPLETE },
                        // Chapter 2 capstone: Mara's ship bounces off the Shell — the cage is REAL.
                        flagsGranted = new[] { ZiptideFlags.W012_COMPLETE, ZiptideFlags.C2_CONTAINMENT_REVEALED },
                    }
                    .GoPoi("camp_a", "Reach the launch gantry")
                    .GoPoi("works", "Stabilize gate core one")
                    .GoPoi("cave", "Stabilize gate core two")
                    .GoPoi("story", "Reach Mara's launch point")
                    .Reward("credits", 150).Reward("jump_core", 1);

                default:
                    return null;
            }
        }

        // ── Authoring (idempotent create-or-update; called per-world from WorldStubGenerator) ─────
        private const float GoArriveDistance = 3.5f; // generous for graybox marker-at-building targets

        public static void EnsureJobsFor(CityLayoutDefinition kit, WorldPackDefinition pack)
        {
            var spec = SpecFor(kit.sceneName);
            if (spec == null || pack == null) return;

            // P1g: resolve POI anchors — "AtPoi" spawn offsets and GoPoi markers land on the POI's
            // terrain pad. Non-experience worlds have no POIs; offsets pass through untouched.
            var poiPos = new Dictionary<string, Vector3>();
            if (kit.pois != null)
                foreach (var p in kit.pois)
                    if (p != null && !string.IsNullOrEmpty(p.id))
                        poiPos[p.id] = new Vector3(p.position.x,
                            WorldExperienceBuilder.HeightAt(kit, p.position.x, p.position.z) + 0.1f,
                            p.position.z);
            Vector3 Resolve(Vector3 offset, string atPoi)
            {
                if (string.IsNullOrEmpty(atPoi)) return offset;
                if (poiPos.TryGetValue(atPoi, out var basePos)) return basePos + offset;
                Debug.LogWarning("[Ziptide] WorldJobLibrary: spec for " + kit.sceneName +
                                 " targets unknown POI '" + atPoi + "' — using raw offset.");
                return offset;
            }

            Directory.CreateDirectory(JobFolder);
            string basePath = JobFolder + "/" + kit.sceneName;

            var job = LoadOrCreate<JobDefinition>(basePath + "_Job.asset");
            job.jobId = spec.jobId;
            job.title = spec.title;
            job.completionFlag = spec.completionFlag;

            job.steps.Clear();
            for (int i = 0; i < spec.steps.Count; i++)
            {
                var s = spec.steps[i];
                string stepPath = basePath + "_S" + (i + 1) + ".asset";
                if (s.kind == "go")
                {
                    var step = LoadOrCreate<GoToMarkerStepDefinition>(stepPath);
                    step.markerId = s.markerId;
                    step.arriveDistance = GoArriveDistance;
                    step.stepLabel = s.label ?? "Go to " + s.markerId.Replace('_', ' ');
                    EditorUtility.SetDirty(step);
                    job.steps.Add(step);

                    // The marker itself is pack DATA — JobDirector creates Marker_<id> at runtime.
                    // "poi_" markers are already synced (with terrain heights) by WorldStubGenerator.
                    if (!s.markerId.StartsWith("poi_"))
                    {
                        var marker = pack.spawnMarkers.Find(m => m != null && m.markerId == s.markerId);
                        if (marker == null)
                            pack.spawnMarkers.Add(new SpawnMarkerDefinition { markerId = s.markerId, localPosition = s.pos });
                        else
                            marker.localPosition = s.pos;
                    }
                }
                else if (s.kind == "collect")
                {
                    // markerId carries the itemId for collect steps (same tuple, no schema churn).
                    var step = LoadOrCreate<CollectItemIdCountStepDefinition>(stepPath);
                    step.itemId = s.markerId;
                    step.count = s.count;
                    step.stepLabel = "Collect " + s.count + " " + s.markerId.Replace('_', ' ');
                    EditorUtility.SetDirty(step);
                    job.steps.Add(step);
                }
                else if (s.kind == "repair")
                {
                    // markerId carries the machineId for repair steps.
                    var step = LoadOrCreate<RepairMachineCountStepDefinition>(stepPath);
                    step.machineId = s.markerId;
                    step.count = s.count;
                    step.stepLabel = "Repair the " + s.markerId.Replace('_', ' ');
                    EditorUtility.SetDirty(step);
                    job.steps.Add(step);
                }
                else // "drones"
                {
                    var step = LoadOrCreate<DisableDronesCountStepDefinition>(stepPath);
                    step.count = s.count;
                    step.stepLabel = "Disable " + s.count + " drones";
                    EditorUtility.SetDirty(step);
                    job.steps.Add(step);
                }
            }

            // Physical pickups + machines + mines are PACK data (JobDirector spawns the runtimes at
            // scene start). AtPoi entries resolve to their POI's terrain pad here.
            pack.collectibles.Clear();
            foreach (var (def, atPoi) in spec.pickups)
            {
                def.localPosition = Resolve(def.localPosition, atPoi);
                pack.collectibles.Add(def);
            }
            pack.machines.Clear();
            foreach (var (def, atPoi, partAtPoi) in spec.machines)
            {
                def.localPosition = Resolve(def.localPosition, atPoi);
                def.partLocalPosition = Resolve(def.partLocalPosition, partAtPoi ?? atPoi);
                pack.machines.Add(def);
            }
            pack.mines.Clear();
            foreach (var (def, atPoi) in spec.mineRigs)
            {
                def.localPosition = Resolve(def.localPosition, atPoi);
                pack.mines.Add(def);
            }

            job.reward.Clear();
            foreach (var (resourceId, amount) in spec.reward)
                job.reward.Add(new ResourceCost { resourceId = resourceId, amount = amount });
            EditorUtility.SetDirty(job);

            if (!pack.jobs.Contains(job)) { pack.jobs.Clear(); pack.jobs.Add(job); }
            pack.flagsRequired.Clear(); pack.flagsRequired.AddRange(spec.flagsRequired);
            pack.flagsGranted.Clear(); pack.flagsGranted.AddRange(spec.flagsGranted);
            EditorUtility.SetDirty(pack);
        }

        private static T LoadOrCreate<T>(string path) where T : ScriptableObject
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(path);
            if (asset != null) return asset;
            asset = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(asset, path);
            return asset;
        }
    }
}
#endif
