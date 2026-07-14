# ZIPTIDE Canonical Owner Decisions

**Status:** R0 proposed decisions  
**Machine source:** [`canonical_owner_decisions.json`](canonical_owner_decisions.json)

## Decisions now locked for recovery

- `TravelCoordinator` remains the sole scene-travel owner. The generated scan found no second runtime loader outside its own synchronous fallback.
- `PlayerRigPersistence` becomes the sole persistent rig and input-session owner. `RuntimeInputEnabler` is a legacy global fallback to retire or explicitly gate after R1 proves the replacement.
- `SaveSystem` remains the sole profile/disk owner.
- `BootLoader` coordinates the cold-boot state; `PlayerRigPersistence` owns the physical boot hold.
- `DevWarpBoard` remains the one temporary development presentation until the common panel framework replaces it. Y+B is retired; `QuickSwap` still encodes the stale chord and must be migrated.
- All player-facing runtime UI migrates to one `DiegeticPanel` contract. Home Hub, travel doors, Quarters, Conquest, objective boards and diagnostics may keep their feature data/callbacks, not their independent layout/facing/input infrastructure.
- The always-on `CreditsHud` and default-on `DebugHUD` do not belong in the GoldenSlice view.
- One central input-contract registry owns button meanings by active mode. Feature scripts no longer create overlapping button assumptions independently.
- One camera-role contract replaces `EnsureXRCameraActive` name-based disabling.
- Build-time material validation plus an explicit development fallback policy replaces `RuntimeMaterialFixer`.
- One scene-presentation coordinator composes sky, grade, fog and key/practical lighting. Legacy sky and newer vista paths cannot both claim the scene implicitly.
- One item-presentation schema owns physical envelope, hand pose, holster pose, muzzle and impact axes. The universal 45-degree fallback is retired.
- One shared shooter/self-collision policy applies to hitscan and projectiles.
- Creature grounding requires authoring, collider/contact and post-settle proof. A blob shadow is presentation, not grounding evidence.
- `AudioDirector` becomes the audio composition owner; `AmbienceDirector` remains a subordinate biome provider.
- Diagnostic systems run only through an explicit Diagnostic profile.
- Multiplayer, Quarters, Tidefront and other parked features are controlled by the recovery exposure gate, including their automatic bootstraps and scene hooks.

## Still unresolved

### Job/repair/objective continuity

The semantic owner is not selected yet. Existing source splits the flow among machine interaction, job director/runtime, resource bank, objective board and ship cast-off observation. The event/save graph must identify which component already owns the durable state transition before R2 creates or selects the canonical coordinator.

### Melee implementation

The canonical contract is decided, but the surviving implementation is not. `SonicThumper` and `PvpHammer/HammerTool` must be resolved to exact source paths and compared before one is migrated and the other is hidden or removed from runtime.

### Ship visual root

The contract is one ship presentation root assembled from loadout data. The source resolver must still identify every hull/refit root and which scenes instantiate each path before selecting the surviving assembler.

## Why this matters

The project’s earlier architecture separated assemblies and pure logic well, but multiple translators still claimed the same player-facing responsibility. These decisions prevent R1/R2 from adding another layer beside the old ones. Every recovery implementation packet must name the owner it preserves and the owner it displaces.
