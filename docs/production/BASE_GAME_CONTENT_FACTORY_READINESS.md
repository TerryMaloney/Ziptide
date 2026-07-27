# BASE-GAME CONTENT FACTORY READINESS

**Status:** CANONICAL PRODUCTION READINESS AUTHORITY · planning only until the current M0 Quest verdict
**Written:** 2026-07-27
**Scope:** W000–W068 main campaign, shared content families, paid-asset intake, and automatic inheritance into Arena/Horde/Tidefront/future side modes

## 0. The blunt verdict

ZIPTIDE has enough **story design** to build the base game quickly, but it is **not yet prepared well enough to crush out the campaign in one month without avoidable drift and rework**.

The strong part is unusually strong:

- the Story Bible and Transmission are locked;
- W000–W068 have a distinct canonical seed;
- every seed already names its role, environment/physics, machine, resource, gear, sky, enemy, wreck/log, mystery object, RILL/story beat, and completion flag;
- W000–W012 have the reference serialization pattern;
- the project has real factories for worlds, jobs, items, creatures, saves, economy, travel, Forge visuals, audits, CI and device evidence;
- the first four worlds have approved visual North Stars;
- the Scrapper, MK2, RILL, artifact key, gate, Warden language, core gear and first creature set have approved concepts.

The weak part is the bridge from that design into mass production:

1. W013–W068 remain chapter prose seeds rather than machine-ready production records.
2. There is no single campaign matrix connecting world, architecture, gear, powers, creature, wreck, ship/space beat, story reveal, asset tier and current implementation state.
3. Gear placements exist in story prose but are not reconciled against actual definitions/runtimes, upgrade tiers, crafting, Arena/Horde/Tidefront reuse or asset readiness.
4. Derelict ships/stations are a Story Bible requirement and appear in nearly every world seed, but no reusable derelict catalog/schema/factory currently owns them.
5. World-flow templates exist, but most worlds do not yet have a measured architecture identity, kit/overlay recipe, hero landmark, POI set and concept acceptance image.
6. The old Symbiotic/Pragmatist/Ethereal visual-faction concept is not reconciled with the locked Wake Guild/Sable/Warden/Architect/Bloom/Pattern story factions.
7. Augments and most special powers are planned but are not assigned into a locked chapter progression or implemented through one shared effect registry.
8. The Scrapper and MK2 are strongly designed, but the MK2 acquisition world, mission, unlock flag and conversion of carried systems are not canonically assigned.
9. The Tripo plan is good intake planning, but WC-5 intake, skin/variant application, one humanoid/creature rig path and representative Quest proof are not built.
10. Main-game improvements are intended to feed side modes, but there is no enforced inheritance contract preventing Arena/Horde/Tidefront from copying or bypassing upgraded content.
11. Audio remains far behind every other content family.
12. Several older design documents carry stale status and can still mislead a future operator unless this file and the current HANDOFF override them.

Therefore the next objective is not “design more worlds.” It is:

> **Turn the existing story catalog into a content compiler input, prove every replaceable asset seam on W001/W002, and make all side modes consume the same content identities.**

---

## 1. What “the main game built in a month” can honestly mean

A one-month push can theoretically produce the **meat of the base game** only after W001/W002 prove the factory. The achievable target is:

- W000–W068 serialized and generated;
- every world enterable, completable and persistent;
- the complete story/reveal/branch structure present;
- each world has its intended environment family, architecture recipe, machine/job, hazard, gear/power, enemy/wreck/mystery and return path;
- representative AAA assets and material families replace the highest-value placeholders;
- all remaining placeholder sockets are explicit and replaceable without runtime rewrites;
- representative worlds per batch receive Quest proof;
- the full campaign can then be fine-tuned, art-replaced, balanced and voiced without rebuilding its foundations.

