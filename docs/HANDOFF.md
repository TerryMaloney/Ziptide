# HANDOFF — session log (single operator ⇄ Terry)

## Required reading

- `docs/FABLE5_START_HERE.md`
- `docs/HANDOFF.md` — current entries (newest 10 only; see the archive below)
- `docs/FAST_LANE.md` — **read this before doing a small fix**; it says what ceremony you may skip
- `docs/HANDOFF_ARCHIVE_2026-07.md` — entries through 2026-07-28 (search, don't read)
- `docs/HANDOFF_HISTORY_THROUGH_RB24.md` — exact prior history through rb24
- `docs/DEVICE_STABILIZATION_FORENSIC_PLAN.md` — Quest device recovery plan
- `docs/recovery/RECOVERY_PROGRAM.md` — active recovery authority, freeze, proof levels and R0–R4 path
- `docs/recovery/SYSTEM_CONTRACT_INVENTORY.md` — initial critical-system ownership map
- `docs/recovery/AUTOMATIC_RUNTIME_OWNERS.md` — automatic bootstraps, persistent mutators and feature injectors
- `docs/recovery/CANONICAL_OWNER_DECISIONS.md` — proposed surviving owners and displaced paths
- `docs/recovery/R1_INTEGRATION_HARNESS_SPEC.md` — locked next-phase verification design
- `docs/recovery/generated/` — generated source, scene, claim and ownership reports
- `docs/MASTER_CHECKLIST.md`
- `docs/FABLE5_BACKLOG.md`
- `docs/TERRY_RUNBOOK.md`

## Rules

1. Read the newest entry before work.
2. Append Did / Next / Heads-up / Commit at session end.
3. Work on `terry-local-wip`; pull before starting.
4. Keep commits small and stop implementation when required CI is red.
5. **Small, reversible change?** Use `docs/FAST_LANE.md` — one line in `FAST_LANE_LOG.md`
   instead of an entry here. Full ceremony is for structural work, not for a rotated weapon.
6. **Any check that only reads `docs/**` belongs in `tools/*_gate.py`, never in a Unity test.**

---

## ENTRIES — newest first

### 2026-08-01 (rb137) — 🔭 MEGA PASS ON LEVEL 1: the first level had no sky of its own

Terry, heading for the headset: *"let's do one more mega pass on the whole first level."* One hour,
looking for anything that would waste a device session. Two real holes, one of them mine.

#### 1 · 🚨 ToxicCity was the only scene in the game with no theme

It pointed at the shared `DefaultWorldProfile`. Which means the sky its own layout has always
authored — olive horizon `(0.62, 0.55, 0.34)`, dark teal zenith, a 22° occluded body — **has never
once been rendered.** W000–W012 author a theme from their layout via `WorldStubGenerator`; the five
arenas do it through `ScenePatcherArena`; the space lane does it. The level the game opens in did not.

The knock-on was bigger than the sky. `VisualThemeProfile.skyVista` is the seam the entire vista
system rides, so with no theme there was **nowhere to hang W001 Toxic Venice** — the vista I built the
acid haze for two days ago. And this was not undiscovered. It is written in two files:

> `SkyVistaLibrary`: `Spec("ToxicCity", …)  // reserved: assigned once ToxicCity gains a theme`
> `SkyVistaAuthor`: *"Scenes whose theme doesn't exist yet (e.g. ToxicCity until its patcher authors a
> theme) are skipped silently — the vista asset waits."*

It waited. **A TODO in a comment is not a task, because nothing ever asks it whether it is done.**
`SkyVistaAuditRules` even warns `SKY_VISTA_UNWIRED` on every run — a warning nobody reads is the same
as no warning. Now `level1_wiring_gate` asks on every push (feature 25, "first level sky").

⚠️ **This changes what ToxicCity looks like.** It is the intended change, and the runbook says so, but
if the sky reads wrong on device this is the commit.

#### 2 · My own bug from this morning: the first walk was one metre long

`ShipyardApproachAuthor` derived its span from the berth's **landward** edge to the Shipyard
district's **seaward** edge. Both names read as "the walk". Those two edges are adjacent — the golden
bridge spans the gap — so the span was **1 m** and nine pieces of junk would have been dumped in a
heap on the bridge.

CI could not catch it: the tests passed because they used invented coordinates (−40 → −22) that
matched my mental model rather than the shipped berth. **A derived span needs its arithmetic checked
against the real numbers, not just its variable names.** Tests now use the shipped berth, and the
corridor is defined as what it actually is — the ship's beam, because the thing standing in the middle
of that deck is your own hull.

#### 3 · Checked and clean

All 24 `tools/*_gate.py` pass or sit on declared holds. Zero audit blockers repo-wide.
`level1_wiring_gate` 25/25. The armoury rack self-seeds and grants the starter pair on first boot, so
a fresh save cannot strand the player unarmed. `CITY_NO_COURTYARD` on ToxicCity is a **substring
check** looking for a GameObject named `*Courtyard*` — the Plaza district is the courtyard; renaming
to satisfy it would be gaming the audit, so it stays.

#### 4 · Noted, not changed

**`ScenePatcherToxicCity.SpawnStarterWeapons` drops a loose taser and gravity gun at the Dispatch
spawn, and the ship's rack now grants the same pair.** Two free guns on the plaza quietly undercut
"the ship is the armoury" (⚖ Terry). The fix is probably to delete the loose pair — but the player
spawns at Dispatch, not aboard, and I could not prove from here that they always reach the ship with a
weapon. **Deliberately not touched before a device session**, because the failure mode is a player
with nothing to fight with. Worth ten minutes with the headset on.

#### Commit

`94b53a79` (rebased to `78babef9`).

---

### 2026-08-01 (rb136) — 📏 THE YARD HAD NO RULER: the crane, the first walk, and the lamp that taught nothing

Terry: *"if this is the first thing we see outside the ship we really want to build a sense of awe and
perspective… the hangar was pretty much just like a bunch of sort of box areas, not very good."*

The diagnosis in `docs/project_art_plan/HANGAR_AND_COUPLER_PASS.md` §1 was measured, not felt: the
Shipyard district is 22 × 18 m with **one 16 m × 2 m crane, one 7 × 7 office, and `props: []`**. The
problem was never that the shapes are boxes. **Nothing in the space stated its own scale.** A 16 m box
and a 1.6 m box are the same box until something in frame has a size the player already knows.

#### Did

1. **§2.1 — the crane stops being a stick.** `LandmarkScaleCore` (Content, pure) owns rung pitch
   (locked to the 25–35 cm band a real ladder uses — the trick is that the player *already knows* the
   distance), slenderness (4:1), and a **7.5 m detail ceiling** so a 40 m tower costs what a 16 m one
   does. `CityBuilder.BuildLandmark` builds mast · ladder · walkway + rail posts · cab · jib + hook;
   only the mast keeps a collider. Crane 2 m → **4.5 m**, moved x=8 → x=5 because at the new width the
   old position intersected the east facade row.
2. **§2.5 — the arrival walk is lit.** The lantern route started *at* Dispatch, so the player crossed
   the whole quay unlit and then arrived at a lit city with no idea the lamps meant anything.
   `ArrivalWalk` (Quay → Shipyard → Dispatch), kept **separate** from `JobRoute` because that one is a
   loop that must come home.
3. **§2.4 — the foreground band exists.** `ApproachClutterCore` places nine pieces along the walk. The
   interesting half is the **corridor**, not the scatter: junk in the walking lane is a soft-lock, and
   the hiding kind — a VR player cannot see their own feet.
4. **§2.5 — one grabbable crate worth nothing.** `LooseCrate`: Rigidbody + `XRGrabInteractable`, no
   `ItemDefinition`, never enters the inventory. The game's first tutorial is a piece of junk, and it
   only works if the junk is really junk.
5. **§2.2 — there is something overhead.** Three gantry arches and two runners over the **landward
   55%** of the berth, trusses at 6.2 m, lamps at 4.6 m. The half is the part worth defending: a roof
   over the whole berth would seal off the sky, and the skyscape is the thing this project is most
   committed to. **A scale win that costs the horizon is not a win**, so `GantryRoofCore` owns the
   ratio and a test fails the day someone extends the roof "just a bit".
6. **§2.3 — the crane hook creeps.** `CraneHookCore` + `CraneHookRuntime`. Two counter-intuitive
   numbers, both now pinned: **slow** (past ~0.35 m/s a hook stops reading as tonnage on a cable and
   starts reading as an animation at the wrong rate) and **eased at both ends** (an instant reversal
   on a heavy object is the clearest tell that nothing here has mass). The cable stretches, because a
   fixed-length one detaches from the jib.

#### Two misses worth the ledger

- **The spec is upstream of the layout asset and I nearly fixed only the asset.**
  `WorldSpecCompiler.ApplyToLayout` calls `FindLayoutBySceneName`, so compiling
  `docs/worldspecs/ToxicCity.spec.json` overwrites `Content/City/ToxicCityLayout.asset`'s districts
  **wholesale**. The crane widening would have silently reverted the next time anyone ran
  *Compile World Specs*. Same shape as #24/#25: right fix, wrong level. Both levels now carry it and
  `CityLandmarkAuthoringTests` checks both.
- **Two lanterns have been z-fighting on Dispatch since the compass shipped**, because `JobRoute` is a
  loop and names Dispatch twice. Found only because merging a second walk forced the question.
  `MergeLanterns` de-duplicates within each list, not just between them.

#### Next

**§2 of the hangar pass is done except one item.** What is left:

- **§3.2 the coupler concept prompt** is written and ready for Terry to run — it unblocks the modelling
  and nothing is built yet. This is the top of the list.
- **§2.4 background band** — frame the walk so the player exits the ship facing the skyline rather
  than the office wall. Needs an eye in the headset, not arithmetic in a container.
- **`W000_DriftIn`'s GantryCrane is 10 m × 2 m** — the same defect, in the first room of the game. Its
  BerthBay is small and ringed by facades near the crane, so the widened footprint needs a placement
  **eyeballed in the scene view**, not derived from bounds arithmetic in a container with no editor.

#### Heads-up

- `LandmarkKind` is a new field on `LandmarkDef`. **Tower renders exactly the single cube it always
  did** — twelve worlds carry landmarks that are thin on purpose (W002's LightShaft, W003's prisms) and
  scale detail is opt-in per landmark, deliberately.
- `docs/worldspecs/ToxicCity.spec.json` now carries `kind` on every landmark. If a re-export drops the
  field, the crane goes back to being a tower.

#### Commits

`70073d2a` crane · `ff960f6b` arrival walk + spec · `5297eb89` foreground + teaching crate ·
`77a3ba96` the hook creeps · plus the gantry roof commit below this entry.

---

### 2026-07-31 (rb134) — 🕳️ THE HOLE HUNT: the player cannot be hurt · the first planet had no air · half the arsenal was unobtainable

Terry: *"we want everything hypothetically working perfectly to be AAA game minus some of the artwork."*
So this session hunted holes rather than adding features. Four real ones, three fixed.

---

#### 1 · 🚨 THE BIGGEST ONE: the campaign player cannot be hurt (NOT FIXED — next operator's task)

`PlayerStunReceiver` says it in a comment: **"NO health, NO death."** `CreatureDefinition.damage` is
authored on every creature in the game and **applied to nobody**. Combat has no stakes anywhere in the
campaign.

`docs/systems/COMBAT_HEALTH_PLAN.md` diagnosed this on 2026-07-07 from Terry's own brief (*"we can't
just have more characters be invincible forever"*). **It never got built because its header said
"PROPOSAL awaiting Terry's two design decisions (§2)" while §2 says "DECIDED (Terry, 2026-07-07)".**
Three weeks behind a stale status line. Header corrected; real state now tabulated there.

What is actually left:
- **A.3** — `ItemDefinition.damage` exists, is **read by nothing**, and is unserialized (0) on every
  weapon asset. Note before "fixing" it: A.2 makes `PvpRules` the single canonical scale, so a second
  authoritative damage field is the disagreement the plan exists to END. Derive it or delete it.
- **A.4** — no guard test, which is why A.3 rotted invisibly.
- **B** — the player's armor on the rig. This is the actual work and it is **not blocked on a
  decision**, only on someone doing it.

⚠ **`ArmorMeter` ALREADY EXISTS** in `Ziptide.Multiplayer`, with `PlayerCombatState` and tests. I wrote
a complete second one in `Core` — pure type, twelve tests — before finding it, and deleted it. The trap
is A.1's own wording (*"put it beside `PvpCombatant` in `Multiplayer`, or in `Core` if `Gameplay` needs
it"*), which reads as create when it meant move. **Do not ship a second combat system into a game that
has one.**

---

#### 2 · The first planet had no air — FIXED (`adc108f4`, CI GREEN)

SKYSCAPE_DESIGN is a north star; its §6 step 1 was proven on **W005, a world the first level never
reaches**, while ToxicCity — the first planet the player ever stands on — shipped with perfectly still,
perfectly clear air, failing rubric §5.1 and §5.2. The stack was never the problem; the REACH was:
atmosphere only arrives via `SkyPlanetRig`'s vista path, which needs a generated theme, and ToxicCity
has none. `SKY_VISTA_UNWIRED` had been warning about exactly this, owned by nobody.

`WorldAtmosphereBinder` drives **only** haze and motes from an authored vista, never the dome, bodies,
palette, fog or light — so a world keeps its look and just gains air. That additive property is why it
could land before a device session instead of after one.

**A gate correctly tried to reject it.** `GateGap2_SignatureRubric` requires pillar 4 (the sky's colour
reaching the ground). ToxicCity satisfies it via its green fog, not via the vista, whose light fields
are never read there. Setting those fields would have been a hollow signature — the exact thing that
gate was written to catch, inverted. The law now models both regimes: a vista either owns its ground
coupling or **names what does** (`atmosphere.groundCouplingOwner`), and claiming both is a failure
because it hides which is real.

---

#### 3 · Half the arsenal was unobtainable — FIXED (`52fee309` CI GREEN, `d655ac0c` rack)

Eight weapons authored. **Four — `prism_beam`, `sonic_thumper`, `static_net`, `tide_pike` — registered,
forge-recipe'd, tested, and placed NOWHERE.** The other four were dropped on the ground at the player's
feet; those two `ItemFactory.Create` calls were the entire placement strategy. And they could not have
been belted if found: `HolsterSocketInteractor` hardcoded a five-id allowlist.

⚠ **The trap:** the public `AllowsItemId` is **called from nowhere**. `CanSelect`/`CanHover` go through
the private instance `ItemIdAllowed`. Editing the public one looks exactly like a fix and widens nothing.

`WeaponCatalog` (Core) is now the single truth for what is a weapon and what may ride the belt.
⚖ Terry: **the ship is the armoury** — a rack by the hatch, and DISEMBARK refuses until a weapon is on
your belt, with two different RILL lines because "unarmed" and "holding but not holstered" are
different mistakes and the second player has already done what the first line asked.

**`ShipArmouryCore` carries a NO-TRAP LAW:** the gate sits between the player and the entire game, so it
never refuses when the player cannot comply — empty rack plus empty belt OPENS. A missing-item bug costs
a confusing walk, not a dead save. Same instinct as the stalker's six-second block yield.

**Deliberately left:** the two starter weapons still spawn on the plaza floor. Deleting the only other
source of weapons before the rack is device-proven is how a no-trap law gets undone by hand. One line,
after the headset.

