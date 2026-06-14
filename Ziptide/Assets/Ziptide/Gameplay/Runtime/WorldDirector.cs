using UnityEngine;
using Ziptide.Visuals;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Gameplay-facing director that applies a VisualThemeProfile at runtime.
    /// Creates SkyPlanetRig and applies ground tint. Does not reference story/art by name.
    /// </summary>
    public class WorldDirector : MonoBehaviour
    {
        private const string GroundObjectName = "Ground";
        private const string ShaderBaseColor = "_BaseColor";

        [Tooltip("Theme to apply on Start. Assign in Editor or leave null to skip world dressing.")]
        [SerializeField] private VisualThemeProfile themeProfile;

        private SkyPlanetRig _skyPlanetRig;

        private void Start()
        {
            if (themeProfile == null) return;

            ApplyGroundTint();
            EnsureSkyPlanetRig();
            if (_skyPlanetRig != null)
                _skyPlanetRig.ApplyProfile(themeProfile, GetPlayerTransform());
        }

        /// <summary>
        /// Apply a theme at runtime (e.g. from Theme Switch Station). Updates sky, planet, and ground tint.
        /// </summary>
        public void ApplyTheme(VisualThemeProfile theme)
        {
            if (theme == null) return;
            themeProfile = theme;
            ApplyGroundTint();
            EnsureSkyPlanetRig();
            if (_skyPlanetRig != null)
                _skyPlanetRig.ApplyProfile(themeProfile, GetPlayerTransform());
        }

        private Transform GetPlayerTransform()
        {
            GameObject xrOrigin = GameObject.Find("XR Origin");
            if (xrOrigin != null) return xrOrigin.transform;
            Camera cam = Camera.main;
            if (cam != null) return cam.transform;
            Debug.LogWarning("[Ziptide] WorldDirector: no 'XR Origin' GameObject or main camera found; planet will follow this object instead of the player.");
            return transform;
        }

        private void EnsureSkyPlanetRig()
        {
            _skyPlanetRig = GetComponentInChildren<SkyPlanetRig>(true);
            if (_skyPlanetRig != null) return;

            GameObject rigGo = new GameObject("SkyRig");
            rigGo.transform.SetParent(transform);
            rigGo.transform.localPosition = Vector3.zero;
            rigGo.transform.localRotation = Quaternion.identity;
            rigGo.transform.localScale = Vector3.one;
            _skyPlanetRig = rigGo.AddComponent<SkyPlanetRig>();
        }

        private void ApplyGroundTint()
        {
            GameObject ground = GameObject.Find(GroundObjectName);
            if (ground == null)
            {
                Debug.LogWarning($"[Ziptide] WorldDirector: no GameObject named '{GroundObjectName}' found; ground tint not applied.");
                return;
            }

            Renderer r = ground.GetComponent<Renderer>();
            if (r == null) return;

            Material mat = r.material;
            if (mat != null && mat.HasProperty(Shader.PropertyToID(ShaderBaseColor)))
                mat.SetColor(ShaderBaseColor, themeProfile.groundTint);
        }
    }
}
