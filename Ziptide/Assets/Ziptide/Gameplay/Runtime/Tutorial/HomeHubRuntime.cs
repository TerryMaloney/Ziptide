using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    public enum HomeHubChoice
    {
        NewGame,
        Continue,
        Settings
    }

    /// <summary>
    /// Pure choice gate for the cold-boot surface. Settings never travels. New/Continue may request
    /// travel exactly once, and Continue is rejected when no valid persisted profile exists.
    /// </summary>
    public sealed class HomeHubFlowState
    {
        public bool CanContinue { get; }
        public bool TravelCommitted { get; private set; }

        public HomeHubFlowState(bool canContinue)
        {
            CanContinue = canContinue;
        }

        public bool TryChoose(HomeHubChoice choice, out bool shouldTravel)
        {
            shouldTravel = false;
            if (TravelCommitted) return false;
            if (choice == HomeHubChoice.Continue && !CanContinue) return false;

            if (choice == HomeHubChoice.Settings) return true;

            TravelCommitted = true;
            shouldTravel = true;
            return true;
        }
    }

    /// <summary>
    /// Minimal diegetic cold-boot Home Hub. It presents New Game / Continue / Settings and delegates
    /// all persistence to SaveSystem and all scene change to the callback supplied by BootLoader.
    /// It owns no save file, profile serializer, scene loader, input map or menu framework.
    /// BOARD_PROBE remains on this actual Golden surface: hover/select plus a one-second ray census
    /// records hit path, layer, interaction-manager binding and readable-face alignment.
    /// </summary>
    public sealed class HomeHubRuntime : MonoBehaviour
    {
        private const int ManagerFastPollFrames = 300;
        private const float ManagerSteadyPollSeconds = 0.25f;
        private const float ProbeIntervalSeconds = 1f;

        public static event Action<bool> BootPresentationReady;
        public static event Action<PlayerProfile> NewGameProfileCreated;
        public static event Action<HomeHubChoice> ChoiceSelected;
        public static event Action SettingsRequested;

        private string _targetScene;
        private Action<string> _travel;
        private HomeHubFlowState _flow;
        private ComfortConsoleRuntime _settingsConsole;
        private bool _configured;
        private float _nextProbeAt;

        public void Configure(string targetScene, Action<string> travel)
        {
            _targetScene = targetScene;
            _travel = travel;
            _configured = true;
        }

        private void Start()
        {
            if (!_configured)
            {
                Debug.LogWarning("ZIPTIDE: HOME_HUB_MISSING_CONFIG");
                return;
            }

            bool canContinue = SaveSystem.HasExistingProfile;
            _flow = new HomeHubFlowState(canContinue);
            BuildSurface(canContinue);
            _nextProbeAt = 0f;
            Debug.Log("ZIPTIDE: HOME_HUB_READY continue=" + canContinue.ToString().ToLowerInvariant());
            PublishSafely(BootPresentationReady, canContinue, "boot_ready");
        }

        private void Update()
        {
            if (!_configured || _flow == null || Time.unscaledTime < _nextProbeAt) return;
            _nextProbeAt = Time.unscaledTime + ProbeIntervalSeconds;
            LogAimProbe();
        }

        private void Choose(HomeHubChoice choice)
        {
            if (_flow == null) return;

            // Persistence prerequisites are checked before committing the one-shot travel state.
            if ((choice == HomeHubChoice.NewGame || choice == HomeHubChoice.Continue) &&
                SaveSystem.Instance == null)
            {
                Debug.LogWarning("ZIPTIDE: HOME_HUB_CHOICE_BLOCKED reason=save_unavailable");
                return;
            }

            if (!_flow.TryChoose(choice, out bool shouldTravel)) return;

            string choiceName = choice == HomeHubChoice.NewGame ? "new" :
                                choice == HomeHubChoice.Continue ? "continue" : "settings";
            Debug.Log("ZIPTIDE: HOME_HUB_CHOICE choice=" + choiceName);
            PublishSafely(ChoiceSelected, choice, "choice");

            if (choice == HomeHubChoice.Settings)
            {
                ShowSettingsConsole();
                PublishSafely(SettingsRequested, "settings");
                return;
            }

            if (choice == HomeHubChoice.NewGame)
            {
                PlayerProfile profile = SaveSystem.Instance.StartNewProfile();
                PublishSafely(NewGameProfileCreated, profile, "new_profile");
            }
            else
            {
                // Load remains SaveSystem's sole profile recovery/migration path.
                SaveSystem.Instance.Load();
            }

            if (shouldTravel)
            {
                if (_travel != null) _travel(_targetScene);
                else Debug.LogWarning("ZIPTIDE: HOME_HUB_TRAVEL_MISSING dest=" + _targetScene);
            }
        }

        private void ShowSettingsConsole()
        {
            if (_settingsConsole != null) return;
            var go = new GameObject("__HOME_HUB_COMFORT_SETTINGS");
            go.transform.SetParent(transform, false);
            go.transform.localPosition = new Vector3(0f, -1.05f, 0.15f);
            _settingsConsole = go.AddComponent<ComfortConsoleRuntime>();
            _settingsConsole.Configure(false); // boot settings do not complete the W000 tutorial beat
        }

        private void BuildSurface(bool canContinue)
        {
            Transform cam = Camera.main != null ? Camera.main.transform : null;
            Vector3 origin = cam != null
                ? cam.position + cam.forward * 2.2f + Vector3.down * 0.15f
                : new Vector3(0f, 1.5f, 2.2f);
            Quaternion facing = cam != null
                ? Quaternion.LookRotation(origin - cam.position, Vector3.up)
                : Quaternion.identity;

            transform.position = origin;
            transform.rotation = facing;

            var board = GameObject.CreatePrimitive(PrimitiveType.Cube);
            board.name = "__HOME_HUB_BOARD";
            board.transform.SetParent(transform, false);
            board.transform.localScale = new Vector3(2.4f, 1.25f, 0.08f);
            Paint(board, new Color(0.06f, 0.09f, 0.13f));
            StripCollider(board);

            // The old 0.052 character size produced an 8.75 m title on a 2.4 m board and cropped
            // most of both lines in the actual tracked-head capture. This size keeps the same wording,
            // anchor and board layout while fitting the measured title bounds inside the panel.
            AddLabel(board.transform, "ZIPTIDE\nCHOOSE YOUR START", new Vector3(0f, 0.27f, -0.56f), 0.0125f);

            if (canContinue)
            {
                AddTile("NEW GAME", new Vector3(-0.72f, -0.22f, -0.62f),
                    new Color(0.18f, 0.62f, 0.78f), () => Choose(HomeHubChoice.NewGame));
                AddTile("CONTINUE", new Vector3(0f, -0.22f, -0.62f),
                    new Color(0.25f, 0.72f, 0.48f), () => Choose(HomeHubChoice.Continue));
                AddTile("SETTINGS", new Vector3(0.72f, -0.22f, -0.62f),
                    new Color(0.72f, 0.48f, 0.18f), () => Choose(HomeHubChoice.Settings));
            }
            else
            {
                AddTile("NEW GAME", new Vector3(-0.42f, -0.22f, -0.62f),
                    new Color(0.18f, 0.62f, 0.78f), () => Choose(HomeHubChoice.NewGame));
                AddTile("SETTINGS", new Vector3(0.42f, -0.22f, -0.62f),
                    new Color(0.72f, 0.48f, 0.18f), () => Choose(HomeHubChoice.Settings));
            }
        }

        private void AddTile(string text, Vector3 localPosition, Color color, Action selected)
        {
            var tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tile.name = "Tile_" + text.Replace(' ', '_');
            tile.transform.SetParent(transform, false);
            tile.transform.localPosition = localPosition;
            tile.transform.localScale = new Vector3(0.58f, 0.28f, 0.12f);
            Paint(tile, color);

            var interactable = tile.AddComponent<XRSimpleInteractable>();
            XRInteractionManager manager = interactable.interactionManager;
            if (manager == null) manager = FindObjectOfType<XRInteractionManager>();

            bool initiallyBound = manager != null;
            if (initiallyBound)
            {
                if (interactable.interactionManager != manager)
                    interactable.interactionManager = manager;
                Debug.Log("ZIPTIDE: HOME_HUB_TILE_BOUND tile=" + tile.name +
                          " mode=immediate manager=" + manager.name);
            }

            // XRI may provide an early manager during interactable OnEnable, while
            // PlayerRigPersistence can adopt another manager and destroy the early one later in
            // startup. Monitor the tile for its lifetime so either a missing manager or a replaced
            // manager is repaired without rebuilding the Home Hub.
            StartCoroutine(MaintainManagerBinding(interactable, tile.name, initiallyBound));
            interactable.hoverEntered.AddListener(_ => LogTileProbe("hover_enter", tile, interactable));
            interactable.hoverExited.AddListener(_ => LogTileProbe("hover_exit", tile, interactable));
            interactable.selectEntered.AddListener(_ =>
            {
                LogTileProbe("select", tile, interactable);
                selected();
            });

            AddLabel(tile.transform, text, new Vector3(0f, 0f, -0.56f), 0.025f);
        }

        private IEnumerator MaintainManagerBinding(
            XRSimpleInteractable interactable,
            string tileName,
            bool everBound)
        {
            int frame = 0;
            bool timeoutLogged = false;
            var steadyPoll = new WaitForSecondsRealtime(ManagerSteadyPollSeconds);

            while (interactable != null)
            {
                if (frame < ManagerFastPollFrames) yield return null;
                else yield return steadyPoll;
                frame++;

                if (interactable == null) yield break;

                XRInteractionManager manager = FindObjectOfType<XRInteractionManager>();
                if (manager != null && interactable.interactionManager != manager)
                {
                    string mode = everBound ? "rebound" : "delayed";
                    interactable.interactionManager = manager;
                    everBound = true;
                    Debug.Log("ZIPTIDE: HOME_HUB_TILE_BOUND tile=" + tileName +
                              " mode=" + mode + " frames=" + frame +
                              " manager=" + manager.name);
                }
                else if (manager != null)
                {
                    everBound = true;
                }

                if (!everBound && !timeoutLogged && frame >= ManagerFastPollFrames)
                {
                    timeoutLogged = true;
                    Debug.LogWarning("ZIPTIDE: HOME_HUB_TILE_BIND_TIMEOUT tile=" + tileName +
                                     " frames=" + ManagerFastPollFrames +
                                     " monitoring=continued");
                }
            }
        }

        private void LogTileProbe(string phase, GameObject tile, XRSimpleInteractable interactable)
        {
            int managerId = interactable != null && interactable.interactionManager != null
                ? interactable.interactionManager.GetInstanceID()
                : 0;
            Camera cam = Camera.main;
            float facingDot = cam != null ? ReadableFacingDot(cam.transform.position) : 0f;
            Debug.Log("ZIPTIDE: BOARD_PROBE surface=HomeHub phase=" + phase
                + " tile=" + GetHierarchyPath(tile != null ? tile.transform : null)
                + " layer=" + (tile != null ? tile.layer : -1)
                + " manager=" + managerId
                + " facingDot=" + facingDot.ToString("F3"));
        }

        private void LogAimProbe()
        {
            XRRayInteractor[] rays = FindObjectsOfType<XRRayInteractor>();
            int active = 0;
            for (int i = 0; i < rays.Length; i++)
            {
                XRRayInteractor ray = rays[i];
                if (ray == null || !ray.isActiveAndEnabled || !ray.gameObject.activeInHierarchy)
                    continue;

                active++;
                bool hasHit = ray.TryGetCurrent3DRaycastHit(out RaycastHit hit);
                string hitPath = hasHit && hit.transform != null
                    ? GetHierarchyPath(hit.transform)
                    : "none";
                int hitLayer = hasHit && hit.collider != null ? hit.collider.gameObject.layer : -1;
                int managerId = ray.interactionManager != null
                    ? ray.interactionManager.GetInstanceID()
                    : 0;

                Debug.Log("ZIPTIDE: BOARD_PROBE surface=HomeHub phase=aim"
                    + " ray=" + GetHierarchyPath(ray.transform)
                    + " hit=" + hitPath
                    + " layer=" + hitLayer
                    + " manager=" + managerId
                    + " facingDot=" + ReadableFacingDot(ray.transform.position).ToString("F3"));
            }

            if (active == 0)
            {
                Debug.Log("ZIPTIDE: BOARD_PROBE surface=HomeHub phase=aim"
                    + " ray=none hit=none layer=-1 manager=0"
                    + " facingDot=0.000");
            }
        }

        private float ReadableFacingDot(Vector3 viewerWorldPosition)
        {
            Vector3 towardViewer = viewerWorldPosition - transform.position;
            if (towardViewer.sqrMagnitude < 0.0001f) return 0f;
            // TextMesh reads from local -Z; the Home Hub transform +Z points away from the viewer.
            return Vector3.Dot(-transform.forward, towardViewer.normalized);
        }

        private static string GetHierarchyPath(Transform value)
        {
            if (value == null) return "none";
            string path = value.name;
            Transform parent = value.parent;
            while (parent != null)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }
            return path;
        }

        private static void AddLabel(Transform parent, string text, Vector3 localPosition, float size)
        {
            var go = new GameObject("Label_" + text.Replace(' ', '_').Replace('\n', '_'));
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
            var tm = go.AddComponent<TextMesh>();
            tm.text = text;
            tm.characterSize = size;
            tm.fontSize = 64;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.color = new Color(0.92f, 0.96f, 1f);
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

        private static void PublishSafely(Action subscribers, string phase)
        {
            if (subscribers == null) return;
            foreach (Action subscriber in subscribers.GetInvocationList())
            {
                try { subscriber(); }
                catch (Exception ex)
                {
                    Debug.LogWarning("ZIPTIDE: HOME_HUB_SUBSCRIBER_FAIL phase=" + phase + " reason=" + ex.Message);
                }
            }
        }

        private static void PublishSafely<T>(Action<T> subscribers, T value, string phase)
        {
            if (subscribers == null) return;
            foreach (Action<T> subscriber in subscribers.GetInvocationList())
            {
                try { subscriber(value); }
                catch (Exception ex)
                {
                    Debug.LogWarning("ZIPTIDE: HOME_HUB_SUBSCRIBER_FAIL phase=" + phase + " reason=" + ex.Message);
                }
            }
        }
    }
}
