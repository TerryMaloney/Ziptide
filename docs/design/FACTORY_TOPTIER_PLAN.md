# THE FACTORY — the pitch, the grammar, and the top-tier plan

**Status:** PLANNING ONLY — Terry-approved direction, 2026-07-15.
**⛔ HARD GATE:** nothing in this document is built until the recovery program completes — R1 exit,
R2 contract consolidation, and the R3 golden slice green on-device. Every envelope here then ships
through the full proof ladder (`PLAYMODE → VISUAL → APK → QUEST`) behind exposure gating, one at a
time. Any model that starts this work before that gate is violating the recovery freeze.
**Written by:** Picasso/Fable 5 (art + design lane), from the 2026-07-15 code deep-dive
(`Gameplay/Runtime/Automation/*`, `Content/Runtime/Automation/*`, `docs/systems/MINING_CONVEYOR.md`)
and Terry's direction: *"pitch this as if it were the game itself — make it top tier."*

---

## 1. THE PITCH

You land on a strange shore with empty hands, and somewhere up the beach a rusted drill is still
running — pumping ore into a pile nobody has collected in years. Fifty meters away, a depot stands
with its mouth open and its gauge on empty. A shelf of belt tiles sits between them.

A four-year-old understands this scene: **that thing is making stuff, that thing is hungry, and
these go in between.** No text. No tutorial. You grab a tile, a ghost snaps to the grid under your
wrist, you click it down, and the first chunk of ore rides your first belt into the depot's mouth —
which *swallows* it, rings like a till, and flips its counter to 1. You are now an industrialist.

Three sentences for the publisher:

1. **The only factory game you stand inside.** Every belt is placed by hand, every machine repaired
   with tools you hold, and when the line runs you can grab the conductor's lantern and *ride it*.
2. **It earns while you sleep and gets raided while you watch.** Come home to overflowing hoppers —
   or to swarm-bugs chewing your line while you defend it with the guns on your own belt.
3. **One grammar, eighty dialects.** Every world in the Ziptide system speaks the same machine
   language a child can read — and every region twists it with one new rule, one new resource, one
   landmark machine, so the ten-thousandth belt is still placed somewhere that feels new.

This is Roblox-tycoon accessibility fused with Factorio depth, built native to VR hands — and it is
the economic engine every other Ziptide system (jobs, combat, flight, trade) feeds on.

## 2. PILLARS (the laws every envelope obeys)

- **P1 — Hands, not menus.** Placing, ripping up, copying, stamping, repairing, riding: all physical
  verbs. If a feature needs a floating menu, it's designed wrong.
