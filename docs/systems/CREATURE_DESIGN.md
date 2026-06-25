# Creature Design — framework + novel, evolution-tied behaviors

**Goal:** make Ziptide's aliens **feel new** — movements/behaviors not seen in games — while each is
**justified by the planet's physics/biome and the story** (per `storyboard/STORY_BIBLE.md`). This is the
buildable framework + a catalog for Fable 5 to implement. Non-lethal: you **disable**, you don't kill.

## The framework (generalize what already works)
Drones already prove the pattern: `DroneRuntime` holds a `CombatDriven` seam, `DroneCombatBehavior` owns
the FSM, `IShockable` is the stun contract, `HitZones` classifies hits for reactions. Generalize:
- **`CreatureBehavior` (base MonoBehaviour seam):** `OnSpawn() · OnPatrol() · OnDetect(player) · OnAttack()
  · OnStunned(seconds) · OnDown(hitZone)`. Owns motion; `CreatureRuntime` enables/disables it (like
  `CombatDriven`). Reuse `IShockable` + `HitZones` for every creature.
- **`CreatureRuntime` (factory):** reads `CreatureDefinition.archetype` (+ a `behaviorId`) and attaches the
  right `CreatureBehavior`. Stats/health/loot/biome from `CreatureDefinition` (DATA, Architect lane).
- **`CreatureZoneDef` (generalize `DroneZoneDef`):** id/center/radius/count/respawn/`creatureId`/combat — so
  any world places any creature from data, same as drone zones today.
- **Lane split:** `CreatureDefinition`/`CreatureZoneDef`/stats/loot = **[A]**; the `CreatureBehavior`
  MonoBehaviours/FSMs/motion = **[T]**. Each novel behavior = one `CreatureBehavior` subclass.

**Design rules:** every creature has (1) a **readable telegraph** before it acts (fair in VR), (2) a
**clear counter** using existing gear (taser/gravity/scanner/splicer/prism/etc.), (3) a **non-lethal
disable** state, (4) an **evolution reason** it moves how it does, and (5) a **story tie** (Bloom/Pattern/
Warden/faction). Keep motion smooth (no player-camera yanks).

## Novel behavior catalog (each: mechanic · evolution reason · counter · fits)
1. **Witness-mite** — freezes solid when *observed/scanned*, moves only while unobserved. *Evolution:* a
   contained-universe organism that learned the Observers' gaze "fixes" matter. *Counter:* keep it in
   view; wrist-scan to pin it, then gravity-toss. *Fits:* Pattern/observation worlds (W015/W029/W039) — ties the meta to a mechanic.
2. **Rewinder** — leaves a visible afterimage trail and can **snap back** to a position from 2s ago when
   hit. *Evolution:* grew in high-Bloom memory fields; its body partly lives in the recent past. *Counter:*
   anticipate the snap-back spot; tag it with the scanner to lock its timeline. *Fits:* Memory Reef/Sea (W013/W052).
3. **Sound-walker** — can only traverse surfaces that are **vibrating**; goes inert in silence. *Evolution:*
   evolved on a resonant world with no stable ground. *Counter:* the Sonic Thumper *silences* a patch to
   strand it; or lure it onto a dead surface. *Fits:* The Hum / Echoes (W011/W025).
4. **Tether-swarm** — many bodies sharing **one health pool via a glowing tether**; killing individuals does
   nothing — you must **sever the tether**. *Evolution:* a colony organism with a shared nervous cord.
   *Counter:* the hammer/plasma cuts the cord; gravity-pull two nodes apart. *Fits:* Chitinwall (W009/W048).
5. **Orbit-grazer** — never approaches directly; **orbits a gravity well** and you must break its orbit, not
   shoot it head-on. *Evolution:* a low-g void grazer that feeds on field lines. *Counter:* gravity gun to
   shove it off-orbit into a wall; time shots to its arc. *Fits:* Void/station worlds (W012/W038/W050).
6. **Inverter** — projects a local **gravity flip** around itself; you fight it on the ceiling/walls.
   *Evolution:* manipulates the Shell's gravity seams. *Counter:* disable it fast or use the flip to reach
   it; comfort-gated (snap zones only — see PvP comfort rules). *Fits:* Lattice/Pattern (W021), Architect chambers.
7. **Mimic-echo** — **copies the player's last movement** on a short delay (telegraphs by mimicking you);
   read your own moves to predict it. *Evolution:* a Pattern fragment that learns by reflection. *Counter:*
   feint to bait its echo, then strike off-rhythm; the Echo Seed tool counters it. *Fits:* Mirror Flats/Broken Pattern (W006/W054).
8. **Husk-molter** — when stunned it **sheds a decoy husk** and skitters out the back; the husk fools your
   scanner once. *Evolution:* a wall-crawler with disposable exoskeleton. *Counter:* scan-then-confirm; hit
   the *moving* one; gravity-grab before it molts. *Fits:* Underground/Chitin (W017/W048).
9. **Tide-phase** — only **solid/vulnerable at low tide** (or low pressure); intangible otherwise. *Evolution:*
   evolved to phase with the world's tidal/pressure cycle. *Counter:* the world's cycle is the timing puzzle;
   strike on the phase. *Fits:* Coastal/Underwater (W010/W013/W032/W045).
10. **Light-grazer** — **grows in darkness, shrinks/flees in light**; a big one is just a small one that sat
    in the dark. *Evolution:* photophobic Bloom-feeder. *Counter:* Prism Beam shrinks it to disable size.
    *Fits:* Underground/Drowned Lab (W002/W032), Bloom interiors.
11. **Fractal-splitter** — splits into **smaller, weaker copies** when hit (the universe replicating itself);
    over-shooting makes more enemies. *Evolution:* Pattern-born self-similar growth. *Counter:* stun (don't
    shoot) to stop the split; one clean disable beats ten hits. *Fits:* Pattern worlds (W039/W067), the Bloom.
12. **Bridge-former** — a tendril mass that, when **spliced (not killed)**, becomes a **bridge/door** you
    need to progress; hostile until repurposed. *Evolution:* the Bloom-as-infrastructure. *Counter:* the
    Bloom Splicer converts it — combat *becomes* traversal. *Fits:* Bloom Cathedral/Nursery (W022/W033/W040).

## Wardens (the Shell's immune system — special)
Not feral: **lawful, telegraphed, recognizable.** They escalate by your Signal level, can become an **ally**
(Ch.6 branch), and one **recognizes RILL** (W037). Build as a `CreatureBehavior` with a "lawful enforcer"
FSM (warn → pursue → disable, never ambush) and an ally-flag path. *Fits:* W027/031/037/043/049/059.

## Build order (for Fable 5 — see `FABLE5_BACKLOG.md` Phase E)
1. **[T]** `CreatureBehavior` base + `CreatureRuntime` factory (generalize the drone seam). **[A]** `CreatureZoneDef` + `CreatureDefinition` extensions.
2. **[T]** the 4 base archetypes (Swarmer/WallCrawler/Flyer/Bruiser) as `CreatureBehavior` subclasses.
3. **[T]** novel behaviors above, ~1–2 per chapter as worlds come online; **[A]** their data/loot/biome tie-in.
4. Each new creature: telegraph + counter + non-lethal disable + evolution/story note in its world README.

*Cross-links: `STORY_BIBLE.md` (Bloom/Pattern/Wardens), `CREATURES.md` + `CREATURE_DRONE.md` +
`DRONE_COMBAT_v1.md`, `09_GEAR_AND_TOOLS.md` (the counters), `OPTICAL_ILLUSIONS.md` (Witness-mite/Mimic).*
