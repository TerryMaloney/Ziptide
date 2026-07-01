using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// The de-garble playback console (GAME_PLAN M1 stub; audio at M6): a small terminal that, when
    /// selected, plays the assembled Transmission at the CURRENT clarity tier —
    /// <see cref="TransmissionProgress.ComputeTier"/> → <see cref="TransmissionText.Render"/> on its
    /// screen. Spawned by JobDirector next to any fragment pickup (the playback device lives where the
    /// recording was found). Re-selecting re-renders, so returning with more fragments shows the
    /// message de-garbled further. Logs ZIPTIDE: TRANSMISSION_PLAYBACK tier=&lt;n&gt;.
    /// </summary>
    public class TransmissionConsole : MonoBehaviour
    {
        private static readonly Color BodyColor = new Color(0.10f, 0.12f, 0.15f);
        private static readonly Color ScreenIdle = new Color(0.15f, 0.35f, 0.45f);
        private static readonly Color ScreenLive = new Color(0.25f, 0.75f, 0.85f);

        private TextMesh _screenText;
        private Renderer _screen;

        private void Start()
        {
            Build();
        }

        private void Build()
        {
            var body = GameObject.CreatePrimitive(PrimitiveType.Cube);
            body.name = "Body";
            body.transform.SetParent(transform, false);
            body.transform.localPosition = new Vector3(0f, 0.55f, 0f);
            body.transform.localScale = new Vector3(0.5f, 1.1f, 0.3f);
            Paint(body, BodyColor);

            var screen = GameObject.CreatePrimitive(PrimitiveType.Cube);
            screen.name = "Screen";
            screen.transform.SetParent(transform, false);
            screen.transform.localPosition = new Vector3(0f, 1.25f, 0f);
            screen.transform.localRotation = Quaternion.Euler(-15f, 0f, 0f);
            screen.transform.localScale = new Vector3(0.62f, 0.42f, 0.05f);
            Paint(screen, ScreenIdle);
            _screen = screen.GetComponent<Renderer>();

            var interactable = screen.AddComponent<XRSimpleInteractable>();
            var mgr = Object.FindObjectOfType<XRInteractionManager>();
            if (mgr != null) interactable.interactionManager = mgr;
            else StartCoroutine(RetryManager(interactable));
            interactable.selectEntered.AddListener(_ => Play());

            var textGo = new GameObject("ScreenText");
            _screenText = textGo.AddComponent<TextMesh>();
            _screenText.text = "TRANSMISSION\n( select to play )";
            _screenText.characterSize = 0.02f;
            _screenText.fontSize = 48;
            _screenText.anchor = TextAnchor.MiddleCenter;
            _screenText.alignment = TextAlignment.Center;
            _screenText.color = new Color(0.8f, 0.95f, 1f);
            textGo.transform.SetParent(transform, false); // sibling of the screen: no non-uniform scale
            textGo.transform.localPosition = new Vector3(0f, 1.25f, -0.06f);
            textGo.transform.localRotation = Quaternion.Euler(-15f, 0f, 0f);
        }

        private void Play()
        {
            var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            int tier = TransmissionProgress.ComputeTier(profile);
            if (_screenText != null) _screenText.text = TransmissionText.Render(tier);
            if (_screen != null)
            {
                var mat = _screen.material;
                if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", ScreenLive);
                else mat.color = ScreenLive;
            }
            Debug.Log("ZIPTIDE: TRANSMISSION_PLAYBACK tier=" + tier);
        }

        private IEnumerator RetryManager(XRSimpleInteractable interactable)
        {
            for (int i = 0; i < 10; i++)
            {
                yield return null;
                if (interactable == null) yield break;
                var mgr = Object.FindObjectOfType<XRInteractionManager>();
                if (mgr != null) { interactable.interactionManager = mgr; yield break; }
            }
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
