using System;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// PUNCH IT — the W000 cast-off beat. The launch control now sits on the boardable cockpit deck,
    /// so the player repairs the coupler, boards the ship, and launches from aboard instead of pressing
    /// a flight command while standing outside on the berth. Pressing it plays a rails take-off:
    /// star-streak lines rush past for a few seconds (the world moves, the rig NEVER parents to the hull —
    /// SPACEFLIGHT_PHYSICS law), then TravelCoordinator carries you to the target scene. Free-flight
    /// replaces the rails later; this ships the fantasy today.
    /// ARMING GATE: launch is blocked until the tutorial's coupler machine is repaired. The pure rule is
    /// <see cref="CastOffArming"/> — a missing machine never strands the launch. Blocked presses flash the
    /// button label as the hint. Logs FLIGHT_LAUNCH / FLIGHT_STREAKS / FLIGHT_DEPART / FLIGHT_BLOCKED.
    /// </summary>
    public class ShipCastOffRuntime : MonoBehaviour
    {
        private const string ArmedLabel = "PUNCH IT";
        private const float HintSeconds = 2.5f;

        // THE FIRST LAUNCH IS NOT A ZIPTIDE. Canon (FIRST_HOUR_DIRECTORS_CUT §5, minute 10-13):
        // "helm -> PUNCH IT -> cast-off rails (ship flight, NO gate FX)". Cal is flying to a routine
        // wreck-clearance job, not crossing the network. This used to point straight at ToxicCity AND
        // fire the full gate spectacle, which spent the game's biggest moment on a bus ride and left
        // nothing for the beat designed to earn it -- the key transit at minute 45.
        [SerializeField] private string targetScene = ZiptideConstants.SceneSpaceLane;

        [Tooltip("Suppress the Ziptide gate effect on this launch. TRUE for the ordinary cast-off; the "
                 + "gate belongs to the key transit alone.")]
        [SerializeField] private bool suppressGateEffect = true;
        [SerializeField] private float streakSeconds = 6f;
        [Tooltip("RepairableMachine id that must be RUNNING before PUNCH IT arms (empty = no gate).")]
        [SerializeField] private string armingMachineId = "gate_coupler";

        private bool _launching;
        private RepairableMachine _armingMachine; // cached once found; absence is re-checked per press
        private TextMesh _buttonLabel;
        private Coroutine _hintRoutine;

        /// <summary>
        /// Author entry point (public Init idiom — the no-reflection law). The generating patcher owns
        /// the destination, so a scene regenerated from data always carries the current route rather
        /// than whatever string was serialized into the .unity file months ago.
        /// </summary>
        public void Configure(string destinationScene, bool suppressGate)
        {
            if (!string.IsNullOrEmpty(destinationScene)) targetScene = destinationScene;
            suppressGateEffect = suppressGate;
        }

        /// <summary>The destination currently consumed by the existing PUNCH IT launch sequence.</summary>
        public string SelectedDestination => targetScene;

        /// <summary>Neutral selection notification; it does not launch or travel.</summary>
        public event Action<string> DestinationSelected;

        /// <summary>
        /// The first-hour helm selects the OUTBOUND SALVAGE LEG. ToxicCity stays accepted because the
        /// return trip and any recovery/dev route still name it, and rejecting it would strand a build
        /// whose lane scene has not been generated yet. Anything else is refused: this is the tutorial's
        /// one-way launch, not a free destination picker.
        ///
        /// TryLaunch and LaunchSequence remain the sole launch/travel owners.
        /// </summary>
        public bool SelectFirstDestination(string destinationScene)
        {
            bool allowed =
                string.Equals(destinationScene, ZiptideConstants.SceneSpaceLane, StringComparison.Ordinal)
                || string.Equals(destinationScene, ZiptideConstants.SceneToxicCity, StringComparison.Ordinal);
            if (!allowed)
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
            // The boardable ship builds CockpitDeck during Awake. Start runs after every Awake, so the
            // console can be placed on that walkable deck. Keep the old grounded berth position only as
            // a fail-safe for malformed/non-boardable test ships.
            Transform deck = transform.Find("CockpitDeck");
            Vector3 pos;
            Vector3 faceTarget;
            if (deck != null)
            {
                pos = deck.position - transform.right * 1.05f - transform.forward * 0.15f;
                var deckCollider = deck.GetComponent<Collider>();
                pos.y = deckCollider != null ? deckCollider.bounds.max.y : deck.position.y + 0.1f;
                faceTarget = deck.position + transform.forward * 0.35f;
                Debug.Log("ZIPTIDE: CASTOFF_CONSOLE_LOCATION mode=cockpit");
            }
            else
            {
                pos = transform.position - transform.right * 4f - transform.forward * 3f;
                if (Physics.Raycast(pos + Vector3.up * 3f, Vector3.down, out var hit, 10f))
                    pos.y = hit.point.y;
                faceTarget = transform.position;
                Debug.LogWarning("ZIPTIDE: CASTOFF_CONSOLE_LOCATION mode=berth_fallback reason=no_cockpit_deck");
            }

            var pedestal = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pedestal.name = "CastOffConsole";
            pedestal.transform.position = pos + Vector3.up * 0.55f;
            Vector3 face = faceTarget - pos;
            face.y = 0f;
            pedestal.transform.rotation = face.sqrMagnitude > 0.001f
                ? Quaternion.LookRotation(face.normalized, Vector3.up)
                : transform.rotation;
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

        private bool _lastArmedLogged = true; // logs the first evaluation too (starts opposite-able)
        private bool _armedLogPrimed;

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
            bool armed = CastOffArming.IsArmed(gateConfigured, _armingMachine != null,
                _armingMachine != null && _armingMachine.IsRepaired);

            // DS-10 evidence (log-only, transitions only): WHICH machine instance the cast-off
            // observes and its repaired state — divergence from the JobDirector-spawned machine
            // (a duplicate) would show here as mismatched instance ids.
            if (!_armedLogPrimed || armed != _lastArmedLogged)
            {
                _armedLogPrimed = true;
                _lastArmedLogged = armed;
                Debug.Log("ZIPTIDE: REPAIR_TRACE hop=castoff armed=" + armed
                    + " gate=" + (gateConfigured ? armingMachineId : "none")
                    + " machineInstance=" + (_armingMachine != null ? _armingMachine.GetInstanceID().ToString() : "NONE")
                    + " repaired=" + (_armingMachine != null && _armingMachine.IsRepaired));
            }
            return armed;
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
            // THE ATMOSPHERE VEIL brackets the cut: the burn builds, the scene swaps at its peak,
            // and the destination's ReentryArrivalRuntime picks the same fire up on the far side —
            // so leaving a planet reads as leaving a planet instead of a load. The lead is bounded
            // by AtmosphereVeilCore, and the veil self-destructs at its hard cap, so this can never
            // hold travel: worst case the burn is invisible and the flight departs on schedule.
            float lead = AtmosphereVeilEffect.Play(VeilLeg.Ascent);
            yield return new WaitForSeconds(lead);

            Debug.Log("ZIPTIDE: FLIGHT_DEPART target=" + targetScene
                + " gate=" + (suppressGateEffect ? "suppressed" : "full"));
            TravelCoordinator.TravelTo(targetScene, skipGate: suppressGateEffect);
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
