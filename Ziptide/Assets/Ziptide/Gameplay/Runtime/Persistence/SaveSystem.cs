using System.IO;
using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Owns the live <see cref="PlayerProfile"/> and persists it to disk as JSON in
    /// Application.persistentDataPath. Lives beside TravelCoordinator / AudioDirector as a
    /// DontDestroyOnLoad singleton, self-bootstrapped by <see cref="EnsureExists"/> (no scene edit).
    ///
    /// Save points: app pause, app quit, and every scene travel (TravelCoordinator calls
    /// <see cref="AutosaveNow"/> before the departing scene unloads — HARDWIRING Phase 0.1).
    /// The pure serialize/migrate logic lives in Ziptide.Core.ProfileSerializer and is covered by
    /// EditMode tests, so this layer is verified headlessly.
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

        /// <summary>Load the newest COMPLETE profile version: main file, else the .bak the atomic
        /// writer keeps (a mid-write battery death can no longer wipe progress), else fresh.</summary>
        public void Load()
        {
            string json = SaveFileStore.ReadBestVersion(SavePath,
                text => ProfileSerializer.TryDeserialize(text, out _), out bool fromBackup);

            if (fromBackup)
                Debug.LogWarning("ZIPTIDE: SAVE_RECOVERED_FROM_BACKUP — main profile was corrupt, " +
                                 "the previous good version was restored");
            else if (json == null && File.Exists(SavePath))
                Debug.LogWarning("ZIPTIDE: SAVE_CORRUPT — profile and backup both unreadable, starting fresh");

            Profile = ProfileSerializer.Deserialize(json);
            Debug.Log("ZIPTIDE: SAVE_LOAD playerId=" + Profile.playerId +
                      " resources=" + Profile.resources.Count + " flags=" + Profile.flags.Count);
        }

        /// <summary>Stamp the save time and write the profile ATOMICALLY (tmp → swap, previous
        /// version demoted to .bak) — the main file is never half-written, at any instant.</summary>
        public void Save()
        {
            if (Profile == null) Profile = ProfileSerializer.NewProfile();
            Profile.lastSavedAtUnix = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            try
            {
                SaveFileStore.WriteAtomic(SavePath, ProfileSerializer.Serialize(Profile));
                Debug.Log("ZIPTIDE: SAVE_OK path=" + SavePath);
            }
            catch (System.Exception e) { Debug.LogWarning("ZIPTIDE: SAVE_FAIL " + e.Message); }
        }

        private void OnApplicationPause(bool paused) { if (paused) Save(); }
        private void OnApplicationQuit() { Save(); }

        /// <summary>
        /// Guarded autosave for hot paths (scene travel). Never throws and no-ops without a live
        /// instance, so a save hiccup can never strand the caller (the boot-strand class of bug).
        /// </summary>
        public static void AutosaveNow(string reason)
        {
            if (Instance == null) return;
            try
            {
                Instance.Save();
                Debug.Log("ZIPTIDE: SAVE_AUTOSAVE reason=" + reason);
            }
            catch (System.Exception e)
            {
                Debug.LogWarning("ZIPTIDE: SAVE_FAIL reason=" + reason + " " + e.Message);
            }
        }
    }
}
