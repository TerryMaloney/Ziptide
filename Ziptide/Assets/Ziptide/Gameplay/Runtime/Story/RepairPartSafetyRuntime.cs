using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Installs a bounded safety component on RepairableMachine replacement parts. The machine remains
    /// the repair-stage owner; this layer only makes selection explicit, prevents holstered weapons from
    /// physically fighting the cell, logs hand ownership, and returns an unheld part that escaped the
    /// playable repair radius. A dropped coupler cell can no longer permanently strand the first level.
    /// </summary>
    public sealed class RepairPartSafetyRuntime : MonoBehaviour
    {
        public const float MaximumPartDistance = 3.0f;
        public const float MinimumRelativeY = -1.5f;

        private RepairableMachine _machine;
        private XRGrabInteractable _grab;
        private Rigidbody _body;
        private Vector3 _homePosition;
        private Quaternion _homeRotation;
        private float _nextCollisionRefresh;

        public void Configure(RepairableMachine machine)
        {
            if (_machine != null) return;
            _machine = machine;
            _grab = GetComponent<XRGrabInteractable>();
            _body = GetComponent<Rigidbody>();
            _homePosition = transform.position;
            _homeRotation = transform.rotation;

            if (_grab != null)
            {
                _grab.selectMode = InteractableSelectMode.Single;
                _grab.useDynamicAttach = false;
                _grab.attachEaseInTime = 0f;
                _grab.throwOnDetach = false;
                if (_grab.attachTransform == null)
                {
                    GameObject attach = new GameObject("RepairPartGrip");
                    attach.transform.SetParent(transform, false);
                    attach.transform.localPosition = Vector3.zero;
                    attach.transform.localRotation = Quaternion.identity;
                    _grab.attachTransform = attach.transform;
                }
                _grab.selectEntered.AddListener(OnSelected);
                _grab.selectExited.AddListener(OnReleased);
            }
            RefreshWeaponCollisionIgnores();
            Debug.Log("ZIPTIDE: REPAIR_PART_SAFETY_READY part=" + name
                + " machine=" + (_machine != null ? _machine.MachineId : "NONE"));
        }

        private void OnDestroy()
        {
            if (_grab == null) return;
            _grab.selectEntered.RemoveListener(OnSelected);
            _grab.selectExited.RemoveListener(OnReleased);
        }

        private void Update()
        {
            if (_machine == null || _machine.CurrentStage != RepairStage.Part) return;
            if (Time.unscaledTime >= _nextCollisionRefresh)
            {
                _nextCollisionRefresh = Time.unscaledTime + 1f;
                RefreshWeaponCollisionIgnores();
            }
            if (_grab != null && _grab.isSelected) return;

            float distance = Vector3.Distance(transform.position, _machine.transform.position);
            float relativeY = transform.position.y - _machine.transform.position.y;
            if (distance <= MaximumPartDistance && relativeY >= MinimumRelativeY) return;
            ResetPart("escaped distance=" + distance.ToString("F2") + " relativeY=" + relativeY.ToString("F2"));
        }

        private void OnSelected(SelectEnterEventArgs args)
        {
            string hand = args.interactorObject is Component c ? c.gameObject.name : "UNKNOWN";
            Debug.Log("ZIPTIDE: REPAIR_PART_SELECT part=" + name + " hand=" + hand
                + " selectors=" + (_grab != null ? _grab.interactorsSelecting.Count : 0));
        }

        private void OnReleased(SelectExitEventArgs args)
        {
            string hand = args.interactorObject is Component c ? c.gameObject.name : "UNKNOWN";
            Debug.Log("ZIPTIDE: REPAIR_PART_RELEASE part=" + name + " hand=" + hand);
        }

        private void ResetPart(string reason)
        {
            if (_body != null)
            {
                _body.velocity = Vector3.zero;
                _body.angularVelocity = Vector3.zero;
                _body.useGravity = false;
                _body.constraints = RigidbodyConstraints.FreezeAll;
            }
            transform.SetPositionAndRotation(_homePosition, _homeRotation);
            Debug.LogError("ZIPTIDE: REPAIR_PART_RECOVERED part=" + name + " reason=" + reason
                + " home=" + _homePosition.ToString("F2"));
        }

        private void RefreshWeaponCollisionIgnores()
        {
            Collider partCollider = GetComponent<Collider>();
            if (partCollider == null) return;
            ItemRuntime[] items = Object.FindObjectsOfType<ItemRuntime>(true);
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == null) continue;
                Collider[] colliders = items[i].GetComponentsInChildren<Collider>(true);
                for (int j = 0; j < colliders.Length; j++)
                    if (colliders[j] != null) Physics.IgnoreCollision(partCollider, colliders[j], true);
            }
        }

        private sealed class Installer : MonoBehaviour
        {
            private float _nextScan;

            private void Update()
            {
                if (Time.unscaledTime < _nextScan) return;
                _nextScan = Time.unscaledTime + 0.5f;
                RepairableMachine[] machines = Object.FindObjectsOfType<RepairableMachine>(true);
                XRGrabInteractable[] grabs = Object.FindObjectsOfType<XRGrabInteractable>(true);
                for (int i = 0; i < grabs.Length; i++)
                {
                    XRGrabInteractable grab = grabs[i];
                    if (grab == null || !grab.gameObject.name.StartsWith("Part_")) continue;
                    if (grab.GetComponent<RepairPartSafetyRuntime>() != null) continue;
                    RepairableMachine nearest = null;
                    float best = float.MaxValue;
                    for (int j = 0; j < machines.Length; j++)
                    {
                        if (machines[j] == null) continue;
                        float d = Vector3.SqrMagnitude(grab.transform.position - machines[j].transform.position);
                        if (d < best) { best = d; nearest = machines[j]; }
                    }
                    if (nearest == null || best > 36f) continue;
                    RepairPartSafetyRuntime safety = grab.gameObject.AddComponent<RepairPartSafetyRuntime>();
                    safety.Configure(nearest);
                }
            }
        }

        private static Installer _installer;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            _installer = null;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureInstaller()
        {
            if (_installer != null) return;
            GameObject go = new GameObject("__RepairPartSafetyInstaller");
            Object.DontDestroyOnLoad(go);
            _installer = go.AddComponent<Installer>();
        }
    }
}
