# COMFORT & ACCESSIBILITY — presets, the console, and the store story (design, 0.4)

> **Status: DESIGN LOCKED 2026-07-10 (Fable endgame item #5).** Decisions > code — this doc makes
> the judgment calls so any operator can build it without new architecture. Board home:
> `SPRINT_HARDWIRING.md` row 0.4. Map row: EXCELLENCE_MAP §3 "Locomotion & comfort".
> Companion: `ONBOARDING_TUTORIAL.md` (its LAW 5 — COMFORT BEFORE MOTION — is *implemented by*
> this design: the comfort console is the tutorial's first interactive moment).

## Why this is store-facing
The Meta store requires a **comfort rating** (Comfortable / Moderate / Intense) and reviewers rate
what the DEFAULT experience does, not what the options menu could do. So the two deliverables are:
(1) a defaults story we can defend as **Moderate**, and (2) a visible, diegetic way to go softer or
bolder. Accessibility (subtitles, seated play, handedness) rides the same surface because it's the
same store checklist section and the same console.

## The laws
1. **One switch, many dials.** The player chooses a PRESET (Cozy / Standard / Bold); every dial
   below follows from it. Custom exists, but presets are the interface — nobody tunes nine sliders
   in a headset.
2. **Comfort is a property of the body, not the save.** Settings live at DEVICE level
   (`PlayerPrefs`, the existing `ComfortVignette.PrefKey` precedent) — they survive profile wipes,
   apply before any save loads, and never sync between players. NOT in `PlayerProfile`.
3. **Every artificial motion reports.** Any system that translates or rotates the rig calls
   `ComfortVignette.ReportExternalMotion(speed01, turn01)` (the zipline already does) or carries an
   explicit `COMFORT_EXEMPT(<reason>)` marker comment. The gate makes this mechanical (§Gate).
4. **The defaults ARE the rating.** Standard preset must, by construction, satisfy the Moderate
   bar: vignette on all smooth motion, snap turn default, no forced camera motion (already law:
   never yank, never parent the rig). Bold is opt-in and never the install state.
5. **Accessibility is redundancy, not modes.** Subtitles for all VO/RILL lines (SubtitleText
   already exists), visual tells for every audio cue (already the creature-tell law), one console
   for all of it. No separate "accessibility menu" ghetto.

## The presets (the exact dial table)
Knobs name REAL fields — `LocomotionProfile`, `ComfortCore`/`ComfortVignette`, `ZiplineRuntime`,
`FlightController`. Build = a `ComfortPreset` table applying these; no new physics.

| Dial (owner) | 🛋 Cozy | 🎮 Standard (DEFAULT) | 🔥 Bold |
|---|---|---|---|
| Turn (`LocomotionProfile.turnMode/snapTurnAngle/smoothTurnSpeed`) | Snap 45° | Snap 30° | Smooth 120°/s |
| Vignette strength (`ComfortVignette.SetStrength`) | 1.0 | 0.6 | 0.15 |
| Slide (`slideBoost/slideSeconds`) | off (boost 1.0) | as shipped (1.35 / 0.8s) | as shipped |
| Dash (`dashEnabled` — 0.15s blink, comfortable by design) | on | on | on |
| Zipline cap (`ZiplineRuntime.maxSpeed`) | 5.5 m/s | 8 m/s (shipped) | 8 m/s |
| Conductor/belt ride | shipped 1.28 m/s everywhere — under every bar; no dial | — | — |
| Flight barrel roll (X/B) | **disabled** (button logs `FLIGHT_ROLL_COMFORT_OFF`) | shipped (hard vignette pulse) | shipped |
| Flight snap-yaw repeat cadence | 0.55s | 0.4s (shipped) | 0.4s |
| Flight boost ceiling | 1.8× | as shipped | as shipped |
| Jump pad / lift / climb | no dial — suspension idioms already comfortable; climb is 1:1 hand motion (natural, exempt) | — | — |

**Standalone toggles (not preset-bound, same console):**
- **Seated mode:** +0.35 m rig height offset (recenter-aware). A posture, not a preset.
- **Subtitles:** ON by default (all-ages law) · size S/M/L (×0.8 / ×1.0 / ×1.35 on SubtitleText).
- **Handedness v1:** mirror holster/belt layout + menu hand. NOT a full input remap (the input
  action asset is a locked contract — full remap is a future row, report-only territory).
- **Haptic intensity:** 0 / 0.5 / 1.0 multiplier routed through the existing SendHapticImpulse
  call sites via one static scale (pairs with gate-gap #7's coverage checklist later).

## Where it lives (the build shape — Opus-ready)
1. **`ComfortSettings` (Core, static):** preset enum + the dial table above as data + PlayerPrefs
   persistence (`ziptide_comfort_*` keys, the vignette's idiom) + `Apply()` that pushes values to
   the owners (LocomotionDirector re-reads its profile values, ComfortVignette.SetStrength, flight
   reads ceilings at input time). Pure resolve logic (preset→dials) is EditMode-tested.
2. **The comfort console (diegetic, W000 ship + every Quarters):** a patcher-authored panel — three
   preset tiles + the toggles, the ONBOARDING doc's first interactive beat. Reuses the
   ArenaLobbyBoard tile idiom (build-tested surface, arm's-length readable). Menu step is
   build-hooked like every generated surface; ALSO exposed as rows in the player menu
   (CONTROL_SCHEME's surface) so it's reachable anywhere.
3. **Logs:** `ZIPTIDE: COMFORT_PRESET preset=standard` on apply · every dial change logs its key.
4. **Touch budget:** LocomotionDirector + FlightController read values they already own (no new
   architecture); the ONLY rig-adjacent change is the seated-mode height offset — that one is
   **report-only** (VR_RIG_GOTCHAS first, announce the diff before pushing, per the escalation law).

## The gate (ships in the same chunk — LAW 3 of the map)
`ComfortCoverageTests` (EditMode):
- **Preset resolve is total:** every preset resolves every dial; Standard values equal the table
  above literally (the store rating is pinned by test).
- **Motion-source coverage (the mechanical LAW-3 check):** a manifest of motion-source files
  (Locomotion/DashLocomotion, ZiplineRuntime, ClimbRuntime, lift/jump-pad, FlightController,
  BeltConductorRuntime, vehicles) — the test reads each source file and asserts it contains
  `ReportExternalMotion(` **or** `COMFORT_EXEMPT`. A new motion source added without joining the
  registry fails CI with a message telling the author exactly what to do (the never-silent-scene
  gate's idiom, applied to comfort).
- Closes the map's 🕳️ "comfort-preset UI unbuilt (0.4)" gap when the console lands; until then the
  gap row points here.

## Store rating mapping (the defense, in one paragraph)
Default install = Standard: snap turn, vignette 0.6 on all artificial motion, no forced camera
motion anywhere (locked laws), flight/ziplines speed-capped with pulse protection, roll opt-in-ish
but vignette-pulsed → claim **Moderate**. Cozy documented in the store description for sensitive
players (45° snap, max vignette, no rolls, slower ziplines, no slide). Intense features (smooth
turn, full-speed flight) exist ONLY behind Bold. If review pushes back, the one-line fallback is:
make Cozy the install default (a one-value change, pinned by the resolve test).

## Photosensitivity review (added 2026-07-18, GAP_AUDIT fold)
Full-view luminance spikes get a review row before ship: the gate flash (`ZiptideGateEffect`
crest shell) is the known case — bound its rise time and peak duration, no strobing/repeated
flashes anywhere (VFX rails already forbid), and expose ONE "flash intensity" option riding the
existing vignette-strength plumbing. Check any new full-view effect against this row (audit
candidate once the caption/flash contrast audit lands — same screenshot machinery).

## Explicitly deferred (named so nobody re-litigates)
Full input remapping (locked contract) · per-eye IPD/lens knobs (platform-owned) · colorblind
palette setting (teal/amber accents already pass common CVD checks; revisit only on player report)
· locomotion vector source (head vs hand steer — shipped behavior stands until Terry's feel note).
