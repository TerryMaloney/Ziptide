using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Gameplay;

namespace Ziptide.Tests.PlayMode
{
    public sealed class RecoveryHomeHubBindingTests
    {
        private readonly List<GameObject> _created = new List<GameObject>();
        private readonly List<ManagerHostState> _existingManagers = new List<ManagerHostState>();

        private readonly struct ManagerHostState
        {
            public readonly GameObject Host;
            public readonly bool WasActive;

            public ManagerHostState(GameObject host, bool wasActive)
            {
                Host = host;
                WasActive = wasActive;
            }
        }

        [UnitySetUp]
        public IEnumerator SetUp()
        {
            foreach (XRInteractionManager manager in Resources.FindObjectsOfTypeAll<XRInteractionManager>())
            {
                if (manager == null || !manager.gameObject.scene.IsValid()) continue;
                GameObject host = manager.gameObject;
                _existingManagers.Add(new ManagerHostState(host, host.activeSelf));
                host.SetActive(false);
            }

            yield return null;
            Assert.IsNull(UnityEngine.Object.FindObjectOfType<XRInteractionManager>(),
                "The replacement test requires no active XR manager before the tile is built.");
        }

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            for (int i = 0; i < _created.Count; i++)
                if (_created[i] != null) UnityEngine.Object.DestroyImmediate(_created[i]);
            _created.Clear();

            for (int i = 0; i < _existingManagers.Count; i++)
            {
                ManagerHostState state = _existingManagers[i];
                if (state.Host != null) state.Host.SetActive(state.WasActive);
            }
            _existingManagers.Clear();
            yield return null;
        }

        [UnityTest]
        public IEnumerator TileBoundToEarlyManager_RebindsWhenManagerIsReplaced()
        {
            var hubHost = new GameObject("__RECOVERY_HOME_HUB_BIND_TEST");
            _created.Add(hubHost);
            var hub = hubHost.AddComponent<HomeHubRuntime>();
            hub.enabled = false; // suppress Start; the private tile seam owns this bounded test.

            MethodInfo addTile = typeof(HomeHubRuntime).GetMethod(
                "AddTile", BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(addTile, "HomeHubRuntime.AddTile test seam was not found.");

            addTile.Invoke(hub, new object[]
            {
                "MANAGER REPLACE",
                Vector3.zero,
                Color.white,
                new Action(() => { })
            });

            Transform tileTransform = hubHost.transform.Find("Tile_MANAGER_REPLACE");
            Assert.IsNotNull(tileTransform, "The Home Hub test tile was not created.");
            var interactable = tileTransform.GetComponent<XRSimpleInteractable>();
            Assert.IsNotNull(interactable, "The Home Hub tile has no XRSimpleInteractable.");

            // XRI may create or resolve an early manager during interactable OnEnable. If this
            // runtime does not, supply one and allow the Home Hub monitor to bind it.
            if (interactable.interactionManager == null)
            {
                var earlyHost = new GameObject("__RECOVERY_EARLY_XRI_MANAGER");
                _created.Add(earlyHost);
                earlyHost.AddComponent<XRInteractionManager>();
                for (int frame = 0; frame < 10 && interactable.interactionManager == null; frame++)
                    yield return null;
            }

            XRInteractionManager earlyManager = interactable.interactionManager;
            Assert.IsNotNull(earlyManager, "The test tile never acquired its initial XR manager.");

            GameObject earlyManagerHost = earlyManager.gameObject;
            if (!WasExistingManagerHost(earlyManagerHost) && !_created.Contains(earlyManagerHost))
                _created.Add(earlyManagerHost);
            earlyManagerHost.SetActive(false);

            LogAssert.Expect(LogType.Log,
                new Regex("ZIPTIDE: HOME_HUB_TILE_BOUND tile=Tile_MANAGER_REPLACE mode=rebound"));

            var replacementHost = new GameObject("__RECOVERY_REPLACEMENT_XRI_MANAGER");
            _created.Add(replacementHost);
            var replacement = replacementHost.AddComponent<XRInteractionManager>();

            for (int frame = 0; frame < 10 && interactable.interactionManager != replacement; frame++)
                yield return null;

            Assert.AreSame(replacement, interactable.interactionManager,
                "The startup tile stayed attached to the early XR manager after replacement.");
        }

        private bool WasExistingManagerHost(GameObject host)
        {
            for (int i = 0; i < _existingManagers.Count; i++)
                if (_existingManagers[i].Host == host) return true;
            return false;
        }
    }
}
