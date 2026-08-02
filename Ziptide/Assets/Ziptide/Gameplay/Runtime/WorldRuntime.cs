using UnityEngine;
using Ziptide.Content;
using Ziptide.Core;
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
            // World entry: advance idle/offline economy for this world FIRST (independent of the visual
            // profile — a world without a WorldProfile still has an economy keyed by its scene name).
            ResolveEconomyOnEntry();

            if (worldProfile == null) return;

            EnsureWorldDirector();
            ApplyGroundScale();
            EnsurePlayAreaBounds();
            EnsureThemeSwitchStation();

            if (_worldDirector != null && worldProfile.defaultTheme != null)
                _worldDirector.ApplyTheme(worldProfile.defaultTheme);
        }

        /// <summary>
        /// Resolve this world's idle economy on entry: ensure its WorldState exists, mark it discovered,
        /// and accrue offline mine/garden production since the last visit. The world is keyed by its
        /// scene name (== <see cref="WorldPackDefinition.sceneName"/>), the stable runtime world id.
        /// No-op until a profile exists; the pure math + WorldState handling live in ProfileEconomy.
        /// </summary>
        private void ResolveEconomyOnEntry()
        {
            var save = SaveSystem.Instance;
            if (save == null || save.Profile == null) return;

            string worldId = gameObject.scene.name;
            if (string.IsNullOrEmpty(worldId)) return;

            long now = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var r = ProfileEconomy.EnterWorld(save.Profile, worldId, now);
            Debug.Log("ZIPTIDE: ECON_RESOLVE world=" + worldId +
                      " mines=" + r.minesResolved +
                      " produced=" + r.totalProduced.ToString("0.##") +
                      " plotsReady=" + r.plotsReady);
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
        /// Move player to the authored spawn and zero motion. Called by FallRespawner and lethal hazards.
        /// </summary>
        public void RespawnPlayer(Transform playerRig)
        {
            if (playerRig == null) return;

            // Prefer the actual __SPAWN_PLAYER marker (on solid ground at the courtyard). The
            // WorldProfile.spawnPosition can sit over collider-disabled geometry (e.g. toxic sludge)
            // and create a respawn-fall loop. Marker first, profile only as fallback.
            Vector3 pos;
            Quaternion rot;
            var marker = FindSpawnMarker();
            if (marker != null)
            {
                pos = marker.transform.position;
                rot = marker.transform.rotation;
            }
            else if (worldProfile != null)
            {
                pos = worldProfile.spawnPosition;
                rot = Quaternion.Euler(worldProfile.spawnEuler);
            }
            else return;

            MovePlayer(playerRig, pos, rot);
        }

        /// <summary>
        /// Recover to an already-proven supported pose without changing the world's canonical spawn.
        /// FallRespawner owns the decision; WorldRuntime remains the single relocation/motion-clear path.
        /// </summary>
        public void RecoverPlayerAt(Transform playerRig, Vector3 position, Quaternion rotation)
        {
            if (playerRig == null) return;
            MovePlayer(playerRig, position, rotation);
        }

        private static void MovePlayer(Transform playerRig, Vector3 position, Quaternion rotation)
        {
            var cc = playerRig.GetComponentInChildren<CharacterController>(true);
            if (cc != null) cc.enabled = false;

            playerRig.SetPositionAndRotation(position, rotation);

            var bodies = playerRig.GetComponentsInChildren<Rigidbody>(true);
            for (int i = 0; i < bodies.Length; i++)
            {
                Rigidbody rb = bodies[i];
                if (rb == null) continue;
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            if (cc != null)
            {
                playerRig.SetPositionAndRotation(position, rotation);
                cc.enabled = true;
            }
        }

        private static SpawnMarkerRuntime FindSpawnMarker()
        {
            var all = Object.FindObjectsOfType<SpawnMarkerRuntime>();
            if (all == null || all.Length == 0) return null;
            foreach (var m in all)
                if (m != null && m.markerId == "player") return m;
            return all[0];
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
                if (_bounds != null) Destroy(_bounds.gameObject);
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