It does **not** honestly mean 69 worlds all reach final AAA art, animation, audio, VO and device polish in 30 days. The factory can build the full playable campaign skeleton and selected AAA model bands in that window; final shipping polish remains a separate pass.

The one-month goal is valid only if:

1. the next Quest test does not expose another shared rig/input/travel/save ownership failure;
2. W002 becomes playable materially faster than W001;
3. W003–W005 can be produced as a batch using data/recipes rather than runtime surgery;
4. every new external asset enters through one proven intake lane;
5. chapter batches use existing mechanics or add one reusable mechanic before all dependent worlds;
6. no side mode forks campaign content.

If any condition fails, stop scaling and repair the factory.

---

## 2. Canon content laws

Every base-game world must carry all of these, with intentional `none` allowed only for authored quiet/transit/choice worlds:

1. **Emotional/story purpose** — why this world exists in the reveal ladder.
2. **Environment form** — city, underground, open exterior, interior, void, coast/underwater, Pattern, station or forest/bio.
3. **Architecture identity** — base kit plus faction/era/material overlays, not only a palette.
4. **Arrival composition** — spawn facing, landmark, route promise, return reference.
5. **Physics/hazard law** — a distinct world behavior or deliberately calm contrast.
6. **Machine/contract verb** — the Shell function Cal repairs, retunes, builds, disables or chooses around.
7. **Resource/economy role** — what this world adds to the one economy.
8. **Gear/power role** — introduce, reinforce or upgrade a reusable verb.
9. **Creature/opposition role** — including intentionally no enemy.
10. **Derelict/log** — prior breakout attempt, prior worker, prior waker or a deliberate sacred exception.
11. **Mystery object** — one anomaly that points forward.
12. **RILL/character beat** — ambient presence plus the world’s specific story trigger.
13. **Signal/Pattern consequence** — where relevant to the chapter ladder.
14. **Surface job and route** — complete progression, reward and return.
15. **Art tier and sockets** — bulk Forge, Forge-plus face-distance, or external hero.
16. **Audio event set and caption twins**.
17. **Side-mode inheritance** — which Arena/Horde/Tidefront/co-op content can reuse this world’s kit, hazard, creature, weapon or mission.
18. **Evidence** — source, generated/audit, visual and representative Quest requirements.

The first two intentionally unusual cases remain canon:

- **W028:** no job; the absence is the content.
- **W057:** transit only; pure passage.

They are exceptions represented in data, not reasons to bypass the world contract.

---

## 3. Content-family readiness

