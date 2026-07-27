# ARSENAL AND POWER PROGRESSION — CAMPAIGN + SIDE MODES

**Status:** CANONICAL RECONCILIATION TARGET · implementation gated by the current M0 Quest verdict
**Written:** 2026-07-27
**Purpose:** connect Story Bible world placements to actual item/effect systems, art tiers, upgrades and side-mode consumers

## 0. Identity and fantasy

Cal is a technician/explorer, not a soldier. The campaign arsenal is built around:

> **scan → diagnose → disable → manipulate/capture → salvage → transform the environment → retune the Shell**

Weapons and powers should solve combat, traversal, repair or puzzles in more than one context. Non-lethal disable/capture/power-down remains canon. Campaign and side modes share item identity, physical interaction, visuals, audio, VFX and skins; mode profiles may alter balance.

---

## 1. What actually exists today

| Content | Current evidence-backed state | Honest limitation |
|---|---|---|
| Wrist scanner/result path | runtime scanner and first-hour result adapters exist | presentation/upgrade progression and campaign placement are incomplete |
| Taser Dart Gun | live physical sticky projectile, stun routing, haptics, Forge visual, cosmetics, holster/travel/save path | still one-hand/basic feedback; no AAA recoil/mechanical/reload/audio stack |
| Gravity Gun | live hitscan gravity pulse/launch runtime and cosmetics | primitive visual; not the planned hand-driven Gravity Glove pull/catch/hold/throw system |
| Pistol | live hitscan/tracer/haptics/Forge visual | conventional sidearm, not reconciled into campaign progression; non-lethal fiction needs explicit role |
| Static Net | live Arena item/runtime/rules | campaign W009 placement and shared campaign definition/persistence remain to be integrated |
| Sonic Thumper | live Arena heavy/melee AoE runtime/rules/Forge visual | campaign W011/W025 placement, two-hand feel and full physical feedback remain |
| Prism Beam | live Arena charge-beam runtime/rules/Forge visual | campaign puzzle/beam interactions and canonical W006 placement remain |
| Breaker Blade | live melee runtime/item and Forge visual | W009/W048 progression, wall/material interaction and final Quest pose/scale still require proof |
| Tide Pike | live melee runtime/item and Forge visual | W010 story placement, reach/counter interactions and final Quest feel remain |
| Shared item factory | stable ids, XR shell, grip/muzzle, Forge applier, cosmetics, holster/save | physical reload, two-hand support, moving parts, unified effects and AAA feedback services remain |
| Augments | strong data/effect/slot design | no verified live launch set or campaign placement in current authority |
| Throwables/deployables | generic runtime direction is designed | shared production runtime and campaign content not yet proven |
| Most chapter powers | story placements are canonical seeds | definitions, effects, assets, recipes, unlocks and mode profiles are mostly absent |

Do not describe the arsenal as greenfield. Do not describe it as shipped-quality.

---

## 2. Locked verb progression

| Stage | Player verb | Primary content family | Purpose |
|---|---|---|---|
| 1 | Observe/scan | wrist scanner, headlamp, translation/decrypt overlays | learn what an object/world is before acting |
| 2 | Disable | Taser, Static Net, Prism/Arc control, melee counters | stop threats without killing |
| 3 | Manipulate/salvage | gravity tractor/gun/glove, magnet tether | move parts, capture disabled targets and make combat pay |
| 4 | Control space | Foam, Bubble, Shield, Decoy, hover/anchor tools | create cover/routes/safe zones |
| 5 | Transform systems/biomes | Bloom Splicer/Tamer, Heat/Cryo, Sonic, Prism | convert environmental rules into tools |
| 6 | Build/traverse | drift/hover, zipline seed, Pocket Platform, Mini Gate | change how worlds are crossed and repaired |
| 7 | Command/upgrade | build tiers, Warden escort, RILL tools | operate larger systems and allies |
| 8 | Retune reality | Phase/Tide/Signal/Pattern tools and endgame choices | move from technician to author/debugger |

Adjacent worlds should alternate or combine verbs rather than introduce a completely new control scheme every level.

