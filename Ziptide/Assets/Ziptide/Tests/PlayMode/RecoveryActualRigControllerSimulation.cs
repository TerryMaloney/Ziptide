using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using Ziptide.Gameplay;
using InputSystemXRController = UnityEngine.InputSystem.XR.XRController;

namespace Ziptide.Tests.PlayMode
{
    /// <summary>
    /// Test-only tracked-rig stand-in for actual-scene PlayMode tests. The real Quest rig is loaded
    /// unchanged, including its actual head camera and left/right direct controller rays. A headless
    /// CI runner has no tracked XR devices, so this helper temporarily installs two neutral generic XR
    /// controllers, rebuilds the real action assets against them, fixes the tracked-head pose and
    /// activates the existing direct-ray hierarchies.
    ///
    /// Input ownership is explicit and finite. The simulator snapshots exact per-action enabled state
    /// only around its own deliberate asset rebind and device-removal windows, while locomotion readers
    /// are quiescent. It never installs a global action-state restorer. Production remains free to
    /// disable Rotate/Translate Anchor after every scene load without the test harness undoing it.
    /// </summary>
    public sealed class RecoveryActualRigControllerSimulation : IDisposable
    {
        public const float DefaultTrackedHeadHeight = 1.65f;

        private readonly PlayerRigPersistence _rig;
        private readonly List<GameObjectState> _gameObjectStates = new List<GameObjectState>();
        private readonly List<BehaviourState> _behaviourStates = new List<BehaviourState>();
        private readonly List<TransformState> _transformStates = new List<TransformState>();
        private readonly List<InputDevice> _virtualDevices = new List<InputDevice>();
        private readonly List<InputActionAssetState> _inputAssetStates =
            new List<InputActionAssetState>();
        private bool _disposed;

        private readonly struct GameObjectState
        {
            public readonly GameObject Object;
            public readonly bool ActiveSelf;

            public GameObjectState(GameObject value)
            {
                Object = value;
                ActiveSelf = value != null && value.activeSelf;
            }
        }

        private readonly struct BehaviourState
        {
            public readonly Behaviour Behaviour;
            public readonly bool Enabled;

            public BehaviourState(Behaviour value)
            {
                Behaviour = value;
                Enabled = value != null && value.enabled;
            }
        }

        private readonly struct TransformState
        {
            public readonly Transform Transform;
            public readonly Vector3 LocalPosition;
            public readonly Quaternion LocalRotation;
            public readonly Vector3 LocalScale;

            public TransformState(Transform value)
            {
                Transform = value;
                LocalPosition = value != null ? value.localPosition : Vector3.zero;
                LocalRotation = value != null ? value.localRotation : Quaternion.identity;
                LocalScale = value != null ? value.localScale : Vector3.one;
            }
        }

        private readonly struct InputActionAssetState
        {
            public readonly InputActionAsset Asset;

            public InputActionAssetState(InputActionAsset asset)
            {
                Asset = asset;
            }
        }

        private readonly struct InputActionEnabledState
        {
            public readonly InputAction Action;
            public readonly bool Enabled;

            public InputActionEnabledState(InputAction action)
            {
                Action = action;
                Enabled = action != null && action.enabled;
            }
        }

        public Camera HeadCamera { get; }
        public XRRayInteractor LeftRay { get; }
        public XRRayInteractor RightRay { get; }
        public string LeftRayPath => RecoveryRuntimeCensus.HierarchyPath(LeftRay.transform);
        public string RightRayPath => RecoveryRuntimeCensus.HierarchyPath(RightRay.transform);

        private RecoveryActualRigControllerSimulation(
            PlayerRigPersistence rig,
            Camera headCamera,
            XRRayInteractor leftRay,
            XRRayInteractor rightRay)
        {
            _rig = rig;
            HeadCamera = headCamera;
            LeftRay = leftRay;
            RightRay = rightRay;
        }

