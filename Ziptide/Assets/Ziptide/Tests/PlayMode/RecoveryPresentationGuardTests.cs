using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Ziptide.Gameplay;

namespace Ziptide.Tests.PlayMode
{
    public sealed class RecoveryPresentationGuardTests
    {
        private readonly List<GameObject> _created = new List<GameObject>();

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            for (int i = _created.Count - 1; i >= 0; i--)
                if (_created[i] != null) Object.DestroyImmediate(_created[i]);
            _created.Clear();
            yield return null;
        }

        [UnityTest]
        public IEnumerator ViewerSideLabel_EnablesOnlyReadableFaceOnEachApproachSide()
        {
            Camera camera = NewCamera(new Vector3(0f, 1f, -3f));
            GameObject root = NewObject("__RECOVERY_LABEL_ROOT", Vector3.zero);

            TextMesh backText = NewText(root.transform, "BACK", Vector3.zero);
            ViewerSideWorldLabel back = backText.gameObject.AddComponent<ViewerSideWorldLabel>();
            back.Configure(Vector3.zero, 0.02f, -1);

            TextMesh frontText = NewText(root.transform, "FRONT", Vector3.zero);
            ViewerSideWorldLabel front = frontText.gameObject.AddComponent<ViewerSideWorldLabel>();
            front.Configure(Vector3.zero, 0.02f, 1);

            yield return null;
            Assert.IsTrue(backText.GetComponent<Renderer>().enabled,
                "The -Z approach did not enable the back-side label.");
            Assert.IsFalse(frontText.GetComponent<Renderer>().enabled,
                "The mirrored +Z label remained enabled from the -Z approach.");
            Assert.IsTrue(WorldLabelFacing.IsReadableFrom(
                backText.transform.rotation,
                backText.transform.position,
                camera.transform.position));

            camera.transform.position = new Vector3(0f, 1f, 3f);
            yield return null;
            Assert.IsFalse(backText.GetComponent<Renderer>().enabled,
                "The old back-side label remained enabled after crossing the doorway.");
            Assert.IsTrue(frontText.GetComponent<Renderer>().enabled,
                "The +Z approach did not enable the front-side label.");
            Assert.IsTrue(WorldLabelFacing.IsReadableFrom(
                frontText.transform.rotation,
                frontText.transform.position,
                camera.transform.position));
        }

