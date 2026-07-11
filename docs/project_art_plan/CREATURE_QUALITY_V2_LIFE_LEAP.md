# CREATURE QUALITY V2 — THE LIFE LEAP (art-lane follow-on program)

**Status: PLANNED, NOT STARTED. Gated behind device-testing FORGE II + FORGE III first.**
**Author: Picasso (Opus 4.8), 2026-07-11.** This plan captures the CREATURE half of GPT-5.6's
Post-Fable packet (`docs/GPT_ADDITIONS/2026-07-10_Post_Fable_Handoff/POST_FABLE_ARCHITECTURE_AND_PICASSO_PACKET.md`,
§3–§4) as executable art-track envelopes. It is a **sub-program of the art track (SPRINT_ART)**,
not a new roadmap-of-record — the master order still lives in `PRIORITIES.md` / `GAME_PLAN.md`.

> **The thesis (packet §3.1):** the creature pipeline is a strong procedural v1/v2 — genomes,
> organic ops, textures, tells, one skinned renderer, role gaits, breathing, the tell bridge, the
> photo loop. Its ceiling is that a believable creature is not "more segments" — it is a
> **coordinated package of anatomy + locomotion + contact + perception + behavior + sound +
> environment.** V2 builds that package behind data contracts a smaller model cannot mis-assemble.

---

## §0 — WHY THIS IS A SEPARATE PROGRAM (and the rails)
- **FORGE III = environmental cohesion. CREATURE QUALITY V2 = organisms.** Keep them independent so
  neither balloons. (FORGE IV stays reserved for diegetic UI art — do not renumber into it.)
- **Do not start until Terry has device-tested what's already built** (FORGE II creatures + FORGE
  III cohesion). The first V2 commit is a *review artifact*, precisely so we can SEE the current
  ceiling before changing anything.
- Same laws as every art program: decision-free envelopes, stated ranges, a checkpoint per
  envelope, "do-not" rails, the 3-red circuit breaker, `.meta` per file, boards in the same commit,
  `ZIPTIDE:` logs, no scene/prefab YAML.
- **The governing law (packet §5 P2 / §7):** promote completeness warnings to BLOCKERS only after
  ONE pilot species passes clean. Never global budget raises for one hero. Imported/hero prefabs
  NEVER own health, AI, targeting, rewards, saves, or networking — gameplay knows only the
  species/creature ID.

## The pilot, chosen up front (packet §4 + §6)
**Tidal Carillon (W010 Tidal Array)** — a tall THREE-LEGGED resonant anchor-organism, translucent
pressure bladder in a mineral rib cage, sequential nerve-line tell, conductive foot pads. It is
deliberately **not a biped** and its planet adaptation (anchor + pressure-chord) changes how it
moves — the hardest test of the architecture, per §6. Every envelope below is validated against it.

---

## The envelopes (Picasso's highest-value sequence, packet §6 — ordered)

### V2.1 — CREATURE REVIEW CONTACT SHEET (do this FIRST, before any behavior change)
CI artifact like the photo booth but for MOTION: render each creature through a fixed state cycle
(idle → walk → turn → alert → attack telegraph → apex → recover → stun → disable) as a labeled
turnaround strip + a per-state pose row. This is the RATCHET a lesser model can see and improve
against, and it shows the current ceiling before we touch it. **Gate:** the sheet builds in
forge-photos for every genome; **Do-not:** no gameplay change in this envelope. **Budget:** 2 commits.

### V2.2 — `CreatureSpeciesDefinition` PASSPORT + completeness audit (no behavior change)
One canonical species asset (evolve/wrap `CreatureDefinition`, do NOT replace it) binding: identity/
fiction (family, evolution reason, non-lethal disable, counters, tier Ambient/Standard/Signature/
Hero) · gameplay (stats, behavior, encounter role, ecology, habitat tags) · visual (source kind
Forge|ImportedHero, body/prefab ID, silhouette class, surface family, tell profile, **socket map**:
head/gaze/mouth/weak-points/feet-anchors/VFX/audio/grab, LOD tier) · motion/audio profile IDs.
`SPECIES_INCOMPLETE` audit = WARN (→ blocker after the pilot passes). **Budget:** 2 commits.

### V2.3 — `CreatureMotionIntent` seam (gait fallback preserved)
Neutral signal populated by gameplay behavior, read by visuals: localVelocity/accel, angularVel,
desiredMoveDir, grounded, surfaceNormal, movementMode (Ground/Wall/Ceiling/Fly/Swim/Burrow),
behaviorState (Idle/Alert/Hunt/Flee/Feed/Social/Attack/Recover/Stunned/Disabled), actionPhase 0..1,
alertness 0..1, stunAmount 0..1. **Laws:** gameplay owns intent, visuals own pose; missing signals
degrade to today's speed-derived gait; pure math EditMode-tested. **Coordination:** the *producer*
is a gameplay behavior seam — announce in HANDOFF; the *consumer* (motion) is mine. **Budget:** 2 commits.

### V2.4 — the P4 remainder folds in here: look-at · stun-droop · distance LOD
(Already specced as FORGE III F3.10 — it lands in V2 instead, now that MotionIntent exists to drive
it properly: head yaw ≤30° toward the player within 8m; stun → limbs sag 15° + breath ×2; beyond
40m swap to the frozen-statue clone with hysteresis.) **Budget:** 2 commits.

