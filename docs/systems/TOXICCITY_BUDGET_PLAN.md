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

Every author paints with `new Material(shader)` and most of them **do not cache**. Fifteen files under
`Editor/Patching` create a fresh material per painted object:
`CityWayfindingAuthor`, `QuayBerthAuthor`, `ToxicCityRiverBuilder`, `ToxicCityStageB`,
`RingCityBuilder`, `ShipHullBuilder`, `PracticalAuthor`, `SignRecipeLibrary`,
`CityStageAPrimitiveFactory`, and others. Nineteen lanterns × three parts is 57 materials **for two
colours.** Five berth pads with decks, bollards, plates and tally bars is another forty **for four
colours.**

`CityBuilder`, `WorldPoiBuilder` and `WorldDressingBuilder` *do* cache — by exact `Color`, each in its
own private dictionary, so the same grey authored in three files is still three materials.

**ToxicCity does not use 333 colours. It probably uses fewer than 50.** Nothing needs to be cut to fix
the only broken budget.

---

## 2. The plan, ranked by savings ÷ risk

### Tier 0 — free. No visual change. Fixes the violated cap outright.

**T0.1 · One shared, quantizing material cache.** *(the whole recommendation, really)*

Replace every `new Material(...)` in the patchers with one `CityMaterials.Get(color)` that:
1. **snaps the colour to a canonical ramp** before lookup — these are near-identical desaturated
   industrial greys and browns; quantising to ~24 tones is invisible at arm's length and impossible to
   see at 30 m; then
2. returns a shared instance from a single cache.

Result: **333 → ~30 materials**, under the hard cap of 60 **with 50% headroom**, and the headroom is
*structural* — a quantizing cache cannot drift past its ramp size no matter how much content is added
later. That last property is worth more than the number: it means this budget never has to be fought
again.

- **Risk:** very low. Same shader, same shading model, colours move by a few percent.
- **Effort:** mechanical, ~15 files, one pure core (`ColorRamp.Snap`) plus tests.
- **Reversible:** entirely — the ramp size is one constant.

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

## 4. What we should measure before cutting further

The Tier 1 and Tier 2 numbers are **derived from loop bounds in the source**, not measured. Before
anyone deletes content on the strength of them, `PerfBudgetAuditRules` should report **renderers and
materials per top-level root** (`District_*`, `__RING_CITY`, `__TOXIC_RIVERS`, `__QUAY_BERTHS`,
`__SHIPYARD_APPROACH`, …).

That is a small addition to an audit rule that already walks every renderer, and it turns "I think the
ring city is expensive" into a number. **Cutting content on an estimate is how a level gets uglier
without getting faster** — the whole reason this document leads with the one measurement we actually
have.