---

#### 4 · Open holes found and NOT fixed

- **The first city's interiors are empty boxes.** `CityBuilder.BuildHeroBuilding` = floor + ceiling +
  4 walls + door gap + invisible marker + **one accent cube**. That is Dispatch (where the contract is
  accepted), the Shipyard Office and the Relay Vault. `RoomFurnishCore`/`InteriorFurnisher` (16 kinds,
  10 tests) exists and is unreachable there.
- **ToxicCity generates no lot-based buildings at all** — its districts carry no `buildingStyleId`, so
  the whole `BuildingBuilder` grammar/interior stack never runs in the first city. Bigger than one
  session; it is an architectural decision, not a bug.
- **The ship interior is three markers** (bunk, helm, porthole). The exterior is genuinely good. The
  armoury rack is the first real furniture the inside has ever had.
- **W002 has no dust** — §4.1's `vibration`/`cave-in` row. Nearly free now the binder exists.
- `ZiplineRuntime.IsDesignatedArrival` still public, tested, called by nothing.

#### 5 · On the canal stalker

Terry asked for "the river fight." **It is deliberately not a fight** — `CanalStalkerCore` carries his
own ⚖ marker: *"interacts with the BOAT, in the water, never on land."* Shadow → Bump → Block, never
lethal, always yields. Asked; he chose **keep the escort, give it a payoff**. Do not turn it into a
damage encounter.

#### 6 · Phase B LANDED after all (`911ccd1f`) — but read the caveat

§1 above says Phase B is the next operator's task. It isn't any more; it turned out small, because the
hard part was already built and mislabelled. `PlayerCombatState` (Multiplayer) is a complete, tested
implementation of the whole armor rule and **its own summary dictates the shell's shape**: *"the
MonoBehaviour (`PlayerArmor`) should be a thin translator: forward hits in, read outcomes out, never
decide anything itself."* So `PlayerArmor` is exactly that — one entry point, reuses the stun receiver's
existing flash and incoming-fire tracer, and takes §2's serverless checkpoint (the world's own
`__SPAWN_PLAYER` marker). Hosted by `PlayerStunReceiver`, so no new automatic owner to register.

`PlayerStunReceiver`'s summary no longer ends "NO health, NO death".

**⚠ CAVEAT — behaviour on device is UNCHANGED.** The damage SOURCES are not wired: creatures and drones
still never call `ApplyDamage`. You can still walk through a drone patrol untouched. That is the next
slice, it is small, and it is the one that genuinely needs a headset — "how many hits until my armor
breaks" is a feel question no test answers. Landing the shell separately keeps that slice small and
keeps this one provable. PlayMode **43/43** on `911ccd1f`.

#### 7 · ⚠ `docs/CI_VERDICT.md` UNDER-REPORTS what is verified — read this before believing it

