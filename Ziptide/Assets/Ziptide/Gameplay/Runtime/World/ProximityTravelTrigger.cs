using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Failsafe walk-through travel trigger attached to door frames.
    /// When the persistent XR Origin root walks into the trigger volume,
    /// scene travel fires even if the ray interactor is not working.
    /// Optionally story-gated (1.4h): with <see cref="requiredFlag"/> set, travel refuses until the
    /// profile carries that flag — the same TRAVEL_LOCKED contract station doors already honor.
    /// Empty flag = every pre-existing door behaves exactly as before.
    /// </summary>
    public class ProximityTravelTrigger : MonoBehaviour
    {
        [Tooltip("Scene name to load when the player walks through.")]
        [SerializeField] private string destinationSceneName;

        [Tooltip("Seconds before this trigger can fire again (prevents double-fire on frame overlap).")]
        [SerializeField] private float cooldownSeconds = 3f;

        [Tooltip("Optional story gate: a ZiptideFlags name the profile must carry, or travel refuses " +
                 "with a TRAVEL_LOCKED log. Empty = ungated.")]
        [SerializeField] private string requiredFlag = "";

        private float _cooldownRemaining;

        private void Update()
        {
            if (_cooldownRemaining > 0f)
                _cooldownRemaining -= Time.deltaTime;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (_cooldownRemaining > 0f) return;
            if (string.IsNullOrEmpty(destinationSceneName)) return;

            // Only fire for the player's XR Origin.
            var rig = other.GetComponentInParent<PlayerRigPersistence>();
            if (rig == null) return;

            _cooldownRemaining = cooldownSeconds;

            if (!string.IsNullOrEmpty(requiredFlag))
            {
                var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
                if (profile == null || !profile.HasFlag(requiredFlag))
                {
                    // The same lock contract station doors log — the door holds, the cooldown stops
                    // the log spamming while the player stands in the trigger.
                    Debug.Log("ZIPTIDE: TRAVEL_LOCKED dest=" + destinationSceneName +
                              " missing=" + requiredFlag);
                    return;
                }
            }

            Debug.Log("ZIPTIDE: PROXIMITY_TRAVEL dest=" + destinationSceneName);

            // Anchor THE ZIPTIDE to this door frame — the tide pours out of the doorway.
            TravelCoordinator.TravelTo(destinationSceneName, transform.position);
        }

        /// <summary>Called by patchers to set the destination without opening the scene.</summary>
        public void SetDestination(string sceneName)
        {
            destinationSceneName = sceneName;
        }

        /// <summary>Called by patchers to story-gate this door (empty = ungated).</summary>
        public void SetRequirement(string flag)
        {
            requiredFlag = flag ?? "";
        }
    }
}