        [UnityTest]
        public IEnumerator ShipPresentationGuard_AdoptsLoosePanelsAndGatesLocalSurfaces()
        {
            Camera camera = NewCamera(new Vector3(0f, 1.65f, 0f));
            GameObject ship = NewObject("__RECOVERY_SHIP", new Vector3(0f, 0f, 10f));
            GameObject cockpit = NewChild(ship.transform, "CockpitDeck", Vector3.zero);
            GameObject helm = NewChild(ship.transform, "HelmRows", Vector3.zero);
            TextMesh helmLabel = NewText(helm.transform, "LOCKED - THE BROADCAST TOMB", Vector3.zero);
            helmLabel.characterSize = 0.028f;

            // ShipBoardingStation historically authored these panel roots at scene root. The recovery
            // guard must adopt them without changing world position before applying visibility/facing.
            GameObject disembark = NewObject(
                "DisembarkPanel",
                ship.transform.position + new Vector3(0f, 1f, -1f));
            GameObject quartersPanel = NewObject(
                "QuartersPanel",
                ship.transform.position + new Vector3(-1f, 1f, -1f));
            GameObject hangar = NewChild(ship.transform, "HangarBay", new Vector3(2f, 1f, -2f));

            GameObject quarters = NewChild(ship.transform, "Quarters", new Vector3(0f, 0f, -5f));
            GameObject bay = NewChild(quarters.transform, "Bay_ShipLivery", Vector3.zero);
            GameObject locker = NewChild(quarters.transform, "LockerBoard", Vector3.zero);
            GameObject returnPanel = NewChild(quarters.transform, "QuartersReturn", Vector3.zero);

            Vector3 disembarkWorld = disembark.transform.position;
            Vector3 quartersWorld = quartersPanel.transform.position;
            ShipBoardingPresentationGuard guard = ship.AddComponent<ShipBoardingPresentationGuard>();
            FieldInfo viewerField = typeof(ShipBoardingPresentationGuard).GetField(
                "_viewer",
                BindingFlags.Instance | BindingFlags.NonPublic);
            FieldInfo scanCountField = typeof(ShipBoardingPresentationGuard).GetField(
                "_hierarchyScanCount",
                BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(viewerField);
            Assert.IsNotNull(scanCountField);
            viewerField.SetValue(guard, camera);
            guard.RefreshNow();
            yield return null; // allow Start to run once; all later frames must use the cache.

            Assert.AreSame(ship.transform, disembark.transform.parent,
                "The loose disembark panel was not adopted by its ship.");
            Assert.AreSame(ship.transform, quartersPanel.transform.parent,
                "The loose Quarters panel was not adopted by its ship.");
            Assert.Less(Vector3.Distance(disembarkWorld, disembark.transform.position), 0.0001f,
                "Adopting the disembark panel changed its world position.");
            Assert.Less(Vector3.Distance(quartersWorld, quartersPanel.transform.position), 0.0001f,
                "Adopting the Quarters panel changed its world position.");
            Assert.IsFalse(helm.activeSelf);
            Assert.IsFalse(disembark.activeSelf);
            Assert.IsFalse(quartersPanel.activeSelf);
            Assert.IsFalse(hangar.activeSelf);
            Assert.IsFalse(bay.activeSelf);
            Assert.IsFalse(locker.activeSelf);
            Assert.IsFalse(returnPanel.activeSelf);
            int steadyStateScanCount = (int)scanCountField.GetValue(guard);
            Assert.Greater(steadyStateScanCount, 0);

            camera.transform.position = cockpit.transform.position + Vector3.up * 1.65f;
            yield return null;

            Assert.IsTrue(helm.activeSelf);
            Assert.IsTrue(disembark.activeSelf);
            Assert.IsTrue(quartersPanel.activeSelf);
            Assert.IsTrue(hangar.activeSelf);
            Assert.AreEqual(0.016f, helmLabel.characterSize, 0.0001f);
            Assert.IsTrue(WorldLabelFacing.IsReadableFrom(
                disembark.transform.rotation,
                disembark.transform.position,
                camera.transform.position));
            Assert.IsTrue(WorldLabelFacing.IsReadableFrom(
                quartersPanel.transform.rotation,
                quartersPanel.transform.position,
                camera.transform.position));
            Assert.IsFalse(bay.activeSelf,
                "Quarters browsing surfaces became visible while the player was only on the deck.");
            Assert.AreEqual(steadyStateScanCount, (int)scanCountField.GetValue(guard),
                "The guard rescanned its hierarchy during steady-state deck presentation.");

            yield return null;
            yield return null;
            Assert.AreEqual(steadyStateScanCount, (int)scanCountField.GetValue(guard),
                "The guard allocated repeated hierarchy scans across unchanged frames.");

            camera.transform.position = quarters.transform.position + Vector3.up * 1.65f;
            yield return null;

            Assert.IsFalse(helm.activeSelf);
            Assert.IsFalse(hangar.activeSelf);
            Assert.IsTrue(bay.activeSelf);
            Assert.IsTrue(locker.activeSelf);
            Assert.IsTrue(returnPanel.activeSelf);
            Assert.AreEqual(steadyStateScanCount, (int)scanCountField.GetValue(guard),
                "Changing presentation zones required an avoidable hierarchy rescan.");
        }

        private Camera NewCamera(Vector3 position)
        {
            GameObject go = NewObject("__RECOVERY_PRESENTATION_CAMERA", position);
            go.tag = "MainCamera";
            Camera camera = go.AddComponent<Camera>();
            camera.enabled = true;
            return camera;
        }

        private GameObject NewObject(string name, Vector3 position)
        {
            var go = new GameObject(name);
            go.transform.position = position;
            _created.Add(go);
            return go;
        }

        private static GameObject NewChild(Transform parent, string name, Vector3 localPosition)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
            return go;
        }

        private static TextMesh NewText(Transform parent, string value, Vector3 localPosition)
        {
            GameObject go = NewChild(parent, "Label_" + value, localPosition);
            TextMesh text = go.AddComponent<TextMesh>();
            text.text = value;
            text.fontSize = 48;
            text.characterSize = 0.04f;
            text.anchor = TextAnchor.MiddleCenter;
            text.alignment = TextAlignment.Center;
            return text;
        }
    }
}
