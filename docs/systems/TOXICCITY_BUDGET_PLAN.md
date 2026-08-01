# GETTING LEVEL 1 UNDER BUDGET — what to cut, in order

**Status:** analysis + ranked plan. ⚖ Terry, 2026-08-01: *"what's the best thing to cut in a way that
still makes everything functional and look good to keep under the budget and leave some room."*

---

## 1. The measurement, and the thing it actually says

From the audit artifact of CI run `30718215590` (the numbers below are **measured**, not estimated):

| Measure | ToxicCity | Target | Hard cap |
|---|---|---|---|
| Unique materials | **333** | 25 | **60** ← the only violated cap |
| Renderers | **1769** | 900 | 2500 |
| Static triangles | 159,944 | 150,000 | 400,000 |

**Only one budget is actually broken: materials, at 5.5× the hard cap.** Renderers and triangles are
over *target* but comfortably inside cap.

And here is the finding that changes the whole plan:

> **The material count is not a content problem. It is a duplication problem.**

> ⚠ **CORRECTION, 2026-08-01.** The first version of this section said fifteen files create a fresh
> material per painted object. **That was wrong and it was the exact mistake Terry warned against** —
> counting the thing incorrectly and then cutting on the strength of it. Reading each factory rather
> than counting `new Material(` call sites:
>
> - **Most authors DO cache, keyed by SLOT NAME** — `RingCityBuilder`, `ToxicCityStageB`,
>   `ToxicCityRiverBuilder`, `CityStageAPrimitiveFactory`, `ShipHullBuilder`. Each holds maybe 6–16
>   materials total. They were never the problem.
> - **Exactly two were genuinely uncached**, one material per painted object:
>   **`CityWayfindingAuthor`** (nineteen lanterns × three parts = 57 materials **for two colours**) and
>   **`QuayBerthAuthor`** (five pads × decks, bollards, plates and tally bars ≈ 35 **for four**).
> - **Four more cached by exact `Color` in private per-file dictionaries** — `CityBuilder`,
>   `WorldPoiBuilder`, `WorldDressingBuilder`, `ShipyardApproachAuthor` — so the same worn grey
>   authored in four files was four materials.
>
> Three of those five slot-keyed authors also already set `BatchingStatic`. **`CityBuilder`, which
> builds most of the city, set no static flags at all.**

So the duplication is real but narrower than first claimed: **two uncached authors plus four private
caches that could not see each other.** Whether that accounts for all 333 is not yet known — which is
why §4's measurement now ships alongside the fix rather than after it.

---

## 2. The plan, ranked by savings ÷ risk

### Tier 0 — free. No visual change. Nothing deleted. ✅ BUILT 2026-08-01

**T0.1 · One shared material cache for the whole bake.** — `PatchMaterials`

Keyed on what actually makes two materials different at draw time: **colour and emission**. The two
uncached authors now route through it, and the four private `Color` caches were replaced by it, so a
grey authored in `CityBuilder` and the same grey authored in `WorldPoiBuilder` are now one instance.

**Deliberately NOT quantizing yet.** The first draft of this plan proposed snapping colours to a ~24
tone ramp to guarantee the cap by construction. That is still available and still a good idea — but
quantizing is the only step here that *changes pixels*, and doing it in the same commit as pure
de-duplication would make it impossible to tell which mechanism produced the number. **Share first,
measure, then quantize only if the measurement says so.**

- **Risk:** none to the look — identical colours, identical shader, fewer instances.
- **Reversible:** each author's `Mat()` is a one-line shim.

**T0.2 · Static batching in `CityBuilder`.** ✅

`CityBuilder.Cube` now sets `BatchingStatic`, which is what the shared cache pays off into: everything
sharing a material collapses into one draw. Three other authors already did this.

⚠ Two things are explicitly **excluded**, and both would have been silent bugs:
- the crane **`HookRig`** (`CraneHookRuntime` drives it every frame), and
- the **`LooseCrate`** (it has a Rigidbody and the player picks it up).

A static-flagged moving object gets baked into a combined mesh and its transform stops mattering — the
hook would just stop creeping, and the grab tutorial would become scenery that ignores your hands.
Neither would throw, log, or fail a test.

**T0.2 · Mark non-moving city geometry `isStatic`.**

`WorldDressingBuilder` already does this and says why in a comment: *"static batching — hundreds of
props must not mean hundreds of draws."* `CityBuilder.Cube` does not, and it builds most of the city.

Static batching merges objects that share a material, so this is what T0.1 *pays off into*: once
everything grey is one material, the grey city becomes a handful of batches.

- ⚠ **Must exclude anything that moves.** In ToxicCity that is: the crane `HookRig` (`CraneHookRuntime`
  drives two transforms), the river `Flow_*` and `Bubble_*` ribbons, the relay fault strobe, and the
  travel-door labels. Marking a moving object static is a silent, confusing bug.
- **Honest caveat:** this does **not** move the audited renderer number — the audit counts renderers,
  not draw calls. It moves the thing the headset actually feels. Judge it on device, not in the report.

### Tier 1 — small content trims, only if Tier 0 leaves you wanting more room

**T1.1 · Cap the fake windows per facade.** `AddFakeDepthWindows` builds a grid of
`cols = width/1.4 × rows = height/1.6`, minus a 35% random skip — up to ~15 cubes **per facade**, and
there are roughly 60 facades across seven districts. **Estimated ~600 renderers, about a third of the
scene.** Capping at 6 per facade would save an estimated ~300 with almost no perceptual cost: windows
read as *a pattern*, and nobody counts them.

