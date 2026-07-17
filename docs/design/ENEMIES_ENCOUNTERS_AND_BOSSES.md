# 👾 ENEMIES, ENCOUNTERS & BOSSES (the disable + salvage combat sandbox)

**Status:** DESIGN (Round 6 research synthesis). No code changed by this doc. Companion to
`WEAPON_FEEL_AND_ARSENAL.md`, `POWERS_MOBILITY_AND_EQUIP.md`, `SHIP_DESIGN_INTERIOR_EXTERIOR.md`.
**Extends the existing enemy stack — does not fork it.** Maps onto `DroneRuntime`, `CreatureRuntime`,
`CreatureBehaviorBase` (its `Tick`/`OnStunned` seam), `IShockable`, `EcologyDirector` /
`EcologyPressureLedger`, `RewardRouter`, `TargetRuntime`, `HitZones`.

Grounded in Halo, Doom Eternal, Left 4 Dead, Luigi's Mansion, Shadow of the Colossus, Cuphead, Pikmin,
Rain World, and VR (Alyx, Robo Recall, Space Pirate Trainer, Until You Fall, Beat Saber).

---

## 0. The core finding — ONE state machine is the atom of the whole combat game
Everything hangs off a single per-enemy loop: **DISABLE → STUN → CAPTURE → SALVAGE.** Weapons *create*
the opening; the gravity glove / capture-net *closes* it; salvage is the reward. This one atom drives:
weapon relevance, the glove's weight puzzle, encounter pacing, boss stages, and even the ecology.

**The sandbox law (Halo):** a rich combat game needs only ~**7 enemy ROLES**, not a big bestiary —
variety comes from *mixing roles × your weapons × the space*, not from more monsters. Doom Eternal:
every enemy has a weak point a specific tool answers, so your whole arsenal stays relevant.

---

## 1. The shared enemy STATE MACHINE (alive + fair)
Every enemy runs ONE readable machine, flavored per species:
**IDLE/PATROL → NOTICE → ALERT → APPROACH → ATTACK → RECOVER → STUNNED → DOWNED(salvage).**
- **Fairness = telegraphs between states, not slower enemies.** Each transition needs a ~0.4s tell a kid
  reads across a room: a color/emissive shift, a wind-up pose, an audio sting, a head-snap (`FaceToward`).
