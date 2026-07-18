# DAMAGE, RESPONSE & RUIN — the world answers the weapon
### Destructible · damageable · movable · recoverable, per weapon and per world's physics — in VR, on Quest

**Status:** 🔵 RESEARCH + PLANNED — no code authorized (recovery freeze). Terry commissioned
2026-07-17: *"some objects should be destroyable, other objects should show sign of damage, other
objects should be movable depending on what weapon is used… if you use a gravity gun and blast
rocks they should go flying, but if you blast a building maybe it cracks a window… low gravity
worlds tie into the physics and therefore into the weapons."*
**Home:** this is Constitution **Law 7 — "The world responds"** grown to full size. Its art half is
an **expansion of FORGE IV CP-6** (surface response); its persistence half lands in **Forge V**
(LS-1 states / LS-6 marks); its physics half is **cross-lane** (weapons + locomotion owners, named
claims required). Envelope-by-envelope homes in §7.

---

## §1 — What already exists (audited in source, 2026-07-17)

More is built than the boards suggest — this program EXTENDS, it does not invent:

- **`ReactiveProp` (F3.6) is live:** a closed 4-reaction vocabulary — `LightFlickerOut` /
  `SteamBurst` / `SparkShower` / `Shatter` — with a pure cooldown/one-shot state machine, VFX via
  `VfxFactory` (F3.5 runtime EXISTS), and a hard law already in the code: *structural/navigation
  colliders are never modified*. Shatter spawns 3 chunks through…
- **`WorldDebrisBudget`:** pooled debris, **≤24 live chunks, 4.5 s lifetime** — the debris rail is
  already a budget, exactly the house pattern.
- **The weapon verb set:** taser darts, scrap pistol, static net, sonic thumper (melee), prism
  beam, and **`GravityGunRuntime`** — today a hitscan "grav pulse" that downs and LAUNCHES drones
  with an impulse. Weapons discover targets through `IPvpDamageable.ReceiveHit(weapon, point, dir)`
  — one interface, every response can hang off it.
- **`RepairableMachine` / job loop:** the *recovery* half of Terry's ask has a gameplay owner —
  repair exists; what's missing is the world visibly answering it.
- **Damage-adjacent art machinery:** CP-3 lived-in masks (wear/heat/repair-patch channels are
  damage art), F3.4 decal baking (`GroundDecal`/`StainAlpha` — a scorch/crack mark is the same
  machinery), FORGE recipes with content hashes (damage stages can be recipe variants).
- **NOT present anywhere:** per-world gravity. `Physics.gravity` is never touched; traversal cores
  (`ZiplineCore`, `LiftCore`) take gravity as a parameter but always receive 9.81; the rig's fall
  net assumes Earth gravity. Low-gravity worlds are a genuinely NEW seam (§6).

## §2 — What the industry does (research findings)

**Half-Life: Alyx (the VR reference):** props carry **health values** and **pre-authored break
pieces** (nested "BreakPieceEmbedded" hierarchies compiled from one source mesh; pieces can
themselves be breakable, with random spawn chances for variety). Crucially, Valve pairs a few
*interactive* physics pieces with many **non-physical mesh particles** — visual shrapnel that
costs almost nothing. Lesson: **authored fracture, tiered cost** — never runtime fracture.
**Boneworks/Bonelab (the full-physics pole):** everything simulated, glass "feels" like glass —
but Valve's stated reason for NOT going there is decisive for us: **VR has no force feedback**, so
simulated resistance the hand can't feel reads as mush. Lesson: favor *readable visual/audio/haptic
response* over simulated resistance; a crisp crack + decal + one chunk beats a wobbly physics pile.
**Quest-specific practice (Meta samples + mobile Unity practice):** pre-segmented meshes with
simple colliders; debris uses **primitive colliders, never mesh colliders**; strict live-rigidbody
caps with pooling (we already have the pool); mesh-swap damage *states* instead of deformation;
decals + material-property tints for marks; progressive removal (shrink/sleep) instead of letting
debris pile up. Deformation, voxel/geo-mod structural destruction, and runtime Voronoi fracture
are off the table on this hardware — and off-genre for this game.

## §3 — The core design: THE RESPONSE MATRIX

One closed, data-driven table answers Terry's whole ask. Every damageable object declares a
**material class** (the CP-3 families: raw/painted metal · stone · glass/visor · polymer · wood/
organic · corroded industrial · rock/boulder · growth/biomass). Every weapon declares a **verb**
(`kinetic` pistol · `electric` taser · `sonic` thumper · `beam` prism · `snare` net · `grav`
pulse). The matrix maps *(material × verb) → response tier + intensity*:

