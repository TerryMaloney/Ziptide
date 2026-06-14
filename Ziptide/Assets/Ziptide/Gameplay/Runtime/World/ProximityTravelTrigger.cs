using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Failsafe walk-through travel trigger attached to door frames.
    /// When the persistent XR Origin root walks into the trigger volume,
    /// scene travel fires even if the ray interactor is not working.
    /// </summary>
    public class ProximityTravelTrigger : MonoBehaviour
    {
        [Tooltip("Scene name to load when the player walks through.")]
        [SerializeField] private string destinationSceneName;

        [Tooltip("Seconds before this trigger can fire again (prevents double-fire on frame overlap).")]
        [SerializeField] private float cooldownSeconds = 3f;

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

            Debug.Log("ZIPTIDE: PROXIMITY_TRAVEL dest=" + destinationSceneName);

            TravelCoordinator.TravelTo(destinationSceneName);
        }

        /// <summary>Called by patchers to set the destination without opening the scene.</summary>
        public void SetDestination(string sceneName)
        {
            destinationSceneName = sceneName;
        }
    }
}
