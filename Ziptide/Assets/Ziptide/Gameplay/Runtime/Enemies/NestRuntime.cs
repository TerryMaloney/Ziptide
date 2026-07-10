using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content.Ecology;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// ECOLOGY 4.3d — a species' physical HOME, built rich per LAW 6 (a nest is a place, not a
    /// marker): a layered mound with an entrance hollow, a clutch of brood-glow eggs breathing
    /// light inside, and a ring of territory scratches that warn you whose ground this is.
    /// Placed by the EcologyDirector at the population's heart and its far ranges
    /// (EcologyCore.NestSitesFor). DISTURB IT — select the brood glow — and the whole species
    /// WAKES, including the denned-up individuals the day/night census had sleeping: the world
    /// answers for its young. Non-lethal canon: nothing is destroyed; the eggs pay a small spore
    /// find and keep glowing angrier. Logs ZIPTIDE: ECOLOGY_NEST / ECOLOGY_NEST_DISTURBED.
    /// </summary>
    public class NestRuntime : MonoBehaviour
    {
        private string _creatureId;
        private Renderer[] _broodRenderers;
        private bool _disturbed;

        /// <summary>Build + arm. Called by EcologyDirector right after AddComponent (runtime only).</summary>
        public void Init(string creatureId, Color accent)
        {
            _creatureId = creatureId;
            Build(accent);
            Debug.Log("ZIPTIDE: ECOLOGY_NEST species=" + creatureId + " at=" + transform.position.ToString("F0"));
        }

        private void Build(Color accent)
        {
            var earth = new Color(0.26f, 0.21f, 0.17f);
            var earthDark = new Color(0.19f, 0.15f, 0.12f);

            // The mound: three offset, squashed layers — a grown thing, not a placed box.
            for (int i = 0; i < 3; i++)
            {
                var layer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                layer.name = "Mound" + i;
                layer.transform.SetParent(transform, false);
                float t = i / 2f;
                layer.transform.localPosition = new Vector3(
                    (i % 2 == 0 ? 1f : -1f) * 0.18f * i, 0.25f + i * 0.28f, 0.1f * i);
                layer.transform.localScale = new Vector3(2.4f - t * 0.9f, 0.9f - t * 0.25f, 2.2f - t * 0.8f);
                Paint(layer, Color.Lerp(earth, earthDark, t));
            }

            // The entrance hollow — a dark mouth low on the south face.
            var mouth = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            mouth.name = "Entrance"; StripCollider(mouth);
            mouth.transform.SetParent(transform, false);
            mouth.transform.localPosition = new Vector3(0f, 0.35f, -1.0f);
            mouth.transform.localScale = new Vector3(0.7f, 0.55f, 0.5f);
            Paint(mouth, new Color(0.05f, 0.04f, 0.04f));

            // The clutch: three brood eggs glowing in the mouth — the thing you shouldn't touch.
            _broodRenderers = new Renderer[3];
            for (int i = 0; i < 3; i++)
            {
                var egg = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                egg.name = "Brood" + i;
                egg.transform.SetParent(transform, false);
                float a = (i - 1) * 0.5f;
                egg.transform.localPosition = new Vector3(a * 0.35f, 0.22f, -1.05f - Mathf.Abs(a) * 0.1f);
                egg.transform.localScale = Vector3.one * (0.22f - Mathf.Abs(a) * 0.04f);
                Paint(egg, accent);
                _broodRenderers[i] = egg.GetComponent<Renderer>();
                if (i == 1) // the center egg is the disturb point
                {
                    var interactable = egg.AddComponent<XRSimpleInteractable>();
                    var mgr = Object.FindObjectOfType<XRInteractionManager>();
                    if (mgr != null) interactable.interactionManager = mgr;
                    interactable.selectEntered.AddListener(_ => Disturb());
                }
            }

            // Territory scratches: a warning ring of clawed stakes and turned stones.
            for (int i = 0; i < 5; i++)
            {
                float a = i * Mathf.PI * 2f / 5f + 0.4f;
                var stake = GameObject.CreatePrimitive(PrimitiveType.Cube);
                stake.name = "Territory" + i;
                stake.transform.SetParent(transform, false);
                float h = 0.5f + (i % 2) * 0.35f;
                stake.transform.localPosition = new Vector3(Mathf.Cos(a) * 2.6f, h * 0.5f, Mathf.Sin(a) * 2.6f);
                stake.transform.localRotation = Quaternion.Euler((i % 2) * 14f - 7f, a * Mathf.Rad2Deg, 8f);
                stake.transform.localScale = new Vector3(0.12f, h, 0.12f);
                Paint(stake, earthDark);
            }
        }

        private void Update()
        {
            // The brood breathes — calm and slow intact, fast and hot once disturbed.
            if (_broodRenderers == null) return;
            float rate = _disturbed ? 6f : 1.4f;
            float pulse = 0.7f + 0.3f * Mathf.Sin(Time.time * rate);
            foreach (var r in _broodRenderers)
                if (r != null && r.material != null)
                {
                    if (r.material.HasProperty("_BaseColor"))
                    {
                        Color c = r.material.GetColor("_BaseColor");
                        r.material.SetColor("_BaseColor", new Color(c.r, c.g, c.b, 1f) * pulse);
                    }
                }
        }

        private void Disturb()
        {
            if (_disturbed) return;
            _disturbed = true;

            // The world answers for its young: EVERY member of this species wakes — including the
            // denned-up ones the census left sleeping.
            int woken = 0;
            foreach (var c in Object.FindObjectsOfType<CreatureRuntime>(true))
            {
                if (c == null || c.creatureId != _creatureId) continue;
                if (!c.gameObject.activeSelf) { c.gameObject.SetActive(true); woken++; }
            }

            // Non-lethal, kid-fair: a small find, not a raid — the eggs stay, glowing angrier.
            var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            double granted = SalvageCacheRuntime.GrantTo(profile, "spore", 2.0);

            Debug.Log("ZIPTIDE: ECOLOGY_NEST_DISTURBED species=" + _creatureId +
                      " woken=" + woken + " granted=" + granted.ToString("F0"));
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
