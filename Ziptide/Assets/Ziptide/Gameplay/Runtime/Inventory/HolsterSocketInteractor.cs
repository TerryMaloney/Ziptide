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
    public class HolsterSocketInteractor : UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor
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
            // Was a hardcoded five-id set, which is why four authored weapons (prism_beam,
            // sonic_thumper, static_net, tide_pike) could not be belted even if the player found
            // one. WeaponCatalog is now the single source of truth, shared with the ship's
            // departure gate; DefaultAllowedIds stays as the serialized-field seed only.
            return Ziptide.Core.WeaponCatalog.IsHolsterable(itemId);
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

        public override bool CanHover(UnityEngine.XR.Interaction.Toolkit.Interactables.IXRHoverInteractable interactable)
        {
            return base.CanHover(interactable) && ItemIdAllowed(interactable);
        }

        public override bool CanSelect(UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable interactable)
        {
            return base.CanSelect(interactable) && ItemIdAllowed(interactable);
        }

        /// <summary>
        /// XRI calls this with the specific interactable as context. The returned child transform is the
        /// target pose for that item's authored hand-grip attach point. Solving root pose from the real
        /// Muzzle/tip axis makes guns hang barrel-down and blades sheath tip-down without assuming model axes.
        /// </summary>
        public override Transform GetAttachTransform(UnityEngine.XR.Interaction.Toolkit.Interactables.IXRInteractable interactable)
        {
            Component component = interactable as Component;
            if (component == null) return base.GetAttachTransform(interactable);

            GameObject go = component.gameObject;
            ItemRuntime item = go.GetComponent<ItemRuntime>();
            UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab = go.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
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
            Transform target, Transform itemRoot, UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab, string itemId)
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

            // ONCE PER ITEM, NOT ONCE PER FRAME. XRI calls GetAttachTransform every frame for as
            // long as something is holstered, so this line was writing ~72 identical logs a second
            // for the whole session. The 2026-08-01 capture came back with dropped=363/720 on the
            // logcat and this was the loudest thing in it. The values are inputs, not a convergence
            // loop, so a repeat carries no information.
            if (_loggedPoseFor.Add(itemId))
                Debug.Log("ZIPTIDE: HOLSTER_TARGET item=" + itemId + " socket=" + gameObject.name
                    + " desiredAxis=" + desiredAxis.ToString("F2")
                    + " localAxis=" + localAxis.ToString("F2"));
        }

        private readonly HashSet<string> _loggedPoseFor = new HashSet<string>();

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

        private bool ItemIdAllowed(UnityEngine.XR.Interaction.Toolkit.Interactables.IXRInteractable interactable)
        {
            GameObject go = (interactable as Component)?.gameObject;
            if (go == null) return false;
            ItemRuntime item = go.GetComponent<ItemRuntime>();
            if (item == null || item.Definition == null) return false;
            string id = item.Definition.itemId;
            // WeaponCatalog is the source of truth. The old hardcoded five-id set is why four
            // authored weapons could not be belted even if the player found one; the serialized
            // list stays as a per-socket widening, never a narrowing.
            bool holsterable = Ziptide.Core.WeaponCatalog.IsHolsterable(id)
                || DefaultAllowedIds.Contains(id)
                || (allowedItemIds != null && allowedItemIds.Contains(id));
            if (!holsterable) return false;

            // ⚖ Terry: the sword belongs on the LEFT hip so it is never in the way, and everything
            // else shifts right. Enforced at the socket, because "wherever your hand happened to be"
            // is not a placement rule (HolsterAssignmentCore owns the law and the no-trap escape).
            return Ziptide.Core.HolsterAssignmentCore.SocketAccepts(gameObject.name, id);
        }

        private void OnHoverEnteredCallback(HoverEnterEventArgs args)
        {
            GameObject go = (args.interactableObject as Component)?.gameObject;
            ItemRuntime item = go != null ? go.GetComponent<ItemRuntime>() : null;
            UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable selectable = args.interactableObject as UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable;
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
            UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab = go.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            if (item == null || item.Definition == null || grab == null) return;

            Rigidbody body = go.GetComponent<Rigidbody>();
            if (body != null)
            {
                body.linearVelocity = Vector3.zero;
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

        private IEnumerator ReportPoseAfterSelection(Transform item, UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab, string itemId)
        {
            yield return null;
            if (item == null || grab == null || !grab.isSelected) yield break;

            bool owned = false;
            foreach (UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor interactor in grab.interactorsSelecting)
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
            UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab = go != null ? go.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>() : null;

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
