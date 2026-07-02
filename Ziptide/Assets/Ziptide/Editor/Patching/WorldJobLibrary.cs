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
            public List<(string kind, string markerId, Vector3 pos, int count)> steps = new List<(string, string, Vector3, int)>();
            public List<(string resourceId, double amount)> reward = new List<(string, double)>();
            public List<CollectibleSpawnDefinition> pickups = new List<CollectibleSpawnDefinition>();
            public List<MachineSpawnDefinition> machines = new List<MachineSpawnDefinition>();
            public string[] flagsRequired = new string[0];
            public string[] flagsGranted = new string[0];

            public Spec Go(string markerId, Vector3 pos) { steps.Add(("go", markerId, pos, 0)); return this; }
            public Spec Drones(int count) { steps.Add(("drones", null, Vector3.zero, count)); return this; }
            // Collect step: requires <count> pickups of <itemId> — pair with Pickup() entries below.
            public Spec Collect(string itemId, int count) { steps.Add(("collect", itemId, Vector3.zero, count)); return this; }
            // A physical pickup in the world (JobDirector spawns a CollectibleRuntime from pack data).
            public Spec Pickup(string itemId, Vector3 pos, string flagOnCollect = "", string label = "")
            {
                pickups.Add(new CollectibleSpawnDefinition
                {
                    itemId = itemId, localPosition = pos, flagOnCollect = flagOnCollect, displayName = label
                });
                return this;
            }
            // Repair step: requires the named machine's hands-on fix — pair with a Machine() entry.
            public Spec Repair(string machineId) { steps.Add(("repair", machineId, Vector3.zero, 1)); return this; }
            // A repairable machine in the world (JobDirector spawns a RepairableMachine from pack data).
            public Spec Machine(string machineId, Vector3 pos, string partItemId, Vector3 partPos, string label = "")
            {
                machines.Add(new MachineSpawnDefinition
                {
                    machineId = machineId, localPosition = pos, partItemId = partItemId,
                    partLocalPosition = partPos, displayName = label
                });
                return this;
            }
            public Spec Reward(string id, double amt) { reward.Add((id, amt)); return this; }
        }

        private static Spec SpecFor(string sceneName)
        {
            switch (sceneName)
            {
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
                    .Go("shaft_descent", new Vector3(10, 0.1f, 26))     // DeepShaft district
                    .Drones(3)                                           // the gallery swarm (drone stand-in)
                    // REAL collect step (was deferred): three mineral samples along the gallery route.
                    .Collect("mineral_sample", 3)
                    .Pickup("mineral_sample", new Vector3(4, 0.1f, 24), label: "mineral sample")
                    .Pickup("mineral_sample", new Vector3(-4, 0.1f, 28), label: "mineral sample")
                    .Pickup("mineral_sample", new Vector3(-10, 0.1f, 24), label: "mineral sample")
                    .Go("pump_house", new Vector3(-16, 0.1f, 22))        // ChamberA hero building
                    // THE M2 GATE LOOP: the contract title was always "Restart the Cistern Pumps" — now
                    // you actually do it with your hands: pull the pump's panel, fetch the valve from
                    // back at the shaft (the walk is the job), seat it, flip the power.
                    .Machine("cistern_pump", new Vector3(-16, 0.1f, 23), "pump_valve", new Vector3(10, 0.1f, 25), "cistern pump")
                    .Repair("cistern_pump")
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
                    .Go("mesa_base", new Vector3(-24, 0.1f, 14))
                    .Go("baffle_relay_a", new Vector3(4, 0.1f, 34))
                    .Go("baffle_relay_b", new Vector3(32, 0.1f, 18))
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
                    .Go("tomb_entry", new Vector3(0, 0.1f, 4))
                    .Go("junction_a", new Vector3(-18, 0.1f, 14))
                    .Go("junction_b", new Vector3(2, 0.1f, 28))
                    .Go("broadcast_core", new Vector3(-16, 0.1f, 40))
                    // THE FIRST TRANSMISSION FRAGMENT IS A PHYSICAL OBJECT (was deferred): you must
                    // pick it up at the broadcast core — FRAGMENT_T1_FOUND fires the moment you grab
                    // it (the pack's flagsGranted keeps it too as an idempotent completion backstop).
                    .Collect("transmission_fragment", 1)
                    .Pickup("transmission_fragment", new Vector3(-16, 0.1f, 41),
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
                    .Go("canopy_lift", new Vector3(-22, 0.1f, 14))
                    .Drones(4)
                    .Go("scrubber", new Vector3(2, 0.1f, 32))
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
                    .Go("flats_edge", new Vector3(0, 0.1f, 4))
                    .Go("prism_tower_a", new Vector3(-26, 0.1f, 15))
                    .Go("prism_tower_b", new Vector3(26, 0.1f, 17))
                    .Go("beam_collector", new Vector3(0, 0.1f, 35))
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
                    .Go("airlock", new Vector3(-20, 0.1f, 12))
                    .Go("fuel_rig", new Vector3(4, 0.1f, 27))
                    .Go("observation_deck", new Vector3(-18, 0.1f, 40)) // the Shell-grid viewport reveal
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
                    .Go("vault_door", new Vector3(-18, 0.1f, 12))
                    .Go("power_core", new Vector3(2, 0.1f, 26))
                    .Go("reader_hall", new Vector3(-16, 0.1f, 38))
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
                    .Go("wall_gate", new Vector3(-22, 0.1f, 12))
                    .Drones(6)                                       // the signature swarm world
                    .Go("pylon_array", new Vector3(0, 0.1f, 34))
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
                    .Go("shore_camp", new Vector3(0, 0.1f, 4))
                    .Go("turbine_a", new Vector3(-24, 0.1f, 17))
                    .Go("turbine_b", new Vector3(24, 0.1f, 19))
                    .Go("salt_works", new Vector3(0, 0.1f, 37))
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
                    .Go("tunnel_mouth", new Vector3(0, 0.1f, 4))
                    .Go("resonator_bank_a", new Vector3(-18, 0.1f, 14))
                    .Go("resonator_bank_b", new Vector3(4, 0.1f, 28))
                    .Go("miners_camp", new Vector3(-16, 0.1f, 40))
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
                    .Go("gantry", new Vector3(0, 0.1f, 4))
                    .Go("gate_core_a", new Vector3(-20, 0.1f, 16))
                    .Go("gate_core_b", new Vector3(6, 0.1f, 30))
                    .Go("launch_point", new Vector3(-16, 0.1f, 44))
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
                    step.stepLabel = "Go to " + s.markerId.Replace('_', ' ');
                    EditorUtility.SetDirty(step);
                    job.steps.Add(step);

                    // The marker itself is pack DATA — JobDirector creates Marker_<id> at runtime.
                    var marker = pack.spawnMarkers.Find(m => m != null && m.markerId == s.markerId);
                    if (marker == null)
                        pack.spawnMarkers.Add(new SpawnMarkerDefinition { markerId = s.markerId, localPosition = s.pos });
                    else
                        marker.localPosition = s.pos;
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

            // Physical pickups + machines are PACK data (JobDirector spawns the runtimes at scene start).
            pack.collectibles.Clear();
            foreach (var p in spec.pickups) pack.collectibles.Add(p);
            pack.machines.Clear();
            foreach (var m in spec.machines) pack.machines.Add(m);

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
