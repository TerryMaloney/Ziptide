using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// The hands-on repair fantasy (GAME_PLAN M2): a broken machine fixed in three PHYSICAL stages —
    /// 1) grab the access PANEL off, 2) fetch the replacement PART (spawned wherever the pack says —
    /// the fetch is part of the job) and seat it in the exposed socket, 3) press the illuminated power
    /// SWITCH. Then the machine hums back to life and credits RepairMachineCount job steps via
    /// <see cref="JobDirector.ReportRepair"/>. Spawned by JobDirector from
    /// <see cref="MachineSpawnDefinition"/> pack data — never in scene YAML.
    /// Logs: ZIPTIDE: MACHINE_STAGE id=… stage=… · ZIPTIDE: MACHINE_REPAIRED id=…
    /// </summary>
    public class RepairableMachine : MonoBehaviour, IScannable
    {
        private static readonly Color BodyColor = new Color(0.16f, 0.17f, 0.20f);
        private static readonly Color PanelColor = new Color(0.32f, 0.20f, 0.14f); // rusted plate
        private static readonly Color SocketEmpty = new Color(0.55f, 0.25f, 0.20f); // exposed fault
        private static readonly Color PartColor = new Color(0.85f, 0.65f, 0.25f);
        private static readonly Color SwitchOff = new Color(0.45f, 0.15f, 0.12f);
        private static readonly Color SwitchReady = new Color(1.0f, 0.48f, 0.08f);
        private static readonly Color RunningColor = new Color(0.25f, 0.75f, 0.55f);
        private const float SeatDistance = 0.3f;

        private MachineSpawnDefinition _def;
        private JobDirector _director;
        private RepairStage _stage = RepairStage.Panel;

        /// <summary>Stable machine id from pack data (e.g. "gate_coupler"). Null before Init.</summary>
        public string MachineId => _def != null ? _def.machineId : null;

        /// <summary>The single public view of the existing physical repair state.</summary>
        public RepairStage CurrentStage => _stage;

        /// <summary>Neutral notification emitted after an established physical stage transition.</summary>
        public event System.Action<RepairStage> StageChanged;

        /// <summary>True once the machine hums (panel off → part seated → switch pressed).
        /// Queried by ShipCastOffRuntime's arming gate.</summary>
        public bool IsRepaired => _stage == RepairStage.Running;

        // IScannable — the same physical machine is the objective; no proxy or second scanner state.
        public Transform ScanTransform => transform;
        public ScanKind ScanKind => Ziptide.Gameplay.ScanKind.Objective;
        public bool ScanActive => _stage != RepairStage.Running;

        private Transform _part;
        private Transform _socket;
        private Renderer _socketRenderer;
        private Renderer _switchRenderer;
        private Renderer _statusLamp;
        private GameObject _powerSwitch;
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

            // Status INDICATOR — deliberately a flat light, not a round button. It is red while broken
            // and green when running, but never receives interaction. The actual power control appears
            // on the child-reachable front face only after the replacement part seats.
            var lamp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            lamp.name = "StatusIndicator"; StripCollider(lamp);
            lamp.transform.SetParent(transform, false);
            lamp.transform.localPosition = new Vector3(0f, 1.34f, -0.37f);
            lamp.transform.localScale = new Vector3(0.18f, 0.07f, 0.035f);
            Paint(lamp, SwitchOff);
            _statusLamp = lamp.GetComponent<Renderer>();

            // Floating label + stage hint. This is guidance only; every actual control is below 0.9 m.
            var labelGo = new GameObject("Label");
            _label = labelGo.AddComponent<TextMesh>();
            _label.characterSize = 0.035f;
            _label.fontSize = 48;
            _label.anchor = TextAnchor.MiddleCenter;
            _label.alignment = TextAlignment.Center;
            _label.color = new Color(1f, 0.85f, 0.6f);
            labelGo.transform.SetParent(transform, false);
            labelGo.transform.localPosition = new Vector3(0f, 1.58f, -0.38f);

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

            // Stage 1: the access panel — a grabbable plate covering the socket. A constrained dynamic
            // body avoids XRI's "throwing a kinematic Rigidbody" warning while remaining bolted in place.
            var panel = new GameObject("Panel");
            panel.transform.SetParent(transform, false);
            panel.transform.localPosition = new Vector3(0f, 0.85f, -0.42f);
            var panelCol = panel.AddComponent<BoxCollider>();
            panelCol.size = new Vector3(0.5f, 0.5f, 0.06f);
            var panelRb = panel.AddComponent<Rigidbody>();
            panelRb.isKinematic = false;
            panelRb.useGravity = false;
            panelRb.constraints = RigidbodyConstraints.FreezeAll;
            var panelGrab = panel.AddComponent<XRGrabInteractable>();
            WireManager(panelGrab);
            panelGrab.selectEntered.AddListener(_ => OnPanelPulled(panel, panelRb));
            var panelVisual = GameObject.CreatePrimitive(PrimitiveType.Cube);
            panelVisual.name = "Plate"; StripCollider(panelVisual);
            panelVisual.transform.SetParent(panel.transform, false);
            panelVisual.transform.localScale = new Vector3(0.5f, 0.5f, 0.05f);
            Paint(panelVisual, PanelColor);

            // Stage 3: a large, front-centre, child-reachable power switch. It remains hidden until the
            // part seats, so the only newly illuminated control is the one the player must press.
            var sw = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sw.name = "PowerSwitch_PRESS";
            sw.transform.SetParent(transform, false);
            sw.transform.localPosition = new Vector3(0f, 0.72f, -0.43f);
            sw.transform.localScale = new Vector3(0.34f, 0.24f, 0.10f);
            Paint(sw, SwitchReady);
            _switchRenderer = sw.GetComponent<Renderer>();
            var swCollider = sw.GetComponent<BoxCollider>();
            if (swCollider != null)
                swCollider.size = new Vector3(1.35f, 1.40f, 1.80f); // forgiving ray/direct target
            var swInteractable = sw.AddComponent<XRSimpleInteractable>();
            WireManager(swInteractable);
            swInteractable.selectEntered.AddListener(_ => OnSwitchFlipped());

            var switchLabelGo = new GameObject("PowerSwitchLabel");
            switchLabelGo.transform.SetParent(sw.transform, false);
            switchLabelGo.transform.localPosition = new Vector3(0f, 0f, -0.58f);
            // Neutralize parent non-uniform scale so the text is readable rather than stretched.
            switchLabelGo.transform.localScale = new Vector3(1f / 0.34f, 1f / 0.24f, 1f / 0.10f) * 0.16f;
            var switchLabel = switchLabelGo.AddComponent<TextMesh>();
            switchLabel.text = "PRESS POWER";
            switchLabel.characterSize = 0.04f;
            switchLabel.fontSize = 48;
            switchLabel.anchor = TextAnchor.MiddleCenter;
            switchLabel.alignment = TextAlignment.Center;
            switchLabel.color = Color.white;

            _powerSwitch = sw;
            _powerSwitch.SetActive(false);

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
            partRb.isKinematic = false;
            partRb.useGravity = false;
            partRb.constraints = RigidbodyConstraints.FreezeAll;
            var partGrab = part.AddComponent<XRGrabInteractable>();
            WireManager(partGrab);
            partGrab.selectEntered.AddListener(_ =>
            {
                if (partRb == null) return;
                partRb.constraints = RigidbodyConstraints.None;
                partRb.useGravity = false;
            });
            partGrab.selectExited.AddListener(_ =>
            {
                if (partRb == null || _part == null || _stage != RepairStage.Part) return;
                partRb.useGravity = true;
            });
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
            if (_stage == RepairStage.Part && _part != null && _socket != null &&
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
            if (_stage != RepairStage.Panel) return;
            _stage = RepairStage.Part;
            // The plate comes free in the hand; once dropped it's junk with physics.
            rb.constraints = RigidbodyConstraints.None;
            rb.useGravity = true;
            panel.transform.SetParent(null, true);
            if (_socket != null) _socket.gameObject.SetActive(true);
            Debug.Log("ZIPTIDE: MACHINE_STAGE id=" + _def.machineId + " stage=panel_off");
            UpdateLabel();
            PublishStageChanged();
        }

        private void SeatPart()
        {
            _stage = RepairStage.Power;
            // Consume the part into the socket. Destroying a selected interactable is the established
            // pattern here (CollectibleRuntime does the same) — XRI unregisters it on destroy.
            Destroy(_part.gameObject);
            _part = null;
            if (_socketRenderer != null) Tint(_socketRenderer, PartColor);
            if (_powerSwitch != null) _powerSwitch.SetActive(true);
            if (_switchRenderer != null) Tint(_switchRenderer, SwitchReady);
            Debug.Log("ZIPTIDE: MACHINE_STAGE id=" + _def.machineId + " stage=part_seated");
            UpdateLabel();
            PublishStageChanged();
        }

        private void OnSwitchFlipped()
        {
            if (_stage != RepairStage.Power) return;
            _stage = RepairStage.Running;
            if (_switchRenderer != null) Tint(_switchRenderer, RunningColor);
            if (_statusLamp != null) Tint(_statusLamp, RunningColor);
            if (_director == null) _director = FindObjectOfType<JobDirector>();
            if (_director != null) _director.ReportRepair(_def.machineId);
            // DS-10 evidence: instance id + whether a director existed to receive the report —
            // a repair that never reaches a director is a different fault than a rejected one.
            Debug.Log("ZIPTIDE: MACHINE_REPAIRED id=" + _def.machineId
                + " instance=" + GetInstanceID()
                + " director=" + (_director != null ? _director.GetInstanceID().ToString() : "NONE"));
            UpdateLabel();
            PublishStageChanged();
        }

        private void PublishStageChanged()
        {
            RepairStageSignals.PublishStageSafely(
                StageChanged,
                _stage,
                ex => Debug.LogWarning("ZIPTIDE: REPAIR_STAGE_SUBSCRIBER_FAIL id=" + MachineId +
                                       " stage=" + _stage + " reason=" + ex.Message));
        }

        private void UpdateLabel()
        {
            if (_label == null) return;
            string name = string.IsNullOrEmpty(_def.displayName)
                ? (_def.machineId ?? "machine").Replace('_', ' ')
                : _def.displayName;
            switch (_stage)
            {
                case RepairStage.Panel: _label.text = name + "\n< pull the access panel >"; break;
                case RepairStage.Part: _label.text = name + "\n< seat the " + (_def.partItemId ?? "part").Replace('_', ' ') + " >"; break;
                case RepairStage.Power: _label.text = name + "\n< press the illuminated POWER switch >"; break;
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
            if (c == null) return;
            if (Application.isPlaying) Destroy(c);
            else DestroyImmediate(c);
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
