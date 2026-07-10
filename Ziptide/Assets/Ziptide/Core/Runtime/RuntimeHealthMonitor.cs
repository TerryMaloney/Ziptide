using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ziptide.Core
{
    /// <summary>
    /// THE GAME'S OWN VITALS (the forgotten architecture, 2026-07-10) — frame-time and memory
    /// telemetry plus the travel janitor. Why it exists: 57 files create runtime Materials/Textures/
    /// AudioClips and NOTHING ever called Resources.UnloadUnusedAssets — Unity never auto-destroys
    /// runtime-created resources, so every travel leaked orphans and long VR sessions marched toward
    /// OOM. As the richness bar multiplies procedural content, this component is the counterweight:
    ///   · FPS vitals: average, 1%-low, dropped frames vs the 72Hz budget (FrameStats pure core).
    ///   · Memory census: Material/Texture2D/Mesh/AudioClip object counts + allocated MB.
    ///   · THE JANITOR: after every world load, Resources.UnloadUnusedAssets() sweeps orphans and
    ///     logs exactly what it freed — the travel-soak leak test reads these lines.
    /// Logcat contract (Terry's diagnosis surface):
    ///   ZIPTIDE: HEALTH fps=71.8 low1=63.2 dropped=4/720 mats=412 tex=189 mesh=143 clips=9 memMB=612
    ///   ZIPTIDE: HEALTH_SWEEP scene=W005_OxidizedCanopy freed mats=37 tex=12 mesh=8 clips=3
    ///   ZIPTIDE: HEALTH_SLOW low1=54.1 — sustained budget misses (investigate the current world)
    /// </summary>
    public class RuntimeHealthMonitor : MonoBehaviour
    {
        private const float ReportEverySeconds = 30f;
        private const float BudgetMs = 1000f / 72f;   // Quest refresh
        private const float SlowOnePercentLowFps = 60f;

        private static RuntimeHealthMonitor _instance;

        private FrameStats _stats;
        private float _nextReportAt;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureExists()
        {
            if (_instance != null) return;
            var go = new GameObject("__RuntimeHealth");
            DontDestroyOnLoad(go);
            go.AddComponent<RuntimeHealthMonitor>();
        }

        private void Awake()
        {
            if (_instance != null && _instance != this) { Destroy(gameObject); return; }
            _instance = this;
            _stats = new FrameStats(720);
            _nextReportAt = Time.unscaledTime + ReportEverySeconds;
            SceneManager.sceneLoaded += OnSceneLoaded;
            Debug.Log("ZIPTIDE: HEALTH_READY budgetMs=" + BudgetMs.ToString("F1"));
        }

        private void OnDestroy()
        {
            if (_instance == this) { SceneManager.sceneLoaded -= OnSceneLoaded; _instance = null; }
        }

        private void Update()
        {
            _stats.Push(Time.unscaledDeltaTime * 1000f);
            if (Time.unscaledTime < _nextReportAt) return;
            _nextReportAt = Time.unscaledTime + ReportEverySeconds;
            Report();
        }

        private void Report()
        {
            var census = Census.Take();
            float low1 = _stats.OnePercentLowFps;
            Debug.Log("ZIPTIDE: HEALTH fps=" + _stats.AverageFps.ToString("F1") +
                      " low1=" + low1.ToString("F1") +
                      " dropped=" + _stats.DroppedFrames(BudgetMs) + "/" + _stats.Count +
                      " mats=" + census.Materials + " tex=" + census.Textures +
                      " mesh=" + census.Meshes + " clips=" + census.Clips +
                      " memMB=" + census.AllocatedMB);
            if (low1 > 0f && low1 < SlowOnePercentLowFps)
                Debug.Log("ZIPTIDE: HEALTH_SLOW low1=" + low1.ToString("F1") +
                          " — sustained budget misses (investigate the current world)");
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == ZiptideConstants.SceneBoot) return;
            _stats.Reset();   // a new world gets a clean vitals window
            StartCoroutine(SweepAfterLoad(scene.name));
        }

        /// <summary>The janitor: give the world a moment to finish building, snapshot, sweep the
        /// unreferenced orphans, log the delta. Runs OUTSIDE TravelCoordinator on purpose — travel
        /// stays report-only per CLAUDE.md; this is an additive observer.</summary>
        private IEnumerator SweepAfterLoad(string sceneName)
        {
            yield return new WaitForSecondsRealtime(1.5f);
            var before = Census.Take();
            var op = Resources.UnloadUnusedAssets();
            yield return op;
            var after = Census.Take();
            Debug.Log("ZIPTIDE: HEALTH_SWEEP scene=" + sceneName +
                      " freed mats=" + (before.Materials - after.Materials) +
                      " tex=" + (before.Textures - after.Textures) +
                      " mesh=" + (before.Meshes - after.Meshes) +
                      " clips=" + (before.Clips - after.Clips) +
                      " memMB=" + after.AllocatedMB);
        }

        /// <summary>Object counts by resource class + total allocation. FindObjectsOfTypeAll walks
        /// everything — called only at report/sweep cadence, never per frame.</summary>
        private struct Census
        {
            public int Materials, Textures, Meshes, Clips;
            public long AllocatedMB;

            public static Census Take() => new Census
            {
                Materials = Resources.FindObjectsOfTypeAll<Material>().Length,
                Textures = Resources.FindObjectsOfTypeAll<Texture2D>().Length,
                Meshes = Resources.FindObjectsOfTypeAll<Mesh>().Length,
                Clips = Resources.FindObjectsOfTypeAll<AudioClip>().Length,
                AllocatedMB = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / (1024 * 1024),
            };
        }
    }
}
