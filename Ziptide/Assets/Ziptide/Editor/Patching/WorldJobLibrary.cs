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
            public string[] flagsRequired = new string[0];
            public string[] flagsGranted = new string[0];

            public Spec Go(string markerId, Vector3 pos) { steps.Add(("go", markerId, pos, 0)); return this; }
            public Spec Drones(int count) { steps.Add(("drones", null, Vector3.zero, count)); return this; }
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
                    .Go("pump_house", new Vector3(-16, 0.1f, 22))        // ChamberA hero building
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
                    .Reward("credits", 90).Reward("memory_shard", 1);

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
                else // "drones"
                {
                    var step = LoadOrCreate<DisableDronesCountStepDefinition>(stepPath);
                    step.count = s.count;
                    step.stepLabel = "Disable " + s.count + " drones";
                    EditorUtility.SetDirty(step);
                    job.steps.Add(step);
                }
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