---

## 3. Campaign placement and implementation matrix

### Chapter 0–1

| Content | Canon introduction/use | Upgrade/payoff worlds | Runtime state | Production action | Asset tier | Side-mode use |
|---|---|---|---|---|---|---|
| **Wrist Scanner** | W000 coupler diagnosis; W001 relay/job inspection | W002 headlamp/scan; W008 translation; W027 Warden scan; W042 decrypt | `PARTIAL LIVE` | unify upgrade/effect ids and presentation tiers; keep scanner as one persistent tool | Tier B held/forearm | Arena locator/scanner, Horde reveal, Tidefront scan missions |
| **Taser Dart Gun** | W000/W001 starter disable tool | charge/feedback/crafting upgrades through later chapters | `LIVE` | first shipped-quality weapon exemplar; keep item id stable | Tier B; Tripo/external visual eligible | Campaign, Arena, Horde, Tidefront missions, co-op |
| **Gravity tractor family** | W000 space salvage should use ship tractor; hand tool follows starter loop | W007 low-g handling; W021 gravity tier 2 | Gravity Gun `LIVE`; Glove `MISSING` | share targeting/effect primitives; build hand-driven pull/catch FSM rather than rename gun | Tier B held tool | Campaign, Arena, Horde, salvage/Tidefront |
| **Headlamp/scan module** | W002 darkness | later translation/decrypt progression | `PLANNED` | make a scanner attachment/upgrade profile, not a second scanner owner | Tier B forearm/head tool | campaign/Horde visibility mutators |
| **Magnet tether/anchor** | W003 wind traversal and anchoring | possible later grapple/zipline interactions | `PLANNED`; general grapple remains open | one tether core with surface/metal target profiles | Tier B | campaign, arena traversal mutator, co-op |
| **Signal dampener** | W004 static/broadcast protection | later Signal/Pattern resistance | `PLANNED` | preferably passive augment/profile over hazard system | Tier A/B wearable | campaign/Horde/Tidefront buff |

### Chapter 2

| Content | Canon introduction/use | Upgrade/payoff worlds | Runtime state | Production action | Asset tier | Side-mode use |
|---|---|---|---|---|---|---|
| **Bloom Splicer v0** | W005 converts vines/opens paths | v2 W022; v3 W033 | `PLANNED` | one effect registry with tiered valid-target/action sets | Tier B tool | campaign, Horde biome objectives, Tidefront Bloom missions |
| **Prism Beam** | W006 prism/light routing | tier 2 W044 | Arena runtime `LIVE` | add campaign beam receivers/reflection/splitting; preserve one item identity | Tier B heavy | campaign, Arena, Horde |
| **Gravity handling upgrade** | W007 low-g | W021 tier 2 | `PLANNED` | upgrade the gravity family rather than spawn an unrelated item | data/attachment | all eligible modes |
| **Translation Overlay v0** | W008 Architect glyph reading | W042 decryptor is later branch | `PLANNED` | scanner capability/effect id; text/caption/localization aware | UI/forearm presentation | campaign/Tidefront intel missions |
| **Static Net** | W009 swarm control | tier 2 W048 | Arena runtime `LIVE` | campaign throwable/launcher integration and swarm target behavior | Tier B | campaign, Arena, Horde |
| **Breaker Blade** | W009 tight alleys/carapace shortcuts | tier 2 W048 | `LIVE PARTIAL` | story unlock, material/contact tags, deflect/return options only after base melee proof | Tier B | campaign, Arena, Horde |
| **Hover/drift mod** | W010 cross flood/water | Hover Shell 2 W030; tier 3 W045 | `PLANNED` | one comfort-capped traversal effect with tier profiles | wearable/vehicle module | campaign and selected Arena/Horde mutators |
| **Tide Pike** | W010 reach against tide-borne tendrils | no explicit later tier currently | `LIVE PARTIAL` | story placement, two-hand reach, environment interactions | Tier B | campaign, Arena, Horde |
| **Sonic Thumper** | W011 resonance and swarm push | tier 2 W025 | Arena runtime `LIVE` | campaign resonance receivers, break/material tags and two-hand feel | Tier B heavy | campaign, Arena, Horde |
| **Radiation shielding** | W012 void exposure | tier 2 W038 | `PLANNED` | passive suit/augment profile through hazard owner | wearable/skin-compatible | campaign/Tidefront missions |
| **Jump Core** | W012 deeper-travel unlock | route/world access progression | resource/flag concept exists | define capability/unlock contract; it is not a handheld weapon | hero story object Tier B | campaign/Tidefront map gating |

