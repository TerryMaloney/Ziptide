using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Static save/restore registry for items across scene travel.
    /// Destroy-and-recreate pattern avoids XRI selection state corruption.
    /// RestoreAfterTravel is an IEnumerator so it can be started as a coroutine,
    /// deferring restore until XRI is frame-stable and using real socket selection.
    /// </summary>
    public static class InventoryState
    {
        public struct SavedItem
        {
            public string itemId;
            public string slotId; // "holster_left", "holster_center", "holster_right", "hand", "loose"
        }

        private static readonly List<SavedItem> _saved = new List<SavedItem>();

        public static IReadOnlyList<SavedItem> Items => _saved;

        public static void SaveBeforeTravel()
        {
            _saved.Clear();

            var allItems = Object.FindObjectsOfType<ItemRuntime>(true);
            Debug.Log("ZIPTIDE: INVENTORY_SAVE count=" + allItems.Length);

            foreach (var item in allItems)
            {
                if (item == null || item.Definition == null) continue;

                string slotId = DetermineSlot(item);
                _saved.Add(new SavedItem { itemId = item.Definition.itemId, slotId = slotId });

                Debug.Log("ZIPTIDE: INVENTORY_SAVED item=" + item.Definition.itemId + " slot=" + slotId);

                ForceDropAndDestroy(item);
            }
        }

        /// <summary>
        /// Must be started as a coroutine (e.g. StartCoroutine(InventoryState.RestoreAfterTravel(transform))).
        /// Waits two frames for XRI interactors to register before attempting socket selection.
        /// </summary>
        public static IEnumerator RestoreAfterTravel(Transform playerRoot)
        {
            if (_saved.Count == 0) yield break;

            // Wait two frames so XRI interactors/sockets are fully enabled and registered.
            yield return null;
            yield return null;

            Debug.Log("ZIPTIDE: INVENTORY_RESTORE count=" + _saved.Count);

            var toRestore = new List<SavedItem>(_saved);
            _saved.Clear();

            foreach (var saved in toRestore)
            {
                var go = ItemFactory.Create(
                    saved.itemId,
                    playerRoot.position + playerRoot.forward * 0.3f + Vector3.up * 0.8f);

                if (go == null)
                {
                    Debug.LogWarning("ZIPTIDE: INVENTORY_RESTORE_FAIL item=" + saved.itemId + " reason=factory_returned_null");
                    continue;
                }

                if (saved.slotId.StartsWith("holster"))
                {
                    // Each TryHolsterCoroutine yields internally; run inline.
                    yield return TryHolsterCoroutine(go, saved.slotId, playerRoot);
                }

                Debug.Log("ZIPTIDE: INVENTORY_RESTORED item=" + saved.itemId + " slot=" + saved.slotId);
            }
        }

        // ── Slot detection ──────────────────────────────────────────────────

        private static string DetermineSlot(ItemRuntime item)
        {
            var grab = item.GetComponent<XRGrabInteractable>();
            if (grab == null) return "loose";

            if (grab.isSelected)
            {
                foreach (var interactor in grab.interactorsSelecting)
                {
                    if (interactor is XRSocketInteractor socket)
                    {
                        string socketName = socket.gameObject.name.ToLowerInvariant();
                        if (socketName.Contains("left")) return "holster_left";
                        if (socketName.Contains("center")) return "holster_center";
                        if (socketName.Contains("right")) return "holster_right";
                        return "holster_center";
                    }
                    return "hand";
                }
            }

            var parent = item.transform.parent;
            while (parent != null)
            {
                string pn = parent.name.ToLowerInvariant();
                if (pn.Contains("holster"))
                {
                    if (pn.Contains("left")) return "holster_left";
                    if (pn.Contains("center")) return "holster_center";
                    if (pn.Contains("right")) return "holster_right";
                    return "holster_center";
                }
                parent = parent.parent;
            }

            return "loose";
        }

        // ── Force-drop and destroy before travel ────────────────────────────

        private static void ForceDropAndDestroy(ItemRuntime item)
        {
            var grab = item.GetComponent<XRGrabInteractable>();
            if (grab != null && grab.isSelected)
            {
                var mgr = grab.interactionManager;
                if (mgr != null)
                {
                    var selectingList = new List<IXRSelectInteractor>(grab.interactorsSelecting);
                    foreach (var interactor in selectingList)
                    {
                        try { mgr.SelectExit(interactor, grab); }
                        catch (System.Exception ex) { Debug.LogWarning("ZIPTIDE: SelectExit error: " + ex.Message); }
                    }
                }
            }

            Object.Destroy(item.gameObject);
        }

        // ── Holster restore (coroutine, uses real socket selection) ─────────

        private static IEnumerator TryHolsterCoroutine(GameObject item, string slotId, Transform playerRoot)
        {
            // Give the freshly-created item one frame to complete Awake/OnEnable.
            yield return null;

            var grab = item != null ? item.GetComponent<XRGrabInteractable>() : null;
            var mgr = Object.FindObjectOfType<XRInteractionManager>();

            if (grab == null || mgr == null)
            {
                Debug.LogWarning("ZIPTIDE: INVENTORY_RESTORE_FAIL item=" + (item != null ? item.name : "null")
                    + " reason=no_grab_or_manager");
                yield break;
            }

            var socket = FindMatchingSocket(slotId, playerRoot);
            if (socket == null)
            {
                Debug.LogWarning("ZIPTIDE: INVENTORY_RESTORE_FAIL item=" + item.name
                    + " reason=no_matching_socket slot=" + slotId);
                yield break;
            }

            if (socket.hasSelection)
            {
                Debug.LogWarning("ZIPTIDE: INVENTORY_RESTORE_FAIL item=" + item.name
                    + " reason=socket_already_occupied slot=" + slotId);
                yield break;
            }

            // Position the item at the socket attach point, then perform a real socket-select.
            item.transform.position = socket.attachTransform != null
                ? socket.attachTransform.position
                : socket.transform.position;
            item.transform.rotation = socket.attachTransform != null
                ? socket.attachTransform.rotation
                : socket.transform.rotation;

            try
            {
                mgr.SelectEnter(socket as IXRSelectInteractor, grab as IXRSelectInteractable);
                Debug.Log("ZIPTIDE: HOLSTER_RESTORED item=" + item.name + " slot=" + slotId);
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning("ZIPTIDE: INVENTORY_RESTORE_FAIL item=" + item.name
                    + " reason=SelectEnter_exception: " + ex.Message);
            }
        }

        private static HolsterSocketInteractor FindMatchingSocket(string slotId, Transform playerRoot)
        {
            var holsters = playerRoot.GetComponentsInChildren<HolsterSocketInteractor>(true);
            foreach (var holster in holsters)
            {
                string hn = holster.gameObject.name.ToLowerInvariant();
                if (slotId == "holster_left" && hn.Contains("left")) return holster;
                if (slotId == "holster_center" && hn.Contains("center")) return holster;
                if (slotId == "holster_right" && hn.Contains("right")) return holster;
            }
            return null;
        }
    }
}