| Family | What is genuinely ready | What prevents month-scale production | Required model-band proof |
|---|---|---|---|
| Story/reveal | Locked Story Bible, Transmission, chapter seeds and endings | W013–W068 are not serialized production records | one production packet schema that can represent normal, quiet, branch and transit worlds |
| World generation | WorldSpec/compiler, terrain, POIs, buildings/interiors, scatter, audits | most canonical worlds have no committed production WorldSpec; recipe-status drift | W001 real spec, W002 replication, W003–W005 batch |
| Architecture | nine flow templates; building/cave/terrain machinery; approved W001–W004 concepts | no complete cross-world architecture-family/overlay manifest; later worlds lack measured specs | W001 city grammar + W002 cavern grammar with zero hard-coded world logic |
| Weapons/tools | canonical item factory, live taser/gravity/pistol and expanded arena arsenal | no reconciled progression manifest; AAA feedback/physicality incomplete; several story tools do not exist | one shipped-quality Taser exemplar + one heavy/tool exemplar, shared in campaign and Arena/Horde |
| Powers/augments | strong design; reusable effect-registry direction | most are paper; no chapter placement/unlock/crafting matrix | one active + one passive through story, Arena and save/equip paths |
| Creatures | behavior families, forged body pipeline, six bodies, non-lethal response seams | signature W001 encounter unresolved; later species/content breadth not assigned by chapter batch | one signature creature from concept/import through behavior/LOD/device; one swarm batch proof |
| Ships | Scrapper and MK2 concepts; flight/loadout systems; multiple chassis data | starter ship still placeholder; MK2 mission/unlock unresolved; ship audit/intake incomplete | Scrapper hero root contract + one external visual swap without gameplay changes |
| Space routes | flight core and minimal disable/salvage | production route, approach/reentry and route data absent | W000→space→W001→return exact route |
| Derelicts/stations | story seeds make them a universal lore/economy device | no definition/catalog/POI/interaction/log recipe family | one derelict ship and one station interior built from the same reusable contract |
| Resources/machines | one economy, jobs, repair, mining/garden/belts/factories | world-specific machines can still become bespoke runtime scripts | W001/W002 machines built from shared stage/socket/effect vocabulary |
| Art intake | tier strategy, concepts, Forge, stable visual-child swap | WC-5 operational intake and skin application unproven | one external prop/held item, one rigged creature/character, one hero hull proof |
| Skins/variants | correct geometry-once design and existing cosmetic locker | general SkinDefinition/material-zone/attachment implementation remains paper | same base weapon/ship/room receives two skins with identical gameplay stats |
| Audio/VO | procedural ambience and limited runtime cues | no stable full event/bus/mix/caption production rail; nearly no final assets | first-hour event rail and representative paid batch |
| Side games | Arena uses story themes/gear; Horde uses creatures; Tidefront uses story worlds | reuse is convention, not enforced; duplicate definitions/presentation can drift | inheritance audits and explicit exception manifest |
| Evidence/release | CI, patch/audit, recovery PlayMode, Golden APK, MISS_LEDGER | no full-campaign profile or representative batch certification | exact model-band profile, then one high-risk + one ordinary world per batch |

---

## 4. Architecture and visual-language families

These are **derived production families**, not new factions or new lore. A world combines one spatial form, one base construction grammar and zero or more overlays.

### Spatial forms

- `CITY`
- `UNDERGROUND`
- `EXTERIOR_OPEN`
- `INTERIOR`
- `VOID`
- `COASTAL_UNDERWATER`
- `PATTERN`
- `STATION`
- `FOREST_BIO`

### Base construction grammars

1. **Salvage industrial** — welded frames, corrugated infill, patched utilities, practical controls, amber work light.
2. **Architect ancient** — monumental stone/grown alloy, deep-amber glyph channels, frozen-current geometry, exact intentional proportions.
3. **Bloom organic** — grown ribs, pods, living apertures, cultivated asymmetry and biological light.
4. **Pattern self-writing** — geometric cells, impossible joins, view-dependent paths, cyan-to-violet wrongness.
5. **Warden enforcement** — sterile ceramic-bone, single-eye hierarchy, smooth segmented authority, minimal warmth.
6. **Wake Guild serviced industrial** — better-maintained salvage infrastructure, corporate overlays, network maps and controlled access.
7. **Sable insurgent** — repurposed stations, stolen systems, improvised military protection, visible consequences of breakout attempts.
8. **Earth/Observer lab** — ordinary hard-science architecture, familiar materials, daylight and institutional indifference; no Bloom/Pattern decoration until an ending deliberately transforms it.
9. **Chitin/symbiotic settlement** — grown carapace city fabric, living walls and harvested organism structure; an application of Bloom-adjacent biology, not a separate story faction.

The old Symbiotic/Pragmatist/Ethereal sheet is retained as an **art/engineering idea bank**, not a competing faction canon. Its useful forms map into the families above:

- Symbiotic → Bloom/chitin construction and garden economy;
- Pragmatist → salvage/Guild/Sable modular industry;
- Ethereal → Architect/Pattern resonant technology.

Future operators must not introduce those three as new central factions without a Story Bible change.

### Overlay rule

Architecture is compositional. Examples:

