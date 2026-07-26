using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// The hands-on repair fantasy (GAME_PLAN M2): a broken machine fixed in three PHYSICAL stages —
    /// 1) grab the access PANEL off, 2) fetch the replacement PART and seat it in the exposed socket,
    /// 3) press the illuminated power SWITCH. Then the machine hums back to life and credits
    /// RepairMachineCount job steps through JobDirector.
    ///
    /// Each physical owner also owns its restrained feedback: the controller that performed the work
    /// receives one bounded pulse, the machine plays one short local procedural sound, and the newly
    /// actionable surface visibly confirms the state change. No global haptic/audio manager and no
    /// parallel repair state are introduced.
    /// </summary>
    public class RepairableMachine : MonoBehaviour, IScannable
    {
        private static readonly Color BodyColor = new Color(0.16f, 0.17f, 0.20f);
        private static readonly Color PanelColor = new Color(0.32f, 0.20f, 0.14f);
        private static readonly Color SocketEmpty = new Color(0.55f, 0.25f, 0.20f);
        private static readonly Color PartColor = new Color(0.85f, 0.65f, 0.25f);
        private static readonly Color SwitchOff = new Color(0.45f, 0.15f, 0.12f);
        private static readonly Color SwitchReady = new Color(1.0f, 0.48f, 0.08f);
        private static readonly Color RunningColor = new Color(0.25f, 0.75f, 0.55f);
        private const float SeatDistance = 0.3f;

        private MachineSpawnDefinition _def;
        private JobDirector _director;
        private RepairStage _stage = RepairStage.Panel;

        public string MachineId => _def != null ? _def.machineId : null;
        public RepairStage CurrentStage => _stage;
        public event System.Action<RepairStage> StageChanged;
        public bool IsRepaired => _stage == RepairStage.Running;

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
        private AudioSource _feelAudio;
        private XRBaseControllerInteractor _lastPartHand;
        private Coroutine _surfacePulse;

        private static AudioClip _panelReleaseClip;
        private static AudioClip _partSeatClip;
        private static AudioClip _powerOnClip;

        /// <summary>Build + arm the machine. Call immediately after AddComponent (spawner does).</summary>
        public void Init(MachineSpawnDefinition def, JobDirector director)
        {
            _def = def ?? new MachineSpawnDefinition();
            _director = director;
            Build();
        }

        private void Build()
        {
            _feelAudio = gameObject.AddComponent<AudioSource>();
            _feelAudio.playOnAwake = false;
            _feelAudio.spatialBlend = 1f;
            _feelAudio.rolloffMode = AudioRolloffMode.Linear;
            _feelAudio.minDistance = 0.4f;
            _feelAudio.maxDistance = 8f;
            _feelAudio.volume = 0.48f;

            var body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "Body";
            body.transform.SetParent(transform, false);
            body.transform.localPosition = new Vector3(0f, 0.75f, 0f);
            body.transform.localScale = new Vector3(1.0f, 1.5f, 0.7f);
            Paint(body, BodyColor);

            var lamp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            lamp.name = "StatusIndicator"; StripCollider(lamp);
            lamp.transform.SetParent(transform, false);
            lamp.transform.localPosition = new Vector3(0f, 1.34f, -0.37f);
            lamp.transform.localScale = new Vector3(0.18f, 0.07f, 0.035f);
            Paint(lamp, SwitchOff);
            _statusLamp = lamp.GetComponent<Renderer>();

            var labelGo = new GameObject("Label");
            _label = labelGo.AddComponent<TextMesh>();
            _label.characterSize = 0.035f;
            _label.fontSize = 48;
            _label.anchor = TextAnchor.MiddleCenter;
            _label.alignment = TextAlignment.Center;
            _label.color = new Color(1f, 0.85f, 0.6f);
            labelGo.transform.SetParent(transform, false);
            labelGo.transform.localPosition = new Vector3(0f, 1.58f, -0.38f);

            var socket = GameObject.CreatePrimitive(PrimitiveType.Cube);
            socket.name = "Socket"; StripCollider(socket);
            socket.transform.SetParent(transform, false);
            socket.transform.localPosition = new Vector3(0f, 0.85f, -0.30f);
            socket.transform.localScale = new Vector3(0.24f, 0.24f, 0.12f);
            Paint(socket, SocketEmpty);
            _socket = socket.transform;
            _socketRenderer = socket.GetComponent<Renderer>();
            socket.SetActive(false);

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
            panelGrab.throwOnDetach = false;
            WireManager(panelGrab);
            panelGrab.selectEntered.AddListener(args =>
                OnPanelPulled(panel, panelRb, SelectingController(args)));
            panelGrab.selectExited.AddListener(_ => SettleLooseBody(panelRb, useGravity: true));
            var panelVisual = GameObject.CreatePrimitive(PrimitiveType.Cube);
            panelVisual.name = "Plate"; StripCollider(panelVisual);
            panelVisual.transform.SetParent(panel.transform, false);
            panelVisual.transform.localScale = new Vector3(0.5f, 0.5f, 0.05f);
            Paint(panelVisual, PanelColor);

            var sw = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sw.name = "PowerSwitch_PRESS";
            sw.transform.SetParent(transform, false);
            sw.transform.localPosition = new Vector3(0f, 0.72f, -0.43f);
            sw.transform.localScale = new Vector3(0.34f, 0.24f, 0.10f);
            Paint(sw, SwitchReady);
            _switchRenderer = sw.GetComponent<Renderer>();
            var swCollider = sw.GetComponent<BoxCollider>();
            if (swCollider != null)
                swCollider.size = new Vector3(1.35f, 1.40f, 1.80f);
            var swInteractable = sw.AddComponent<XRSimpleInteractable>();
            WireManager(swInteractable);
            swInteractable.selectEntered.AddListener(args =>
                OnSwitchFlipped(SelectingController(args)));

            var switchLabelGo = new GameObject("PowerSwitchLabel");
            switchLabelGo.transform.SetParent(sw.transform, false);
            switchLabelGo.transform.localPosition = new Vector3(0f, 0f, -0.58f);
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
            partGrab.throwOnDetach = false;
            WireManager(partGrab);
            partGrab.selectEntered.AddListener(args =>
            {
                _lastPartHand = SelectingController(args);
                if (partRb == null) return;
                partRb.velocity = Vector3.zero;
                partRb.angularVelocity = Vector3.zero;
                partRb.constraints = RigidbodyConstraints.None;
                partRb.useGravity = false;
            });
            partGrab.selectExited.AddListener(_ =>
            {
                if (partRb == null || _part == null || _stage != RepairStage.Part) return;
                SettleLooseBody(partRb, useGravity: true);
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
            if (_stage == RepairStage.Part && _part != null && _socket != null &&
                Vector3.Distance(_part.position, _socket.position) <= SeatDistance)
            {
                SeatPart();
            }

            var cam = Camera.main;
            if (cam != null && _label != null)
                _label.transform.rotation = Quaternion.LookRotation(_label.transform.position - cam.transform.position);
        }

        private void OnPanelPulled(GameObject panel, Rigidbody rb, XRBaseControllerInteractor hand)
        {
            if (_stage != RepairStage.Panel) return;
            _stage = RepairStage.Part;
            rb.constraints = RigidbodyConstraints.None;
            rb.useGravity = true;
            panel.transform.SetParent(null, true);
            if (_socket != null) _socket.gameObject.SetActive(true);
            SendFeedback(hand, "repair_panel", 0.22f, 0.045f, PanelReleaseClip());
            PulseSurface(_socketRenderer, SocketEmpty, PartColor, 0.28f);
            Debug.Log("ZIPTIDE: MACHINE_STAGE id=" + _def.machineId + " stage=panel_off");
            UpdateLabel();
            PublishStageChanged();
        }

        private void SeatPart()
        {
            _stage = RepairStage.Power;
            Destroy(_part.gameObject);
            _part = null;
            if (_socketRenderer != null) Tint(_socketRenderer, PartColor);
            if (_powerSwitch != null) _powerSwitch.SetActive(true);
            if (_switchRenderer != null) Tint(_switchRenderer, SwitchReady);
            SendFeedback(_lastPartHand, "repair_part_seated", 0.38f, 0.075f, PartSeatClip());
            PulseSurface(_switchRenderer, SwitchReady, Color.white, 0.32f);
            _lastPartHand = null;
            Debug.Log("ZIPTIDE: MACHINE_STAGE id=" + _def.machineId + " stage=part_seated");
            UpdateLabel();
            PublishStageChanged();
        }

        private void OnSwitchFlipped(XRBaseControllerInteractor hand)
        {
            if (_stage != RepairStage.Power) return;
            _stage = RepairStage.Running;
            if (_switchRenderer != null) Tint(_switchRenderer, RunningColor);
            if (_statusLamp != null) Tint(_statusLamp, RunningColor);
            SendFeedback(hand, "repair_power_on", 0.62f, 0.12f, PowerOnClip());
            PulseSurface(_statusLamp, RunningColor, Color.white, 0.48f);
            if (_director == null) _director = FindObjectOfType<JobDirector>();
            if (_director != null) _director.ReportRepair(_def.machineId);
            Debug.Log("ZIPTIDE: MACHINE_REPAIRED id=" + _def.machineId
                + " instance=" + GetInstanceID()
                + " director=" + (_director != null ? _director.GetInstanceID().ToString() : "NONE"));
            UpdateLabel();
            PublishStageChanged();
        }

        private void SendFeedback(XRBaseControllerInteractor hand, string verb,
            float amplitude, float duration, AudioClip clip)
        {
            if (hand != null)
            {
                hand.SendHapticImpulse(amplitude, duration);
                Debug.Log("ZIPTIDE: HAPTIC verb=" + verb + " hand=" + hand.gameObject.name
                    + " amp=" + amplitude.ToString("F2") + " sec=" + duration.ToString("F3"));
            }
            if (_feelAudio != null && clip != null) _feelAudio.PlayOneShot(clip);
        }

        private void PulseSurface(Renderer renderer, Color baseColor, Color peak, float seconds)
        {
            if (renderer == null) return;
            if (_surfacePulse != null) StopCoroutine(_surfacePulse);
            _surfacePulse = StartCoroutine(PulseSurfaceRoutine(renderer, baseColor, peak, seconds));
        }

        private IEnumerator PulseSurfaceRoutine(Renderer renderer, Color baseColor, Color peak, float seconds)
        {
            if (renderer == null) yield break;
            Material material = renderer.material;
            string prop = material.HasProperty("_BaseColor") ? "_BaseColor" :
                          (material.HasProperty("_Color") ? "_Color" : null);
            if (prop == null) yield break;

            float half = Mathf.Max(0.04f, seconds * 0.5f);
            for (float t = 0f; t < seconds; t += Time.deltaTime)
            {
                float p = t < half ? t / half : 1f - (t - half) / half;
                material.SetColor(prop, Color.Lerp(baseColor, peak, Mathf.Clamp01(p)));
                yield return null;
            }
            material.SetColor(prop, baseColor);
            _surfacePulse = null;
        }

        private static XRBaseControllerInteractor SelectingController(SelectEnterEventArgs args)
        {
            return args != null ? args.interactorObject as XRBaseControllerInteractor : null;
        }

        private static AudioClip PanelReleaseClip()
        {
            if (_panelReleaseClip == null)
                _panelReleaseClip = BuildStageClip("Repair_PanelRelease", 170f, 95f, 0.11f, 0.22f);
            return _panelReleaseClip;
        }

        private static AudioClip PartSeatClip()
        {
            if (_partSeatClip == null)
                _partSeatClip = BuildStageClip("Repair_PartSeat", 260f, 390f, 0.13f, 0.08f);
            return _partSeatClip;
        }

        private static AudioClip PowerOnClip()
        {
            if (_powerOnClip == null)
                _powerOnClip = BuildStageClip("Repair_PowerOn", 240f, 660f, 0.28f, 0.04f);
            return _powerOnClip;
        }

        private static AudioClip BuildStageClip(string name, float fromHz, float toHz,
            float duration, float noiseAmount)
        {
            const int sampleRate = 22050;
            int count = Mathf.CeilToInt(sampleRate * duration);
            var samples = new float[count];
            uint noise = 0x9E3779B9u;
            for (int i = 0; i < count; i++)
            {
                float t = i / (float)sampleRate;
                float p = Mathf.Clamp01(t / duration);
                float frequency = Mathf.Lerp(fromHz, toHz, p);
                float envelope = Mathf.Sin(Mathf.PI * p) * Mathf.Exp(-t * 3.2f);
                float tone = Mathf.Sin(2f * Mathf.PI * frequency * t);
                noise = noise * 1664525u + 1013904223u;
                float n = ((noise >> 8) & 0xFFFFu) / 32768f - 1f;
                samples[i] = Mathf.Clamp((tone * 0.52f + n * noiseAmount) * envelope, -0.72f, 0.72f);
            }
            AudioClip clip = AudioClip.Create(name, count, 1, sampleRate, false);
            clip.SetData(samples, 0);
            return clip;
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

        private static void SettleLooseBody(Rigidbody body, bool useGravity)
        {
            if (body == null) return;
            body.isKinematic = false;
            body.constraints = RigidbodyConstraints.None;
            body.useGravity = useGravity;
            body.velocity = Vector3.zero;
            body.angularVelocity = Vector3.zero;
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
