using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Placed in the _Boot scene. On Start, loads the first world scene via TravelCoordinator.
    /// _Boot is always the first scene in the build (index 0) and is never unloaded.
    /// All singletons (PlayerRigPersistence, TravelCoordinator, AudioDirector, etc.) live here.
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

            Debug.Log("ZIPTIDE: BOOT_LOAD dest=" + target);

            // Use TravelCoordinator for consistency with all mid-game travel.
            // On first boot, inventory is empty so save/restore is a no-op.
            TravelCoordinator.TravelTo(target);
        }
    }
}
