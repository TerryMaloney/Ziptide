# SUNRIG — interface + authored-data contract (the felt sun; multi-sun)
### Assist on GPT's offline-queue item #3 (2026-07-23). The consuming rig for the star data GPT put in `celestial_system_catalog.json`.

**Status:** 🔵 DESIGN/INTERFACE SPEC — zero code (freeze). Turns `SPACE_ENVIRONMENT_AUDIT` §2.1
(the #1 build-order item) into an implementable contract. Consumes the celestial catalog's `stars[]`
(`lightRole: SunRig-primary-key`, `temperatureFamily`, `exactColor`/`exactDiscAngularSize`
author-required, `countRole`). Both the ground `SkyVistaRig` and the space environment instantiate a
SunRig from the SAME star data → ground↔space match by construction (`CELESTIAL_SYSTEM_CANON` §1).

---

## §1 — WHAT A SunRig OWNS (one per star; a system may have 1–2)

A SunRig is the felt sun — NOT a painted disc. It owns four coupled outputs, all driven from one
star record:
1. **The scene KEY LIGHT** — a real directional light at the star's bearing (drives shadows,
   terminator, hull specular). The primary star's light is the scene's dominant key; a second star
   is a weaker fill key of its own color.
2. **The disc** — the visible sun body at the star's angular size + color (the `discRole:
   shared-ground-space` element — same disc asset ground and space).
3. **Bloom / lens-flare** — the "it blinds you when you turn into it" read, done via bloom + a
   flare, NOT a FOV-filling flash (comfort, §4).
4. **Hull/terrain response** — specular hotspot + a sharp terminator line across the ship/ground,
   so the sun is felt on surfaces, not just seen in the sky.

## §2 — THE AUTHORED-DATA CONTRACT (extends the celestial catalog `stars[]`)

Each star the SunRig consumes declares (author fills the two `author-required` ⚖ fields):
```
star:
  id:                  <string>              (e.g. moss_star_primary)
  countRole:           primary | secondary   → key vs fill light
  direction:           <bearing°, elevation°> → the light + disc placement (shared ground/space)
  exactColor:          <color>               ⚖ author-required (warm-K-or-G family for Moss)
  temperatureFamily:   <enum>                → default color/flare tint if exactColor absent
  exactDiscAngularSize:<deg>                 ⚖ author-required → disc size (ground & space identical)
  keyIntensity:        <float>               → directional light intensity (primary > secondary)
  flareProfile:        <id>                  → which bloom/flare style (subtle→harsh, comfort-capped)
  groundHorizonWarm:   <0..1>                → how much atmosphere reddens/softens it near the horizon
```
**Rule:** direction + angular size are SHARED across ground and space for a star; only the
atmosphere response differs (§3). So a sun at bearing 250°/elev 8° at dusk on the ground is at
250°/8° from orbit — the celestial match, enforced by reading one record.

## §3 — GROUND vs SPACE BEHAVIOR (same star, two renders)

- **Ground (through atmosphere):** the disc is softened/reddened near the horizon (`groundHorizonWarm`);
  bloom scatters wider (hazy); the key light is warmer/dimmer low, whiter/stronger high. The ascent
  lerp (`LAUNCH_AND_ATMOSPHERE_TRANSITION` §3) animates `groundHorizonWarm`→0 as you climb.
- **Space (no atmosphere):** crisp small disc, tight harsh flare, hard terminator, cold-white key,
  no scatter. Same bearing/size — just revealed and sharpened (`CELESTIAL_SYSTEM_CANON` §3).

## §4 — COMFORT RULES (non-negotiable, VR)

- **No FOV-filling flash, ever** — the "blinding" is bloom + flare on the disc, never a white-out of
  the eyes. Consistent with `SHIP_ALERT`/launch-veil comfort law.
- **The camera never moves** because of the sun.
- **Flare intensity is capped + comfort-toggleable**; a second sun must not double-strobe.
- Smooth transitions only (the ascent warm→cold lerp eases; never snaps).

## §5 — INTERFACE SURFACE (for the implementer)

- `SunRig.ApplyStar(StarRecord)` — configures the four outputs from one star record.
- `SunRigDirector.ApplySystem(systemId)` — reads the celestial catalog, spawns one SunRig per
  `stars[]` entry (1–2), assigns primary=key / secondary=fill; called by BOTH `SkyVistaRig` (ground)
  and the space-environment loader → parity for free.
- `SunRig.KeyLight` — the `Light` handle the rest of the scene reads (grounding, hull specular).
- `SunRig.SetAtmosphere(t)` — 0=space, 1=full ground; the launch/ascent lerp drives it.
- Reads only DATA (no per-scene hardcoding) — the one-truth rule.

## §6 — BUILD / TEST / FIT (matches GPT's gate pattern)

- **A `sunrig_contract_gate` validator** should enforce this against the celestial catalog (parallel
  to GPT's `celestial_system_gate`): every system has ≥1 `SunRig-primary-key` star; author-required
  fields are flagged when unfilled; ≤2 stars; direction/size present. (Freeze-safe Python gate,
  same idiom as GPT's — CI-enforceable.)
- **Order (post-checkpoint):** SunRig component (single sun) → SunRigDirector reads the catalog →
  ground SkyVista consumes it (proves parity on the ground first) → space env consumes it (match is
  then free) → second-sun fill support → the ascent atmosphere lerp.
- **⚖ for Terry (the author-required fields):** the Moss sun's exact color + disc angular size (the
  two `author-required` values), and whether the Moss system stays single-sun (recommended;
  binary reserved for a later signature system per `CELESTIAL_SYSTEM_CANON` §4).
- **Freeze:** all post-Golden-Checkpoint; this is the contract GPT's data plugs into.
