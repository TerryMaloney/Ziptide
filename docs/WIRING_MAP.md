# ZIPTIDE — THE WIRING MAP (authoritative)

**Read this before changing anything that spans data → runtime.** It answers one question for every
system: *how is it wired, and how do I prove BOTH sides are connected?* Three models built in parallel;
the failure class this map kills is **"wired on one side but not the other"** (an authored library not
build-hooked, a string ID with no asset, a produced mesh with no consumer, a transport with no caller).

- **Status legend:** ✅ wired both sides (verified) · ⚠ one-sided gap (see `WIRING_AUDIT_FINDINGS.md`) ·
  🔵 intentional stub/envelope (deliberate, documented).
- **Companion docs:** `WIRING_AUDIT_FINDINGS.md` (the per-seam ledger + gaps) · `HOW_TO_CHANGE_ANYTHING.md`
  (the edit→then→verify table) · `BOARD_INDEX.md` (which board is the truth) · `00_LOCKED_CONTRACTS.md`
  (the rules you cannot break). This supersedes the *system-map* section of `CONNECTIONS_AND_RECOVERY.md`
  (that doc's root-cause table is still valid history).
- **Audited:** 2026-07-06 (Picasso/Fable 5), against `terry-local-wip`.

---

## Part 1 — The runtime spine (boot / rig / travel)

```
_Boot scene (build index 0, persistent — owns ALL runtime; NEVER a travel destination)
  XR Origin (DontDestroyOnLoad)
    ├─ Camera + L/R controllers + ray/direct interactors
    ├─ XRInteractionManager + InputActionManager        ← the ONE input owner (kept enabled across loads)
    ├─ Locomotion providers + CharacterController
    ├─ DashLocomotion        ← move/turn + jump(A)/sprint(L3)/autorun(2×L3)/crouch(R3)/slide
    └─ PlayerRigPersistence.Awake → EnsureXRIWiring + the ENSURE CHAIN (see §2-B):
         Belt · StunReceiver · CreditsHud · RillCompanion · PingTool · QuickSwap · PlayerAvatarRig
  TravelCoordinator (DontDestroyOnLoad)  ← the ONLY way to change scenes
  AudioDirector / SaveSystem / diagnostics (DontDestroyOnLoad)
  BootLoader.Start → TravelCoordinator.TravelTo(FirstWorldScene = W000_DriftIn, skipGate:true)

World / arena scene (W000…W012, ToxicCity, Arena_* — CONTENT ONLY, loaded Single)
  ├─ geometry / props / __SPAWN_PLAYER (SpawnMarkerRuntime)
  ├─ WorldRuntime (+ WorldDirector → SkyVistaRig/SkyPlanetRig theme)
  ├─ guns / creatures / drones / job objects / travel doors
  ├─ ShipCastOffRuntime (W000 only — PUNCH IT)
  └─ DevWarpBoard (dev builds — physical warp tiles beside spawn)
```

**Travel flow (every transition):** `TravelCoordinator.TravelTo(scene[, gatePos][, skipGate])` → **THE
ZIPTIDE departure** (gate FX + RILL line, wrapped in try/catch so it can NEVER strand `_travelling`;
skipped on cold boot) → save HOLSTERED inventory → `SceneManager.LoadScene(Single)` → teleport to
`__SPAWN_PLAYER` → `EnsureXRIWiring` → **THE ZIPTIDE arrival** → wait XRI ready → restore holstered →
`TRAVEL_OK`. Log tags: `BOOT_LOAD`, `TRAVEL_START/OK/FAIL`, `ZIPTIDE_GATE depart/arrive`, `GATE_FAIL`.

**The four locked rules** (`00_LOCKED_CONTRACTS`): (1) world scenes contain no rig/XRI/InputManager/
TravelCoordinator/AudioDirector; (2) `_Boot` is never a destination; (3) `TravelCoordinator.TravelTo` is
the only travel entry; (4) only holstered items travel.

## Part 2 — Assembly DAG (enforced by `DependencyValidator`)
`Core` (no deps) ← `Content` ← `Gameplay` ← `Visuals`(refs Core only) · `Multiplayer`(Core) · `Editor`,
`Platform.Quest`, `Tests`, `Ship` on top. `ZiptideNet/` = Assembly-CSharp (no asmdef) so it sees Photon +
Multiplayer without an asmdef cycle. **Core must never reference Visuals** (validator fails the build).

---

## Part 3 — THE SEAM TABLE (the core: producer → hook → consumer → verifier)

### A. Build-hook chain — every asset producer must be called in `Editor/Build/BuildAndroid.PatchScenesThenAPK`
*If an author isn't hooked, its assets ship stale/missing (the "nothing I fixed worked" class).* **All 17 are hooked ✅.**

| Producer (Editor/Patching) | Produces | Consumed by | Status |
|---|---|---|---|
| WorldLayoutLibrary · ArenaLayoutLibrary | world/arena layout `.asset` | scene patchers at build | ✅ |
| CreatureVariantAuthor | `Resources/Enemies/*` CreatureDefinition | CreatureRuntime by id | ✅ |
| BotProfileAuthor | `Resources/Bots/*` | PvpBot | ✅ |
| BuildingStyleAuthor · CosmeticAuthor · GardenAuthor · EconomyAuthor | style/cosmetic/plant/resource defs | city/garden/economy runtime | ✅ |
| RillLineAuthor | `Resources/Story/RillLines` | RillCompanion | ✅ |
| SkyVistaLibrary + SkyVistaAuthor | vista `.asset` + theme assignment | SkyVistaRig | ✅ |
| ForgeRecipeLibrary + ForgeAuthor | `Resources/Forge/*` recipes + item.forgeRecipeId | ForgeVisualApplier | ✅ |
| ForgeBodyLibrary | `Resources/Forge/Bodies/*` genomes | ForgeCreatureVisualApplier | ✅ |
| ForgeBaker | `Resources/ForgeBaked/*` (gitignored) | ForgeVisualApplier (prefers baked) | ✅ |
| ArenaWeaponAuthor | `Resources/Items/*` arena weapons | ItemFactory | ✅ |
| DevWorldManifestBuilder (Rebuild, LAST) | `Resources/DevWorldManifest` | DevMenu, DevWarpBoard, TravelCoordinator tint | ✅ |

### B. Rig-ensure chain — `Gameplay/.../Player/PlayerRigPersistence` ensures the `_Boot`-owned singletons
*Everything here must be ensured on the persistent rig, never baked per-scene (the old belt bug).*

| Component | Ensured in | Purpose | Verifier | Status |
|---|---|---|---|---|
| XRInteractionManager + InputActionManager | EnsureXRIWiring / EnsurePersistentInputActions | the one input owner | `XRI_READY` | ✅ |
| BeltRig (+ holster sockets) | EnsureBelt | holstered-item travel | `BELT_ENSURED` | ✅ |
| PlayerStunReceiver | EnsureStunReceiver | incoming-fire stun | `PLAYER_HIT` | ✅ |
| CreditsHud | EnsureCreditsHud | credits UI | — | ✅ |
| RillCompanion | EnsureRillCompanion | subtitles + gate lines + memory | `RILL_LINE` | ✅ |
| PingTool · QuickSwap · PlayerAvatarRig | EnsurePingTool | ping / weapon swap / player skin | `PING_AT`,`QUICK_SWAP`,`AVATAR_READY` | ✅ |

### C. String-ID registries (data → asset by ID; never hard refs)
| Registry | ID source | Resolves to | Status |
|---|---|---|---|
| ItemFactory | ItemDefinition.itemId / forgeRecipeId | `Resources/Items/*` + `Resources/Forge/*` | ✅ 5 guns mapped |
| CreatureRuntime | creatureId (zone data) | `Resources/Enemies/*` + behavior switch (CityBuilder.MakeCreature) | ✅ 10 defs |
| ForgeVisualApplier | recipeId | `Resources/Forge/<id>` (+ baked prefab) | ✅ |
| ForgeCreatureVisualApplier | creatureId | `Resources/Forge/Bodies/<id>` | 🔵 2 of 10 authored (rest primitive by design) |
| Melee/gravity items (BreakerBlade, TidePike, Sandbox_GravityGun) | itemId | primitive look — **no forge recipe** | 🔵 intentional (forge look = future polish) |

### D. Applier → consumer (every produced look has a runtime caller)
| Produced | Applier | Called by | Status |
|---|---|---|---|
| gun mesh/maps | ForgeVisualApplier.TryApply | ItemFactory on spawn | ✅ |
| creature skinned body | ForgeCreatureVisualApplier.TryApply | CreatureBehaviorBase.Awake | ✅ |
| creature gait | ForgeCreatureAnimator.Bind | ForgeCreatureVisualApplier | ✅ |

### E. Transport & self-bootstrap seams (the "install a hook / invoke the hook" pairs — A6's risk class)
| Install side | Invoke side | Status |
|---|---|---|
| `ZiptideNet/NetBootstrap` sets `PvpNetHub.OnlineStarter` (RuntimeInitialize, `#if ZIPTIDE_PHOTON`) | `ArenaLobbyBoard` GO ONLINE → `PvpNetHub.StartOnline` | ✅ |
| PhotonPvpLauncher.OnJoinedRoom → `PvpNetHub.SetTransport` | `PvpOnlinePresence` subscribes to `Active.OnPose` | ✅ |
| DevMenu / DevWarpBoard / NetBootstrap `[RuntimeInitializeOnLoadMethod]` | self-bootstrap (no external caller needed) | ✅ |
| ShipCastOffRuntime added by CityBuilder when `sceneName == "W000_DriftIn"` | PUNCH IT → TravelCoordinator | ✅ |

### F. Story flags (`ZiptideFlags`) — granted by completion, consumed by lines/gates/transmission
45 grant sites / 48 consume sites (roughly balanced). Canonical flag→beat map lives in the **story bible**
(`docs/storyboard/STORY_BIBLE.md` + `RillLineAuthor`). Flags for unbuilt worlds (W013+) are authored now
and fire when those worlds ship — 🔵 intentional. Not exhaustively both-sided here; see FINDINGS §F.

### G. Audit-gate coverage — `Editor/Audit/WorldAuditRunner` + rules (the automated wiring surface)
BLOCKER (fails build): world-rig-leak, spawn-safety, singleton-dup, travel-destination, city-geometry,
Forge-recipe-missing, Sky-vista. **WARN-ONLY (pending device baseline): WorldContent, PerfBudget,
Reachability.** ✅ **`Editor/Validation/WiringValidator` (+ EditMode test) NOW FAILS CI** on the
deterministic one-sided seams (build-hook orphan · item→recipe · genome→creature) — menu
`Ziptide → Validate wiring`. See FINDINGS "SAFEGUARDS — Phase 2".

### H. Scene / Build Settings
24 enabled scenes; `_Boot` index 0; boot target = `W000_DriftIn` (guarded by `BootConfigTests`). Every
enabled world/arena has `__SPAWN_PLAYER` + travel; every travel destination is an enabled scene.

---

## Part 4 — THE BOTH-SIDES LAW (how to add a wired system without breaking things)
When you add anything data-driven, you are adding a SEAM. Wire **both** sides in the SAME change, and add
the verifier, or it's a latent gap:
1. **Producer** — an author/library that writes the asset AND is called in `BuildAndroid.PatchScenesThenAPK`.
2. **Consumer** — the runtime that loads it by ID (Resources/registry/factory), never a hard asset ref.
3. **Verifier** — a `ZIPTIDE:` log tag, an EditMode test, or an audit rule that proves the link.
4. **Record it** — a row in this map + `HOW_TO_CHANGE_ANYTHING.md`.
If you can't name all four, you're about to ship a one-sided seam. `WIRING_AUDIT_FINDINGS.md` is the
running proof that every seam has all four.
