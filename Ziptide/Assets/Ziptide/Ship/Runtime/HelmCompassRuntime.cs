using UnityEngine;

namespace Ziptide.Ship
{
    /// <summary>
    /// The physical compass ribbon on the cockpit frame: a dim strip with a marker that slides to
    /// where the next ring is (<see cref="CompassRibbonCore"/>). An INSTRUMENT, not a HUD — it is
    /// scene geometry bolted to the helm, so it obeys the same law as every other readout in this
    /// project: nothing is welded to the player's face.
    ///
    /// Silent when there is nothing to steer toward (not flying, course complete) — the strip
    /// simply goes dark rather than pointing at a ring that no longer matters.
    /// Serialized by ScenePatcherSpaceLane.
    /// </summary>
    public class HelmCompassRuntime : MonoBehaviour
    {
        [Tooltip("Flight runtime whose course bearing drives the marker.")]
        [SerializeField] private ShipFlightRuntime flight;

        [Tooltip("Ribbon half-width in metres — the marker slides between ±this.")]
        [SerializeField] private float halfWidth = 0.42f;

        private Transform _marker;
        private Renderer _markerRenderer;
        private Renderer _stripRenderer;
        private MaterialPropertyBlock _block;

        private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
        private static readonly int ColorId = Shader.PropertyToID("_Color");

        private static readonly Color Idle = new Color(0.18f, 0.20f, 0.24f);

        private void Start()
        {
            _block = new MaterialPropertyBlock();
            Build();
        }

        private void Build()
        {
            if (transform.Find("Strip") != null)
            {
                _stripRenderer = transform.Find("Strip").GetComponent<Renderer>();
                _marker = transform.Find("Marker");
                if (_marker != null) _markerRenderer = _marker.GetComponent<Renderer>();
                return;
            }

            var strip = GameObject.CreatePrimitive(PrimitiveType.Cube);
            strip.name = "Strip";
            strip.transform.SetParent(transform, false);
            strip.transform.localScale = new Vector3(halfWidth * 2f, 0.035f, 0.01f);
            DestroyImmediateSafe(strip.GetComponent<Collider>());
            _stripRenderer = strip.GetComponent<Renderer>();

            var marker = GameObject.CreatePrimitive(PrimitiveType.Cube);
            marker.name = "Marker";
            marker.transform.SetParent(transform, false);
            marker.transform.localScale = new Vector3(0.045f, 0.075f, 0.014f);
            DestroyImmediateSafe(marker.GetComponent<Collider>());
            _marker = marker.transform;
            _markerRenderer = marker.GetComponent<Renderer>();
        }

        private static void DestroyImmediateSafe(Object o)
        {
            if (o == null) return;
            if (Application.isPlaying) Destroy(o); else DestroyImmediate(o);
        }

        private void LateUpdate()
        {
            if (_marker == null) return;

            if (flight == null || !flight.TryGetCourseBearing(out float bearing))
            {
                Paint(_markerRenderer, Idle);
                Paint(_stripRenderer, Idle);
                return;
            }

            float offset = CompassRibbonCore.RibbonOffset(bearing);
            _marker.localPosition = new Vector3(offset * halfWidth, 0f, -0.004f);
            Paint(_markerRenderer, CompassRibbonCore.MarkerColor(bearing));
            Paint(_stripRenderer, CompassRibbonCore.IsBehind(bearing)
                ? new Color(0.30f, 0.12f, 0.10f)   // the turn-around state reads on the whole strip
                : Idle);
        }

        private void Paint(Renderer r, Color color)
        {
            if (r == null || _block == null) return;
            r.GetPropertyBlock(_block);
            _block.SetColor(BaseColorId, color);
            _block.SetColor(ColorId, color);
            r.SetPropertyBlock(_block);
        }
    }
}
