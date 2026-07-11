using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// PUNCH IT — the W000 cast-off beat (Test Day 1: "i dont see punch it for the ship.. cant
    /// fly yet"). Added to the ship root by the shipyard patcher; builds a console beside the
    /// stern with one big button. Pressing it plays a rails take-off: star-streak lines rush past
    /// for a few seconds (the world moves, the rig NEVER parents to the hull — SPACEFLIGHT_PHYSICS
    /// law), then TravelCoordinator carries you to the target scene. Free-flight (FlightModel,
    /// P4b) replaces the rails later; this ships the fantasy today.
    /// ARMING GATE (the boarded fuel-cell follow-up, PRIORITIES #3): launch is blocked until the
    /// tutorial's coupler machine is repaired. The pure rule is <see cref="CastOffArming"/> — a
    /// missing machine never strands the launch. Blocked presses flash the button label as the hint.
    /// Logs FLIGHT_LAUNCH / FLIGHT_STREAKS / FLIGHT_DEPART / FLIGHT_BLOCKED.
    /// </summary>
    public class ShipCastOffRuntime : MonoBehaviour
    {
        private const string ArmedLabel = "PUNCH IT";
        private const float HintSeconds = 2.5f;

        [SerializeField] private string targetScene = "ToxicCity";
        [SerializeField] private float streakSeconds = 6f;
        [Tooltip("RepairableMachine id that must be RUNNING before PUNCH IT arms (empty = no gate).")]
        [SerializeField] private string armingMachineId = "gate_coupler";

        private bool _launching;
        private RepairableMachine _armingMachine; // cached once found; absence is re-checked per press
        private TextMesh _buttonLabel;
        private Coroutine _hintRoutine;

        /// <summary>The destination currently consumed by the existing PUNCH IT launch sequence.</summary>
        public string SelectedDestination => targetScene;

        /// <summary>Neutral selection notification; it does not launch or travel.</summary>
        public event Action<string> DestinationSelected;

        /// <summary>
        /// The first-hour helm may select only W001/ToxicCity. This changes the existing serialized
        /// destination truth; TryLaunch and LaunchSequence remain the sole launch/travel owners.
        /// </summary>
        public bool SelectFirstDestination(string destinationScene)
        {
            if (!string.Equals(destinationScene, ZiptideConstants.SceneToxicCity, StringComparison.Ordinal))
            {
                Debug.LogWarning("ZIPTIDE: FLIGHT_DESTINATION_REJECTED target=" + destinationScene);
                return false;
            }

            targetScene = destinationScene;
            Debug.Log("ZIPTIDE: FLIGHT_DESTINATION_SELECTED target=" + targetScene);
            PublishDestinationSelected(targetScene);
            return true;
        }

        private void Start()
        {
            BuildConsole();
        }

        private void BuildConsole()
        {
            // Beside the stern, grounded by raycast so it works at any berth height.
            Vector3 pos = transform.position - transform.right * 4f - transform.forward * 3f;
            if (Physics.Raycast(pos + Vector3.up * 3f, Vector3.down, out var hit, 10f))
                pos.y = hit.point.y;

            var pedestal = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pedestal.name = "CastOffConsole";
            pedestal.transform.position = pos + Vector3.up * 0.55f;
            pedestal.transform.rotation = Quaternion.LookRotation(transform.position - pos);
            pedestal.transform.localScale = new Vector3(0.5f, 1.1f, 0.35f);
            ItemFactory.ApplyURPColor(pedestal, new Color(0.16f, 0.18f, 0.2f));

            var button = GameObject.CreatePrimitive(PrimitiveType.Cube);
            button.name = "Tile_PUNCH_IT";
            button.transform.SetParent(pedestal.transform, false);
            button.transform.localPosition = new Vector3(0f, 0.35f, -0.6f);
            button.transform.localScale = new Vector3(0.72f, 0.22f, 0.5f);
            ItemFactory.ApplyURPColor(button, new Color(0.85f, 0.25f, 0.15f));

            var label = new GameObject("Label_PUNCH_IT");
            label.transform.SetParent(button.transform, false);
            label.transform.localPosition = new Vector3(0f, 0f, -0.55f);
            // Neutralize the button's non-uniform scale so glyphs don't stretch.
            label.transform.localScale = new Vector3(1f / 0.72f, 1f / 0.22f, 1f / 0.5f) * 0.35f;
            var tm = label.AddComponent<TextMesh>();
            tm.text = ArmedLabel;
            tm.characterSize = 0.03f;
            tm.fontSize = 64;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.color = new Color(1f, 0.9f, 0.7f);
            _buttonLabel = tm;

            var interactable = button.AddComponent<XRSimpleInteractable>();
            var mgr = FindObjectOfType<XRInteractionManager>();
            if (mgr != null) interactable.interactionManager = mgr;
            interactable.selectEntered.AddListener(_ => TryLaunch());

            ObjectiveBeacon.Attach(pedestal, new Color(0.95f, 0.45f, 0.2f), 8f);
        }

        private void TryLaunch()
        {
            if (_launching) return;
            if (!IsArmed())
            {
                Debug.Log("ZIPTIDE: FLIGHT_BLOCKED reason=unarmed machine=" + armingMachineId);
                ShowHint("COUPLER OFFLINE\nrepair the " + armingMachineId.Replace('_', ' '));
                return;
            }
            _launching = true;
            Debug.Log("ZIPTIDE: FLIGHT_LAUNCH scene=" + gameObject.scene.name + " target=" + targetScene);
            StartCoroutine(LaunchSequence());
        }

        private bool IsArmed()
        {
            bool gateConfigured = !string.IsNullOrEmpty(armingMachineId);
            if (gateConfigured && _armingMachine == null)
            {
                // The machine is spawned at runtime by JobDirector, so keep looking until found —
                // but never cache absence: a truly machine-less scene stays armed (CastOffArming law).
                foreach (var m in FindObjectsOfType<RepairableMachine>())
                    if (m.MachineId == armingMachineId) { _armingMachine = m; break; }
            }
            return CastOffArming.IsArmed(gateConfigured, _armingMachine != null,
                _armingMachine != null && _armingMachine.IsRepaired);
        }

        private void ShowHint(string text)
        {
            if (_buttonLabel == null) return;
            if (_hintRoutine != null) StopCoroutine(_hintRoutine);
            _hintRoutine = StartCoroutine(HintSequence(text));
        }

        private IEnumerator HintSequence(string text)
        {
            _buttonLabel.text = text;
            _buttonLabel.characterSize = 0.018f;
            yield return new WaitForSeconds(HintSeconds);
            _buttonLabel.text = ArmedLabel;
            _buttonLabel.characterSize = 0.03f;
            _hintRoutine = null;
        }

        private IEnumerator LaunchSequence()
        {
            var cam = Camera.main;
            Debug.Log("ZIPTIDE: FLIGHT_STREAKS seconds=" + streakSeconds);
            float t = 0f;
            var rng = new System.Random(12345);
            while (t < streakSeconds)
            {
                t += 0.06f;
                if (cam != null)
                {
                    // Star streaks rushing PAST the player: spawn on a ring around the gaze,
                    // racing backward — speed reads without moving the camera at all.
                    for (int i = 0; i < 3; i++)
                    {
                        float ang = (float)rng.NextDouble() * Mathf.PI * 2f;
                        float r = 1.5f + (float)rng.NextDouble() * 6f;
                        Vector3 side = cam.transform.right * Mathf.Cos(ang) + cam.transform.up * Mathf.Sin(ang);
                        Vector3 from = cam.transform.position + cam.transform.forward * (8f + (float)rng.NextDouble() * 15f) + side * r;
                        float len = 2f + (t / streakSeconds) * 9f; // streaks lengthen as "speed" builds
                        TracerFx.Spawn(from, from - cam.transform.forward * len,
                            new Color(0.8f, 0.9f, 1f, 0.8f), 0.015f, 0.12f);
                    }
                }
                yield return new WaitForSeconds(0.06f);
            }
            Debug.Log("ZIPTIDE: FLIGHT_DEPART target=" + targetScene);
            TravelCoordinator.TravelTo(targetScene);
        }

        private void PublishDestinationSelected(string destination)
        {
            Action<string> subscribers = DestinationSelected;
            if (subscribers == null) return;
            foreach (Action<string> subscriber in subscribers.GetInvocationList())
            {
                try { subscriber(destination); }
                catch (Exception ex)
                {
                    Debug.LogWarning("ZIPTIDE: FLIGHT_DESTINATION_SUBSCRIBER_FAIL reason=" + ex.Message);
                }
            }
        }
    }
}