### Chapter 3

| Content | Canon introduction/use | Upgrade/payoff worlds | Runtime state | Production action | Asset tier | Side-mode use |
|---|---|---|---|---|---|---|
| **Rebreather/pressure suit** | W013 | tier 3 W052 | `PLANNED` | one hazard/suit upgrade profile | wearable/skin | campaign/Tidefront underwater missions |
| **Heat Lance + cryo coolant** | W014 | pairs with Cryo Sprayer W032 | `PLANNED` | shared temperature effect taxonomy; tool/receiver data | Tier B heavy/tool | campaign, Arena/Horde if counter is readable |
| **Mini Gate Puck** | W015 object portals | later endgame/Pattern use possible | `PLANNED HARD` | object-only portal first; no player portal until comfort proof | Tier B | campaign puzzles; usually exclude competitive modes initially |
| **Override/lockpick** | W016 | tier 2 W023 | `PLANNED` | scanner/interaction capability profile with faction locks | Tier B tool | campaign/Tidefront sabotage/intel |
| **Drift Belt** | W017 descent/glide | tier 3 W058 | `PLANNED` | shared comfort-capped movement effect | wearable | campaign and declared mutators |
| **Build Tier 2** | W018 Blueprint | Tier 3 W047 | production/economy systems exist; story unlock not reconciled | one capability flag/recipe tier across machines, ship and world building | no single visual | campaign/Tidefront economy |

### Chapter 4

| Content | Canon introduction/use | Upgrade/payoff worlds | Runtime state | Production action | Asset tier | Side-mode use |
|---|---|---|---|---|---|---|
| **Foam Cannon** | W020 neutralize acid/create cover | later reusable leak/platform/cover roles | `PLANNED` | one foam volume/material receiver system | Tier B heavy | campaign, Arena, Horde |
| **Gravity tier 2** | W021 | builds on W007 family | `PLANNED` | context/effect upgrade on canonical gravity family | data/attachment | all eligible modes |
| **Splicer v2** | W022 nursery | v3 W033 | `PLANNED` | tier data only after v0 effect is proven | same base/tool skin | campaign/Horde |
| **Override tier 2** | W023 Vault | builds on W016 | `PLANNED` | tier data/valid-target expansion | same tool | campaign/Tidefront |
| **Thumper tier 2** | W025 | builds on W011 | runtime base `LIVE` | resonance/radius/power profile; avoid new item code | same base/skin or modular head | campaign/Arena/Horde |
| **Zipline Seed** | W026 creates routes | later world building/traversal | zipline runtime `LIVE`; seed tool `MISSING` | author a route-placement tool over canonical zipline owner | Tier B tool | campaign/co-op; competitive use needs map rules |
| **Warden scanner** | W027 reads Warden tech | W031/W037/W043 branch support | `PLANNED` | scanner effect/translation tier, not new scanner | data/forearm skin | campaign/Tidefront intel |

### Chapter 5

