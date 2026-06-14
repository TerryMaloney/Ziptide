# Prompt-to-World Workflow

**The dream:** Terry says *"red glass building on the right, short white stone tower on the
left, alien glyphs"* and it gets built — safely, on-budget, without breaking the game.

This doc defines how that works so it's structured and auditable, not freestyle scene editing.

Last updated: 2026-06-14

---

## The flow

```
1. Terry describes a scene in natural language.
2. Claude converts it to a structured ArtBuildPlan (JSON/ScriptableObject) — NOT direct edits.
3. ArtBuildPlan is validated: approved kits only, within perf budget, route stays walkable.
4. A scene patcher places approved kit objects from the plan (idempotent).
5. Art audit checks: route, materials, lights, colliders, transparency, object counts.
6. Build + on-device smoke confirms no runtime damage.
```

**Claude does NOT freestyle-edit the scene.** It emits a plan; the patcher + audit are the
guardrails. This is the rule that keeps "fast art" from re-breaking the game.

---

## ArtBuildPlan schema (per placed object)

| Field | Example | Purpose |
|---|---|---|
| `objectType` | building / tower / canal / bridge / decal | what it is |
| `kitId` | `ToxicVenice` | which approved kit supplies the asset |
| `surfaceFamily` | `UpperClassGlass` | material family (resolved via registry) |
| `placementZone` | `right`, `left`, `far`, `route` | where, relative to spawn/route |
| `role` | walkable / decorative / blocker | drives collider mode + audit |
| `colliderMode` | none / convex / mesh | physics |
| `decalSet` | `OligarchGlyphs` | symbols/glyphs to apply |
| `lightingContribution` | none / emissive-trim / probe | perf-aware lighting |
| `perfCost` | small / medium / large | budget accounting |
| `auditExpectations` | "route clear", "≤ N materials" | what audit must confirm |

A plan is a list of these + a header (`worldId`, `kitId`, target perf tier).

## Example

Prompt: *"I want a red building on the right made mostly of glass with cool alien symbols.
On the left I want a short tower made of white stone."*

→ ArtBuildPlan:
```
worldId: W001, kitId: ToxicVenice
- objectType: building, surfaceFamily: UpperClassGlass, tint: red, placementZone: right,
  role: blocker, colliderMode: convex, decalSet: OligarchGlyphs, perfCost: medium
- objectType: tower, surfaceFamily: CeremonialStone, tint: white, placementZone: left,
  role: blocker, colliderMode: convex, height: short, perfCost: small
```

## Validation gates (plan rejected if any fail)

- references an unknown `kitId` / `surfaceFamily` → `MISSING_ART_KIT`
- tries to add/modify a boot-owned runtime system → `WORLD_CONTRACT_FAIL`
- would block the player route between spawn and exits → audit blocker
- exceeds material / light / transparency / object-count budget → `PERF_BUDGET_FAIL`

## Phasing (build the pipeline before relying on it)

- **D4.3** — primitive kit only (cubes/quads tinted per surface family). Proves placement +
  audit work without heavy assets.
- **D4.4** — first real prompt → ArtBuildPlan → patcher → audit round-trip on W001.
- **D4.5** — swap primitives for imported meshes/materials behind the same IDs (LODs, texture
  budgets) — no gameplay change.
