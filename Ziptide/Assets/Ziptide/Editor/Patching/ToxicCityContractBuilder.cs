#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Ziptide.Core;
using Ziptide.Content;

namespace Ziptide.Editor.Patching
{
    /// <summary>
    /// Authors the W001 "Dockmaster's Bounty" contract as data — the 5-beat Toxic City job from
    /// docs/storyboard/W001_ToxicCity/STORY.md, mapped onto the existing job-step types — and attaches it
    /// to the ToxicCity WorldPack so the DispatchKiosk (jobIndex 0) offers it. Completing it pays passage
    /// credits + sets the `toxiccity_complete` flag via JobDefinition.reward / JobRewards.Grant.
    ///
    /// Idempotent: load-or-create each asset, in place. Run once from the menu (cloud can't make .asset
    /// files), then commit the generated assets. Marker ids match what CityBuilder emits
    /// (Marker_&lt;interiorMarkerId&gt;): dispatch_inside, relay_node, shipyard_office; GoToMarker resolves by
    /// that GameObject name (JobDirector.CheckGoToMarker).
    /// </summary>
    public static class ToxicCityContractBuilder
    {
        private const string JobFolder = "Assets/Ziptide/Content/Jobs";
        private const string JobPath = JobFolder + "/ToxicCity_Contract.asset";

        // Tunables (data — change freely).
        private const string CompletionFlag = "toxiccity_complete";
        private const string RewardResourceId = "credits"; // passage credits
        private const double RewardAmount = 100;
        private const int DronesToClear = 5;               // Patrol_Market (3) + Patrol_Canal (2)

        /// <summary>The repairable the contract turns back on — authored into the ToxicCity pack.</summary>
        public const string RelayMachineId = "signal_relay";

        [MenuItem("Ziptide/Worlds/Build Toxic City Contract")]
        public static void Build()
        {
            EnsureFolder(JobFolder);

            // ── Steps (the playable beats; accept-at-kiosk and ship-out-via-door are not steps) ──
            var s1 = GoToMarker("ToxicCity_S1_Dispatch", "dispatch_inside",
                "Report to the Dockmaster at Dispatch", 2.5f);
            var s2 = DisableDrones("ToxicCity_S2_Drones", DronesToClear,
                "Clear the feral maintenance drones");
            var s3 = GoToMarker("ToxicCity_S3_Relay", "relay_node",
                "Find the downed signal relay", 2.5f);
            // The product contract's teaching law: W000 teaches SCAN and REPAIR on the gate coupler,
            // and W001 USES them on a real job. This step used to be a third walk-to-a-marker, which
            // meant the first level's contract never asked the player to do the thing the tutorial
            // spent its whole opening teaching.
            var s4 = RepairMachine("ToxicCity_S4_RelayRepair", RelayMachineId,
                "Seat the relay cell and bring the signal back online");
            // ⚖ THE EXPEDITION (Terry, 2026-07-29). The Dockmaster's second job: a survey site
            // outside the sea wall where the instruments died the same week the relay went wrong.
            // This is the step that sends the player through the breach in a vehicle — without it
            // the drive, the flats, and half B are content nothing ever asks for. The marker is
            // authored into the scene by FlatsSiteAuthor and resolved by JobDirector's scene
            // fallback; arrive distance is generous because you get there driving, not walking.
            var s5 = GoToMarker("ToxicCity_S5_Flats", FlatsSiteAuthor.SiteMarkerId,
                "Take the crawler out past the breach — the survey site on the flats", 6f);
            var s6 = GoToMarker("ToxicCity_S6_Return", "shipyard_office",
                "Return to your berth at the shipyard", 2.5f);

            // ── Job ──
            var job = AssetDatabase.LoadAssetAtPath<JobDefinition>(JobPath);
            bool created = job == null;
            if (created) job = ScriptableObject.CreateInstance<JobDefinition>();

            job.jobId = "toxiccity_contract";
            job.title = "Dockmaster's Bounty";
            job.steps = new List<JobStepDefinition> { s1, s2, s3, s4, s5, s6 };
            job.completionFlag = CompletionFlag;
            job.reward = new List<ResourceCost>
            {
                new ResourceCost { resourceId = RewardResourceId, amount = RewardAmount },
            };

            if (created) AssetDatabase.CreateAsset(job, JobPath);
            else EditorUtility.SetDirty(job);

            AttachToToxicCityPack(job);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("[Ziptide] Built Toxic City contract: " + JobPath
                + " (reward " + RewardAmount + " " + RewardResourceId + ", flag '" + CompletionFlag + "').");
            EditorUtility.DisplayDialog("Toxic City Contract",
                "Authored ToxicCity_Contract (6 steps incl. the relay repair, the flats expedition, and the bounty reward) and attached it to the ToxicCity "
                + "WorldPack as job 0.\n\nStill needed (T-Dog/runtime): JobDirector -> JobRewards.Grant on "
                + "completion, and ObjectiveBoard/RILL text.", "OK");
        }