The committed verdict names **`52fee309`** as the last GREEN. That is stale and **pessimistic**: two
later code commits also passed. The verdict job refuses a write when the head has moved to a path it
does not tolerate (it allows only generated evidence, `CI_VERDICT.md`, `HANDOFF.md`,
`handoff_queue/**`) — and I pushed docs touching `TONIGHT_TEST_CARD.md`, `MISS_LEDGER.md` and
`EXCELLENCE_MAP.md` within a minute of each code push. The guard is right; my sequencing was wrong
(MISS_LEDGER #26). **Land docs before the code they describe, or batch them into it.**

Authoritative results, by run id — cite these, not the file:

| Commit | What | Run | Conclusion |
|---|---|---|---|
| `adc108f4` | atmosphere | `30627442522` | ✅ success (run-level) |
| `52fee309` | weapon catalog + armoury core | `30630600884` | ✅ success (run-level) |
| `d655ac0c` | rack + disembark gate | `30633631182` | ✅ success (run-level) |
| `911ccd1f` | Phase B armor shell | `30645974326` | ✅ EditMode + audit green; PlayMode **43/43** |

**Commits:** `adc108f4` (atmosphere) · `52fee309` (weapon catalog + armoury core) · `d655ac0c` (rack +
disembark gate) · `a1f12982` (combat plan status) · `911ccd1f` (Phase B shell). Earlier today: the
`ApplyProcessors` NRE proven fixed (PlayMode 43/43 ×3) and `WorldPackAuditRules`.



### 2026-07-31 (rb133) — 🔧 the `ApplyProcessors` NRE, FIXED at its real cause: the repair only ran after a travel

**Read §1 if you take any input work.** rb132's fix direction ("disable the turn provider across the
consolidation window") turned out to be the wrong diagnosis, and following it would have added a new
suspend/resume race to a system that already had a correct fix sitting in the tree, unreachable.

---

#### 1 · The fix was already written. It was installed on one trigger.

`docs/systems/VR_RIG_GOTCHAS.md` #9 nailed this on 2026-07-20: ZIPTIDE's control law is left-stick
move / right-stick turn, so the left-hand turn and snap properties are authored as embedded direct
actions with **zero bindings**. Disabling one is not durable — XRI's `OnEnable` runs
`EnableAllDirectActions`, revives it, and the next `ReadInput` enters the Input System with no
binding state and throws inside `InputActionState.ApplyProcessors`. The durable spelling is a **null
action**, and `InputMutationRepairDriver.ClearInertDirectProperties()` did exactly that.

**The mutation was right. Its trigger was wrong.** It lived inside the post-travel repair, and that
driver's `Update()` opens with:

```csharp
if (!_sawTravel) return;      // _sawTravel is only set by a TravelCoordinator trip
```

So **cold boot never swept**, and neither did any PlayMode test that merely loads a scene. `_Boot`'s
placeholders stayed enabled and the identical crash kept landing on whichever test read first — which
is precisely the 1–3 test wobble rb132 tabulated and three sessions read as flakiness. rb130 §4 saw
one name, rb131 saw another, rb132 saw a third; one defect, three costumes.

**Shipped (`e0fad407`):**
- `LocomotionInertActionSweep` — one implementation, with the asymmetry stated: a property carrying an
  `InputActionReference` is **never** swept (the asset owns its binding state; nulling it would delete
  real input). Only a reference-free, map-free, zero-binding action qualifies.
- The driver runs it at **install (`OnEnable`)**, on **every `sceneLoaded`**, and in the post-travel
  repair. The log now carries `reason=installed|scene_loaded:<name>|post_travel_repair`, so which
  trigger fired is visible in a logcat instead of inferred.
- Both editor authoring sites (`LocomotionContractEnforcer`, `EnsureLocomotionRig`) now say why the
  placeholder exists and who clears it. Unity cannot serialize a null embedded action, so the
  build-time half legitimately authors the placeholder — the pairing just has to be legible.
- 7 EditMode tests: both directions (clear too little → crash, clear too much → delete real input)
  and all three provider types.

**No protected owner was touched.** `PlayerInputSessionGuard`, `PlayerRigPersistence` and the input
action assets are unchanged; the whole change is inside the driver the guard already installs.

**✅ Proof status — the three-run bar was set and then met.** An intermittent defect is not cleared by
one green run (the part-for-whole trap of MISS_LEDGER #20), so the bar was three consecutive 43/43
PlayMode observations on identical Unity source. All three landed:

| Run | Tested SHA | Result |
|---|---|---|
| `30624847856` | `a7508da6` | **43/43** |
| `30625474855` | `65e63f8f` | **43/43** |
| `30626042812` | `886da9ee` | **43/43** |

All three SHAs are bot-authored `[skip ci]` docs/verdict descendants of `8c8af7dc`;
`git diff 8c8af7dc 886da9ee -- Ziptide/` is **empty**, so the Unity source under test is byte-identical
across the three. For context, the same lane read 41/43, 40/43 and 42/43 across the three runs before
this fix, with the failure moving between tests each time.

CI on `8c8af7dc` is **run-level success** (run `30623881031`) — EditMode, patch-scenes + world audit,
and Android all green, not one job read as the whole.

One caveat that is not a hedge: 43/43 proves the crash is gone from the recovery route in the editor.
It does not prove turning *feels* right on device. That is Terry's verdict, beat 2 of the test card.

---

#### 2 · Every WorldPack is now checked at build time (`5982e469`)

MISS_LEDGER #21's open half, safe to land now that no bake is imminent. `WorldPackAuditRules` runs
`WorldPackValidator` over every generated pack as a project-wide section of `WorldAuditRunner`, in the
order the build already uses (patch all scenes → audit). **WARN-only** per the `PerfBudgetAuditRules`
ratchet — a new blocker in the audit aborts Terry's local build, so it earns promotion after one clean
run. An empty pack set reports `WORLD_PACK_NONE_FOUND` rather than passing silently.

**Correction to my own rb131:** I wrote that `WorldPackValidator` "is called from nothing but its own
unit tests." Wrong — `JobDirector.cs:37` calls it at world entry. The gap is narrower than I stated
and still real: that is a runtime check, on device, after you have already travelled into the broken
world, and only for the world you entered. I read the test call sites and generalised. Same class as
MISS_LEDGER #20; recorded there.

---

#### 3 · State

| Item | Where |
|---|---|
| CI | ✅ **run-level success** on `8c8af7dc`, run `30623881031` |
| Golden Android | ✅ `8355ca07` (unchanged by this work) |
| PlayMode | ✅ **43/43 ×3** — see the table in §1 |
| `level1_wiring_gate` | pass, 22 features, 0 findings |
| `tools/tests` | 297 OK |
| Headset | ❌ **still has not happened.** `tools/level1_test.ps1` + `quest_capture.ps1` + `docs/production/TONIGHT_TEST_CARD.md` are ready and unchanged |

**Two CI reds on the way here, both mine, both test-side** — an asmdef reference my test file needed,
then two assertions whose expected counts the package's own defaults contradict. Neither red
challenged the shipped fix. Circuit breaker closed at 2 of 3; MISS_LEDGER #23 records the class.

#### 4 · Next

1. **The headset run.** Everything else on this list is cheaper than one device session; that is the
   bottleneck now, not code.
2. Promote `WORLD_PACK_INVALID` from Warning to Blocker. §2's ratchet condition — one clean audit run
   — is **met**: the patch-scenes + world audit job was green on `8c8af7dc` with the new section live.
3. Still unbuilt from rb130 §3: W002's defend wave / garden plot / glyph plate · pause+settings board ·
   title + legal/credits · all music and VO · all final art.
4. `ZiplineRuntime.IsDesignatedArrival` is public, tested, and called by nothing — either wire it or
   delete it.

**Commits:** `e0fad407` (input sweep), `d55c81fa` (pack audit), `8a798183` + `8c8af7dc` (my two CI
reds, both test-side), `13d3e6f4` (docs).


### 2026-07-30 (rb132) — 🔬 the last PlayMode red, DIAGNOSED: it is the Input System `ApplyProcessors` NRE, still alive

**Correction to rb131 first:** I wrote "the PlayMode red is cleared". It was not, and I claimed it
from an offline reproduction instead of the lane — the same part-for-whole error as MISS_LEDGER #20,
one day later. What my registration actually did is narrower and still worth having.

#### What the three runs actually say

| Run | Result | Failing |
|---|---|---|
| `30f3ed84` (before) | 41/43 | `ActualBoot_SettingsSelectsThroughXri` · `RuntimeBootstrapDiscovery` |
| `8355ca07` (my registration) | 40/43 | `ActualBoot_…` · `GoldenRoute_…` (timeout) · `NewGame_W000_ToxicCity_W000` |
| `b4fb5ec8` (re-run, dispatch) | **42/43** | `NewGame_W000_ToxicCity_W000` only |

- **`RuntimeBootstrapDiscovery` is gone in both post-fix runs — the catalog registration worked.**
- **The other failures are ONE defect wearing different names.** `ActualBoot_…`, `NewGame_…` and the
  `GoldenRoute_…` timeout all carry the same signature, and which test catches it moves run to run.
  That is why the count wobbles 1–3 and why rb130 §4 saw only one of them.

#### The defect, with its stack

```
InputActionState.ApplyProcessors        (com.unity.inputsystem@1.6.3 :2820)
InputAction.ReadValue<TValue>()         (:992)
ActionBasedContinuousTurnProvider.ReadInput()   (com.unity.xr.interaction.toolkit@2.4.3 :62)
ContinuousTurnProviderBase.Update()     (:35)
```

**This is the exact failure rb36 called "ONE residual poll" and rb37 believed the 1.6.3/2.4.3 package
matrix had resolved. It is still reproducing.** XRI's continuous-turn provider calls `ReadValue` on
its turn action and the Input System throws inside the processor chain — the signature of an action
read against state that was re-resolved underneath it (the travel/scene-load boundary).

**⚠ Do not be misled by the adjacent log line.** The stack printed next to it is
`HomeHubRuntime:LogAimProbe` (`HomeHubRuntime.cs:452`), which is only the `BOARD_PROBE` diagnostic's
own `Debug.Log` stack landing beside the exception. `LogAimProbe` reads no input action. **The NRE is
entirely inside the packages; no Ziptide frame appears in it.** I checked before writing this,
because "our class is in the log" is exactly how a package bug becomes a week of chasing our code.

#### Not fixed here, deliberately

Input-session and locomotion are protected owners, `CLAUDE.md` puts input actions in the
report-only/get-confirmation column, and Terry was mid-headset-session on a build already made —
changing runtime input underneath a live test invalidates the evidence he is collecting. **Fix
direction for whoever takes it:** the provider must not read its action across the window where the
session guard re-consolidates the action asset — either the turn provider is disabled for that
window, or the read is guarded on the action being enabled and bound. Reproduce by re-running the
PlayMode lane a few times; it is intermittent, so a single green run is NOT proof.

**On device it would look like:** smooth/continuous turning dying, or a burst of
`NullReferenceException` right after boot or immediately after a travel.

**Commits:** none (diagnosis only). Golden Android and CI patch+audit are green on `a198112e`, so
this does not block the device session.


### 2026-07-29 (rb131) — 🚨 A LEVEL-LOCK FOUND AND FIXED before the headset, + the PlayMode red cleared + the hangar walk built

**Read §1 first: the first level could not be completed, and the reason was not on anyone's list.**
Took over from rb130 with Terry driving home. Verified rb130's state claims (all accurate; §0 below),
then audited the whole Level 1 flow rather than only the items rb130 named as unbuilt — which is how
the real blocker surfaced.

---

#### 0 · VERIFICATION OF rb130 (do this before trusting any handoff, including this one)
Every workflow on `30f3ed84` matched rb130's table exactly: CI ✅ · Fast Preflight ✅ · Golden
Android ✅ · Contract Scan ✅ · Owner Proof ✅ · **PlayMode ❌ 41/43**. `level1_wiring_gate` clean,
`tools/tests` 297 OK. rb130's §0 was honest.

---

#### 1 · 🚨 THE LEVEL-LOCK: the contract's relay machine never existed (`8355ca07`)

`ToxicCityContractBuilder` step 4 is `RepairMachine("signal_relay")`. `ScenePatcherToxicCity`'s
`EnsureWorldPack` set packId/displayName/sceneName/spawnMarkers and **never touched
`pack.machines`** — and `JobDirector` materialises repairables **from the pack**. So no object in
the shipped world carried that id.

**Consequence:** the Dockmaster's Bounty stopped at **step 4 of 6**. Step 5 (drive out through the
breach to the flats — *how you get artifact half B*) and step 6 (return to the berth) sat behind a
step that could never complete, and **`toxiccity_complete`, the flag gating W002, was ungrantable.**
The back half of the level was reachable by wandering but not by playing the contract.

**Why nobody saw it:** generated worlds pair the halves automatically — `WorldJobLibrary.Repair()`
is literally documented "pair with a `Machine()` entry", and the same spec writes `pack.machines`.
**ToxicCity is the one world that hand-writes its contract and its pack in two different files**, so
the pairing was a convention with no mechanism behind it.

**Fixed:** the machine is authored into the pack at the RelayVault its own `relay_node` marker lives
in, read off the LIVE layout (not typed coordinates), part id `relay_cell` (the item already exists;
`RepairableMachine` uses the id as a label and builds its own geometry, so nothing else can break).
**Guard:** wiring-gate feature 22 binds the contract step to the pack machine — **mutation-tested,
removing either half reds CI.** → **MISS_LEDGER #21.**

**⚠ The systemic half is OPEN and is the highest-value next task:** `WorldPackValidator` *already*
contains the rule that predicts this exact defect in words — "Repair 'X' but the pack spawns no such
machine — likely un-completable" — and **it is called from nothing but its own unit tests.** Wire it
into `WorldAuditRunner` as a project-wide pack report (WARN first per the `PerfBudgetAuditRules`
ratchet, promote after one clean run). I deliberately did NOT do this mid-session: a new audit
blocker aborts Terry's local bake, and he is about to run one.

---

#### 2 · THE PLAYMODE RED IS CLEARED (`d859d69b`) — rb130 §4 executed

Registered all four uncatalogued bootstraps exactly as rb130 specced: `Required` (not `Gated`, so no
runtime file is edited), one `RecoveryFeatureId` each, `GoldenFeatures` inclusion (every
AlwaysRequired owner must be allowed by every profile), and the mirrored R0 owner records.
**Two of the four — `FirstHourDirector` and `FirstHourW001Orchestrator` — drive five of the
twenty-two first-hour beats, so this was Level 1 critical path, not paperwork.**
Verified offline against all three PlayMode assertions before pushing: 25 discovered bootstraps all
catalogued · 29 enum ids = 29 registrations = 29 owner records · no `Required` id missing from
GoldenFeatures.

---

#### 3 · THE HANGAR WALK IS BUILT (`6d7012de`) — rb130 §3's berth row closed

Berths 1–5 west of your own at the script's 14 m spacing: walkable decks, mooring bollards, and the
berth number told in **tally bars, not text** — countable by a player who cannot read, and immune to
the Quest-resolution problem rb130 flagged for the helm's `TextMesh`. Empty on purpose: five slips
that used to hold something is what makes berth six read as home, and it is the runway the beacon
thread descends. Positions come from a pure core measured off the live berth; `QuayBerthCoreTests`
pins the property that actually bites — five coplanar decks overlapping would z-fight the length of
the quay — and the guard is proven to bite, not merely to pass. Wiring-gate feature 21.

---

#### 4 · ⚖ CORRECTION TO rb130 §3: the zipline IS built and wired

rb130 listed "the zipline — placement as data was never authored". It **is** authored:
`ScenePatcherToxicCity.EnsureFirstHourRoute` strings it Plaza→CanalRow, and `FirstHourDirector`
subscribes to `RideEnded` and accepts `FIRST_JOB_ZIPLINE_USED`, so beat 19 fires. The obvious worry
— a nearly flat line that will not slide — is already answered inside the core: `ZiplineRide` has a
2 m/s `kickSpeed` whose documented purpose is "so flat lines still move". §3 is corrected in place.
*(Minor, not chased: `ZiplineRuntime.IsDesignatedArrival` is public, tested, and called by nothing —
the director accepts ANY zipline's arrival. Harmless today because ToxicCity has exactly one line;
worth binding when a second one exists.)*

---

#### 5 · THE FLOW AUDIT — what I actually verified end to end

- **All 22 first-hour beats have a producer.** 17 accept-tokens in `FirstHourDirector`, 3 in
  `FirstHourObservationAdapter` (look/move/arrival), 2 in `FirstHourW001Orchestrator` (observe,
  payoff). No orphan beats.
- **Every proof tag in `LEVEL1_BAKE_AND_SMOKE` §3 has a non-test producer** — all 16 checked
  (`KEY_SEATED`, `ARTIFACT_JOINED`, `SALVAGE_FIND`, `RESONANCE_TELL`, `SKIFF_WATER`, `STALKER stage`,
  `RING_LIGHTS`, `DRONE_MOOD`, `REENTRY_ARRIVAL`, `VEIL leg`, `FLIGHT_DEPART/BLOCKED`, …).
- **Every ToxicCity contract step resolves:** s1 `dispatch_inside` ✅ · s2 five drones ✅
  (Patrol_Market 3 + Patrol_Canal 2 authored) · s3 `relay_node` ✅ · **s4 `signal_relay` ❌ → fixed
  above** · s5 `flats_site` ✅ (FlatsSiteAuthor) · s6 `shipyard_office` ✅.
- **Scene chain:** `_Boot` · `W000_DriftIn` · `ToxicCity` · `W002_DryCistern` all enabled in Build
  Settings; `SpaceLane_Trial` is created and enabled by the bake hook rb130 added, so **it exists
  only after §1 of the bake batch runs** — one more reason step order matters.
- **Terry installs the FULL build** (`dev_build_install.ps1` → `PatchScenesThenAPK`), not the
  three-scene recovery Golden APK. Correct for tonight: the Golden profile locks to
  `_Boot`/`W000`/`ToxicCity` and has no space leg.

---

#### 6 · NEXT OPERATOR

1. Verify the run-level conclusions on the head sha before trusting §0 of anything.
2. **Highest value: wire `WorldPackValidator` into the audit** (§1's open half, MISS_LEDGER #21) —
   it turns one fixed world into a rule that covers all of them.
3. Be a fix-responder for Terry's device reports; every defect gets a ledger class.
4. Unchanged from rb130: circuit breaker at 3 CI reds; report what was BUILT, not planned.

**Commits:** `d859d69b` bootstraps · `6d7012de` hangar walk · `8355ca07` relay machine + gate ·
this entry + MISS_LEDGER #21 + the rb130 §3 correction. CI on the head sha was still running at
write time — **do not infer green from this entry; read the runs.**


> **📕 Older entries live in `docs/HANDOFF_ARCHIVE_2026-07.md`** (archived 2026-07-28).
> Read them only when you need older context — reading the whole history every session is
> the read-in tax `docs/FAST_LANE.md` exists to stop. Oldest of all:
> `docs/HANDOFF_HISTORY_THROUGH_RB24.md`.


### 2026-07-29 (rb130) — 🔴 OPERATOR TAKEOVER PACKET (session ended near usage limit; Terry is driving home to test)

**Read this entry first. It supersedes rb129 below as the current picture.** Terry is on his way home
to put the headset on. Your job before he arrives: confirm the state below is real, do NOT start a new
feature, and be ready to turn his one-line defect reports into fixes.

---

#### 0 · STATE OF THE WORKFLOW (verify this first — CLAUDE.md §WORKFLOW INTEGRITY)

Head at handoff: **`30f3ed84`** (plus `[skip ci]` doc/verdict commits on top).

| Workflow | On `30f3ed84` |
|---|---|
| **CI** (EditMode + patch-scenes + audit) | ✅ **success** — RUN-level, all jobs |
| **Fast Preflight** (python gates) | ✅ success |
| **Recovery Golden Android** (APK) | ✅ success |
| **Recovery Contract Scan / Owner Proof** | ✅ success |
| **Recovery PlayMode Observation** | ❌ **failure, 41/43 — PRE-EXISTING, see §4** |

**⚖ Verification rule adopted today (MISS_LEDGER #20):** a CI-green claim must cite the **RUN's**
conclusion, never one job's. I claimed green from the EditMode job while the audit job was red for
three commits. Do not repeat it — check every workflow on the sha.

---

#### 1 · WHAT WAS BUILT TODAY (committed, CI-verified, NOT device-verified)

**A. The five Catch keepers became geometry** (`25db6a43`)
- `docs/project_art_plan/measured_specs/the_catch_measured_spec.md` — numbers pulled off Terry's five
  approved Nano Banana keepers, including the generator's departures that are BETTER than my prompt
  (ring came back double-ringed; pod has two drive bands; tender is a soft rounded box). Stencils
  (`CP-0974`, `SERVICER-9`, `SECTION A-12`, `P/N 99`…) adopted as canon nomenclature.
- `ScenePatcherSpaceLane.BuildDebrisField` — **28 grey rocks replaced by 9 typed pieces**, each a
  broken part of something else in the pack, so the Overrun reads as consequence not nature.
  Lane kept clear (`|x| >= 14`), every piece gets `DriftTumbleRuntime`.
- `BuildCargoPod(root, burst)` — twin drive bands per the keeper; burst splits at the waist.
- `BuildDroneTarget` → **SERVICER-9**: Hull + chamfers + side pods + nozzles + Eye/Bezel/Indicators +
  **Arm_L/Arm_R** + **AccessPanel** (hatch, green board, copper loom).
- `SpaceTargetRuntime.PoseForMood` — **was written last pass and never called** (dead code wearing a
  feature's name). Now called from `Awake`, `SetMood` and `TakeHit`. Arms stow dormant → deploy woken
  → drop slack with the panel hanging open when disabled (that open panel IS the salvage read).
- **⚖ THE LIGHTING LAW (Terry):** `EnsureLighting()` now DERIVES the lane's key light from
  `SkyVistaLibrary.MossSunBearing` (0.18, 0.62, −0.76) instead of a hand-typed `Euler(35,−30,0)` that
  pointed nowhere near the sun in the sky. Sun over the shoulder; everything ahead is lit.
- **Rings squared to the trajectory** via `FlightBoundsCore.PathAxis` (patcher and runtime call the
  SAME function, so aim and measurement cannot drift), and `RingPassRadius` is now the 6 m bore —
  it was 7 m against a 6 m opening, so a run could be credited on a line that clips the truss.

**B. THE BOUNDS LADDER — Terry's "what happens if I fly too close?"** (`25db6a43`)
New pure cores: `Content/Runtime/Flight/FlightBoundsCore.cs` + `FlightBoundsVoiceCore.cs`.
Wired in `ShipFlightRuntime` (`BuildBounds` / `TickBounds` / `SampleBounds`).
- Five bound classes — **Corridor · Structure · Gate · Ground · Deep** — on a three-tier ladder:
  **ADVISORY (words only) → CORRECTING (push + speed bleed) → HARD (double authority)**.
- Before this the ONLY answer was `FlightModel`'s silent 1.8 km sphere snap. No warning, no gradation,
  RILL said nothing.
- **Three laws, each pinned by a test:** nothing is taken from the pilot without a warning tier first ·
  **straying off the swept lane is ADVISORY forever** (exploring is not a mistake) · **no correction
  ever rotates the ship** (comfort).
- **Variety is mechanical, not hopeful:** 40 authored RILL lines in pools of 4; the picker WALKS each
  pool by a coprime stride so every line is spent before any repeats; she only speaks on a RISE, so
  holding station near a wreck is silent. Cooldown 6 s, re-arm 4 s.
- **Ground is authored but OFF in orbit** (`boundsHasGround=false`) — there is no floor out there.
  It binds the first time an atmospheric leg turns it on. Say that plainly if Terry asks about
  "too close to a building": in Level 1 he never flies near buildings.
- Diagnostics: `ZIPTIDE: FLIGHT_BOUNDS_READY hulls=… corridor=… ground=…` on entering flight, and
  `ZIPTIDE: FLIGHT_BOUNDS kind=… level=… sev=… line=…` per spoken line.

**C. THE CITY'S COMPASS** (`30f3ed84`) — closes `LEVEL1_SPATIAL_SCRIPT` §3 row 5.
New: `Content/Runtime/City/WayfindingCore.cs` (pure) · `Editor/Patching/CityWayfindingAuthor.cs` ·
`Gameplay/Runtime/World/FaultStrobeRuntime.cs`. Called from `ScenePatcherToxicCity.Populate`.
- **Lantern route** over the contract's walk (Dispatch → Market → Plaza → Colonnade → CanalRow →
  Dispatch, ~147 m, ~18 lanterns at 12 m) **and nowhere else** — unlit street = you are exploring.
- **Sightline triple** from dispatch: relay mast's RED fault strobe NW (offset −6.5,0,−5.5 off the
  CanalRow anchor so it does not grow through the RelayVault's roof) · north tower's WHITE crown ·
  berth FLOODLIGHTS south. Measured separation **72°**.
- **Both are LAWS:** `CityWayfindingTests` reads the compiled spec and fails CI if a lantern leg has
  no authored street under it, or if the three landmarks fall within 45° of each other.
- Diagnostic: `ZIPTIDE: WAYFINDING lanterns=… legs=… tripleSeparation=…`.

**D. Two CI reds fixed that had been hiding**
- `9a9e281c` — **`RESOURCE_ID_UNREGISTERED: 'scrap'`**. Disabled ring-tenders and the interior salvage
  cache both pay `scrap`; nothing had ever registered it. Added via `EconomyAuthor` (create-only) AND
  committed as `Resources/Economy/scrap.asset` so a local project has it without a build.
- `d87836c5` — two stale first-hour evidence tokens (`FH_INTERACT_HELM`, `FH_FIRST_ZIPTIDE`) that named
  `ShipCastOffRuntime` lines my earlier cast-off work had rewritten. Refreshed through
  `tools/first_hour_evidence_refresh.py` with a queued reviewable request, never by hand.

**E. Guardrails added so none of this can rot into dead code**
`tools/level1_wiring_gate.py` grew from 16 → **20 features**: `bounds ladder`, `tender tool arms`,
`city compass`, `relay fault strobe`. Each must be EXISTS + CREATED + DRIVEN or CI reds.

---

#### 2 · WHAT TERRY MUST DO BEFORE HE CAN PLAY ANY OF IT

**None of today's geometry is in a scene yet.** It is all generators. He must run the bake batch:
`docs/production/LEVEL1_BAKE_AND_SMOKE.md` §1 — steps 1–6, **in order**, and step 2 (Compile World
Specs) MUST run before step 3 (Build Toxic City) or the bake uses the old prototype layout.
Then §2 build+install, then §3 the play route.

**Code-green ≠ device-green.** Nothing below has been seen in a headset. Expect defects; that is the
point of the session.

---

#### 3 · WHAT IS **NOT** BUILT (honest list — do not let this drift)

- ~~**The zipline**~~ — **CORRECTED rb131: it IS authored and wired.** `ScenePatcherToxicCity.EnsureFirstHourRoute` strings it Plaza→CanalRow and `FirstHourDirector` subscribes to `RideEnded`, so beat 19 fires. The flat-line worry is answered in the core: `ZiplineRide`'s 2 m/s kickSpeed exists so flat lines still move.
- **Berths 1–5 quay pads** (the hangar walk). The beacon thread IS built; the pads are not.
- **W002's defend wave · garden plot · glyph plate** — `WorldStubGenerator` bakes the scene and
  `w002_pumps` is a full 9-step contract, but these three staging beats are absent.
- **Pause/settings board** and the **title + legal/credits panel**.
- **All music and VO.** Zero.
- **All final art.** Everything is procedural stand-in geometry per the stand-in law; Tripo re-skins
  behind the same ids later.
- **`TextMesh` labels on the helm** — likely to read badly at Quest resolution; flag it if Terry
  squints at the console.

---

#### 4 · THE ONE OPEN RED, FULLY DIAGNOSED (I chose NOT to fix it — read why)

`Recovery PlayMode Observation` → **41/43**, failing since at least `59a91c2f`, well before today's
batch. Named failure: `RecoveryGateBypassTests.RuntimeBootstrapDiscovery_CoversAllFirstPartySource
Attributes:78`.

**Cause, reproduced offline (scan `[RuntimeInitializeOnLoadMethod]` vs the catalog):** four runtime
files auto-bootstrap but are absent from the closed exposure catalog
(`Core/Runtime/Recovery/RecoveryAutomaticOwnerCatalog.cs` + `docs/recovery/automatic_runtime_owners.json`):

1. `Gameplay/Runtime/Story/ArtifactJoinRuntime.cs`
2. `Gameplay/Runtime/World/ReentryArrivalRuntime.cs`
3. `Gameplay/Runtime/Tutorial/FirstHourDirector.cs`
4. `Gameplay/Runtime/Tutorial/FirstHourW001Orchestrator.cs`

**Two of those four came out of this Level 1 stream, so part of the debt is ours.**

**The fix shape:** one `Required(...)` registration each (use `Required`, not `Gated` — the test only
demands an exact `RecoveryRuntimeGate.Allows(RecoveryFeatureId.X)` token in source for `FeatureGated`
entries, so `Required` avoids editing four runtime files), a `RecoveryFeatureId` enum value each, and a
mirrored owner record in the JSON (schema: id/source/symbol/startup/persistence/sideEffects/
recoveryExposure/canonicalDisposition/priority).

**Why I stopped:** the recovery program is a separate authority under a documented freeze, PlayMode is
not run by the main CI (so verification costs a full separate workflow cycle), and doing four contract
registrations blind at the end of a session is exactly how the three reds earlier today happened.
**Terry's call whether to open that lane.** It does not block the headset test — Golden Android is green.

---

#### 5 · NEXT OPERATOR: DO THIS, IN THIS ORDER

1. **Verify, don't trust.** `git log --oneline -5`; confirm every workflow on the head sha (not one
   job). Run `python3 tools/level1_wiring_gate.py --root .` and
   `python3 -m unittest discover tools/tests -q` — both should be clean in seconds.
2. **Do not start a new feature.** Terry is minutes from a headset. Be a fix-responder.
3. **When defects come in**, they will arrive as one-liners with a `ZIPTIDE:` tag. The tag map for the
   new work: `FLIGHT_BOUNDS*` (bounds ladder) · `WAYFINDING` (compass) · `DRONE_MOOD` (tender arms) ·
   `FLATS_SITE` (expedition) · `CATCH_*` / `ARTIFACT_JOIN_HINT` (RILL cues).
4. **Every defect → fix AND a MISS_LEDGER class.** That is the standing contract.
5. **Circuit breaker: 3 CI reds on one task → STOP and escalate to Terry.** I hit exactly 3 today on
   the guardrail-conformance task; the discipline is what kept it from becoming a lost evening.

**⚖ Terry's standing corrections (canon, do not re-litigate):** artifact half B is at the crashed
survey skiff OUTSIDE town, reached BY VEHICLE through the sea-wall breach — not the Dockmaster's
paperweight · the canal stalker interacts with the player's BOAT · the rings are THE CATCH's arrestor
infrastructure (see `docs/design/THE_CATCH.md`), not a lamp chase · art/sound are modular procedural
stand-ins swappable via Tripo later · **report honestly what was BUILT, not planned.**

---

### 2026-07-29 (rb129) — the five Catch keepers built, the bounds ladder, and a CI-green retraction

- **⚠ WORKFLOW INTEGRITY — I got this wrong and it is logged.** I reported the space-leg batch as
  "CI GREEN on `94e363a4`". The **EditMode job** was green; the **"Patch scenes + world audit"
  job was RED on that commit and stayed red** through `c42b1311`, on one blocker:
  `RESOURCE_ID_UNREGISTERED: 'scrap'`. Fast Preflight was red too, on two stale first-hour
  evidence tokens. Both are fixed here. **MISS_LEDGER #20** records the class: part-for-whole
  verification — one job's conclusion reported as the run's. A CI-green claim must now cite the
  RUN's conclusion, never a job's.

- **BUILT (committed, real) — the keepers become geometry:**
  1. **`measured_specs/the_catch_measured_spec.md`** — numbers pulled off the five approved
     keepers, including the departures the generator made that are better than my prompt (the
     ring came back DOUBLE-ringed; the pod has TWO drive bands; the tender is a soft rounded box,
     bureaucratic not hostile). Stencil strings (`CP-0974`, `SERVICER-9`, `SECTION A-12` …)
     adopted as canon nomenclature.
  2. **The debris field is no longer 28 grey rocks** — 9 typed pieces, each a broken part of
     something else in this pack, so the Overrun reads as consequence rather than as nature.
  3. **SERVICER-9** — the tender rebuilt with eye, side pods, access panel and **two tool arms
     that now actually move**: stowed dormant, deployed woken, slack with the panel hanging open
     when disabled. `PoseForMood` was written last pass and never called; it is wired now and a
     new wiring-gate row keeps it that way.
  4. **THE LIGHTING LAW (⚖ Terry):** the lane's key light is DERIVED from
     `SkyVistaLibrary.MossSunBearing` instead of a hand-authored Euler pointing nowhere near the
     sun in the sky. Sun over the shoulder; everything ahead lit.
  5. **Rings are squared to the trajectory** (`FlightBoundsCore.PathAxis`), and the ring PASS test
     is now the bore itself (was 7 m against a 6 m opening — a run could be credited on a line
     that also clips the truss).

- **BUILT — THE BOUNDS LADDER (⚖ Terry: "what happens if I get too close…"):**
  `FlightBoundsCore` + `FlightBoundsVoiceCore`, both pure. Five bound classes (corridor · hull ·
  gate · ground · deep) on a three-tier ladder — **told, then nudged, then held**. Three laws are
  pinned by tests: nothing is taken from the pilot without a warning tier first; **straying off
  the swept lane is ADVISORY forever** because exploring is not a mistake; and no correction ever
  rotates the ship. Variety is mechanical: **40 authored RILL lines in pools of four**, and the
  picker WALKS each pool by a coprime stride, so every line is spent before any repeats; she also
  only speaks on a RISE, so holding station near a wreck is silent. Ground is authored but OFF in
  orbit (there is no floor); it binds the first time an atmospheric leg turns it on.
  Before this the only answer the game had was FlightModel's silent 1.8 km sphere snap.

- **NOT BUILT (unchanged, still honest):** city lantern route + sightline triple · the zipline ·
  berths 1–5 quay pads · W002's defend wave / garden plot / glyph plate · pause board · title and
  credits panel · all music and VO. Everything visual remains a procedural stand-in.

- **Next:** confirm the run (not the job) is green, then the city wayfinding pass.

- **✅ THE CITY'S COMPASS (built after the above, `30f3ed84`, CI + Fast Preflight green):**
  `WayfindingCore` + `CityWayfindingAuthor` + `FaultStrobeRuntime`. Lantern route over the
  contract's walk and nowhere else (unlit street = you are exploring), and the sightline triple
  — relay mast's red fault strobe NW · north tower's white crown · berth floodlights S.
  **Both are LAWS:** `CityWayfindingTests` reads the compiled spec and fails CI if a lantern leg
  has no authored street under it, or if the three landmarks fall within 45° of each other.
  Measured today: **72°**. That closes LEVEL1_SPATIAL_SCRIPT §3 row 5.

- **🔎 HEADS-UP — a pre-existing red I did NOT fix, precisely diagnosed:**
  `Recovery PlayMode Observation` has been failing since at least `59a91c2f`, **41/43**, and the
  named failure is `RecoveryGateBypassTests.RuntimeBootstrapDiscovery_CoversAllFirstPartySource
  Attributes:78`. Cause, reproduced offline: **four runtime files carry
  `[RuntimeInitializeOnLoadMethod]` but are absent from the closed exposure catalog**
  (`RecoveryAutomaticOwnerCatalog` / `docs/recovery/automatic_runtime_owners.json`) —
  `Story/ArtifactJoinRuntime.cs`, `World/ReentryArrivalRuntime.cs`,
  `Tutorial/FirstHourDirector.cs`, `Tutorial/FirstHourW001Orchestrator.cs`. **Two of those four
  came out of this Level 1 stream, so this is partly my debt.** The fix is a registration each
  (`Required` classification avoids needing a `RecoveryRuntimeGate.Allows` call in the source),
  a `RecoveryFeatureId`, and a mirrored owner record in the JSON. **Not done here on purpose:**
  the recovery program is a separate authority under a documented freeze, PlayMode is not run by
  the main CI, and doing four contract registrations blind at the end of a session is how the
  three reds earlier today happened. Terry's call on whether to open that lane.

### 2026-07-29 (rb127) — Fable 5: 🏗 B1 — ToxicCity world spec (the city interior as spec data) + relay_cell item + bake-order fix

- **Terry approved the completion plan with the honest-reporting mandate** ("just let me know
  honestly what you've built and what hasn't been built, not planned, built"). Executing the batch
  checklist B1→B12. This entry is B1.
- **BUILT (committed, real):**
  1. **`docs/worldspecs/ToxicCity.spec.json`** — the FIRST world spec (folder was empty). A
     faithful superset of the committed `ToxicCityLayout.asset` + `ToxicCity_WorldPack.asset`
     (all 5 districts/7 connections/2 canals/3 droneZones/4 creatureZones/2 hazards/shipyard/
     palette/sky verbatim; pack collectibles+machines+flags mirrored EXACTLY so
     `WorldSpecCompiler.ApplyToPack`'s wholesale overwrite cannot wipe anything) **plus the B1
     expansion:** **Quay** district east of the berth (Dockmaster booth hero w/ new
     `dockmaster_booth` interior marker, quay crane, crate props — the §3 toy-beat dockfront),
     **Colonnade** district on the Plaza→CanalRow leg (the Husk-Molter observation walk;
     `Molters_Relay` zone re-sited there as `Molters_Colonnade` per WS2), **Canal One** east–west
     between quay and city + its Flood hazard, and 3 new connections (Quay bridge, colonnade walk).
  2. **`Resources/Items/RelayCell.asset`** — `relay_cell` ItemDefinition (was a LATENT GAP: the
     pack's `signal_relay` machine referenced it but no definition existed; the spec validator
     would have red-lit `SPEC_ITEM_UNKNOWN` at Terry's desk).
  3. **`LEVEL1_BAKE_AND_SMOKE.md` bake order fixed:** Compile World Specs is now step 2, BEFORE
     Build Toxic City (else the bake uses the old prototype layout), with the one-time
     `SPEC_DRIFT` formatting warn explained.
- **NOT BUILT yet (honesty per the mandate):** the spec is DATA — it becomes geometry only when
  Terry runs Compile + Build Toxic City (nothing bakes in the cloud). Skiff dock/zipline-over-
  Canal-One placement, garage/breach/flats site (B5), stalker (B7 — no `tox_canal_stalker_01`
  CreatureDefinition exists yet, so NO stalker zone in this spec: it would silently fall back to
  a Swarmer), space-leg presentation batches (B3/B4x), keyed departure (B9), W002 close (B10).
- **Heads-up:** decisions of record — spec keeps `experience.enabled=0` + `pois:[]` (POIs are
  gated on experience; expedition site is staked by B5's author instead), half B stays "the
  Dockmaster's paperweight" collectible until B5 resites it to the flats per Terry's ⚖ (rb126),
  `rings`/jobs/spawnMarkers/audioProfile are not spec-carried (compiler never touches them —
  verified in `WorldSpecCompiler.cs`). Next: B2 (contract marker retarget check) then Wave 2
  space batches.
- **B2 (same day, second commit): `tools/worldspec_contract_gate.py` + 12 tests + CI hook** —
  reconciles every spec against its world's ATTACHED contract assets (pack→jobs→steps by guid):
  GoToMarker ids must exist as spec hero/poi markers, RepairMachine ids in spec machines,
  DisableDrones counts within spec droneZone capacity, spec item ids resolvable in
  `Resources/Items`. Verified against the real repo: 5 attached W001 steps (`dispatch_inside`,
  ×5 drones, `relay_node`, `signal_relay`, `shipyard_office`) all reconcile with the B1 spec —
  **no contract retarget was needed**; the gate now makes that divergence a CI red forever (the
  rb120 JOB_MARKER_MISSING class). All 233 tools tests green. Note: committed job assets are the
  older 5-step bake whose return step file is named `ToxicCity_S4_Return`; the builder's next run
  authors `S5_Return` — cosmetic, both carry `shipyard_office`.
- **B3 (Wave 2 opens — the space leg): ring lamp-chase, CODE-built.** `RingLampChaseCore` (pure:
  next ring = amber window sweeping its segments at 0.8 Hz, passed = dim green, future = dim
  amber — a traveling lamp, never a strobe) + `RingCourseLightsRuntime` (MaterialPropertyBlock
  painting, full repaint only on ring change, `ZIPTIDE: RING_LIGHTS next=` on transitions) +
  `ShipFlightRuntime.NextRingIndex/RingCountTotal` accessors + `EnsureRingLights` in
  `ScenePatcherSpaceLane.Populate` + 7 EditMode tests. NOT yet real until Terry's Build Space
  Lane bake re-runs (the committed scene predates the component).
- **B4: the drones now REACT + THE FIND gets its debris field.** `SpaceTargetReactionCore` (pure:
  wake at 40 m / sleep at 55 m — a hysteresis band so a pilot hovering at the boundary never
  sees the eye strobe; a 3 s evade window after each hit that outranks waking; ±6 m slide at
  0.35 Hz; eye emissive + bob ×2 escalate with mood) wired into `SpaceTargetRuntime`
  (`ObservePilot` fed from `ShipFlightRuntime.TickCombat`, `ZIPTIDE: DRONE_MOOD` on transitions,
  non-lethal law intact — reactions change motion and light only, nothing chases or shoots),
  plus `DriftTumbleRuntime` and a 9-piece golden-angle scrap shell around the artifact cradle so
  "one object does not scan" has ordinary objects to hide among. 12 more EditMode tests.
- **B4b: the ascent + reentry are REAL presentations now, not a logged seam.**
  `AtmosphereVeilCore` (pure schedule: ascent builds 1.2 s → peak 0.5 s with the travel cut at
  1.5 s *inside* the peak; reentry holds 0.6 s then clears 1.9 s = exactly the 2.5 s
  `ReentryArrivalCore.PresentationSeconds` budget; `HardCapSeconds` 4 s forces intensity to zero
  no matter what) + `AtmosphereVeilEffect` (gate-effect house style: 16 opaque unlit corona
  blades parented to the CAMERA so they ride the rig across the scene load, licking inward with
  per-blade flicker, ember tracers streaming back past the canopy, procedural 2.4 s roar, camera
  never moves, self-destructs at the cap, `ZIPTIDE: VEIL leg=/phase=`). Hooked BOTH ends:
  `ShipCastOffRuntime.LaunchSequence` plays Ascent and waits the bounded lead before
  `TravelCoordinator.TravelTo`; `ReentryArrivalRuntime` plays Reentry on the space→world route
  it already owns. 9 EditMode tests, most of them the safety contract (bounded intensity, lead
  inside the peak and shorter than the veil, always clear past the cap). This retires the
  contract's "reentry visual is queued art" row — the veil is a procedural v1 per the stand-in
  law, tunable/replaceable, but BUILT.
- **✅ CI GREEN `fa1bc5ec`** — that run compiled B1–B4b together (spec + gate + lamp-chase + drone
  reactions + veils) and passed EditMode. B1/B2/B3/B4's own runs show `cancelled`: that is the
  concurrency group superseding in-flight runs on each new push, NOT a red.
- **B4c: SPACE IS A PLACE.** (1) `SkyVistaLibrary` gains `SpaceLane_Trial` → **the Moss-orbit
  vista**: the ringed giant on W001's *exact ground bearing* (canon §3 "directions are identical")
  grown only 12°→14° because "space adds REVELATION, not scale", the sibling grey moon, the one
  warm sun, starfield density 0.9 with `horizonFade=0` (no air, no horizon), the faint galactic
  band, sun-as-key-light + dark ambient — replacing the near-black placeholder. `ScenePatcherSpaceLane`
  now calls `EnsureAllAuthored` + `AssignAll` itself, so Terry's ONE menu item produces the sky.
  2 canon tests pin the ground↔space relationship. **⚖ FOR TERRY:** canon §4 also calls the giant
  "~⅓ of sky", ~3× the shipped W001 ground value; I matched the SHIPPED GROUND ASSET (continuity
  between two skies seen minutes apart) rather than the doc — retuning both together is a two-line
  change, your call. (2) **The W000 porthole** — `PortholeStarfieldCore` (pure, deterministic
  starfield + galactic band bake) + `PortholeRuntime` (framed pane, generated 128² texture, the
  giant hanging in the corner so the window agrees with the sky outside), authored into W000 by
  `FirstHourSurfaceAuthor` at standing eye height past the bunk. This closes the "no space in a
  space game" hole: the first ten minutes happened inside a spaceship you could not see out of.
  (3) **The helm compass ribbon** — `CompassRibbonCore` (pure yaw bearing → clamped ribbon offset,
  distinct BEHIND state, color ramp) + `HelmCompassRuntime` (physical strip on the canopy bar, an
  instrument not a HUD, dark when there is nothing to steer to) + `ShipFlightRuntime.TryGetCourseBearing`.
  11 more EditMode tests.
- **B4d closes Wave 2: the salvage loop is now VISIBLE.** It always worked and always paid — but
  silently, so on device the only evidence salvage existed was a log line.
  `SpaceCombatCore.SalvageApproach01` (pure, 2.5× range band) drives a downed drone glowing toward
  salvage-teal as you close, and the payout draws a `SalvageTractorFx` beam from wreck to cockpit
  with a rising pluck (deliberately unlike any damage cue). 2 more tests. **WAVE 2 COMPLETE:**
  lamp-chase, drone reactions, ascent/reentry veils, Moss-orbit sky, porthole, compass, salvage
  confirm — the space leg is built, not deferred.
- **B5 (Wave 3): THE EXPEDITION IS BUILT — half B moved out of town.** `FlatsSiteAuthor` stages
  the crashed survey skiff (tilted hull, cracked cargo cage you can see the pickup inside), the
  burn-off smoke column (7 stacked puffs, the "follow the wall then the smoke" wayfinding), the
  salvage scatter, and rubble ramps at the breach mouth. **The breach bearing is COMPUTED with
  RingCityBuilder's own formula, not guessed** — a ramp against solid wall would have silently
  killed the leg; 3 tests pin it (real gap · not the harbour mouth · faces the site). The spec
  moves `artifact_half_b` from "the Dockmaster's paperweight" at (-3.2, 0.1, -30) to the wreck at
  (151.6, 1.4, 87.5) and adds a `waker_log_flats` slate + its ItemDefinition. The contract gains
  step **S5 "Take the crawler out past the breach"** (now 6 steps) and `cavern_crawler` re-parks
  at the **Quay** beside the Dockmaster — work order and keys in the same breath. The **resonance
  tell** fires on grabbing either half: `ResonanceTellCore` (pure — 0.8 s blackout, smooth
  recovery, provably always hands power back) + `ResonanceTellRuntime`, reaching the vehicle
  through a new `Ziptide.Core.IResonanceSensitive` so Gameplay never has to reference Ship.
  `VehicleRuntime` implements it: LOOKS only, the ride still drives. 8 EditMode + 2 gate tests.
  The gate learned about editor-authored markers (the `…MarkerId = "…"` convention) — the flats
  site is outside the wall, so it is neither a district hero nor a POI, but a step pointing at a
  marker *nobody* plants is still a red.
- **⚠ HONEST SCALE CORRECTION:** the spatial script called the drive "~700 m". The city shell is
  only 190 m in radius, so the real route is **quay → breach ≈75 m → around the wall to the site
  ≈195 m, ~270 m total**. Still a real drive with the city shrinking behind you; not 700 m. The
  script's number was written before the site was placed against the actual ring geometry.
- **B6+B7 (Wave 4): THE CANALS ARE A WATERWAY AND SOMETHING LIVES IN THEM.** The insight that made
  this cheap: the **ring canal already exists as real generated geometry** (`canalRingRadius` 74,
  width 9 → a ~465 m continuous circuit), so the "navigable route" was built all along and unused.
  `CanalWaterCore` (pure: ring annulus + authored rects, 1.2 m shore tolerance, a unit-length
  correction *direction* — the lock NUDGES like grounding out, never teleports or freezes, both of
  which would be comfort events) + `SkiffWaterLockRuntime`, and **`tide_skiff` now spawns ON the
  ring canal** at the CanalRow bearing instead of on a shipyard slab, configured with the layout's
  own canal data at bake time. `CanalStalkerCore` + `CanalStalkerBehavior` deliver Terry's three
  stages — **ride 1 is ALWAYS just the shadow** (escort, never ambush), ride 2+ or long lingering
  earns the bump (10° yaw, assist recovers), ride 3 + lingering earns a block that yields after
  6 s; stun always drops it back to shadowing; **no boat = the animal isn't there at all**. Added
  the missing `tox_canal_stalker_01` CreatureDefinition (without it the zone would have silently
  fallen back to a Swarmer — the exact trap I flagged at B1) + the CityBuilder id case + the spec
  zone on the ring canal. 13 EditMode tests.
- **B8+B9 (Wave 5): THE FIRST ZIPTIDE CAN NOW ACTUALLY HAPPEN.** ⚠ **Found while building: the
  minute-45 beat had nowhere to occur.** `KeySocketRuntime` + `ShipCastOffRuntime` were only
  authored onto W000's ship (`if (kit.sceneName == "W000_DriftIn")`), but per Terry's ⚖ the join
  happens back at the **Toxic City** berth — so the hull that is supposed to erupt had no socket,
  no launch, and no way to reach W002. Fixed: `CityBuilder` now authors the socket + cast-off on
  the ToxicCity berth too, gated on the KEY rather than a coupler.
  `CastOffArming.IsArmed` gains the key overload with a deliberately **opposite** absence rule —
  a missing coupler must never strand the launch, a missing key always must (5 tests pin both
  directions, since that is the easiest thing here to get backwards). PUNCH IT with no key reads
  "NO DESTINATION / seat the key", not "repair" — it is a route it doesn't have, not a fault.
  The keyed departure now passes the **berth position** to `TravelCoordinator.TravelTo(scene,
  gatePos)`, so the tide pours out of your own hull instead of ringing you wherever you stand —
  and it deliberately plays **no atmosphere veil**, because the gate IS the event. `BeaconThreadRuntime`
  (the rb121 deferred LOOK problem) hangs a sagging tide-lit arc between the joined key and the
  socket, brightening as you carry it closer, alive only in the ARTIFACT_JOINED → KEY_SEATED
  window — so "follow the beacon back to your berth" is something you can see.
- **🔴→ CI red on `a260b835`, diagnosed and fixed (1326/1332 — it COMPILED; all 6 were guardrail
  tests doing their job, and every one was right):**
  1. `OnlyTheKeySocketReArmsTheTide` — I authored the ToxicCity berth `suppressGate: false`, which
     would have let the gate exist without the key. **The invariant is better than my code was:**
     both berths are now authored gate-SUPPRESSED and `KeySocketRuntime` remains the only thing in
     the game that hands the gate back.
  2. `TheFirstLaunchIsACastOff_NotAZiptide` + `HelmAndAuthorSourcesDoNotCreateASecondTravelOrLaunchPath`
     — my two-branch launch created a SECOND `TravelCoordinator.TravelTo(` call in the launch owner
     (exactly how a parallel travel path is born). Collapsed to one exit via a new
     `TravelTo(scene, skipGate, gatePos)` overload; both paths now leave through one call.
  3. Three creature-readability tests — `tox_canal_stalker_01` shipped without a
     `CreatureBehaviorReadabilityCatalog` profile. Added, with its three stages as the readable
     states (shadow escort / hull bump / channel block) and the stun as the counter.
  Also folded the W000 and ToxicCity berth authoring into ONE parameterized block rather than two
  near-duplicate ones.
- Commit: (this one).

### 2026-07-29 (rb126) — Fable 5: 📐 LEVEL1_SPATIAL_SCRIPT — the placement layer, + ⚖ Terry's two story corrections (expedition + boat/stalker)

- **Terry demanded the missing DETAIL layer** ("placement of everything, how far apart, the little
  stuff in between, how do we know where we're going, are the rings there immediately, how do the
  droids act") and guessed it wasn't built. ~~**Confirmed at the data layer:** the committed
  `ToxicCityLayout.asset` has EMPTY `districts:/canals:/creatureZones:/droneZones:/pois:/shipyard:/
  experience:` blocks — the city is a 250 m shell (26 towers, fog, sky) with NO level inside it.~~
  **⚠ CORRECTED rb127 / MISS_LEDGER #19:** that was a grep artifact — the asset carries 5 districts,
  7 connections, 2 canals, 3 drone zones, 4 creature zones + berth (a prototype-scale interior);
  only `pois:[]` was empty. The spec (rb127) supersedes it either way.
  Wrote **`docs/design/LEVEL1_SPATIAL_SCRIPT.md`**: every placement marked **[M] measured** (space
  lane ring/drone coordinates, 40 m/s flight, drone bob/recharge/list behavior, city shell) /
  **[∅] empty** / **[S] spec'd** (W000 eyeline chain, the 400 m job loop, dispatch sightline
  triple, lantern wayfinding, ring lamp-chase, drone reaction layer…). §5 = the [S]→[M] build order.
- **⚖ TERRY STORY CORRECTIONS (2026-07-29, exercised via DC §8 modifiability — now canon in the
  script §3b/§3c):**
  1. **HALF B IS OUTSIDE TOWN, REACHED BY VEHICLE** — not the Dockmaster's paperweight. Dockmaster
     gives the LEAD; you take the vehicle (code exists: `ToxicCityVehicleBuilder`/`VehicleRuntime`)
     out the sea-wall breach, ~700 m across the tidal flats to a crashed survey skiff; half B in
     its cargo cage + the first outside waker-log. Join happens back at the berth after the drive.
  2. **THE CANAL STALKER INTERACTS WITH YOUR BOAT** — the skiff is a pilotable water vehicle on
     ~300 m of navigable canal; the stalker escalates shadow→bump→block (non-lethal, comfort-
     capped, never leaves the water; stun OR drift resolves it).
- **The chasm is real and named:** vehicle/boat legs, city interior data, W002 staging, wayfinding,
  reactions — all [∅] with [S] specs now. **Next: §5 item 1 — land the city interior as DATA via
  the CityBuilder/author path (never hand-edited YAML), then items 2–4.** 📣 story lane: DC v2.1
  minute-map + half-B sourcing superseded per the ⚖ above — reconcile the DC doc.
- **Commit:** spatial script + additions + this entry.

### 2026-07-29 (rb125) — Fable 5: 🔨 LEVEL 1 build pass 1 — the reentry handoff has an OWNER (first 🔴 closed at code layer)

- **Terry: "finish the level."** Working the rb124 accurate 🔴 list top-down as compiling increments;
  CI is the compile-verifier (cloud has no Unity), circuit breaker 3 reds → stop.
- **BUILT (commit `cc21b03b`):** the **reentry/landing handoff** — product contract §4's red row
  with "no canonical owner/beat" — now has one:
  - `ReentryArrivalCore` (Core, pure): plays ONLY on space-leg→world; cold boot / gate travel /
    dev warp / in-leg all fail closed. **5 EditMode tests** pin the routing table (the "gate
    spectacle on the wrong beat" class from rb121 cannot return).
  - `ReentryArrivalRuntime` + `SceneArrivalLog` (Gameplay): origin via `sceneUnloaded` (no travel-
    owner edits; TravelCoordinator untouched), `ZIPTIDE: REENTRY_ARRIVAL from=… world=…`, optional
    guarded RILL line (default empty — the existing `react_w001_arrived` producer keeps the beat;
    `FH_W001_ARRIVAL`'s adapter is NOT double-fired).
  - `ScenePatcherToxicCity.EnsureReentryArrival()` — authored into the scene at bake, idempotent.
  - **v1 is the SEAM: the plasma-veil visual is an art stand-in** (a LOOK problem needing reference
    plates, same rule that deferred the hangar walk) — logged for the Tripo/art pass.
- **State honesty:** row moves 🔴→🟨 only when CI (EditMode + patch-scenes) is green on this SHA;
  tracker updated then, not now. Nothing device-proven.
- **NEXT INCREMENTS (in order, after this batch's verdict):** ① space-leg neutral signals
  (helm/course/disable/salvage/approach — needs coordinated `first_hour_beats.json` + producer +
  gate edits, done as one batch so the beats-lose-producer test stays green) · ② the shell
  (pause/legal/credits — genuinely absent) · ③ W001 district re-layout (tower island + wedges) ·
  ④ changed-ship payoff decal content · ⑤ W002 staging check. 📣 other auditing lanes: claim rows
  in the tracker before building — collisions red the tree.
- **Commit:** `cc21b03b` + this entry.

### 2026-07-29 (rb124) — Fable 5: ⚠ RETRACTION of rb123's "~70% built" — the ACCURATE state (Terry was right to be skeptical)

- **rb123 was WRONG. I conflated "a generator exists" with "content is committed and works."** Terry
  pushed back; a rigorous pass proves the pessimistic tracker/contract rows are right and my
  optimism was not. Correcting the record so no lane inherits the false claim.
- **THE PLAYABLE SCENES ARE NOT COMMITTED — they are bake-only.** `git ls-files`: only
  `Scenes/ToxicCity.unity` exists; **`W000_DriftIn.unity`, `SpaceLane_Trial.unity`,
  `W002_DryCistern.unity` are ABSENT.** They are (re)generated by editor authors + `EnsureSceneEnabled`
  when Terry runs the bakes. WorldPack/asset DATA is committed; the SCENES that host them are not.
  So there is **no loadable level until the bake batch runs**, and ToxicCity itself is empty until
  `CityBuilder` populates it.
- **BOTH authoritative self-assessments say core route pieces are 🔴 (I contradicted them):**
  `FIRST_LEVEL_PRODUCT_CONTRACT.md` §4 honest matrix + `LEVEL1_MASTER_TRACKER` agree — 🔴/⬜ on: the
  **space scene as a real route leg** ("generated scene absent from live repo"), the **first Ziptide
  FROM space** (currently departs W000), the **reentry/landing handoff** ("no canonical owner/beat"),
  **W001 city topology/districts**, **audio production rails** (buses/settings/ducking/captions), the
  **changed-ship payoff**, and **full-route performance/evidence** — plus many 🟡 partials. Nothing is
  device-proven.
- **Stubs found in route runtimes (intentional but incomplete):** `TransmissionConsole` (M1 stub,
  audio at M6), `QuartersRoom` (stub), `BeltMinePortRuntime`.
- **ACCURATE "build the whole level" = the 🔴 list is the real build list** (authorable as CI-green
  code, breadth-first): space route leg → first-Ziptide-from-space binding → reentry handoff (net-new)
  → W001 districts → audio rails → payoff orchestration → finish the 🟡 partials → W002. THEN Terry
  bakes + builds + plays.
- **Method correction:** "built" is claimed only at the committed-content or device layer, never from
  a generator's existence; trust the tracker/matrix ⬜/🔴 (set by operators who can compile) over
  code-presence inference. **Commit:** this retraction.

### 2026-07-29 (rb123) — ⛔ SUPERSEDED BY rb124 — over-claimed "~70% built" (see the retraction above)

- **Terry:** *"I want the whole thing actually built… everything the level is supposed to have… the
  whole thing could end up broken temporarily, we repair as we go."* Traced the actual CODE (not the
  tracker's ⬜ rows) to find what genuinely needs building vs what already exists. **Hard floor: the
  build must still COMPILE** — a level that won't build is one Terry can't test at all; "broken" = feel/
  gameplay, never a red tree.
- **VERIFIED ALREADY BUILT IN CODE (do NOT rebuild — no-parallel-system law):** W000 wake + coupler
  repair (`ShipCastOffRuntime` arming) · cast-off/PUNCH IT → `SceneSpaceLane` · **the space leg is real**
  (`ScenePatcherSpaceLane`: rings + drone SALVAGE targets via `SpaceTargetRuntime` + drift-parallax +
  `EnsureOnwardLeg` to ToxicCity + `EnsureTheFind` = the artifact on a wreck) · ring city (`CityBuilder`)
  · artifact FIND/JOIN/SOCKET (`ArtifactJoinRuntime`/`KeySocketRuntime`) · W001 contract (5 steps, marker
  fix) · **vehicles** (`ToxicCityVehicleBuilder`+`VehicleSafetyRuntime`+`VehiclePlayabilityTests`) ·
  teaching/RILL/Cal lines · `LEVEL_INVARIANTS` gates · `TravelCoordinator` already masks the scene loads.
- **THE GENUINELY-MISSING (the 30% I will build, breadth-first, each a compiling increment):**
  1. **Launch/reentry VEIL feel** (W-3) — the plasma ascent/reentry beat as an ADDITIVE presenter around
     `TravelCoordinator.TravelCompleted` (structure exists via the fade; this is the designed "one
     journey" polish). Stand-in VFX.
  2. **W001 ARRIVAL composition + landmark legibility** (W-5, reqs 36/39) — CityBuilder arrival framing.
  3. **First-contract polish** — story fragment (48), visible world-change (49), meaningful spend (55).
  4. **Extraction/return FICTION** (50-53) — the fictional return path (travel exists; the fiction doesn't).
  5. **Audio RAILS** (⬜) — mixer/buses/sliders/ducking + caption twins (stand-in SFX).
  6. **The SHELL** (⬜) — volume sliders, save-failure UX, version/legal/credits, pause.
  7. **W002 build** (Level 2 staged) — from the same route packet.
  8. Deferred by T-Dog rb121: beacon thread, hangar-walk-to-berth-6, W000 viewport/prior-owner log.
- **Execution:** build the above as compiling increments; keep CI green; **circuit breaker 3 reds →
  stop.** Terry runs ONE bake batch (`LEVEL1_MASTER_TRACKER` §5) + build + plays the WHOLE level, finds
  what's broken; defects → `MISS_LEDGER` so Level 2 can't inherit them. Art/SFX are Forge stand-ins now,
  Tripo swap in ~6 days via the applier.
- **Next:** build gap #1 (the launch/reentry veil presenter, additive). **Commit:** this entry.

### 2026-07-29 (rb122) — Fable 5 takes the LEVEL 1 baton: build it playable, start to finish

- **Terry, verbatim:** *"I want it all built, the whole level start to finish… I want to be able to
  actually play this level… one level, that's all we need… we've spent months planning, let's
  execute."* Discover-by-playing; broken things get **fixed AND logged so the next level doesn't
  repeat them**; art/sound are **modular stand-ins logged as yet-to-be-done** (real Tripo art in
  ~6 days once he's paid). Continuing from T-Dog's rb121 session close; T-Dog's Level 1 lane is
  wrapped, I hold it now (if T-Dog resumes we split by wave).
- **THE SYSTEMS TERRY ASKED FOR ALREADY EXIST — I will NOT build parallels (no-parallel-system law):**
  - *Stand-ins / art-swap:* `TRIPO_INTAKE_EXECUTION_PACKET.md` — everything ships with a **Forge-native
    placeholder now**; the real Tripo GLB **swaps in via `ForgeVisualApplier`** later; "no visual swap
    may erase gameplay tells." That IS "play now, pretty in 6 days." Nothing new needed.
  - *Defect → prevention:* `MISS_LEDGER.md` (every miss → class → a gate/test so it can't recur) +
    `LEVEL_INVARIANTS.md` (the cross-level consistency gates) + `FAST_LANE.md` (velocity). The
    play→find→fix→log→prevent loop is these, used per defect.
  - *State truth:* `LEVEL1_MASTER_TRACKER.md` (rows move only with proof; code-green ≠ device-green).
- **COMPLETION EXECUTION ORDER (to playable start→finish; each = Forge-stand-in content + CI-green
  proof + a Terry build+play checkpoint; defects route to MISS_LEDGER):**
  - **W-A · make the CURRENT slice playable TODAY** — the Terry bake batch (`LEVEL1_MASTER_TRACKER`
    §5: Author W000 Surfaces · Build Toxic City + Contract · Build Space Lane) so W000→helm→cast-off
    →W001→contract→artifact FIND/JOIN→return runs on device now, with a smoke checklist keyed to log
    tags so any break is localized in minutes (the anti-lost-day mechanism).
  - **W-2 · production space route** (W000→space→W001 chain from idempotent authors; PlayMode headless).
  - **W-3 · launch / first-Ziptide / reentry seam** (data-driven sequencer around `TravelCoordinator`,
    timeouts so presentation never strands travel, rig never parents the hull; reads as one journey).
  - **W-4 · flight + salvage playable pass** (soft-lock target, tractor-salvage, non-lethal disable).
  - **W-5 · W001 model-city arrival + route legibility** (arrival composition; landmarks read).
  - **W-6 · first contract + creature encounter + payoff + audio rails** (stand-in SFX; the Husk-Molter).
  - **W-7 · W002 replication** — Level 2 staged from the same route packet (Terry's "ready for level 2").
- **Next (this operator):** W-A — write the exact bake+smoke runbook so Terry can play the existing
  slice today, then start W-2 code (space route) keeping CI green; **circuit breaker holds — 3 CI reds
  on one task → stop and escalate.**
- **Commit:** this entry (ownership + order; docs-only, CI-safe).

### 2026-07-29 (rb121) — THE STORY: the first world was never built as one

- **Terry, verbatim:** *"It doesn't really make sense to punch it because we haven't even found the
  artifact yet at that point… which also tells me this world isn't really built, or at least not
  built correctly according to the story."* He was right, and the repo proves it.
- **THE CANON THE BUILD CONTRADICTED.** `docs/design/FIRST_HOUR_DIRECTORS_CUT.md` **v2.1 "The Key
  That Knew You"** (2026-07-16/17, Terry-directed) puts minute 10–13 at *"helm → PUNCH IT → cast-off
  rails (ship flight, **NO gate FX**)"* — a routine wreck-clearance job — and **the first Ziptide at
  minute 45**, from Cal's own berth, after the key is joined and seated. The build fired the full
  gate spectacle at minute ten for a bus ride and left nothing for the beat designed to earn it.
- **THE PROOF W000 WAS NEVER A STORY WORLD:** `docs/storyboard/` carries a per-world README for
  W001, W002, W003 and W004 and **none for W000**. It is an onboarding box with a ship bolted on.
- **FOUR CONFLICTING FIRST-HOURS EXIST** in the repo: W001's STORY.md (Cal *starts* at Toxic City,
  no W000, no artifact), CHAPTER_0-1 (RILL dark pre-boot, boots in W001), the Director's Cut
  (current authority), and the build. Named in `LEVEL1_MASTER_TRACKER` / the plan so the next
  operator does not re-derive it.
- **SHIPPED:** `PUNCH IT` demoted to an ordinary cast-off at the salvage lane with the gate
  suppressed — and the **AUTHOR** owns that route now, because the committed scene serialized
  `targetScene: ToxicCity` and relying on serialization would have meant regenerating the world did
  not fix it. The lane gained an onward leg to Toxic City (it was a cul-de-sac whose only exit was
  RETURN HOME, which is why the middle of the hour was unreachable). Three artifact
  `ItemDefinition`s; THE FIND on a wreck past the last ring; half B as the Dockmaster's paperweight
  in W001's pack; `ArtifactJoinRuntime` (both halves HELD, one per hand — two on the floor must not
  do it — and they REACH before they snap); `KeySocketRuntime` on the hull, the **only** thing in
  the game that re-arms the gate, with a test asserting nothing else ever hands it back; ten lines
  including Vex Bootstrapper's signature on both work orders.
- **⚠ NOT BUILT, deliberately:** the beacon thread and the hangar walk to berth six (a LOOK problem
  needing a reference plate, not code to guess at), W000's viewport and prior-owner log — *the
  opening of a space game still has no space in it* — and W002's rebuild/defend/grow loop.
- **⚠ NOTHING IN THE THREAD HAS RUN IN A HEADSET.** The join and the find are exactly the two beats
  whose FEEL decides whether any of it works.
- **Five CI reds, all mine, all named:** a copied constant, a namespace, leaked colliders, a missing
  import, and a test that failed on its own documentation. The three logging fixes added at the
  start of the session (failing test names · compile errors · audit blockers) each caught something
  on first use and turned a 20-minute artifact dig into one line.
- **Next:** `docs/production/LEVEL1_MASTER_TRACKER.md` and `docs/design/LEVEL_INVARIANTS.md`.

### 2026-07-29 (rb120) — LEVEL 1: four silent faults, then the missing middle

- **Terry, verbatim:** *"we're building the whole f****** level… I want everything from level one
  built all the way to level two. Everything down to the finest detail."* Protocols off, one goal.
- **FOUR FAULTS, EACH INVISIBLE TO CI, EACH DISABLING PART OF LEVEL 1.** Every owner satisfied its
  own contract; nothing asked whether a player could finish the level.
  1. **The first level's contract could never complete.** `JobDirector.CheckGoToMarker` searched only
     pack-declared markers. ToxicCity declares ONE (`player`) while three of its four steps point at
     `dispatch_inside` / `relay_node` / `shipyard_office` — objects `CityBuilder` authors straight
     into the scene. Step data right, scene right, lookup returned null. **W002 was therefore locked
     forever**, since its gate needs `toxiccity_complete`, which only the finished contract grants.
     Fixed: cached scene fallback + a loud `JOB_MARKER_MISSING` when an id cannot resolve at all.
  2. **W000's exit door led to a test scene** (`MilestoneA_GrabCube`). Retargeted.
  3. **One stale asset switched off half of W001.** `ToxicCityLayout.asset` predated `sceneName`,
     `experience`, `pois`, `hazards`, `creatureZones` and the sky block — all defaulted off — and the
     empty `sceneName` made ToxicCity list ITSELF as a ship destination.
  4. **Silent and empty.** No audio profile on either world though the asset existed; ZERO creatures
     placed though seven species ship; three RILL lines for both worlds combined.
- **THE MISSING MIDDLE WAS A MENU ITEM.** `ScenePatcherSpaceLane` is 316 complete lines — dock,
  cockpit frame, helm, five-ring course, three disable-and-salvage targets. It was reachable only
  from a menu item whose header asked Terry to run it once by hand. Nobody ever did, so
  `SpaceLane_Trial.unity` never existed and every plan calling the space leg "code-only" was
  describing a world that could not exist. **A world only a human can generate is a world that does
  not exist** — it now self-generates in the build like every other scene. Same class, same fix for
  `FirstHourSurfaceAuthor` (W000's comfort console, bunk keepsake and curated helm were absent from
  every APK while their code sat CI-green).
- **FH-S05 + FH-S08 shipped.** `CreatureDisabled` once per down cycle; `FirstHourDirector` conducts
  all 22 beats from the real owners' signals; `FirstHourW001Orchestrator` measures the safe
  observation window from the tracked head pose at ≥3 m and completes the payoff ONLY when the decal
  plate is on the hull. All 15 teaching lines authored on a new `Cue` trigger — they fire by id after
  the beat's hesitation interval, so a player who just does the thing hears nothing.
- **FH-A01 decided:** the Husk-Molter, because its counter is the only one that is a LESSON — swing
  at the decoy and you learn to read a creature before acting. Passport committed.
- **The ring city was UNEXPRESSIBLE, not unbuilt.** Rectangular districts and canals, and a landmark
  of {name, pos, h, w}. `RingCityDef` + `RingCityBuilder` ship all six elements; ToxicCity takes the
  tidal flat, canal ring, sea wall, harbour, outskirts and horizon pillars now. Tower island and
  wedges wait for the district re-layout — turning them on today would stand a 78 m leaning tower
  through the Dispatch plaza.
- **Audio:** no volume control existed anywhere. `AudioMixCore` + `AudioMixSettings` — zero means
  silent, stored in PlayerPrefs because a volume belongs to the room and not to a campaign.
- **Process:** the CI gate now names the failing test AND prints compile errors; `ci.yml` got the
  concurrency group it was missing. Both were open `FAST_LANE.md` §4 items, and both paid for
  themselves inside this session.
- **⚠ Heads-up:** `QuestDeviceCorrectionsRuntime` runs AFTER `ItemFactory` and overrides its grip.
  That is why every factory-side sword fix appeared to regress. `PUNCH IT` is deliberately NOT
  retargeted at the space leg yet — locked travel contract, and aiming the tutorial's one-way launch
  at a leg nobody has flown could block the whole level on a device session.
- **Next:** `docs/production/LEVEL1_MASTER_TRACKER.md` holds every remaining row and its proof.

### 2026-07-28 (rb119) — T-Dog lane: ⚡ FAST LANE — velocity fix after three trivial changes cost SIX HOURS

- **Terry, verbatim:** *"it literally needed to do like two or three fixes… basically just change
  the direction of the sword and like two other minor things… that took literally 6 hours. If it is
  going to take that long then I'm just tossing this project in the trash."* Treating this as a
  project-ending defect, because it is one.
- **MEASURED diagnosis (not guessed):** `docs/HANDOFF.md` is **3,155 lines / 138 entries**, and the
  laws require reading it + CLAUDE.md + OPERATOR_START_HERE + the checklist = **~3,600 lines of
  read-in EVERY session before any code**. Add per-change ceremony (HANDOFF entry, ledger, ratchet
  gate, board re-dating, catalog reconcile) applied equally to a sword angle and to a new subsystem.
  Add CI at a **60 min median / 176 min p75** verdict-to-verdict. Add a measured **27-hour
  continuous RED streak** (07-25 17:18 → 07-26 20:19) caused by a **stale board row, not code**.
  Add three serialised workflows before an APK exists. Add hand-written install steps regenerated
  per build. **Root cause: we built governance for 80 worlds and are running single screws through
  it.**
- **Shipped now:** **`tools/install_latest.ps1`** — one permanent installer (finds newest APK,
  prints SHA-256, uninstall-first per rb109, clears logcat, installs, supports `-Serial`/`-Both`,
  prints the logging command). Nobody hand-writes install instructions again.
  **`docs/FAST_LANE.md`** + **`docs/FAST_LANE_LOG.md`** — the tiered protocol.
- **⚠️ DISCOVERY THAT REMOVES AN OPERATOR FROM THE LOOP ENTIRELY:** `recovery-golden-android.yml`
  already has `workflow_dispatch`. **Terry can build his own APK from the GitHub mobile app**
  (Actions → Recovery Golden Android → Run workflow). ~12 min to artifact. He never has to wait on
  a session for a build again.
- **THE FAST LANE (adopted):** ≤3 files / ≤30 lines · no new system or contract · not touching rig,
  travel, save, input, build config or recovery artifacts · single-revert reversible · correctness
  decided by compile + existing tests + Terry's eyes. **Waives:** HANDOFF entry (one line in the
  fast-lane log instead) · MISS_LEDGER unless the class repeats · the ratchet gate (deferred as
  logged debt) · EXCELLENCE_MAP row · board reconcile · a dedicated build (batched). **Never
  waives:** compile, existing tests, revertibility, and the report-only law.
- **📣 MECHANICAL FIXES FOR WHOEVER OWNS CI (ranked by minutes saved, none change what gates check):**
  ① **Archive HANDOFF.md** to ~10 newest entries — a **10× cut to the per-session read-in tax**,
  highest-value single action available. ② **Move doc/board staleness checks OUT of Unity EditMode**
  — `GateGap5_NoBoardClaim_RotsSilently` costs a full Unity boot + 1,171-test run to report a stale
  date that `factory_governance_gate.py` reports in ~1 second; **this alone would have prevented the
  27-hour red streak**. Any check reading only `docs/**` belongs in the python preflight.
  ③ **Add a `concurrency` group to `ci.yml`** (golden-android already has one). ④ **Print the failing
  test name into `CI_VERDICT.md`** — finding it currently means downloading and parsing NUnit XML by
  hand. ⑤ One dispatch that chains CI → PlayMode → Golden APK unattended.
- **Not done unilaterally:** the HANDOFF archive and the EditMode→python gate move touch shared
  files while GPT is mid-fix; colliding there would cost more than it saves. They are specified
  precisely enough to apply in minutes.
- **Commit:** this one (installer + fast-lane protocol + log + this entry).

### 2026-07-28 (rb118) — T-Dog lane: 📋 ALL 24 CONCEPT SHEETS GIVEN BODY-CONTRACT PASSPORTS (pre-Tripo, docs only)

- **Terry's ask:** get the whole concept batch wired in while GPT works the device fixes.
  Art lane is collision-free with the runtime lane. Expanded
  **`docs/project_art_plan/BESTIARY_BODY_CONTRACTS.md` from 3 → 24 passports** (770 lines):
  21 creatures/machines + 3 plants, each with the four gate-required headings (Scale and
  ergonomics · State vocabulary · Interaction and collider envelope · Build acceptance).
- **Every passport settles what is IRREVERSIBLE at mesh time:** metre scale + threat-height band +
  closest-approach-to-headset · locomotion mode · the silhouette set the rig must reach (incl. the
  new pre-mesh **ambient** pose) · attack anatomy → required joints · full socket map (weak-points,
  face-safe grab handles, tether, feet, VFX/audio mounts, carry handle) · separable parts WITH cap
  geometry · tell-channel material region · collider proxies · variant mechanism. Every entry
  carries its **CI-required COUNTER** (`CreatureBehaviorReadabilityCatalog` enforces it).
- **Pre-mesh constraints surfaced that would have cost a remodel:** tether-swarm must be ONE
  instanced object, not 7 agents · light-grazer's two sizes should be a blendshape (continuous
  inflate), not a mesh swap · tide-phase's phase transition is a whole-body height-masked material
  pass, so UV/material regions must support it · bridge-former's BRIDGE FORM needs its own authored
  walkable collider · mimic-vault's closed box must match real container dimensions exactly and its
  unfold must not sweep through the player's head at 1 m · seal-ward is wall-mounted (scene socket,
  not floor spawn) · inverter's rig must be genuinely reversible (no "up" assumption) · orbit-grazer
  has no feet and the rig must not assume a floor · rewinder's after-images are ghost meshes, budget
  3 concurrent draws · tide-kite needs a carry socket between its hooks for the stolen item ·
  sludge-melon is a two-handed carry so it needs two opposed grab handles.
- **Flora passports added** (dew_bulb, rust_fern, sludge_melon) with the plant-specific pre-mesh
  call: **growth stages as separate meshes, harvested output as its own independent prop** with cap
  geometry, plus harvest-reach checked against seated/child height.
- **§0 rewritten** as the full 24-sheet ingest recipe (filenames, `concepts/bestiary_ch1/` +
  `concepts/flora_ch1/`, manifest entry shape, the four requiredMarkers, gate run). Registration
  still deliberately deferred — `concept_intake_gate.py:214` verifies conceptPaths exist on disk
  and the PNGs are still on Terry's phone. **Outstanding sheets: `sump_tender`, `glass_reed`.**
- **Verified:** `concept_intake_gate.py` PASS · `catalog_doc_sync_gate.py` 0 blocking / 0 warnings.
  No runtime, recovery, first-hour, licensing or release file touched.
- **Commit:** this one (passports + this entry).

### 2026-07-28 (rb117) — T-Dog lane: 🌾 AMBIENT LIFE research → creature card BLOCK D + ⚠️ TWO VERIFIED PERF BUGS

- **Terry's ask:** creatures should be DOING something when you come across them, gated by distance
  or line of sight so it doesn't cost performance. Researched (AC Unity AI Recycling, KCD2, Horizon,
  Rain World abstractization, theHunter animal AI, Watch Dogs Census, Nemesis, Unity CullingGroup /
  animator culling, Quest budgets, VR impostor limits) and wired the result into
  `CREATURE_ROSTER_AND_PROMPT_QUEUE.md` as **Block D + field 3b + the tier model + the anti-pop-in law**.
- **⚠️ TWO LIVE PERF BUGS FOUND AND VERIFIED IN SOURCE** (`CreatureBehaviorBase.cs`, runtime lane —
  NOT my lane to fix, flagging for the owner):
  ① **`Update()` runs unconditionally for every creature every frame** (`:56-64`) — `Vector3.Distance`
  + `Tick` + `TryTouch` per creature per frame with **no distance gating at all**. There is currently
  no AI LOD in the project.
  ② **Full-scene type scan per creature per frame**: `Update` calls `FindPlayer()` whenever
  `Player == null` (`:59`), and `FindPlayer` calls `FindObjectOfType<PlayerStunReceiver>()` (`:82`).
  If the rig isn't found — early frames after a `TravelCoordinator` load, or any world missing the
  receiver — **every creature runs a full-scene scan every frame, forever.** Real Quest hazard,
  independent of the ambient question. Also `Physics.SphereCastAll(..., ~0, ...)` (`:96`) is
  allocating and all-layers, with `GetComponentInParent` per hit.
- **⚠️ TERRY'S INSTINCT CORRECTED ON ONE POINT:** distance-tiering YES, **line-of-sight tiering NO**.
  A VR player turns their head in ~200 ms with no camera cut, and will stand still and STARE. Visibility
  may PROMOTE, never demote. Use `CullingGroup` distance bands and ignore its `isVisible` flag.
- **The reframe that saves the work:** the expensive thing was never "creatures doing something" — it
  was "creatures DECIDING what to do." Make ambient a pure function of `(seed, worldTime)` and a
  creature at 100 m is correctly mid-graze, at the right phase, in the right place, for the price of a
  sine wave. **We are already ~60% there** — `ForgeCreatureAnimator` (gait from `(body, Time.time,
  speed01)` + per-instance breath seed) and `WorldAmbientMotionRuntime` (`sin(time+seed)`) are exactly
  the right primitive; extend from pose to activity+position and add a tier manager above.
- **Card changes:** new **field 3b AMBIENT SILHOUETTE is PRE-MESH** (can the Dredge-Bull's plate
  physically reach the ground to graze? that's a rig constraint, not an animation one) · new Block D
  fields 24-32 (anchor/den, activity set, schedule, **trace props**, notice-but-not-alarmed +
  displaced, tier profile, ambient audio that outlives the visual tier, promotion safety, social beat)
  · clip budget **12 → 14**. ⚠ `witness_mite` needs a tier exemption — "moves only when unwatched"
  structurally collides with observation-based culling.
- **Prerequisite flagged:** `docs/07_PERF_BUDGET.md` is still entirely TBD. You cannot tier a budget
  you have not written down; proposed numbers are in the research (13.9 ms frame, ≤1.5 ms all creature
  AI, ≤2.0 ms animation+skinning).
- **Commit:** this one (card Block D + 3b + tier model + anti-pop-in law + this entry).

### 2026-07-28 (rb116) — T-Dog lane: 🎨 THREE BESTIARY SHEETS APPROVED + body contracts written (pre-Tripo)

- **Terry generated prompts 4-6 and all three came back build-ready:** **Husk-molter** (incl. the
  empty husk from 3 angles WITH a hollow-interior view — the separable prop we need), **Dredge-Bull**
  (the new Bruiser; charge wind-up and the exposed vent in the winded pose both read instantly),
  **Warden Sentinel** (chest-iris detail panels excellent; kneeling disabled pose).
- **⚠️ Sentinel deviated from spec: it came back HUMANOID**, not the specced four folding limbs.
  Recommendation recorded: **KEEP it** — a humanoid rig auto-rigs and retargets far more easily,
  which materially helps the Tripo pipeline. But logged a **kid-comfort flag ⚖**: a silent,
  faceless, 3 m humanoid approaching you in VR is much more intimidating than a bird-legged
  machine, and this is an E/E10 family title. Mitigations already in the design (it warns first,
  never enters arm's reach, unarmed, kneels when disabled) plus a **hard 1.5 m minimum approach**
  for this species and a child-tester review before it ships in a required encounter.
- **Wrote `docs/project_art_plan/BESTIARY_BODY_CONTRACTS.md`** — Block A/B/C passports for all
  three: metre scale + threat-height band + closest-approach-to-headset, full state vocabulary
  incl. the **CI-required COUNTER** and the weakened tell, socket maps (weak-point, grab handle
  placed away from the face, tether, VFX/audio mounts, carry handle), separable parts with cap
  geometry, collider proxies, and build-acceptance tests. Applied the VR timing rule: the
  Dredge-Bull's charge telegraph is **0.8 s** because sidestepping is a whole-body answer.
- **⚠️ DELIBERATELY NOT REGISTERED YET:** `concept_intake_gate.py` verifies every `conceptPaths`
  file exists on disk (`:214`). The three PNGs are still on Terry's phone, so adding manifest
  entries now would turn CI red. §0 of the new doc carries the exact 4-step registration recipe
  for when the images are committed at the computer. Gates verified green as-is.
- **Process correction for all operators:** Terry is on his PHONE by default and cannot open repo
  files. Deliverables he needs to act on (prompts, commands, lists) must be IN the chat message,
  not only committed. Docs are for the record; chat is the interface.
- **Commit:** this one (body contracts + this entry).

### 2026-07-26 (rb114) — T-Dog lane: 🐛 CREATURE ROSTER + PROMPT QUEUE (Terry's Tripo run-up)

- **Terry asked** for the concept-prompt framework, what creature art already exists, the full
  expected roster, and the first prompts to run in Gemini before his Tripo membership (~1 week).
  Wrote **`docs/project_art_plan/CREATURE_ROSTER_AND_PROMPT_QUEUE.md`** against the canonical
  `CONCEPT_ART_PROMPT_PLAYBOOK.md` (§3 template, §5.1 species archetype, §4 constants).
- **⚠️ HEADLINE FINDING — four SHIPPED creatures have NO concept art:** `witness_mite`,
  `light_grazer`, `husk_molter`, `tether_swarm`. Correction after checking `ForgeBodyLibrary`:
  the BODY pipeline is fine — six of seven have forged bodies defined (only 2 committed as
  assets; the rest are patcher-generated, so a disk listing under-reports them), and
  `tether_swarm` intentionally has none (cluster + cord, not one body). So these bodies were
  built BLIND with no reference — the exact condition the pipeline doc blames for the Warden
  taking five rounds. Art for them is the highest-value work, ahead of any new species.
- **⚠️ ID DRIFT to resolve BEFORE models are imported:** concept sheets say
  `creature_cistern_swarmer` / `creature_glass_tendril`; shipped assets say `swarm_bug` /
  `tendril`. Pick one ID set now — retrofitting post-import is the expensive version. ⚖ Terry/art
  lane.
- **Roster filed:** 5 approved sheets (canal stalker, cistern swarmer, glass tendril, warden drone,
  rogue-drone family + Wake-Guild spider, plus the pulser sheet) · the 12 novel behaviours from
  `CREATURE_DESIGN.md` with art status each · the 4 archetypes — **Bruiser is a genuine roster hole
  (no heavy exists at all)** · Warden classes beyond the drone, Architect constructs, and the three
  named bosses all unarted.
- **Locked laws restated for prompt consistency** (already blessed, now in one place): two glow
  languages (biological bioluminescence vs machine powered-light, never mixed) · machine-eye colour
  law (RILL amber / Warden white / hostile RED; player cyan vs hostile red-orange) · Warden kinship
  family · Wake-Guild anchor+cog · every hostile REQUIRES a disabled/powered-down panel (non-lethal
  canon).
- **Three full prompts written and paste-ready** (Witness-mite, Light-grazer, Tether-swarm), each
  demanding the state extremes the rig will need. Next in queue: Husk-molter (its shed husk is a
  separate prop), a Bruiser heavy, Fractal-splitter, Bridge-former.
- **The part that makes them game creatures:** every sheet must carry a **six-line behaviour card**
  (IDLE · NOTICE tell · ~0.4 s ATTACK telegraph · HIT reaction · STUNNED window · DOWNED pose) tied
  to the existing state machine and the `CreatureBehaviorReadabilityCatalog` requirements — because
  a sheet showing only a neutral pose produces a Tripo mesh that cannot be rigged for the states the
  game actually plays. Six-step Tripo-month workflow recorded.
- **Commit:** this one (roster/prompt doc + this entry). No code; art lane unclaimed.
### 2026-07-27 (gpt-first-route-feel-golden-ready) — first-route packet merged; exact Golden candidate authorized for Quest evidence

- **Did:** reconciled live `terry-local-wip` at docs-only head `b83185da` against tested vehicle source `bc8b73cb`, then completed full-file review of `gpt/first-route-feel-live-20260726`. Found and corrected one guaranteed stale test (`REWARD SECURED` after ownership wording changed), ObjectiveBoard subscription/disable/material lifecycle gaps, RepairableMachine procedural clip/runtime-material cleanup and interrupted-pulse restoration, plus source tests that could match comments. Opened bounded PR #90 with exactly four authorized files and squash-merged as executable source `cf94c60883c0f80aaeddacf08a65cd89a929e9f7`. Exact ordinary CI is GREEN run `30258567706` (EditMode + patch/world audit + contract reports; Android skipped). Exact Recovery PlayMode is GREEN run `30258567679`, 43/43 with 0 failed/skipped/inconclusive. Exact Recovery Golden Android is SUCCESS run `30258567678`; artifact `recovery-golden-apk-cf94c60883c0f80aaeddacf08a65cd89a929e9f7`, artifact ID `8650189616`, APK bytes `102563558`, independently recomputed APK SHA-256 `EB018B4EEA643D080443784E1039D0A96090517DD2EC302B0797A8AD0C16ED2F`. Build profile is `GoldenSlice`, define `ZIPTIDE_RECOVERY_GOLDEN`, locked scenes `_Boot`, `W000_DriftIn`, `ToxicCity`. Artifact audit has 0 blockers and 179 warnings. `_Boot` has 0 warnings; W000 has 8; ToxicCity has 15, including the existing report-only material warning at 62 unique materials versus cap 60.
- **Next:** Terry installs ONLY the exact artifact above using the rb109 uninstall-first law, verifies the APK hash before install, starts complete logcat before launch, then runs Boot/Home Hub → W000 locomotion/weapon scale+pose/holster laser/R3/coupler repair feedback/PUNCH IT → ToxicCity hazard/objective/vehicle/Y Return to Ship → repeat route → relaunch/Continue. Capture exact timestamps/screenshots for blockers, falls/spawn overlap, judder, material/warning growth and excessive combat pressure. M0 closes only from this Quest 3S evidence, never from automated green alone.
- **Heads-up:** first-route repair/objective presentation, recovery fixes and vehicle fixes remain DEVICE-YELLOW. No speculative fix was made for the prior single fall or `buriedAtTorso=True`; Quick Swap B remains unproven until a gun is deliberately socket-selected and `QUICK_SWAP` evidence is captured. Picasso ownership of `Visuals/**`, Forge/art/water/materials/shaders and FH-A01 remains intact; FH-S05 still waits for FH-A01 and FH-S08 remains last. The audit warning debt is real and must inform the later production pass, but it did not block this bounded M0 checkpoint. No executable source changed after `cf94c608`; later descendants are generated proof/CI/handoff only.
- **Commit:** PR #90 squash `cf94c60883c0f80aaeddacf08a65cd89a929e9f7`; CI verdict run `30258567706`; PlayMode observation run `30258567679`; Golden Android run `30258567678`; this queue-entry commit follows.
### 2026-07-27 (gpt-first-level-product-contract) — complete W000→space→W001→return route audited and unified

- **Did:** audited the current recovery route, `first-hour-v1` 22-beat contract, first-hour envelopes, `ShipCastOffRuntime`, `ShipFlightRuntime`, `SpaceTargetRuntime`, `ScenePatcherSpaceLane`, flight/ship/space-combat plans, W001 production order and T-Dog's first-two-worlds report. Added `docs/production/FIRST_LEVEL_PRODUCT_CONTRACT.md`, the canonical production target for the complete first playable level and W002 replication framework. It defines the full band: boot/home → W000 identity + coupler scan/repair → board/PUNCH IT → generated playable space leg → non-lethal disable/salvage → first Ziptide → approach/reentry → W001 arrival/job/creature/zipline/story/reward → extraction → changed ship/home/save payoff. Updated `FIRST_TWO_WORLDS_STATE_AND_ROUTE.md` so its city route is explicitly a subset of the full product contract.
- **Found:** ZIPTIDE does not yet have one coherent first-level implementation. The current machine contract omits the mandatory W000 coupler even though `PUNCH IT` is hard-gated by it; it jumps helm → first Ziptide → W001 while `CONTROLS_AND_FLIGHT` places a guided flight before the first Ziptide; live `ShipCastOffRuntime` still travels directly to ToxicCity; SpaceLane flight/combat/salvage exists as source/test code but `SpaceLane_Trial.unity` and its WorldPack are absent from the live branch; reentry/approach has no canonical owner or beat; W001 City Stage A, signature creature resolution and final orchestration remain unbuilt. This is a real contract/integration gap, not merely polish.
- **Next:** Terry runs the exact `cf94c608` Quest packet first. A systemic M0 blocker remains priority zero; bounded feel/content/presentation notes go to the ledger without freezing production. On a non-systemic verdict, begin Wave 1 from `FIRST_LEVEL_PRODUCT_CONTRACT.md`: migrate the 22-beat first-hour contract to v2, move first scan/repair teaching to the W000 coupler, add launch/space/flight/salvage/Ziptide/reentry/extraction signals, define safe persisted-beat migration, and prove every beat has a real producer or named blocked dependency. Then build the production route seam W000 → generated space leg → W001 through existing owners before broad city/content work stacks.
- **Heads-up:** do not treat `SpaceLane_Trial` as shipped content merely because its runtime and patcher exist. Do not add a second travel/flight/job/repair/reward/save owner. For the model band, ascent/reentry is an authored comfort-safe transition around `TravelCoordinator`, not a seamless planet-scale physics program. The first level must exercise the full route vocabulary once; W002 must reproduce it materially faster through data/recipes or the factory is not ready for W003.
- **Commit:** contract `0a013e7d2714bcc1d86b55656b3bce329fe18e94`; city-route binding `c8793eec9839f02a2a2ca7df2058739fc5a6da1c`; this queue entry follows.


### 2026-07-26 (rb113) — T-Dog lane: 🗺️ FIRST TWO WORLDS — state + route (answers Terry's "why is the city ugly / what's the concept→cityscape plan")

- **Terry asked** where level 1 and 2 actually stand across art, SFX, city layout, skyscape, space
  flight and the first hour, and what the plan is for turning his approved concept art into the
  real cityscape. Surveyed the repo and wrote
  **`docs/production/FIRST_TWO_WORLDS_STATE_AND_ROUTE.md`** (routing doc — replaces nothing).
- **The answer: the plan EXISTS and is approved; the gap is execution.** `CITY_VISUAL_SPEC.md` is
  🟢 REFERENCE APPROVED and defines ToxicCity precisely off Terry's own K1–K6 kit (concentric
  rings on a drowned tidal flat: Tower island → shanty wedges + canal ring → breached sea wall →
  harbour wedge/breakwater/moored Scrapper → flyable outskirts with wrecks, stilt villages, glowing
  tide pools → NE gate pillar ring). **None of that shape is built:** `CityBuilder.cs` still emits
  the older boulevard layout, and the handoff record states plainly that *"City Stage A remains a
  separate later bridge"* — both the bridge doc pass and the code pass are unstarted. That is the
  complete explanation for "doesn't make sense and is super ugly": a generic layout with no
  authored intent, nothing to do with the concept kit.
- **Also confirmed:** `CONCEPT_TO_BUILT_PIPELINE.md` already defines the 5-step route and step 1
  (measured visual spec) is a SOLVED skill — proven twice (ship + city). What has never run for the
  city is steps 3–5 (booth loop against the sheet → gates → device verdict).
- **State table filed** for city/concept-machinery/buildings/skyscape/SFX/first-hour/space/creatures
  across W001+W002. Thinnest lanes: **city Stage A (highest visible impact)** and **audio** (1
  track, 0 VO, no SFX library, no mix pass — master plan + 34-row queue ready to execute).
- **Route recorded (all behind M0 per POST_HEADSET_WORLD_FACTORY_ORDER §2):** bridge the city spec
  into a Stage A recipe (cheapest, ~1 session) → build Stage A **plus the booth reference-plate so
  built-vs-concept is one image** → K4 facade module recipes → W001 skyscape signature pass (K2) →
  audio rails THEN the paid SFX batch → close first-hour payoff beats (FH-A01→S05→S08) → W002 as
  the replication test. Paid 3D month still starts at the W002 gate, not before.
- **Named the recurring failure mode for the record:** specification outruns execution and the gap
  stays invisible until the headset. The fix is running the concept→built loop once end-to-end with
  the sheet pinned beside the render, then repeating it.
- **Commit:** this one (routing doc + this entry). No code, no lane claimed — city Stage A remains
  GPT's lane and this doc is reference for whoever fires it.

### 2026-07-26 (gpt-recovery-vehicle-first-route-handoff) — recovery + vehicle green; first-route feel branch pending

- **Did:** shipped the bounded Quest 3S recovery fixes through PR #87: Y field menu with Resume/canonical Return to Ship and post-travel XRI rebinding; Breaker Blade-only tracked-hand pose; holstered/socketed guns no longer show laser sights; centered-stick R3 crouch/slide guard with blended camera height; modest Home Hub distance retune retaining direct-hand liveness; toxic exposure aligned to the visible rendered surface; taser velocity cleared before kinematic lock. The initial test-only red was 1177/1178 passing and caused solely by matching `Time.timeScale` in an XML comment; corrected in `c0dc9c803c66030f794c23cc2bcdc0c5c401319b`. Exact ordinary CI is GREEN for `c0dc9c80`, run `30224832279`. Then shipped the isolated vehicle pass through PR #89: compact seat-side mount affordance, X dismount, Y menu remains available while mounted, hoverbike eye-clearance channel, split low controls, smooth deadzoned steering, and rig world-yaw follow while preserving local HMD freedom. Exact ordinary CI is GREEN for `bc8b73cb741a9c8e10eb7a0949a229226b54f95f`, run `30225458255`; durable verdict commit is `9a4a5dbad3e996954df2422cf49989896e1f46f8`.
- **Next:** finish the isolated branch `gpt/first-route-feel-live-20260726` at head `78fe6212076827f0e12817868acacc7630b41743` before further source work. It is five commits ahead of live and changes only `Gameplay/Runtime/Jobs/ObjectiveBoard.cs`, `Gameplay/Runtime/Story/RepairableMachine.cs`, `Tests/EditMode/FirstRouteFeelTests.cs`, and its `.meta`. It adds exact-hand bounded repair haptics/local procedural stage feedback plus clearer objective/contract presentation while keeping reward/save/quest state with existing owners. No PR has been opened and it has not run authoritative CI. Review complete files/diff for Unity lifecycle, duplicate feedback, controller-null fallback, material/audio cleanup and test truth; open one bounded PR against `terry-local-wip`; merge only when review-clean; then wait for exact Unity EditMode + patch/audit GREEN. After final source is green, obtain an exact-source Recovery Golden Android APK and prepare Terry's next-day Quest route and evidence packet.
- **Heads-up:** all new recovery/vehicle behavior remains DEVICE-YELLOW until Terry tests on Quest 3S. Do not claim M0 closed from CI. Retest Boot/New Game → W000 locomotion/coupler/PUNCH IT → ToxicCity → Y Return to Ship → repeat route; verify Breaker Blade pose, gun laser hidden while holstered, R3 no longer combines turn/drop/slide, Home Hub distance, visible toxic contact boundary, no kinematic warning spam, vehicle eye clearance/mount/X dismount/smooth steering/yaw follow, and post-return input. The single prior fall/spawn overlap was not guessed at; investigate only if reproduced with logs. Excess combat pressure, 1%-low spikes/material growth and later Quick Swap B proof remain open. Respect Picasso ownership of `Visuals/**`, Forge/art authors/water and FH-A01; FH-S05 waits for FH-A01 and FH-S08 remains last. Ordinary CI normally skips Android, so do not hand Terry an APK without exact source SHA, workflow run and artifact SHA. Keep the longer production order in `docs/production/POST_HEADSET_WORLD_FACTORY_ORDER.md`: device proof → W000/W001 model band → W002 replication → automation ratchet → W003-W005 mini-batch → chapter batches; paid 3D/audio months and reusable framework extraction only after their named contracts are proven.
- **Commit:** recovery functionality squash `134aefa6dcf1b5f4cbf275889f1a2991a93a629a` + test correction `c0dc9c803c66030f794c23cc2bcdc0c5c401319b`; vehicle squash `bc8b73cb741a9c8e10eb7a0949a229226b54f95f`; pending feel branch head `78fe6212076827f0e12817868acacc7630b41743`; this queue-entry commit follows.


### 2026-07-26 (gpt-m0-device-verdict-54e23022) — Quest evidence closes major recovery questions; M0 remains open

- **Did:** inspected Terry's complete evidence bundle `ziptide_headset_evidence_20260726_143829.zip` (SHA-256 `fca46c99bd2fafdc46ca6a49b65b5cdf00753dcd2e91cda450705b8807dc27e7`) from Quest 3S against exact Recovery Golden Android run `30213241501`, source `54e230229e8bbe57af69ccd0ec6ce77a9c8bc501`, APK SHA-256 `0141A7858C358F3EDD317603A55C48004B976BF39E442CAD3455D48600759B8F`. Added `docs/recovery/M0_DEVICE_VERDICT_54E23022_20260726.md`. Device evidence now positively proves Home Hub liveness, two active production rays at boot/W000/ToxicCity, New Game, W000 locomotion, complete coupler repair, ship board/PUNCH IT, ToxicCity travel, and post-travel input recovery. Source-correlated failures: Home Hub tile row at 0.45 m; Breaker Blade default/override pose points forward; `GunLaserSight` mistakes socket selection for hand-held selection; R3 crouch/slide + 0.55 m camera drop conflicts with smooth-turn stick use; vehicle rider follows seat position but not vehicle yaw; procedural fork/control bars and giant RIDE/STEP OFF cubes obstruct the cockpit; ToxicRiver uses large AABB bounds rather than rendered-liquid contact; one real fall and `buriedAtTorso=True` spawn probes; repeated kinematic-velocity warnings; excessive combat pressure; four performance 1%-low spikes and material growth requiring report-only follow-up.
- **Next:** recovery owner should assign one narrow candidate: deterministic in-headset pause/return path; Breaker Blade-only pose correction; laser hidden for holster/socket selection; R3 comfort/control resolution; modest Home Hub distance retune preserving live anchor/direct-hand fallback; toxic visual/consequence boundary alignment; investigate the fall/spawn overlap; fix touched kinematic warning order. Keep vehicle work in a separate mini-sprint: measured rider eye-clearance envelope, hierarchy-identified obstruction removal/rebuild, contextual VR mount/dismount, one vehicle-yaw owner with rig yaw following while preserving local head freedom, deliberate smooth/snap steering choice, then device proof. Later target B with a gun deliberately socket-selected and require `QUICK_SWAP` evidence; this run did not prove B broken.
- **Heads-up:** M0 remains open because melee orientation failed and the second full route could not be repeated due to no world escape/menu path. Boot, coupler, travel, and post-travel input now have strong positive device evidence, but freeze-lift scope belongs to the recovery owner. No code, scene, prefab, test implementation, workflow, asset, build, or APK changed. The ZIP and screenshots are hashed in the report but still need durable binary archival if required beyond conversation/file-library retention. Do not mix ship art polish into the recovery candidate unless geometry blocks play.
- **Commit:** `2b211631cf188a5de633ae392ab08ddf15ea84f9` (device verdict report); this queue entry commit follows.


