# HARVEST 001 — the ToxicCity ring city and the SLV-01 Scrapper

**Stage:** 5 — The Assembly Line  
**Type:** ledger  
**Protocol:** [`docs/THE_RATCHET.md`](../THE_RATCHET.md) · **Status:** 🟡 open — R4 partial, awaiting device evidence

The first harvest, and the reference shape for every one after it. Source: an uncommitted Unity AI
Assistant pass on `claude/unity6-migration`, 2026-08-02, audited 2026-08-05.

---

## R1 · FREEZE

| Fact | Value |
|---|---|
| Packages | `com.unity.ai.assistant 2.17.0-pre.1`, `com.unity.ai.inference 2.6.1` |
| Diff | 10 files, **459,136 insertions / 16,143 deletions** |
| Of which | **474,540 lines are `ToxicCity.unity` alone** |
| Scene objects | **+3,239 GameObjects**, −495 · +2,814 MeshRenderers · **+1,637 embedded Materials** · +1,203 BoxColliders |
| The AI's own plan | `Ziptide/Assets/Plans/toxic-city-makeover.md` — committed verbatim in `1e5d1c8c` |
| Reference art | 5 concept plates (Assistant cache, not kept) + 4 ortho turnaround sheets kept at `Assets/Ziptide/_ArtGenRefs/ship_scavenger_mk1/` |

Every generated object count matches a serialized field in `RingCityDef` — 3 crown cranes
(`crownCraneCount: 3`), 5 wedges (`wedgeCount: 5`), 4 causeways, 6 beached wrecks, 9 tide pools.
**That is the proof it was a deterministic re-bake and not hand-authoring**, and it is why the scene
could be safely held out of the commit: it regenerates from the layout asset.

---

## R2 · SORT

### ✅ KEEP — committed in `1e5d1c8c`

| What | Why it's good |
|---|---|
| `ToxicCityLayout` — `buildTowerIsland`/`buildWedges` → true, 5 × `buildingStyleId` | Two booleans and five strings produced 3,239 objects. `RingCityDef` could already express all of it; nobody had turned it on. |
| `ToxicCity_Theme` + `ToxicCity_WorldProfile` | Level 1's first `VisualThemeProfile`. Closes precisely the gap rb137 named: ToxicCity was the only scene with no theme of its own, so its authored sky had never rendered. |
| `ShipHullBuilder` → SLV-01 "Scrapper" | Replaced a generic winged fighter with a hull measured off `SHIP_SCAVENGER_VISUAL_SPEC.md`. Source of T-002, T-003, T-004, T-006. |
| `ShipRefit` remap + `ApplyClawStow` | Chassis knobs remapped onto the new silhouette; claw swings as a rigid group. |
| `HeroShipHullBuilderTests` identity guards | **The crown jewel — see T-001.** |
| `ToxicCity_WorldPack` — `signal_relay` → `(-26, 0, 8)` | Exactly the CanalRow district anchor. Precise, not approximate. |

### 🔴 REVERT — fixed in `2cafb483`

**`ToxicCityExit_WorldPack.sceneName`: `W000_DriftIn` → `MilestoneA_GrabCube`.** The "Leave" door
repointed from the drift-in world to a grab-cube test scene, during a bake that had nothing to do
with travel.

Checked before reverting whether it was *repairing* a dead reference — it was not:
`Scenes/Generated/W000_DriftIn.unity` exists and is build index 7 in `EditorBuildSettings.asset`.
Both readings were plausible; two minutes of checking decided it. **Do this check every time.**

### 🟠 FIX — `2cafb483`, plus one unrelated find

- `tools/dev_build_install.ps1` and `tools/level1_test.ps1` defaulted `$UnityExe` to `2022.3.62f3`
  while the project moved to `6000.2.9f1` — **every documented build command was aimed at the wrong
  editor.** Both now resolve from `ProjectVersion.txt` and fail loudly listing installed editors.
