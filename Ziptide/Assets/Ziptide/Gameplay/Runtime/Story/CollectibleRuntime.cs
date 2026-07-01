using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// A physical story/job pickup (GAME_PLAN M1): a small glowing shard that slowly spins, grabbable
    /// with either hand. On first grab it credits the active job's CollectItemIdCount step
    /// (<see cref="JobDirector.ReportCollect"/>), sets its story flag (this is how a Transmission
    /// fragment becomes a real object — FRAGMENT_T#_FOUND fires the moment you take it), re-syncs the
    /// Transmission clarity tier, then absorbs (destroys). Spawned at runtime by JobDirector from
    /// <see cref="CollectibleSpawnDefinition"/> pack data — never lives in scene YAML.
    /// Build order honors gotcha #6: collider + Rigidbody exist BEFORE the XRGrabInteractable.
    /// </summary>
    public class CollectibleRuntime : MonoBehaviour
    {
        private static readonly Color DefaultAccent = new Color(0.35f, 0.9f, 1f);

        private string _itemId;
        private string _flagOnCollect;
        private JobDirector _director;
        private Transform _visual;
        private bool _collected;

        /// <summary>Build + arm the pickup. Call immediately after AddComponent (spawner does).</summary>
        public void Init(CollectibleSpawnDefinition def, JobDirector director)
        {
            _itemId = def != null ? def.itemId : "sample";
            _flagOnCollect = def != null ? def.flagOnCollect : "";
            _director = director;

            Color accent = def != null && def.accentColor.a > 0.01f ? def.accentColor : DefaultAccent;
            string label = def != null && !string.IsNullOrEmpty(def.displayName)
                ? def.displayName
                : (_itemId ?? "sample").Replace('_', ' ');

            BuildBody(accent, label);
        }

        private void BuildBody(Color accent, string label)
        {
            // Grab volume FIRST (collider + RB before the interactable initializes — gotcha #6).
            var sphere = gameObject.AddComponent<SphereCollider>();
            sphere.radius = 0.16f;
            var rb = gameObject.AddComponent<Rigidbody>();
            rb.isKinematic = true; // floats in place until collected; no physics needed
            rb.useGravity = false;

            var grab = gameObject.AddComponent<XRGrabInteractable>();
            var mgr = Object.FindObjectOfType<XRInteractionManager>();
            if (mgr != null) grab.interactionManager = mgr;
            grab.selectEntered.AddListener(OnGrabbed);

            // Visual: a small spinning "shard" (stretched cube reads as a crystal at graybox quality).
            var shard = GameObject.CreatePrimitive(PrimitiveType.Cube);
            shard.name = "Shard";
            var shardCol = shard.GetComponent<Collider>();
            if (shardCol != null) Destroy(shardCol);
            shard.transform.SetParent(transform, false);
            shard.transform.localScale = new Vector3(0.06f, 0.14f, 0.06f);
            shard.transform.localRotation = Quaternion.Euler(20f, 0f, 25f);
            ApplyUnlit(shard, accent);
            _visual = shard.transform;

            // Floating label so it reads as "pick me up" from a distance.
            var labelGo = new GameObject("Label");
            var tm = labelGo.AddComponent<TextMesh>();
            tm.text = label;
            tm.characterSize = 0.03f;
            tm.fontSize = 48;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.color = accent;
            labelGo.transform.SetParent(transform, false);
            labelGo.transform.localPosition = Vector3.up * 0.32f;
        }

        private void Update()
        {
            if (_visual != null)
            {
                _visual.Rotate(Vector3.up, 45f * Time.deltaTime, Space.World);
                _visual.localPosition = Vector3.up * (Mathf.Sin(Time.time * 1.6f) * 0.03f);
            }

            // Billboard the label at the camera so it's always readable.
            var cam = Camera.main;
            if (cam != null)
            {
                var label = transform.Find("Label");
                if (label != null)
                    label.rotation = Quaternion.LookRotation(label.position - cam.transform.position);
            }
        }

        private static void ApplyUnlit(GameObject go, Color color)
        {
            var r = go.GetComponent<Renderer>();
            if (r == null) return;
            var shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null) shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) return;
            var mat = new Material(shader);
            mat.color = color;
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
            r.sharedMaterial = mat;
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        private void OnGrabbed(SelectEnterEventArgs _)
        {
            if (_collected) return;
            _collected = true;

            if (_director == null) _director = FindObjectOfType<JobDirector>();
            if (_director != null) _director.ReportCollect(_itemId);

            var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            if (profile != null && !string.IsNullOrEmpty(_flagOnCollect))
            {
                profile.SetFlag(_flagOnCollect);
                // A fragment pickup must raise the clarity tier immediately (not only at job end).
                TransmissionProgress.SyncClarityFlags(profile);
            }

            Debug.Log("ZIPTIDE: COLLECTED item=" + _itemId +
                      (string.IsNullOrEmpty(_flagOnCollect) ? "" : " flag=" + _flagOnCollect));

            // Absorb: it's in the inventory now, not the hand.
            Destroy(gameObject);
        }
    }
}
