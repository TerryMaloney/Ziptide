using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

namespace Ziptide.Tests.PlayMode
{
    /// <summary>
    /// Tests-only, hardware-free XR composition for recovery PlayMode checks.
    /// It does not start OpenXR, read a device or touch production scenes.
    /// </summary>
    public sealed class RecoveryTestRig : IDisposable
    {
        public const string RootName = "__RECOVERY_TEST_RIG";
        public const string FloorName = "__RECOVERY_TEST_FLOOR";
        public const string SpawnName = "__RECOVERY_TEST_SPAWN";

        public GameObject Root { get; }
        public Transform CameraOffset { get; }
        public Camera HeadCamera { get; }
        public Transform LeftController { get; }
        public Transform RightController { get; }
        public XRRayInteractor LeftRay { get; }
        public XRRayInteractor RightRay { get; }
        public XRInteractionManager InteractionManager { get; }
        public InputActionManager InputManager { get; }
        public InputActionAsset ActionAsset { get; }
        public InputActionMap ActionMap { get; }
        public GameObject Floor { get; }
        public Transform Spawn { get; }

        private bool _disposed;

        public RecoveryTestRig(Vector3 trackedHeadLocalOffset)
        {
            Root = new GameObject(RootName);

            var managerHost = new GameObject("InteractionManager");
            managerHost.transform.SetParent(Root.transform, false);
            InteractionManager = managerHost.AddComponent<XRInteractionManager>();

            var inputHost = new GameObject("InputActionManager");
            inputHost.transform.SetParent(Root.transform, false);
            InputManager = inputHost.AddComponent<InputActionManager>();
            InputManager.enabled = false;

            ActionAsset = ScriptableObject.CreateInstance<InputActionAsset>();
            ActionAsset.name = "RecoveryTestActions";
            ActionMap = new InputActionMap("RecoveryTest");
            ActionMap.AddAction("LeftSelect", InputActionType.Button);
            ActionMap.AddAction("RightSelect", InputActionType.Button);
            ActionAsset.AddActionMap(ActionMap);
            InputManager.actionAssets.Add(ActionAsset);
            InputManager.enabled = true;

            var offset = new GameObject("Camera Offset");
            offset.transform.SetParent(Root.transform, false);
            CameraOffset = offset.transform;

            var head = new GameObject("Main Camera");
            head.tag = "MainCamera";
            head.transform.SetParent(CameraOffset, false);
            head.transform.localPosition = trackedHeadLocalOffset;
            HeadCamera = head.AddComponent<Camera>();
            HeadCamera.enabled = true;
            var listener = head.AddComponent<AudioListener>();
            listener.enabled = false;

            LeftController = CreateController("LeftHand Controller", new Vector3(-0.25f, 1.25f, 0.35f));
            RightController = CreateController("RightHand Controller", new Vector3(0.25f, 1.25f, 0.35f));
            LeftRay = CreateRay(LeftController, "Left Ray");
            RightRay = CreateRay(RightController, "Right Ray");

            Floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            Floor.name = FloorName;
            Floor.transform.position = new Vector3(0f, -0.05f, 0f);
            Floor.transform.localScale = new Vector3(8f, 0.1f, 8f);
            var floorRenderer = Floor.GetComponent<Renderer>();
            if (floorRenderer != null) floorRenderer.enabled = false;

            var spawn = new GameObject(SpawnName);
            spawn.transform.position = Vector3.zero;
            Spawn = spawn.transform;
        }

        public Vector3 TrackedHeadWorldPosition => HeadCamera.transform.position;

        public void TeleportTrackedHeadTo(Vector3 desiredWorldPosition)
        {
            Vector3 delta = desiredWorldPosition - HeadCamera.transform.position;
            Root.transform.position += delta;
        }

        private Transform CreateController(string name, Vector3 localPosition)
        {
            var controller = new GameObject(name);
            controller.transform.SetParent(Root.transform, false);
            controller.transform.localPosition = localPosition;
            controller.transform.localRotation = Quaternion.identity;
            return controller.transform;
        }

        private XRRayInteractor CreateRay(Transform controller, string name)
        {
            var rayHost = new GameObject(name);
            rayHost.transform.SetParent(controller, false);
            rayHost.transform.localPosition = Vector3.zero;
            rayHost.transform.localRotation = Quaternion.identity;

            var ray = rayHost.AddComponent<XRRayInteractor>();
            ray.interactionManager = InteractionManager;
            ray.lineType = XRRayInteractor.LineType.StraightLine;
            ray.maxRaycastDistance = 10f;
            ray.raycastMask = Physics.DefaultRaycastLayers;
            return ray;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            if (ActionAsset != null)
            {
                ActionAsset.Disable();
                UnityEngine.Object.Destroy(ActionAsset);
            }
            if (Floor != null) UnityEngine.Object.Destroy(Floor);
            if (Spawn != null) UnityEngine.Object.Destroy(Spawn.gameObject);
            if (Root != null) UnityEngine.Object.Destroy(Root);
        }
    }
}