**T1.2 · Skyline `skylineCount` 26 → 14.** Background silhouettes on a 250 m ring. Saves 12 renderers —
small, but it is one number in the layout and costs nothing to try.

*(Both estimates are derived from loop bounds in the source, not measured. See §4.)*

### Tier 2 — the only question that is genuinely yours to answer

**T2.1 · Does Level 1 need `__RING_CITY`?**

The audit's own example paths show ToxicCity is building **three** whole subsystems on top of its seven
hand-authored districts:

- `__RING_CITY` — tower island, causeways with rails, wedge blocks, sea wall, stilt villages, tide
  pools, gate pillars (`RingCityBuilder`, opted in via the spec)
- `__TOXIC_RIVERS` — flow ribbons and bubbles
- `__FLATS_EXPEDITION_SITE` — the expedition outside the wall

`__FLATS_EXPEDITION_SITE` **stays**: artifact half B lives there, so it is route. The rivers are the
Canal Stalker fight. **The ring city is the open question** — it is the concentric city silhouette from
`CITY_VISUAL_SPEC §1`, and much of it is detail the player never gets near.

**The cut worth considering is not deleting it, it is demoting it to a silhouette:** keep the ring, the
sea wall and the gate pillars (they are the horizon and they carry the shape); drop the per-object
detail on stilt villages, causeway rails and wedge rows the player will never stand beside. Every ring
element already has its own switch, so this is layout, not code.

**This is the one that needs your eyes rather than my arithmetic** — it is a "what is this city
supposed to feel like" call, not a budget call.

---

## 3. Recommended order, and what it buys

| Step | Materials | Renderers | Risk |
|---|---|---|---|
| now | 333 | 1769 | — |
| **T0.1** quantizing cache | **~30** ✅ | 1769 | very low |
| **T0.2** static batching | ~30 | 1769 *(draw calls collapse)* | low, with the exclusion list |
| T1.1 window cap | ~30 | ~1450 | low |
| T2.1 ring demotion | ~30 | ~1100 ⟶ near target | **needs Terry** |

**Do T0.1 first and the violated budget is gone.** Everything after it is headroom, and headroom is
worth having but is not urgent.

---

## 4. The measurement — ✅ SHIPPED WITH THE FIX, NOT AFTER IT

The Tier 1 and Tier 2 numbers are **derived from loop bounds in the source**, not measured. Nobody
should delete content on the strength of them.

So `PerfBudgetAuditRules` now emits, every audit:

```
ZIPTIDE: PERF_BREAKDOWN scene=ToxicCity total=1769 roots= __TOXIC_CITY_ROOT=1502/301 …
```

— renderers and unique materials **per top-level root**, biggest first. It turns *"I think the ring
city is expensive"* into a number, and it will also show exactly how much of the 333 the shared cache
actually removed.

It is a `Debug.Log`, not a report finding, on purpose: the report has two severities, Warning and
Blocker, and a measuring tape is neither. Adding an `Info` severity would change a JSON schema other
tools read in order to carry something the build log already holds.

**Cutting content on an estimate is how a level gets uglier without getting faster.** That is the whole
reason this document leads with the one measurement we actually had — and why the next decision waits
for this one.

---

## 4b. RESULT of Tier 0 — measured, run `30721250486`

| | before | after T0.1 | cap |
|---|---|---|---|
| ToxicCity unique materials | 333 | **222** | 60 |

**111 materials gone — a third — and it is still 3.7× the cap.** That is the answer to the question
this document was written to ask, and it is not the answer the first draft assumed.

Two things follow, and both matter more than the number:

1. **De-duplication alone does not clear this budget.** The plan's original framing — *"nothing needs
   to be cut to fix the only broken budget"* — was **wrong**. Something more has to happen.
2. **Splitting the fix was the right call.** Had quantizing shipped in the same commit, 222 would have
   been indistinguishable from "quantizing works, ship it" and nobody would know that the remaining
   162-over-cap has a different cause. Now we know sharing was worth 111 and the rest is elsewhere.

**Where the remaining 222 live is not yet known**, and that is the next thing to find out rather than
guess. Candidates, in the order worth checking:
- the five **slot-keyed** authors, which do NOT route through `PatchMaterials` (`RingCityBuilder`,
  `ToxicCityStageB`, `ToxicCityRiverBuilder`, `ShipHullBuilder`, `CityStageAPrimitiveFactory`);
- **`BuildingBuilder`/`InteriorFurnisher`**, which paint from per-building `BuildingStyleDefinition`
  colours — genuinely different colours per style, which quantizing WOULD collapse;
- district **`paletteOverride`s** — seven districts × ten palette fields is up to seventy colours
  before anything else is built.

`PERF_BREAKDOWN` now writes into `AUDIT_REPORT.md` (not just the log) so the next run answers this.

---

## 5. Next, once the breakdown lands

1. Read `PERF_BREAKDOWN` from the CI job log and record the real per-root split here.
2. If materials are under 60 with room — **stop.** Nothing else in this document is urgent.
3. If not, quantize (T0.1's second half) before touching any content.
4. Only then look at the window cap, and only then ask Terry about `__RING_CITY`.