        // Add the contract to the ToxicCity pack at index 0 (DispatchKiosk default jobIndex) — idempotent.
        private static void AttachToToxicCityPack(JobDefinition job)
        {
            var packPath = ZiptideConstants.PathToxicCityWorldPack;
            var pack = AssetDatabase.LoadAssetAtPath<WorldPackDefinition>(packPath);
            if (pack == null)
            {
                Debug.LogWarning("[Ziptide] ToxicCity WorldPack not found at " + packPath
                    + " — run 'Ziptide > Worlds > Build Toxic City' first, then re-run this. "
                    + "Contract asset was still created.");
                return;
            }

            var so = new SerializedObject(pack);
            var jobsProp = so.FindProperty("jobs");
            if (jobsProp == null) return;

            for (int i = 0; i < jobsProp.arraySize; i++)
                if (jobsProp.GetArrayElementAtIndex(i).objectReferenceValue == job)
                    return; // already attached

            jobsProp.InsertArrayElementAtIndex(0);
            jobsProp.GetArrayElementAtIndex(0).objectReferenceValue = job;
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(pack);
        }

        // ── helpers (load-or-create, mirror ScenePatcherD1's authoring) ──
        private static GoToMarkerStepDefinition GoToMarker(string assetName, string markerId, string label, float arrive)
        {
            var path = JobFolder + "/" + assetName + ".asset";
            var step = AssetDatabase.LoadAssetAtPath<GoToMarkerStepDefinition>(path);
            bool created = step == null;
            if (created) step = ScriptableObject.CreateInstance<GoToMarkerStepDefinition>();
            step.markerId = markerId;
            step.arriveDistance = arrive;
            step.stepLabel = label;
            if (created) AssetDatabase.CreateAsset(step, path); else EditorUtility.SetDirty(step);
            return step;
        }

        private static RepairMachineCountStepDefinition RepairMachine(string assetName, string machineId, string label)
        {
            var path = JobFolder + "/" + assetName + ".asset";
            var step = AssetDatabase.LoadAssetAtPath<RepairMachineCountStepDefinition>(path);
            bool created = step == null;
            if (created) step = ScriptableObject.CreateInstance<RepairMachineCountStepDefinition>();
            step.machineId = machineId;
            step.count = 1;
            step.stepLabel = label;
            if (created) AssetDatabase.CreateAsset(step, path); else EditorUtility.SetDirty(step);
            return step;
        }

        private static DisableDronesCountStepDefinition DisableDrones(string assetName, int count, string label)
        {
            var path = JobFolder + "/" + assetName + ".asset";
            var step = AssetDatabase.LoadAssetAtPath<DisableDronesCountStepDefinition>(path);
            bool created = step == null;
            if (created) step = ScriptableObject.CreateInstance<DisableDronesCountStepDefinition>();
            step.count = count;
            step.stepLabel = label;
            if (created) AssetDatabase.CreateAsset(step, path); else EditorUtility.SetDirty(step);
            return step;
        }

        private static void EnsureFolder(string folder)
        {
            if (AssetDatabase.IsValidFolder(folder)) return;
            if (!AssetDatabase.IsValidFolder("Assets/Ziptide/Content"))
                AssetDatabase.CreateFolder("Assets/Ziptide", "Content");
            AssetDatabase.CreateFolder("Assets/Ziptide/Content", "Jobs");
        }
    }
}
#endif
