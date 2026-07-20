# PERCEPTUAL GATE PROGRAM — closing the space between "tests pass" and "it's right"
### Why device testing keeps finding what CI can't see, the five gates that fix it, and the ratchet law
**Status: RESEARCHED SPEC — ready to implement. 2026-07-19, Terry-directed. Implementation owner: GPT (hwr31).**

The problem, in Terry's words: *"if we have the project basically set... why do we still have janky
architecture and random spaces in the map... why do I have to go into the Meta Quest to find out
the weapons are super tiny? It's going to be very difficult to build 80 worlds at consistent
quality if our system isn't top-notch."*

---

## 0 · DIAGNOSIS (grounded in this week's actual escapes)

Our gates verify **logic** (compiles, 1,062 EditMode tests, contracts, budgets, save round-trips).
The escapes are all **spatial or perceptual truths** no current gate evaluates:

| Device-found bug | Why it escaped | Verified root cause |
|---|---|---|
| Weapons tiny; muzzles point up | **No gate examines geometry of factory output.** Nothing measures a held item against a hand, or a muzzle against grip-forward. | No scale/pose audit exists anywhere in `Editor/Audit/` (verified 2026-07-19). |
| ToxicCity Leave door loaded a scene absent from the build | **A gate EXISTED and was correct** (`TRAVEL_DEST_NOT_IN_BUILD` blocker in `WorldAuditRunner.CheckTravelDestinations`) **but validated the wrong universe**: it reads `EditorBuildSettings.scenes` (full list, where `MilestoneA_GrabCube` IS enabled), while the Golden profile shipped only 3 locked scenes. | Gate/build-profile mismatch — the audit never runs against the shipped scene set. |
| Shipyard walkway gap | Walkable continuity is never sampled. `WorldReachabilityAuditRules` emits only a soft `POI_MAYBE_UNREACHABLE` **Warning**, and only for POIs. | Confirmed: one Warning-level rule, no route sampling. |
| "Boxes/janky architecture" | Not a gate failure — a **recipe ceiling** (see `CITY_STREETSCAPE_AND_AMBIENT_LIFE.md`). Gates make quality *consistent*; recipes make it *high*. Keep the two fixes separate. | — |
| Everything visual generally | `forge-photos.yml` renders images to an artifact — but **no process requires anyone to look** before a lane closes, and items render with no scale reference, so "tiny" isn't visible. | Verified: workflow exists, review is optional, no reference frame. |

