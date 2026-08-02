using System;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Diegetic Cozy / Standard / Bold selector. It persists the device preset, then delegates each
    /// dial to existing owners: LocomotionDirector, ComfortVignette and ZiplineRuntime. It never moves
    /// the rig, writes PlayerProfile, creates a second vignette, or owns traversal physics.
    /// </summary>
    public sealed class ComfortConsoleRuntime : MonoBehaviour
    {
        public static event Action<ComfortPreset> PresetConfirmed;

        [SerializeField] private bool publishFirstHourConfirmation = true;
        private bool _built;

        public void Configure(bool publishConfirmation)
        {
            publishFirstHourConfirmation = publishConfirmation;
        }

        private void Start()
        {
            BuildSurface();
        }

        public void SelectPreset(ComfortPreset preset)
        {
            ComfortSettings.SelectPreset(preset);
            ComfortDialSet settings = ComfortSettings.Resolve(preset);
            ApplyToScene(settings);

            string name = preset.ToString().ToLowerInvariant();
            Debug.Log("ZIPTIDE: COMFORT_PRESET preset=" + name);
            if (publishFirstHourConfirmation)
                PublishSafely(PresetConfirmed, preset);
        }

        public static void ApplyCurrentToScene()
        {
            ApplyToScene(ComfortSettings.Resolve(ComfortSettings.CurrentPreset));
        }

        public static void ApplyToScene(ComfortDialSet settings)
        {
            foreach (var director in FindObjectsOfType<LocomotionDirector>())
                director.ApplyComfortSettings(settings);

            foreach (var vignette in FindObjectsOfType<ComfortVignette>())
                vignette.SetStrength(settings.vignetteStrength);

            foreach (var zipline in FindObjectsOfType<ZiplineRuntime>())
                zipline.maxSpeed = settings.ziplineMaxSpeed;
        }

        private void BuildSurface()
        {
            if (_built) return;
            _built = true;

            var board = GameObject.CreatePrimitive(PrimitiveType.Cube);
            board.name = "ComfortBoard";
            board.transform.SetParent(transform, false);
            board.transform.localPosition = new Vector3(0f, 1.15f, 0f);
            board.transform.localScale = new Vector3(1.75f, 0.8f, 0.08f);
            Paint(board, new Color(0.07f, 0.11f, 0.16f));
            StripCollider(board);

            AddLabel(board.transform, "COMFORT\nCHOOSE BEFORE MOVING", new Vector3(0f, 0.22f, -0.56f), 0.035f);
            AddTile("COZY", ComfortPreset.Cozy, new Vector3(-0.52f, -0.18f, -0.62f),
                new Color(0.22f, 0.55f, 0.72f));
            AddTile("STANDARD", ComfortPreset.Standard, new Vector3(0f, -0.18f, -0.62f),
                new Color(0.25f, 0.72f, 0.48f));
            AddTile("BOLD", ComfortPreset.Bold, new Vector3(0.52f, -0.18f, -0.62f),
                new Color(0.78f, 0.35f, 0.18f));
        }

        private void AddTile(string label, ComfortPreset preset, Vector3 localPosition, Color color)
        {
            var tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tile.name = "Tile_" + label;
            tile.transform.SetParent(transform, false);
            tile.transform.localPosition = localPosition;
            tile.transform.localScale = new Vector3(0.42f, 0.24f, 0.12f);
            Paint(tile, color);

            var interactable = tile.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>();
            var manager = FindObjectOfType<XRInteractionManager>();
            if (manager != null) interactable.interactionManager = manager;
            interactable.selectEntered.AddListener(_ => SelectPreset(preset));
            AddLabel(tile.transform, label, new Vector3(0f, 0f, -0.56f), 0.021f);
        }

        private static void AddLabel(Transform parent, string text, Vector3 localPosition, float size)
        {
            var go = new GameObject("Label_" + text.Replace('\n', '_'));
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
            var tm = go.AddComponent<TextMesh>();
            tm.text = text;
            tm.characterSize = size;
            tm.fontSize = 64;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.color = new Color(0.94f, 0.97f, 1f);
        }

        private static void StripCollider(GameObject go)
        {
            var collider = go.GetComponent<Collider>();
            if (collider != null) Destroy(collider);
        }

        private static void Paint(GameObject go, Color color)
        {
            var renderer = go.GetComponent<Renderer>();
            if (renderer == null) return;
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) return;
            var material = new Material(shader);
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            else material.color = color;
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        private static void PublishSafely(Action<ComfortPreset> subscribers, ComfortPreset preset)
        {
            if (subscribers == null) return;
            foreach (Action<ComfortPreset> subscriber in subscribers.GetInvocationList())
            {
                try { subscriber(preset); }
                catch (Exception ex)
                {
                    Debug.LogWarning("ZIPTIDE: COMFORT_SUBSCRIBER_FAIL reason=" + ex.Message);
                }
            }
        }
    }
}