- `Assets/AI Toolkit/` (~10 MB Assistant image cache) and `Assets/_Recovery/` (a crash dump) were
  untracked but *not ignored*; `git add -A` would have committed both. Now ignored.
- **Unrelated but blocking, fixed in `3b67aee9`:** Fast Preflight had been red on every commit of
  this branch. `first_hour_binding_gate --strict` was failing because Unity 6's API Updater
  fully-qualified the XRI type names in `HolsterSocketInteractor.cs` and the doc contract still
  expected the short form. The binding was intact; the contract was stale.

### ⚠️ HELD — Terry's call, deliberately not auto-applied

- **`DynamicsManager.asset` `m_AutoSyncTransforms: 0 → 1`** — a live physics behaviour change,
  bundled invisibly into the Unity 6 schema migration. It was deliberately off. See A-002.
- **~1,464 inlined materials in `ToxicCity.unity`** — see A-001. Scene held out of git until the
  device evidence is in.
- **`ToxicCity_WorldProfile` spawn `(0, 0, 0)`, `playAreaSize` 4×4 default** — see A-003.

### NOISE — named, per the R2 rule

`kind: 0` on two Plaza landmarks (`LandmarkKind` enum default written out on re-serialize) ·
`beltFloors: []` field-order re-serialization in both WorldPacks · `SceneTemplateSettings.json`
Unity 6 schema shrink · `com.unity.ai.assistant/Settings.json` `CheckpointEnabled` flip (the AI's
git-backed undo was switched on mid-session) · `Assets/Ziptide/Generated/WorldImprovement/Materials/`
— **not AI output at all**, that is our own `WorldImprovementModules.cs` cache regenerating.

---

## R3 · HARVEST

### Techniques

| ID | Technique | Why it generalises | Carrier |
|---|---|---|---|
| **T-001** | **Prose → assertion.** Art direction compiled into EditMode assertions whose *failure message is the sentence itself*. | This is LAW 3 ("a gate per quality dimension") applied to **aesthetics**, which we had never done. Taste usually can't be tested; three of these can. | **C2** ✅ live |
| **T-002** | **Spec-fraction authoring.** `float Z(float f) => l * (0.5f - f);` — every part placed as a fraction of total length (nose 0.00 → nozzle 1.00). | Rescaling can never break proportion, and the builder reads as the spec document. Applies to any measured asset. | **C1** |
| **T-003** | **Endpoint segments.** `Segment(a, b, thickness)` deriving rotation via `Quaternion.FromToRotation`, instead of hand-written Eulers. | Its own comment says it best: *"sign errors in a hand-written rotation are exactly how a leg ends up bending the wrong way."* Mirrored sides become provably correct. | **C1** |
| **T-004** | **Identity-vs-theme blend + canon marks.** `Color.Lerp(identity, themed, 0.25f)`, plus a skip-list of marks a livery may never repaint. | Lets a world theme tint everything *without* dissolving the things that make an asset recognisable. Directly reusable for creatures, vehicles, props. | **C1 + C2** |
| **T-005** | **Data-first re-bake.** Flip ScriptableObject fields, invoke the named menu command, never hand-author geometry. | The AI **independently rediscovered LAW 1** with no access to our docs. That is the strongest evidence we have that spec-is-truth is right. | **C3** (already law) |
| **T-006** | **Rigid-group transforms.** Rotate an articulated group about one pivot; per-segment rotation pulls the joints apart. | Applies to every jointed thing we will ever build — cranes, claws, legs, creature limbs. | **C1** |
| **T-007** | **The plan-file schema.** *Key Asset & Context* path manifest → steps tagged role/dependencies/parallelizable → verification split visual vs. automated. | The best brief format we have seen for this tool. Adopt it as the shape of our briefs **to** the AI. | **C3** |

**T-001 in the wild** — the assertion messages, verbatim from `HeroShipHullBuilderTests.cs`:

