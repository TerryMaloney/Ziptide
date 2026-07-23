# SPACE MISSION TYPES — the "why you fly," built FROM the story bible
### Terry 2026-07-21: "design the mission-type sets — these especially relate to our story bible, so look at it and make everything match."

**Status:** 🔵 DESIGN — zero code (freeze). Fills the gap-audit's biggest hole (LOOP) with mission
types that ARE the bible's devices in flight. Grounded in `STORY_BIBLE.md` §5 (mechanics-as-fiction)
+ §7 (recurring devices) + §4/§6 (factions + the chapter ladder). Follows the locked design fit
(`SPACE_DESIGN_COMPARABLES_AND_FIT.md`: exploration+salvage forward, soft-lock, no timers, combat =
optional spice). Reuses the existing `JobDefinition` + step contract idiom.

---

## §0 — THE PHILOSOPHY (from the fit doc + the bible's tone)

Space missions are **exploration + salvage forward, no timers, pacifist-viable** — and every one is
a bible device, not filler. The player earning passage across space *is the Shell keeping her
docile* (bible §5, passage credits = the leash); the wrecks she salvages *are the graves of prior
breakout attempts* (§5); repairing relays *wakes the universe* (§7, the Signal meter). So the space
loop tells the story by existing.

## §1 — THE STORY ENGINE THAT GENERATES MISSIONS (bible → missions)

- **The wreck thread (§5, §7)** = *"abandoned ships & space stations = wrecks of prior breakout
  attempts; every wreck has a log, a part you need, and a clue the Shell has been tested before."*
  → the backbone of SALVAGE + DISCOVERY missions; the logs read in order are the hidden second story.
- **Passage credits = the leash (§5)** → FERRY/contract missions are the economic spine (you fly to
  afford to leave — the system's design).
- **Maintaining the cage (§5)** → RELAY/SEAL REPAIR missions; late game they RE-TUNE (unseal).
- **The Signal / Pattern meter (§7)** → repairing seals/relays nudges Signal up → more Pattern bleed
  + more Warden attention. Missions that touch Signal have consequences.
- **The factions (§4)** own mission families: **Wake Guild/Mara** (get out — contracts, escorts, the
  W046 offer), **Sable** (tear it down — loud raids), **Wardens/Aegis-Nine** (the immune system —
  patrols, the W037 recognition, escorting the defector), **Architects** (dead — their derelict
  chambers + the endgame cloaked ship).
- **RILL** rides every mission (comms, the log, the de-garbling Transmission delivered in flight).

## §2 — THE MISSION TYPES (each: verb · bible tie · fit · Signal/pacifist · reuses)

### CORE / REPEATABLE (the everyday loop)
1. **SALVAGE RUN** — find a derelict/debris → disable any feral guardian → tractor the part/wreck →
   economy. *Bible: the wrecks are prior breakout attempts; a part you need + a waker's log clue.*
   Every chapter. Pacifist-viable (guardians evadable/stunnable). Reuses disable+tractor+economy+log.
2. **DERELICT DISCOVERY / LORE DIVE** — approach a bigger wreck/station, dock or scan it, recover the
   **waker's log + the mystery object + a Shell-tested-before clue**. *Bible §7 the wreck thread read
   in order = the hidden story; one mystery object per world.* Ramps mystery, feeds lore. Non-combat.
3. **FERRY / PASSAGE CONTRACT** — carry cargo/passengers dock→dock or world→world. *Bible §5 passage
   credits = the leash; ferrying IS earning your docile way off.* Early/mid economic spine. Non-combat.
4. **DISTRESS CALL / RESCUE** — answer a beacon: tow a stranded ship, defend or repair it. *Bible tone
   (warmth) + it mirrors OUR disabled-state rescue (`SHIP_ALERT` D2 — you were towed, now you tow);
   sometimes the stranded IS a prior waker.* Kindness texture. Reuses tow+repair+the disabled system.
5. **RELAY / SEAL REPAIR (orbital)** — fly to a broken orbital relay/seal, repair it. *Bible §5 you're
   maintaining the cage; §7 RAISES THE SIGNAL → wakes the universe, draws Wardens.* Late = re-tune.
   The Signal knob. Reuses repair + Signal + Warden response.

### STORY-GATED (unlock with the chapter ladder / factions)
6. **WARDEN PATROL / EVASION** — Warden patrols enforce containment; evade or non-lethally disable.
   *Bible §4/§6 the immune system; high Signal = more attention; the **W037 recognition** beat (a
   Warden knows RILL → Aegis-Nine).* Ch.6+. Pacifist-viable (evade). Reuses soft-lock + alert system.
7. **ESCORT / ASSIST** — protect a slow ship through danger (a Guild transport, a fleeing waker, the
   defecting **Nine**). *Bible §4 faction-trust beats.* Non-lethal. Reuses soft-lock + AI.
8. **FACTION OP / SABLE RAID** — the loud, destructive faction missions; **ally-or-oppose branch
   (Ch.4)**. *Bible §4 Sable's war = "the loud cost of the truth."* Mid-late; combat-forward (the ONE
   place combat leads — and even here, opposing Sable is a valid path). Reuses combat + branch flags.
