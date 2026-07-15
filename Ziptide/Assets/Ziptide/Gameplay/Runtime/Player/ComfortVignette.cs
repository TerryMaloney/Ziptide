using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// HARDWIRING 0.4 — the ONE comfort vignette (CONSISTENCY_SPINE §B). A procedural black iris on
    /// the player camera whose aperture follows <see cref="ComfortCore"/>, driven by the RIG's motion
    /// only — rig translation/yaw is artificial locomotion; natural head turns move the CAMERA within
    /// the rig and never tunnel. Because it samples the rig (not any one controller), walking, ship
    /// flight, and vehicles all get comfort from this single component with zero per-system wiring.
    ///
    /// Self-bootstrapped like SaveSystem (no scene edit); strength is a device-level setting in
    /// PlayerPrefs ("ziptide_comfort_vignette", 0..1, default 0.35 — subtle; 0 disables). Survives
    /// travel on the persistent rig.
    /// </summary>
    public class ComfortVignette : MonoBehaviour
    {
        public const string PrefKey = "ziptide_comfort_vignette";
        private const float DefaultStrength = 0.35f;
        private const float SprintSpeed = 6f;      // m/s ≈ speed01 of 1 (Fortnite-class sprint)
        private const float FullTurnDegPerSec = 120f; // deg/s ≈ turn01 of 1 (full smooth turn)

        private Transform _rig;
        private Vector3 _lastRigPos;
        private float _lastRigYaw;
        private float _aperture = ComfortCore.OpenAperture;
        private float _strength = DefaultStrength;
        private Mesh _mesh;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void EnsureExists()
        {
            if (!RecoveryRuntimeGate.Allows(RecoveryFeatureId.ComfortVignette)) return;
            if (Object.FindObjectOfType<ComfortVignette>() != null) return;
            var rig = Object.FindObjectOfType<PlayerRigPersistence>();
            if (rig == null) return; // no rig (e.g. bare test scene) — nothing to comfort
            var cam = rig.GetComponentInChildren<Camera>();
            if (cam == null) return;

            var go = new GameObject("ComfortVignette");
            go.transform.SetParent(cam.transform, false);
            go.transform.localPosition = new Vector3(0f, 0f, cam.nearClipPlane + 0.05f);
            var v = go.AddComponent<ComfortVignette>();
            v._rig = rig.transform;
            Debug.Log("ZIPTIDE: COMFORT_VIGNETTE ready strength=" + v._strength.ToString("F2"));
        }

        private void Awake()
        {
            _strength = Mathf.Clamp01(PlayerPrefs.GetFloat(PrefKey, DefaultStrength));
            if (_rig == null)
            {
                var rig = Object.FindObjectOfType<PlayerRigPersistence>();
                _rig = rig != null ? rig.transform : null;
            }
            if (_rig != null)
            {
                _lastRigPos = _rig.position;
                _lastRigYaw = _rig.eulerAngles.y;
            }
            BuildIris();
        }

        /// <summary>Runtime setter for a future settings surface — persists and applies immediately.</summary>
        public void SetStrength(float strength01)
        {
            _strength = Mathf.Clamp01(strength01);
            PlayerPrefs.SetFloat(PrefKey, _strength);
        }

        // World-moves-around-you frames (ship flight per the SPACEFLIGHT_PHYSICS law, vehicles later)
        // leave the rig STILL, so rig sampling reads zero motion. Those translators report their
        // apparent motion here each frame; the latch folds into this frame's sample and clears.
        private float _reportedSpeed01;
        private float _reportedTurn01;

        /// <summary>Report externally-rendered artificial motion (normalized 0..1) for THIS frame.
        /// Call every frame while the frame moves; a missed frame simply reopens the iris.</summary>
        public void ReportExternalMotion(float speed01, float turn01)
        {
            if (speed01 > _reportedSpeed01) _reportedSpeed01 = speed01;
            if (turn01 > _reportedTurn01) _reportedTurn01 = turn01;
        }

        private void LateUpdate()
        {
            if (_rig == null || _mesh == null) return;
            float dt = Time.deltaTime;
            if (dt <= 0f) return;

            // Artificial motion only: the rig moves/turns via locomotion code, never via the head.
            Vector3 pos = _rig.position;
            float yaw = _rig.eulerAngles.y;
            float speed01 = (pos - _lastRigPos).magnitude / dt / SprintSpeed;
            float turn01 = Mathf.Abs(Mathf.DeltaAngle(_lastRigYaw, yaw)) / dt / FullTurnDegPerSec;
            _lastRigPos = pos;
            _lastRigYaw = yaw;

            if (_reportedSpeed01 > speed01) speed01 = _reportedSpeed01;
            if (_reportedTurn01 > turn01) turn01 = _reportedTurn01;
            _reportedSpeed01 = 0f;
            _reportedTurn01 = 0f;

            float target = ComfortCore.TargetAperture(speed01, turn01, _strength);
            _aperture = ComfortCore.Step(_aperture, target, dt);
            RebuildRing(_aperture);
        }

        // ── Iris mesh: an annulus (hole → far outer ring) in camera space; unlit black, drawn over
        // everything. No shader/texture dependency — Quest-safe by construction. ──

        private const int Segments = 48;
        private const float OuterRadius = 2.0f;   // comfortably past the view edges at z≈0.06
        private const float FullyOpenHole = 0.60f; // hole radius at aperture 1 (off-screen: invisible)
        private const float ClosedHole = 0.16f;    // hole radius at the MinAperture floor

        private void BuildIris()
        {
            _mesh = new Mesh { name = "ComfortIris" };
            var mf = gameObject.AddComponent<MeshFilter>();
            var mr = gameObject.AddComponent<MeshRenderer>();
            mf.sharedMesh = _mesh;

            var shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null) shader = Shader.Find("Unlit/Color");
            var mat = new Material(shader) { color = Color.black };
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", Color.black);
            mat.renderQueue = 4000; // overlay: after everything, so the tunnel always reads
            mr.sharedMaterial = mat;
            mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            mr.receiveShadows = false;

            RebuildRing(_aperture);
        }

        private Vector3[] _verts;     // cached — inner ring updated in place, zero per-frame alloc
        private float _builtHole = -1f;

        private void RebuildRing(float aperture)
        {
            float t = Mathf.InverseLerp(ComfortCore.MinAperture, ComfortCore.OpenAperture, aperture);
            float hole = Mathf.Lerp(ClosedHole, FullyOpenHole, t);
            if (Mathf.Abs(hole - _builtHole) < 0.0015f) return; // still — don't touch the mesh

            bool firstBuild = _verts == null;
            if (firstBuild)
            {
                _verts = new Vector3[Segments * 2];
                var tris = new int[Segments * 6];
                for (int i = 0; i < Segments; i++)
                {
                    float a = (i / (float)Segments) * Mathf.PI * 2f;
                    _verts[i * 2 + 1] = new Vector3(Mathf.Cos(a) * OuterRadius, Mathf.Sin(a) * OuterRadius, 0f);
                    int ni = (i + 1) % Segments;
                    int b = i * 6;
                    tris[b] = i * 2; tris[b + 1] = ni * 2; tris[b + 2] = i * 2 + 1;
                    tris[b + 3] = ni * 2; tris[b + 4] = ni * 2 + 1; tris[b + 5] = i * 2 + 1;
                }
                FillInner(hole);
                _mesh.vertices = _verts;
                _mesh.triangles = tris;
            }
            else
            {
                FillInner(hole);
                _mesh.vertices = _verts; // same array, updated in place
            }
            _mesh.RecalculateBounds();
            _builtHole = hole;
        }

        private void FillInner(float hole)
        {
            for (int i = 0; i < Segments; i++)
            {
                float a = (i / (float)Segments) * Mathf.PI * 2f;
                _verts[i * 2] = new Vector3(Mathf.Cos(a) * hole, Mathf.Sin(a) * hole, 0f);
            }
        }

        private void OnDestroy()
        {
            if (_mesh != null) Destroy(_mesh);
        }
    }
}
