using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Attach to XR Origin (or player root). When player Y drops below WorldProfile.fallYThreshold, respawns via WorldRuntime.
    /// </summary>
    public class FallRespawner : MonoBehaviour
    {
        [SerializeField] private WorldRuntime worldRuntime;

        private void Update()
        {
            if (worldRuntime == null || worldRuntime.WorldProfile == null) return;
            var profile = worldRuntime.WorldProfile;
            if (!profile.respawnOnFall) return;

            if (transform.position.y < profile.fallYThreshold)
                worldRuntime.RespawnPlayer(transform);
        }

        /// <summary>
        /// Assign at runtime if not set in Inspector.
        /// </summary>
        public void SetWorldRuntime(WorldRuntime runtime)
        {
            worldRuntime = runtime;
        }
    }
}
