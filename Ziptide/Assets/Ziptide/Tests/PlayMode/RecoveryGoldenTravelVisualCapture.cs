using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Core;
using Ziptide.Gameplay;

namespace Ziptide.Tests.PlayMode
{
    /// <summary>
    /// Golden-destination visual proof. TravelCompleted only MARKS a destination pending — it must
    /// not capture there: normal travel plays the ZiptideGateEffect arrival, whose crest flash is a
    /// deliberately OPAQUE shell parented around the head camera (torn down ~12% into the arrival
    /// animation). A synchronous capture at TravelCompleted photographs the inside of that shell —
    /// the exact uniform pale-cyan (Crest ≈ luminance 0.954), three-color, near-zero-dynamic-range
    /// frame run 29494329424 recorded for ToxicCity, while W000 passed only because boot travel is
    /// skipGate. The test drives <see cref="CaptureSettledPending"/> after each golden arrival: it
    /// waits (bounded REAL time) for the transition to actually leave the player's view, then runs
    /// the UNCHANGED frame + UI assertions against the arrived world. The assertion surface GROWS:
    /// a stranded flash/arrival effect is now itself a failure, and the round-trip test asserts the
    /// full capture count so deferral can never silently skip a destination.
    /// </summary>
    internal static class RecoveryGoldenTravelVisualCapture
    {
        private const float TransitionClearRealtimeLimit = 10f; // arrival effect designs at ~2 s

        private static readonly HashSet<string> CapturedDestinations =
            new HashSet<string>(StringComparer.Ordinal);
        private static readonly HashSet<string> PendingDestinations =
            new HashSet<string>(StringComparer.Ordinal);
        private static bool _registered;

        public static int CapturedCount => CapturedDestinations.Count;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Register()
        {
            if (_registered) return;
            _registered = true;
            TravelCoordinator.TravelCompleted -= MarkCompletedDestinationPending;
            TravelCoordinator.TravelCompleted += MarkCompletedDestinationPending;
            Debug.Log("ZIPTIDE: RECOVERY_GOLDEN_VISUAL_CAPTURE_READY");
        }

        // Publication-path handler: bookkeeping ONLY. No assertions and no rendering may run inside
        // TravelCoordinator's completion publication (the prior synchronous assert interrupted the
        // travel-completion route and cascaded into the suite timeout).
        private static void MarkCompletedDestinationPending(string destination)
        {
            if (!IsGoldenDestination(destination) || CapturedDestinations.Contains(destination)) return;
            PendingDestinations.Add(destination);
            Debug.Log("ZIPTIDE: RECOVERY_GOLDEN_VISUAL_PENDING dest=" + destination);
        }

        /// <summary>
        /// Drive the deferred capture for one golden destination: wait for the arrival transition
        /// (gate effect + opaque crest flash) to leave the view under a hard real-time limit, let
        /// the frame settle, then run the exact same frame + UI spatial assertions as before.
        /// </summary>
        public static IEnumerator CaptureSettledPending(string destination)
        {
            if (CapturedDestinations.Contains(destination)) yield break;
            Assert.IsTrue(PendingDestinations.Contains(destination),
                "TravelCompleted never marked " + destination + " for golden visual capture.");

            float deadline = Time.realtimeSinceStartup + TransitionClearRealtimeLimit;
            while (Time.realtimeSinceStartup < deadline && ArrivalTransitionInView())
                yield return null;
            Assert.IsFalse(ArrivalTransitionInView(),
                "The arrival transition (ZiptideGateEffect / __ZiptideFlash) never cleared within "
                + TransitionClearRealtimeLimit + "s after " + destination
                + " — a stranded flash shell is a real defect, not a capture-timing issue.");

            yield return null;
            yield return null; // two settled frames after the transition teardown

            PlayerRigPersistence rig = UnityEngine.Object.FindObjectOfType<PlayerRigPersistence>();
            Assert.IsNotNull(rig,
                "Golden visual capture could not find PlayerRigPersistence after " + destination + ".");
            Camera camera = rig.GetComponentInChildren<Camera>(true);
            Assert.IsNotNull(camera,
                "Golden visual capture could not find the persistent head camera after " + destination + ".");
            Assert.IsTrue(camera.isActiveAndEnabled,
                "Golden visual capture found an inactive head camera after " + destination + ".");

            string labelToken = destination == ZiptideConstants.SceneW000
                ? "W000_SPAWN"
                : "TOXIC_CITY_SPAWN";
            string stemToken = destination == ZiptideConstants.SceneW000
                ? "w000_spawn"
                : "toxic_city_spawn";

            RecoveryActualSceneSnapshotTests.AssertFrameRendered(
                RecoveryRenderSnapshot.Capture(
                    camera,
                    "R1_7_ACTUAL_" + labelToken,
                    "r1_7_actual_" + stemToken));
            RecoveryActualSceneSnapshotTests.AssertUiSpatial(
                camera,
                "R1_8_ACTUAL_" + labelToken,
                "r1_8_actual_" + stemToken);
            RecoveryFallbackSurfaceAudit.AssertAndWrite(
                "R1_9_ACTUAL_" + labelToken,
                "r1_9_actual_" + stemToken);

            PendingDestinations.Remove(destination);
            CapturedDestinations.Add(destination);
            Debug.Log("ZIPTIDE: RECOVERY_GOLDEN_VISUAL_OK dest=" + destination
                + " captured=" + CapturedDestinations.Count + "/2");
        }

        private static bool ArrivalTransitionInView()
        {
            if (UnityEngine.Object.FindObjectOfType<ZiptideGateEffect>() != null) return true;
            return GameObject.Find("__ZiptideFlash") != null;
        }

        private static bool IsGoldenDestination(string destination)
        {
            return string.Equals(destination, ZiptideConstants.SceneW000, StringComparison.Ordinal) ||
                   string.Equals(destination, ZiptideConstants.SceneToxicCity, StringComparison.Ordinal);
        }
    }
}
