using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace Ziptide.Tests.PlayMode
{
    public sealed class RecoveryTestRigTests
    {
        private RecoveryTestRig _fixture;
        private readonly List<GameObject> _created = new List<GameObject>();

        [UnityTearDown]
        public IEnumerator TearDown()
        {
            foreach (var go in _created)
                if (go != null) Object.Destroy(go);
            _created.Clear();

            _fixture?.Dispose();
            _fixture = null;
            yield return null;

            Assert.IsNull(GameObject.Find(RecoveryTestRig.RootName), "Tests-only rig leaked after teardown.");
            Assert.IsNull(GameObject.Find(RecoveryTestRig.FloorName), "Tests-only floor leaked after teardown.");
            Assert.IsNull(GameObject.Find(RecoveryTestRig.SpawnName), "Tests-only spawn leaked after teardown.");
        }

        [UnityTest]
        public IEnumerator Composition_HasOneCameraManagerAndTestOwnedActions()
        {
            var headOffset = new Vector3(0.28f, 1.68f, 0.17f);
            _fixture = new RecoveryTestRig(headOffset);
            yield return null;

            Assert.AreEqual(1, _fixture.Root.GetComponentsInChildren<Camera>(true).Length);
            Assert.AreEqual(1, _fixture.Root.GetComponentsInChildren<XRInteractionManager>(true).Length);
            Assert.AreEqual(1, _fixture.Root.GetComponentsInChildren<InputActionManager>(true).Length);
            Assert.AreEqual(_fixture.HeadCamera, Camera.main);
            Assert.IsTrue(_fixture.ActionMap.enabled, "InputActionManager did not enable the tests-only action asset.");

            Assert.That(Vector3.Distance(_fixture.Root.transform.position, _fixture.TrackedHeadWorldPosition),
                Is.GreaterThan(0.2f), "Tracked head must not collapse onto the rig root.");
            Assert.That(_fixture.HeadCamera.transform.localPosition, Is.EqualTo(headOffset));

            var desiredHeadWorld = new Vector3(5.4f, 2.1f, -3.2f);
            _fixture.TeleportTrackedHeadTo(desiredHeadWorld);

            Assert.That(Vector3.Distance(_fixture.TrackedHeadWorldPosition, desiredHeadWorld), Is.LessThan(0.0001f));
            Assert.That(_fixture.HeadCamera.transform.localPosition, Is.EqualTo(headOffset),
                "Teleporting the rig must preserve room-scale tracked-head offset.");
            Assert.IsNotNull(_fixture.Floor.GetComponent<Collider>());
            Assert.That(_fixture.Spawn.position, Is.EqualTo(Vector3.zero));
        }

        [UnityTest]
        public IEnumerator RightRay_CanHitHoverAndSelectPrimitiveInteractable()
        {
            _fixture = new RecoveryTestRig(new Vector3(0.2f, 1.65f, 0.1f));
            yield return null;

            var target = GameObject.CreatePrimitive(PrimitiveType.Cube);
            target.name = "__RECOVERY_RAY_TARGET";
            target.transform.position = _fixture.RightRay.transform.position +
                                        _fixture.RightRay.transform.forward * 2f;
            target.transform.localScale = Vector3.one * 0.35f;
            _created.Add(target);

            var interactable = target.AddComponent<XRSimpleInteractable>();
            interactable.interactionManager = _fixture.InteractionManager;
            Physics.SyncTransforms();

            Assert.IsTrue(Physics.Raycast(
                _fixture.RightRay.transform.position,
                _fixture.RightRay.transform.forward,
                out RaycastHit hit,
                10f,
                Physics.DefaultRaycastLayers));
            Assert.AreEqual(target, hit.collider.gameObject, "The tests-only right ray is not aligned to the target.");

            int selected = 0;
            interactable.selectEntered.AddListener(_ => selected++);

            var hoverInteractor = (IXRHoverInteractor)_fixture.RightRay;
            var selectInteractor = (IXRSelectInteractor)_fixture.RightRay;
            var hoverInteractable = (IXRHoverInteractable)interactable;
            var selectInteractable = (IXRSelectInteractable)interactable;

            _fixture.InteractionManager.HoverEnter(hoverInteractor, hoverInteractable);
            Assert.IsTrue(_fixture.RightRay.hasHover);
            Assert.IsTrue(interactable.isHovered);

            _fixture.InteractionManager.SelectEnter(selectInteractor, selectInteractable);
            Assert.IsTrue(_fixture.RightRay.hasSelection);
            Assert.IsTrue(interactable.isSelected);
            Assert.AreEqual(1, selected, "Selection event did not traverse the XRI interaction manager.");

            _fixture.InteractionManager.SelectExit(selectInteractor, selectInteractable);
            _fixture.InteractionManager.HoverExit(hoverInteractor, hoverInteractable);
            Assert.IsFalse(interactable.isSelected);
            Assert.IsFalse(interactable.isHovered);
        }
    }
}