        public static RecoveryActualRigControllerSimulation Activate(
            PlayerRigPersistence rig,
            XRInteractionManager canonicalManager,
            float trackedHeadHeight = DefaultTrackedHeadHeight)
        {
            if (rig == null) throw new ArgumentNullException(nameof(rig));
            if (canonicalManager == null) throw new ArgumentNullException(nameof(canonicalManager));
            if (trackedHeadHeight < 0.5f || trackedHeadHeight > 2.5f)
                throw new ArgumentOutOfRangeException(nameof(trackedHeadHeight));

            Camera headCamera = rig.GetComponentInChildren<Camera>(true);
            if (headCamera == null)
                throw new InvalidOperationException("The actual persistent rig contains no head camera.");

            XRRayInteractor[] rays = rig.GetComponentsInChildren<XRRayInteractor>(true);
            XRRayInteractor leftRay = SelectDirectRay(rays, "Left");
            XRRayInteractor rightRay = SelectDirectRay(rays, "Right");
            if (leftRay == null || rightRay == null || leftRay == rightRay)
            {
                throw new InvalidOperationException(
                    "The actual persistent rig does not contain distinct left/right direct rays. " +
                    DescribeRays(rays));
            }

            var simulation = new RecoveryActualRigControllerSimulation(
                rig,
                headCamera,
                leftRay,
                rightRay);

            try
            {
                simulation.CaptureCanonicalInputAssets(canonicalManager);
                simulation.DisableModalityManagers(rig);

                List<BehaviourState> rebindReaders =
                    simulation.QuiesceLocomotionProviders();
                try
                {
                    simulation.InstallVirtualControllerDevices();
                    simulation.RefreshCanonicalInputAssets();
                }
                finally
                {
                    RestoreBehaviourStates(rebindReaders);
                }

                simulation.SetTrackedHeadPose(rig, trackedHeadHeight);
                simulation.ActivateControllerRay(rig, leftRay, canonicalManager);
                simulation.ActivateControllerRay(rig, rightRay, canonicalManager);

                Debug.Log(
                    "ZIPTIDE: RECOVERY_TRACKED_RIG_SIM head=" +
                    RecoveryRuntimeCensus.HierarchyPath(headCamera.transform) +
                    " headHeight=" + trackedHeadHeight.ToString("F2") +
                    " leftRay=" + simulation.LeftRayPath +
                    " rightRay=" + simulation.RightRayPath +
                    " manager=" + canonicalManager.GetInstanceID() +
                    " sourceRays=" + rays.Length +
                    " virtualDevices=" + simulation._virtualDevices.Count);
                return simulation;
            }
            catch
            {
                simulation.Dispose();
                throw;
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            Debug.Log(
                "ZIPTIDE: RECOVERY_VIRTUAL_XR_DISPOSE phase=begin assets=" +
                _inputAssetStates.Count + " devices=" + _virtualDevices.Count);

            List<InputActionEnabledState> actionStates = CaptureCurrentActionStates();
            List<BehaviourState> locomotionStates = QuiesceLocomotionProviders();

            for (int i = _behaviourStates.Count - 1; i >= 0; i--)
            {
                Behaviour behaviour = _behaviourStates[i].Behaviour;
                if (behaviour != null) behaviour.enabled = false;
            }

            SetInputAssetsEnabled(false);
            InputSystem.Update();
            Debug.Log("ZIPTIDE: RECOVERY_VIRTUAL_XR_DISPOSE phase=assets_disabled");

            for (int i = _virtualDevices.Count - 1; i >= 0; i--)
            {
                InputDevice device = _virtualDevices[i];
                if (device == null) continue;
                try
                {
                    InputSystem.RemoveDevice(device);
                }
                catch (Exception ex)
                {
                    Debug.LogWarning(
                        "ZIPTIDE: RECOVERY_VIRTUAL_XR_REMOVE_FAIL device=" +
                        device.displayName + " reason=" + ex.Message);
                }
            }
            InputSystem.Update();
            Debug.Log("ZIPTIDE: RECOVERY_VIRTUAL_XR_DISPOSE phase=devices_removed");

            for (int i = _transformStates.Count - 1; i >= 0; i--)
            {
                TransformState state = _transformStates[i];
                if (state.Transform == null) continue;
                state.Transform.localPosition = state.LocalPosition;
                state.Transform.localRotation = state.LocalRotation;
                state.Transform.localScale = state.LocalScale;
            }

            for (int i = _gameObjectStates.Count - 1; i >= 0; i--)
            {
                GameObjectState state = _gameObjectStates[i];
                if (state.Object != null) state.Object.SetActive(state.ActiveSelf);
            }

            int restoredActions = RestoreActionStates(actionStates);
            InputSystem.Update();

            for (int i = _behaviourStates.Count - 1; i >= 0; i--)
            {
                BehaviourState state = _behaviourStates[i];
                if (state.Behaviour != null) state.Behaviour.enabled = state.Enabled;
            }
            RestoreBehaviourStates(locomotionStates);

            Debug.Log(
                "ZIPTIDE: RECOVERY_VIRTUAL_XR_DISPOSE phase=restored actionChanges=" +
                restoredActions);

            _transformStates.Clear();
            _behaviourStates.Clear();
            _gameObjectStates.Clear();
            _virtualDevices.Clear();
            _inputAssetStates.Clear();
        }

        private void CaptureCanonicalInputAssets(XRInteractionManager canonicalManager)
        {
            InputActionManager inputManager = canonicalManager.GetComponent<InputActionManager>();
            if (inputManager == null)
                throw new InvalidOperationException(
                    "The canonical XRInteractionManager has no InputActionManager to refresh.");

            foreach (InputActionAsset asset in inputManager.actionAssets)
                if (asset != null) _inputAssetStates.Add(new InputActionAssetState(asset));
            if (_inputAssetStates.Count == 0)
                throw new InvalidOperationException(
                    "The canonical InputActionManager owns no action assets for tracked-rig simulation.");
        }

        private void InstallVirtualControllerDevices()
        {
            InputSystemXRController left = InputSystem.AddDevice<InputSystemXRController>();
            InputSystem.SetDeviceUsage(left, CommonUsages.LeftHand);
            _virtualDevices.Add(left);

            InputSystemXRController right = InputSystem.AddDevice<InputSystemXRController>();
            InputSystem.SetDeviceUsage(right, CommonUsages.RightHand);
            _virtualDevices.Add(right);
            InputSystem.Update();
        }

        private void RefreshCanonicalInputAssets()
        {
            List<InputActionEnabledState> states = CaptureCurrentActionStates();
            SetInputAssetsEnabled(false);
            InputSystem.Update();
            int restored = RestoreActionStates(states);
            InputSystem.Update();

            int locomotionActions = 0;
            int locomotionControls = 0;
            for (int i = 0; i < _inputAssetStates.Count; i++)
            {
                InputActionAsset asset = _inputAssetStates[i].Asset;
                if (asset == null) continue;
                foreach (InputActionMap map in asset.actionMaps)
                {
                    foreach (InputAction action in map.actions)
                    {
                        if (action.name.IndexOf("Turn", StringComparison.OrdinalIgnoreCase) < 0 &&
                            action.name.IndexOf("Move", StringComparison.OrdinalIgnoreCase) < 0)
                            continue;
                        locomotionActions++;
                        locomotionControls += action.controls.Count;
                    }
                }
            }

            Debug.Log(
                "ZIPTIDE: RECOVERY_VIRTUAL_XR_BINDINGS assets=" + _inputAssetStates.Count +
                " actionsRestored=" + restored +
                " locomotionActions=" + locomotionActions +
                " locomotionControls=" + locomotionControls);
            if (locomotionActions > 0 && locomotionControls == 0)
            {
                throw new InvalidOperationException(
                    "The canonical locomotion actions did not bind to the virtual left/right XR controllers.");
            }
        }

        private List<InputActionEnabledState> CaptureCurrentActionStates()
        {
            var states = new List<InputActionEnabledState>();
            var seen = new HashSet<InputAction>();
            for (int i = 0; i < _inputAssetStates.Count; i++)
            {
                InputActionAsset asset = _inputAssetStates[i].Asset;
                if (asset == null) continue;
                foreach (InputActionMap map in asset.actionMaps)
                {
                    foreach (InputAction action in map.actions)
                    {
                        if (action == null || !seen.Add(action)) continue;
                        states.Add(new InputActionEnabledState(action));
                    }
                }
            }
            return states;
        }

        private static int RestoreActionStates(
            IList<InputActionEnabledState> states)
        {
            int changed = 0;
            for (int i = 0; i < states.Count; i++)
            {
                InputActionEnabledState state = states[i];
                if (state.Action == null || state.Action.enabled == state.Enabled) continue;
                if (state.Enabled) state.Action.Enable();
                else state.Action.Disable();
                changed++;
            }
            return changed;
        }

        private void SetInputAssetsEnabled(bool enabled)
        {
            for (int i = 0; i < _inputAssetStates.Count; i++)
            {
                InputActionAsset asset = _inputAssetStates[i].Asset;
                if (asset == null) continue;
                if (enabled) asset.Enable();
                else asset.Disable();
            }
        }

        private List<BehaviourState> QuiesceLocomotionProviders()
        {
            var states = new List<BehaviourState>();
            if (_rig == null) return states;
            LocomotionProvider[] providers =
                _rig.GetComponentsInChildren<LocomotionProvider>(true);
            for (int i = 0; i < providers.Length; i++)
            {
                LocomotionProvider provider = providers[i];
                if (provider == null) continue;
                states.Add(new BehaviourState(provider));
                provider.enabled = false;
            }
            Debug.Log(
                "ZIPTIDE: RECOVERY_VIRTUAL_XR_READERS_QUIESCED providers=" + states.Count);
            return states;
        }

        private static void RestoreBehaviourStates(
            IList<BehaviourState> states)
        {
            if (states == null) return;
            for (int i = states.Count - 1; i >= 0; i--)
            {
                BehaviourState state = states[i];
                if (state.Behaviour != null) state.Behaviour.enabled = state.Enabled;
            }
        }

        private void DisableModalityManagers(PlayerRigPersistence rig)
        {
            XRInputModalityManager[] managers =
                rig.GetComponentsInChildren<XRInputModalityManager>(true);
            for (int i = 0; i < managers.Length; i++)
            {
                XRInputModalityManager manager = managers[i];
                TrackBehaviour(manager);
                manager.enabled = false;
            }
        }

        private void SetTrackedHeadPose(
            PlayerRigPersistence rig,
            float trackedHeadHeight)
        {
            Transform head = HeadCamera.transform;
            Transform current = head;
            while (current != null)
            {
                Behaviour[] behaviours = current.GetComponents<Behaviour>();
                for (int i = 0; i < behaviours.Length; i++)
                {
                    Behaviour behaviour = behaviours[i];
                    if (behaviour == null) continue;
                    string typeName = behaviour.GetType().FullName ?? string.Empty;
                    if (typeName.IndexOf(
                            "TrackedPoseDriver",
                            StringComparison.OrdinalIgnoreCase) >= 0 ||
                        typeName.IndexOf(
                            "PoseDriver",
                            StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        TrackBehaviour(behaviour);
                        behaviour.enabled = false;
                    }
                }
                if (current == rig.transform) break;
                current = current.parent;
            }

            TrackTransform(head);
            head.position = rig.transform.position + rig.transform.up * trackedHeadHeight;
            head.rotation = rig.transform.rotation;
        }

        private void ActivateControllerRay(
            PlayerRigPersistence rig,
            XRRayInteractor ray,
            XRInteractionManager canonicalManager)
        {
            ActivateHierarchy(rig.transform, ray.transform);
            EnableControllerBehaviours(ray.transform, rig.transform);
            TrackBehaviour(ray);
            ray.enabled = true;
            ray.interactionManager = canonicalManager;
        }

        private void ActivateHierarchy(Transform rigRoot, Transform leaf)
        {
            var chain = new List<GameObject>();
            Transform current = leaf;
            while (current != null)
            {
                chain.Add(current.gameObject);
                if (current == rigRoot) break;
                current = current.parent;
            }
            if (chain.Count == 0 || chain[chain.Count - 1] != rigRoot.gameObject)
                throw new InvalidOperationException(
                    "Selected controller ray is not under the persistent rig.");

            for (int i = chain.Count - 1; i >= 0; i--)
            {
                GameObject go = chain[i];
                TrackGameObject(go);
                if (!go.activeSelf) go.SetActive(true);
            }
        }

        private void EnableControllerBehaviours(
            Transform leaf,
            Transform rigRoot)
        {
            Transform current = leaf;
            while (current != null)
            {
                Behaviour[] behaviours = current.GetComponents<Behaviour>();
                for (int i = 0; i < behaviours.Length; i++)
                {
                    Behaviour behaviour = behaviours[i];
                    if (behaviour == null) continue;
                    string typeName = behaviour.GetType().FullName ?? string.Empty;
                    if (behaviour is XRBaseController ||
                        behaviour is XRBaseControllerInteractor ||
                        typeName.IndexOf(
                            "Controller",
                            StringComparison.OrdinalIgnoreCase) >= 0 ||
                        typeName.IndexOf(
                            "InteractorLineVisual",
                            StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        TrackBehaviour(behaviour);
                        behaviour.enabled = true;
                    }
                }
                if (current == rigRoot) break;
                current = current.parent;
            }
        }

        private void TrackGameObject(GameObject go)
        {
            for (int i = 0; i < _gameObjectStates.Count; i++)
                if (_gameObjectStates[i].Object == go) return;
            _gameObjectStates.Add(new GameObjectState(go));
        }

        private void TrackBehaviour(Behaviour behaviour)
        {
            if (behaviour == null) return;
            for (int i = 0; i < _behaviourStates.Count; i++)
                if (_behaviourStates[i].Behaviour == behaviour) return;
            _behaviourStates.Add(new BehaviourState(behaviour));
        }

        private void TrackTransform(Transform value)
        {
            if (value == null) return;
            for (int i = 0; i < _transformStates.Count; i++)
                if (_transformStates[i].Transform == value) return;
            _transformStates.Add(new TransformState(value));
        }

        private static XRRayInteractor SelectDirectRay(
            XRRayInteractor[] rays,
            string handName)
        {
            XRRayInteractor fallback = null;
            for (int i = 0; i < rays.Length; i++)
            {
                XRRayInteractor ray = rays[i];
                if (ray == null) continue;
                string path = RecoveryRuntimeCensus.HierarchyPath(ray.transform);
                if (path.IndexOf(handName, StringComparison.OrdinalIgnoreCase) < 0) continue;
                if (path.IndexOf("Teleport", StringComparison.OrdinalIgnoreCase) >= 0) continue;
                if (path.IndexOf("Gaze", StringComparison.OrdinalIgnoreCase) >= 0) continue;
                if (fallback == null) fallback = ray;
                if (ray.name.IndexOf("Ray", StringComparison.OrdinalIgnoreCase) >= 0)
                    return ray;
            }
            return fallback;
        }

        private static string DescribeRays(XRRayInteractor[] rays)
        {
            if (rays == null || rays.Length == 0) return "rays=0";
            var values = new List<string>(rays.Length);
            for (int i = 0; i < rays.Length; i++)
            {
                XRRayInteractor ray = rays[i];
                if (ray == null) continue;
                values.Add(
                    RecoveryRuntimeCensus.HierarchyPath(ray.transform) +
                    " activeSelf=" + ray.gameObject.activeSelf +
                    " activeHierarchy=" + ray.gameObject.activeInHierarchy +
                    " enabled=" + ray.enabled);
            }
            return "rays=" + values.Count + " [" + string.Join("; ", values) + "]";
        }
    }
}