- W002 = underground form + Architect ancient base + salvage-industrial pump overlay.
- W009 = city form + chitin settlement base + salvage utility overlay.
- W039 = city form + existing city base + Pattern conversion overlay.
- W047 = interior form + pristine Architect ancient grammar.
- Earth approach = interior/exterior Earth grammar with no alien overlay until the branch machinery activates.

This prevents 69 bespoke builders while preserving strong world identity.

---

## 5. Arsenal and power progression laws

### One content identity

Each usable item keeps one stable item/effect identity across campaign, Arena, Horde, Tidefront missions and future co-op. Modes may supply a balance profile or allowed-use rule; they do not receive copied meshes, copied grip data, copied effects or copied upgrade state.

### Progression shape

The campaign should introduce verbs in this order:

1. **Observe/scan** — wrist scanner and information layer.
2. **Disable** — taser/stun.
3. **Manipulate/salvage** — gravity tractor/glove.
4. **Control space** — Static Net, Foam, Bubble, shield and decoy tools.
5. **Transform the environment** — Splicer, Prism, Heat/Cryo, Sonic, phase tools.
6. **Traverse/build** — tether, drift, hover, zipline seed, pocket platform, mini-gate.
7. **Command allies/systems** — tamer, escort drone, RILL projector, build tiers.
8. **Retune reality** — Pattern/Signal/endgame tools and choices.

Every item entry must declare:

- stable item/effect id;
- campaign introduction and upgrade worlds;
- prerequisite/unlock flag;
- role and visible counter;
- target interfaces/effect tags;
- energy/charge/ammo model;
- one-hand/two-hand/throw/deploy interaction module;
- crafting/resource recipe;
- Forge/Tripo tier and canonical visual reference;
- grip, muzzle, sockets, moving parts and skin zones;
- campaign, Arena, Horde, Tidefront and co-op eligibility;
- bot/AI use profile where applicable;
- save/holster/travel behavior;
- automated and Quest evidence.

### First shipped-quality exemplars

1. **Taser Dart Gun** — crosses the most existing systems and defines non-lethal weapon feel.
2. **Gravity tractor/glove family** — defines the scan→stun→pull→place/throw→salvage identity.
3. **Sonic Thumper or Breaker Blade** — proves heavy/two-hand/melee physicality.
4. **One throwable runtime configuration** — proves many grenades are data, not scripts.
5. **One augment active and one augment passive** — proves shared power inheritance.

Do not mass-author the arsenal until these interaction modules and feedback services are proven.

---

## 6. Ships, stations and derelicts

### Hero-ship progression

The canonical campaign ship spine should be locked as:

1. **SLV-01 Scrapper** — W000 home, blue-collar salvage identity, physical controls, grabber arm, first space route.
2. **Architect-derived MK2 living yacht** — major early/mid campaign reward; same travel/coupler/ship/save contracts, upgraded capability and presentation.
3. **Endgame cloaked workshop ship** — Earth Approach story vessel, used for the final outside flight; not a routine player loadout replacement unless explicitly decided.

The six mechanical chassis/loadouts remain useful as **functional ship presets/fleet variants**, not six competing hero-story ships.

### MK2 lock still required

Before implementation, canon must assign:

- acquisition world and mission;
- why the artifact key activates it;
- unlock/completion flag;
- which Scrapper systems physically transfer;
- initial module/capability difference;
- whether it replaces the active home or becomes a selectable fleet slot;
- how Warden/Mara/Sable react;
- save migration and old-ship availability.

Recommended placement for story review: an Architect berth tied to W004, W008, W013 or W018. Do not silently choose in runtime code.

### Derelict content contract

Derelicts are not decoration. Every derelict recipe must support a configurable subset of:

- `derelictId` and stable family id;
- hull/station/ground-wreck form;
- origin/faction/era;
- intact, breached, flooded, burned, Pattern-converted or Bloom-grown state;
- exterior approach landmark;
- boarding/docking/entry socket;
- optional interior room recipe;
- salvage nodes and resource table;
- required tool/power;
- log/transmission id and ordered wreck-thread index;
- mystery object/socket;
- hazard and creature occupants;
- completion/looted state persistence;
- side-mode encounter eligibility;
- art tier, LOD, collider, portal-culling and budget data.

