# QUARTERS — the customization room (locker/cosmetics), game-wide from day one

**Vision (Terry, 2026-07-02):** a room on the ship — "your quarters" — where you keep and pick your
modifications: weapon skins, ship livery, trails/emblems (the item-shop/locker feel). It must work
**throughout the whole game**: story worlds, the ship, and the other modes — e.g. a PvP pre-round
locker where you start each round. Built stubbed (no items authored yet) but with the REAL
architecture, so stocking it is pure data.

## The three layers (each independently useful)
1. **Data — `CosmeticDefinition`** (`Content/Runtime/Definitions/`): id, displayName, kind
   (WeaponSkin / ShipLivery / Trail / Emblem), `targetItemId` (weapon skins), the LOOK (body/accent
   color, scale mult — **a look, never a stat**, so cosmetics are legal in every mode incl. PvP), and
   `ownedFlag` (ownership = a profile flag → story rewards, bounty milestones, or a future shop all
   just set flags). Assets live under **`Resources/Cosmetics/`**. **The first drop is authored** (`CosmeticAuthor`,
   create-only, build-wired): Rustline (free) · Tidebreak (W002) · Ember Coil (W004) · Voidglass
   (the W012 capstone) · Wake Guild livery + First Contract emblem (first bounty). Add drops there.
2. **Pure state — `CosmeticLocker`** (`Core/Runtime/`, tested): THE one source of truth for "what's
   equipped on what". Equip state = profile flags with a reserved prefix (`COSM_EQUIP:<target>=<id>`),
   so it **saves, travels, and crosses modes for free** — no new persistence. One equip per target;
   `IsLockerFlag` lets flag-scanners skip locker noise.
3. **Presentation — `QuartersRoom`** (`Gameplay/Runtime/World/`): a self-building enclosed cabin
   (floor/walls/doorway/warm trim) with three display bays — WEAPON SKINS · SHIP LIVERY · TRAILS &
   EMBLEMS — each browsing owned cosmetics of its kind and equipping through the locker; a locker
   board shows what you're wearing. **Empty stock shows the stub:** "NO ITEMS AVAILABLE — check back
   after the next supply drop." **HOST-AGNOSTIC by design:** zero ship references — any mode can
   spawn one and get identical behavior because state lives in the profile, not the room.

## Application seam (live already)
`ItemFactory.Create` asks the locker for the equipped skin of every weapon it builds and tints it
(`ZIPTIDE: COSMETIC_APPLIED`). Zero cosmetics → no-op. Ship livery/trails hook into their systems the
same way when authored (the livery consumer is `CityBuilder`'s hull tint at regen; trails land with
M6 VFX).

## Where it lives today
The ship hosts one: cockpit deck → **QUARTERS** panel → teleport into the cabin aft (`QUARTERS_ENTER`),
**RETURN TO DECK** inside. Built by `ShipBoardingStation.BuildQuarters` on every berthed hull.

## 🤝 ARCHITECT CROSSOVER — the PvP pre-round locker (noted, not built)
Terry's call: before a PvP round starts, you could spend the pre-round timer **in your locker**.
Ownership split so we never collide:
- **Mine (story/ship lane):** `CosmeticDefinition`, `CosmeticLocker`, `QuartersRoom`, the ItemFactory
  seam — all shipped and tested. The room is spawnable anywhere: `new GameObject("Quarters")
  .AddComponent<QuartersRoom>()` at any anchor.
- **Architect (PvP/multiplayer lane):** WHEN/WHERE it appears in a match — spawn a `QuartersRoom` in
  the arena's spawn area, gate the exit on the round timer, teleport both players out on round start,
  and (Phase 4) sync each player's equipped-cosmetic flags in the match handshake so the opponent SEES
  your skin (read `CosmeticLocker.GetEquipped` per weapon on the remote profile snapshot — it's just
  strings). Nothing in the locker API needs to change for netcode; it was built string-pure for this.

## Playbook rows
| I want to… | Edit |
|---|---|
| Author the first cosmetics drop | a `CosmeticAuthor` (create-only, like `CreatureVariantAuthor`) writing `CosmeticDefinition`s into `Resources/Cosmetics/` |
| Change what a skin looks like | its asset's bodyColor/accentColor/scaleMultiplier |
| Gate a cosmetic behind story/a bounty | its `ownedFlag` (any `ZiptideFlags` or job completion flag) |
| Change the room's layout | `QuartersRoom` constants (RoomW/RoomD/WallH, bay positions) |
| Put a Quarters anywhere else | `AddComponent<QuartersRoom>()` at an anchor + your own entry/exit teleports |

*Verify tags: `QUARTERS_ENTER/EXIT` · `QUARTERS_BROWSE kind=… owned=…` · `COSMETIC_EQUIPPED id=… target=…` ·
`COSMETIC_APPLIED id=… item=…`. Tests: `CosmeticLockerTests` (6).*
