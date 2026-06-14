using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Creates four invisible boundary walls around the play area. Uses Ignore Raycast layer so XR rays don't hit.
    /// </summary>
    public class PlayAreaBounds : MonoBehaviour
    {
        private const float WallHeight = 3f;
        private const float WallThickness = 0.2f;
        private const string IgnoreRaycastLayerName = "Ignore Raycast";
        // Unity reserves layer index 2 as the built-in "Ignore Raycast" layer; it always exists.
        private const int BuiltinIgnoreRaycastLayer = 2;

        private int _ignoreRaycastLayer;

        private void Awake()
        {
            _ignoreRaycastLayer = LayerMask.NameToLayer(IgnoreRaycastLayerName);
            if (_ignoreRaycastLayer < 0)
            {
                Debug.LogWarning($"[Ziptide] PlayAreaBounds: layer '{IgnoreRaycastLayerName}' not found; falling back to built-in Ignore Raycast layer {BuiltinIgnoreRaycastLayer} so walls don't block XR rays.");
                _ignoreRaycastLayer = BuiltinIgnoreRaycastLayer;
            }
        }

        /// <summary>
        /// Build or rebuild the four walls. Call from WorldRuntime on Start.
        /// </summary>
        public void Build(Vector2 playAreaSize, float groundY)
        {
            ClearChildren();

            float halfW = Mathf.Max(0.01f, playAreaSize.x) * 0.5f;
            float halfL = Mathf.Max(0.01f, playAreaSize.y) * 0.5f;
            float y = groundY + WallHeight * 0.5f;

            CreateWall("WallNorth", new Vector3(0f, y, halfL), new Vector3(playAreaSize.x + WallThickness * 2f, WallHeight, WallThickness));
            CreateWall("WallSouth", new Vector3(0f, y, -halfL), new Vector3(playAreaSize.x + WallThickness * 2f, WallHeight, WallThickness));
            CreateWall("WallEast", new Vector3(halfW, y, 0f), new Vector3(WallThickness, WallHeight, playAreaSize.y + WallThickness * 2f));
            CreateWall("WallWest", new Vector3(-halfW, y, 0f), new Vector3(WallThickness, WallHeight, playAreaSize.y + WallThickness * 2f));
        }

        private void CreateWall(string name, Vector3 position, Vector3 size)
        {
            var go = new GameObject(name);
            go.transform.SetParent(transform);
            go.transform.localPosition = position;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;
            go.layer = _ignoreRaycastLayer;

            var col = go.AddComponent<BoxCollider>();
            col.size = size;
            col.isTrigger = false;
        }

        private void ClearChildren()
        {
            while (transform.childCount > 0)
            {
                var child = transform.GetChild(0);
                if (Application.isPlaying)
                    Destroy(child.gameObject);
                else
                    DestroyImmediate(child.gameObject);
            }
        }
    }
}
