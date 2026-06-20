using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// In-world doorway that loads a scene by WorldPackDefinition.sceneName.
    /// Builds a visible door frame + interactable door per destination at runtime.
    /// </summary>
    public class WorldTravelStation : MonoBehaviour
    {
        [Tooltip("Destination packs (scene names loaded from pack.sceneName). One door per entry.")]
        [SerializeField] private List<WorldPackDefinition> destinationPacks = new List<WorldPackDefinition>();

        private const float DoorWidth = 1.0f;
        private const float DoorHeight = 2.2f;
        private const float FrameThickness = 0.12f;
        private const float DoorDepth = 0.06f;
        private const float DoorSpacing = 1.6f;

        private static readonly Color FrameColor = new Color(0.15f, 0.15f, 0.18f);
        private static readonly Color DoorColor = new Color(0.12f, 0.42f, 0.52f);
        private static readonly Color DoorHoverColor = new Color(0.22f, 0.60f, 0.72f);
        private static readonly Color LabelColor = Color.white;

        private readonly List<GameObject> _owned = new List<GameObject>();

        private void Start()
        {
            BuildStation();
        }

        // #region agent log
        private static void WLog(string msg, string data)
        {
            // Disabled: this previously wrote a JSON line to persistentDataPath on every door build,
            // which accumulated junk on the headset. Kept as a no-op so call sites still compile.
        }
        // #endregion agent log

        private void BuildStation()
        {
            foreach (var b in _owned)
            {
                if (b != null) Destroy(b);
            }
            _owned.Clear();

            if (destinationPacks == null || destinationPacks.Count == 0)
            {
                // #region agent log
                WLog("BuildStation_NoPacks", "'scene':'" + gameObject.scene.name + "'");
                // #endregion agent log
                return;
            }

            float startX = -(destinationPacks.Count - 1) * DoorSpacing * 0.5f;

            for (int i = 0; i < destinationPacks.Count; i++)
            {
                var pack = destinationPacks[i];
                if (pack == null) continue;
                CreateDoorway(pack, new Vector3(startX + i * DoorSpacing, 0f, 0f));
            }
        }

        private void CreateDoorway(WorldPackDefinition pack, Vector3 localPos)
        {
            string sceneName = pack.sceneName;
            string label = FormatLabel(pack);
            // #region agent log
            WLog("CreateDoorway", "'sceneName':'" + sceneName + "','label':'" + label + "','stationScene':'" + gameObject.scene.name + "','pos':'" + transform.position + "'");
            // #endregion agent log

            var doorRoot = new GameObject("TravelDoor_" + (pack.packId ?? "?"));
            doorRoot.transform.SetParent(transform, false);
            doorRoot.transform.localPosition = localPos;
            doorRoot.transform.localRotation = Quaternion.identity;
            doorRoot.transform.localScale = Vector3.one;

            float frameW = DoorWidth + FrameThickness * 2f;
            float frameH = DoorHeight + FrameThickness;

            var frameLeft = CreatePrimitiveCube("FrameLeft", doorRoot.transform,
                new Vector3(-(DoorWidth * 0.5f + FrameThickness * 0.5f), DoorHeight * 0.5f, 0f),
                new Vector3(FrameThickness, DoorHeight, FrameThickness), FrameColor, false);

            var frameRight = CreatePrimitiveCube("FrameRight", doorRoot.transform,
                new Vector3(DoorWidth * 0.5f + FrameThickness * 0.5f, DoorHeight * 0.5f, 0f),
                new Vector3(FrameThickness, DoorHeight, FrameThickness), FrameColor, false);

            var frameTop = CreatePrimitiveCube("FrameTop", doorRoot.transform,
                new Vector3(0f, DoorHeight + FrameThickness * 0.5f, 0f),
                new Vector3(frameW, FrameThickness, FrameThickness), FrameColor, false);

            var door = new GameObject("Door");
            door.transform.SetParent(doorRoot.transform, false);
            door.transform.localPosition = new Vector3(0f, DoorHeight * 0.5f, 0f);
            door.transform.localRotation = Quaternion.identity;
            door.transform.localScale = Vector3.one;

            var doorCol = door.AddComponent<BoxCollider>();
            doorCol.size = new Vector3(DoorWidth, DoorHeight, DoorDepth + 0.15f);

            var interactable = door.AddComponent<XRSimpleInteractable>();

            var mgr = Object.FindObjectOfType<XRInteractionManager>();
            if (mgr != null)
            {
                interactable.interactionManager = mgr;
            }
            else
            {
                Debug.LogWarning("ZIPTIDE: DOOR_NO_MANAGER at build time, will retry");
                StartCoroutine(RetryManagerAssignment(interactable));
            }

            interactable.selectEntered.AddListener(_ =>
            {
                // #region agent log
                WLog("Door_selectEntered", "'sceneName':'" + (sceneName ?? "NULL") + "'");
                // #endregion agent log
                LoadScene(sceneName);
            });

            var doorVisual = CreatePrimitiveCube("DoorVisual", door.transform,
                Vector3.zero, new Vector3(DoorWidth, DoorHeight, DoorDepth), DoorColor, false);

            var visual = doorVisual.GetComponent<Renderer>();
            interactable.hoverEntered.AddListener(_ =>
            {
                TintRenderer(visual, DoorHoverColor);
                // #region agent log
                WLog("Door_hoverEntered", "'sceneName':'" + (sceneName ?? "NULL") + "'");
                // #endregion agent log
            });
            interactable.hoverExited.AddListener(_ => TintRenderer(visual, DoorColor));

            var labelGo = CreateTextMesh("To " + label, 0.06f, LabelColor);
            labelGo.transform.SetParent(doorRoot.transform, false);
            labelGo.transform.localPosition = new Vector3(0f, DoorHeight + FrameThickness + 0.15f, 0f);
            NeutralizeScale(labelGo.transform);
            _owned.Add(labelGo);

            var doorLabel = CreateTextMesh(label, 0.05f, new Color(0.85f, 0.9f, 1f));
            doorLabel.transform.SetParent(door.transform, false);
            doorLabel.transform.localPosition = new Vector3(0f, 0.2f, -(DoorDepth * 0.5f + 0.005f));
            NeutralizeScale(doorLabel.transform);
            _owned.Add(doorLabel);

            _owned.Add(doorRoot);
        }

        private static string FormatLabel(WorldPackDefinition pack)
        {
            if (!string.IsNullOrEmpty(pack.displayName))
                return pack.displayName;
            if (string.IsNullOrEmpty(pack.packId)) return "TRAVEL";
            return pack.packId.Replace('_', ' ').ToUpperInvariant();
        }

        private GameObject CreatePrimitiveCube(string name, Transform parent, Vector3 localPos, Vector3 scale, Color color, bool colliderEnabled)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.transform.SetParent(parent, false);
            cube.transform.localPosition = localPos;
            cube.transform.localScale = scale;
            cube.transform.localRotation = Quaternion.identity;
            cube.GetComponent<Collider>().enabled = colliderEnabled;
            ApplyColor(cube, color);
            _owned.Add(cube);
            return cube;
        }

        // Legacy TextMesh inherits the parent's (often non-uniform) world scale → letters stretch and
        // stack into the "random vertical letters" Terry saw on the toxic-city doors. Counter the
        // parent's lossy scale so the text renders square regardless of how the door cube is scaled.
        private static void NeutralizeScale(Transform t)
        {
            var ls = t.parent != null ? t.parent.lossyScale : Vector3.one;
            t.localScale = new Vector3(
                Mathf.Approximately(ls.x, 0f) ? 1f : 1f / ls.x,
                Mathf.Approximately(ls.y, 0f) ? 1f : 1f / ls.y,
                Mathf.Approximately(ls.z, 0f) ? 1f : 1f / ls.z);
        }

        private static GameObject CreateTextMesh(string text, float charSize, Color color)
        {
            var go = new GameObject("Label");
            var tm = go.AddComponent<TextMesh>();
            tm.text = text;
            tm.characterSize = charSize;
            tm.fontSize = 48;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.color = color;
            return go;
        }

        private static void ApplyColor(GameObject go, Color color)
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

        private static void TintRenderer(Renderer r, Color color)
        {
            if (r == null || r.material == null) return;
            if (r.material.HasProperty("_BaseColor")) r.material.SetColor("_BaseColor", color);
            else if (r.material.HasProperty("_Color")) r.material.SetColor("_Color", color);
        }

        private IEnumerator RetryManagerAssignment(XRSimpleInteractable interactable)
        {
            for (int i = 0; i < 10; i++)
            {
                yield return null;
                if (interactable == null) yield break;
                var mgr = Object.FindObjectOfType<XRInteractionManager>();
                if (mgr != null)
                {
                    interactable.interactionManager = mgr;
                    Debug.Log("ZIPTIDE: DOOR_MANAGER_RETRY_OK frame=" + i);
                    yield break;
                }
            }
            Debug.LogWarning("ZIPTIDE: DOOR_MANAGER_RETRY_FAIL after 10 frames");
        }

        private static void LoadScene(string sceneName)
        {
            // #region agent log
            WLog("LoadScene_called", "'sceneName':'" + (sceneName ?? "NULL") + "','activeScene':'" + SceneManager.GetActiveScene().name + "'");
            // #endregion agent log
            if (string.IsNullOrEmpty(sceneName)) return;
            // TravelCoordinator handles SaveBeforeTravel + XRI-ready gate + RestoreAfterTravel.
            TravelCoordinator.TravelTo(sceneName);
        }
    }
}
