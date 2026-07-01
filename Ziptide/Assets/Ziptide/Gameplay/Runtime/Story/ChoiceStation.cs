using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// A two-option story choice set-piece (GAME_PLAN M1): a small pedestal with two selectable panels.
    /// Choosing one writes its story flag into the live profile (RILL reacts via its FlagSet lines,
    /// gating/endings read it like any other flag) and locks the station — a choice is permanent.
    /// Self-built at runtime by JobDirector from <see cref="ChoiceSpawnDefinition"/> pack data (never in
    /// scene YAML). Interactable wiring mirrors WorldTravelStation (manager find + retry).
    /// Logs ZIPTIDE: CHOICE_MADE choice=&lt;id&gt; flag=&lt;flag&gt;.
    /// </summary>
    public class ChoiceStation : MonoBehaviour
    {
        private static readonly Color PanelAColor = new Color(0.16f, 0.45f, 0.55f);
        private static readonly Color PanelBColor = new Color(0.55f, 0.32f, 0.16f);
        private static readonly Color ChosenColor = new Color(0.85f, 0.8f, 0.45f);
        private static readonly Color LockedColor = new Color(0.2f, 0.2f, 0.22f);

        private ChoiceSpawnDefinition _def;
        private Renderer _panelA;
        private Renderer _panelB;
        private bool _resolved;

        /// <summary>Build + arm the station. Call immediately after AddComponent (spawner does).</summary>
        public void Init(ChoiceSpawnDefinition def)
        {
            _def = def ?? new ChoiceSpawnDefinition();

            // If this save already made the choice, show it resolved instead of re-offering it.
            var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            bool aTaken = profile != null && !string.IsNullOrEmpty(_def.flagA) && profile.HasFlag(_def.flagA);
            bool bTaken = profile != null && !string.IsNullOrEmpty(_def.flagB) && profile.HasFlag(_def.flagB);

            Build();
            if (aTaken || bTaken) Resolve(aTaken ? _panelA : _panelB, null, silent: true);
        }

        private void Build()
        {
            // Pedestal.
            var pedestal = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pedestal.name = "Pedestal";
            pedestal.transform.SetParent(transform, false);
            pedestal.transform.localPosition = new Vector3(0f, 0.5f, 0f);
            pedestal.transform.localScale = new Vector3(0.5f, 1.0f, 0.5f);
            Paint(pedestal, new Color(0.13f, 0.13f, 0.16f));

            // Prompt text floating above.
            var prompt = new GameObject("Prompt");
            var tm = prompt.AddComponent<TextMesh>();
            tm.text = _def.prompt;
            tm.characterSize = 0.045f;
            tm.fontSize = 48;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.color = Color.white;
            prompt.transform.SetParent(transform, false);
            prompt.transform.localPosition = new Vector3(0f, 1.75f, 0f);

            _panelA = BuildPanel("OptionA", new Vector3(-0.45f, 1.25f, 0f), _def.labelA, PanelAColor,
                () => Resolve(_panelA, _def.flagA));
            _panelB = BuildPanel("OptionB", new Vector3(0.45f, 1.25f, 0f), _def.labelB, PanelBColor,
                () => Resolve(_panelB, _def.flagB));
        }

        private Renderer BuildPanel(string name, Vector3 localPos, string label, Color color, System.Action onChosen)
        {
            var panel = GameObject.CreatePrimitive(PrimitiveType.Cube);
            panel.name = name;
            panel.transform.SetParent(transform, false);
            panel.transform.localPosition = localPos;
            panel.transform.localScale = new Vector3(0.55f, 0.35f, 0.06f);
            Paint(panel, color);
            var renderer = panel.GetComponent<Renderer>();

            var interactable = panel.AddComponent<XRSimpleInteractable>();
            var mgr = Object.FindObjectOfType<XRInteractionManager>();
            if (mgr != null) interactable.interactionManager = mgr;
            else StartCoroutine(RetryManager(interactable));
            interactable.selectEntered.AddListener(_ => { if (!_resolved) onChosen(); });

            var labelGo = new GameObject("Label");
            var tm = labelGo.AddComponent<TextMesh>();
            tm.text = label;
            tm.characterSize = 0.14f; // counter the panel's non-uniform scale via small char size
            tm.fontSize = 48;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.color = Color.white;
            labelGo.transform.SetParent(panel.transform, false);
            labelGo.transform.localPosition = new Vector3(0f, 0f, -0.6f);
            // Legacy TextMesh inherits the panel's non-uniform scale — neutralize it (travel-door lesson).
            var ls = panel.transform.lossyScale;
            labelGo.transform.localScale = new Vector3(
                Mathf.Approximately(ls.x, 0f) ? 1f : 0.06f / ls.x,
                Mathf.Approximately(ls.y, 0f) ? 1f : 0.06f / ls.y,
                Mathf.Approximately(ls.z, 0f) ? 1f : 0.06f / ls.z);

            return renderer;
        }

        private void Resolve(Renderer chosen, string flag, bool silent = false)
        {
            _resolved = true;

            if (!silent && !string.IsNullOrEmpty(flag))
            {
                var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
                if (profile != null) profile.SetFlag(flag);
                Debug.Log("ZIPTIDE: CHOICE_MADE choice=" + _def.choiceId + " flag=" + flag);
            }

            if (_panelA != null) Tint(_panelA, _panelA == chosen ? ChosenColor : LockedColor);
            if (_panelB != null) Tint(_panelB, _panelB == chosen ? ChosenColor : LockedColor);
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
            Debug.LogWarning("ZIPTIDE: CHOICE_NO_MANAGER after 10 frames");
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
