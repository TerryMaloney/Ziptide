using System.Collections.Generic;

namespace Ziptide.Content
{
    /// <summary>
    /// Pure structural sanity check for a <see cref="WorldPackDefinition"/> — the WorldPack twin of
    /// <see cref="CityLayoutDefinition.Validate"/>. Returns a list of problems (empty == valid) so bad
    /// pack data fails LOUD at world entry (`ZIPTIDE: PACK_VALIDATION_FAIL`) instead of silently
    /// no-opping jobs/travel. Headless (data-only) — EditMode-tested; JobDirector logs the results.
    /// </summary>
    public static class WorldPackValidator
    {
        public static List<string> Validate(WorldPackDefinition pack)
        {
            var issues = new List<string>();
            if (pack == null) { issues.Add("pack is null"); return issues; }

            if (string.IsNullOrEmpty(pack.packId)) issues.Add("empty packId");
            if (string.IsNullOrEmpty(pack.sceneName)) issues.Add("empty sceneName");

            if (pack.jobs != null)
            {
                for (int j = 0; j < pack.jobs.Count; j++)
                {
                    var job = pack.jobs[j];
                    if (job == null) { issues.Add("jobs[" + j + "] is null"); continue; }
                    string jid = string.IsNullOrEmpty(job.jobId) ? "jobs[" + j + "]" : job.jobId;
                    if (string.IsNullOrEmpty(job.jobId)) issues.Add(jid + " has empty jobId");
                    if (job.steps == null || job.steps.Count == 0) { issues.Add(jid + " has no steps"); continue; }

                    for (int s = 0; s < job.steps.Count; s++)
                    {
                        var step = job.steps[s];
                        string where = jid + ".steps[" + s + "]";
                        if (step == null) { issues.Add(where + " is null"); continue; }

                        if (step is GoToMarkerStepDefinition go && string.IsNullOrEmpty(go.markerId))
                            issues.Add(where + " GoToMarker has empty markerId");
                        if (step is DisableDronesCountStepDefinition dd && dd.count <= 0)
                            issues.Add(where + " DisableDrones count must be > 0");
                        if (step is ShootTargetsCountStepDefinition st && st.count <= 0)
                            issues.Add(where + " ShootTargets count must be > 0");
                        if (step is CollectItemIdCountStepDefinition ci)
                        {
                            if (string.IsNullOrEmpty(ci.itemId)) issues.Add(where + " Collect has empty itemId");
                            if (ci.count <= 0) issues.Add(where + " Collect count must be > 0");
                        }
                        if (step is DeliverToSocketStepDefinition de)
                        {
                            if (string.IsNullOrEmpty(de.socketId)) issues.Add(where + " Deliver has empty socketId");
                            if (string.IsNullOrEmpty(de.itemId)) issues.Add(where + " Deliver has empty itemId");
                            if (de.count <= 0) issues.Add(where + " Deliver count must be > 0");
                        }
                    }

                    if (job.reward != null)
                        for (int r = 0; r < job.reward.Count; r++)
                        {
                            var rc = job.reward[r];
                            if (rc != null && rc.amount != 0 && string.IsNullOrEmpty(rc.resourceId))
                                issues.Add(jid + ".reward[" + r + "] has amount but empty resourceId");
                        }
                }
            }

            if (pack.spawnMarkers != null)
                for (int m = 0; m < pack.spawnMarkers.Count; m++)
                {
                    var sm = pack.spawnMarkers[m];
                    if (sm == null) issues.Add("spawnMarkers[" + m + "] is null");
                    else if (string.IsNullOrEmpty(sm.markerId)) issues.Add("spawnMarkers[" + m + "] has empty markerId");
                }

            // Blank strings inside the gating lists are ignored by WorldGating, but they're almost
            // always an authoring slip — surface them.
            if (pack.flagsRequired != null)
                for (int i = 0; i < pack.flagsRequired.Count; i++)
                    if (string.IsNullOrEmpty(pack.flagsRequired[i]))
                        issues.Add("flagsRequired[" + i + "] is blank");
            if (pack.flagsGranted != null)
                for (int i = 0; i < pack.flagsGranted.Count; i++)
                    if (string.IsNullOrEmpty(pack.flagsGranted[i]))
                        issues.Add("flagsGranted[" + i + "] is blank");

            return issues;
        }
    }
}
