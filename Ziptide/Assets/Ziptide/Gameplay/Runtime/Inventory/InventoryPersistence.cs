using UnityEngine;

namespace Ziptide.Gameplay
{
    public class InventoryPersistence : MonoBehaviour
    {
        private Transform _root;

        public Transform Root => _root != null ? _root : (_root = EnsureRoot());

        public Transform EnsureRoot()
        {
            var t = transform.Find("__InventoryRoot");
            if (t != null) return t;
            var go = new GameObject("__InventoryRoot");
            go.transform.SetParent(transform, false);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            return go.transform;
        }

        public void Adopt(Transform item)
        {
            if (item == null) return;
            item.SetParent(Root, true);
        }
    }
}
