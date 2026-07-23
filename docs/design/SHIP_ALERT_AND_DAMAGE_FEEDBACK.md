# SHIP ALERT & DAMAGE FEEDBACK — debris impacts, warnings, the whole system (VR-safe)
### Terry 2026-07-21: "when space debris hits the ship we need a warning/alert that flashes and makes alarm sounds — we need a whole system for that."

**Status:** 🔵 DESIGN — zero code (freeze). Yes — a full ship alert/damage-feedback system. The ONE
correction: in VR the alert must be **diegetic + comfort-safe**, NOT a screen-filling flash. That's
more immersive AND more comfortable. Companions: `SPACE_COMBAT.md` (armor-only damage model this
reuses), `SPACE_ENVIRONMENT_AUDIT.md` (the space env), `CELESTIAL_SYSTEM_CANON.md` (per-system
debris palette), `SPACEFLIGHT_PHYSICS.md` (cockpit-locked comfort frame).

---

## §0 — THE VR CORRECTION (why "flash across the screen" becomes something better)

A sudden full-FOV red flash or **any camera shake** = nausea + a Meta comfort violation. So we
translate the instinct into **diegetic, directional, multi-channel** alerts that come from the
COCKPIT and the WORLD. The player already sits in a cockpit-locked frame looking out real windows
(our design) — so the cockpit IS the HUD. A klaxon, a red panel lighting up, the hull shuddering
through the CONTROLLERS (not the camera), sparks at the impact point, RILL calling it out. That
reads as "we're being hit!" without ever moving the head camera or whiting-out the eyes.

## §1 — PRINCIPLES

1. **Diegetic-first.** Alerts live in the cockpit + world (panels, lights, klaxons, sparks), not a
   flat screen overlay. The used-future aesthetic makes this a feature, not a limitation.
2. **Multi-channel redundancy.** Every alert fires across AUDIO + VISUAL(diegetic) + HAPTIC, so it
   lands even if you're looking the wrong way. (Accessibility bonus: never audio-only or visual-only.)
3. **COMFORT-LOCKED (non-negotiable):** the head camera NEVER moves on impact — no shake, no
   knock-the-view. No FOV-filling flash. Convey force through haptics + audio + the cockpit, not the eyes.
4. **DIRECTIONAL.** Tell the player WHERE — spatial audio from the threat's bearing + a cockpit edge-
   light on that side. A warning you can act on, not just a startle.
5. **Severity-tiered.** Feedback scales: a pebble tick ≠ a hull breach. Escalation is legible.
6. **Non-lethal canon.** Armor recharges, disable-don't-die (`SPACE_COMBAT`) — alerts build TENSION,
   not punishment. Getting hit is a "brace and recover" beat, not a death spiral.

## §2 — THE FEEDBACK CHANNELS (what actually fires)

- **AUDIO** (primary — most comfort-safe, most information): a layered klaxon by severity, a
  SPATIAL impact thud from the hit's bearing, metal groan/creak on the hull, system-down tones,
  and **RILL's voice callout** (§6). Audio ducking so the warning cuts through.
- **VISUAL — DIEGETIC (in the cockpit/world):**
  - a **physical warning panel** on the console lighting **RED** (our accent: red = danger),
  - **edge-light strips** around the cockpit pulsing on the threat's SIDE (directional),
  - **gauges/readouts dropping** (armor bar on the console, not a floating HUD),
  - **sparks + a scorch/dent decal at the impact point** on the hull (seen through the window),
  - the **drive-heart / channels flicker** on a big hit (the MK2's cyan veins stutter — ties to
    the ship-as-organism design),
  - **hull-breach VFX** (venting mist, a cracked canopy overlay ON THE GLASS, not the eye).
- **HAPTIC:** controller rumble scaled to severity (a tick for a pebble, a hard jolt for a slam),
  optionally directional (the hand nearest the impact side buzzes harder). This is where "you FEEL
  the hit" lives — the safe replacement for camera shake.
- **SCREEN-SPACE — RESTRAINED ONLY:** a soft, brief **peripheral vignette pulse** (edges only, low
  intensity, red) is comfort-standard and allowed; a full-FOV flash is NOT. Toggleable in comfort
  settings; off-by-default intensity conservative.

## §3 — ALERT TYPES × SEVERITY TIERS

