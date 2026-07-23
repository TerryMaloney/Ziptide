# SPACE FLIGHT — GAP AUDIT (what the whole space domain is still missing)
### Terry 2026-07-21: "look at all those aspects and figure out what we're missing."

**Status:** 🔵 AUDIT — zero code (freeze). Laid the complete space-flight EXPERIENCE (ship-backward,
per the finished-game benchmark method) against everything we've designed. Sources: `SPACE_
ENVIRONMENT_AUDIT`, `CELESTIAL_SYSTEM_CANON`, `LAUNCH_AND_ATMOSPHERE_TRANSITION`, `ATMOSPHERIC_
FLIGHT_AND_LANDING`, `SHIP_ALERT_AND_DAMAGE_FEEDBACK`, `SPACE_COMBAT`, `SPACEFLIGHT_PHYSICS`,
`SHIP_SYSTEM`, `CONTROLS_AND_FLIGHT`. Feeds `MISS_LEDGER` (a class fired — §3).

---

## §1 — WHAT'S COVERED (the map — so the gaps are visible)

| Domain slice | Owned by | State |
|---|---|---|
| Flight verb (comfort-capped model) | `FlightModel` + runtimes | ✅ built + tested |
| Space env (sun/stars/parallax/debris/VFX/audio) | `SPACE_ENVIRONMENT_AUDIT` | designed |
| Sky↔space consistency + genome | `CELESTIAL_SYSTEM_CANON` | designed |
| Launch / ascent / veil / re-entry | `LAUNCH_AND_ATMOSPHERE_TRANSITION` | designed |
| Fly-the-world / boundary / terrain / dock landing | `ATMOSPHERIC_FLIGHT_AND_LANDING` | designed |
| Damage / alerts / repair | `SHIP_ALERT_AND_DAMAGE_FEEDBACK` | designed |
| Combat + salvage (skeleton) | `SPACE_COMBAT` | skeleton |
| Seat / fly-hull / interior physics | `SHIP_SYSTEM` + `SPACEFLIGHT_PHYSICS` | designed |

**The pattern that jumps out:** we've thoroughly designed the *cool path* (flying, the vista, the
transition, taking hits) and the *place*, but under-designed the **connective tissue** (how you
pilot/aim/navigate) and the **edge/failure states** (out of power, disabled, stranded) and the
**"why am I out here"** loop. That's a class, not a list — see §3.

## §2 — THE GAPS (grouped; each = what · why it matters · where it lands)

### A — PILOTING & COCKPIT (how you actually fly, in VR hands)
- **A1 Diegetic piloting interaction** — do you GRAB a physical throttle + stick (the cockpit
  levers we concepted) or push an abstract stick? The VR "reach out and fly the controls" spec is
  thin (`FlightInputCore` exists but not the hand-interaction design). *The single biggest immersion
  lever and it's undesigned.*
- **A2 Cockpit instrument suite** — the full diegetic readouts (speed/altitude/nav/armor/target/
  comms) as physical gauges. Scattered across the alert doc; no unified cockpit-UI spec.
- **A3 Hover / station-keeping** — stopping to look, salvage, or fight (you can't do those at cruise
  speed). No design for a hold/hover mode.
- **A4 Throttle range detail** — boost/brake/reverse/idle feel + the speed-cue ladder. Partial.

### B — NAVIGATION & PURPOSE (where do I go, and why)
- **B1 In-flight wayfinding** — how you know where to go: a diegetic nav target / objective marker /
  RILL directions / a gate beacon. Undesigned (the galaxy map exists for SELECTING, not for the
  in-flight "fly toward it").
- **B2 THE SPACE GAMEPLAY LOOP — what you DO out there.** Mission types (salvage run, patrol,
  escort, distress call, discovery, delivery, gate-run). *The reason to fly is barely designed* —
  `SPACE_COMBAT` lists encounters but no loop/mission-type taxonomy. **Biggest design gap.**
- **B3 Discovery/exploration** — finding derelicts/POIs, the reward for wandering; ties to the
  leave-a-mark idea (a lantern in space).

### C — COMBAT & SALVAGE INTERACTION (the mechanics under the skeleton)
- **C1 VR combat targeting** — aim/lock/lead in a cockpit without nausea (aim-assist, soft-lock,
  diegetic reticle). `SPACE_COMBAT` flags it as TODO; still undesigned. *Comfort-critical.*
- **C2 The salvage interaction** — actually tractor/grab/tow a wreck and turn it into economy. Named,
  not mechanized.
- **C3 Enemy telegraphing** — how you READ an incoming attack in VR (so combat is fair + comfortable).

### D — FAILURE & EDGE STATES (the un-fun-but-necessary — a known blind class)
- **D1 Flight energy/fuel model** — is there a power/fuel resource? What runs out? (Or is flight
  free?) Undecided ⚖.
- **D2 The STRANDED / DISABLED state** — armor breaks or power dies in space: towed home? RILL
  distress beacon + rescue? drift-and-repair? **This is the failure FICTION the benchmark flagged we
  skip** (engineering states with no presentation state). Undesigned.
- **D3 Getting lost / boundary abuse / griefing the assists** — edge behavior of the auto-turn &
  avoidance (what if you fight them?). Light.

### E — PRESENCE & LIFE (making space feel inhabited, not a void)
- **E1 Ambient traffic** — other ships going about their business (salvagers, patrols, Wardens) so
  space is alive. Mentioned in celestial/combat, not designed as ambient life.
- **E2 Scale & awe moments** — the authored "whoa" beats (first sight of the giant from orbit, a
  Warden capital passing). Partly in celestial; no beat list.

### F — NARRATIVE & AUDIO IN FLIGHT
- **F1 Story delivery in flight** — RILL/Cal dialogue, **incoming transmissions (the de-garbling
  Transmission!)**, story beats during travel. Undesigned — a huge missed delivery channel (COD-
  cutscene-in-transit territory Terry raised long ago).
- **F2 Flight/space music & audio direction** — does the score shift in flight? Ties to the first-
  hour music genomes. Undesigned for space.

### G — ONBOARDING / ACCESSIBILITY / PERSISTENCE
- **G1 First-flight tutorial** — teaching flight comfortably (the `SpaceLane_Trial` scene exists as a
  harness; the teaching design doesn't). 
- **G2 Flight accessibility** — seated play, **assist-fly / auto-pilot-to-target for players who
  can't pilot**, one-handed, flight-specific comfort presets. Partial in CONTROLS.
- **G3 Save/resume in flight** — autosave on dock? mid-flight resume? (`RESUME_MOMENT`/`ConquestSave`
  may cover — needs a flight pass.)

### H — SPECIAL DOCKING / TRAVERSAL CASES
- **H1 Station docking** — docking at an in-space station (vs a planet dock). Undesigned.
- **H2 Gate traversal from space** — flying THROUGH a Ziptide gate in space (the ship-crossing we
  concepted) vs landing. The exterior spiral crossing exists as art; the flight mechanic doesn't.
- **H3 Derelict boarding / EVA** — do you land on / board / spacewalk to a wreck? ⚖ scope (could be
  "tractor from cockpit" only, no EVA). Decide the scope.

## §3 — THE CLASS FINDING (systemic — for the MISS_LEDGER)

**WHAT:** across the space domain we designed the cool-path + the place thoroughly but under-designed
(a) the connective tissue (pilot/aim/navigate), (b) the failure/edge states (fuel/stranded/disabled),
and (c) the "why you're here" loop — the SAME asymmetry the finished-game benchmark caught for the
game as a whole (entrances polished, exits assumed). **CLASS:** *domain designed cool-path-first;
verbs/loop/failure/onboarding assumed.* **SYSTEM CHANGE (proposed):** a **DOMAIN-COMPLETENESS
CHECKLIST** every domain audit must run — the eight columns: **VERB** (how you do it) · **PLACE**
(where) · **LOOP** (why / what activities) · **FAILURE** (what happens when it goes wrong) · **NAV/
WAYFINDING** · **NARRATIVE delivery** · **ONBOARDING** · **ACCESSIBILITY/COMFORT**. Space just failed
columns LOOP, FAILURE, NAV, NARRATIVE, ONBOARDING → those are exactly §2's B/D/F/G. Logging as
MISS_LEDGER #15 so it closes only when the checklist is standing machinery (not just this audit).

## §4 — PRIORITY (the MVP "space actually works" slice vs later)

**Must-have before a space vertical slice ships (the honest minimum):**
- A1 piloting interaction · A2 core instruments · B1 wayfinding · B2 at least ONE mission type
  (salvage run) · C1 comfortable targeting (if combat is in the slice) · D2 the disabled/stranded
  state · F1 basic RILL flight comms · G1 first-flight teaching · G2 assist-fly.
- Plus the already-designed spine (launch/veil, boundary/landing, alerts, sun/starfield).

**Can follow:** ambient traffic, station docking, gate-from-space, discovery depth, full mission
taxonomy, flight music polish, EVA/derelict-boarding (⚖ scope), the full combat build-out.

## §5 — Notes
- All build work is post-Golden-Checkpoint; this is the map + the ⚖ decisions (fuel model, EVA
  scope, mission-type set) for Terry.
- Nothing here contradicts the designed docs — it's the ring of missing pieces AROUND them.