### V2.5 — CONTACT RIG: ground-contact / foot-lock proof on `swarm_bug`
Tiered `CreatureContactRig` (raycast + bone budget only, NO ragdoll): foot/anchor markers from the
visual provider · raycast only during planted phases · short foot-locks · ankle correction within
strict angle/length limits · body height/pitch follow an averaged support plane · turn-in-place
kills moonwalking. Prove on the six-legged `swarm_bug` first. Wall/ceiling + tentacle variants are
later envelopes. **Gate:** feet plant, don't slide, on a slope in the review sheet. **Budget:** 3 commits.

### V2.6 — `CreatureMotionProfile` — species motion vocabulary (curves, not AnimatorControllers)
Closed profile of curves/ranges for the minimum state vocabulary (idle · locomotion · turn · alert ·
telegraph · apex · recovery · stun · disable · one ecology action). Pure evaluators layered over
today's role sines. **Do-not:** never a per-species AnimatorController web. **Budget:** 2 commits.

### V2.7 — SECONDARY MOTION (spring-bones): the body gets mass
Cheap spring layer for tails/antennae/ribbons/sacs/fins/loose plates, driven by MotionIntent
accel + angularVel (stiffness/damping/gravity/max-angle/lag). Tiny fixed chain count, disabled at
distance. Replaces "all motion is a sine." **Budget:** 2 commits.

### V2.8 — TELL PROFILE + VOICE GRAMMAR (read state without an HUD)
Expand the tell bridge to `CreatureTellProfile` (sensor gaze limits, idle emission, alert transition,
wind-up pulse/expansion, weak-point reveal, stun, disabled, faction/Signal reaction, jaw/throat, VFX/
audio IDs per transition) + `CreatureVoiceProfile` (a xenomorph-class creature is remembered by
SOUND as much as shape). Voice = coordination with the audio lane. **Budget:** 2 commits.

### V2.9 — HABITAT AFFORDANCES + planet specificity
`HabitatAffordance` tags stamped from existing layout/POI data (Ground/Wall/Water/Air, Dark/Warm,
Conductive/Resonant, Nest/Perch/Feeding/Basking, Machine/Garden/Bloom/Canal…). Creatures choose
among compatible stamped points — they never search the scene by name. Species passport declares
required native affordances + one world-condition response. **Coordination:** stamping touches
world builders — announce. **Budget:** 2 commits.

### V2.10 — THE IMPORTED-HERO ESCAPE HATCH (behind the same contract)
Neutral visual contract + two providers: `ForgeCreatureVisualProvider` (default — ambient, morphs,
CI, catalog) and `ImportedCreatureVisualProvider` (Blender/Tripo/contractor/scan). Both emit the
same runtime outputs (renderers, bone map, sockets, weak-points, contact anchors, tell channels,
LOD, cleanup). Imported validation: meters/+Y/+Z, shader+material caps, tri/texture/bone budgets by
tier, required socket names, collider PROXY (never mesh-collider gameplay), LODs, NO gameplay-owning
AnimatorController. **This is the Hero path Terry can spend real art budget on later.** **Budget:** 3 commits.

### V2.11 — SPECIES FAMILIES + one PLANET MORPH proof
`SpeciesFamilyDefinition` (topology, locomotion grammar, perception concept, niche) +
`PlanetMorphDefinition` (proportions/appendages, materials, adaptation organ, palette, affordances,
physics-tied behavior twist, size/stat ranges). Derivation HELPERS (high-g → low/wide; low-g → long
limbs/sails; dense atmo → membranes; dark → declared alt-sense; corrosive → protected joints;
resonant → vibration adaptation) check presence/compatibility — a human/model still authors the
form. ~8–12 families, morphs per family, 1–2 Signature per chapter. **Budget:** 3 commits.

### V2.12 — THE PILOT END-TO-END: Tidal Carillon, then promote the gates
Build the Carillon through EVERY contract above (three-legged delayed-tripod gait, pressure-bladder
breath, sequential nerve tell, foot contact anchoring, disabled curl, resonant-chord voice, W010
affordances). When its review sheet reads without labels, **promote `SPECIES_INCOMPLETE` +
completeness warnings to BLOCKERS** (packet §5 P2.8 — only after the pilot proves the rule).
**Budget:** 3 commits + iteration.

---

## Creature quality gates (packet §3.13 — what the ratchet enforces, per Standard+ species)
Body source + every required socket valid · budget-utilization above the Signature floor (unless
waived) · silhouette readable in 3 registered views · asymmetry OR a documented symmetry reason ·
≥2 material-response regions (Signature/Hero) · minimum motion-state vocabulary · telegraph/counter/
stun/disable all implemented · contact anchors for grounded/wall · voice profile · native
affordance/evolution/story tie · LOD/distance behavior · pose/motion contact sheet · device cap on
simultaneous high-tier creatures. **The gate prevents known INCOMPLETENESS; it never judges beauty.**

## What NOT to do (packet §7)
No second world/Forge factory · no bespoke unrelated prefab per creature · no global bone/tri raise
for one hero · imported prefabs never own health/AI/targeting/rewards/saves/net · no per-species
AnimatorController web · no forced VR camera moves for "cinematics" · promote art warnings to
blockers only after a pilot proves them.

## Execution phasing (packet §5)
V2.1 review sheet → V2.2 passport → V2.3 MotionIntent → V2.4 P4 remainder → V2.5 contact (swarm_bug)
→ V2.6 motion profile → V2.7 secondary → V2.8 tell+voice → V2.9 affordances → V2.10 imported hero →
V2.11 families/morph → V2.12 Carillon pilot + gate promotion. ~30 commits; every one green.
**None of it starts until FORGE III is done and Terry has tested the current build on-device.**
