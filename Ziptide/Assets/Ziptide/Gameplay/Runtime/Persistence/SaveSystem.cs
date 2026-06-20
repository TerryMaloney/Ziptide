using System.IO;
using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Owns the live <see cref="PlayerProfile"/> and persists it to disk as JSON in
    /// Application.persistentDataPath. Intended to live in _Boot as a DontDestroyOnLoad singleton
    /// alongside TravelCoordinator / AudioDirector.
    ///
    /// NOT yet wired into _Boot or travel-autosave — that touches the boot/travel contract and is
    /// report-only (needs sign-off). The pure serialize/migrate logic lives in
    /// Ziptide.Core.ProfileSerializer and is covered by EditMode tests, so this layer is verified
    /// headlessly before it is ever placed in a scene.
    /// </summary>
    public class SaveSystem : MonoBehaviour
    {
        public static SaveSystem Instance { get; private set; }

        private const string FileName = "profile.json";
        private const string GoName = "SaveSystem";

        public PlayerProfile Profile { get; private set; }

        public static string SavePath => Path.Combine(Application.persistentDataPath, FileName);

        /// <summary>
        /// Self-bootstrap: guarantee a live profile exists at runtime without editing the _Boot scene.
        /// Creates the DontDestroyOnLoad singleton on first scene load if one wasn't placed manually.
        /// The Awake dup-guard makes this safe even if SaveSystem is later added to _Boot. Lets the
        /// economy/bounty payout use SaveSystem.Instance.Profile from anywhere.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureExists()
        {
            if (Instance != null) return;
            var go = new GameObject(GoName);
            go.AddComponent<SaveSystem>();
            Debug.Log("ZIPTIDE: SAVE_BOOTSTRAP (auto-created live profile holder)");
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Debug.Log("ZIPTIDE: DUP_SINGLETON name=" + GoName);
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }

        /// <summary>Load the profile from disk (or create a fresh one if absent/corrupt).</summary>
        public void Load()
        {
            string json = null;
            try { if (File.Exists(SavePath)) json = File.ReadAllText(SavePath); }
            catch (System.Exception e) { Debug.LogWarning("ZIPTIDE: SAVE_LOAD_FAIL " + e.Message); }

            Profile = ProfileSerializer.Deserialize(json);
            Debug.Log("ZIPTIDE: SAVE_LOAD playerId=" + Profile.playerId +
                      " resources=" + Profile.resources.Count + " flags=" + Profile.flags.Count);
        }

        /// <summary>Stamp the save time and write the profile to disk.</summary>
        public void Save()
        {
            if (Profile == null) Profile = ProfileSerializer.NewProfile();
            Profile.lastSavedAtUnix = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            try
            {
                File.WriteAllText(SavePath, ProfileSerializer.Serialize(Profile));
                Debug.Log("ZIPTIDE: SAVE_OK path=" + SavePath);
            }
            catch (System.Exception e) { Debug.LogWarning("ZIPTIDE: SAVE_FAIL " + e.Message); }
        }

        private void OnApplicationPause(bool paused) { if (paused) Save(); }
        private void OnApplicationQuit() { Save(); }
    }
}
