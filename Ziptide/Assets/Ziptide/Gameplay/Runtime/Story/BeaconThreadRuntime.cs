using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// THE BEACON THREAD (FIRST_HOUR_DIRECTORS_CUT §2.4: "the joined key beacons back to Cal's own
    /// berth"). The moment the two halves become one thing in your hands, the key starts pulling
    /// toward the socket on your hull — a thin line of tide-light that hangs in the air between the
    /// key and the ship, brightening as you carry it closer.
    ///
    /// This was rb121's deferred LOOK problem: without it, the join tells the player nothing about
    /// where to go next, and the peak of the first hour becomes a hunt for the right piece of
    /// geometry. It is a stand-in per the stand-in law — one LineRenderer, no assets — and it exists
    /// only in the window between ARTIFACT_JOINED and KEY_SEATED, which is exactly the stretch where
    /// the player needs a direction.
    /// </summary>
    [DisallowMultipleComponent]
    public class BeaconThreadRuntime : MonoBehaviour
    {
        private const int Points = 14;
        private const float ArcHeight = 1.1f;
        private const float MaxRange = 90f;

        private static readonly Color Tide = new Color(0.35f, 0.88f, 0.92f);

        [Tooltip("Item id of the joined key the thread reaches for.")]
        [SerializeField] private string keyItemId = ArtifactJoinRuntime.KeyItemId;

        private LineRenderer _line;
        private Material _material;
        private Transform _key;
        private float _nextScanAt;

        private void Start()
        {
            _line = gameObject.AddComponent<LineRenderer>();
            _line.positionCount = Points;
            _line.widthMultiplier = 0.035f;
            _line.useWorldSpace = true;
            _line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _line.receiveShadows = false;

            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null) shader = Shader.Find("Sprites/Default");
            _material = new Material(shader);
            _material.SetColor("_BaseColor", Tide);
            _line.sharedMaterial = _material;
            _line.enabled = false;
        }

        private void OnDestroy()
        {
            if (_material != null) Destroy(_material);
        }

        private void LateUpdate()
        {
            if (_line == null) return;

            PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            bool wanted = profile != null
                          && profile.HasFlag(ZiptideFlags.ARTIFACT_JOINED)
                          && !profile.HasFlag(ZiptideFlags.KEY_SEATED);
            if (!wanted)
            {
                // Before the join there is nothing to lead you to; after seating, the ship itself
                // is the landmark and a line still hanging there would be litter.
                _line.enabled = false;
                return;
            }

            if (Time.time >= _nextScanAt)
            {
                _nextScanAt = Time.time + 0.5f;
                _key = FindKey();
            }
            if (_key == null) { _line.enabled = false; return; }

            float distance = Vector3.Distance(_key.position, transform.position);
            if (distance > MaxRange) { _line.enabled = false; return; }

            _line.enabled = true;
            Draw(_key.position, transform.position, distance);
        }

        /// <summary>A sagging arc, brightening as the key closes — the thread reads as PULL.</summary>
        private void Draw(Vector3 from, Vector3 to, float distance)
        {
            float closeness = 1f - Mathf.Clamp01(distance / MaxRange);
            Color c = Color.Lerp(Tide * 0.45f, Color.Lerp(Tide, Color.white, 0.4f), closeness);
            _material.SetColor("_BaseColor", c);
            _line.widthMultiplier = 0.02f + 0.03f * closeness;

            for (int i = 0; i < Points; i++)
            {
                float t = i / (float)(Points - 1);
                Vector3 p = Vector3.Lerp(from, to, t);
                // Sag plus a slow travelling ripple, so the thread is alive rather than a laser.
                p.y += Mathf.Sin(t * Mathf.PI) * ArcHeight
                       + Mathf.Sin(t * 12f - Time.time * 3f) * 0.05f * closeness;
                _line.SetPosition(i, p);
            }
        }

        private Transform FindKey()
        {
            foreach (var item in FindObjectsOfType<ItemRuntime>())
            {
                if (item == null || item.Definition == null) continue;
                if (item.Definition.itemId == keyItemId) return item.transform;
            }
            return null;
        }
    }
}
