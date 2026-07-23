# CELESTIAL SYSTEM CANON — one sky truth per system, matched ground↔space, across all worlds
### Terry 2026-07-21: "our first world is our first space trip — how many stars, nearby planets, constellations, and it MUST match when you go up into space. Plan it, keep it consistent across all worlds + space debris/derelicts."

**Status:** 🔵 DESIGN — zero code (freeze). A meta-building (genome-schema) addition: every world
declares its place in a star system, and the ground sky + the space view are two renderings of the
SAME data. Companions: `SPACE_ENVIRONMENT_AUDIT.md` (the space renderer this feeds), `systems/
SKYSCAPE_DESIGN.md` (the Prospect bar), `SkyVistaDefinition.cs` (the existing dome this extends).

---

## §1 — THE PRINCIPLE (the thing that must not break)

**One celestial truth per system; the ground sky and space are two VIEWS of it — so they match by
construction, not by hand.** The failure we're preventing: a hand-painted ground sky that doesn't
line up when you fly up. Fix: a single `StarSystemDefinition` (data) that BOTH the ground
`SkyVistaRig` and the space environment read. Ground = that truth seen through atmosphere (hazed,
horizon-cut, day/night). Space = the same truth with no atmosphere (crisper, darker, full 360, real
depth). Same bodies, same directions, same sizes — always.

## §2 — THE DATA MODEL (extends the existing SkyVista, doesn't replace it)

- **`StarSystemDefinition`** (NEW — the shared truth; many worlds can point at ONE):
  - **Star(s):** count (1–2), each with color/temperature, disc size, direction. Drives the SunRig
    key light (`SPACE_ENVIRONMENT_AUDIT` §2.1) AND the sky gradient's warm source.
  - **Major bodies:** the system's planets/gas-giants (the ringed giant), each with size + orbit
    slot — the big fixed features every world in the system sees.
  - **Starfield + constellations:** a fixed deep-background (stars are effectively infinitely far →
    identical from every world in the system/region). A named constellation set + the galactic-band
    arc. This is the "you're in the same part of the sky" glue.
  - **Region flags:** the Shell-grid presence (story canon 0→1 across W001→W012), nebula.
- **World-local placement** (each world adds, referencing its system):
  - **Parent body** it orbits (e.g. the Moss = a MOON of the ringed giant → why the giant looms).
  - **Its own moon(s)** + sibling bodies visible (a sibling moon = a nearby world).
  - **Sun direction / time-of-day**, atmosphere tint, horizon cut.
- **Both renderers derive from this:** the ground `SkyVistaDefinition` becomes (mostly) a DERIVED
  view of `StarSystem + placement` rather than hand-set bodies; the space env reads the same. The
  existing SkyVista fields (stars/nebula/bodies incl. `SunDisc`/`BandedPlanet`/`Moon`, shell grid,
  zenith shimmer) already cover 80% — this adds the shared-system layer above them.

## §3 — GROUND ↔ SPACE CONSISTENCY RULES (answers "does it get bigger?")

Going up out of the atmosphere is a SHORT trip, so:
- **Apparent size barely changes.** The ringed giant stays ~the same angular size — it's already
  huge and far; you didn't travel toward it. **What changes is REVELATION, not scale.**
- **Space removes the atmosphere:** haze/clouds gone → darker sky, MORE stars, sharper bodies,
  truer colors, the sun's glare harsher.
- **Space removes the horizon cut:** you see the FULL giant/moon (the ground only showed the top
  half above the sea).
- **Space adds depth/parallax:** bodies sit at real distances and shift slightly as you fly
  (`DistantBodyRig`), where the ground dome was flat. The sun stays fixed; near bodies parallax.
- **You see your OWN world below you** — the Moss shrinking as you climb (the launch beat).
- **Directions are identical:** if the giant was south-southwest at dusk on the ground, it's
  south-southwest from orbit. Same clock positions, always.

## §4 — W001 · THE MOSS SYSTEM CANON (proposed ⚖ — from the two skyscape keepers)

Read off `concepts/skyscape_moss/` (dusk + night):
- **The ringed giant** (Jupiter bands + Saturn rings), dominant, fills ~⅓ of sky. **⚖ CANON:
  the Moss is a MOON of this giant** — that's why it looms. Name ⚖ (story lane).
- **One grey cratered moon** — **⚖ a SIBLING moon of the same giant** = a nearby visitable world
  (a short in-system space hop, no gate). Ties worlds together.
- **The sun: ⚖ ONE warm star** (a K/G-type giving the amber dusk glow) — recommend single sun for
  W001 (binary suns saved as a signature for a LATER, stranger system, so W001 stays homey).
- **Starfield + a faint galactic-band arc** (night keeper) → the Moss-system constellation set;
  reused by the sibling moon and any other worlds in this system.
- **Shell grid = 0** at W001 (baseline; grows to 1.0 by W012 — existing SkyVista canon).
- **In space from the Moss you should see:** the giant (same size, fully revealed, ringplane
  crisp) + the sibling grey moon + the sun + the full starfield/arc + the Moss below you. All
  matching the ground keepers' layout.

