using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// S1 of the north star (GAME_PLAN M4; architecture LOCKED in docs/systems/SHIPS.md): the berthed
    /// ship becomes BOARDABLE — a mobile travel station wearing a ship costume. A boarding panel at the
    /// hull door teleports you up to the cockpit deck; the helm lists destination worlds (story-gated
    /// exactly like travel doors via <see cref="WorldGating"/>); selecting one goes through
    /// <see cref="TravelCoordinator.TravelTo"/> — the ONLY legal path (locked contract #1). A disembark
    /// panel puts you back on the berth. The rig is TELEPORTED, never parented (SHIPS.md guardrail).
    /// Fields are SERIALIZED (assigned at edit time by CityBuilder — gotcha #7); geometry builds in
    /// Awake. Logs ZIPTIDE: SHIP_BOARD / SHIP_DISEMBARK / SHIP_DEPART dest=…
    /// The S2 fly-out presentation (engine audio + window starfield) layers onto Depart later.
    /// </summary>
    public class ShipBoardingStation : MonoBehaviour
    {
        [Tooltip("Destination packs (assigned at edit time from the authored world packs).")]
        [SerializeField] private List<WorldPackDefinition> destinationPacks = new List<WorldPackDefinition>();

        [Tooltip("Cockpit deck position, local to the hull root (player stands here after boarding).")]
        [SerializeField] private Vector3 cockpitLocalPos = new Vector3(0f, 2.2f, 2.4f);

        [Tooltip("Boarding panel position, local to the hull root (outside, by the door).")]
        [SerializeField] private Vector3 doorLocalPos = new Vector3(-3.2f, 0.2f, 0f);

        private static readonly Color PanelColor = new Color(0.16f, 0.40f, 0.50f);
        private static readonly Color PanelHot = new Color(0.25f, 0.62f, 0.75f);
        private static readonly Color LockedColor = new Color(0.30f, 0.10f, 0.10f);

        private Vector3 _berthReturnPos;
        private Transform _cockpitDeck;

        /// <summary>Edit-time wiring (CityBuilder) — serialized fields only, no scene work here.</summary>
        public void Configure(List<WorldPackDefinition> packs, Vector3 cockpitPos, Vector3 doorPos)
        {
            destinationPacks = packs ?? new List<WorldPackDefinition>();
            cockpitLocalPos = cockpitPos;
            doorLocalPos = doorPos;
        }

        private void Awake()
        {
            BuildBoardingPanel();
            BuildCockpitDeck();
            BuildQuarters();

            // Ship pillar 2.1/2.2 (2026-07-09): the profile's refit lands on this hull (chassis
            // silhouette + livery + journey decals + nameplate + hum), and the hangar bay stands
            // beside the quarters — select a tile, watch YOUR ship transform live.
            ShipRefit.Apply(gameObject);
            var hangar = new GameObject("HangarBay");
            hangar.transform.SetParent(transform, false);
            hangar.transform.localPosition = cockpitLocalPos + new Vector3(3.4f, 0f, -5.4f);
            hangar.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
            hangar.AddComponent<HangarBayRuntime>().shipRoot = gameObject;
        }

        // ── The Quarters: your locker/customization cabin, aft of the cockpit (QUARTERS.md) ──────

        private void BuildQuarters()
        {
            // The room itself is host-agnostic (QuartersRoom) — the ship just parks one aft and adds
            // the two teleports. The PvP pre-round locker reuses the same component (architect's seam).
            Vector3 roomLocal = cockpitLocalPos + new Vector3(0f, 0f, -5.4f);
            var room = new GameObject("Quarters");
            room.transform.SetParent(transform, false);
            room.transform.localPosition = roomLocal;
            room.AddComponent<QuartersRoom>();

            Vector3 deckCenter = cockpitLocalPos;
            var toQuarters = MakePanel("QuartersPanel",
                transform.TransformPoint(deckCenter + new Vector3(-1.2f, 0.6f, -1.55f)),
                "QUARTERS", new Color(0.30f, 0.22f, 0.40f), () =>
                {
                    if (!StepTo(transform.TransformPoint(roomLocal) + Vector3.up * ShipDeckCore.StandClearance,
                            "quarters")) return;
                    var prof = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
                    if (prof != null) prof.SetFlag("QUARTERS_FIRST_VISIT"); // RILL's line, once per save
                    Debug.Log("ZIPTIDE: QUARTERS_ENTER");
                }, small: true);
            toQuarters.transform.rotation = transform.rotation * Quaternion.Euler(0f, 180f, 0f);

            var backToDeck = MakePanel("QuartersReturn",
                transform.TransformPoint(roomLocal + new Vector3(0f, 1.3f, -2.15f)),
                "RETURN TO DECK", new Color(0.30f, 0.22f, 0.40f), () =>
                {
                    if (TryResolveDeckStand(out Vector3 deckStand, out string deckSource))
                    {
                        TeleportRig(deckStand);
                        Debug.Log("ZIPTIDE: QUARTERS_EXIT source=" + deckSource);
                    }
                    else Debug.LogWarning("ZIPTIDE: QUARTERS_EXIT_ABORT source=" + deckSource);
                }, small: true);
            backToDeck.transform.SetParent(room.transform, true);
            backToDeck.transform.rotation = transform.rotation; // faces you as you enter the doorway
        }

        // ── Boarding ─────────────────────────────────────────────────────────

        private void BuildBoardingPanel()
        {
            var panel = MakePanel("BoardPanel", transform.TransformPoint(doorLocalPos) + Vector3.up * 1.2f,
                "BOARD SHIP", PanelColor, Board);

            // THE MIRRORED LABEL (device pass 2026-08-01: "BOARD SHIP" read backwards). The panel
            // stands on the PORT flank, so the reader approaches from -X, but this took the hull's
            // rotation and pointed +Z down the bow. WorldLabelFacing is the one facing contract:
            // +Z away from the reader. ShipBoardingPresentationGuard re-faces it live from there.
            panel.transform.rotation = WorldLabelFacing.FaceViewer(
                panel.transform.position,
                panel.transform.position - transform.right * 2f);
        }

        /// <summary>
        /// BOARD SHIP. Separated out of the lambda because it now has a failure mode worth naming:
        /// the deck must be PROVEN under your feet before the rig is moved (ShipDeckCore's law).
        /// </summary>
        private void Board()
        {
            if (!TryResolveDeckStand(out Vector3 stand, out string source))
            {
                // Refusing is the correct behaviour. On 2026-08-01 this teleported anyway and the
                // player fell sixty metres into the fall-safety net. A button that does nothing is
                // a bug you can see; a button that deletes you off the map wastes a headset session.
                Debug.LogWarning("ZIPTIDE: SHIP_BOARD_ABORT reason=no_proven_deck source=" + source
                    + " ship=" + gameObject.name);
                return;
            }

            _berthReturnPos = RigPosition() ?? (transform.TransformPoint(doorLocalPos) + Vector3.forward);
            // Re-evaluate story gating EVERY boarding — you may have just finished the contract
            // that unlocks the next world (locks were stale when computed once in Awake).
            RebuildHelmRows();
            TeleportRig(stand);
            // RILL notices your first boarding (its FlagSet line fires once per save).
            var prof = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            if (prof != null) prof.SetFlag("SHIP_FIRST_BOARD");
            Debug.Log("ZIPTIDE: SHIP_BOARD stand=" + stand.ToString("F2") + " source=" + source);
        }

        /// <summary>
        /// Where the player's feet go when they board. Derived from the deck THAT EXISTS — the
        /// collider's own bounds — and then proven with a downward probe, because the serialized
        /// offset and the built deck were free to disagree and did. Falls back to the offset only
        /// when there is no deck collider at all, and even then the probe still has to pass.
        /// </summary>
        private bool TryResolveDeckStand(out Vector3 stand, out string source)
        {
            Vector3 candidate;
            if (_cockpitDeck != null && _cockpitDeck.TryGetComponent(out Collider deckCollider)
                && deckCollider.enabled)
            {
                Bounds b = deckCollider.bounds;
                candidate = new Vector3(b.center.x, ShipDeckCore.StandY(b.max.y), b.center.z);
                source = "deck_collider";
            }
            else
            {
                candidate = transform.TransformPoint(cockpitLocalPos) + Vector3.up * ShipDeckCore.StandClearance;
                source = "serialized_offset";
            }

            return TryProveStand(candidate, ref source, out stand);
        }

        /// <summary>
        /// Shared proof for both ends of the trip. A stand point is only usable if something solid
        /// is within ShipDeckCore.MaxStandDrop below it; if it is, we settle onto that surface
        /// exactly rather than trusting the arithmetic that produced the candidate.
        /// </summary>
        private static bool TryProveStand(Vector3 candidate, ref string source, out Vector3 stand)
        {
            stand = candidate;
            Vector3 from = candidate + Vector3.up * ShipDeckCore.StandProbeLift;
            bool hit = Physics.Raycast(from, Vector3.down, out RaycastHit ground,
                ShipDeckCore.StandProbeLength, Physics.DefaultRaycastLayers,
                QueryTriggerInteraction.Ignore);

            float drop = hit ? candidate.y - ground.point.y : float.MaxValue;
            if (!ShipDeckCore.StandIsProven(hit, drop))
            {
                source += hit ? "+drop" + drop.ToString("F1") : "+no_ground";
                return false;
            }

            stand = new Vector3(candidate.x, ShipDeckCore.StandY(ground.point.y), candidate.z);
            return true;
        }

        private static readonly string[] RailNames = { "RailL", "RailR", "RailB", "RailF" };

        private void BuildCockpitDeck()
        {
            // A walkable deck on the hull top at the cockpit: floor + rails on all four sides +
            // seat + helm. Every number comes from ShipDeckCore so the boarding teleport, the
            // builder and the tests cannot drift apart again.
            Vector3 deckCenter = cockpitLocalPos;
            var deck = MakeCube("CockpitDeck", deckCenter + new Vector3(0f, ShipDeckCore.PlateCentreY, 0f),
                new Vector3(ShipDeckCore.DeckSize, ShipDeckCore.DeckThickness, ShipDeckCore.DeckSize),
                new Color(0.18f, 0.20f, 0.24f), collider: true);
            _cockpitDeck = deck.transform;

            // RailF is new. Until 2026-08-02 the bow side was OPEN: walking forward off the deck was
            // a legal move with a long drop under it, and the fall-safety net was doing the catching.
            ShipDeckCore.RailOffsets(out float[] railX, out float[] railZ);
            for (int i = 0; i < RailNames.Length; i++)
            {
                ShipDeckCore.RailSize(i, out float sx, out float sy, out float sz);
                MakeCube(RailNames[i],
                    deckCenter + new Vector3(railX[i], ShipDeckCore.RailCentreY, railZ[i]),
                    new Vector3(sx, sy, sz), new Color(0.13f, 0.14f, 0.17f), collider: true);
            }
            MakeCube("PilotSeat", deckCenter + new Vector3(0f, 0.3f, -0.9f), new Vector3(0.6f, 0.6f, 0.6f), new Color(0.25f, 0.22f, 0.20f), true);

            // The helm console: destination rows, story-gated like the travel doors.
            var console = MakeCube("Helm", deckCenter + new Vector3(0f, 0.7f, 1.3f),
                new Vector3(2.6f, 1.3f, 0.15f), new Color(0.10f, 0.12f, 0.15f), collider: false);
            console.transform.localRotation = Quaternion.Euler(-20f, 0f, 0f);

            RebuildHelmRows();

            // THE ARMOURY RACK (⚖ Terry: the ship is where weapons live). Built before the
            // disembark panel because the panel's gate counts what is on it.
            BuildArmouryRack(deckCenter);

            // Disembark — gated on being armed. The BUTTON is the gate, not a collider: a press can
            // refuse and explain, where a wall can only shove, and shoving a standing VR player is
            // how you make someone take the headset off.
            var off = MakePanel("DisembarkPanel", transform.TransformPoint(deckCenter + new Vector3(0f, 0.6f, -1.55f)),
                "DISEMBARK", new Color(0.35f, 0.28f, 0.14f), () =>
                {
                    DepartureVerdict verdict = ShipArmouryRuntime.EvaluateNow(_armouryRack);
                    if (ShipArmouryCore.Blocks(verdict))
                    {
                        var rill = FindObjectOfType<RillCompanion>();
                        if (rill != null) rill.SayById(ShipArmouryCore.CueFor(verdict));
                        Debug.Log("ZIPTIDE: SHIP_DISEMBARK_BLOCKED verdict=" + verdict);
                        return;
                    }

                    Vector3 back = _berthReturnPos != Vector3.zero
                        ? _berthReturnPos
                        : transform.TransformPoint(doorLocalPos) + Vector3.forward;
                    // Same proof as boarding: the berth you remember may not be the berth that is
                    // there (a respawn, a refit, a world reloaded under you). Never step off into
                    // an unproven point — that is the trip that ends at FALL_SAFETY.
                    string source = "berth_return";
                    if (!TryProveStand(back, ref source, out Vector3 landing))
                    {
                        Debug.LogWarning("ZIPTIDE: SHIP_DISEMBARK_ABORT reason=no_proven_ground source=" + source);
                        return;
                    }
                    TeleportRig(landing);
                    Debug.Log("ZIPTIDE: SHIP_DISEMBARK verdict=" + verdict + " landing=" + landing.ToString("F2"));
                }, small: true);
            off.transform.rotation = transform.rotation * Quaternion.Euler(0f, 180f, 0f);
        }

        private Transform _armouryRack;
        private ArmouryCarouselRuntime _carousel;

        /// <summary>
        /// The weapon rail by the hatch. Slot geometry comes from ShipArmouryCore so the rack, the
        /// departure verdict and the tests all read one set of numbers.
        ///
        /// Weapons are spawned ON the rack rather than on the floor of the plaza, which is where the
        /// two starter guns used to land. Anything already carried stays carried — the rack seeds the
        /// ship once, it does not confiscate.
        /// </summary>
        private void BuildArmouryRack(Vector3 deckCenter)
        {
            if (_armouryRack != null) return;

            var rackGo = new GameObject("ArmouryRack");
            rackGo.transform.SetParent(transform, false);
            rackGo.transform.localPosition = deckCenter + new Vector3(-1.45f, 0f, -0.6f);
            _armouryRack = rackGo.transform;

            // THE DRUM, not a 2.4 m rail. Same slot module, same spacing, a fifth of the bulkhead —
            // and the footprint stops depending on how many weapons the player owns.
            _carousel = _armouryRack.gameObject.AddComponent<ArmouryCarouselRuntime>();

            // The column the slots ride on: a physical noun RILL's line can point at ("rack by the
            // hatch"), and the thing the player's hand actually turns.
            MakeCube("ArmouryDrum",
                _armouryRack.localPosition + new Vector3(0f, ArmouryCarouselCore.LowerRingHeight, 0f),
                new Vector3(ArmouryCarouselCore.Diameter * 0.35f,
                            ArmouryCarouselCore.RingGap * 2.2f,
                            ArmouryCarouselCore.Diameter * 0.35f),
                new Color(0.24f, 0.26f, 0.30f), collider: false);

            // Every weapon the player OWNS is restored, not just a hardcoded starter pair — "once you
            // have it, you have it" has to survive death, travel and quitting, and the rack is where
            // that promise becomes visible.
            PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            int restored = 0;
            foreach (string id in WeaponOwnership.OwnedIds(profile))
                if (SeedRackWeapon(id, WeaponOwnership.SlotIndexOf(id))) restored++;

            // First boot owns nothing, so the starter pair is GRANTED rather than merely placed —
            // otherwise it would vanish the first time the player died holding neither.
            if (restored == 0)
            {
                foreach (string id in new[] { "taser_dart_gun", "gravity_gun" })
                {
                    WeaponOwnership.Grant(profile, id);
                    if (SeedRackWeapon(id, WeaponOwnership.SlotIndexOf(id))) restored++;
                }
            }

            // Face the player at something they own rather than a blanking plate — the drum's first
            // read should be "here is your gear", not "here is an empty rack".
            var owned = WeaponOwnership.OwnedIds(profile);
            if (_carousel != null && owned.Count > 0)
                _carousel.PresentSlot(WeaponOwnership.SlotIndexOf(owned[0]));

            Debug.Log("ZIPTIDE: ARMOURY_RACK built=drum capacity=" + ArmouryCarouselCore.Capacity
                + " restored=" + restored + " diameter=" + ArmouryCarouselCore.Diameter.ToString("F2")
                + " owned=" + owned.Count);
        }

        /// <summary>Place one weapon at its permanent detent on the drum. Returns false if it failed.</summary>
        private bool SeedRackWeapon(string itemId, int slot)
        {
            if (slot < 0) return false;

            // Radial placement: the slot's face angle around the drum, at its ring's height.
            float yaw = ArmouryCarouselCore.FaceOf(slot) * ArmouryCarouselCore.StepDegrees;
            Vector3 outward = Quaternion.Euler(0f, yaw, 0f) * Vector3.forward;
            Vector3 local = new Vector3(0f, ArmouryCarouselCore.HeightOf(slot), 0f)
                + outward * (ArmouryCarouselCore.Diameter * 0.5f);

            GameObject go = ItemFactory.Create(itemId, _armouryRack.TransformPoint(local));
            if (go == null)
            {
                Debug.LogWarning("ZIPTIDE: ARMOURY_RACK seed_failed item=" + itemId + " slot=" + slot);
                return false;
            }
            go.transform.SetParent(_armouryRack, true);
            return true;
        }

        // The helm's destination rows, rebuilt on every boarding so lock states are always CURRENT
        // (finishing a contract then boarding must show the next world unlocked). Fits 12 worlds
        // (2 cols × 6 rows) — the whole authored arc, not the first 8.
        private Transform _helmRowsRoot;

        private void RebuildHelmRows()
        {
            if (_helmRowsRoot != null) Destroy(_helmRowsRoot.gameObject);
            _helmRowsRoot = new GameObject("HelmRows").transform;
            _helmRowsRoot.SetParent(transform, false);

            Vector3 deckCenter = cockpitLocalPos;
            var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            int shown = 0;
            for (int i = 0; i < destinationPacks.Count && shown < 12; i++)
            {
                var pack = destinationPacks[i];
                if (pack == null || string.IsNullOrEmpty(pack.sceneName)) continue;
                if (pack.sceneName == gameObject.scene.name) continue; // not the world we're parked in

                bool locked = !WorldGating.MeetsRequirements(pack, profile);
                string label = string.IsNullOrEmpty(pack.displayName) ? pack.packId : pack.displayName;
                string sceneName = pack.sceneName;
                var packRef = pack;

                int row = shown / 2, col = shown % 2;
                Vector3 pos = deckCenter + new Vector3(-0.65f + col * 1.3f, 1.25f - row * 0.24f, 1.22f);
                var rowPanel = MakePanel("Dest_" + pack.packId, transform.TransformPoint(pos),
                    (locked ? "LOCKED - " : "") + label, locked ? LockedColor : PanelColor,
                    locked
                        ? (System.Action)(() => Debug.Log("ZIPTIDE: TRAVEL_LOCKED pack=" + packRef.packId +
                            " missing=" + (WorldGating.FirstMissingRequirement(packRef,
                                SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null) ?? "?")))
                        : () => StartCoroutine(FlyOutThenTravel(sceneName)),
                    small: true);
                rowPanel.transform.SetParent(_helmRowsRoot, true);
                rowPanel.transform.localRotation = Quaternion.Euler(-20f, 0f, 0f);
                shown++;
            }
        }

        // ── S2: the fly-out presentation (comfort-first — the WORLD moves, never the camera) ─────

        [Tooltip("Seconds of fly-out presentation before the travel fires (S2; tune per SHIPS.md).")]
        [SerializeField] private float flyOutSeconds = 4.5f;

        private bool _departing;

        private IEnumerator FlyOutThenTravel(string sceneName)
        {
            if (_departing) yield break; // one departure at a time
            _departing = true;
            Debug.Log("ZIPTIDE: SHIP_DEPART dest=" + sceneName);

            // Seat the pilot (a teleport, not parenting) so the streaks read from the right spot.
            // Best-effort by design: a departure already committed must never be held up by a
            // failed seat nudge, so this proves the point but does not abort on a miss.
            string seatSource = "pilot_seat";
            Vector3 seat = transform.TransformPoint(cockpitLocalPos + new Vector3(0f, 0.1f, -0.9f));
            if (TryProveStand(seat, ref seatSource, out Vector3 seated)) TeleportRig(seated);

            // Star streaks: elongated unlit slivers racing PAST the deck, ramping with a launch rumble
            // feel — pure world motion, zero camera manipulation (VR comfort law).
            var streaks = new List<Transform>();
            var streakRoot = new GameObject("__FlyOutStreaks").transform;
            for (int i = 0; i < 26; i++)
            {
                var sGo = GameObject.CreatePrimitive(PrimitiveType.Cube);
                sGo.name = "Streak_" + i;
                var col = sGo.GetComponent<Collider>();
                if (col != null) Destroy(col);
                sGo.transform.SetParent(streakRoot, false);
                var r = sGo.GetComponent<Renderer>();
                if (r != null)
                {
                    var shader = Shader.Find("Universal Render Pipeline/Unlit");
                    if (shader == null) shader = Shader.Find("Universal Render Pipeline/Lit");
                    if (shader != null)
                    {
                        var mat = new Material(shader);
                        var c = new Color(0.75f, 0.85f, 1f);
                        mat.color = c;
                        if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", c);
                        r.material = mat;
                        r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                    }
                }
                ResetStreak(sGo.transform, randomizeAlong: true);
                streaks.Add(sGo.transform);
            }

            // Departure readout floats over the helm — a countdown sells the launch.
            var countGo = new GameObject("__DepartReadout");
            var countText = countGo.AddComponent<TextMesh>();
            countText.characterSize = 0.06f;
            countText.fontSize = 48;
            countText.anchor = TextAnchor.MiddleCenter;
            countText.alignment = TextAlignment.Center;
            countText.color = new Color(0.75f, 0.9f, 1f);
            countGo.transform.position = transform.TransformPoint(cockpitLocalPos + new Vector3(0f, 1.6f, 1.2f));
            countGo.transform.rotation = transform.rotation;

            float t = 0f;
            while (t < flyOutSeconds)
            {
                t += Time.deltaTime;
                float ramp = Mathf.Clamp01(t / 1.5f); // engines spool up
                float speed = 18f + 42f * ramp;
                float remaining = flyOutSeconds - t;
                countText.text = remaining > 1f
                    ? "DEPARTING IN " + Mathf.CeilToInt(remaining)
                    : "PUNCH IT";
                foreach (var st in streaks)
                {
                    if (st == null) continue;
                    st.position -= transform.forward * speed * Time.deltaTime;
                    // Streaks stretch with speed — reads as acceleration without moving the player.
                    st.localScale = new Vector3(0.05f, 0.05f, 1.5f + 5f * ramp);
                    if (Vector3.Dot(st.position - transform.position, transform.forward) < -25f)
                        ResetStreak(st, randomizeAlong: false);
                }
                yield return null;
            }

            Destroy(streakRoot.gameObject); // travel unloads the scene anyway; be tidy if it's slow
            Destroy(countGo);
            TravelCoordinator.TravelTo(sceneName); // the ONLY legal path (locked contract #1)
        }

        private void ResetStreak(Transform st, bool randomizeAlong)
        {
            // Scatter around the deck in a ring, ahead of the ship, oriented along the flight axis.
            Vector2 ring = Random.insideUnitCircle.normalized * (4f + Random.value * 8f);
            float along = randomizeAlong ? Random.Range(-20f, 30f) : 30f;
            Vector3 deckWorld = transform.TransformPoint(cockpitLocalPos);
            st.position = deckWorld + transform.right * ring.x + transform.up * (ring.y * 0.6f + 1f)
                        + transform.forward * along;
            st.rotation = transform.rotation;
        }

        // ── Rig teleport (never parent the rig — SHIPS.md guardrail) ─────────

        private static Vector3? RigPosition()
        {
            var rig = FindObjectOfType<PlayerRigPersistence>();
            return rig != null ? rig.transform.position : (Vector3?)null;
        }

        /// <summary>Proven teleport for the short in-ship hops. False = refused, player not moved.</summary>
        private bool StepTo(Vector3 candidate, string label)
        {
            string source = label;
            if (!TryProveStand(candidate, ref source, out Vector3 landing))
            {
                Debug.LogWarning("ZIPTIDE: SHIP_STEP_ABORT reason=no_proven_ground source=" + source);
                return false;
            }
            TeleportRig(landing);
            return true;
        }

        private static void TeleportRig(Vector3 worldPos)
        {
            var rig = FindObjectOfType<PlayerRigPersistence>();
            if (rig == null) return;
            var cc = rig.GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;   // same pattern as the fall-safety respawn
            rig.transform.position = worldPos;
            if (cc != null) cc.enabled = true;
        }

        // ── Primitive helpers ────────────────────────────────────────────────

        private GameObject MakeCube(string name, Vector3 localPos, Vector3 scale, Color color, bool collider)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(transform, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = scale;
            var c = go.GetComponent<Collider>();
            if (c != null) c.enabled = collider;
            Paint(go, color);
            return go;
        }

        private GameObject MakePanel(string name, Vector3 worldPos, string label, Color color,
                                     System.Action onSelect, bool small = false)
        {
            var root = new GameObject(name);
            root.transform.position = worldPos;

            var plate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            plate.name = "Plate";
            plate.transform.SetParent(root.transform, false);
            plate.transform.localScale = small ? new Vector3(1.15f, 0.22f, 0.05f) : new Vector3(0.9f, 0.45f, 0.06f);
            Paint(plate, color);
            var renderer = plate.GetComponent<Renderer>();

            var interactable = plate.AddComponent<XRSimpleInteractable>();
            var mgr = Object.FindObjectOfType<XRInteractionManager>();
            if (mgr != null) interactable.interactionManager = mgr;
            interactable.selectEntered.AddListener(_ => onSelect?.Invoke());
            interactable.hoverEntered.AddListener(_ => Tint(renderer, PanelHot));
            interactable.hoverExited.AddListener(_ => Tint(renderer, color));

            var textGo = new GameObject("Label");
            var tm = textGo.AddComponent<TextMesh>();
            tm.text = label;
            tm.characterSize = small ? 0.028f : 0.05f;
            tm.fontSize = 48;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.color = Color.white;
            textGo.transform.SetParent(root.transform, false); // unit-scale parent — no stretch (door-label lesson)
            textGo.transform.localPosition = new Vector3(0f, 0f, -0.06f);

            return root;
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
