# WORLD PHYSICS VARIANTS — planets that play by different rules
### Gravity, air, wind, medium and anomaly dials per world — rare, story-anchored, VR-comfortable

**Status:** 🔵 RESEARCH + PLANNED — no code authorized (recovery freeze; Terry commissioned
2026-07-17: *"not just low gravity… a little higher gravity, move a little slower, weapon shots
drop a little… other physics or weather aspects that can switch it up — a variant on different
planets that makes sense with the story bible, not something that should be overused."*
**Home:** design/gameplay program with art tie-ins. This doc **absorbs and supersedes DR-6** from
`docs/project_art_plan/DAMAGE_RESPONSE_AND_RUIN.md` (per-world gravity) — gravity becomes one dial
of a closed vocabulary. Physics/locomotion halves are cross-lane (named claims); the *visible*
half (atmosphere, particulate, flora, ballistics VFX) rides existing art systems.

---

## §1 — What exists today (audited in source)

- **16 hazard tags are already CODE:** `SkyAtmosphereCore` implements the SKYSCAPE §4.1
  hazard→atmosphere table (acid, radiation, static, wind, …) — every story-bible world already
  carries a hazard in its header. **The hazard tag is the natural story anchor for a physics
  dial** — `wind` worlds already LOOK windy; this program makes the wind *push*.
- **Locomotion is already profile data:** `LocomotionProfile` (SO) carries moveSpeed/sprint/
  crouch/slide — a per-world multiplier is a data field away, not a new system.
- **Projectile drop is nearly free:** taser darts and thrown nets are real `useGravity`
  rigidbodies (they already drop; nobody tuned it as a feature). `PvpBolt` integrates its own
  velocity manually — the one path that would need the shared ballistics params.
- **Traversal cores are pre-adapted:** `ZiplineCore`/`LiftCore` take gravity as a constructor
  parameter (today always 9.81) — wiring a world value in is by-design trivial.
- **`Physics.gravity` is never written anywhere** — the global seam is clean and unclaimed.
- **The ship flight system** owns its own physics envelope (its own program) — flight is OUT of
  this doc's scope except that world dials must not fight it (claim on entry/exit).

## §2 — Research findings

- **Outer Wilds** is the flat-screen proof that per-planet physics IS world identity: each body's
  gravity changes how you think before you land, and the game stays honest by simplifying
  (one sphere of influence at a time) rather than simulating everything. Lesson: **one readable
  rule per world beats compound realism** — the player should be able to say the rule out loud.
- **Lone Echo / Echo VR** proves the surprising VR result: zero-g hands-first movement is among
  the most comfortable locomotion schemes ever shipped, because motion is **self-generated,
  constant-velocity, and vestibularly honest** (you pushed; you glide). Ubisoft's Space Junkies
  work says the same from the other side. Lesson: VR tolerates *different* physics well; it
  punishes *acceleration and imposed motion*.
- **VR comfort literature:** constant velocity ≫ acceleration; world-level rule changes at a
  load boundary are fine, mid-scene rule changes are the sickness trigger; existing comfort kit
  (vignette, snap turn) must keep working identically in every variant.
- **Game-feel literature:** lowering gravity risks "floaty" (weightless-but-cheap) — the fix is
  pairing the dial with FEEL evidence: dust kicked on landing, longer shot arcs, slower debris,
  deeper landing dip. A dial without its tells reads as a bug, not a planet. (This is why the
  art tie-in is mandatory even though the program is design-lane.)

## §3 — THE DIAL VOCABULARY (closed — extend by decision, never per-world hacks)

Every world gets a `WorldPhysicsProfile` (data on/next to `WorldProfile`), all dials defaulting
to baseline. Clamps are the contract; `Validate()` + audits enforce them.

| Dial | Range (clamp) | What it changes | The out-loud rule |
|---|---|---|---|
| **gravityScale** | 0.3–1.5 (baseline 1.0) | `Physics.gravity` at travel-apply; fall net thresholds; jump/dash arcs; debris/chunk arcs; DR impulse law `(g/9.81)^k`; traversal cores; creature gait bob; falling-particle speeds | "Everything falls slow/hard here" |
| **airDensity** | 0.5–1.6 | ballistic drag: projectile range/drop, thrown-item arcs, debris tumble damping; particulate fall speed; sound-design cue (muffled/thin — audio lane) | "Shots carry forever / die fast here" |
| **windVector** | 0–6 m/s steady + bounded gusts | lateral push on projectiles/nets/debris; flora `ForgeSway` bias (exists); particulate streaking (hazard `wind` visual already exists); zipline speed bias along/against | "The wind is a hand on everything" |
| **moveScale** | 0.8–1.15 | LocomotionProfile speed multiplier (walk/sprint/crouch/slide entry) — SPEEDS only, never accelerations | "You are heavier/lighter here" |
| **buoyantMedium** | off / on (thick-air worlds) | debris/props sink slowly with drag; jump apex float; dust hangs; pairs with airDensity high end | "This air is almost water" |
| **anomalyTag** | none / `static_surge` / `mag_lift` / `tide_pull` (closed list, story-gated) | scripted LOCAL physics pockets (see §5) — never whole-world | "In THAT zone, the rules bend" |

**Explicitly REJECTED dials** (recorded so they stay rejected):
- **Whole-world zero-g** — needs Lone Echo-class hands-first locomotion, a full program of its
  own; parked on the Forge horizon list, not smuggled in as gravityScale 0.
- **Time dilation** — VO/audio sync, save timers, and comfort all fight it; a slow-motion
  *moment* is a staged-event (LS-2) trick, not a world dial.
- **Mid-scene dial changes** — the comfort law (§4). Includes "gravity storms."
- **Inverted/wall gravity** — rig, fall net, and comfort assumptions all break; story can do
  this with geometry and staging instead.

## §4 — The comfort constitution (VR — these are laws, not preferences)

1. **Dials apply at travel only.** One legal writer (the travel/arrival seam), applied before
   the spawn settles, restored on exit. Mid-scene, the world's physics are constant.
2. **Scale velocities, never accelerations or curves.** moveScale multiplies speeds; input
   response time stays identical everywhere (the 50–200 ms feel window is sacred).
3. **The head is untouched.** No dial ever alters head motion, camera behavior, or comfort-kit
   behavior (vignette/snap-turn settings identical in all worlds).
4. **The fall net recalibrates with gravity.** `PlayerRigPersistence` thresholds derive from
   gravityScale (a 60 m/s "hard fall" is unreachable at 0.4 g — thresholds become
   `f(gravityScale)`, tested) — the DR-6 coupling, promoted to law.
5. **Deviation must be announced.** RILL gets one arrival line per physics-variant world
   ("Grav's light here — watch your step") + a one-time HUD-free tell (long dust hang on the
   first landing). Players forgive different rules they were told about.
6. **PvP/shared arenas pin to baseline** unless a mode explicitly opts in — fairness beats
   flavor in competitive spaces.
7. **Every dial's comfort is a headset verdict** on at least two testers-worth of sessions
   (Terry now; broader later). A dial that reads as sickness at its clamp edge gets its clamp
   tightened — the never-weaken rule applies to comfort, inverted.

## §5 — Anomalies: the story-gated local exception

Whole worlds keep ONE readable rule; *anomalies* are small authored volumes where a second rule
lives, because the story bible earns it (the Pattern, static fields, tidal machinery):
- **`static_surge`** (hazard `static` worlds): a crackling volume where electric-verb weapons
  arc further/chain, and un-shielded practicals flicker (F3.6 reuse). Ties weapon verbs to
  hazards — the DAMAGE doc's matrix gets a per-world modifier column.
- **`mag_lift`** (industrial/wreck worlds): a column where debris and thrown metal objects rise
  slowly (mass-class filtered — never the player); visually sold by suspended scrap (dressing).
- **`tide_pull`** (tidal worlds, W010-class): a periodic horizontal drift on floating objects
  synced to the LS-5 tide act — the tide as force, not just water level.
Anomalies are authored markers (dressing-author placed), budgeted (≤2 per world), and NEVER
affect the rig — objects only. Player-affecting anomalies are a Terry-level future decision.

## §6 — Story-bible anchoring (proposals ⚖ — story lane + Terry own the final mapping)

The rarity law: **most worlds are baseline.** Target ≤1 strongly-deviating world per chapter,
each deviation paired with its hazard/biome so it feels inevitable, not gimmicky:

| World (bible) | Hazard/identity | Proposed dial | Why it fits |
|---|---|---|---|
| W000 The Drift In | space arrival | gravityScale ~0.6 (first taste, tutorial-safe) | the drift IN — you arrive light |
| W003 Glass Shelf | crystal / thin heights | airDensity 0.6 (shots carry, sound thins) | high shelf, thin air, ringing crystal |
| W006 Mirror Flats | flat / exposed | windVector strong steady | nothing to block the wind on a mirror plain |
| W010 Tidal Array | tidal machinery | tide_pull anomaly + LS-5 acts | the array IS the tide made mechanical |
| W011 The Hum | static / resonance | static_surge anomalies | the Hum made physical |
| one Ch.3+ heavy world | industrial giant | gravityScale 1.25 + moveScale 0.85 | a world that makes you feel small and slow |

## §7 — Envelopes

| Env | What | Lane | Acceptance |
|---|---|---|---|
| **WP-1** | `WorldPhysicsProfile` schema + clamps + `Validate()` + rarity audit (`PHYSICS_DIAL_OVERUSE` warn: >1 strong deviation/chapter) + baseline-identity test (all-default == today, hash-stable) | design/core | pure tests; audit wired |
| **WP-2** | gravityScale seam (ex-DR-6): travel-apply/restore + fall-net derivation + traversal wiring + gait/VFX couplings | **cross-lane: travel/locomotion owners** | round-trip tests; device comfort verdict |
| **WP-3** | unified ballistics params (gravity+drag+wind consumed by darts/nets/bolts alike — `PvpBolt`'s manual path adopts the same struct) | **cross-lane: weapons owner** | trajectory purity tests; range contact-proof in the test alley |
| **WP-4** | moveScale through LocomotionProfile + arrival announcement (RILL line + first-landing tell) | **cross-lane: locomotion owner** + story | comfort verdict; announce-once test |
| **WP-5** | wind as force (steady+gust model, one world-level vector + per-zone overrides ≤2): projectiles/debris/sway/zipline | cross-lane + art | sway/streak visual sheet; gust bound tests |
| **WP-6** | anomaly volumes (closed 3-tag list, object-only, budget ≤2/world) + DAMAGE-matrix modifier column | cross-lane + art + story | volume budget audit; per-tag booth/scene sheet |

Order: **WP-1 → WP-2 → WP-3 → WP-4 → WP-5 → WP-6.** Gravity first (it's the master dial and
already spec'd via DR-6); ballistics before moveScale (weapon feel sells the planet before leg
feel does); anomalies last (they need the matrix + staging machinery around them).
**Gate:** same ladder as everything (recovery → slice → FORGE III close → FORGE IV window);
WP-1's schema + §6 mapping are paper-draftable early, like WC-1 and DR-1.

## §8 — Proof standard

- **The physics alley** (shared with the DAMAGE test alley): fire every projectile down the
  row under each dial extreme; CI contact-sheets the arcs (trail renderers make trajectories
  photographable). A drag/gravity/wind regression is a visible bent line, not a hidden number.
- **PlayMode:** apply/restore round trips per dial; fall-net derivation; baseline identity;
  rarity audit; announce-once.
- **Device:** comfort verdicts per dial at clamp edges (the §4.7 protocol); frame cost of wind
  (it touches many movers — budget it like everything else).
- Logs: `ZIPTIDE: PHYS world=… g=… air=… wind=… move=…` on arrival · `ANOMALY tag=… enter|exit`.

## §9 — Do-nots

- No dial outside the closed vocabulary; no per-world bespoke physics scripts.
- No mid-scene dial mutation; no player-affecting anomalies (v1); no PvP deviation by default.
- No dial ships without its FEEL tells (art) and its announcement (story) — a silent rule
  change is a bug report waiting to happen.
- No compound extremes (a world at BOTH gravity 0.3 AND heavy wind AND thick air is a physics
  demo, not a place — the rarity audit counts deviations).
- The rejected list (§3) stays rejected without Terry + evidence.