| Tier | Name | What happens | Machinery |
|---|---|---|---|
| **T0** | MARK | scorch/chip/crack decal + puff, nothing else | decal pool (F3.4 bake) + F3.5 VFX |
| **T1** | REACT | the F3.6 vocabulary: flicker, steam, sparks, dim | `ReactiveProp` (exists) |
| **T2** | WOUND | visible damage STATE: crack overlay mask, dented/charred stage swap | recipe damage stages (§4) |
| **T3** | SHOVE | object moves: impulse scaled by mass class and world gravity | physics tier (§5) |
| **T4** | BREAK | pre-authored chunks fly + non-physical shrapnel + T0 marks on neighbors | `Shatter` grown up (§4) |

Rules that make it feel RIGHT (Terry's examples, encoded):
- **Rocks vs. grav pulse → T3/T4:** loose props in the `boulder`/`debris` mass classes take the
  full impulse — they FLY (scaled by world gravity, §6).
- **Building vs. grav pulse → T0/T2 + authored weak points:** structures are immovable and
  unbreakable BY LAW (nav/structural colliders untouched — already ReactiveProp's law); the wall
  takes a crack decal, and only **authored weak points** (windows, vents, hanging signs — placed
  by the dressing/practical authors) are T4-breakable. "You cracked a window" is exactly a weak
  point answering.
- **Verb identity:** electricity makes lights/machines REACT (T1) but barely marks stone; sonic
  shoves loose props hard (T3) but can't scorch; beam marks/wounds (T0/T2) with heat visuals but
  shoves nothing; net snares (its own existing behavior — the matrix records it as no-op vs.
  environment). Each weapon gets a *recognizable environmental signature* — that's the fun.
- **Every cell has an answer.** The minimum legal response is T0 — a shot that produces NOTHING
  is the one forbidden outcome (dead-world tell). Matrix completeness is a CI test.

## §4 — Damage states & breakage through the Forge (the art half)

- **Damage stages as recipe data:** `ForgeRecipeDefinition` gains `damageStages` (0–2 extra
  stages, e.g. `dented` / `wrecked`): baked variant textures (CP-3 crack/char/dent masks composited
  at bake time — same atlas, no new materials at runtime) and optionally a stage mesh delta within
  class budget. Stage swap = the `ForgeModuleLook`/applier machinery. Both stages photograph in the
  booth like any asset — a damage stage that doesn't read in turnarounds doesn't ship.
- **Break pieces as recipe data:** breakable (T4) recipes declare `breakPieces` (≤5 authored
  chunks, generated at bake from the recipe's own ops — the Alyx pattern in Forge idiom) plus a
  shrapnel VFX id. Chunks inherit the parent's baked material; primitive colliders only;
  spawned through `WorldDebrisBudget` (cap may need a device-evidenced raise — measured, not
  assumed). Nested breakability (chunk → sub-chunks) is explicitly OUT (cost, and 3 mid-size
  chunks + shrapnel reads better than 12 slivers at Quest scale).
- **Neighbor splash:** a T4 break applies T0 marks in a small radius (the wall behind the shattered
  lantern gets the scorch) — one raycast ring at break time, budget-capped, sells causality.
- **Persistence:** within a scene visit, damage states persist (component state). Across visits:
  wounds/breaks on *named story objects* become Forge V **LS-6 marks / LS-1 state deltas** (the
  scarred machine stays scarred); anonymous props reset — the world heals ambiently, which is also
  the **recovery** story Terry named. Repairing via `RepairableMachine` REVERSES stages visibly
  (wrecked → dented → clean + repair-patch mask) — recovery is a first-class response, not a
  respawn.

## §5 — Movable: the physics tier (cross-lane with weapons)

- **Mass classes, not per-object tuning:** `debris` (≤2 kg) · `prop` (2–15 kg) · `heavy` (15–80 kg,
  budges only) · `anchored` (∞ — structures, machines, travel doors). Class lives on the recipe/
  item definition; the matrix consumes it.
- **Impulse law:** `impulse = verbBase × matrixIntensity × (g_world / 9.81)^k` with k≈0.5 for
  launch feel (see §6) — one formula, tested pure, no per-weapon hacks. Grav pulse gets the big
  base + upward bias (that's its identity, already in `GravityGunRuntime`'s drone kick — this
  extends the SAME code path to props).
- **Rigidbody budget:** movable props sleep aggressively; ≤8 simultaneously awake non-debris
  rigidbodies per world (audited); waking is proximity/hit-driven. Debris stays under its own
  existing cap.
- **VR honesty (the Valve lesson):** we never simulate resistance the hand can't feel. Push/carry
  of heavy objects is out of scope; SHOVE is ballistic and instantaneous — hit, impulse, done —
  with response sold by sound + haptic tick + arc, not by sustained force simulation.

## §6 — Per-world gravity (NEW seam — cross-lane, Terry-level feature decision)

- **Data:** `WorldProfile.gravityScale` (clamp 0.3–1.5; default 1.0). Applied ONCE per arrival by
  the travel seam (the only legal writer of `Physics.gravity` — audited), restored by the same
  seam. Never touched mid-scene.
- **What it must couple to (each a named claim):** the rig's fall net + jump/fall arcs
  (locomotion owner — the fall-net thresholds in `PlayerRigPersistence` assume 9.81 TODAY and
  would misfire at 0.4 g); debris/chunk arcs (free — they're rigidbodies); the §5 impulse law
  (`(g/9.81)^k` — low-g worlds make the SAME shot fly further and hang longer, which is exactly
  Terry's tie-in); traversal cores (`ZiplineCore`/`LiftCore` already take g as a parameter — wire
  the profile value in); creature gaits (V2 motor — reduced footfall rate/higher bob at low g,
  data-only); VFX gravity modifiers on falling-particle kinds.
- **Comfort law:** player gravity changes are WORLD-level and travel-gated only — never dynamic
  mid-play (VR comfort; the fall net and boot-hold contracts stay deterministic per world).
- ⚖ **Terry decision:** low-gravity as a *world identity* (which worlds? how low?) is a roadmap/
  design call — this program builds the seam and proves it in ONE test world first.

## §7 — Envelopes and where each one lives

| Env | What | Forge home | Lane |
|---|---|---|---|
| **DR-1** | Response matrix data + `Validate()` + completeness tests + per-verb intensity table | FORGE IV CP-6 (expansion) | art |
| **DR-2** | Recipe `damageStages` + baked crack/char variants + stage-swap applier + booth sheets | FORGE IV CP-6 / CP-3 | art |
| **DR-3** | Impact-mark decal pool (budgeted) + neighbor splash + per-verb mark art | FORGE IV CP-6 | art |
| **DR-4** | `breakPieces` bake + Shatter v2 (chunks + shrapnel) + weak-point authoring in dressing/practical authors | FORGE IV CP-6 | art |
| **DR-5** | Mass classes + impulse law + rigidbody budget + weapon wiring | FORGE IV CP-6 window | **cross-lane: weapons owner** |
| **DR-6** | `gravityScale` seam + couplings (fall net, traversal, gaits, VFX) + one low-g proof world | after DR-5 | **cross-lane: locomotion/travel owners** |
| **DR-7** | Repair-reversal visuals + story-object persistence via LS-1/LS-6 | Forge V | art + story |
| **DR-8** | Audits: matrix completeness · debris/rigidbody/decal budgets · `RESPONSE_DEAD_CELL` blocker · device frame proof | rides DR-1..6 | art |

Order: **DR-1 → DR-3 → DR-2 → DR-4 → DR-5 → DR-8 gate → DR-6 → DR-7.** (Marks first — cheapest,
biggest dead-world fix; physics after art because art proves the matrix reads; gravity last
because it multiplies everything before it.)

## §8 — Proof standard

- **The test alley:** a booth-style scene (W002 corner or dedicated proof scene) with one object
  of every material class in a row; contact sheets fire every verb at every column — the matrix
  photographed. CI re-renders it; a cell whose response stops reading is a visible regression.
- **PlayMode:** matrix completeness, budget caps, impulse-law purity, gravity-seam
  apply/restore round trip, persistence round trip.
- **Device:** frame cost of worst case (T4 break + full debris + VFX during combat) measured
  before any budget is trusted; haptic/audio read verdicts are headset-only.
- Log tags: `ZIPTIDE: RESPONSE verb=… mat=… tier=…` · `DEBRIS live=…` · `GRAV world=… scale=…`.

## §9 — Do-nots (the walls)

- No runtime fracture/Voronoi, no deformation, no voxel/structural destruction — authored pieces
  only, structures are inviolate (nav/structural colliders never change — the existing law).
- No per-object bespoke response code — everything routes matrix → tier machinery; a new special
  case = a new matrix cell or a rejected idea.
- No mesh colliders on debris; no nested chunk breakability; no sustained force-push mechanics.
- No mid-scene `Physics.gravity` writes; no per-object gravity except debris arc modifiers.
- No response without a budget and an audit; no tier promoted without its booth/device proof.
- The one FORBIDDEN outcome stays forbidden: a weapon hit that produces nothing at all.

## §10 — Answer to "which Forge does this belong in?"

**FORGE IV owns the art of it** — DR-1..DR-4 + DR-8 slot directly into CP-6 ("surface-aware
impacts and environmental reactions"), which this doc expands from a paragraph into a program.
**The physics of it (DR-5/DR-6) is cross-lane** — same FORGE IV window, but weapons/locomotion/
travel owners hold the pen on their halves, with claims boarded before code. **The memory of it
(DR-7) is Forge V** — damage that persists is a world state, repair that shows is a mark. Nothing
starts before the gate ladder allows FORGE IV; DR-1's matrix + DR-2's stage schema can be drafted
on paper during late FORGE III device passes, same as WC-1.
