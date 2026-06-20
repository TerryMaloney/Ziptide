using Ziptide.Core;

namespace Ziptide.Content
{
    /// <summary>
    /// Pays out a job's completion reward into a <see cref="PlayerProfile"/>: grants each resource in
    /// <see cref="JobDefinition.reward"/> and sets the optional completion flag. Pure + headless (no
    /// scene, no Unity types beyond the data asset), so it's EditMode-testable; the runtime
    /// <c>JobDirector</c> calls this once when a job's last step finishes.
    /// </summary>
    public static class JobRewards
    {
        /// <summary>
        /// Grant <paramref name="job"/>'s reward to <paramref name="profile"/>. Null-safe and
        /// idempotent on the flag (calling twice still ends with the flag set and double resources —
        /// callers should grant once on completion).
        /// </summary>
        public static void Grant(JobDefinition job, PlayerProfile profile)
        {
            if (job == null || profile == null) return;

            if (job.reward != null)
            {
                for (int i = 0; i < job.reward.Count; i++)
                {
                    var r = job.reward[i];
                    if (r == null || string.IsNullOrEmpty(r.resourceId) || r.amount == 0) continue;
                    profile.AddResource(r.resourceId, r.amount);
                }
            }

            if (!string.IsNullOrEmpty(job.completionFlag))
                profile.SetFlag(job.completionFlag);
        }
    }
}