- **P2 — The kids test.** A new player, zero text, identifies producer / consumer / path within 30
  seconds and completes a working line within 2 minutes. This is the ACCEPTANCE BAR for every
  envelope, tested on real humans (Terry's kids are the QA department).
- **P3 — The payoff is a show.** Money earned invisibly is money not earned. Every unit delivered
  is seen, heard, and felt.
- **P4 — The factory is alive.** It moves when empty, hums when working, groans when jammed, and
  fills the yard when you've been away. It is a character, not scenery.
- **P5 — One grammar, eighty dialects.** The verbs and silhouettes never change between worlds;
  the materials, hazards, and one special rule per region do.
- **P6 — Depth is optional, never required.** The ladder below goes as high as a spreadsheet
  optimizer wants; rung one is always enough to have fun.

## 3. THE LEARNING LADDER (three-year-old → thirty-year-old)

| Rung | Player verb | Who it's for |
|---|---|---|
| 0 | **Watch** — the drill pumps, the pile grows, the depot gapes | everyone, first 30 seconds |
| 1 | **Connect** — one straight belt, first payout chime | small kids; the hook |
| 2 | **Route** — corners, splitters, two lines at once | kids; spatial play |
| 3 | **Multiply** — blueprint wand: copy a line, stamp it five times | tweens; the power moment |
| 4 | **Optimize** — throughput meters, jam management, source ratios | teens/adults; the itch |
| 5 | **Logistics** — conductor transit between districts, multi-floor routing | adults |
| 6 | **Interplanetary** — ship cargo hauls, cross-world supply contracts | endgame |
| 7 | **Wonders** — restore a region's landmark mega-machine | prestige |

Rungs 0–3 already exist mechanically in code (see §8). Rungs 4–7 are design headroom the current
contracts were explicitly built to support (shared `MineState`, `ProductionGraph`, ship cargo).

## 4. THE GRAMMAR — identical on all ~80 worlds

Five silhouettes, learned once, never violated:

1. **PRODUCER** — a tower with a visibly pumping piston head and a spill of product at its outlet.
   If it's making something, you can see the something.
2. **PATH** — a trough with rollers and a scrolling tread. Always in motion, even empty: the belt
   itself tells you its direction from across the yard.
3. **DECIDER** — a Y-housing (splitter today; filters/priorities later). Cargo visibly commits to a
   branch at the fork.
4. **CONSUMER** — an open mouth with a fill gauge on its face and a flip-counter above it. Hungry
   when empty, satisfied when fed, OVERFLOWING when you come home to idle earnings.
5. **TOOL** — the dispenser shelf (visible stack of tiles), the blueprint wand, the conductor
   lantern, the repair tools. Things you pick up look pick-up-able.

Shared sensory language: the cargo-teal accent means "automation touches this"; per-resource cargo
identity comes from the existing `BeltPuckStyle` hash (extended from 3 primitive shapes to a Forge
shape family: chunk / ingot / drum / crate / pod); the sound verbs are fixed game-wide — belt hum
scales with cargo in flight, *ka-chunk* per port emit, till-chime per payout, grinding groan for a
jam, creature-chewing alarm for a raid. A blind player can hear their factory's health.

## 5. THE DIALECTS — variation across 80 worlds without 80 designs

The anti-boredom budget is spent on **tiers, not worlds** — roughly 8 world-tiers of ~10 worlds
each. Within a tier, worlds vary by skin and terrain; between tiers, ONE new rule enters:

- **Resource dialect (every world, free):** local resources = local puck shapes/colors via the hash
  system, local pile looks, local depot payouts. Costs nothing; already half-built.
- **Terrain dialect (every world, cheap):** the same grid fights different ground — canal worlds
  route across stilts over water, cavern worlds thread lines through tunnels, vertical worlds
  earn lift cells. Placement itself becomes the puzzle; no new mechanics needed.
- **Tier rule (one per ~10 worlds, the real content):** e.g. T2 *tide-lock cells* (the namesake —
  lines that only run at high tide; batch for the surge), T3 *heat vents* (free overclock, melts
  cheap belts), T4 *spore fouling* (lines need flora clearance), T5 *magnet lifts* (vertical
  logistics), T6 *dual-cargo braiding*, T7 *creature-symbiosis* (tamed fauna as haulers), T8
  *tidal carillon logistics* (the endgame world's music-machine). Each tier rule is ONE cell/
  machine type built once and reused across its ten worlds.
- **The Wonder (one per region):** a huge derelict landmark machine — restoring it is that region's
  factory questline, and once running it does something spectacular and region-wide (the visible
  monument to your progress, readable from the sky vista).
- **The dialect law:** a dialect may never break the grammar. A producer on world 73 still looks
  like a producer. New rules change *routing decisions*, never *reading comprehension*.

## 6. ALIVE & DYNAMIC — the fun multipliers (what makes it a GAME, not a screensaver)

- **Defense fusion (Ziptide's unique fusion):** raid events target your lines — swarm bugs chew
  belts, wardens smash hoppers. You defend with the game's existing guns, standing on your own
  catwalks; later, turret cells feed ammo FROM the belt they defend. Factory and shooter stop
  being separate games. (Creature/combat contracts already exist; wear/repair already designed in
  `TOOLS_AND_REPAIR.md`.)
- **The tide event:** on a world clock, the tide comes in — visible, audible, world-wide. Prepared
  factories surge (tide-locked lines fire, waterwheels spin); unprepared ones flood and jam. The
  namesake becomes a gameplay heartbeat, not just scenery.
- **The homecoming moment:** idle accrual already works in the economy math. Make it *physical* —
  return to a world and the yard is FULL: piles taller, hoppers brimming, one overflowing depot
  spilling pucks onto the ground. Ten seconds of "look what my machine did" before you touch
  anything. This is the retention loop, staged like a gift.
- **Conductor transit:** riding your line matures from novelty to transport — long lines between
  districts become the fast-travel you *built*, with the cell-lip clicks as rhythm.
- **Jobs integration:** contracts ask for throughput ("bank 50 scrap at the north depot"), wiring
  the existing JobDirector to the sink payout event — the tutorial's repair loop grows into a
  career.
- **Ship logistics (rung 6):** haul a cargo pod from your W004 yard to a W011 refinery through the
  existing flight lane. The 80 worlds become one economy.
- **Co-op (post-multiplayer):** two players, one grid — the placement contract is already
  server-authoritative-friendly (authored ⊕ overlay). Parked until the multiplayer lane reopens.

## 7. MOMENT-TO-MOMENT JUICE (the sensory spec, condensed from the deep dive)

1. **Silhouettes:** every machine class becomes a Forge recipe with its §4 silhouette; primitive
   construction stays as the law-mandated fallback.
2. **The living belt:** scrolling tread via committed material + MaterialPropertyBlock offset
   (never runtime keyword flips — the Amendment-1 law), idle-turning rollers, emissive chevrons
   pulsing flow direction.
3. **Chunky cargo:** pucks 2–3× bigger, seated in the trough, slight bob, Forge shape family.
4. **The payoff show:** squash-gulp swallow, gauge rise, flip-counter tick, till-chime + near-field
   haptic, credit burst toward the CR readout; overflow state for homecoming.
5. **Placement juice:** hologram ghost (teal valid / red invalid), magnetic *chunk* on snap, spring
   settle; wand-stamp cascades the line in cell-by-cell (~50 ms/cell).
6. **The instrument:** the §4 sound verbs, mixed so a working factory is a pleasant hum and every
   state change is audible before it's visible.

## 8. WHAT ALREADY EXISTS AND SURVIVES (no rewrite)

The 2026-07-15 deep-dive verified all of this in code — the overhaul is a skin + feedback layer +
content program on top of contracts that are already right:

- Pure deterministic sim (`BeltLattice`), visuals explicitly a skin over state (`BeltFloorRuntime`).
- Physical placement with ghost + wrist-yaw direction (`BeltTileItem`), blueprint capture/stamp
  (`BeltBlueprintWandItem` + `BeltBlueprint`), line riding (`BeltConductorRuntime` + `BeltRoute`).
- Economy-safe extraction (`BeltMinePortRuntime` shares `MineState`; jam-safe emit; offline accrual
  via `ProfileEconomy`/`IdleEngine`), payout → profile → `ProductionGraph`.
- World-pack deployment (`BeltPadSpawner`), player-edit persistence (4.1f), perf budgets enforced
  by `AutomationAuditRules` (4.1g), per-resource identity (`BeltPuckStyle`).

## 9. EXECUTION PROGRAM (opens ONLY after the §0 gate)

Envelope order — each 1–3 commits, each individually behind `VISUAL` + `QUEST` proof and the
exposure gate, each ending with the kids test:

| # | Envelope | Contents | Acceptance |
|---|---|---|---|
| F-0 | **Anchor contract** | every machine exposes mouth/outlet/gauge/plate anchor points; the R2-style contract other envelopes attach to | contract tests only |
| F-1 | **Five silhouettes** | Forge recipes for producer/path/decider/consumer/tools via the photo booth | kids name each from 5 m |
| F-2 | **Living belt** | tread/rollers/chevrons motion layer | direction readable from 10 m, empty |
| F-3 | **Chunky cargo** | puck scale/seating/Forge shapes | resource identifiable at a glance |
| F-4 | **Payoff show** | swallow/gauge/counter/chime/overflow | a payout is FELT; homecoming staged |
| F-5 | **Placement juice** | hologram ghost, snap sound, stamp cascade | 2-minute first-line test |
| F-6 | **The instrument** | full sound layer | jam diagnosed by ear, eyes closed |
| F-7 | **Diegetic onboarding** | the no-text first-factory scene (§1) staged in the golden world | full kids test, cold |
| F-8 | **First tier rule** | tide-lock cells + the tide event on one world | grammar survives the dialect |
| F-9 | **First Wonder** | one region landmark machine questline | visible from the vista; players go look |

Rails: Quest budgets stay under the existing 4.1g audit (instanced tread/roller meshes, pooled
pucks already in place); all materials are committed assets (the shader-variant static gate applies
here first); every envelope leaves the system strictly better and shippable; the boards + this doc
updated per envelope; 3 CI-reds on one envelope → stop and escalate.

## 10. RISKS

- **Scope creep:** the ladder invites building rung 6 before rung 1 shines. The envelope order is
  the defense — F-0..F-7 before ANY dialect work.
- **80-world content cost:** solved structurally in §5 — tiers not worlds; dialects are one
  mechanic per ~10 worlds plus free skin variation.
- **Boredom regression:** if a tier rule tests as confusing, it's cut, not patched — the grammar
  outranks any single mechanic.
- **The old failure mode:** shipping booth-pretty, device-broken art. Every envelope's exit is a
  Quest screenshot/QUEST proof, per the recovery program. No exceptions, including for the author
  of this document.