9. **GATE RUN / PASSAGE** — fly THROUGH a Ziptide gate in space to a new system (the concepted
   exterior ship-crossing). *Bible: stepping through the membrane; new starfield = you went far
   (`CELESTIAL_SYSTEM_CANON`).* Traversal beat. Reuses the gate + system-jump.

### UNIQUE (once, endgame)
10. **THE APPROACH** — board the **cloaked Architect ship at Jupiter's L4/L5**, fly it in-system to
    **EARTH** (bible §8b Earth Approach). *The first time truly OUTSIDE the cage; real-world sky after
    60 alien worlds = maximum gut-punch.* Bespoke Earth space + the four-ending branch. Once.

## §3 — PROGRESSION MAP (mission types unlock along the chapter ladder — bible §6)

| Chapter | Unlocks | Signal / faction note |
|---|---|---|
| **Ch.1 (W001–04)** | first flight trial, simple SALVAGE + FERRY | "it's a job"; Signal ~0 |
| **Ch.2 (W005–12)** | DERELICT DISCOVERY (first breakout-wreck), Guild FERRY/contracts | "it's a cage"; Guild appears (W012) |
| **Ch.3 (W013–19)** | RELAY REPAIR (Signal starts rising), ESCORT (Guild) | Architects named; Signal ↑ |
| **Ch.4 (W020–28)** | FACTION OP / SABLE RAID (ally-or-oppose) | Sable's war; the loud cost |
| **Ch.5 (W029–38)** | Pattern-touched DISCOVERY/salvage (memory leaks in) | Pattern bleed; high Signal |
| **Ch.6 (W039–51)** | WARDEN EVASION + the W037 recognition + escort NINE | immune system active |
| **Ch.7 (W052–61)** | ARCHITECT-tomb derelicts, DISTRESS from prior wakers, GATE RUNs to far systems | outside comes into focus |
| **Ch.8 → Endgame** | **THE APPROACH** (Lagrange → Earth) → the four endings | the revelation + the choice |

## §4 — HOW THEY USE THE EXISTING JOBS SYSTEM (reuse, don't reinvent)

Space missions = space-flavored **`JobDefinition`s** (the same contract idiom as ground jobs), with a
few new step types alongside the existing `GoToMarker / Disable / Collect / Deliver / ShootTargets`:
- `TractorSalvage(targetId)` · `ScanDerelict(id)` / dock-and-read-log · `EscortTo(marker, ward)` ·
  `EvadePatrol(zone)` · `RepairRelay(id)` (raises Signal) · `AnswerBeacon(id)`.
- Rewards route through `ProfileEconomy` (salvage → parts/credits → ship upgrades/passage), same as
  ground. Story beats/flags (`ZiptideFlags`, `SIGNAL_THRESHOLD_*`, `PLAYER_*`) fire the same way.
- So a "space mission" is data + a few step assets — the World Compiler can author them like any job.

## §5 — THE PACIFIST PATH (canon: "disable, don't kill / pacifists still finish")

Every CORE type is completable without firing (salvage/ferry/discovery/distress/repair = evade or
stun). Only FACTION OP/SABLE RAID is combat-forward — and opposing Sable or refusing the op is a
valid branch. So a player who never fights can still fly, salvage, earn passage, and reach the
endgame — matching the ground canon exactly. Combat is spice, not a gate.

## §6 — BUILD / FREEZE / ⚖

- **Reuses:** `JobDefinition`+steps, `ProfileEconomy`, the Signal/flag system, soft-lock combat, the
  tractor/repair/tow systems, RILL lines, `CELESTIAL` systems + gates. Mostly data + a few space steps.
- **Order (post-checkpoint):** the core five as job types → the space step assets → the Signal hook →
  faction ownership + the chapter-ladder gating → the unique Approach.
- **Freeze:** all post-Golden-Checkpoint; this is the design + the map.
- **⚖ for Terry:** the exact per-chapter mission counts/density · whether GATE RUN is a mission or just
  traversal · how hard the Signal consequence bites (Warden aggression curve).
