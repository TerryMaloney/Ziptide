using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Visuals;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// In-world station that spawns one button per available theme. Ray-select a button to apply that theme.
    /// </summary>
    public class ThemeSwitchStation : MonoBehaviour
    {
        private const float ButtonSize = 0.15f;
        private const float ButtonSpacing = 0.25f;

        private WorldRuntime _worldRuntime;
        private List<VisualThemeProfile> _themes = new List<VisualThemeProfile>();
        private readonly List<GameObject> _buttons = new List<GameObject>();

        /// <summary>
        /// Call from WorldRuntime on Start to pass themes and runtime. Builds the button row.
        /// </summary>
        public void SetThemes(List<VisualThemeProfile> themes, WorldRuntime worldRuntime)
        {
            _worldRuntime = worldRuntime;
            _themes.Clear();
            if (themes != null)
                _themes.AddRange(themes);

            BuildButtons();
        }

        private void BuildButtons()
        {
            foreach (var b in _buttons)
            {
                if (b != null)
                    Destroy(b);
            }
            _buttons.Clear();

            if (_worldRuntime == null || _themes.Count == 0) return;

            float startX = -(_themes.Count - 1) * ButtonSpacing * 0.5f;

            for (int i = 0; i < _themes.Count; i++)
            {
                VisualThemeProfile theme = _themes[i];
                if (theme == null) continue;

                GameObject button = GameObject.CreatePrimitive(PrimitiveType.Cube);
                button.name = "ThemeButton_" + (theme.name ?? i.ToString());
                button.transform.SetParent(transform);
                button.transform.localPosition = new Vector3(startX + i * ButtonSpacing, 0.5f, 0f);
                button.transform.localScale = Vector3.one * ButtonSize;
                button.transform.localRotation = Quaternion.identity;

                var r = button.GetComponent<Renderer>();
                if (r != null)
                {
                    var shader = Shader.Find("Universal Render Pipeline/Lit");
                    if (shader != null)
                    {
                        var mat = new Material(shader);
                        mat.color = theme.groundTint;
                        r.sharedMaterial = mat;
                    }
                }

                var col = button.GetComponent<BoxCollider>();
                if (col != null)
                    col.isTrigger = false;

                var interactable = button.AddComponent<XRSimpleInteractable>();
                var hook = button.AddComponent<ThemeSwitchButtonHook>();
                hook.Setup(_worldRuntime, theme, interactable);

                _buttons.Add(button);
            }
        }
    }

    /// <summary>
    /// Per-button hook: on select, applies the assigned theme via WorldRuntime.
    /// </summary>
    public class ThemeSwitchButtonHook : MonoBehaviour
    {
        private WorldRuntime _worldRuntime;
        private VisualThemeProfile _theme;
        private XRSimpleInteractable _interactable;

        public void Setup(WorldRuntime worldRuntime, VisualThemeProfile theme, XRSimpleInteractable interactable)
        {
            _worldRuntime = worldRuntime;
            _theme = theme;
            _interactable = interactable;
            if (_interactable != null)
                _interactable.selectEntered.AddListener(OnSelectEntered);
        }

        private void OnSelectEntered(SelectEnterEventArgs args)
        {
            if (_worldRuntime != null && _theme != null)
                _worldRuntime.ApplyTheme(_theme);
        }

        private void OnDestroy()
        {
            if (_interactable != null)
                _interactable.selectEntered.RemoveListener(OnSelectEntered);
        }
    }
}
