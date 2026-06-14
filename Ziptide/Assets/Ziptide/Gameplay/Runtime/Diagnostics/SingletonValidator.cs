using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Lightweight runtime check that critical singletons are unique after scene travel.
    /// Runs on every scene load and logs warnings if duplicates found.
    /// </summary>
    public class SingletonValidator : MonoBehaviour
    {
        private static SingletonValidator _instance;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            if (_instance == this)
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
                _instance = null;
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            CheckSingleton<XRInteractionManager>("XRInteractionManager");
            CheckSingleton<AudioDirector>("AudioDirector");
            CheckSingleton<PlayerRigPersistence>("PlayerRigPersistence");
        }

        private static void CheckSingleton<T>(string label) where T : Object
        {
            var all = FindObjectsOfType<T>();
            if (all.Length > 1)
                Debug.LogWarning("ZIPTIDE: DUP_SINGLETON " + label + " count=" + all.Length);
        }
    }
}
