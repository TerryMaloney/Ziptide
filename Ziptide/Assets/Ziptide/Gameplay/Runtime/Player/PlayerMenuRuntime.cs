using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Persistent, in-headset player escape surface. Y toggles a compact diegetic field menu in
    /// every content world. The menu never loads _Boot (bootstrap is not a destination); Return to
    /// Ship travels through the canonical TravelCoordinator to W000_DriftIn.
    ///
    /// It pauses only player locomotion/turning. World time keeps running so scene systems, travel,
    /// networking and lifecycle owners are never wedged by Time.timeScale changes.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class PlayerMenuRuntime : MonoBehaviour
    {
        private const float MenuDistance = 0.85f;
        private const float MenuDrop = 0.12f;

        private readonly List<Behaviour> _suspended = new List<Behaviour>();
        private InputAction _toggleAction;
        private GameObject _menuRoot;
        private bool _open;

        public bool IsOpen => _open;

        private void OnEnable()
        {
            if (_toggleAction == null)
            {
                _toggleAction = new InputAction("ZiptidePlayerMenu", InputActionType.Button);
                _toggleAction.AddBinding("<XRController>{LeftHand}/secondaryButton"); // Y
            }
            _toggleAction.Enable();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            _toggleAction?.Disable();
            if (_open) CloseMenu("component_disabled");
        }

        private void OnDestroy()
        {
            _toggleAction?.Dispose();
        }

        private void Update()
        {
            if (_toggleAction == null || !_toggleAction.WasPressedThisFrame()) return;
            if (TravelCoordinator.IsTravelling)
            {
                Debug.Log("ZIPTIDE: PLAYER_MENU_BLOCKED reason=travel_in_progress");
                return;
            }

            if (_open) CloseMenu("y_toggle");
            else OpenMenu();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (_open) CloseMenu("scene_loaded");
        }

        private void OpenMenu()
        {
            Camera cam = GetComponentInChildren<Camera>(true);
            if (cam == null) cam = Camera.main;
            if (cam == null)
            {
                Debug.LogWarning("ZIPTIDE: PLAYER_MENU_BLOCKED reason=no_camera");
                return;
            }

            BuildMenuIfNeeded();
            if (_menuRoot == null) return;

            Vector3 flat = cam.transform.forward;
            flat.y = 0f;
            if (flat.sqrMagnitude < 0.0001f) flat = transform.forward;
            if (flat.sqrMagnitude < 0.0001f) flat = Vector3.forward;
            flat.Normalize();

            _menuRoot.transform.SetPositionAndRotation(
                cam.transform.position + flat * MenuDistance + Vector3.down * MenuDrop,
                Quaternion.LookRotation(flat, Vector3.up));
            _menuRoot.SetActive(true);
            SuspendPlayerMotion();
            _open = true;
            Debug.Log("ZIPTIDE: PLAYER_MENU open scene=" + SceneManager.GetActiveScene().name);
        }

        private void CloseMenu(string reason)
        {
            _open = false;
            if (_menuRoot != null) _menuRoot.SetActive(false);
            ResumePlayerMotion();
            Debug.Log("ZIPTIDE: PLAYER_MENU close reason=" + reason);
        }

        private void ReturnToShip()
        {
            string current = SceneManager.GetActiveScene().name;
            CloseMenu("return_to_ship");
            if (current == ZiptideConstants.SceneW000)
            {
                Debug.Log("ZIPTIDE: PLAYER_MENU_RETURN already_at_ship=true");
                return;
            }

            Debug.Log("ZIPTIDE: PLAYER_MENU_RETURN dest=" + ZiptideConstants.SceneW000);
            TravelCoordinator.TravelTo(ZiptideConstants.SceneW000);
        }

        private void SuspendPlayerMotion()
        {
            _suspended.Clear();
            Collect(GetComponentsInChildren<ActionBasedContinuousMoveProvider>(true));
            Collect(GetComponentsInChildren<ActionBasedContinuousTurnProvider>(true));
            Collect(GetComponentsInChildren<ActionBasedSnapTurnProvider>(true));

            DashLocomotion dash = GetComponent<DashLocomotion>();
            if (dash != null && dash.enabled)
            {
                dash.enabled = false;
                _suspended.Add(dash);
            }
        }

        private void Collect(Behaviour[] behaviours)
        {
            if (behaviours == null) return;
            for (int i = 0; i < behaviours.Length; i++)
            {
                Behaviour behaviour = behaviours[i];
                if (behaviour == null || !behaviour.enabled) continue;
                behaviour.enabled = false;
                _suspended.Add(behaviour);
            }
        }

        private void ResumePlayerMotion()
        {
            for (int i = 0; i < _suspended.Count; i++)
            {
                Behaviour behaviour = _suspended[i];
                if (behaviour != null) behaviour.enabled = true;
            }
            _suspended.Clear();
        }

        private void BuildMenuIfNeeded()
        {
            if (_menuRoot != null) return;

            _menuRoot = new GameObject("__PLAYER_FIELD_MENU");
            DontDestroyOnLoad(_menuRoot);

            GameObject board = GameObject.CreatePrimitive(PrimitiveType.Cube);
            board.name = "FieldMenuBoard";
            board.transform.SetParent(_menuRoot.transform, false);
            board.transform.localScale = new Vector3(1.05f, 0.62f, 0.05f);
            Paint(board, new Color(0.045f, 0.07f, 0.10f));
            StripCollider(board);

            AddLabel(board.transform, "FIELD MENU\nY TO RESUME", new Vector3(0f, 0.20f, -0.56f), 0.018f);
            AddTile("RESUME", new Vector3(-0.28f, -0.12f, -0.16f),
                new Color(0.20f, 0.68f, 0.52f), () => CloseMenu("resume_tile"));
            AddTile("RETURN TO SHIP", new Vector3(0.28f, -0.12f, -0.16f),
                new Color(0.78f, 0.48f, 0.18f), ReturnToShip);

            _menuRoot.SetActive(false);
        }

        private void AddTile(string text, Vector3 localPosition, Color color, System.Action selected)
        {
            GameObject tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tile.name = "Tile_" + text.Replace(' ', '_');
            tile.transform.SetParent(_menuRoot.transform, false);
            tile.transform.localPosition = localPosition;
            tile.transform.localScale = new Vector3(0.25f, 0.15f, 0.08f);
            Paint(tile, color);

            XRSimpleInteractable interactable = tile.AddComponent<XRSimpleInteractable>();
            XRInteractionManager manager = FindObjectOfType<XRInteractionManager>();
            if (manager != null) interactable.interactionManager = manager;
            interactable.selectEntered.AddListener(_ => selected());

            AddLabel(tile.transform, text, new Vector3(0f, 0f, -0.56f), 0.018f);
        }

        private static void AddLabel(Transform parent, string text, Vector3 localPosition, float characterSize)
        {
            GameObject label = new GameObject("Label");
            label.transform.SetParent(parent, false);
            label.transform.localPosition = localPosition;
            TextMesh mesh = label.AddComponent<TextMesh>();
            mesh.text = text;
            mesh.characterSize = characterSize;
            mesh.fontSize = 64;
            mesh.anchor = TextAnchor.MiddleCenter;
            mesh.alignment = TextAlignment.Center;
            mesh.color = new Color(0.94f, 0.96f, 1f);
        }

        private static void StripCollider(GameObject go)
        {
            Collider collider = go.GetComponent<Collider>();
            if (collider != null) Destroy(collider);
        }

        private static void Paint(GameObject go, Color color)
        {
            Renderer renderer = go.GetComponent<Renderer>();
            if (renderer == null) return;
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) return;
            Material material = new Material(shader);
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            else if (material.HasProperty("_Color")) material.SetColor("_Color", color);
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }
}
