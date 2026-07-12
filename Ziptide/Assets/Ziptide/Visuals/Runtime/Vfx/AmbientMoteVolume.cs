using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>
    /// Scene-owned ambient-air marker. It delegates all particles, pooling and budgets to VfxFactory;
    /// this component only owns one looping mote reservation for the lifetime of its world marker.
    /// It never follows the player and never creates content while Unity is in Edit Mode.
    /// </summary>
    public sealed class AmbientMoteVolume : MonoBehaviour
    {
        [SerializeField] private string recipeId = "motes_spore";

        private ParticleSystem _system;

        public string RecipeId => recipeId;
        public ParticleSystem System => _system;
        public bool IsRunning => _system != null && _system.gameObject.activeSelf;

        /// <summary>Editor-authoring seam. Only looping mote recipes are accepted at runtime.</summary>
        public void Configure(string id)
        {
            recipeId = id ?? string.Empty;
        }

        /// <summary>
        /// Start the authored looping volume. Public for deterministic EditMode verification; normal
        /// gameplay calls it automatically from OnEnable only while the application is playing.
        /// </summary>
        public bool TryStart()
        {
            if (_system != null && _system.gameObject.activeSelf) return true;

            VfxRecipeDefinition recipe = VfxLibrary.Get(recipeId);
            if (recipe == null || recipe.oneShot || recipe.kind != VfxKind.Motes)
            {
                Debug.LogWarning("ZIPTIDE: AMBIENT_MOTES_REJECTED id=" +
                                 (recipeId ?? "<null>") +
                                 " reason=missing_or_not_looping_motes");
                return false;
            }

            _system = VfxFactory.Spawn(recipeId, transform.position, Vector3.up);
            if (_system == null) return false;

            Debug.Log("ZIPTIDE: AMBIENT_MOTES_STARTED id=" + recipeId +
                      " position=" + transform.position);
            return true;
        }

        /// <summary>Return this looping reservation to the shared six-system pool.</summary>
        public bool StopVolume()
        {
            if (_system == null) return false;
            ParticleSystem system = _system;
            _system = null;
            return VfxFactory.Stop(system);
        }

        private void OnEnable()
        {
            if (Application.isPlaying) TryStart();
        }

        private void OnDisable()
        {
            StopVolume();
        }

        private void OnDestroy()
        {
            StopVolume();
        }
    }
}
