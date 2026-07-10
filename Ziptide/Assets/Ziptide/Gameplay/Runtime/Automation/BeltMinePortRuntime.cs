using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// HARDWIRING 4.1e — the machine-port adapter: a mini extractor whose ORE PHYSICALLY RIDES THE
    /// BELT. Binds (or creates) the same <see cref="MineState"/> a MiningRigRuntime would — same
    /// machineId = same shared state, and ProfileEconomy still resolves its offline accrual — then
    /// pumps: every beat, if ≥1 is stored AND the port's facing belt can accept, one unit becomes a
    /// riding item (stock only decrements on a successful emit — a jammed line never eats ore).
    /// Downstream, the sink pays the profile, which is exactly what ProductionGraph processors
    /// consume from: mine → belt → sink → profile → factory batches. The loop is closed.
    ///
    /// Patch-time = serialized fields only; visuals + binding happen in Start() (the standing law).
    /// Logs ZIPTIDE: BELT_PORT_EMIT.
    /// </summary>
    public class BeltMinePortRuntime : MonoBehaviour
    {
        [Tooltip("The floor whose port cell this adapter feeds.")]
        public BeltFloorRuntime floor;
        [Tooltip("Port cell coords on the floor (author a Port there).")]
        public int portX, portZ;
        [Tooltip("World id the MineState lives under (scene name by convention).")]
        public string worldId = "";
        [Tooltip("Machine id — share it with a MiningRigRuntime to feed from the SAME hopper.")]
        public string machineId = "belt_mine";
        [Tooltip("Resource mined + emitted.")]
        public string resourceId = "scrap";
        [Tooltip("Accrual rate per second (definition truth — the save keeps only progress).")]
        public float ratePerSecond = 0.5f;
        [Tooltip("Storage cap (0 = uncapped).")]
        public float storageCap = 50f;

        private const float PumpPeriod = 0.4f; // brisk drain when the line is clear

        private MineState _mine;
        private TextMesh _readout;
        private float _nextPump;
        private Transform _drill;    // 4.1j (LAW 6): the rig visibly WORKS while it mines
        private Transform _piston;
        private float _lastEmitAt = -10f;

        private void Start()
        {
            BuildVisual();
            Bind();
        }

        private void Bind()
        {
            var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            if (profile == null) return;
            var world = profile.GetWorld(worldId, createIfMissing: true);
            _mine = world.mines.Find(m => m != null && m.machineId == machineId);
            if (_mine == null)
            {
                _mine = new MineState
                {
                    machineId = machineId,
                    resourceId = resourceId,
                    ratePerSecond = ratePerSecond,
                    storageCap = storageCap,
                    stored = 0,
                    lastResolvedAtUnix = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds()
                };
                world.mines.Add(_mine);
            }
            else
            {
                _mine.resourceId = resourceId;
                _mine.ratePerSecond = ratePerSecond;
                _mine.storageCap = storageCap;
            }
        }

        private void BuildVisual()
        {
            // A stub drill over the port — enough to read "this is where the ore comes from".
            var body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "PortRig";
            body.transform.SetParent(transform, false);
            body.transform.localPosition = new Vector3(0f, 0.55f, 0f);
            body.transform.localScale = new Vector3(0.5f, 0.9f, 0.5f);
            ItemFactory.ApplyURPColor(body, new Color(0.20f, 0.22f, 0.26f));

            var drill = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            drill.name = "PortDrill";
            var dc = drill.GetComponent<Collider>();
            if (dc != null) Destroy(dc);
            drill.transform.SetParent(transform, false);
            drill.transform.localPosition = new Vector3(0f, 0.2f, 0.35f);
            drill.transform.localRotation = Quaternion.Euler(60f, 0f, 0f);
            drill.transform.localScale = new Vector3(0.12f, 0.4f, 0.12f);
            ItemFactory.ApplyURPColor(drill, new Color(0.30f, 0.55f, 0.45f));
            _drill = drill.transform;

            // 4.1j: a counterweight piston riding the body — the rig breathes while it accrues.
            var piston = GameObject.CreatePrimitive(PrimitiveType.Cube);
            piston.name = "PortPiston";
            var pc = piston.GetComponent<Collider>();
            if (pc != null) Destroy(pc);
            piston.transform.SetParent(transform, false);
            piston.transform.localPosition = new Vector3(0f, 0.85f, -0.22f);
            piston.transform.localScale = new Vector3(0.16f, 0.28f, 0.10f);
            ItemFactory.ApplyURPColor(piston, new Color(0.55f, 0.60f, 0.65f));
            _piston = piston.transform;

            var readoutGo = new GameObject("PortReadout");
            _readout = readoutGo.AddComponent<TextMesh>();
            _readout.characterSize = 0.028f;
            _readout.fontSize = 44;
            _readout.anchor = TextAnchor.MiddleCenter;
            _readout.alignment = TextAlignment.Center;
            _readout.color = new Color(0.55f, 0.95f, 0.75f);
            readoutGo.transform.SetParent(transform, false);
            readoutGo.transform.localPosition = new Vector3(0f, 1.35f, 0f);

            foreach (var r in GetComponentsInChildren<Renderer>())
                r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        private void Update()
        {
            if (_mine == null) { Bind(); if (_mine == null) return; }

            // Live accrual while present (offline accrual is ProfileEconomy's job, as with rigs).
            _mine.stored += _mine.ratePerSecond * Time.deltaTime;
            if (_mine.storageCap > 0 && _mine.stored > _mine.storageCap) _mine.stored = _mine.storageCap;
            _mine.lastResolvedAtUnix = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            // The pump: one stored unit becomes one riding item — only on a successful emit.
            if (Time.time >= _nextPump && _mine.stored >= 1.0 && floor != null)
            {
                _nextPump = Time.time + PumpPeriod;
                if (floor.TryEmitPort(portX, portZ, _mine.resourceId))
                {
                    _mine.stored -= 1.0;
                    _lastEmitAt = Time.time;
                    Debug.Log("ZIPTIDE: BELT_PORT_EMIT resource=" + _mine.resourceId +
                              " stored=" + System.Math.Floor(_mine.stored));
                }
            }

            // 4.1j: the rig works on camera — drill spins (frantic while actually feeding the
            // line, lazy while just accruing) and the piston breathes with it.
            bool pumping = Time.time - _lastEmitAt < 1.2f;
            if (_drill != null)
                _drill.Rotate(0f, (pumping ? 540f : 120f) * Time.deltaTime, 0f, Space.Self);
            if (_piston != null)
            {
                float stroke = Mathf.Sin(Time.time * (pumping ? 9f : 2.5f)) * 0.06f;
                _piston.localPosition = new Vector3(0f, 0.85f + stroke, -0.22f);
            }

            if (_readout != null)
            {
                _readout.text = _mine.resourceId.Replace('_', ' ') + "\n"
                    + System.Math.Floor(_mine.stored)
                    + (_mine.storageCap > 0 ? " / " + _mine.storageCap : "");
                var cam = Camera.main;
                if (cam != null)
                    _readout.transform.rotation = Quaternion.LookRotation(
                        _readout.transform.position - cam.transform.position);
            }
        }
    }
}