A reusable catalog should begin with:

1. small crashed skiff;
2. beached salvage barge;
3. orbital transport wreck;
4. failed jump ship;
5. sealed station section;
6. flooded research craft;
7. Warden wreck;
8. Architect berth/workshop;
9. faction war wreck;
10. Earth/Observer craft.

Worlds compose these families with local damage/overlay/material data. They do not get one-off wreck runtimes.

---

## 7. Paid Tripo month readiness

The paid month should begin only after one complete import pilot proves:

concept → generation source archive → provenance/license record → cleanup → real-meter scale → pivot/orientation → rig/skeleton if required → UV/material zones → attach points → LODs → colliders → URP/conformance → stable visual-child swap → booth comparison → Quest budget/device verdict.

The current plan’s “bulk creatures stay Forge” law remains correct, but Terry’s new priority requires a refinement:

- **bulk/common creatures:** Forge-native variants;
- **signature or face-close monsters/characters:** eligible for Tripo/external Tier B/C intake;
- **one imported signature creature** must prove the rig/animation/LOD path before generating a roster.

### Current priority order

**Intake pilots before volume**

1. one simple prop or held tool;
2. Taser hero visual;
3. one rigged signature creature;
4. RILL;
5. Scrapper hull + separate grabber arm;
6. one modular room/architecture proof;
7. one character body only if the approved first-person/mirror use requires it.

**Then high-reuse production**

- W001/W002 hero landmarks and props;
- approved W001–W004 signature creatures;
- gravity tool and heavy/melee exemplar;
- MK2 only after its gameplay root, zones and story unlock are stable;
- one close-range capital/derelict proof;
- modular attachment parts and skin templates.

### Every generated asset brief must include

- stable target id and all worlds/modes using it;
- canonical concept images;
- real dimensions and camera-distance band;
- Tier A/B/C classification;
- rig/skeleton and minimum animation list;
- UV and separated material zones;
- fixed identity zones;
- interaction/weapon/ship sockets;
- collider and physics ownership;
- LOD and texture budgets;
- skin/variant requirements;
- provenance and purchase-date rights evidence;
- fallback visual id;
- booth/device acceptance questions.

Do not generate assets merely because they look useful. No stable runtime/content socket means no paid generation yet.

---

## 8. Side-game inheritance requirement

The campaign is the content source. Side modes are alternate rules and arrangements over the same content.

- **Arena:** consumes the same item visuals, grip/feel/audio/VFX and biome/kit/sky identities; only balance/spawn rules differ.
- **Horde:** consumes the same creature definitions, bodies, reactions, hazards and weapons; only waves/difficulty/reward rules differ.
- **Tidefront:** consumes the same world identity, biome, resource, hazards, mission verbs and ship/faction families; its VR missions use the actual worlds.
- **Future co-op:** consumes campaign jobs/interactions through network adapters; it does not receive duplicated jobs.
- **Cosmetics/skins:** one locker and one asset/skin identity across all modes.
- **Audio/VFX:** one event id and one effect vocabulary across all modes.

A main-game upgrade is incomplete if an eligible side mode still uses a copied primitive, old effect, duplicate definition or stale material.

The detailed enforcement contract lives in `SHARED_CONTENT_INHERITANCE_CONTRACT.md`.

---

## 9. Aggressive post-headset production order

### Gate 0 — device truth

- complete the exact M0 route twice;
- fix only shared blockers before content scaling;
- preserve the new known-good source/APK.

### Gate 1 — W000/W001/W002 model band

1. migrate the first-level contract into machine data;
2. finish W000→space→W001→return;
3. rebuild W001 against its approved city/arrival concepts;
4. prove one complete weapon, one creature, one machine, one derelict, one story object and one real external-asset swap;
5. build W002 from the same packet with no special-case runtime.

