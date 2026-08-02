using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// GARDEN AAA 4.2c — the watering can, the design doc's signature hands-on moment: grab it,
    /// TILT it past ~55° and water actually pours (the pure <see cref="PourCore"/> law — the feel
    /// is pinned by tests, not frame luck). Pour enough onto a plot and it counts as a TEND through
    /// the one garden pipeline (GardenPlotRuntime.TryTend → GardenService). The can empties in ~2s
    /// of full tilt and refills on any plot's soil the moment it's upright — kid-simple, no fill
    /// station to hunt for. A visible water level + a falling stream read the state at a glance.
    /// One can spawns beside the FIRST garden plot in a scene (EnsureNear guard).
    /// Logs GARDEN_POUR / GARDEN_TEND (via the plot).
    /// </summary>
    public class WateringCanRuntime : MonoBehaviour
    {
        private const float TendPourNeeded = 0.25f;  // a quarter can waters one plot
        private const float PlotReach = 1.6f;        // pour must be near-ish the soil — no sprinkler sniping
        private const float RefillTiltMaxDeg = 25f;  // upright near soil = refill

        private CanState _can = CanState.Full;
        private float _pouredOnPlot;
        private GardenPlotRuntime _nearPlot;
        private float _scanTimer;

        private ToolDefinition _canTool;
        private Transform _stream;
        private Transform _waterLevel;

        /// <summary>Spawn one can beside the first plot of a scene (idempotent).</summary>
        public static void EnsureNear(Vector3 position)
        {
            if (Object.FindObjectOfType<WateringCanRuntime>() != null) return;

            var go = new GameObject("WateringCan");
            go.transform.position = position + new Vector3(0.7f, 0.6f, 0f);
            var col = go.AddComponent<BoxCollider>();
            col.size = new Vector3(0.22f, 0.26f, 0.34f);
            var rb = go.AddComponent<Rigidbody>();
            rb.useGravity = true;
            var grab = go.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            var mgr = Object.FindObjectOfType<XRInteractionManager>();
            if (mgr != null) grab.interactionManager = mgr;
            go.AddComponent<WateringCanRuntime>();
        }

        private void Start()
        {
            _canTool = Resources.Load<ToolDefinition>("Garden/watering_can");

            BuildVisual();
        }

        private void BuildVisual()
        {
            var body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "Body"; StripCollider(body);
            body.transform.SetParent(transform, false);
            body.transform.localScale = new Vector3(0.2f, 0.24f, 0.2f);
            Paint(body, new Color(0.35f, 0.55f, 0.65f));

            var spout = GameObject.CreatePrimitive(PrimitiveType.Cube);
            spout.name = "Spout"; StripCollider(spout);
            spout.transform.SetParent(transform, false);
            spout.transform.localPosition = new Vector3(0f, 0.06f, 0.17f);
            spout.transform.localRotation = Quaternion.Euler(35f, 0f, 0f);
            spout.transform.localScale = new Vector3(0.05f, 0.05f, 0.16f);
            Paint(spout, new Color(0.3f, 0.48f, 0.58f));

            // Visible water level — a blue slab inside the body that sinks as you pour.
            var level = GameObject.CreatePrimitive(PrimitiveType.Cube);
            level.name = "WaterLevel"; StripCollider(level);
            level.transform.SetParent(transform, false);
            level.transform.localScale = new Vector3(0.16f, 0.18f, 0.16f);
            Paint(level, new Color(0.25f, 0.6f, 0.95f));
            _waterLevel = level.transform;

            // The falling stream — hidden until the pour law says water is moving.
            var stream = GameObject.CreatePrimitive(PrimitiveType.Cube);
            stream.name = "Stream"; StripCollider(stream);
            stream.transform.SetParent(transform, false);
            stream.transform.localPosition = new Vector3(0f, -0.15f, 0.28f);
            stream.transform.localScale = new Vector3(0.03f, 0.5f, 0.03f);
            Paint(stream, new Color(0.35f, 0.7f, 1f, 0.9f));
            _stream = stream.transform;
            stream.SetActive(false);
        }

        private void Update()
        {
            float tilt = Vector3.Angle(transform.up, Vector3.up);
            _can = PourCore.Tick(_can, tilt, Time.deltaTime, out float poured);

            bool pouring = poured > 0f;
            if (_stream != null && _stream.gameObject.activeSelf != pouring)
                _stream.gameObject.SetActive(pouring);
            if (_stream != null && pouring)
                _stream.rotation = Quaternion.identity; // water falls DOWN however the can is held
            if (_waterLevel != null)
                _waterLevel.localScale = new Vector3(0.16f, 0.18f * Mathf.Max(0.02f, _can.fill01), 0.16f);

            // Find the plot under the pour (cheap scan on a timer — gardens are small).
            _scanTimer -= Time.deltaTime;
            if (_scanTimer <= 0f)
            {
                _scanTimer = 0.4f;
                _nearPlot = NearestPlot();
            }
            if (_nearPlot == null) return;

            if (pouring)
            {
                _pouredOnPlot += poured;
                if (_pouredOnPlot >= TendPourNeeded)
                {
                    _pouredOnPlot = 0f;
                    Debug.Log("ZIPTIDE: GARDEN_POUR plotTend fill=" + _can.fill01.ToString("F2"));
                    _nearPlot.TryTend(_canTool);
                }
            }
            else if (tilt <= RefillTiltMaxDeg && _can.fill01 < 1f)
            {
                // Upright near a plot = refill. Kid-simple; no fill station to hunt for.
                _can = PourCore.Refill(_can);
                _pouredOnPlot = 0f;
                Debug.Log("ZIPTIDE: GARDEN_POUR refill");
            }
        }

        private GardenPlotRuntime NearestPlot()
        {
            GardenPlotRuntime best = null;
            float bestSqr = PlotReach * PlotReach;
            foreach (var plot in Object.FindObjectsOfType<GardenPlotRuntime>())
            {
                float d = (plot.transform.position - transform.position).sqrMagnitude;
                if (d < bestSqr) { bestSqr = d; best = plot; }
            }
            return best;
        }

        private static void StripCollider(GameObject go)
        {
            var c = go.GetComponent<Collider>();
            if (c != null) Destroy(c);
        }

        private static void Paint(GameObject go, Color color)
        {
            var r = go.GetComponent<Renderer>();
            if (r == null) return;
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) return;
            var mat = new Material(shader);
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
            else if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);
            r.sharedMaterial = mat;
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }
}
