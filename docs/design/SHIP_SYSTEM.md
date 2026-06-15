# ZIPTIDE — Ship System

The player walks a ship's interior, sits, and flies it first-person (looking out the windows); it's
visible from outside; it's also the backbone of (later, cosmetic-only) monetization. `Ziptide.Ship`
is currently an EMPTY assembly — this is greenfield. We build ONE ship well via a repeatable
pipeline, then scale to many. Builds on `CONTROLS_AND_FLIGHT.md` + `SYSTEMS_ARCHITECTURE.md`.

Decisions (Terry, 2026-06-15): **one seamless model** (interior + exterior are the same object);
**first-person cockpit** (look out the windows; cockpit = stable VR reference frame);
**least-expensive-but-great** content pipeline; **cosmetic-only** monetization, **store later**,
never pay-to-win (Fortnite model).

---

## Architecture — the seamless ship
- **One model, two zones:** a single ship hierarchy with a **walkable interior** (cockpit + small
  cabin) and an **exterior hull + windows**. Modest crew-ship scale so Quest can handle it.
- **States:**
  - *Aboard/walkable* — walk the interior with normal XR locomotion, docked or drifting.
  - *Flight* — sit in the pilot **seat** → flight mode: the **XR rig parents to the cockpit seat
    anchor** (cockpit-locked = comfortable) and a `FlightController` moves the WHOLE ship through a
    space scene. You look out the real windows. Stand → back to walkable.
- **Space environment:** flying loads a lightweight **space scene** (skybox/vista + objectives)
  around the persistent ship via a TravelCoordinator-style transition. Ship is the constant; the
  world streams around it.
- **Comfort:** cockpit-locked frame, optional vignette on hard maneuvers, auto-level assist for
  first-time flyers.

## Data model (reuse Definition + Init pattern + registries)
- `ShipDefinition`: id, blueprint ref, **stats** (speed/handling/boost/health), **cockpit seat
  anchor**, interior layout ref, exterior LODs, **cosmetic style**, **gating** (abilityRequired),
  `isCosmeticPaid` + price (store later).
- `ShipBlueprint`: the modular-kit **assembly recipe** (hull/cockpit/wings/engine/greeble parts +
  proportions + paint/decals) matching the AI concept — editable data.
- `ShipRegistry` (mirrors `ItemFactory` cache), in the `Ziptide.Ship` assembly.

## "AI image → playable ship" pipeline (least-expensive, scalable, Quest-friendly)
**Chosen: modular kit matched to the concept** — cheapest at scale, reuses parts, looks good with
greebling/paint, repeatable (same idea as our prompt-to-world ArtBuildPlan).
1. Generate the **AI exterior concept image**.
2. Author a **`ShipBlueprint`**: choose kit parts + proportions + paint/decals/greebles to match the
   concept silhouette (where each ship gets its unique style).
3. **`ShipAssembler`** (editor tool) builds the exterior from the kit, fits the **interior** from a
   room template, places the **seat anchor + windows**, generates **LODs** + collision.
4. **Ship audit** (extends WorldAuditRunner): perf budget (tris/materials/draw calls), seat present,
   interior walkable + collidered, windows, exterior LODs, no gameplay scripts on cosmetics, and the
   no-pay-to-win check (a paid ship may not out-stat an earnable one).
5. Blueprint stays **editable data** → tweak in-editor.
> Optional later: a hero ship can use AI image-to-3D (Meshy/Luma) + retopo for Quest. The kit is the
> default because it's cheap, consistent, and scales.

Build ONE ship end-to-end, lock the pipeline, then new ships = new blueprints reusing the kit.

## Monetization / cosmetics (design now, store later, never pay-to-win)
- `PlayerProfile` gains `ownedShips[]`, `ownedOutfits[]`, `equippedShip`, `equippedOutfit`.
- Gameplay ships are **earned by ability/progression**. Paid items are **cosmetic only** — alternate
  STYLES/outfits with **identical performance** (Fortnite model). `CosmeticDefinition` + a **stubbed
  `StoreService`** now; real Quest IAP is a later **online** milestone (needs the backend).

## Quest performance plan (critical for a seamless model)
Only **one ship active**; interior + exterior **LODs**; **occlusion** (cull interior detail in
external focus and vice versa); **baked lighting**; modest poly/material budget; **instanced
greebles**; pooled VFX. Per-ship perf budget enforced by the ship audit.

## Build order (CI-gated; after the on-foot slice + CI green)
1. `ShipDefinition` + `ShipBlueprint` + `ShipRegistry` + profile ownership fields (data only; EditMode test).
2. Modular **ship kit** (parts library) + `ShipAssembler` editor tool + ship audit.
3. Build **ONE ship** end-to-end (blueprint → seamless model → walkable interior → seat + windows).
4. `SeatInteractor` + flight-mode transition + `FlightController` (arcade flight, cockpit-locked) — ties to CONTROLS_AND_FLIGHT.
5. Simple **space scene** + the short **flight-tutorial hop** (the teaser before the first ziptide).
6. Cosmetic/ownership layer in profile (`CosmeticDefinition`, `StoreService` stub).
7. A **second ship** to prove the pipeline scales.

## Critical files (to create on execution)
- `Ziptide/Assets/Ziptide/Ship/Runtime/`: `ShipDefinition.cs`, `ShipBlueprint.cs`, `ShipRegistry.cs`,
  `FlightController.cs`, `SeatInteractor.cs`, `ShipModeService.cs`.
- `Ziptide/Assets/Ziptide/Ship/Editor/`: `ShipAssembler.cs`, ship audit checks.
- Extend `PlayerProfile` (SaveSystem) with ship/outfit ownership.
- Reuse: `TravelCoordinator` (space-scene transition), `WorldAuditRunner` (ship audit), the
  Definition+Init pattern, `CONTROLS_AND_FLIGHT.md` mappings.

## Verification (on device, when built)
Board ship → walk interior; sit → flight mode (comfortable, look out windows); fly the tutorial hop;
stand → walkable. Tags: `SHIP_BOARD`, `FLIGHT_START/END`. Ship audit green; perf within budget on
Quest. A second ship assembles from a new blueprint with no code changes.
