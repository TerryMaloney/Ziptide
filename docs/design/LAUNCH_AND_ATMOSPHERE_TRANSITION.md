# LAUNCH & ATMOSPHERE→SPACE TRANSITION — liftoff, the re-entry veil, meta-consistent
### Terry 2026-07-21: "actual ship-raising-off-the-ground mechanics, then at the edge of space an upper-atmosphere push — everything turns red-hot and covers the screen, which is really the load into the space 'world' (so we don't render planet + space at once). Meta-consistent across all planets. We planned this but I'm not sure where or how detailed."

**Status:** 🔵 DESIGN — zero code (freeze). This EXPANDS the thin prior plan: `SHIPS_AND_FACTIONS`
§2 (atmospheric transition masking a load), `SPACE_ENVIRONMENT_AUDIT` §2.3 (launch transition),
`CELESTIAL_SYSTEM_CANON` (the sky both scenes share). Reuses `TravelCoordinator` (the ONLY scene-
change path) + the comfort-safe ziptide transition veil (`SANDBOX_TEST_LAB`).

---

## §0 — THE THREE BEATS (and the reverse)

**Liftoff → Ascent → the Veil (scene swap to space).** And the mirror on the way down:
**Re-entry veil → Descent → Touchdown.** The veil is the same effect both directions; it's the
load mask that lets a planet scene and a space scene never coexist.

## §1 — THE VR-COMFORT FRAMING (your red-hot idea is CORRECT — here's the safe form)

A **transition veil is NOT a combat flash.** A combat flash is banned (sudden, unexpected, you're
trying to act — nausea). A transition veil is FINE because it's **expected, brief, smooth, and you
aren't acting during it** — this is exactly how our `TravelCoordinator` already fades between
worlds. So "everything turns red-hot and covers the screen" is a legit load mask, with three rules:
1. **Smooth build, never a strobe** — the heat glow creeps in and peaks over ~1–2s, it doesn't snap.
2. **Diegetic-leaning** — the red-hot is **plasma on the canopy/hull** building around the ship
   (re-entry fire), not a flat color slapped on the eyes. A brief brightness peak masks the swap.
3. **The camera never moves.** Cockpit-locked frame throughout.

## §2 — BEAT 1: LIFTOFF (the ship raises off the ground)

- **Comfort first (vertical motion = the #1 vection nausea risk).** So liftoff is a **scripted,
  slow, smooth rise from the seat-locked cockpit frame**, NOT free vertical flight. Optional
  comfort vignette during the rise; speed conservative; the ground recedes gently.
- **Intentional trigger:** the player initiates it (grip/lever "engage launch"), so motion is never
  a surprise — a core comfort principle.
- **Diegetic sell:** engines spin up (audio + drive-heart brightening), dust/water blasts outward
  on the dock, landing skids retract, the dock and salvage yard drop away below, RILL callout
  ("taking us up"). The MK2's gill-nacelles glow, wingtip vanes deploy.
- **Then it hands off to Ascent** (still same planet scene).

## §3 — BEAT 2: ASCENT (climb through the atmosphere — the sky becomes space)

- The planet scene stays loaded; the **SkyVista lerps toward the space look** — because both are the
  SAME system truth (`CELESTIAL_SYSTEM_CANON`), this is a smooth data blend, not a cut: sky gradient
  darkens (blue→indigo→black), **stars fade in**, haze thins, the sun's glare sharpens, the horizon
  begins to curve. The ringed giant/moon stay put (same directions) and get crisper.
- Short and cinematic (a climb, not a chore). Comfort vignette optional on the climb.
- Ends at the **edge of space** → the Veil fires.

## §4 — BEAT 3: THE VEIL (the red-hot mask = the actual scene swap)

**This is the load.** At the atmosphere's edge:
- A **plasma/heat envelope builds around the ship** — orange→white-hot fire streaming over the
  canopy and hull (diegetic re-entry/ascent fire), audio roar rising, haptic rumble.
- Brightness **smoothly peaks** and briefly fills the view (the comfort-safe veil) — and UNDER that
  peak, `TravelCoordinator` **unloads the planet scene and loads the space scene**, ship persists
  (our standard travel swap; the ship is the constant, the world streams around it).
- The veil **clears** to reveal calm space: the full starfield, the sun, the planet **now seen from
  orbit below/behind you** (the CELESTIAL match — same bodies, revealed, with depth).
- **This solves Terry's render-budget point exactly:** planet and space are SEPARATE scenes; only
  one's heavy meshes are ever loaded; the veil hides the hand-off. No "render both at once."
- **Reverse (descent):** the same veil masks space→planet; you punch down through re-entry fire and
  it clears to the planet sky, then a scripted comfortable descent to the dock.

## §5 — META-CONSISTENCY ACROSS ALL PLANETS (the important part)

**Same MECHANIC everywhere; per-planet FLAVOR from the genome.** Every world declares its launch/
transition params (see `CELESTIAL_SYSTEM_CANON` §8 genome block):
- `atmosphereColor` + `thickness` → the ascent gradient + how long/thick the veil's fire reads
  (a thick toxic sky = a longer, greener-edged burn; a thin sky = a quick, faint veil; an airless
  moon = almost NO veil, just a fade).
- `systemId` → **which space scene you arrive in** (the system's shared space environment).
- `gravity` → the liftoff feel (heavier world = a slower, more labored rise).
- `veilTint` → the transition color (default red-hot; a toxic world might burn amber-green).
So the code path is ONE reusable launch sequence; the data makes W001's launch feel like W001 and a
desert world's feel like a desert world — and it can NEVER mismatch, because both the sky and the
launch read the same per-world block.

## §6 — REUSE / BUILD / COMFORT GATE

- **Reuses:** `TravelCoordinator` (launch = a Travel, so the ONLY-way-to-change-scenes law holds),
  the ziptide transition veil, SkyVista lerp, the comfort layer, F3.5 plasma VFX, the audio director.
  Mostly composition + a launch sequencer, not a new engine.
- **Order (post-checkpoint):** launch sequencer (liftoff→ascent→veil→arrive) as a scripted
  TravelCoordinator variant → SkyVista ascent lerp → the plasma veil VFX → per-planet genome params
  → the descent mirror.
- **Comfort device-test is the gate:** slow intentional rise, vignette option, smooth (never
  strobing) veil, camera never moves, no nausea on ascent/descent. Freeze: all post-Golden-Checkpoint.
- **Diagnostics:** `ZIPTIDE: LAUNCH phase=liftoff|ascent|veil|arrive world=… system=…`.
