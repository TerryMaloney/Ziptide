using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Canonical belt socket. It accepts declared portable items, remains the real XRI selector while
    /// an item is holstered, and returns a per-item attach target from GetAttachTransform. It never
    /// rotates one shared socket anchor after selection.
    /// </summary>
    public class HolsterSocketInteractor : XRSocketInteractor
    {
        private static readonly HashSet<string> DefaultAllowedIds = new HashSet<string>
        {
            "pistol", "taser_dart_gun", "gravity_gun", "handheld_camera", "breaker_blade"
        };

        [Tooltip("Item IDs this socket accepts.")]
        [SerializeField] private List<string> allowedItemIds = new List<string>
        {
            "pistol", "taser_dart_gun", "gravity_gun", "handheld_camera", "breaker_blade"
        };

        private readonly Dictionary<int, Transform> _poseTargets = new Dictionary<int, Transform>();
        private bool _firstHolsterReported;
        public static event Action<string> ItemHolstered;

        public static bool AllowsItemId(string itemId)
        {
            return !string.IsNullOrEmpty(itemId) && DefaultAllowedIds.Contains(itemId);
        }

        protected override void Start()
        {
            base.Start();
            socketActive = true;
            allowHover = true;
            allowSelect = true;
            hoverEntered.AddListener(OnHoverEnteredCallback);
            selectEntered.AddListener(OnSelectEnteredCallback);
            selectExited.AddListener(OnSelectExitedCallback);
            Debug.Log("ZIPTIDE: HOLSTER_SOCKET_READY socket=" + gameObject.name
                + " active=" + socketActive + " enabled=" + enabled);
        }

        protected override void OnDestroy()
        {
            hoverEntered.RemoveListener(OnHoverEnteredCallback);
            selectEntered.RemoveListener(OnSelectEnteredCallback);
            selectExited.RemoveListener(OnSelectExitedCallback);
            _poseTargets.Clear();
            base.OnDestroy();
        }

        public override bool CanHover(IXRHoverInteractable interactable)
        {
            return base.CanHover(interactable) && ItemIdAllowed(interactable);
        }

        public override bool CanSelect(IXRSelectInteractable interactable)
        {
            return base.CanSelect(interactable) && ItemIdAllowed(interactable);
        }

        /// <summary>
        /// XRI calls this with the specific interactable as context. The returned child transform is the
        /// target pose for that item's authored hand-grip attach point. Solving root pose from the real
        /// Muzzle/tip axis makes guns hang barrel-down and blades sheath tip-down without assuming model axes.
        /// </summary>
        public override Transform GetAttachTransform(IXRInteractable interactable)
        {
            Component component = interactable as Component;
            if (component == null) return base.GetAttachTransform(interactable);

            GameObject go = component.gameObject;
            ItemRuntime item = go.GetComponent<ItemRuntime>();
            XRGrabInteractable grab = go.GetComponent<XRGrabInteractable>();
            if (item == null || item.Definition == null || grab == null)
                return base.GetAttachTransform(interactable);

            int key = go.GetInstanceID();
            if (!_poseTargets.TryGetValue(key, out Transform target) || target == null)
            {
                GameObject poseGo = new GameObject("HolsterPose_" + item.Definition.itemId + "_" + key);
                poseGo.transform.SetParent(transform, false);
                target = poseGo.transform;
                _poseTargets[key] = target;
            }

            ConfigurePerItemTarget(target, go.transform, grab, item.Definition.itemId);
            return target;
        }

        private void ConfigurePerItemTarget(
            Transform target, Transform itemRoot, XRGrabInteractable grab, string itemId)
        {
            Transform itemAttach = grab.attachTransform != null ? grab.attachTransform : itemRoot;
            Vector3 localGripPosition = itemRoot.InverseTransformPoint(itemAttach.position);
            Quaternion localGripRotation = Quaternion.Inverse(itemRoot.rotation) * itemAttach.rotation;

            Transform tip = FindTip(itemRoot);
            Vector3 localAxis = WeaponPoseCore.ResolveAxisLocal(itemRoot, tip, localGripPosition);
            Vector3 localUp = WeaponPoseCore.ResolveUpHintLocal(localAxis);
            Vector3 desiredAxis = HolsterPoseCore.ResolveDesiredAxis(transform, gameObject.name);
            Vector3 desiredUp = HolsterPoseCore.ResolveDesiredUp(transform, desiredAxis);

            Quaternion desiredRootRotation = WeaponPoseCore.MapLocalBasisToWorld(
                localAxis, localUp, desiredAxis, desiredUp);
            Transform socketAnchor = attachTransform != null ? attachTransform : transform;
            Vector3 desiredRootPosition = socketAnchor.position;

            target.position = desiredRootPosition + desiredRootRotation * localGripPosition;
            target.rotation = desiredRootRotation * localGripRotation;

            Debug.Log("ZIPTIDE: HOLSTER_TARGET item=" + itemId + " socket=" + gameObject.name
                + " desiredAxis=" + desiredAxis.ToString("F2")
                + " localAxis=" + localAxis.ToString("F2"));
        }

        private static Transform FindTip(Transform root)
        {
            if (root == null) return null;
            Transform direct = root.Find("Muzzle");
            if (direct != null) return direct;
            Transform[] all = root.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < all.Length; i++)
                if (all[i] != null && all[i].name == "Muzzle") return all[i];
            return root;
        }

        private bool ItemIdAllowed(IXRInteractable interactable)
        {
            GameObject go = (interactable as Component)?.gameObject;
            if (go == null) return false;
            ItemRuntime item = go.GetComponent<ItemRuntime>();
            if (item == null || item.Definition == null) return false;
            string id = item.Definition.itemId;
            return DefaultAllowedIds.Contains(id)
                || (allowedItemIds != null && allowedItemIds.Contains(id));
        }

        private void OnHoverEnteredCallback(HoverEnterEventArgs args)
        {
            GameObject go = (args.interactableObject as Component)?.gameObject;
            ItemRuntime item = go != null ? go.GetComponent<ItemRuntime>() : null;
            IXRSelectInteractable selectable = args.interactableObject as IXRSelectInteractable;
            bool canSelect = selectable != null && CanSelect(selectable);
            Debug.Log("ZIPTIDE: HOLSTER_CANDIDATE item="
                + (item != null && item.Definition != null ? item.Definition.itemId : "UNKNOWN")
                + " socket=" + gameObject.name + " canSelect=" + canSelect);
        }

        private void OnSelectEnteredCallback(SelectEnterEventArgs args)
        {
            GameObject go = (args.interactableObject as Component)?.gameObject;
            if (go == null) return;
            ItemRuntime item = go.GetComponent<ItemRuntime>();
            XRGrabInteractable grab = go.GetComponent<XRGrabInteractable>();
            if (item == null || item.Definition == null || grab == null) return;

            Rigidbody body = go.GetComponent<Rigidbody>();
            if (body != null)
            {
                body.velocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
            }

            StartCoroutine(ReportPoseAfterSelection(go.transform, grab, item.Definition.itemId));
            PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            string itemId = item.Definition.itemId;
            bool firstHolster = FirstHourHolsterSignal.TryReport(
                itemId, profile, ref _firstHolsterReported, PublishItemHolstered);
            if (firstHolster)
                Debug.Log("ZIPTIDE: FIRST_HOLSTER item=" + itemId + " socket=" + gameObject.name);

            Debug.Log("ZIPTIDE: HOLSTER_SELECT_ENTER item=" + itemId + " socket=" + gameObject.name
                + " selectors=" + grab.interactorsSelecting.Count);
        }

        private IEnumerator ReportPoseAfterSelection(Transform item, XRGrabInteractable grab, string itemId)
        {
            yield return null;
            if (item == null || grab == null || !grab.isSelected) yield break;

            bool owned = false;
            foreach (IXRSelectInteractor interactor in grab.interactorsSelecting)
                if (ReferenceEquals(interactor, this)) { owned = true; break; }

            Transform tip = FindTip(item);
            Vector3 axis = tip != null ? (tip.position - item.position).normalized : item.forward;
            Debug.Log((owned ? "ZIPTIDE: HOLSTER_POSE_APPLIED" : "ZIPTIDE: HOLSTER_OWNERSHIP_FAIL")
                + " item=" + itemId + " socket=" + gameObject.name
                + " worldAxis=" + axis.ToString("F2") + " root=" + item.position.ToString("F2"));
        }

        private void OnSelectExitedCallback(SelectExitEventArgs args)
        {
            GameObject go = (args.interactableObject as Component)?.gameObject;
            ItemRuntime item = go != null ? go.GetComponent<ItemRuntime>() : null;
            XRGrabInteractable grab = go != null ? go.GetComponent<XRGrabInteractable>() : null;

            Debug.Log("ZIPTIDE: HOLSTER_SELECT_EXIT item="
                + (item != null && item.Definition != null ? item.Definition.itemId : "UNKNOWN")
                + " socket=" + gameObject.name
                + " remainingSelectors=" + (grab != null ? grab.interactorsSelecting.Count : 0));
        }

        private static void PublishItemHolstered(string itemId)
        {
            ItemHolstered?.Invoke(itemId);
        }
    }
}
