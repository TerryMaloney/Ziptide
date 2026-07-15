using System;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using Ziptide.Core;
using Ziptide.Gameplay;

namespace Ziptide.Tests.PlayMode
{
    internal static class RecoveryGoldenTravelVisualCapture
    {
        private static readonly HashSet<string> CapturedDestinations =
            new HashSet<string>(StringComparer.Ordinal);
        private static bool _registered;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Register()
        {
            if (_registered) return;
            _registered = true;
            TravelCoordinator.TravelCompleted -= CaptureCompletedDestination;
            TravelCoordinator.TravelCompleted += CaptureCompletedDestination;
            Debug.Log("ZIPTIDE: RECOVERY_GOLDEN_VISUAL_CAPTURE_READY");
        }

        private static void CaptureCompletedDestination(string destination)
        {
            if (!IsGoldenDestination(destination) || CapturedDestinations.Contains(destination)) return;

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

            CapturedDestinations.Add(destination);
            Debug.Log("ZIPTIDE: RECOVERY_GOLDEN_VISUAL_OK dest=" + destination
                + " captured=" + CapturedDestinations.Count + "/2");
        }

        private static bool IsGoldenDestination(string destination)
        {
            return string.Equals(destination, ZiptideConstants.SceneW000, StringComparison.Ordinal) ||
                   string.Equals(destination, ZiptideConstants.SceneToxicCity, StringComparison.Ordinal);
        }
    }
}
