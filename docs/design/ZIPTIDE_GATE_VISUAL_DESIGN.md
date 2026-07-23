# THE ZIPTIDE ITSELF — the photon-tide design (upgrading the gate from janky to sacred)

**Status:** 🔵 DESIGN + CONCEPT PHASE — zero code (freeze). Terry commissioned 2026-07-20:
"it works but it's janky… I love Stargate's water… but think photons — a reasonably plausible
scientific way this could work." Feeds: queue C2 (gate concept), the future `ZiptideGateEffect`
upgrade (post-checkpoint, budget-gated), LS-5 tide acts, the night-glow motif family.

> **Machine catalog:** `docs/project_art_plan/gate_lifecycle_catalog.json` — CI-enforced; keep in sync (`docs/CATALOG_DOC_RECONCILE.md`).

## §1 — The pseudo-science canon (⚖ Terry to bless; built on REAL physics with one stretch)

- **Real basis:** "quantum fluids of light" is an actual research field — in slow-light media
  and polariton condensates, photons behave as a FLUID (pooling, flowing, waves, ripples);
  labs have slowed light to meters per second. The one stretch: ours does it at room scale.
- **The canon:** the Shell's membrane is a slow-light boundary. Where it intersects our space
  — pinned open between the pillars — light condenses into a **photon fluid**: a liquid of
  light. A bounded liquid under cycling tension has TIDES. The membrane breathes (the network
  "inhales" — existing canon); the fluid surges and ebbs. **Crossing = the crest picks you up;
  you arrive on the ebb elsewhere.** No hole, no tunnel — a tide with a memory ("the tide does
  not carry us; it remembers where we were going" — the pool line, now physics).
- **The pillars are resonators** pinning the membrane open, spaced at its wavelength. RILL's
  "do not count the pillars — you will lose track on purpose" = interference nodes shift.
- **THE UNIFIER:** trace photon-fluid dissolves into real seawater around every gate — which
  IS: the night-glowing horizon band (sky keeper) · the glowing tide pools (K5) · what the
  skiff's fin-keels grip · why gates stand in water. One mechanism, five visuals, no new lore
  debt. Cal's "how do you make a tide out of light?" → you slow it down until it pools.

## §1b — THE ENTRAINMENT LAW (Terry's addition, 2026-07-20 — the tide is made of HERE)

**Terry:** "over water it pulls the water up… I want the metal of the area around it to
dematerialize and turn into the vortex — the ground, whether dirt or metal, dematerializes
into this wave."
**The canon (one clean physics step deeper):** light strongly coupled to matter is a
POLARITON — a light-matter hybrid (real physics). At full surge the photon fluid ENTRAINS
nearby matter: floor plates, grit, sand, water — whatever is local — hybridizes into the
luminous fluid grain by grain and flows up into the wave. On the ebb it RE-DEPOSITS,
restored. The ground is never destroyed; it is BORROWED. Consequences:
- **Per-world crossing identity for free:** a sea gate makes a water-tide, the Moss arena
  gate makes a metal-tide (plates peeling up in spirals), a desert gate makes a sand-tide —
  every world's crossing is made OF that world. (WC identity machinery gains a gate flavor
  axis at zero extra systems — the entrained material = the world's palette.)
- **The inhale gains its image:** before the crest rises, a ring of ground around the gate
  LOOSENS — grit lifts, plates tremble and peel, debris floats (the arena keepers' floating
  debris, now canon) — the anti-grav held breath.
- **The restore is the grace note:** after travel, everything settles back — the last few
  glowing grains raining down onto a floor that reassembles. (Non-destructive = kid-safe awe,
  and no persistent world damage to manage.)
**Feasibility (honest):** authored "loose rim" pieces around gate floors (small separate
meshes that lift/spiral/dissolve — the debris-pool machinery reused), a dissolve ring on the
ground texture (mask + emissive edge, CP-3-style — no custom shader required for v1:
tint+sink+particles sells it), particle streams within F3.5 caps. The crest itself unchanged
from §2.

## §2 — The visual language (vs. Stargate's vertical puddle)

- **The resting gate:** between the pillars, the sea is DIFFERENT — glassier, faintly deep-lit
  from below, slow-motion ripples (slow light: even waves obey the fiction). At night it's the
  brightest stretch of the glowing horizon.
- **THE INHALE (our kawoosh):** the water between the pillars draws BACK into a trough — a
  held breath — then the crest RISES: a standing wave that never breaks, spanning the ring.
- **The crest:** layered like real surf made of light — deep teal base → crest-cyan body
  (the canon 0.85, 0.98, 1 family) → a top edge of pure white light in place of foam; spray
  droplets that DISSOLVE INTO GLOW as they leave the membrane (light un-slowing back to
  ordinary light — the tell that this is not water).
- **Departure:** the crest sweeps through the traveler/dock/ship; the world goes crest-bright
  (the existing flash shell = the inside-the-wave moment, now with fiction); arrival = the ebb
  receding around you in the destination's water.
- **Scale + sound:** the crest is TALL (pillar-height); its sound is the ocean's held breath —
  low surge + a crystalline shimmer over it (SFX family: `tide_surge` + `gate_shimmer`).

## §3 — Feasibility (honest; the concept guides, the budget governs)

The upgrade path for `ZiptideGateEffect` is layered cheap tech we already own: ZiptideWater's
normal machinery on curved crest planes (2–3 transparent layers, scrolling at two rates) +
emissive gradient (teal→cyan→white) + spray as F3.5 particles under existing caps + the
current flash shell retained as the inside-the-wave beat + the trough/rise as vertex
animation on ONE mesh. No refraction, no sim — a SCULPTED wave, art-directed, device-proven
against the concept. Post-checkpoint envelope; transparent-coverage budget (≤35% brief
authored reveal, FORGE IV) is the governing rail.

## §4 — ✅ KEEPERS LANDED (2026-07-20, `concepts/ziptide_gate/` — five images = the gate's LIFECYCLE)

- **`gate_standing_crest_hero_v1`** — **THE CROSSING IDENTITY** (matches §2 exactly): the
  towering held crest between weathered pillar clusters, teal→cyan→white-light foam, the
  TROUGH visibly pulled back at the base (the inhale sells "the sea is doing this"), spray
  dissolving into glow-motes, a tiny rusted craft dwarfed before it. Canon read for every
  ordinary crossing.
- **`gate_resting_pillars_v1`** — the RESTING state + **pillar material reference**: the
  stone is SOAKED in tide — glowing cyan speckles/lichen on the monoliths, luminous drips,
  glow patches in the water. **ADOPTED MOTIF: the pillars are stained by the tide** (photon-
  fluid residue on stone — ties the night-glow family to the architecture; feeds the pillar
  recipe's emissive mask).
- **`gate_spiral_ship_crossing_v1`** — the tide at FULL REACH: a sky-filling spiral of
  luminous water with THE SCRAPPER riding it (design consistency held again). Ruling: the
  no-swirl law stands for the standard crossing read — but this is adopted as **the
  SHIP-crossing seen from outside**: when a whole vessel rides, the tide climbs the sky.
  Rare, earned, trailer-grade (the Director's Cut first-crossing exterior beat).
- **`gate_arena_column_v1` + `gate_arena_inhale_v1`** — a NEW architecture the generation
  proposed: a double-ring stone colonnade "gate arena" with a machined circular floor —
  column-of-tide surge state + a thin-beam inhale state with debris floating (anti-grav
  moment). **⚖ FORK FOR TERRY:** adopt as a SECOND gate class? (e.g., the Moss transport
  hub's built-up civic gate = arena form; wild gates in open water = pillar-ring form. Two
  tiers of gate architecture, one phenomenon.) Pending his call; both stored either way.

- **`gate_entrainment_hero_v1`** (✅ same-day) — **THE ENTRAINMENT REFERENCE, nailed first
  try:** deck plates peeling up in ORDERED luminous streamlines (called, not blasted), edges
  glowing cyan as metal hybridizes into light, the inner floor ring unbuilt down to glowing
  seams, weightless debris, the freed material feeding the rising column's base. The §1b law
  as an image; the ZiptideGateEffect upgrade's surge-state money reference.

**Lifecycle assembled — COMPLETE:** resting glass-water → inhale/trough (debris lifts) →
ENTRAINMENT (the ground borrowed into the fluid) → the crest (person-scale crossing) → the
spiral (ship-scale crossing) → ebb/restore. Every state the effect upgrade needs now has an
approved reference. Still optional: the RESTORE grace-note shot (grains raining onto a
reassembling floor).
