using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Inputs;
using Ziptide.Gameplay;

namespace Ziptide.Tests.PlayMode
{
    /// <summary>
    /// Test-only tracked-controller stand-in for actual-scene PlayMode tests. The real Quest rig is
    /// loaded unchanged, including its actual controller ray. A headless CI runner has no tracked
    /// controller devices, so XRInputModalityManager correctly deactivates every controller/hand
    /// group. This helper temporarily disables modality switching and activates only the existing
    /// right direct ray hierarchy so the real XRI components can be exercised deterministically.
    /// It does not create an alternate ray, action map, manager, camera, or production bootstrap.
    /// </summary>
    public sealed class RecoveryActualRigControllerSimulation : IDisposable
    {
        private readonly List<GameObjectState> _gameObjectStates = new List<GameObjectState>();
        private readonly List<BehaviourState> _behaviourStates = new List<BehaviourState>();
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

        public XRRayInteractor RightRay { get; }
        public string RightRayPath => RecoveryRuntimeCensus.HierarchyPath(RightRay.transform);

        private RecoveryActualRigControllerSimulation(XRRayInteractor rightRay)
        {
            RightRay = rightRay;
        }

        public static RecoveryActualRigControllerSimulation Activate(
            PlayerRigPersistence rig,
            XRInteractionManager canonicalManager)
        {
            if (rig == null) throw new ArgumentNullException(nameof(rig));
            if (canonicalManager == null) throw new ArgumentNullException(nameof(canonicalManager));

            XRRayInteractor[] rays = rig.GetComponentsInChildren<XRRayInteractor>(true);
            XRRayInteractor rightRay = SelectRightDirectRay(rays);
            if (rightRay == null)
            {
                throw new InvalidOperationException(
                    "The actual persistent rig contains no right non-teleport XRRayInteractor. " +
                    DescribeRays(rays));
            }

            var simulation = new RecoveryActualRigControllerSimulation(rightRay);
            simulation.DisableModalityManagers(rig);
            simulation.ActivateHierarchy(rig.transform, rightRay.transform);
            simulation.EnableControllerBehaviours(rightRay.transform, rig.transform);
            simulation.TrackBehaviour(rightRay);
            rightRay.enabled = true;
            rightRay.interactionManager = canonicalManager;

            Debug.Log("ZIPTIDE: RECOVERY_TRACKED_CONTROLLER_SIM ray=" + simulation.RightRayPath
                + " manager=" + canonicalManager.GetInstanceID()
                + " sourceRays=" + rays.Length);
            return simulation;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;

            for (int i = _behaviourStates.Count - 1; i >= 0; i--)
            {
                BehaviourState state = _behaviourStates[i];
                if (state.Behaviour != null) state.Behaviour.enabled = state.Enabled;
            }
            for (int i = _gameObjectStates.Count - 1; i >= 0; i--)
            {
                GameObjectState state = _gameObjectStates[i];
                if (state.Object != null) state.Object.SetActive(state.ActiveSelf);
            }
            _behaviourStates.Clear();
            _gameObjectStates.Clear();
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
                throw new InvalidOperationException("Selected right ray is not under the persistent rig.");

            for (int i = chain.Count - 1; i >= 0; i--)
            {
                GameObject go = chain[i];
                TrackGameObject(go);
                if (!go.activeSelf) go.SetActive(true);
            }
        }

        private void EnableControllerBehaviours(Transform leaf, Transform rigRoot)
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
                        typeName.Contains("Controller") ||
                        typeName.Contains("InteractorLineVisual"))
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

        private static XRRayInteractor SelectRightDirectRay(XRRayInteractor[] rays)
        {
            XRRayInteractor rightFallback = null;
            for (int i = 0; i < rays.Length; i++)
            {
                XRRayInteractor ray = rays[i];
                if (ray == null) continue;
                string path = RecoveryRuntimeCensus.HierarchyPath(ray.transform);
                if (path.IndexOf("Right", StringComparison.OrdinalIgnoreCase) < 0) continue;
                if (path.IndexOf("Teleport", StringComparison.OrdinalIgnoreCase) >= 0) continue;
                if (path.IndexOf("Gaze", StringComparison.OrdinalIgnoreCase) >= 0) continue;
                if (rightFallback == null) rightFallback = ray;
                if (ray.name.IndexOf("Ray", StringComparison.OrdinalIgnoreCase) >= 0)
                    return ray;
            }
            return rightFallback;
        }

        private static string DescribeRays(XRRayInteractor[] rays)
        {
            if (rays == null || rays.Length == 0) return "rays=0";
            var values = new List<string>(rays.Length);
            for (int i = 0; i < rays.Length; i++)
            {
                XRRayInteractor ray = rays[i];
                if (ray == null) continue;
                values.Add(RecoveryRuntimeCensus.HierarchyPath(ray.transform)
                    + " activeSelf=" + ray.gameObject.activeSelf
                    + " activeHierarchy=" + ray.gameObject.activeInHierarchy
                    + " enabled=" + ray.enabled);
            }
            return "rays=" + values.Count + " [" + string.Join("; ", values) + "]";
        }
    }
}