**The law this program installs:** every quality dimension gets a gate that runs at build time —
**logical** (have it: tests/contracts) · **spatial** (this doc: geometry audits) · **perceptual**
(this doc: referenced contact sheets + agent eyes) · **performance** (have it: budgets) · **feel**
(irreplaceably human — Terry's headset minutes are spent ONLY here once the rest is gated).

---

## 1 · THE GATES (implementation specs — all build on existing infra, no new frameworks)

### PG-1 · Held-item scale & grip-pose audit  ⚙ NEW `Editor/Audit/HeldItemAuditRules.cs`
**Catches:** tiny/giant weapons, upward muzzles, double-applied scale — this week's bug class.
**How:** register in `WorldAuditRunner.RunAll` (belt-gate idiom). In an EditMode context,
instantiate every registered item via `ItemFactory` (iterate `Resources.LoadAll<ItemDefinition>`)
into a temp scene, then per item:
- **Bounds sanity (Blocker `ITEM_SCALE_OUT_OF_BAND`):** combined renderer bounds' max dimension
  within **0.06–0.55 m** for one-hand items (band per-definition overridable via an explicit
  `expectedSizeBand` field, so a two-hand pike can declare its own). This is the tape measure
  no one was holding.
- **Compound-scale trap (Blocker `ITEM_SCALE_COMPOUNDED`):** flag any renderer whose *lossyScale*
  differs from its authored local intent by >2× — the exact "primitive scale applied twice" fault.
- **Grip pose (Blocker `ITEM_POSE_MISALIGNED`):** for items with a `Muzzle`/`Lens`/named forward
  child: angle between grip-forward (attach transform forward) and that child's forward ≤ **25°**.
- **Held-only check** — items without grips (props) skip pose, keep bounds.
Destroy everything in `finally` (RuntimeHealthMonitor discipline). ~2 commits incl. tests.

### PG-2 · Build-profile-aware travel audit  ⚙ EDIT `WorldAuditRunner` + recovery workflows
**Catches:** the Leave-door class — valid-in-editor, broken-in-shipped-build destinations.
**How:** parameterize `GetBuildSceneNames()` with an explicit scene list:
`RunAll(IReadOnlyCollection<string> shippedScenes = null)` defaulting to editor settings. The
Golden/recovery build method (`RecoveryBuildAndroid.PatchScenesThenGoldenAPK`) already knows its
locked list — make it invoke the audit with that list and **fail the build** on any
`TRAVEL_DEST_NOT_IN_BUILD`. Also emit `TRAVEL_DEST_UNREACHABLE_IN_PROFILE` for destinations
reachable only via excluded scenes. Rule of the class: **a gate must run against the artifact's
actual universe, not the editor's.** Audit every other rule for the same assumption while there
(spawn markers, packs, jobs referencing excluded content). ~1–2 commits.

### PG-3 · Walkable-continuity audit  ⚙ NEW `Editor/Audit/RouteContinuityAuditRules.cs`
**Catches:** the shipyard-gap class — holes/ledges/dead-ends on authored routes.
**How:** derive the route graph from data that already exists: `__SPAWN_PLAYER` → every
`Marker_*` (JobDirector job steps) → every travel door/`WorldTravelStation` → shipyard berth;
plus `CityLayoutDefinition.connections`. For each edge, sample every **0.25 m** along the
straight segment (v1; follow `ElevatedWalkway`/`GroundStreet` objects when named): raycast down
from +2 m;
- **Blocker `ROUTE_HOLE`:** no hit within 3 m below expected walk height.
- **Blocker `ROUTE_STEP`:** consecutive samples differ by > **0.35 m** (step ceiling) without a
  Ramp-named object nearby.
- **Warning `ROUTE_SQUEEZE`:** lateral clearance < **0.7 m** (comfort minimum).
Promote `POI_MAYBE_UNREACHABLE` from Warning to Blocker for job-critical markers (any marker a
`JobDefinition` step references). ~2–3 commits incl. a deliberately-broken-scene test.

### PG-4 · Reach-envelope audit  ⚙ NEW rules in the same file as PG-3
**Catches:** interactables floating out of reach, controls above the child-reach ceiling.
**How:** for every interactable (XRSimpleInteractable/XRGrabInteractable/repair stage points/
kiosks/buttons): nearest walkable point (PG-3's sampler) must be within **0.65 m** horizontal,
and the interaction point **0.35–1.9 m** above that walkable surface (the coupler's
"child-height reach" law, generalized — the PRESS POWER fix proved the need). Blocker
`INTERACTABLE_OUT_OF_REACH`. Per-object opt-out field for deliberate high objects (zipline
anchors) with a required reason string. ~1 commit.

### PG-5 · Referenced contact sheets + mandatory review  ⚙ EDIT `ForgePhotoBooth` + process law
**Catches:** everything "you have to see": proportion, palette, readability, place-vs-boxes.
**How, mechanical half:** every held item renders **next to a reference mannequin silhouette
(1.7 m) and a 10 cm grid**, camera at eye height — scale error becomes visible in a 2D image
(this is why "tiny" survived the existing unreferenced renders). Worlds render from
`__SPAWN_PLAYER` eye pose (the arrival view) + each hero interactable at interaction distance.
Keep it in the existing `forge-photos.yml` artifact flow.
**Process half (Definition of Done amendment):** a lane that changes any player-visible geometry
closes ONLY after an operator downloads the sheet, looks, and writes one line in HANDOFF:
`SHEET REVIEWED <workflow-run> — verdict`. No stamp, no close. (An LLM operator can and should
do this review — screenshots are exactly what we CAN judge; log what looks wrong even when
merging anyway per Terry's broken-is-data tolerance.) ~2 commits + `OPERATOR_START_HERE` edit.

### PG-6 · On-device agent bus (designed here, build post-recovery)  ⚙ `Gameplay/Runtime/Debug/DevAgentServer.cs`
**Catches:** whole-loop truths only the real device shows (thermals, real timing, whole-APK boot).
**How:** dev-build-only TCP server on localhost (reach via `adb forward`), speaking JSON verbs
that map 1:1 onto existing systems: `list_state` (position, held items, nearby interactables,
active job step), `goto <marker>` (teleport-step locomotion, never smooth — comfort laws bind
agents too), `grab <itemId>` / `activate` / `release`, `press <name>`, `travel <world>`,
`screenshot` (adb screencap on the PC side). An LLM session drives the Quest on Terry's desk
overnight: walk every route, open every door, file structured findings. Meta's **Quest Agentic
Tools (MCP)** cover the device side (deploy, logcat, screenshots) — adopt rather than rebuild;
our half is only the in-game verb server. Explicit non-goals: no comfort/fun verdicts (human),
no shipping in release builds (compile-defined out). ~3 commits, AFTER recovery exits.

**Implementation order for GPT: PG-2 → PG-1 → PG-5 → PG-3 → PG-4 → (post-recovery) PG-6.**
PG-2 first because it's a live bug class with a one-day fix; PG-1 next (just bit us, cheap);
PG-5 before PG-3/4 because it catches the widest class for the least code.

---

## 2 · THE RATCHET LAW (process — this is what makes 80 worlds possible)

**Every device-found bug must die three deaths:** (1) the fix; (2) a gate that would have caught
it before install; (3) a named line in HANDOFF: `RATCHET: <bug> → <gate/tag>`. A fix without a
gate is a loan, not a payment — the class WILL recur in world #47.

Adopted into the Definition of Done in `OPERATOR_START_HERE.md`. GPT's coupler/exit workflow
coverage from the 2026-07-19 session is the model case — that instinct, made law. Terry's
headset session notes become the gate backlog; each session should *shrink* the class list, and
a session that finds only NEW classes (never repeats) is the metric that the ratchet is working.

---

## 3 · THE CONSISTENCY FRAMEWORK (beyond this game — Terry's actual goal)

Terry: *"we're not just building this game, we're trying to build a framework that could make
other games like this and make them consistent regardless of what the game is."*

What we've actually built, named honestly, is a **five-layer game factory**. None of it is
Ziptide-specific except the data:

1. **Spec-is-truth** — every aspect has a design doc that outranks code (`docs/design/*`,
   EXCELLENCE_MAP as the registry of aspect → state → standard → guardrail).
2. **Deterministic generators** — content is data (ScriptableObjects/JSON) compiled to scenes by
   patchers; no hand-built content (`CityBuilder`, `WorldStubGenerator`, forge, `*Author`).
   FORGE VI ("World Compiler") already formalizes the endgame: recipe + compile pass + hash law.
   This program feeds it, not competes with it.
3. **Gate-per-quality-dimension** — the table in §0: logical, spatial, perceptual, performance
   gates in CI; feel reserved for humans. A new world ships when its gates pass, its sheet is
   stamped, and its device pass has evidence — *the same bar for world 3 and world 78*.
4. **The evidence blackboard** — HANDOFF/runbook/verdict files that let any model, any session,
   any vendor pick up the work cold (this week proved it across Fable, GPT, and a lost chat).
5. **The ratchet** — §2. Quality is monotonic because lessons become gates, not memories.

**To make "each world consistently excellent, all aspects":** the per-world contract is the
checklist a `WorldExcellenceAuditRules` can enforce mechanically — every shipped world has: a
pack + recipe (no hand-authored scenes), all §1 gates green, a stamped contact sheet, an
ambience declaration, a job/beat set (`GateGap3` already enforces this), perf within budget on
its row, and a device-pass evidence line. That rule set IS the franchise: swap the art kits,
verbs, and story data, and the same factory builds a different game to the same floor.
The EXCELLENCE_MAP row becomes the single place where "is this aspect at standard?" is answered
— and the map itself should gain a generated column: which gates cover each row (aspects with
zero gates are where the next escape lives).

## 4 · ACCEPTANCE
- PG-1..PG-5 land CI-green with their Blocker tags observable in a deliberately-broken test scene.
- The Golden/profile build fails loudly on a destination outside its locked scenes (PG-2 proof).
- Next Terry headset session: zero recurrence of the four §0 classes; his notes produce new gate
  classes (ratchet working) rather than repeats (ratchet broken).
- EXCELLENCE_MAP gains the gate-coverage column; the next world shipped (W002 push) goes through
  the full per-world contract without a single manual exception.
