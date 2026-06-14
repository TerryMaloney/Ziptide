using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>
    /// Runtime sky + planet rig. Creates procedural sky sphere and planet from a VisualThemeProfile.
    /// No colliders, no shadows; Quest-safe.
    /// </summary>
    public class SkyPlanetRig : MonoBehaviour
    {
        private const string URPUnlitShaderName = "Universal Render Pipeline/Unlit";
        private const string ShaderBaseMap = "_BaseMap";
        private const string ShaderBaseColor = "_BaseColor";
        private const int SkyTextureSize = 256;
        private const int PlanetTextureSize = 256;
        private const float SkySphereRadius = 500f;

        private GameObject _skyRoot;
        private GameObject _planetRoot;
        private Material _skyMaterial;
        private Material _planetMaterial;
        private Texture2D _skyTexture;
        private Texture2D _planetTexture;
        private Transform _playerTransform;
        private VisualThemeProfile _currentProfile;

        /// <summary>
        /// Apply theme and optionally set the transform to follow (e.g. XR Origin).
        /// Creates or updates sky and planet. Call from WorldDirector on Start.
        /// </summary>
        public void ApplyProfile(VisualThemeProfile profile, Transform playerTransform)
        {
            if (profile == null) return;

            _currentProfile = profile;
            _playerTransform = playerTransform != null ? playerTransform : transform;

            EnsureSky();
            EnsurePlanet();
            ApplySkyFromProfile(profile);
            ApplyPlanetFromProfile(profile);
        }

        private void Update()
        {
            if (_currentProfile == null || !_currentProfile.planet.followPlayer || _planetRoot == null || _playerTransform == null)
                return;

            Vector3 dir = _currentProfile.planet.direction.sqrMagnitude > 0.01f
                ? _currentProfile.planet.direction.normalized
                : new Vector3(0f, 0.5f, 0.866f);
            _planetRoot.transform.position = _playerTransform.position + dir * _currentProfile.planet.distance;

            if (_currentProfile.planet.rotationSpeed != 0f)
                _planetRoot.transform.Rotate(Vector3.up, _currentProfile.planet.rotationSpeed * Time.deltaTime);
        }

        private void EnsureSky()
        {
            if (_skyRoot != null) return;

            _skyRoot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _skyRoot.name = "SkySphere";
            _skyRoot.transform.SetParent(transform);
            _skyRoot.transform.localPosition = Vector3.zero;
            _skyRoot.transform.localRotation = Quaternion.identity;
            _skyRoot.transform.localScale = new Vector3(-SkySphereRadius * 2f, -SkySphereRadius * 2f, -SkySphereRadius * 2f);

            Object.Destroy(_skyRoot.GetComponent<Collider>());

            Shader unlit = Shader.Find(URPUnlitShaderName);
            if (unlit == null) return;
            _skyMaterial = new Material(unlit);
            _skyMaterial.name = "SkyPlanetRig_Sky";
            _skyMaterial.hideFlags = HideFlags.HideAndDontSave;
            _skyMaterial.renderQueue = 1000;

            var r = _skyRoot.GetComponent<Renderer>();
            if (r != null)
            {
                r.sharedMaterial = _skyMaterial;
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                r.receiveShadows = false;
            }
        }

        private void EnsurePlanet()
        {
            if (_planetRoot != null) return;

            _planetRoot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _planetRoot.name = "PlanetSphere";
            _planetRoot.transform.SetParent(transform);
            Object.Destroy(_planetRoot.GetComponent<Collider>());

            Shader unlit = Shader.Find(URPUnlitShaderName);
            if (unlit == null) return;
            _planetMaterial = new Material(unlit);
            _planetMaterial.name = "SkyPlanetRig_Planet";
            _planetMaterial.hideFlags = HideFlags.HideAndDontSave;

            var r = _planetRoot.GetComponent<Renderer>();
            if (r != null)
            {
                r.sharedMaterial = _planetMaterial;
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                r.receiveShadows = false;
            }
        }

        private void ApplySkyFromProfile(VisualThemeProfile profile)
        {
            if (_skyMaterial == null || profile == null) return;

            Gradient grad = profile.skyGradient;
            if (grad == null)
            {
                grad = new Gradient();
                grad.SetKeys(
                    new[] { new GradientColorKey(Color.gray, 0f), new GradientColorKey(new Color(0.4f, 0.5f, 0.7f), 1f) },
                    new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) });
            }

            if (_skyTexture == null)
                _skyTexture = new Texture2D(SkyTextureSize, SkyTextureSize);
            _skyTexture.wrapMode = TextureWrapMode.Clamp;
            for (int y = 0; y < SkyTextureSize; y++)
            {
                float t = y / (float)(SkyTextureSize - 1);
                Color c = grad.Evaluate(t);
                for (int x = 0; x < SkyTextureSize; x++)
                    _skyTexture.SetPixel(x, y, c);
            }
            _skyTexture.Apply();

            if (_skyMaterial.HasProperty(Shader.PropertyToID(ShaderBaseMap)))
                _skyMaterial.SetTexture(ShaderBaseMap, _skyTexture);
        }

        private void ApplyPlanetFromProfile(VisualThemeProfile profile)
        {
            if (_planetMaterial == null || _planetRoot == null || profile == null) return;

            VisualThemeProfile.PlanetSettings p = profile.planet;
            Vector3 dir = p.direction.sqrMagnitude > 0.01f ? p.direction.normalized : new Vector3(0f, 0.5f, 0.866f);
            _planetRoot.transform.position = _playerTransform != null
                ? _playerTransform.position + dir * p.distance
                : dir * p.distance;
            _planetRoot.transform.rotation = Quaternion.LookRotation(-dir);

            float worldRadius = Mathf.Tan(p.angularSizeDegrees * Mathf.Deg2Rad * 0.5f) * p.distance * 2f;
            _planetRoot.transform.localScale = Vector3.one * worldRadius;

            if (_planetTexture == null)
                _planetTexture = new Texture2D(PlanetTextureSize, PlanetTextureSize);
            _planetTexture.wrapMode = TextureWrapMode.Clamp;
            for (int y = 0; y < PlanetTextureSize; y++)
                for (int x = 0; x < PlanetTextureSize; x++)
                {
                    float stripe = Mathf.Sin((x + y) * 0.2f) * 0.5f + 0.5f;
                    _planetTexture.SetPixel(x, y, Color.Lerp(p.baseColor, p.accentColor, stripe));
                }
            _planetTexture.Apply();

            if (_planetMaterial.HasProperty(Shader.PropertyToID(ShaderBaseMap)))
                _planetMaterial.SetTexture(ShaderBaseMap, _planetTexture);
        }

        private void OnDestroy()
        {
            if (_skyMaterial != null) Destroy(_skyMaterial);
            if (_planetMaterial != null) Destroy(_planetMaterial);
            if (_skyTexture != null) Destroy(_skyTexture);
            if (_planetTexture != null) Destroy(_planetTexture);
        }
    }
}
