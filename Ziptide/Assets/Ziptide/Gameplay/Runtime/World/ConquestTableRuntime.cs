using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Core;
using Ziptide.Multiplayer.Conquest;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// TIDEFRONT B2/5.2 — THE WAR TABLE: the first visible surface for the built-and-tested conquest
    /// sim (18 headless tests; this component contains ZERO rules — every number comes from
    /// ConquestState/Resolver/AI). A waist-high holo table: planet orbs (color = owner, size grows
    /// with defenses), adjacency filaments, a resource ticker. The loop the spec calls "the dopamine":
    ///   tap YOUR planet → info card + build tiles (Shield Spire / Pulse Frigate)
    ///   → tap an ADJACENT enemy/neutral world → live odds card → COMMIT
    ///   → THE RESOLUTION MOMENT: outcome stamp pops over the target (MAJOR VICTORY gold /
    ///     COUNTERSTRIKE red), the orb recolors, the ticker reports losses
    ///   → END TURN → the RIVAL AI takes its turn VISIBLY (one ticker line per move, never a silent
    ///     state jump) → production → yours again. Win = the whole map glows your color.
    /// Deterministic resolution (seed from turn+ids) — the same battle replays identically, the
    /// async-multiplayer property the sim was built for. First attack sets CONQUEST_ATTACKED (RILL:
    /// "The Wardens will notice this." — the spec's own line).
    /// </summary>
    public class ConquestTableRuntime : MonoBehaviour
    {
        private ConquestState _state;
        private readonly Dictionary<string, Transform> _orbs = new Dictionary<string, Transform>();
        private readonly Dictionary<string, TextMesh> _stamps = new Dictionary<string, TextMesh>();
        private string _selected;          // the player's selected OWN planet
        private TextMesh _ticker, _card;
        private bool _aiTurnRunning;

        private static readonly Color PlayerColor = new Color(0.3f, 0.85f, 0.95f);  // tide cyan
        private static readonly Color RivalColor = new Color(0.95f, 0.35f, 0.3f);   // rival red
        private static readonly Color NeutralColor = new Color(0.45f, 0.45f, 0.5f);

        private void Start()
        {
            _state = ConquestGalaxy.BuildTwoPlayer(ConquestGalaxy.ChapterOneTwoSeeds());
            BuildTable();
            Refresh();
            Debug.Log("ZIPTIDE: WARTABLE_READY planets=" + _state.planets.Count);
        }

        // ── The table body ───────────────────────────────────────────────────
        private void BuildTable()
        {
            var slab = GameObject.CreatePrimitive(PrimitiveType.Cube);
            slab.name = "TableSlab";
            Object.Destroy(slab.GetComponent<Collider>());
            slab.transform.SetParent(transform, false);
            slab.transform.localPosition = new Vector3(0f, 0.85f, 0f);
            slab.transform.localScale = new Vector3(1.7f, 0.06f, 1.2f);
            ItemFactory.ApplyURPColor(slab, new Color(0.1f, 0.12f, 0.16f));

            // Planet orbs on a two-row arc (chain order = story order — W001 near you, W012 far).
            int n = _state.planets.Count;
            for (int i = 0; i < n; i++)
            {
                var p = _state.planets[i];
                float t = n <= 1 ? 0.5f : i / (float)(n - 1);
                Vector3 pos = new Vector3(
                    Mathf.Lerp(-0.72f, 0.72f, t),
                    1.02f,
                    (i % 2 == 0 ? -0.22f : 0.22f) + Mathf.Sin(t * Mathf.PI) * 0.14f);

                var orb = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                orb.name = "Orb_" + p.planetId;
                orb.transform.SetParent(transform, false);
                orb.transform.localPosition = pos;
                var col = orb.GetComponent<Collider>();
                if (col != null) col.isTrigger = true;
                string captured = p.planetId; // closure copy
                orb.AddComponent<XRSimpleInteractable>().selectEntered
                   .AddListener(_ => OnPlanetTapped(captured));
                _orbs[p.planetId] = orb.transform;

                var label = NewText(p.displayName, pos + new Vector3(0f, 0.1f, 0f), 0.006f);
                label.color = new Color(0.8f, 0.85f, 0.95f);

                var stamp = NewText("", pos + new Vector3(0f, 0.2f, 0f), 0.012f);
                _stamps[p.planetId] = stamp;
            }

            // Adjacency filaments (draw once — topology never changes mid-campaign).
            var drawn = new HashSet<string>();
            foreach (var p in _state.planets)
                foreach (var adjId in p.adjacentPlanetIds)
                {
                    string key = string.CompareOrdinal(p.planetId, adjId) < 0
                        ? p.planetId + "|" + adjId : adjId + "|" + p.planetId;
                    if (!drawn.Add(key) || !_orbs.ContainsKey(adjId)) continue;
                    Vector3 a = _orbs[p.planetId].localPosition, b = _orbs[adjId].localPosition;
                    var line = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    line.name = "Lane_" + key;
                    Object.Destroy(line.GetComponent<Collider>());
                    line.transform.SetParent(transform, false);
                    line.transform.localPosition = (a + b) * 0.5f - new Vector3(0f, 0.03f, 0f);
                    line.transform.localRotation = Quaternion.LookRotation(b - a);
                    line.transform.localScale = new Vector3(0.008f, 0.008f, Vector3.Distance(a, b));
                    ItemFactory.ApplyURPColor(line, new Color(0.3f, 0.4f, 0.55f));
                }

            _ticker = NewText("", new Vector3(0f, 0.72f, -0.72f), 0.009f);
            _ticker.color = new Color(0.7f, 0.9f, 0.85f);
            _card = NewText("", new Vector3(0f, 1.55f, 0f), 0.01f);
            _card.color = new Color(0.9f, 0.95f, 1f);

            Tile("END TURN", new Vector3(0.95f, 0.95f, -0.45f), EndTurnPressed,
                 new Color(0.85f, 0.7f, 0.25f));
            Tile("SPIRE +3DEF\n(1F 3A)", new Vector3(-0.98f, 0.98f, -0.3f), BuildSpire,
                 new Color(0.35f, 0.55f, 0.8f));
            Tile("FRIGATE +2ATK\n(2F 2A)", new Vector3(-0.98f, 0.98f, 0.1f), BuildFrigate,
                 new Color(0.6f, 0.4f, 0.75f));
        }

        // ── Interaction FSM ──────────────────────────────────────────────────
        private void OnPlanetTapped(string planetId)
        {
            if (_aiTurnRunning) return;
            var p = _state.GetPlanet(planetId);
            if (p == null) return;

            if (p.ownerId == 0)
            {
                _selected = planetId;
                ShowCard(p, "YOUR WORLD — select an adjacent target to attack, or build here.");
                return;
            }

            // Non-owned tap: an attack attempt from the selected world.
            if (_selected == null) { ShowCard(p, "Select one of YOUR worlds first."); return; }
            if (!_state.CanAttack(0, _selected, planetId))
            {
                ShowCard(p, "Can't strike from " + _selected + " — not adjacent, or out of attacks.");
                return;
            }
            float odds = ConquestAI.EstimateOdds(_state, 0, planetId);
            _card.text = p.displayName.ToUpperInvariant() + "  —  ODDS " + Mathf.RoundToInt(odds * 100f)
                       + "%\nTap it AGAIN to commit the strike.";
            if (_pendingTarget == planetId) CommitAttack(planetId);
            else _pendingTarget = planetId;
        }

        private string _pendingTarget;

        private void CommitAttack(string targetId)
        {
            _pendingTarget = null;
            var player = _state.GetPlayer(0);
            var order = new AttackOrder
            {
                attackerId = 0,
                fromPlanetId = _selected,
                targetPlanetId = targetId,
                vesselIds = new List<string>(player.fleetVesselIds), // commit the fleet
            };
            int seed = _state.turn * 8191 + targetId.GetHashCode();
            var report = ConquestResolver.Resolve(_state, order, seed);
            StampOutcome(targetId, report);
            var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            profile?.SetFlag("CONQUEST_ATTACKED");
            Refresh();
            Debug.Log("ZIPTIDE: WARTABLE_BATTLE outcome=" + report.outcome + " odds=" + report.odds.ToString("F2"));
        }

        private void StampOutcome(string planetId, BattleReport r)
        {
            if (!_stamps.TryGetValue(planetId, out var stamp)) return;
            switch (r.outcome)
            {
                case BattleOutcome.MajorVictory: stamp.text = "MAJOR VICTORY"; stamp.color = new Color(1f, 0.85f, 0.3f); break;
                case BattleOutcome.CostlyVictory: stamp.text = "COSTLY VICTORY"; stamp.color = new Color(0.8f, 0.9f, 0.5f); break;
                case BattleOutcome.Stalemate: stamp.text = "STALEMATE"; stamp.color = new Color(0.7f, 0.7f, 0.7f); break;
                case BattleOutcome.FailedAttack: stamp.text = "ATTACK FAILED"; stamp.color = new Color(0.9f, 0.5f, 0.4f); break;
                default: stamp.text = "COUNTERSTRIKE"; stamp.color = new Color(1f, 0.25f, 0.2f); break;
            }
            StartCoroutine(FadeStamp(stamp));
            _ticker.text = "Losses — vessels: " + r.attackerVesselsLost.Count +
                           " · defenders: " + r.defenderUnitsLost;
        }

        private IEnumerator FadeStamp(TextMesh stamp)
        {
            float t = 0f;
            Vector3 basePos = stamp.transform.localPosition;
            while (t < 3f)
            {
                t += Time.deltaTime;
                stamp.transform.localPosition = basePos + Vector3.up * (t * 0.03f);
                var c = stamp.color; c.a = Mathf.Clamp01(1.2f - t / 3f); stamp.color = c;
                yield return null;
            }
            stamp.text = "";
            stamp.transform.localPosition = basePos;
        }

        // ── Builds ───────────────────────────────────────────────────────────
        private void BuildSpire()
        {
            if (_aiTurnRunning || _selected == null) return;
            bool ok = _state.BuildDefense(0, _selected, "shield_spire");
            _ticker.text = ok ? "Shield Spire raised at " + _selected
                              : "Can't afford a Spire (1 flux, 3 alloy).";
            Refresh();
        }

        private void BuildFrigate()
        {
            if (_aiTurnRunning) return;
            bool ok = _state.BuildVessel(0, "pulse_frigate");
            _ticker.text = ok ? "Pulse Frigate joins your fleet (" + _state.GetPlayer(0).fleetVesselIds.Count + " vessels)"
                              : "Can't afford a Frigate (2 flux, 2 alloy).";
            Refresh();
        }

        // ── The visible AI turn ──────────────────────────────────────────────
        private void EndTurnPressed()
        {
            if (_aiTurnRunning) return;
            StartCoroutine(RunRivalTurn());
        }

        private IEnumerator RunRivalTurn()
        {
            _aiTurnRunning = true;
            _card.text = "RIVAL IS MOVING…";
            var plan = ConquestAI.PlanTurn(_state, 1, ConquestAiProfile.Balanced, seed: _state.turn);
            foreach (var action in plan)
            {
                yield return new WaitForSeconds(0.8f); // never a silent state jump — you WATCH it move
                switch (action.kind)
                {
                    case ConquestAction.Kind.BuildDefense:
                        if (_state.BuildDefense(1, action.planetId, action.catalogId))
                            _ticker.text = "RIVAL builds " + action.catalogId + " at " + action.planetId;
                        break;
                    case ConquestAction.Kind.BuildVessel:
                        if (_state.BuildVessel(1, action.catalogId))
                            _ticker.text = "RIVAL commissions a " + action.catalogId;
                        break;
                    case ConquestAction.Kind.Attack when action.attack != null:
                        var report = ConquestResolver.Resolve(_state, action.attack,
                            seed: _state.turn * 4093 + action.attack.targetPlanetId.GetHashCode());
                        _ticker.text = "RIVAL strikes " + action.attack.targetPlanetId + " — " + report.outcome;
                        StampOutcome(action.attack.targetPlanetId, report);
                        break;
                }
                Refresh();
            }
            _state.EndTurn(); // production for everyone, upkeep, attack counters reset
            _aiTurnRunning = false;
            _selected = null;
            Refresh();
            CheckEnd();
        }

        // ── Display ──────────────────────────────────────────────────────────
        private void Refresh()
        {
            foreach (var p in _state.planets)
            {
                if (!_orbs.TryGetValue(p.planetId, out var orb)) continue;
                Color c = p.ownerId == 0 ? PlayerColor : p.ownerId == 1 ? RivalColor : NeutralColor;
                if (p.planetId == _selected) c = Color.Lerp(c, Color.white, 0.35f);
                var r = orb.GetComponent<Renderer>();
                if (r != null)
                {
                    var mat = r.material;
                    if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", c);
                    else mat.color = c;
                }
                orb.localScale = Vector3.one * (0.055f + 0.008f * p.defenseLevel);
            }
            var you = _state.GetPlayer(0);
            _card.text = _card.text ?? "";
            string tickerBase = "TURN " + _state.turn +
                "   FLUX " + you.flux + " · ALLOY " + you.alloy + " · BLOOM " + you.bloommatter +
                "   FLEET " + you.fleetVesselIds.Count +
                "   WORLDS " + _state.CountOwned(0) + "/" + _state.planets.Count;
            if (string.IsNullOrEmpty(_ticker.text) || _ticker.text.StartsWith("TURN "))
                _ticker.text = tickerBase;
        }

        private void ShowCard(PlanetNode p, string hint)
        {
            string owner = p.ownerId == 0 ? "YOURS" : p.ownerId == 1 ? "RIVAL" : "NEUTRAL";
            _card.text = p.displayName.ToUpperInvariant() + " (" + owner + ")  DEF " + p.defenseLevel +
                         "  PROD " + p.resourceProductionRate.ToString("F0") + " " + p.resourceType +
                         "\n" + hint;
        }

        private void CheckEnd()
        {
            int mine = _state.CountOwned(0), theirs = _state.CountOwned(1);
            if (theirs == 0) { _card.text = "THE NETWORK IS YOURS.\nEvery gate answers to you now."; _aiTurnRunning = true; }
            else if (mine == 0) { _card.text = "THE RIVAL HOLDS THE NETWORK.\nWalk away from the table… and come back stronger."; _aiTurnRunning = true; }
        }

        // ── Helpers ──────────────────────────────────────────────────────────
        private void Tile(string label, Vector3 localPos, System.Action onSelect, Color color)
        {
            var tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tile.name = "Tile_" + label.Split('\n')[0];
            tile.transform.SetParent(transform, false);
            tile.transform.localPosition = localPos;
            tile.transform.localScale = new Vector3(0.3f, 0.16f, 0.05f);
            ItemFactory.ApplyURPColor(tile, color);
            tile.AddComponent<XRSimpleInteractable>().selectEntered.AddListener(_ => onSelect());
            var tm = NewText(label, localPos + new Vector3(0f, 0f, -0.05f), 0.007f);
            tm.color = Color.white;
        }

        private TextMesh NewText(string text, Vector3 localPos, float charSize)
        {
            var go = new GameObject("Txt_" + (text.Length > 12 ? text.Substring(0, 12) : text));
            go.transform.SetParent(transform, false);
            go.transform.localPosition = localPos;
            var tm = go.AddComponent<TextMesh>();
            tm.text = text;
            tm.characterSize = charSize; tm.fontSize = 64;   // the characterSize×fontSize lesson
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            return tm;
        }
    }
}
