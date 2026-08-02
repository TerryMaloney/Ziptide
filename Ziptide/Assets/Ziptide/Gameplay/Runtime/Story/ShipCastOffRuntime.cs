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

        /// <summary>Matches the rest of the ship's console signage (~7.7 cm a line at fontSize 48).</summary>
        private const float LabelCharacterSize = 0.016f;
        private const float HintCharacterSize = 0.011f;

        /// <summary>Console-local pose of the launch tile. The helm tile docks one row below it.</summary>
        public static readonly Vector3 ButtonLocalPos = new Vector3(0f, 1.0f, -0.22f);

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

        [Tooltip("TRUE for the berth the first hour ends at: this hull cannot leave until the joined "
                 + "key is seated in its socket, because the key IS the route.")]
        [SerializeField] private bool requireKeySeated;

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

        /// <summary>
        /// Author entry point for the key-gated berth (the ship the first hour ends on). Separate
        /// from Configure because KeySocketRuntime CALLS Configure when it arms the tide, and a
        /// key gate that Configure could reset would unlock the launch at the exact moment it was
        /// supposed to be the reward.
        /// </summary>
        public void ConfigureKeyGate(bool required, string machineId = null)
        {
            requireKeySeated = required;
            if (machineId != null) armingMachineId = machineId;
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
            RefreshLabel();               // the launch control acknowledges the choice
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
            Vector3 readerStandsAt;
            if (deck != null)
            {
                pos = deck.position - transform.right * 1.05f - transform.forward * 0.15f;
                var deckCollider = deck.GetComponent<Collider>();
                pos.y = deckCollider != null ? deckCollider.bounds.max.y : deck.position.y + 0.1f;
                readerStandsAt = deck.position;   // you read this standing in the middle of the deck
                Debug.Log("ZIPTIDE: CASTOFF_CONSOLE_LOCATION mode=cockpit");
            }
            else
            {
                pos = transform.position - transform.right * 4f - transform.forward * 3f;
                if (Physics.Raycast(pos + Vector3.up * 3f, Vector3.down, out var hit, 10f))
                    pos.y = hit.point.y;
                readerStandsAt = transform.position;
                Debug.LogWarning("ZIPTIDE: CASTOFF_CONSOLE_LOCATION mode=berth_fallback reason=no_cockpit_deck");
            }

            // THE CONSOLE IS AN UNSCALED ROOT. It used to be the scaled pillar cube itself, so every
            // child inherited (0.5, 1.1, 0.35) and the glyphs came out stretched more than 2:1 — the
            // "glitchy looking text" from the 2026-08-01 device pass. Nothing that carries a TextMesh
            // may hang off a non-uniformly scaled parent.
            var console = new GameObject("CastOffConsole");
            console.transform.position = pos;

            // FACING. WorldLabelFacing is the one contract: +Z points AWAY from the reader, so the
            // label reads instead of mirroring, and the button (built on the -Z face) ends up on the
            // side you are standing on instead of round the back where it used to be.
            console.transform.rotation = WorldLabelFacing.FaceViewer(pos, readerStandsAt);
            _consoleAnchor = console.transform;

            var pillar = GameObject.CreatePrimitive(PrimitiveType.Cube);
            pillar.name = "CastOffPillar";
            pillar.transform.SetParent(console.transform, false);
            pillar.transform.localPosition = new Vector3(0f, 0.55f, 0f);
            pillar.transform.localScale = new Vector3(0.5f, 1.1f, 0.35f);
            ItemFactory.ApplyURPColor(pillar, new Color(0.16f, 0.18f, 0.2f));

            var button = GameObject.CreatePrimitive(PrimitiveType.Cube);
            button.name = "Tile_PUNCH_IT";
            button.transform.SetParent(console.transform, false);
            button.transform.localPosition = ButtonLocalPos;
            button.transform.localScale = new Vector3(0.36f, 0.12f, 0.06f);
            ItemFactory.ApplyURPColor(button, new Color(0.85f, 0.25f, 0.15f));

            // The sign sits ABOVE the button on an unscaled parent, and slightly PROUD of the button
            // face so the glyphs are never inside the plate. A TextMesh's world line height is
            // roughly fontSize * characterSize / 10, so these are the numbers used everywhere else
            // in the ship UI (48 / 0.016 ~ 7.7 cm a line) rather than a fresh guess.
            var label = new GameObject("Label_PUNCH_IT");
            label.transform.SetParent(console.transform, false);   // unscaled parent: no glyph stretch
            label.transform.localPosition = new Vector3(0f, 1.17f, -0.27f);
            var tm = label.AddComponent<TextMesh>();
            tm.characterSize = LabelCharacterSize;
            tm.fontSize = 48;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.color = new Color(1f, 0.9f, 0.7f);
            _buttonLabel = tm;
            RefreshLabel();

            var interactable = button.AddComponent<XRSimpleInteractable>();
            var mgr = FindObjectOfType<XRInteractionManager>();
            if (mgr != null) interactable.interactionManager = mgr;
            interactable.selectEntered.AddListener(_ => TryLaunch());

            ObjectiveBeacon.Attach(console, new Color(0.95f, 0.45f, 0.2f), 8f);
        }

        /// <summary>
        /// Where the helm tile docks. The first-destination tile used to be authored 3.4 m off the
        /// PORT FLANK of the hull at hull-centre height, which is why the 2026-08-01 device pass
        /// reported "a random toxic city button on the side of the ship". A destination selector
        /// belongs beside the launch control, on the deck, or it is furniture.
        /// </summary>
        public Transform ConsoleAnchor => _consoleAnchor;

        private Transform _consoleAnchor;

        /// <summary>
        /// The button says where it is going. Selecting a destination and pressing PUNCH IT were two
        /// unconnected acts on device — seven destination selections in the log and not one launch —
        /// because nothing on the launch control ever acknowledged the choice.
        /// </summary>
        private void RefreshLabel()
        {
            if (_buttonLabel == null || _hintRoutine != null) return;
            _buttonLabel.text = ArmedLabel + "\n" + DestinationLabel(targetScene);
            _buttonLabel.characterSize = LabelCharacterSize;
        }

        /// <summary>Scene name to something a pilot would read on a console.</summary>
        public static string DestinationLabel(string scene)
        {
            if (string.Equals(scene, ZiptideConstants.SceneToxicCity, StringComparison.Ordinal))
                return "-> W001 TOXIC CITY";
            if (string.Equals(scene, ZiptideConstants.SceneSpaceLane, StringComparison.Ordinal))
                return "-> SALVAGE LANE";
            if (string.Equals(scene, ZiptideConstants.SceneW002, StringComparison.Ordinal))
                return "-> W002 DRY CISTERN";
            return string.IsNullOrEmpty(scene) ? "-> NO DESTINATION" : "-> " + scene.ToUpperInvariant();
        }

        private void TryLaunch()
        {
            if (_launching) return;
            if (!IsArmed())
            {
                var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
                bool keyMissing = requireKeySeated
                                  && (profile == null || !profile.HasFlag(ZiptideFlags.KEY_SEATED));
                if (keyMissing)
                {
                    // Not a fault to repair — a route the ship does not have yet.
                    Debug.Log("ZIPTIDE: FLIGHT_BLOCKED reason=no_key");
                    ShowHint("NO DESTINATION\nseat the key");
                    return;
                }
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
            var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            bool keySeated = profile != null && profile.HasFlag(ZiptideFlags.KEY_SEATED);
            bool armed = CastOffArming.IsArmed(gateConfigured, _armingMachine != null,
                _armingMachine != null && _armingMachine.IsRepaired,
                requireKeySeated, keySeated);

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
            _buttonLabel.characterSize = HintCharacterSize;
            yield return new WaitForSeconds(HintSeconds);
            _hintRoutine = null;
            RefreshLabel();
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
            // TWO LAUNCHES, ONE EXIT. An ordinary cast-off is a FLIGHT: the atmosphere veil brackets
            // the cut so leaving a planet reads as leaving a planet, and the destination's
            // ReentryArrivalRuntime picks the same burn up on the far side. The keyed transit is not
            // a flight at all — the tide erupts around the hull and takes the ship, anchored at the
            // berth so it pours out of YOUR ship instead of ringing you wherever you stand, and with
            // no veil on purpose: the gate IS the event, and a plasma burn over it would bury the
            // beat this whole hour exists to earn.
            //
            // Both paths leave through the SAME call into travel. A second entry point here is how a
            // parallel travel path gets born, and HomeHubFlowTests pins that there is only one.
            Vector3? gateAnchor = null;
            if (suppressGateEffect)
            {
                // The veil is bounded by AtmosphereVeilCore and self-destructs at its hard cap, so
                // this can never hold travel: worst case the burn is invisible and the flight leaves
                // on schedule.
                float lead = AtmosphereVeilEffect.Play(VeilLeg.Ascent);
                yield return new WaitForSeconds(lead);
            }
            else
            {
                gateAnchor = transform.position;
            }

            Debug.Log("ZIPTIDE: FLIGHT_DEPART target=" + targetScene
                + " gate=" + (suppressGateEffect ? "suppressed" : "full berth=" + transform.position.ToString("F1")));
            TravelCoordinator.TravelTo(targetScene, skipGate: suppressGateEffect, gatePos: gateAnchor);
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
