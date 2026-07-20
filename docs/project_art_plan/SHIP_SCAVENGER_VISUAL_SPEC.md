# SHIP VISUAL SPEC — the Scavenger, measured from Terry's approved concepts
### The canonical reference for the B+ Forge build (and, later, the Tier-C hero mesh)

**Status:** 🟢 REFERENCE APPROVED (Terry, 2026-07-20) · recipe work is PAPER-PLANNED here;
code + booth loop are post-checkpoint per the freeze.
**Canonical images (stored in-repo):** `concepts/ship_scavenger_mk1/ship_scavenger_hero_v1.png`
(dock beauty shot) · `concepts/ship_scavenger_mk1/ship_scavenger_ortho_sheet_v1.png`
(front/back/left/right orthographic sheet). These two images outrank any text description,
including this one — when in doubt, look.
**In-fiction designation on the sheet:** "SLV-01 SCRAPPER" (AI-generated; ⚖ Terry — keep as
the ship's model designation? It's good, and "Scrapper" reads kid-friendly).

## §1 — Rubric verdict (why these keepers passed)

Fill-it-black ✓ (nameable from outline: cab + engine cluster + claw + legs) · one dominant
mass ✓ (the engine cluster) · every detail claims a function ✓ (radiators, pipes, floodlights,
hydraulics) · salvage-yard grammar ✓ (mismatched plates, one builder's hands) · canon match ✓
(truck cab w/ panoramic glass, oversized engine, articulated claw, chipped hazard amber,
floodlights, ONE cyan power port = the coupler, wide stable legs, thrust logic honored) ·
kid-drawable ✓.

## §2 — Measured proportions (ratios off the ortho sheet; ignore the sheet's printed
dimensions — they're generation noise ("700M"/"32MM"). Scale is OURS to set: ⚖ propose
**~18 m length** to fit the berth and read big-but-personal in VR; `ShipDefinition.hullSize`
is the consumer.)

Working in fractions of total LENGTH (side view, nose→nozzle):
- **Nose block** (sloped, headlights): 0.00–0.18 · lower half of hull height, slopes down-forward.
- **Cab** (the truck cabin): 0.10–0.30 · sits ON TOP of the hull line, its roof is the ship's
  highest point (breaks the roofline — keep); panoramic front glass + 2 side-door windows,
  teal-lit interior; small antenna mast.
- **Mid hull** (the working body): 0.18–0.65 · boxy, slightly wider than tall; roofline
  carries machinery blocks + intake grilles; flanks carry radiator panels, pipe runs, access
  hatches; belly tucks up behind the nose.
- **Engine cluster** (the dominant mass): 0.62–1.00 · full hull height and slightly taller;
  reads as a wrapped/banded drum from the side. **Back view is the money shot: ONE large
  central nozzle + FOUR smaller nozzles in a quincunx** (top-left/top-right/bottom-left/
  bottom-right), all recessed in a heavy collar. Armor bands wrap the drum diagonally.
- **Claw arm:** shoulder-mounted at ~0.12 on the LEFT flank only (asymmetry is canon);
  3 segments + 3-finger claw; folded pose hangs the claw just below belly line, reaching
  slightly forward.
- **Landing legs ×4:** hydraulic 2-segment (upper strut + piston), splayed wide; feet are
  broad flat skids; front pair mounts ~0.25, rear pair ~0.70; stance width > hull width
  (front view shows daylight between legs and hull — keep, it's the "stable workhorse" read).
- **Height ratios (side):** hull body ≈ 1.0 unit; cab adds ~0.35 on top; legs add ~0.45
  below; engine drum ≈ 1.15.

## §3 — Palette & material story (maps to existing Forge machinery)

- **Base:** weathered blue-grey steel, MISMATCHED plate values (the buildingModule low-freq
  wash + tintJitter machinery, applied per-plate) + rust streaks at seams/bolts (existing
  streak bake).
- **Hazard amber:** chipped diagonal striping on the nose cheeks, leg struts, and 2–3 flank
  patches (new albedo style entry: `HazardStripe` — declared faces only).
- **Lights:** floodlight clusters (2×2 warm-white GlowPanels) on nose and mid-flank; cab
  windows teal-emissive (interior glow); small red/amber marker dots.
- **THE cyan power port:** one recessed glowing socket mid-hull left (the artifact coupler —
  the ship's single crest-cyan accent, LIVE emissive submesh so story states can drive it).
- **Engines:** darker gunmetal drum, near-black nozzle throats (subtle amber inner glow slot
  for later thrust states).
- Material split per the Tier-B law: worn steel (primary) + gunmetal engine + emissive
  submesh. One primary material + focal emissive = inside the Forge's own rules.

## §4 — The B+ recipe plan (`ship_scavenger_mk1`, hull class ≤15k tris — op decomposition)

| Group | Ops | ~Tris |
|---|---|---|
| Nose block | frustum (slope) + light housings + hazard faces | 1.2k |
| Cab | box + inset window panes (emissive) + light bar + antenna capsule | 1.8k |
| Mid hull | main box + roof machinery boxes ×3 + flank radiator insets + pipe runs (thin capsules ×6) + hatches (texture) | 4.0k |
| Engine cluster | main drum (cylinder + torus bands ×2) + collar frustum + central nozzle (cylinder+frustum) + 4 outer nozzles + rear greeble boxes | 4.2k |
| Claw arm | Limb chain ×3 (the creature-leg op, reused!) + wrist cylinder + 3 finger frustums | 1.6k |
| Legs ×4 | upper strut capsule + piston capsule + skid box each | 1.6k |
| **Total** | | **~14.4k** ✓ |

**Sequencing:** ① silhouette pass (blockout of the seven groups, booth side-view vs. the
ortho sheet overlaid — silhouette must match before ANY detail) → ② back-view engine pass
(the quincunx is the signature; get it exact) → ③ texture pass (plates/hazard/rust/lights) →
④ claw + leg articulation-friendly pivots (the arm should be poseable later; legs static) →
⑤ the cyan port emissive + hero verdict vs. the beauty shot. Each pass = one booth
turnaround verdict, per the taser/warden method. Post-checkpoint; the plan above is the
whole brief a session needs.
**Integration:** visual child under the existing `ShipHullBuilder` root via ForgeModuleLook
(scene keeps graybox contract; device shows this). LOD note: THIS MODEL is the future
distant/berth LOD when the Tier-C hero mesh lands — build clean at 14k, decimation to LOD1/2
comes with FORGE IV CP-2.

## §5 — Open items

- ⚖ canon scale (~18 m proposed) · ⚖ "SLV-01 Scrapper" as official designation · Terry to
  paste his final winning PROMPT into `concepts/PROMPTS_LOG.md` for the record (the evolved
  variant of the rb62 base) · cockpit/quarters interior sheets (S4/S5) still open in the
  concept queue — next generation session.
