using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Static save/restore registry for items across scene travel.
    /// Destroy-and-recreate avoids stale XRI selection state; restore uses a real XRSocketInteractor
    /// SelectEnter so a holstered item has one unambiguous owner instead of being manually parented.
    /// </summary>
    public static class InventoryState
    {
        public struct SavedItem
        {
            public string itemId;
            public string slotId;
        }

        private static readonly List<SavedItem> _saved = new List<SavedItem>();
        public static IReadOnlyList<SavedItem> Items => _saved;

        public static void SaveBeforeTravel()
        {
            _saved.Clear();
            ItemRuntime[] allItems = Object.FindObjectsOfType<ItemRuntime>(true);
            Debug.Log("ZIPTIDE: INVENTORY_SAVE count=" + allItems.Length);

            foreach (ItemRuntime item in allItems)
            {
                if (item == null || item.Definition == null) continue;
                string slotId = DetermineSlot(item);
                if (!slotId.StartsWith("holster")) continue;

                _saved.Add(new SavedItem { itemId = item.Definition.itemId, slotId = slotId });
                Debug.Log("ZIPTIDE: INVENTORY_SAVED item=" + item.Definition.itemId + " slot=" + slotId);
                ForceDropAndDestroy(item);
            }
        }

        public static IEnumerator RestoreAfterTravel(Transform playerRoot)
        {
            if (_saved.Count == 0) yield break;
            yield return null;
            yield return null;

            Debug.Log("ZIPTIDE: INVENTORY_RESTORE count=" + _saved.Count);
            List<SavedItem> toRestore = new List<SavedItem>(_saved);
            _saved.Clear();

            foreach (SavedItem saved in toRestore)
            {
                GameObject go = ItemFactory.Create(saved.itemId,
                    playerRoot.position + playerRoot.forward * 0.3f + Vector3.up * 0.8f);
                if (go == null)
                {
                    Debug.LogError("ZIPTIDE: INVENTORY_RESTORE_BLOCKER item=" + saved.itemId
                        + " reason=factory_returned_null");
                    continue;
                }

                if (saved.slotId.StartsWith("holster"))
                    yield return TryHolsterCoroutine(go, saved.slotId, playerRoot);

                Debug.Log("ZIPTIDE: INVENTORY_RESTORED item=" + saved.itemId + " slot=" + saved.slotId);
            }
        }

        private static string DetermineSlot(ItemRuntime item)
        {
            XRGrabInteractable grab = item.GetComponent<XRGrabInteractable>();
            if (grab == null) return "loose";

            if (grab.isSelected)
            {
                foreach (IXRSelectInteractor interactor in grab.interactorsSelecting)
                {
                    XRSocketInteractor socket = interactor as XRSocketInteractor;
                    if (socket != null)
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

            // Legacy parent fallback remains readable for one migration cycle, but proximity alone is
            // deliberately not treated as ownership. A weapon near a socket is not holstered until XRI
            // says the socket selected it.
            Transform parent = item.transform.parent;
            while (parent != null)
            {
                string pn = parent.name.ToLowerInvariant();
                if (pn.Contains("holster"))
                {
                    Debug.LogWarning("ZIPTIDE: INVENTORY_LEGACY_PARENT item=" + item.Definition.itemId
                        + " parent=" + parent.name);
                    if (pn.Contains("left")) return "holster_left";
                    if (pn.Contains("center")) return "holster_center";
                    if (pn.Contains("right")) return "holster_right";
                    return "holster_center";
                }
                parent = parent.parent;
            }
            return "loose";
        }

        private static void ForceDropAndDestroy(ItemRuntime item)
        {
            XRGrabInteractable grab = item.GetComponent<XRGrabInteractable>();
            if (grab != null && grab.isSelected)
            {
                XRInteractionManager manager = grab.interactionManager;
                if (manager != null)
                {
                    List<IXRSelectInteractor> selecting = new List<IXRSelectInteractor>(grab.interactorsSelecting);
                    foreach (IXRSelectInteractor interactor in selecting)
                    {
                        try { manager.SelectExit(interactor, grab); }
                        catch (System.Exception ex)
                        {
                            Debug.LogWarning("ZIPTIDE: SelectExit error: " + ex.Message);
                        }
                    }
                }
            }
            Object.Destroy(item.gameObject);
        }

        private static IEnumerator TryHolsterCoroutine(GameObject item, string slotId, Transform playerRoot)
        {
            yield return null;
            if (item == null) yield break;

            HolsterSocketInteractor socket = FindMatchingSocket(slotId, playerRoot);
            XRGrabInteractable grab = item.GetComponent<XRGrabInteractable>();
            if (socket == null || grab == null)
            {
                Debug.LogError("ZIPTIDE: HOLSTER_DOCK_BLOCKER item=" + item.name + " reason="
                    + (socket == null ? "no_socket" : "no_grab") + " slot=" + slotId);
                yield break;
            }

            Transform anchor = socket.attachTransform != null ? socket.attachTransform : socket.transform;
            item.transform.SetPositionAndRotation(anchor.position, anchor.rotation);
            Rigidbody body = item.GetComponent<Rigidbody>();
            if (body != null)
            {
                body.velocity = Vector3.zero;
                body.angularVelocity = Vector3.zero;
                body.useGravity = false;
                body.isKinematic = true;
            }

            XRInteractionManager manager = socket.interactionManager;
            if (manager == null) manager = Object.FindObjectOfType<XRInteractionManager>();
            if (manager == null)
            {
                FailHolsterOpen(item, body, slotId, "no_interaction_manager");
                yield break;
            }
            socket.interactionManager = manager;
            grab.interactionManager = manager;

            Physics.SyncTransforms();
            yield return null;
            if (!socket.CanSelect(grab))
            {
                FailHolsterOpen(item, body, slotId, "socket_rejected");
                yield break;
            }

            manager.SelectEnter(socket, grab);
            yield return null;

            bool owned = false;
            foreach (IXRSelectInteractor interactor in grab.interactorsSelecting)
                if (ReferenceEquals(interactor, socket)) { owned = true; break; }

            if (!owned)
            {
                FailHolsterOpen(item, body, slotId, "select_enter_not_owned");
                yield break;
            }

            Debug.Log("ZIPTIDE: HOLSTER_DOCKED item=" + item.name + " slot=" + slotId
                + " owner=" + socket.gameObject.name);
        }

        private static void FailHolsterOpen(GameObject item, Rigidbody body, string slotId, string reason)
        {
            if (body != null)
            {
                body.isKinematic = false;
                body.useGravity = true;
            }
            Debug.LogError("ZIPTIDE: HOLSTER_DOCK_BLOCKER item=" + item.name
                + " reason=" + reason + " slot=" + slotId);
        }

        private static HolsterSocketInteractor FindMatchingSocket(string slotId, Transform playerRoot)
        {
            HolsterSocketInteractor[] holsters = playerRoot.GetComponentsInChildren<HolsterSocketInteractor>(true);
            foreach (HolsterSocketInteractor holster in holsters)
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
