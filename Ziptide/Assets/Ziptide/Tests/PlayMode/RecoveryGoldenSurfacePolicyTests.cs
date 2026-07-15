using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Ziptide.Core;
using Ziptide.Gameplay;

namespace Ziptide.Tests.PlayMode
{
    /// <summary>
    /// Proves that a canonical persistent owner cannot bypass the Golden player-view policy by
    /// adding the legacy CreditsHud component directly.
    /// </summary>
    public sealed class RecoveryGoldenSurfacePolicyTests
    {
        private readonly List<GameObject> _created = new List<GameObject>();

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            for (int i = 0; i < _created.Count; i++)
                if (_created[i] != null) Object.DestroyImmediate(_created[i]);
            _created.Clear();
            RecoveryRuntimeGate.SetActiveProfile(RecoveryExposureProfiles.FullDevelopment);
            yield return null;
        }

        [UnityTest]
        public IEnumerator GoldenSlice_DirectCreditsHudInjection_RemainsInvisibleAndInert()
        {
            RecoveryRuntimeGate.SetActiveProfile(RecoveryExposureProfiles.GoldenSlice);

            var host = new GameObject("__RECOVERY_GOLDEN_CREDITS_HUD");
            _created.Add(host);
            var hud = host.AddComponent<CreditsHud>();
            yield return null;

            Assert.IsFalse(hud.enabled,
                "CreditsHud remained active after direct injection into GoldenSlice.");
            Assert.IsNull(host.transform.Find("CreditsHudText"),
                "The forbidden yellow CR text was created in the Golden player view.");
        }

        [UnityTest]
        public IEnumerator FullDevelopment_CreditsHudCompatibilitySurface_StillBuilds()
        {
            RecoveryRuntimeGate.SetActiveProfile(RecoveryExposureProfiles.FullDevelopment);

            var host = new GameObject("__RECOVERY_FULL_CREDITS_HUD");
            _created.Add(host);
            var hud = host.AddComponent<CreditsHud>();
            yield return null;

            Assert.IsTrue(hud.enabled,
                "FullDevelopment unexpectedly lost the compatibility credits surface.");
            Assert.IsNotNull(host.transform.Find("CreditsHudText"),
                "FullDevelopment CreditsHud did not build its text surface.");
        }
    }
}
