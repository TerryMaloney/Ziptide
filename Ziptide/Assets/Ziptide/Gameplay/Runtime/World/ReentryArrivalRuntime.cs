using UnityEngine;
using UnityEngine.SceneManagement;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// THE CANONICAL OWNER of the reentry/landing handoff beat (product contract §4 row
    /// "Reentry/landing handoff" — was red: "no canonical owner/beat"). Authored into a world scene
    /// by its patcher; on scene start it asks <see cref="ReentryArrivalCore"/> whether the player
    /// just came DOWN from the space leg, and if so plays the arrival act. Canon guard: this is
    /// atmospheric reentry, NOT a Ziptide — the gate tide stays suppressed on the routine legs
    /// (rb121 story pass), so this deliberately does not touch ZiptideGateEffect.
    ///
    /// v1 is the SEAM + diagnostics + an optional RILL callout: the plasma-veil visual is an art
    /// stand-in tracked in LEVEL1_MASTER_TRACKER (a LOOK problem needing reference plates, per the
    /// same rule that deferred the hangar walk). Everything here is fail-safe: nothing can strand
    /// travel, move the camera, or run on any route except space → world.
    /// Logs: ZIPTIDE: REENTRY_ARRIVAL from=… world=…
    /// </summary>
    public class ReentryArrivalRuntime : MonoBehaviour
    {
        [Tooltip("The space-leg scene this world reenters from (ZiptideConstants.SceneSpaceLane).")]
        [SerializeField] private string expectedOriginScene = ZiptideConstants.SceneSpaceLane;

        [Tooltip("Optional RILL line id spoken on reentry. Empty = no line (default, avoids double-firing the arrival react).")]
        [SerializeField] private string rillLineId = "";

        private void Start()
        {
            string previous = SceneArrivalLog.LastUnloadedScene;
            string current = SceneManager.GetActiveScene().name;
            if (!ReentryArrivalCore.ShouldPlay(previous, current, expectedOriginScene)) return;

            Debug.Log("ZIPTIDE: REENTRY_ARRIVAL from=" + previous + " world=" + current);

            // The plasma veil is no longer a stand-in seam: the same burn that closed around the
            // hull on the way up opens here and CLEARS, so arriving from space reads as punching
            // down through atmosphere. It cannot strand anything — the effect owns its own hard
            // cap and the arrival has already happened by the time it plays.
            AtmosphereVeilEffect.Play(VeilLeg.Reentry);

            if (!string.IsNullOrEmpty(rillLineId))
            {
                var rill = FindObjectOfType<RillCompanion>();
                if (rill != null) rill.SayById(rillLineId);
            }
        }
    }

    /// <summary>
    /// Session-scoped record of the most recently unloaded scene, so a destination scene can know
    /// where the player came FROM without touching the travel owner. sceneUnloaded is used (not
    /// activeSceneChanged) because the unloaded Scene keeps a valid name during the callback.
    /// </summary>
    public static class SceneArrivalLog
    {
        public static string LastUnloadedScene { get; private set; } = "";

        private static bool _hooked;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Hook()
        {
            if (_hooked) return;
            _hooked = true;
            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private static void OnSceneUnloaded(Scene scene)
        {
            if (!string.IsNullOrEmpty(scene.name)) LastUnloadedScene = scene.name;
        }
    }
}