- **NOTICE is its own beat** before ALERT — the enemy *looks*, a "?" cue fires, a grace window to hide or
  hit first (Alien Isolation's readable escalation).
- **Predictable trigger, unpredictable detail** (Halo): Grunts *always* flee when their leader dies — you
  just don't know where. Legible AI that "performs" so the player reads intent.
- **Group behaviors = roles reading a shared blackboard** (F.E.A.R./Halo): swarm (converge), flank (an
  APPROACH that arcs to your side), protect-the-leader (guards orbit a Warden; stun the Warden → guards
  drop to IDLE — a legible priority target).
- **Richness-bar floor:** no enemy ships with one idle loop. Every species = a filled vocabulary
  (notice/alert/recover telegraphs, a stun-flail, a resist tier).

---

## 2. The DISABLE + SALVAGE model (why the glove + every weapon matter)
Two data axes on every enemy make combat a puzzle, not a stat check:
- **Weight tier — Light / Medium / Heavy.** The gravity glove grabs Light freely, drags Medium slowly,
  and **cannot lift Heavy until it's disabled.** Weight is a puzzle, not a number.
- **Armor tier — Bare / Plated / Shielded.** Shielded enemies *ignore* the taser stun until the shield is
  stripped (prism beam / static net) — Luigi's "quench the element / remove the mask," Doom's "break the
  shield first."

**The capture skill-window (Luigi's Mansion tug-of-war):** a hit fills a **stun meter**; at full →
**STUNNED** (grabbable, cyan). STUNNED *performs urgency* — the enemy **flails, sparks (`SpawnArc`), and
runs a shrinking RECOVER timer** (ticks faster the longer it's down) before re-forming to ALERT. Grab it
and it **pulls away from your grab vector** — you yank counter to its lean. Light pops free; **Heavy
RESISTS** (Robo Recall's white weak-dots) — strip a panel first, land a second stun, or freeze the recover
timer with stasis. Win the tug → **DOWNED** salvage husk (`RewardRouter.Grant`). Miss → it recovers and
you *feel* it. The peak sandbox moment = a Heavy+Shielded enemy forcing the full rotation: **strip → stun → grab.**

---

## 3. The ROSTER — 7 roles, a 10-bot starter set, and cheap variety
**The 7 roles (the rich-sandbox floor)** — each demands a different tool:
fodder (swarm; taser→grab) · ranged harasser (taser interrupts its charge) · rusher (snare/thumper the
dash) · shielded/tank (strip shield → then grabbable; Heavy) · flyer (thumper knocks it down into glove
range) · support/healer (net its tethers; priority) · disruptor (capture-net the harmless pop).

**Charming starter roster (kid-friendly, readable):** Buzzbit (fodder flyer) · Peekbot (ranged) ·
Scuttle-crab (rusher) · Clunker/Turtletank (shielded tank, Heavy) · Propa-pal (flyer) · Mendy (healer) ·
Puffo (disruptor) · Magno-mite (Medium fodder variant) · Sparkler (elite harasser) · Lugnut (mini-boss).

**The AFFIX system — extend a tiny roster across 80 worlds with NO new art (data-only):** composable tags
on the enemy definition — *Shielded* (armor tier), *Jumbo* (scale + weight tier up), *Charged* (resists
taser), *Leader* (buffs fodder; stun it → they panic), *Frosted/Sparky* (needs a specific tool). One base
drone + 4 affixes ≈ **16 readable encounters.** Escalate by adding affixes + remixing spawn tables per
world tier (reuses the existing `difficultyTier` rescale), never by modeling new enemies.

---

## 4. VR ENCOUNTER FLOW (the fight orbits a roughly-stationary player)
- **The standoff dome:** the player is the fixed anchor; enemies hold a shallow dome ~**5–8m** in front
  (your `DroneCombatBehavior` already does standoff 5.5 / leash 9). Per-drone phase offsets so packs read
  *alive*, not cloned.
- **Spawn front-180°, escalate to the sides — never pop in behind the head.** Flankers *travel* into
  peripheral vision (you see them cross). Alyx never jump-scares and cut fast closers — **slow closers = fair.**
- **The 3-beat telegraph** (the most visceral moment in the game): ① wind-up (brighten to telegraph cyan +
  rising tone) → ② **an aiming line drawn to the player's chest for the last ~0.5s** (Robo Recall — turns
  "a bolt is coming" into "lean *left*") → ③ a slow, dodgeable bolt (`StunBolt` ~6.5) you can duck or
  **swat/deflect with the gravity glove** (deflection = the best close verb — reward it). Melee creatures
  get a big wind-up pose + a floor telegraph zone.
- **Comfort ceiling (kid-safe, nausea-safe):** ZERO forced player motion (intensity comes from *incoming*
  things) · nothing fast un-telegraphed · cap ~**4–6 active threats, only 1–2 firing at once** · downed
  enemies **spark, wobble, power off** (comedy, not death; no jump-scares/darkness-lunges).
- **Pacing — waves with salvage downbeats (FREE):** calm → build → spike → **the spike ends when the last
  enemy goes down**, and now the floor is littered with stunned/downed drones to glove + salvage. **That
  salvage sweep IS the breather** — the next wave waits until cleanup is mostly done. The disable+salvage
  loop hands you the arena rhythm for free.
- **AI Director** (a lightweight pacing layer beside `EcologyDirector`): reads player stress, gates
  ALERT-count and swarm spawns — Left 4 Dead's build-up/peak/relax, tuned gentle for kids.

---

## 5. BOSSES — disable-in-stages puzzles, not damage sponges
Principles: a boss is **a sequence of small legible problems** (not one HP bar); **every phase telegraphs
its own solution** (Cuphead); the **weak point is somewhere you must reach** (Shadow of the Colossus climb);
**each stage strips a part you salvage** (progress is physical + visible). Concepts:
- **The Salvage Titan (grapple-and-climb):** bait a stomp → a joint glows → grapple onto its leg, ride up,
  stun a shoulder node, PULL the plate (salvage), repeat up the body. Comfort: anchored/stationary-relative
  while riding — vection-safe.
- **The Hive-Warden (protect-the-healer, scaled up):** a swarm rebuilds a core; sweep the swarm with a power
  to expose the core, stun it, grab a battery, it re-swarms weaker. Three cycles = disassembled.
- **The Mimic Vault (puzzle-box):** a "treasure chest" boss — open a panel, it snaps shut and skitters;
  each face has a latch you stun in the right *order*. Pure disable-logic, zero threat spikes — ideal for
  the youngest players.

---

## 6. LIVING-WORLD CREATURES (charm + ecology)
`EcologyDirector` already does time-of-day casts + nests, and `EcologyPressureLedger` already models
over-hunting. Lean in with **non-combat fauna that give worlds a pulse:** grazers that scatter when you
sprint (`LightGrazer` exists), perchers that land on your glove if you hold still, curious burrowers,
glow-schools that part around you. They **coexist with salvage:** a downed enemy draws scavenger critters;
over-hunting a zone *visibly* thins it (surface the pressure ledger). **Reward kindness** — feed/pet/
photograph → drops cosmetic salvage. Charm, not just combat.

---

## 7. NOVEL VR enemy/boss ideas (Quest-feasible, kid-friendly)
- **The Judo Drone** — turns YOUR gravity glove against you: as you yank, it lets go and slings *toward* you;
  you must release-and-redirect (skill, not damage).
- **Ride-the-Beast boss** — grapple a fleeing giant, hang on through bucks (fixed relative frame = comfort),
  calm it by pulling stasis-pins along its spine; it becomes a **mount, not a corpse.**
- **Broom-the-Swarm** — a cloud you literally *sweep* with a wide-arc power gesture into a collector (Pikmin-reverse).
- **The Puzzle-Box Salvager** — an enemy that IS a sequence of latches; disabling it *is* solving it, and it
  opens into loot.
- **The Prop Mimic** — a scene object that animates only when your back's turned; a "gotcha" stun revealing a
  shy salvage critter, not a scare. Kid delight.

---

## 8. THE FIRING RANGE (weapon testbed + kid play space) — full spec
A safe world scene, **no combat pressure, no fail state** (also the Phase-1 weapon-feel testbed; GPT's lane):
- **Static target lanes** at 3/6/10m — reuse `TargetRuntime` (steel "ding" + light-up).
- **Moving targets** — a rail sliding L↔R at 2 speeds + pop-up targets on timers.
- **Spawnable dummy enemies** — dummy `DroneRuntime` with **`respawnDelay > 0`** (already supported: down,
  spark, pop back) so kids test every weapon + the glove grab endlessly. "Spawn drone" + "clear all" levers.
- **THE PROOF WALL:** spawn **one of every role at every weight/armor tier, labeled**, so the player (and
  Terry) verify the whole matrix on one screen — "taser stuns fodder, prism strips the Clunker, glove can't
  lift Heavy until down, thumper drops flyers." This is what proves the sandbox before content scenes exist.
- **Fun:** hit-counter, accuracy %, a timed **"bust-the-wave"** mini-game (spawn 10, beat the clock),
  knock-down reactive targets (hit-zone physics make these delightful), friendly beeps on streaks.

---

## 9. Architecture mapping (additive, data-driven)
- **One shared `EnemyState` enum** on `CreatureBehaviorBase` (promote the ad-hoc `DroneRuntime.State` /
  `WardenState` / `ChargeState`) carrying the §1 vocabulary + a `[SerializeField]` telegraph duration per
  transition. Subclass `Tick` switches on it — **additive; existing behaviors keep working.**
- **Capture loop:** add a **STUNNED-grabbable** state between the current stun and `Disable()`; a parallel
  **`IGrabbable`/`ICapturable`** (weight-gated) so the glove + capture-net talk through interfaces, not type
  checks (keeps `Core` clean). Extend the existing `OnStunned` with a **recover timer** + a **`grabResistTier`**
  read from `CreatureDefinition`. Win → `Disable()`/`RewardRouter` salvage as today.
- **`CreatureDefinition` (data only):** add `role` enum, `weightTier`, `armorTier` (+ `shieldStripWeapons`).
  A Shielded creature's `Shock()` early-outs (like the existing `!shockable` guard) until stripped.
- **Affixes = a definition/`CreatureVariantAuthor` list** applied at spawn by tier — no per-enemy code.
- **`EncounterDirector` / wave-spawner** (Core/Content, string-ID spawns, gated on "salvage mostly cleared")
  + the **aiming-line telegraph** (extend `UpdateTelegraph` with a line renderer for the last 0.5s). New.
- **`BossRuntime`** orchestrating an ordered list of **`BossStageDefinition`** SOs (weak-point transform,
  required power, salvage-part id) — reuses `IShockable` / hit-zones / `RewardRouter`. One class + one SO.
- **Firing Range** = a new **content-only world scene** (geometry + `SpawnMarkerRuntime` + `WorldRuntime` +
  dummy prefabs) — additive, low-risk.
- **⛔ Report-only (per CLAUDE.md — device-verify, never auto-apply):** anything spawning enemies *behind*
  the player, any hit/bolt that drives vignette/snap/`PlayerStunReceiver` movement, and all combat comfort
  defaults (spawn-arc limits, chaos ceiling, telegraph timing).

---

## 10. What to prototype first (recommendation)
1. **The shared `EnemyState` + STUNNED-grabbable + `grabResistTier`/`weightTier`/`armorTier`** — the atom
   that makes the glove and every weapon matter (extends `CreatureRuntime`/`OnStunned`).
2. **The Firing Range "proof wall"** — one of every role×tier, labeled, in the testbed. Proves the whole
   rock-paper-scissors matrix (and lets the kids play) before any content scene.
3. **The 3-beat telegraph + one wave** via a minimal `EncounterDirector` (salvage-cleared gating = the free breather).
4. **The affix system** — turn the 10-bot roster into ~50 readable encounters, data-only.
5. Then one **disable-in-stages boss** (the Mimic Vault is the cheapest) + the living-world ecology surfacing.