| Content | Canon introduction/use | Upgrade/payoff worlds | Runtime state | Production action | Asset tier | Side-mode use |
|---|---|---|---|---|---|---|
| **Phase Anchor** | W029 stabilizes Pattern ground | tier 2 W054 | `PLANNED` | shared Pattern receiver/effect and stable-zone rules | Tier B deployable/tool | campaign, Horde Pattern events, Tidefront mission |
| **Hover Shell tier 2** | W030 flooded city | tier 3 W045 | `PLANNED` | shared hover progression | wearable/vehicle | campaign/selected mutators |
| **Decoy / Orbital Pebble** | W031 distracts Wardens | may evolve into RILL projector/companion | `PLANNED` | one deployable/companion effect family | Tier B small hero | campaign, Arena, Horde |
| **Cryo Sprayer** | W032 lab | pairs with W014 heat | `PLANNED` | temperature/state runtime and surface receivers | Tier B heavy | campaign, Arena/Horde |
| **Splicer v3** | W033 mastery | end of Splicer tier ladder | `PLANNED` | data expansion only | same base + skin/attachments | campaign/Horde |
| **Pocket Platform** | W034 construction/traversal | later scaffold/Pattern use | `PLANNED` | one deployable surface runtime with strict budget/lifetime | Tier B gadget | campaign/co-op; competitive mode optional |
| **Arc Rifle** | W035 chain through Pattern/targets | capstone electrical role | `PLANNED` | beam/chain module over shared target tags; visible counter | Tier B heavy | campaign, Arena, Horde |
| **Void shielding tier 2** | W038 | builds on W012 | `PLANNED` | hazard-profile upgrade | wearable | campaign/Tidefront |

### Chapter 6

| Content | Canon introduction/use | Upgrade/payoff worlds | Runtime state | Production action | Asset tier | Side-mode use |
|---|---|---|---|---|---|---|
| **Tide Glove** | W039 pushes Pattern back and manipulates flows | endgame/environment mastery | `PLANNED SIGNATURE` | palm/body-language force effect; move world objects, not player | Tier B hero hand tool | campaign, Arena/Horde if comfort/fairness proven |
| **Bloom Tamer v0** | W040 briefly controls small creature | Warden escort is later command parallel | `PLANNED HARD` | allegiance/command adapter over creature owner, time-bounded | Tier B tool/companion | campaign/Horde; PvP excluded initially |
| **Energy Shield Disc** | W041 block/boomerang/switch | future return/deflect mechanics | `PLANNED` | returning-projectile + shield modules | Tier B | campaign, Arena, Horde |
| **Signal Decryptor** | W042 outside records | builds scanner/translation family | `PLANNED` | scanner effect tier and story decoder | data/forearm | campaign/Tidefront intel |
| **Prism Beam tier 2** | W044 | builds on W006 | base runtime `LIVE` | richer campaign receiver/effect profile | same weapon/skin/attachment | campaign/Arena/Horde |
| **Hover Shell tier 3** | W045 | builds on W010/W030 | `PLANNED` | final traversal tier | same wearable | campaign |
| **Build Tier 3** | W047 master Blueprint | builds on W018 | systems partial | unlock master recipes/capabilities through one tier field | no single visual | campaign/Tidefront |
| **Static Net tier 2** | W048 | builds on W009 | base runtime `LIVE` | larger/Pattern-aware profile | same item | campaign/Arena/Horde |
| **Breaker Blade tier 2** | W048 | builds on W009 | base runtime `LIVE` | Pattern-hard material/interaction profile; preserve base grip/runtime | same mesh with attachment/skin or Tier-B variant | campaign/Arena/Horde |
| **Warden Escort Drone** | W049 ally payoff | branch-dependent | `PLANNED` | companion definition/command adapter over Warden behavior | Tier B/C recurring ally | campaign, Tidefront; Horde only as reward/mutator |
| **Capstone gear upgrade** | W050 | combines campaign resources | `UNDEFINED` | must be resolved after the prior progression is real; do not invent a catch-all script | TBD | campaign/endgame |

### Chapter 7 and endgame

Chapter 7 intentionally slows new gear. It pays off mastered systems:

- W052 Rebreather tier 3;
- W054 Phase Anchor tier 2;
- W058 Drift Belt tier 3;
- W053/W055/W056/W057/W059/W060/W061 focus on memory, factions and choices rather than loot.

W062–W068 introduce no ordinary arsenal. Alliance keys, Signal Core, ship access and the four ending machines are story capabilities/flags. They should consume existing verbs at their highest level rather than hand the player unexplained final weapons.