| Tier | Example trigger | Audio | Visual | Haptic |
|---|---|---|---|---|
| **1 Tick** | small debris graze, spent | soft tick + groan | brief spark, panel amber blink | light buzz |
| **2 Impact** | debris chunk / a hit | thud + short klaxon | spark burst + dent decal, armor gauge drops, edge-light on side | firm jolt |
| **3 Warning** | armor low / incoming lock / collision imminent | sustained klaxon + RILL callout | red panel, pulsing edge-lights, proximity indicator | rhythmic pulse |
| **4 Critical** | hull breach / drive hit / armor gone | breach klaxon + venting roar + RILL urgent | canopy crack overlay, venting mist, drive-heart stutter, everything red | strong sustained |

**Alert kinds:** debris impact · incoming fire / weapon lock · **proximity/collision warning**
(approaching debris field, asteroid, another ship) · low-armor · hull breach · system offline
(engine/drive/nav). Proximity is the PREVENTIVE one — warns BEFORE the hit so a skilled pilot steers clear.

## §4 — DATA MODEL (event-driven; reuses what exists)

- **Reuse the armor-only damage model** (`SPACE_COMBAT`: recharging armor, no health bar). Debris/
  fire → armor damage → if armor breaks, systems degrade (not death) → recharge/repair restores.
- **`ShipStatusRuntime`** holds armor/system state and **raises typed events** (`OnImpact(bearing,
  severity)`, `OnArmorLow`, `OnBreach`, `OnProximity(threat)`, `OnSystemDown`). The cockpit visuals,
  the audio director, the haptics, and RILL all **subscribe** — same event-bus idiom as the rest of
  the game (no polling, decoupled).
- **`ShipAlertDefinition`** (data, by id): per-alert audio set, panel/light target, haptic curve,
  severity, RILL line ref. Authorable/tunable without code — consistent with our Definition pattern.
- **Diagnostics:** `ZIPTIDE: SHIP_ALERT kind=… sev=… bearing=…` (our log grammar) for device tuning.

## §5 — THE DEBRIS SYSTEM ITSELF (Terry's "whole system for that")

- **Debris as data, per system:** density, chunk size range, speed, material — declared in the
  system's **space-object palette** (`CELESTIAL_SYSTEM_CANON` §6), so each system's space feels
  distinct (a busy salvage system vs. a clean one).
- **Pooled debris objects** (reuse `GamePool`) flow through the flight volume; cheap on Quest.
- **On collision:** armor damage (scaled by chunk mass/speed) → raise `OnImpact(bearing, severity)`
  → the whole §2 feedback fires. **The ship's course is NOT knocked** (comfort) — the hull takes it,
  you feel it in hands/ears/eyes, but your flight stays your own.
- **Avoidable + salvageable:** proximity warning lets a good pilot steer clear; big chunks that hit
  (or that you tractor in) become **salvage** → the economy loop. Debris is threat AND resource.
- **Debris fields as encounters:** a dense field = a tension set-piece (thread the rocks), tying to
  the disable+salvage POI design.

## §6 — RILL AS THE VOICE OF ALERTS (the warmth, per Terry's "emotion" note)

The alert system's callouts route through **RILL** — she already keeps the incident log and has a
bark library. So warnings have a *character*, not a cold computer: **"Debris — starboard, brace!"**
· "Armor's thin, Cal." · a quiet "…that one hurt." after a big hit. This (a) makes alerts feel
human/tense instead of arcadey, (b) reuses the RILL line system, (c) is the same "make it a world,
not a game" texture Terry liked — the ship has a soul reacting to danger. Callouts obey the
never-annoying rule (severity-gated, cooldowns, variety) from `CAL_VOICE_AND_BARKS`.

## §7 — CONSISTENCY / REUSE / BUILD

- **The alert CHANNEL is reusable beyond ships:** ground hazard worlds (static W004, toxic W001)
  get the same klaxon + red-panel + edge-pulse + haptic grammar → one alert language game-wide.
- **Reuses:** armor model (`SPACE_COMBAT`), event bus, `GamePool`, RILL lines, the comfort layer,
  F3.5 VFX (sparks/venting), spatial audio director. Mostly composition, not new engines.
- **Order (post-checkpoint):** `ShipStatusRuntime` + events → cockpit diegetic feedback (panel/
  lights/gauges) → audio + haptic channels → debris system as the first producer → RILL callouts →
  reuse the channel for ground hazards.
- **Comfort test on device is the gate:** confirm zero nausea (no camera move, no FOV flash), the
  directional read works, and severity escalation is legible. Freeze: all post-Golden-Checkpoint.