## §5 — CONSISTENCY ACROSS ALL WORLDS (the meta-building rule)

- **Few SYSTEMS, many WORLDS.** The 80 worlds map onto a manageable set of star systems; a
  `StarSystemLibrary` (sibling of `SkyVistaLibrary`) holds them. Worlds in the same system SHARE
  star + giant + starfield → the region feels real and it's cheap.
- **Gates jump systems.** Traveling within a system (moon→sibling moon) keeps the same sky; going
  through a Ziptide gate to a far world = a NEW system = a DIFFERENT starfield/sun/giant. **The
  starfield changing is how the player FEELS they went far.** (Ground-hop = same sky; gate = new sky.)
- **The progression canon rides the system layer:** the banded giant slowly changes, the Shell grid
  0→1 across W001→W012, the zenith Pattern shimmer appearing (W003+) — all authored per system/world,
  never in code (as today).
- **Meta-building integration:** add a **CELESTIAL block to the world genome/`_WORLD_TEMPLATE`** —
  every world declares `systemId` + local placement (parent, moons, sun dir, time). This is a
  genome-schema addition (PIPELINE Stage 3). No world ships without it → nothing can mismatch.

## §6 — SPACE DEBRIS / DERELICTS CONSISTENCY (the "stuff" out there)

- **Same salvage grammar** as everything else (used-future families) — Warden white ceramic, salvage
  tugs, Guild hulls; nothing store-bought-looking (conformance audit applies in space too).
- **A per-system SPACE-OBJECT PALETTE:** each system declares what floats in it — asteroid type,
  debris density, derelict classes, station presence — so a system reads as a consistent PLACE, and
  it's authorable like the sky.
- **Ground↔space object echo (the nice touch):** a wreck you see beached on the Moss (RUSTBUCKET
  etc.) can appear as a derelict in the Moss's orbital space, and vice-versa — the same history seen
  from two altitudes. Debris fields tie to the system's story (Shell fragments near the grid, dead
  ships near old gates).
- **Faction-consistent traffic:** which ships patrol a system's space matches who controls it
  (Wardens near the Shell, Guild near W012/W018, salvagers near the Moss) — reuses `BotBrain` + the
  concepted ship cast.
- **Salvage loop:** derelicts/debris are POIs feeding the disable+salvage economy (`SPACE_COMBAT`).

## §7 — BUILD NOTES

- **Extends, doesn't replace:** `StarSystemDefinition` + `StarSystemLibrary` sit ABOVE the shipped
  SkyVista; the ground vista becomes a derived view, the space env a second consumer. Feeds the
  `SunRig` + `DistantBodyRig` from `SPACE_ENVIRONMENT_AUDIT`.
- **Order:** define the system schema → author the Moss system → make the ground vista derive from
  it (proves parity) → build the space env as the second consumer (the match is then free).
- **Freeze:** all post-Golden-Checkpoint; this is the map + the ⚖ Moss-system decisions for Terry.
- **⚖ stack for Terry:** Moss = moon-of-the-giant · sibling grey moon = a world · W001 = single warm
  sun · the giant's + system's names · binary-suns reserved for a later signature system.

## §8 — THE WORLD GENOME BLOCK (the exact fields every world declares — Stage-3 schema)

*So the sky, the space view, AND the launch transition (`LAUNCH_AND_ATMOSPHERE_TRANSITION.md`) all
read ONE per-world truth and can never mismatch. Added to `_WORLD_TEMPLATE` / WorldPack.*

```
world.celestial:
  systemId:            <string>   → the StarSystemDefinition this world belongs to
                                    (also = which SPACE SCENE you launch into)
  parentBody:          <bodyId>   → what it orbits (Moss → the ringed giant); "" if a primary
  moons:               [<bodyId>] → its own moon(s) / visible sibling bodies
  sunDirection:        <vec3>     → primary sun bearing (drives SunRig + sky warm source)
  timeOfDay:           <0..1>     → dusk/night/day slot for the ground vista
  atmosphereColor:     <color>    → ascent gradient tint + veil fire edge color
  atmosphereThickness: <0..1>     → ascent length + how thick/long the re-entry veil reads
                                    (airless moon ≈ 0 → almost no veil, just a fade)
  gravity:             <float>    → liftoff feel (heavier = slower, more labored rise)
  veilTint:            <color>    → transition-veil color (default red-hot; toxic ≈ amber-green)
  shellGridIntensity:  <0..1>     → story canon 0(W001)→1(W012); usually system/region-owned
```

**Rules:** every world MUST fill `celestial` (audit-enforced — no world ships without it, so nothing
can mismatch). `systemId` is the single knob that ties a world's sky, its space environment, its
debris palette (§6), and its launch destination together. Worlds sharing a `systemId` share the
star/giant/starfield for free. This block is the meta-building spine for the whole space domain.
