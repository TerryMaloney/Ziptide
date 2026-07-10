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
            // Campaign continuity, freshest first: the live session (survives travel) → the profile
            // save (survives quitting the game entirely) → a fresh war.
            bool resumedFromDisk = false;
            if (ConquestSession.State != null) _state = ConquestSession.State;
            else
            {
                _state = TryLoadCampaign();
                resumedFromDisk = _state != null;
                if (_state == null)
                    _state = ConquestGalaxy.BuildTwoPlayer(ConquestGalaxy.ChapterOneTwoSeeds());
                ConquestSession.State = _state;
            }
            BuildTable();
            ResolveReturnedBattle();
            Refresh();
            if (resumedFromDisk)
            {
                _ticker.text = "CAMPAIGN RESUMED — TURN " + _state.turn;
                Debug.Log("ZIPTIDE: CONQ_SAVE_RESUMED turn=" + _state.turn);
            }
            CheckEnd();   // a finished saved war shows its banner instead of a dead board
            Debug.Log("ZIPTIDE: WARTABLE_READY planets=" + _state.planets.Count);
        }

        // ── Campaign persistence (ConquestSave overlay in one profile flag) ──
        private void Autosave()
        {
            var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            if (profile == null || _state == null) return;
            profile.flags.RemoveAll(f => f.StartsWith(ConquestSave.FlagPrefix, System.StringComparison.Ordinal));
            profile.flags.Add(ConquestSave.FlagPrefix + ConquestSave.Serialize(_state));
        }

        private ConquestState TryLoadCampaign()
        {
            var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            if (profile == null) return null;
            foreach (var f in profile.flags)
                if (f.StartsWith(ConquestSave.FlagPrefix, System.StringComparison.Ordinal))
                    return ConquestSave.Deserialize(f.Substring(ConquestSave.FlagPrefix.Length),
                                                    ConquestGalaxy.ChapterOneTwoSeeds());
            return null;
        }

        private void ClearCampaignSave()
        {
            var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            profile?.flags.RemoveAll(f => f.StartsWith(ConquestSave.FlagPrefix, System.StringComparison.Ordinal));
        }

        /// <summary>B3: the player is back from (or walked out of) a mission — the held battle
        /// resolves NOW, with the mission's tilt folded into the order.</summary>
        private void ResolveReturnedBattle()
        {
            var pending = ConquestSession.Pending;
            if (pending == null || pending.order == null) return;
            ConquestSession.Pending = null;

            var attempt = pending.attempt;
            if (attempt != null && !attempt.IsTerminal) attempt.Abandon(); // walked out = decline
            int tilt = attempt != null ? attempt.ResultTilt() : 0;
            pending.order.missionModifier = tilt;

            var report = ConquestResolver.Resolve(_state, pending.order, pending.seed);
            StampOutcome(pending.order.targetPlanetId, report);
            string verdict = attempt == null || attempt.phase == MissionPhase.Declined
                ? "MISSION PASSED UP — base odds"
                : attempt.phase == MissionPhase.Won
                    ? "MISSION WON — tilt " + (tilt > 0 ? "+" : "") + tilt
                    : "MISSION BOTCHED — tilt " + (tilt > 0 ? "+" : "") + tilt;
            _card.text = verdict + "\n" + pending.order.targetPlanetId.ToUpperInvariant()
                       + " — " + report.outcome;
            Debug.Log("ZIPTIDE: CONQ_MISSION_RESOLVE tilt=" + tilt + " outcome=" + report.outcome);

            if (pending.rivalInitiated)
            {
                // Flying the defense pre-empted the rest of the rival's offensive — its turn ends.
                _ticker.text = "Your sortie disrupted the rival's turn.";
                _state.EndTurn();
            }
            Autosave();
            CheckEnd();
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
            Tile("NEW WAR", new Vector3(0.95f, 0.95f, 0.35f), NewWarPressed,
                 new Color(0.45f, 0.3f, 0.35f));
            Tile("SPIRE +3DEF\n(1F 3A)", new Vector3(-0.98f, 0.98f, -0.3f), BuildSpire,
                 new Color(0.35f, 0.55f, 0.8f));
            Tile("FRIGATE +2ATK\n(2F 2A)", new Vector3(-0.98f, 0.98f, 0.1f), BuildFrigate,
                 new Color(0.6f, 0.4f, 0.75f));
        }

        // ── Interaction FSM ──────────────────────────────────────────────────
        private void OnPlanetTapped(string planetId)
        {
            if (_aiTurnRunning) return;
            _newWarArmed = false;   // any other tap disarms the reset
            if (_offerPanel != null) { CloseOffer(); _ticker.text = "Strike called off."; }
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
            var wave = CommittedWave();
            float odds = ConquestAI.EstimateOdds(_state, wave, planetId);
            _card.text = p.displayName.ToUpperInvariant() + "  —  ODDS " + Mathf.RoundToInt(odds * 100f)
                       + "%  (WAVE " + wave.Count + "/" + _state.GetPlayer(0).fleetVesselIds.Count + ")"
                       + "\nTap it AGAIN to commit the strike.";
            if (_pendingTarget == planetId) CommitAttack(planetId);
            else _pendingTarget = planetId;
        }

        // ── The fleet rack: tap tokens to hold vessels back from the wave ────
        private GameObject _rackRoot;
        private readonly HashSet<int> _heldBack = new HashSet<int>();
        private string _rackSignature;

        /// <summary>The vessels actually going into the next strike (losses only bite the wave).</summary>
        private List<string> CommittedWave()
        {
            var fleet = _state.GetPlayer(0).fleetVesselIds;
            var wave = new List<string>();
            for (int i = 0; i < fleet.Count; i++)
                if (!_heldBack.Contains(i)) wave.Add(fleet[i]);
            return wave;
        }

        private void RebuildRack()
        {
            if (_rackRoot != null) Destroy(_rackRoot);
            _rackRoot = new GameObject("FleetRack");
            _rackRoot.transform.SetParent(transform, false);
            var fleet = _state.GetPlayer(0).fleetVesselIds;
            for (int i = 0; i < fleet.Count && i < 12; i++)
            {
                int idx = i;   // closure copy
                bool held = _heldBack.Contains(i);
                var tok = GameObject.CreatePrimitive(PrimitiveType.Cube);
                tok.name = "Token_" + i + "_" + fleet[i];
                tok.transform.SetParent(_rackRoot.transform, false);
                tok.transform.localPosition = new Vector3(-0.66f + i * 0.12f, held ? 0.93f : 0.98f, -0.52f);
                tok.transform.localScale = Vector3.one * (held ? 0.05f : 0.075f);
                Color c = TokenColor(fleet[i]);
                ItemFactory.ApplyURPColor(tok, held ? Color.Lerp(c, Color.black, 0.65f) : c);
                tok.AddComponent<XRSimpleInteractable>().selectEntered.AddListener(_ => ToggleToken(idx));
            }
            if (fleet.Count > 0)
            {
                var lbl = NewText("YOUR FLEET — tap a token to hold it back from the wave",
                                  new Vector3(0f, 0.88f, -0.6f), 0.005f, _rackRoot.transform);
                lbl.color = new Color(0.6f, 0.7f, 0.8f);
            }
        }

        private void ToggleToken(int idx)
        {
            if (_aiTurnRunning) return;
            if (!_heldBack.Remove(idx)) _heldBack.Add(idx);
            RebuildRack();
            // Re-show live odds if a target is mid-confirm, so the number tracks the wave.
            if (_pendingTarget != null && _state.GetPlanet(_pendingTarget) != null && _selected != null)
            {
                var wave = CommittedWave();
                float odds = ConquestAI.EstimateOdds(_state, wave, _pendingTarget);
                _card.text = _state.GetPlanet(_pendingTarget).displayName.ToUpperInvariant() +
                             "  —  ODDS " + Mathf.RoundToInt(odds * 100f) + "%  (WAVE " + wave.Count +
                             "/" + _state.GetPlayer(0).fleetVesselIds.Count + ")\nTap it AGAIN to commit the strike.";
            }
        }

        /// <summary>Stable per-vessel-type colour (the decal hash-to-hue idiom).</summary>
        private static Color TokenColor(string vesselId)
        {
            float h = Mathf.Abs(vesselId.GetHashCode() % 360) / 360f;
            return Color.HSVToRGB(h, 0.55f, 0.9f);
        }

        private string _pendingTarget;
        private GameObject _offerPanel;
        private bool _awaitingDefense;

        private void CommitAttack(string targetId)
        {
            var wave = CommittedWave();
            if (wave.Count == 0)
            {
                _ticker.text = "Your whole fleet is held back — free some tokens to strike.";
                return;
            }
            _pendingTarget = null;
            var order = new AttackOrder
            {
                attackerId = 0,
                fromPlanetId = _selected,
                targetPlanetId = targetId,
                vesselIds = wave,   // only the committed tokens ride — losses only bite the wave
            };
            int seed = _state.turn * 8191 + targetId.GetHashCode();
            // B3 — the gulag: strike at base odds, or fly a 2–3 minute mission IN that world to
            // tilt them. Declining costs nothing.
            bool underdog = _state.CountOwned(0) < _state.CountOwned(1);
            var mission = ConquestMissionLibrary.Offer(targetId, MissionSide.Attack, underdog);
            ShowBattleOffer(order, seed, mission, rivalInitiated: false);
        }

        // ── B3: the mission offer (both directions) ──────────────────────────
        private void ShowBattleOffer(AttackOrder order, int seed, ConquestMission mission, bool rivalInitiated)
        {
            CloseOffer();
            _offerPanel = new GameObject("BattleOffer");
            _offerPanel.transform.SetParent(transform, false);

            string pct = "+" + (mission.winTilt * 5) + "%";
            string passLabel = rivalInitiated ? "LET IT RIDE" : "STRIKE NOW";
            string flyLabel = (rivalInitiated ? "DEFEND\n" : "FLY THE MISSION\n") + pct;
            Tile(passLabel, new Vector3(-0.28f, 1.3f, -0.35f),
                 () => { CloseOffer(); ResolveNow(order, seed, rivalInitiated); },
                 new Color(0.85f, 0.55f, 0.3f), _offerPanel.transform);
            Tile(flyLabel, new Vector3(0.28f, 1.3f, -0.35f),
                 () => { CloseOffer(); LaunchMission(order, seed, mission, rivalInitiated); },
                 new Color(0.3f, 0.85f, 0.95f), _offerPanel.transform);

            _card.text = mission.title + "\n" + mission.brief +
                         "\nOptional — passing keeps your base odds.";
            Debug.Log("ZIPTIDE: CONQ_MISSION_OFFER planet=" + mission.planetId +
                      " side=" + mission.side + " tilt=" + pct);
        }

        private void CloseOffer()
        {
            if (_offerPanel != null) Destroy(_offerPanel);
            _offerPanel = null;
        }

        private void ResolveNow(AttackOrder order, int seed, bool rivalInitiated)
        {
            var report = ConquestResolver.Resolve(_state, order, seed);
            StampOutcome(order.targetPlanetId, report);
            if (rivalInitiated)
            {
                _ticker.text = "RIVAL strikes " + order.targetPlanetId + " — " + report.outcome;
                _awaitingDefense = false;   // the rival's turn coroutine may continue
            }
            else
            {
                var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
                profile?.SetFlag("CONQUEST_ATTACKED");
            }
            Autosave();
            Refresh();
            CheckEnd();
            Debug.Log("ZIPTIDE: WARTABLE_BATTLE outcome=" + report.outcome + " odds=" + report.odds.ToString("F2"));
        }

        private void LaunchMission(AttackOrder order, int seed, ConquestMission mission, bool rivalInitiated)
        {
            string scene = ConquestSession.SceneForPlanet(mission.planetId);
            if (string.IsNullOrEmpty(scene))
            {
                Debug.Log("ZIPTIDE: CONQ_MISSION_NO_SCENE planet=" + mission.planetId);
                ResolveNow(order, seed, rivalInitiated);
                return;
            }
            var attempt = new MissionAttempt(mission);
            attempt.Accept();
            ConquestSession.State = _state;
            ConquestSession.Pending = new PendingBattle
            { order = order, seed = seed, attempt = attempt, rivalInitiated = rivalInitiated };
            ConquestSession.ReturnScene = gameObject.scene.name;
            Debug.Log("ZIPTIDE: CONQ_MISSION_START planet=" + mission.planetId +
                      " kind=" + mission.kind + " scene=" + scene);
            TravelCoordinator.TravelTo(scene, transform.position);
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
            if (ok) Autosave();
            Refresh();
        }

        private void BuildFrigate()
        {
            if (_aiTurnRunning) return;
            bool ok = _state.BuildVessel(0, "pulse_frigate");
            _ticker.text = ok ? "Pulse Frigate joins your fleet (" + _state.GetPlayer(0).fleetVesselIds.Count + " vessels)"
                              : "Can't afford a Frigate (2 flux, 2 alloy).";
            if (ok) Autosave();
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
                if (_gameOver) break;   // a mid-turn capture can end the war
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
                        int aiSeed = _state.turn * 4093 + action.attack.targetPlanetId.GetHashCode();
                        var struck = _state.GetPlanet(action.attack.targetPlanetId);
                        if (struck != null && struck.ownerId == 0)
                        {
                            // B3 — YOUR world is under attack: fly the defense, or let it ride.
                            bool underdog = _state.CountOwned(0) < _state.CountOwned(1);
                            var defense = ConquestMissionLibrary.Offer(
                                action.attack.targetPlanetId, MissionSide.Defense, underdog);
                            _card.text = "RIVAL STRIKES " + struck.displayName.ToUpperInvariant() + "!";
                            _awaitingDefense = true;
                            ShowBattleOffer(action.attack, aiSeed, defense, rivalInitiated: true);
                            // Defend → travel kills this coroutine (the return path ends the rival
                            // turn). Let it ride → ResolveNow clears the flag and the turn goes on.
                            yield return new WaitWhile(() => _awaitingDefense);
                        }
                        else
                        {
                            var report = ConquestResolver.Resolve(_state, action.attack, aiSeed);
                            _ticker.text = "RIVAL strikes " + action.attack.targetPlanetId + " — " + report.outcome;
                            StampOutcome(action.attack.targetPlanetId, report);
                        }
                        break;
                }
                Refresh();
            }
            _state.EndTurn(); // production for everyone, upkeep, attack counters reset
            Autosave();
            _aiTurnRunning = _gameOver;   // a finished war stays frozen (NEW WAR still works)
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
                bool scouted = Scouted(p);
                Color c = p.ownerId == 0 ? PlayerColor : p.ownerId == 1 ? RivalColor : NeutralColor;
                if (!scouted) c = Color.Lerp(c, Color.black, 0.72f);   // fog of war
                if (p.planetId == _selected) c = Color.Lerp(c, Color.white, 0.35f);
                var r = orb.GetComponent<Renderer>();
                if (r != null)
                {
                    var mat = r.material;
                    if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", c);
                    else mat.color = c;
                }
                // Unscouted worlds hide their development — a flat dim dot, no intel.
                orb.localScale = Vector3.one * (scouted ? 0.055f + 0.008f * p.defenseLevel : 0.045f);
            }
            var you = _state.GetPlayer(0);
            // The rack mirrors the fleet; composition changes (builds/losses) reset the hold-backs.
            string sig = string.Join("+", you.fleetVesselIds);
            if (sig != _rackSignature) { _rackSignature = sig; _heldBack.Clear(); RebuildRack(); }
            _card.text = _card.text ?? "";
            string tickerBase = "TURN " + _state.turn +
                "   FLUX " + you.flux + " · ALLOY " + you.alloy + " · BLOOM " + you.bloommatter +
                "   FLEET " + you.fleetVesselIds.Count +
                "   WORLDS " + _state.CountOwned(0) + "/" + _state.planets.Count;
            if (string.IsNullOrEmpty(_ticker.text) || _ticker.text.StartsWith("TURN "))
                _ticker.text = tickerBase;
        }

        /// <summary>Fog of war: you can see what you hold, and what borders what you hold.</summary>
        private bool Scouted(PlanetNode p)
        {
            if (p.ownerId == 0) return true;
            foreach (var adjId in p.adjacentPlanetIds)
            {
                var n = _state.GetPlanet(adjId);
                if (n != null && n.ownerId == 0) return true;
            }
            return false;
        }

        private void ShowCard(PlanetNode p, string hint)
        {
            if (!Scouted(p))
            {
                _card.text = p.displayName.ToUpperInvariant() + "\nUNSCOUTED SPACE — take an adjacent world to reveal it.";
                return;
            }
            string owner = p.ownerId == 0 ? "YOURS" : p.ownerId == 1 ? "RIVAL" : "NEUTRAL";
            _card.text = p.displayName.ToUpperInvariant() + " (" + owner + ")  DEF " + p.defenseLevel +
                         "  PROD " + p.resourceProductionRate.ToString("F0") + " " + p.resourceType +
                         "\n" + hint;
        }

        private void CheckEnd()
        {
            int mine = _state.CountOwned(0), theirs = _state.CountOwned(1);
            if (theirs == 0) _card.text = "THE NETWORK IS YOURS.\nEvery gate answers to you now.\nNEW WAR starts another.";
            else if (mine == 0) _card.text = "THE RIVAL HOLDS THE NETWORK.\nNEW WAR — come back stronger.";
            else return;
            _gameOver = true;
            _aiTurnRunning = true;          // freezes normal input; NEW WAR bypasses it
            ClearCampaignSave();            // a finished war never resumes
            ConquestSession.State = null;
            Debug.Log("ZIPTIDE: WARTABLE_END mine=" + mine + " theirs=" + theirs);
        }

        // ── NEW WAR (arm-then-confirm reset) ─────────────────────────────────
        private bool _gameOver, _newWarArmed;

        private void NewWarPressed()
        {
            if (_aiTurnRunning && !_gameOver) return;   // never mid-AI-turn
            if (!_newWarArmed)
            {
                _newWarArmed = true;
                _ticker.text = "Abandon this campaign? Tap NEW WAR again.";
                return;
            }
            StopAllCoroutines();
            ClearCampaignSave();
            ConquestSession.Clear();
            for (int i = transform.childCount - 1; i >= 0; i--)
                Destroy(transform.GetChild(i).gameObject);
            _orbs.Clear(); _stamps.Clear();
            _rackRoot = null; _heldBack.Clear(); _rackSignature = null;   // swept with the children
            _selected = null; _pendingTarget = null; _offerPanel = null;
            _aiTurnRunning = false; _gameOver = false; _newWarArmed = false; _awaitingDefense = false;

            _state = ConquestGalaxy.BuildTwoPlayer(ConquestGalaxy.ChapterOneTwoSeeds());
            ConquestSession.State = _state;
            BuildTable();
            Refresh();
            _ticker.text = "A NEW WAR BEGINS.";
            Debug.Log("ZIPTIDE: WARTABLE_NEW_WAR");
        }

        // ── Helpers ──────────────────────────────────────────────────────────
        private void Tile(string label, Vector3 localPos, System.Action onSelect, Color color,
                          Transform parent = null)
        {
            var tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tile.name = "Tile_" + label.Split('\n')[0];
            tile.transform.SetParent(parent != null ? parent : transform, false);
            tile.transform.localPosition = localPos;
            tile.transform.localScale = new Vector3(0.3f, 0.16f, 0.05f);
            ItemFactory.ApplyURPColor(tile, color);
            tile.AddComponent<XRSimpleInteractable>().selectEntered.AddListener(_ => onSelect());
            var tm = NewText(label, localPos + new Vector3(0f, 0f, -0.05f), 0.007f, parent);
            tm.color = Color.white;
        }

        private TextMesh NewText(string text, Vector3 localPos, float charSize, Transform parent = null)
        {
            var go = new GameObject("Txt_" + (text.Length > 12 ? text.Substring(0, 12) : text));
            go.transform.SetParent(parent != null ? parent : transform, false);
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
