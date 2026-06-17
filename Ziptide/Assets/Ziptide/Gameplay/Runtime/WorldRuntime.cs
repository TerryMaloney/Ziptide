using UnityEngine;
using Ziptide.Content;
using Ziptide.Visuals;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Applies WorldProfile at runtime: theme, ground scale, bounds, respawn, and theme switching.
    /// </summary>
    public class WorldRuntime : MonoBehaviour
    {
        private const string GroundObjectName = "Ground";

        [SerializeField] private WorldProfile worldProfile;

        private WorldDirector _worldDirector;
        private PlayAreaBounds _bounds;
        private ThemeSwitchStation _themeStation;

        private void Start()
        {
            if (worldProfile == null) return;

            EnsureWorldDirector();
            ApplyGroundScale();
            EnsurePlayAreaBounds();
            EnsureThemeSwitchStation();

            if (_worldDirector != null && worldProfile.defaultTheme != null)
                _worldDirector.ApplyTheme(worldProfile.defaultTheme);
        }

        /// <summary>
        /// Switch the active theme (sky, planet, ground). Called by ThemeSwitchStation.
        /// </summary>
        public void ApplyTheme(VisualThemeProfile theme)
        {
            if (theme == null || _worldDirector == null) return;
            _worldDirector.ApplyTheme(theme);
        }

        /// <summary>
        /// Move player to spawn and zero velocity. Called by FallRespawner.
        /// </summary>
        public void RespawnPlayer(Transform playerRig)
        {
            if (worldProfile == null || playerRig == null) return;

            playerRig.position = worldProfile.spawnPosition;
            playerRig.rotation = Quaternion.Euler(worldProfile.spawnEuler);

            var rb = playerRig.GetComponentInChildren<Rigidbody>(true);
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            var cc = playerRig.GetComponentInChildren<CharacterController>(true);
            if (cc != null)
            {
                cc.enabled = false;
                playerRig.position = worldProfile.spawnPosition;
                cc.enabled = true;
            }
        }

        public WorldProfile WorldProfile => worldProfile;

        private void EnsureWorldDirector()
        {
            _worldDirector = FindObjectOfType<WorldDirector>();
            if (_worldDirector != null) return;

            GameObject go = new GameObject("WorldDirector");
            go.transform.SetParent(transform);
            _worldDirector = go.AddComponent<WorldDirector>();
        }

        private void ApplyGroundScale()
        {
            GameObject ground = GameObject.Find(GroundObjectName);
            if (ground == null) return;

            float w = Mathf.Max(0.1f, worldProfile.playAreaSize.x) / 10f;
            float l = Mathf.Max(0.1f, worldProfile.playAreaSize.y) / 10f;
            ground.transform.localScale = new Vector3(w, 1f, l);
            var pos = ground.transform.position;
            pos.y = worldProfile.groundY;
            ground.transform.position = pos;
        }

        private void EnsurePlayAreaBounds()
        {
            _bounds = GetComponentInChildren<PlayAreaBounds>(true);

            // Open worlds: NO invisible boundary box (the global fall-safety net covers falls). Only
            // build walls when a world explicitly opts into a roomscale box. This removes the
            // "invisible wall I can jump over" in large worlds like Toxic City.
            if (!worldProfile.usePlayAreaBounds)
            {
                if (_bounds != null) Destroy(_bounds.gameObject); // remove any existing boundary walls
                _bounds = null;
                return;
            }

            if (_bounds != null)
            {
                _bounds.Build(worldProfile.playAreaSize, worldProfile.groundY);
                return;
            }

            GameObject go = new GameObject("PlayAreaBounds");
            go.transform.SetParent(transform);
            go.transform.localPosition = Vector3.zero;
            _bounds = go.AddComponent<PlayAreaBounds>();
            _bounds.Build(worldProfile.playAreaSize, worldProfile.groundY);
        }

        private void EnsureThemeSwitchStation()
        {
            _themeStation = FindObjectOfType<ThemeSwitchStation>();
            if (_themeStation != null)
            {
                _themeStation.SetThemes(worldProfile.availableThemes, this);
                return;
            }

            GameObject go = new GameObject("ThemeSwitchStation");
            go.transform.SetParent(transform);
            go.transform.position = worldProfile.spawnPosition + Vector3.forward * 1.5f;
            _themeStation = go.AddComponent<ThemeSwitchStation>();
            _themeStation.SetThemes(worldProfile.availableThemes, this);
        }
    }
}