### Gate 2 — production catalogs

1. serialize W013–W068 into the same production-record shape;
2. land the campaign world matrix;
3. land arsenal/power progression data;
4. land architecture-family/overlay recipes;
5. land derelict catalog;
6. assign the MK2 acquisition;
7. land side-mode inheritance audits;
8. stabilize audio event ids.

### Gate 3 — W003–W005 throughput proof

- W003 open exterior/wind/tether;
- W004 interior/dread/Transmission;
- W005 forest/Bloom/garden/splicer;
- generate together, test together, fix the factory rather than the worlds.

### Gate 4 — chapter batches

Build in mechanic/kit clusters, not numerical isolation:

- Chapter 2 remainder: prism, station, archive, chitin city, tide, resonance, void;
- Chapter 3: underwater, heat, Pattern, faction outpost, deep cavern, HQ, Unmade World;
- Chapter 4: Sable war/branch, Bloom nursery, vertical scaffold, Warden introduction, quiet capstones;
- Chapters 5–6: Pattern conversion, large cities, Warden systems, ally/branch content, convergence;
- Chapter 7: memory/return/resolution worlds;
- Endgame: Lagrange ship, Earth lab, branch and four endings.

Each batch receives:

- deterministic generation/audit for every world;
- one high-risk Quest representative;
- one ordinary Quest representative;
- full chapter route smoke;
- one side-mode reuse proof.

---

## 10. Theoretical 30-day campaign build schedule

This is an aggressive capacity test, not a promise.

### Days 1–5 — prove the factory

- close M0 blockers;
- finish W001 model quality and W002 replication;
- prove first Tripo intake, first signature creature, first shipped-quality weapon;
- lock production packet and inheritance contract.

### Days 6–10 — complete Chapter 1/2 machinery

- W003–W005 throughput batch;
- serialize and regenerate W006–W012;
- ship missing chapter mechanics as reusable modules;
- first full W000–W012 campaign pass.

### Days 11–17 — Chapters 3–4

- serialize W013–W028;
- build shared underwater/heat/Pattern/station/deep-cave/branch modules first;
- generate worlds in batches;
- representative Quest passes and side-mode reuse.

### Days 18–24 — Chapters 5–7

- serialize W029–W061;
- build Pattern/Warden/memory overlays and later gear upgrades;
- batch-generate and run chapter smoke routes.

### Days 25–28 — Revelation/Earth/endings

- W062 revelation;
- Lagrange hidden ship and Earth approach;
- W063 branch and W064–W068 ending variants;
- end-to-end save/branch validation.

### Days 29–30 — campaign integration verdict

- full route/flag/save audit;
- representative Quest soak;
- identify all remaining placeholder/art/audio/VO/polish debt;
- freeze the content-complete campaign baseline before fine-tuning.

This schedule is feasible only if W002 and W003–W005 show that later work is mostly data/recipe authoring. If those gates fail, continuing the calendar would create 60 broken worlds faster, not a game.

---

## 11. Definition of “ready to scale”

The base-game factory is ready when all of these are true:

- the exact M0 device route passes;
- W001 is coherent and W002 is substantially faster;
- every W000–W068 row exists in the campaign matrix and production record;
- every recurring content family has one canonical owner and catalog;
- every world references architecture, gear, creature, wreck, story and audio identities rather than copied scene objects;
- Tripo/imported visuals can replace placeholders without moving gameplay roots or changing ids;
- skins/variants do not alter gameplay;
- Arena/Horde/Tidefront automatically inherit eligible upgrades or fail an audit;
- chapter batches can be regenerated deterministically;
- representative Quest evidence closes each batch;
- a new operator can build a world from its packet without reconstructing project history.

Until then, more isolated feature work is secondary to closing these preparation gaps.
