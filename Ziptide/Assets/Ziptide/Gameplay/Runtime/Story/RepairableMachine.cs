using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// The hands-on repair fantasy (GAME_PLAN M2): a broken machine fixed in three PHYSICAL stages —
    /// 1) grab the access PANEL off, 2) fetch the replacement PART (spawned wherever the pack says —
    /// the fetch is part of the job) and seat it in the exposed socket, 3) flip the power SWITCH.
    /// Then the machine hums back to life and credits RepairMachineCount job steps via
    /// <see cref="JobDirector.ReportRepair"/>. Spawned by JobDirector from
    /// <see cref="MachineSpawnDefinition"/> pack data — never in scene YAML.
    /// Logs: ZIPTIDE: MACHINE_STAGE id=… stage=… · ZIPTIDE: MACHINE_REPAIRED id=…
    /// </summary>
    public class RepairableMachine : MonoBehaviour
    {
        private enum Stage { Panel, Part, Power, Running }

        private static readonly Color BodyColor = new Color(0.16f, 0.17f, 0.20f);
        private static readonly Color PanelColor = new Color(0.32f, 0.20f, 0.14f); // rusted plate
        private static readonly Color SocketEmpty = new Color(0.55f, 0.25f, 0.20f); // exposed fault
        private static readonly Color PartColor = new Color(0.85f, 0.65f, 0.25f);
        private static readonly Color SwitchOff = new Color(0.45f, 0.15f, 0.12f);
        private static readonly Color RunningColor = new Color(0.25f, 0.75f, 0.55f);
        private const float SeatDistance = 0.3f;

        private MachineSpawnDefinition _def;
        private JobDirector _director;
        private Stage _stage = Stage.Panel;

        private Transform _part;
        private Transform _socket;
        private Renderer _socketRenderer;
        private Renderer _switchRenderer;
        private Renderer _statusLamp;
        private TextMesh _label;

        /// <summary>Build + arm the machine. Call immediately after AddComponent (spawner does).</summary>
        public void Init(MachineSpawnDefinition def, JobDirector director)
        {
            _def = def ?? new MachineSpawnDefinition();
            _director = director;
            Build();
        }

        private void Build()
        {
            // Body.
            var body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "Body";
            body.transform.SetParent(transform, false);
            body.transform.localPosition = new Vector3(0f, 0.75f, 0f);
            body.transform.localScale = new Vector3(1.0f, 1.5f, 0.7f);
            Paint(body, BodyColor);

            // Status lamp on top — red while broken, green when running.
            var lamp = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            lamp.name = "StatusLamp"; StripCollider(lamp);
            lamp.transform.SetParent(transform, false);
            lamp.transform.localPosition = new Vector3(0f, 1.62f, 0f);
            lamp.transform.localScale = Vector3.one * 0.12f;
            Paint(lamp, SwitchOff);
            _statusLamp = lamp.GetComponent<Renderer>();

            // Floating label + stage hint.
            var labelGo = new GameObject("Label");
            _label = labelGo.AddComponent<TextMesh>();
            _label.characterSize = 0.035f;
            _label.fontSize = 48;
            _label.anchor = TextAnchor.MiddleCenter;
            _label.alignment = TextAlignment.Center;
            _label.color = new Color(1f, 0.85f, 0.6f);
            labelGo.transform.SetParent(transform, false);
            labelGo.transform.localPosition = new Vector3(0f, 1.95f, 0f);

            // The exposed socket behind the panel (visible once the panel is off).
            var socket = GameObject.CreatePrimitive(PrimitiveType.Cube);
            socket.name = "Socket"; StripCollider(socket);
            socket.transform.SetParent(transform, false);
            socket.transform.localPosition = new Vector3(0f, 0.85f, -0.30f);
            socket.transform.localScale = new Vector3(0.24f, 0.24f, 0.12f);
            Paint(socket, SocketEmpty);
            _socket = socket.transform;
            _socketRenderer = socket.GetComponent<Renderer>();
            socket.SetActive(false);

            // Stage 1: the access panel — a grabbable plate covering the socket.
            var panel = new GameObject("Panel");
            panel.transform.SetParent(transform, false);
            panel.transform.localPosition = new Vector3(0f, 0.85f, -0.42f);
            var panelCol = panel.AddComponent<BoxCollider>();
            panelCol.size = new Vector3(0.5f, 0.5f, 0.06f);
            var panelRb = panel.AddComponent<Rigidbody>();
            panelRb.isKinematic = true; // bolted on until grabbed
            var panelGrab = panel.AddComponent<XRGrabInteractable>();
            WireManager(panelGrab);
            panelGrab.selectEntered.AddListener(_ => OnPanelPulled(panel, panelRb));
            var panelVisual = GameObject.CreatePrimitive(PrimitiveType.Cube);
            panelVisual.name = "Plate"; StripCollider(panelVisual);
            panelVisual.transform.SetParent(panel.transform, false);
            panelVisual.transform.localScale = new Vector3(0.5f, 0.5f, 0.05f);
            Paint(panelVisual, PanelColor);

            // Stage 3: the power switch (armed after the part seats).
            var sw = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sw.name = "PowerSwitch";
            sw.transform.SetParent(transform, false);
            sw.transform.localPosition = new Vector3(0.42f, 1.1f, -0.30f);
            sw.transform.localScale = new Vector3(0.12f, 0.2f, 0.08f);
            Paint(sw, SwitchOff);
            _switchRenderer = sw.GetComponent<Renderer>();
            var swInteractable = sw.AddComponent<XRSimpleInteractable>();
            WireManager(swInteractable);
            swInteractable.selectEntered.AddListener(_ => OnSwitchFlipped());

            // Stage 2: the replacement part, spawned where the pack says (fetch = gameplay).
            Vector3 partPos = _def.partLocalPosition == Vector3.zero
                ? transform.position + new Vector3(0.8f, 0.9f, 0f)
                : transform.parent != null ? transform.parent.TransformPoint(_def.partLocalPosition + Vector3.up * 0.9f)
                                           : _def.partLocalPosition + Vector3.up * 0.9f;
            var part = new GameObject("Part_" + _def.partItemId);
            part.transform.position = partPos;
            var partCol = part.AddComponent<SphereCollider>();
            partCol.radius = 0.12f;
            var partRb = part.AddComponent<Rigidbody>();
            partRb.isKinematic = true; // floats until grabbed; VelocityTracking after
            var partGrab = part.AddComponent<XRGrabInteractable>();
            WireManager(partGrab);
            var partVisual = GameObject.CreatePrimitive(PrimitiveType.Cube);
            partVisual.name = "PartVisual"; StripCollider(partVisual);
            partVisual.transform.SetParent(part.transform, false);
            partVisual.transform.localScale = new Vector3(0.18f, 0.18f, 0.18f);
            Paint(partVisual, PartColor);
            var partLabel = new GameObject("PartLabel");
            var ptm = partLabel.AddComponent<TextMesh>();
            ptm.text = (_def.partItemId ?? "part").Replace('_', ' ');
            ptm.characterSize = 0.025f; ptm.fontSize = 48;
            ptm.anchor = TextAnchor.MiddleCenter; ptm.alignment = TextAlignment.Center;
            ptm.color = PartColor;
            partLabel.transform.SetParent(part.transform, false);
            partLabel.transform.localPosition = Vector3.up * 0.25f;
            _part = part.transform;

            UpdateLabel();
        }

        private void Update()
        {
            // Stage 2: seat the part — snaps when it comes near the exposed socket (held or tossed).
            if (_stage == Stage.Part && _part != null && _socket != null &&
                Vector3.Distance(_part.position, _socket.position) <= SeatDistance)
            {
                SeatPart();
            }

            // Billboard the label.
            var cam = Camera.main;
            if (cam != null && _label != null)
                _label.transform.rotation = Quaternion.LookRotation(_label.transform.position - cam.transform.position);
        }

        private void OnPanelPulled(GameObject panel, Rigidbody rb)
        {
            if (_stage != Stage.Panel) return;
            _stage = Stage.Part;
            // The plate comes free in the hand; once dropped it's junk with physics.
            rb.isKinematic = false;
            rb.useGravity = true;
            panel.transform.SetParent(null, true);
            if (_socket != null) _socket.gameObject.SetActive(true);
            Debug.Log("ZIPTIDE: MACHINE_STAGE id=" + _def.machineId + " stage=panel_off");
            UpdateLabel();
        }

        private void SeatPart()
        {
            _stage = Stage.Power;
            // Consume the part into the socket. Destroying a selected interactable is the established
            // pattern here (CollectibleRuntime does the same) — XRI unregisters it on destroy.
            Destroy(_part.gameObject);
            _part = null;
            if (_socketRenderer != null) Tint(_socketRenderer, PartColor);
            Debug.Log("ZIPTIDE: MACHINE_STAGE id=" + _def.machineId + " stage=part_seated");
            UpdateLabel();
        }

        private void OnSwitchFlipped()
        {
            if (_stage != Stage.Power) return;
            _stage = Stage.Running;
            if (_switchRenderer != null) Tint(_switchRenderer, RunningColor);
            if (_statusLamp != null) Tint(_statusLamp, RunningColor);
            if (_director == null) _director = FindObjectOfType<JobDirector>();
            if (_director != null) _director.ReportRepair(_def.machineId);
            Debug.Log("ZIPTIDE: MACHINE_REPAIRED id=" + _def.machineId);
            UpdateLabel();
        }

        private void UpdateLabel()
        {
            if (_label == null) return;
            string name = string.IsNullOrEmpty(_def.displayName)
                ? (_def.machineId ?? "machine").Replace('_', ' ')
                : _def.displayName;
            switch (_stage)
            {
                case Stage.Panel: _label.text = name + "\n< pull the access panel >"; break;
                case Stage.Part: _label.text = name + "\n< seat the " + (_def.partItemId ?? "part").Replace('_', ' ') + " >"; break;
                case Stage.Power: _label.text = name + "\n< flip the power switch >"; break;
                default: _label.text = name + "\nRUNNING"; _label.color = RunningColor; break;
            }
        }

        private static void WireManager(XRBaseInteractable interactable)
        {
            var mgr = Object.FindObjectOfType<XRInteractionManager>();
            if (mgr != null) interactable.interactionManager = mgr;
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

        private static void Tint(Renderer r, Color color)
        {
            if (r == null || r.material == null) return;
            if (r.material.HasProperty("_BaseColor")) r.material.SetColor("_BaseColor", color);
            else if (r.material.HasProperty("_Color")) r.material.color = color;
        }
    }
}
