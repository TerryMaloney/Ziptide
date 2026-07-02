# ABILITIES & ARSENAL — the expansion design (Terry directive 2026-07-02)

Four asks in one program: **more weapon types** · a new **ability-item category** sprinkled through the
story AND every mode · **dual-wield** that isn't overpowered · a **wrist locator rebuilt to A-grade**.
This doc is the buildable design; tasks land on the MP board (`SPRINT_MULTIPLAYER.md`) with story-side
placements flowing through `WorldJobLibrary`/`WORLD_DATA` as usual.

## 1. The arsenal roadmap (weapons)
The idea bank is `09_GEAR_AND_TOOLS.md` (40+ tools, categorized). Build order = gameplay-role gaps first:
| Wave | Weapon | Role | Where it appears |
|---|---|---|---|
| NOW (A4) | **Static Net** | thrown area-slow zone | arena pads · story vs swarms (W009!) |
| NOW (A4) | **Sonic Thumper** | melee AoE shove + wall-breaker synergy | arena pads · story vs Sound-walkers (W011) |
| NOW (A4) | **Prism Beam** | hold-to-charge lane beam, long telegraph | arena pads · story vs Light-grazers (W002) |
| Next | **Arc Rifle** (chain to 2nd target) · **Foam Cannon** (temporary cover!) · **Bloom Splicer** (story: converts Bridge-formers) | new tactical verbs | per-chapter with M5 |
**Rules that keep it sane:** every weapon ships with a `PvpRules` damage entry + a visible counter +
a `WeaponCharge` profile + a `BotProfileDefinition` preference weight (bots must use it) + a Gun Game
ladder slot. Non-lethal canon always. All stats live on the Definition asset.

## 2. AUGMENTS — the ability-item category (the new thing)
**What:** wearable/holsterable items that grant an ability — not a gun. Worn in a new **augment slot**
(belt-adjacent), one active + one passive equipped at a time (slot scarcity IS the balance).
- **Data model (⚙CI, mirrors the weapon pattern):** `AugmentDefinition : ItemDefinition` — `augmentKind`
  (Active/Passive), `cooldownSeconds`, `durationSeconds`, `effectId` (string → effect registry),
  `magnitude`. Effects resolve through a pure `AugmentEffects` registry (id → tested effect logic).
- **Launch set (six, all buildable on existing systems):**
  | Augment | Kind | Effect | Built on |
  |---|---|---|---|
  | **Surge Dash** | Active | the gravity-hop burst, any direction, 8s cd | PvpComfortHop (proven comfort) |
  | **Bubble Guard** | Active | 2s projectile shield sphere, 20s cd | bolt/dart collision filter |
  | **Overclock** | Active | 4s faster WeaponCharge recharge, 30s cd | WeaponCharge scale |
  | **Magnet Palm** | Passive | pickups/collectibles pull to hand from 3m | Collectible/ItemFactory |
  | **Sure Step** | Passive | hazard slows reduced 50% | HazardZoneRuntime multiplier |
  | **Sixth Sense** | Passive | locator cooldown halved + threat ping on incoming fire | LocatorState/WristScanner |
- **Where they appear (Terry: "sprinkled throughout, all modes"):** story = found in worlds as
  collectible-style pickups + crafted at machines + one per chapter tied to its biome/creature ·
  arenas = augment pads (pick-up-on-touch, drop-on-death — map control) · Horde = wave-clear reward
  choice · Tidefront = temporary war-buffs from won VR missions · Quarters shows your collection.
- **Balance laws:** one active + one passive equipped · actives never deal damage directly (utility
  only) · cooldowns are `PvpRules`-style constants · bots get the same augments at Veteran+ (fairness
  is symmetry).

## 3. Dual-wield (fun without the overpowered)
**The elegant fix already in the codebase: the SHARED CHARGE POOL.** Both hands draw from ONE
`WeaponCharge` (2 shots / 1.5s recharge) — dual-wielding gives you *flexibility* (two angles, two
weapon types, style) but the SAME sustained fire ceiling as one gun. No damage nerfs, no accuracy
fiction — the math simply can't be OP.
- Implementation: a `DualWieldCoordinator` on the rig links the two held guns' fire gates to one pool;
  holstering either gun returns to per-gun charge. Mixed pairs allowed (taser + gravity = the classic).
- Heavy weapons (Prism Beam, Sonic Thumper) are flagged `twoHandedOnly` on the definition — physically
  can't pair (the second grab is refused with a haptic buzz).
- Bots: Nightmare dual-wields (alternating hands = its cadence unchanged — cosmetic + intimidation).
- Gun Game: dual is disabled (the ladder is the point). Everywhere else: on.

## 4. Wrist Locator v2 (from C/D-grade to A-grade)
Today: functional pulse + flat radar disc. The rework (style AND function):
- **Form:** a proper forearm gauntlet silhouette (kit mesh queued with the art track's ART-4; until then a cleaner
  primitive assembly: angled faceplate, edge-lit rim, recessed lens) that sits along the forearm, not on it.
- **Charge language:** covering it charges a visible ring that fills with rising pitch + haptic ramp —
  release fires the pulse as an expanding ground-ring shockwave (not just a flash).
- **Radar v2:** holographic cylinder (not a flat disc) — blips get ELEVATION; enemy blips pulse to
  their distance; Fragment-carrier shows as a crown blip (Fragment Rush integration); scan-kind colors
  (enemy red / objective gold / loot cyan / node green) with a tiny legend etched on the rim.
- **Function upgrades:** persistent 8s afterglow trail on tagged enemies (through-wall silhouette fades
  over the duration) · cooldown shown as the rim de-lighting · **upgrade tiers via the ship's S3
  scanner slot** (range 20→30→40m, cooldown 60→45→30s) — the first upgrade-socket payoff.
- Split: timing/logic stays pure (`LocatorState` extended: afterglow window, tier params — tested);
  visuals/haptics are scene work; the gauntlet mesh is a Picasso item.

## Build order (slotting into the boards)
1. 🎮 **A4** (three weapons + pads-as-respawners) — already on the MP board.
2. 🎮 **A4.5 Augments core**: AugmentDefinition + pure `AugmentEffects` + slot/equip runtime + the six
   launch augments + arena pads + Horde reward hook. (Story/Tidefront placements follow via the
   libraries.)
3. 🎮 **A4.6 Dual-wield**: shared-pool coordinator + twoHandedOnly flags + Nightmare bot pairing.
4. 🎮 **A4.7 Locator v2**: pure LocatorState extension first, then the scene rework; the gauntlet mesh
   is queued with Picasso (ART-4 gear kits — see `docs/ART_PLUG_POINTS.md`).
Each chunk: tests + playbook rows + `PvpRules` constants + bot parity. Balance numbers are ALL data.
