using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Placed in the _Boot scene. Presents the cold-boot Home Hub, then loads the selected first
    /// world via TravelCoordinator. _Boot is always the first scene in the build (index 0) and is
    /// never unloaded. All singletons (PlayerRigPersistence, TravelCoordinator, AudioDirector, etc.)
    /// live here.
    /// </summary>
    public class BootLoader : MonoBehaviour
    {
        [Tooltip("Override the first world scene. Leave empty to use ZiptideConstants.FirstWorldScene.")]
        [SerializeField] private string overrideFirstScene;

        private void Start()
        {
            string target = string.IsNullOrEmpty(overrideFirstScene)
                ? ZiptideConstants.FirstWorldScene
                : overrideFirstScene;

            // DS-01 cold-boot hold: _Boot has no floor, but the D2-patched rig arrives with gravity
            // locomotion + the fall net live — that combination is the boot fall/respawn loop. Hold
            // the rig (suspend locomotion, disarm the net, pin the pose) while the Home Hub owns the
            // scene; PlayerRigPersistence releases the hold after the chosen world's spawn settles.
            PlayerRigPersistence.SetBootHold(true);

            // The Home Hub is the one cold-boot gate. It never loads scenes itself: the callback keeps
            // TravelCoordinator as the sole travel path and preserves skipGate for the empty _Boot.
            var existing = FindObjectOfType<HomeHubRuntime>();
            var home = existing != null
                ? existing
                : new GameObject("__HOME_HUB_RUNTIME").AddComponent<HomeHubRuntime>();
            home.Configure(target, destination =>
            {
                Debug.Log("ZIPTIDE: BOOT_LOAD dest=" + destination);
                TravelCoordinator.TravelTo(destination, skipGate: true);
            });
        }
    }
}