> `part.name + " must stay on the port flank — the claw is never mirrored."`
> `leg + " must plant outboard of the hull, not tucked under it."`
> `"The truck cab sits ON TOP of the hull line."`

Each is a sentence Terry could have said in the headset. Each is now a red test if broken.

### Anti-techniques

| ID | What went wrong | Gate that would catch it |
|---|---|---|
| **A-001** | Re-bake **inlined ~1,464 material instances into the scene file** instead of sharing assets — the entire 474k-line diff. rb137 had just measured 333 materials against a **hard cap of 60** and got it to 111; this reopens it. | Material-count/perf audit run on the baked scene, BLOCK at the cap |
| **A-002** | **Cross-domain edits rode in silently on a content bake** — a travel `sceneName` and a global physics setting, neither related to building a city. | R2 SORT catches it by hand today; a WorldPack `sceneName`-validity gate + a ProjectSettings-diff review would catch it in CI |
| **A-003** | **Half-authored assets presented as done** — a `WorldProfile` with spawn `(0,0,0)` and the default 4×4 play area, in a ring city where the origin is probably inside geometry. | Authoring-completeness check: a profile whose spawn is untouched default is not authored |

---

## R4 · ENCODE — status

| ID | Carrier | State |
|---|---|---|
| T-001 | C2 | ✅ **CLOSED** — live in `HeroShipHullBuilderTests.cs`, committed `1e5d1c8c` |
| T-005 | C3 | ✅ **CLOSED** — house rules §data-first |
| T-007 | C3 | ✅ **CLOSED** — house rules §how to plan |
| A-001 A-002 A-003 | C3 | ✅ **CLOSED as rules** — house rules; ⛔ **OPEN as gates** (C2) |
| T-002 T-003 T-004 T-006 | C1 | ⛔ **OPEN** — currently live only inside `ShipHullBuilder`. They must be lifted into a shared authoring core before another builder can use them. |

**This harvest is OPEN.** Two named board rows close it:

1. **Lift T-002/T-003/T-004/T-006 into a shared authoring core** (C1) — a `MeasuredPartCore` /
   `SegmentAuthoringCore` pair with EditMode tests, so `RingCityBuilder`, `BuildingBuilder` and the
   creature builders get them too. Until this lands, four of seven techniques are trapped in one file.
2. **Build the three gates for A-001/A-002/A-003** (C2) — the material cap is the urgent one.

---

## R5 · RE-BAKE LEDGER

| Technique | Worlds that get it free on re-bake | Worlds needing authored work |
|---|---|---|
| T-002 T-003 T-006 (once in a shared core) | every world using the shared builders — W000–W012, arenas, all ship variants | none |
| T-004 | every themed world, once the blend moves into the material author | none |
| A-001 gate | all — it is a check, not content | none |
| T-001 | — | **per asset**: each hero asset needs its own identity guards written. Budget: ~3 assertions per asset, ~20 min each. |
| T-007 | — | per brief, free |

**Cost of "go back and improve the early levels" from this harvest: ~zero for six of seven
techniques, once step 1 above lands.** Only T-001 is paid per asset — and it is paid on hero assets
only, which is a short list. This is what THE RATCHET means by *improve the builder and re-bake*.

---

## What this harvest changed about how we work

1. **`docs/` is invisible to Unity AI.** The single most important finding, and it is not about
   ToxicCity at all. Every rule this project has ever written for operators was unreadable by the
   one operator that can open a scene. `Assets/Plans/_ZIPTIDE_HOUSE_RULES.md` exists because of this
   harvest.
2. **Unity AI reaches for the builder first.** Unprompted, with no access to LAW 1, it chose to flip
   data and call a menu command. Our architecture and this tool want the same thing — so the highest
   leverage we have over its output is *the quality of our builders*, not the quality of our prompts.
3. **It edits outside its brief without saying so.** Two of three problems here were cross-domain
   edits riding on a content bake. Review the whole diff, never just the files you expected.