---

## 4. Canon conflicts and required decisions

1. **Prism Beam placement:** chapter canon makes W006 the prism world; the arsenal roadmap mentions W002. Production recommendation is W006 introduction, with W002 retaining scanner/headlamp. Terry may override explicitly.
2. **Pistol campaign role:** it exists and serves Arena, but the explorer-tech/non-lethal campaign fantasy does not assign it. Decide whether it is a low-power pulse tool, optional salvage sidearm or Arena-only conventional template.
3. **Gravity Gun versus Glove:** preserve both only if they have distinct roles—gun = heavy shove/projector; glove = pull/catch/hold/throw. Do not claim the Glove exists.
4. **Tier upgrades:** decide whether tiers are new item ids, modular attachments or effect profiles. Default recommendation: stable base item + upgrade profile/attachment, so skins/grips/side modes stay coherent.
5. **Capstone gear:** W050 names a capstone without defining the verb. It should emerge from the proven progression, not be designed now in isolation.
6. **MK2 capability relationship:** ship modules and Jump Core must be represented in the same progression view as ground gear, but remain ship definitions/effects rather than handheld items.
7. **Special powers versus augments:** suit/passive hazard resistance should use augments/profiles; distinct physical world tools should remain items.

---

## 5. Proposed six-augment chapter placement

The launch augment set exists in design but has no canonical campaign placement. The following is a **production proposal, not locked canon**, chosen to reinforce existing worlds rather than add six new tutorials:

| Augment | Proposed introduction | Why it fits | Other modes |
|---|---|---|---|
| **Magnet Palm** | W002 completion or W007 low-g | reinforces mining/collectibles and gravity language | Arena/Horde pickup utility |
| **Sure Step** | W005 or W010 | hazard-slow resistance after spore/flood teaching | Horde/Tidefront hazard buff |
| **Surge Dash** | W026 Scaffold | traversal mastery in an open vertical world | Arena active pickup, Horde escape |
| **Bubble Guard** | W020 Copper Shore | acid assault/cover context makes protection readable | Arena/Horde defense |
| **Sixth Sense** | W027 or W031 | Warden scanning/threat awareness | Arena locator/Horde warning |
| **Overclock** | W035 Signal Crest | high-Signal power spike and weapon-charge mastery | Arena/Horde charge utility |

Before locking, verify adjacent-world pacing and ensure one active + one passive slot scarcity remains meaningful.

---

## 6. Shared production schema requirements

Do not build a mega-manager. Add fields to the narrowest existing Definition/registry/profile owners, while maintaining one source-controlled progression manifest containing references only.

Each arsenal/power row must resolve:

- `contentId` and content kind;
- base Definition/visual/effect ids;
- intro world, upgrade worlds and unlock flags;
- target interfaces/tags and effect module;
- interaction module (`fire`, `beam`, `contact`, `throw`, `deploy`, `wear`, `companion`, `ship`);
- one/two-hand requirements;
- charge/resource/cooldown profile;
- crafting costs and resource source;
- mode context profiles;
- bot/AI eligibility;
- asset tier, concept, skin zones and fallback;
- save/holster/travel behavior;
- tests, booth artifact and Quest questions.

World records reference the content id. They do not duplicate item stats.

---

## 7. Build order after the Quest gate

1. Ship-quality Taser feedback/physicality without changing its id.
2. Shared weapon inheritance proof in campaign and Arena.
3. Gravity family: tractor/ship salvage seam, then hand Glove FSM.
4. Campaign integration for Static Net, Thumper, Prism, Blade and Pike.
5. One generic throwable runtime and two data configurations.
6. One active and one passive augment through campaign/Arena/save.
7. Remaining chapter powers in dependency order, one reusable effect family at a time.
8. Upgrade/tier profiles and crafting.
9. Full arsenal progression audit, bot parity and representative Quest campaign.

Mass Tripo weapon generation begins only after the base’s grip, moving-part sockets, material zones and skin contract are frozen. Generate geometry once; upgrades use attachments, effect profiles and skins wherever possible.
