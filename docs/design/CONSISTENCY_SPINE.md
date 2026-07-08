# CONSISTENCY SPINE — the shared systems every feature rides (design doc)

> **Hardwiring §13 / Phase 0.** The answer to "make it all consistent across the game." These shared
> layers are built ONCE and every later system snaps onto them. Status: skeleton for Fable 5 — expand
> freely, but do not fork these into per-feature one-offs.

## Why
Ziptide already has the right bones (the Forge, `ArtModuleRegistry`, string-id Definitions,
`TravelCoordinator`, `ProfileEconomy`). The hardwiring adds a lot of new systems (ships, vehicles,
interiors, automation, cosmetics). Without shared spines they will drift into inconsistent one-offs.
This doc defines the shared layers so they don't.

## The spines (build/verify each)

### A · One cosmetic layer — `WrapDefinition` / `SkinDefinition`  *(looks-never-stats)*
- **Schema:** `CosmeticDefinition : Definition { targetKind (Ship|Vehicle|Weapon|Avatar|Building);
  targetId or family; material/palette/decal/emissive overrides; unlockRule; previewIcon }`.
- **Apply seam:** a `CosmeticApplier` that layers overrides onto a base Forge-baked asset at spawn,
  changing *only* look — never stats (locked law). Registry-driven; primitive fallback if unfulfilled.
- **Wardrobe:** one shared UI surfaced in the home hub; selections persist in `PlayerProfile`.
- **Verify:** apply/remove a wrap on a ship and a weapon through the same code path; stats identical.

### B · One movement + comfort layer
- **Input map:** a single shared action set + a `ComfortSettings` profile (vignette strength,
  snap vs smooth, flight-assist on/off, deadzone/response curves, seated).
- **Reference-frame helper:** a `StabilizedRig`/cockpit-frame utility so character, ship, and vehicle
  all move the *world/vehicle* relative to a fixed rig — **never parent the XR rig to a moving hull**
  (locked contract). Dynamic comfort vignette on acceleration/roll.
- **Seat/mount:** one enter/exit + seat-anchor system shared by ship boarding, vehicle driving, turrets.
- **Verify:** ride a ship and a vehicle through the same comfort layer; vignette + presets behave identically.

### C · One content-by-id discipline
- Every new thing is a `Definition` ScriptableObject resolved by string id through a factory/registry —
  no runtime reflection (locked). New ship/vehicle/plant/building/cosmetic = a new asset, not new code.

### D · One world contract
- Ground, cavern, elevated, and space are all **content-only world scenes** reached by
  `TravelCoordinator.TravelTo` with a `SpawnMarkerRuntime`; `_Boot` owns the rig; fade via
  `ZiptideTransitionEffect`. Reuse — do not invent a second scene-loading path.

### E · One economy + save spine
- `PlayerProfile` holds gardens, factories, ships, vehicles, cosmetics, and story flags;
  `ProfileEconomy` resolves offline; `SaveSystem` persists (wire it in Phase 0).

### F · One audit + budget discipline
- Each new content type ships an `*AuditRules` entry + a `PerfBudget` cap. CI is the safety net.

## Tests
- Cosmetic apply/remove leaves stats invariant (golden test).
- Comfort layer: same presets drive character/ship/vehicle (shared-path test).
- Save round-trips every new profile sub-state.

## 🚀 Room to expand
Cosmetic *sets*/themes that span ship+vehicle+weapon+avatar (a matching "livery"); earned vs bought
unlock rules; a comfort auto-tuner from motion telemetry; a shared "loadout" abstraction over both
functional modules and cosmetics. Propose more — keep looks-never-stats and the rig contract intact.
