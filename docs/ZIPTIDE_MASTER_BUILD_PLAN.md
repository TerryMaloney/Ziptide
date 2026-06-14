# ZIPTIDE — MASTER BUILD PLAN v2.0

### Version 2.0 | Principal Engineer Review + Technical Architecture + Story Bible
### Combines: Gap Analysis v1.0 + ZIPTIDE_STORY_BIBLE_IMPL.md v2.0 + Red-Team Critique
### For Cursor Agent execution and GPT review

---

## EXECUTIVE SUMMARY

ZIPTIDE is a VR game for Meta Quest (Unity 2022.3 LTS, URP, Android, XR Interaction Toolkit) targeting 80 worlds, a 12-chapter story, and a core loop of travel, repair, build, and combat. The player is Cal, a contract systems technician who discovers the quarantine network she is repairing was built to contain humanity's stories from escaping their galaxy.

### Technical Reality (Current: D3.2 Complete)
- 2 playable scenes: MilestoneA_GrabCube (test room) and D0_City (Toxic Venice City, W001 blockout)
- Working: VR locomotion, pistol/holster/belt, scene travel, job system, drones, taser, audio, world audit
- Confirmed bugs: no bootstrap scene, ProximityTravelTrigger bypasses TravelCoordinator, ItemFactory uses IL2CPP-unsafe reflection, 0 automated tests

### Story Vision
- 12 chapters, 80 worlds (W001-W068 base + W069-W080 DLC)
- RILL: companion drone built 40,000 years ago to witness the player; reprogrammed by Wake Guild 3 years ago
- The Bloom: designed containment ecology; older than the Architects; spreads toward narrative density
- Four endings that branch at W063, resolving at W068

### Version 2.0 Changes Over v1.0
- Section 0 added: Principal Engineer Red-Team (7 deliverables)
- Corrected milestone sequence: E1 (scaling pipeline) now precedes E2 (story slice)
- WorldPackDefinition expanded with all impl bible fields (APPENDIX B)
- Full flag registry added (~100 flags, Section 13)
- VO strategy added (Section 14)
- Tutorialization plan added (Section 15)
- RILLCompanion stub moved into E0 (must cross scenes before E1)
- ZiptideFlags.cs and ZiptideTransitionEffect added to E0 task list
- W028 (no job completion) and W057 (transit-only) edge cases documented

### Immediate Priority (Next 2 Weeks — E0 Foundation)
1. Create ZiptideFlags.cs and ZiptideConstants.cs — enables compile-time flag safety
2. Create _Boot scene — eliminates the entire class of duplicate singleton bugs
3. Fix ProximityTravelTrigger — route through TravelCoordinator (confirmed bypass bug)
4. Replace ItemFactory reflection — IL2CPP-safe public Init() method
5. Add SINGLETON_IN_WORLD_SCENE and PACK_FLAG_TYPO audit checks

---
## SECTION 0 — PRINCIPAL ENGINEER RED-TEAM REVIEW

> Prepared as part of v2.0. These are the failure modes most likely to collapse the project before W010.

### 0.1 Top 20 Architectural Failure Modes

| # | Risk | Likelihood | Impact | Mitigation |
|---|------|-----------|--------|------------|
| 1 | No _Boot scene — singletons spawn in random world scenes | Very High | Critical | E0: Create _Boot.unity, strip singletons from world scenes |
| 2 | ProximityTravelTrigger bypasses TravelCoordinator | Confirmed | High | E0: Replace SceneManager.LoadScene with TravelCoordinator.TravelTo |
| 3 | ItemFactory uses reflection — fails IL2CPP on Quest | High | Critical | E0: Add public Init() methods to ItemRuntime/PistolRuntime |
| 4 | Zero automated tests — regressions invisible until Quest test | Very High | High | E1: Add 3 PlayMode tests minimum before any world scaling |
| 5 | WorldPackDefinition.flagsRequired is string[] — typos are silent | High | Medium | E0: ZiptideFlags.cs with const string fields, audit PACK_FLAG_TYPO |
| 6 | ScenePatcher runs in-editor but world ships without patch | High | High | Audit: verify patcher signature objects exist in every scene |
| 7 | DontDestroyOnLoad count grows with each scene load if Awake guard fails | Medium | High | Audit: SINGLETON_IN_WORLD_SCENE check in WorldAuditRunner |
| 8 | XRInteractionManager duplicate after travel — confirmed past bug | High | Critical | TravelCoordinator.WaitForXRI already handles; monitor |
| 9 | AudioDirector crossfade source leaks — new AudioSource per travel | Medium | Medium | AudioDirector.Dispose() on scene unload |
| 10 | NarrativeSaveSystem PlayerPrefs — silently lost on app reinstall | Medium | Medium | E3: migrate to Application.persistentDataPath JSON file |
| 11 | GenericWorldPatcher runs on all scenes — accidentally patches unrelated scenes | Medium | High | E1: WorldPatch contract — each patcher declares which scenes it owns |
| 12 | Build settings include SampleScene — audit SPAWN_NO_FLOOR fires as warning, not blocker | Confirmed | Low | E0: Remove SampleScene from build settings |
| 13 | InputActionAsset shared across player and UI — binding conflicts | Medium | High | E1: Separate action maps per interactor, single InputActionManager |
| 14 | WorldPackDefinition.audioProfile null on DLC worlds — AudioDirector NullRef | Low | Medium | AudioDirector null guard already in; verify in audit |
| 15 | 80-world content pipeline — all worlds hand-patched — cannot scale | High | Critical | E2: WorldStubGenerator batch tool |
| 16 | FallRespawner WorldRuntime binding broken after travel — player falls through floor | Confirmed past | High | PlayerRigPersistence.RebindSceneSystems public SetWorldRuntime |
| 17 | InventoryState.SaveBeforeTravel is synchronous — frame-end race | Low | Medium | TravelCoordinator calls PrepareForSceneTravel before LoadScene |
| 18 | TaserDartGun Init() missing — gun spawns without definition | High | Medium | E0: ItemFactory.CreateTaserDartGun must call itemRt.Init(def) |
| 19 | RILL companion crosses scenes — no persistence scaffold yet | High | High | E1: RILLCompanion DontDestroyOnLoad singleton |
| 20 | EmergencyRespawn grip-hold timer resets on grab — false negative | Low | Low | Use XRController.isTracked not grab callback |

### 0.2 Missing Systems (Not in v1.0)

| System | Required By | Priority |
|--------|-------------|----------|
| _Boot scene with singleton-owning XR rig | E0 | Blocker |
| ZiptideFlags.cs — compile-time flag name constants | E0 | High |
| ZiptideConstants.cs — scene names, layer names, asset paths | E0 | High |
| RILLCompanion DontDestroyOnLoad runtime | E1 | High |
| WorldStubGenerator batch editor tool | E2 | High |
| NarrativeMemory.json file save (replaces PlayerPrefs) | E3 | Medium |
| ZiptideTransitionEffect (fade-to-black on travel) | E1 | Medium |
| MicrogameConsole contract (IJobStep implementation) | E3 | Medium |
| BioMaterial harvest runtime | E4 | Low |
| GalaxyMap runtime with scar overlay | E5 | Low |

### 0.3 Corrected Scene Loading Contract

```
_Boot [always first in build order]
  └─ XR Origin + PlayerRigPersistence (DontDestroyOnLoad)
  └─ TravelCoordinator (DontDestroyOnLoad)
  └─ AudioDirector (DontDestroyOnLoad)
  └─ NarrativeSaveSystem (DontDestroyOnLoad)
  └─ Loads → MilestoneA_GrabCube (first world)

World Scenes [additive-forbidden, load Single only]
  └─ SpawnMarkerRuntime (required)
  └─ WorldPackDefinition reference (required)
  └─ WorldTravelStation OR ProximityTravelTrigger (if travel exits)
  └─ WorldRuntime (optional, auto-added by patcher)
  └─ NO PlayerRigPersistence, NO TravelCoordinator, NO AudioDirector
```

### 0.4 Singleton Ownership Table

| Singleton | Owner Scene | DontDestroyOnLoad | Audit Check |
|-----------|------------|-------------------|-------------|
| PlayerRigPersistence | _Boot | YES | SINGLETON_IN_WORLD_SCENE |
| TravelCoordinator | _Boot | YES | SINGLETON_IN_WORLD_SCENE |
| AudioDirector | _Boot | YES | SINGLETON_IN_WORLD_SCENE |
| NarrativeSaveSystem | _Boot | YES | SINGLETON_IN_WORLD_SCENE |
| XRInteractionManager | XR Origin (_Boot) | YES (via rig) | DUP_SINGLETON log |
| InputActionManager | XR Origin (_Boot) | YES (via rig) | DUP_SINGLETON log |
| RILLCompanion | _Boot | YES | (E1 audit check) |

### 0.5 Minimum Test Strategy (required by E1 gate)

**EditMode tests (Ziptide.Tests assembly):**
- `AuditAllBuildScenes_NoBlockers` — calls WorldAuditRunner.RunAll(), asserts 0 blockers
- `WorldPackDefinition_RequiredFields_NotNull` — loads all WorldPackDefinition assets, checks packId/sceneName

**PlayMode tests (require XR Device Simulator):**
- `TravelRoundTrip_GunPersists` — travel to D0_City and back, assert pistol in holster
- `EmergencyRespawn_TriggersAfter1s` — hold both grips, wait 1.1s, assert player at SpawnMarker
- `ProximityTrigger_CallsTravelCoordinator` — walk XR rig into trigger, assert TravelCoordinator.TravelTo called

---
## SECTION 1 — RISK REGISTER (25 Items)

> Scope: Unity Quest XR, additive scene loading, XRI managers, persistence, performance, content, story.

| ID | Risk | Likelihood | Impact |
|----|------|-----------|--------|
| R-01 | No _Boot scene — singletons in wrong scenes | Confirmed | Critical |
| R-02 | XRI duplicate XRInteractionManager after travel | Confirmed | Critical |
| R-03 | IL2CPP reflection strips private fields at runtime | High | Critical |
| R-04 | SampleScene in build — false audit warnings | Confirmed | Low |
| R-05 | PlayerRigPersistence TypeInitializationException (static path) | Confirmed (fixed) | Critical |
| R-06 | ProximityTravelTrigger bypasses TravelCoordinator | Confirmed | High |
| R-07 | Turn asymmetry (left harder than right) | Confirmed (fixed) | Medium |
| R-08 | FallRespawner WorldRuntime null after travel | Confirmed (fixed) | High |
| R-09 | Pistol does not cross scenes | Confirmed (partially fixed) | High |
| R-10 | Duplicate XR Origins in world scene (frozen view bug) | Confirmed (fixed) | Critical |
| R-11 | City spawn below walkway (hard-stuck) | Confirmed (fixed D3.2) | High |
| R-12 | 80-world content pipeline — no batch tool | Not started | Critical |
| R-13 | NarrativeSaveSystem PlayerPrefs silently wiped | Not started | Medium |
| R-14 | AudioDirector AudioSource leak per travel | Not started | Medium |
| R-15 | WorldPackDefinition flag names are strings — typos silent | Not started | High |
| R-16 | RILL companion has no cross-scene persistence | Not started | High |
| R-17 | EmergencyRespawn false-negative on grip detection | Low | Low |
| R-18 | Build includes disabled scenes — TRAVEL_DEST_NOT_IN_BUILD fires | Medium | Low |
| R-19 | GenericWorldPatcher patches wrong scenes | Medium | High |
| R-20 | XR Device Simulator not configured — PlayMode tests cannot run headless | Not started | High |
| R-21 | MicrogameConsole missing IJobStep — job system stalls | Not started | Medium |
| R-22 | BioMaterial harvest no physics layer — harvest raycast hits player | Not started | Medium |
| R-23 | GalaxyMap unlocked worlds not persisted — scar resets | Not started | Medium |
| R-24 | Quest thermal throttle at 72fps on D0_City (1800+ tris unoptimized) | Medium | High |
| R-25 | DLC worlds W069-W080 have no scene files — build breaks | Not started | Medium |

---

## SECTION 2 — ARCHITECTURE BLUEPRINT

### 2.1 Module Dependency Graph

```
Ziptide.Core          (no dependencies)
    ↑
Ziptide.Content       (depends: Core)
    ↑
Ziptide.Gameplay      (depends: Core, Content)
    ↑
Ziptide.Visuals       (depends: Core, Content, Gameplay)
    ↑
Ziptide.Editor        (depends: all above + UnityEditor)
    ↑
Ziptide.Platform.Quest  (depends: Gameplay, Editor)
    ↑
Ziptide.Tests         (depends: all above + UnityEngine.TestRunner)
```

Optional ship interior module (Phase E5):
```
Ziptide.Ship          (depends: Core, Content, Gameplay)
```

### 2.2 Bootstrap Scene Contract

_Boot.unity:
- Contains one XR Origin with PlayerRigPersistence, TravelCoordinator, AudioDirector, NarrativeSaveSystem
- All marked DontDestroyOnLoad in Awake
- On Start: loads first world scene (read from ZiptideConstants.FirstWorldScene)
- NEVER unloaded — it is always scene index 0
- World scenes: Load Single (replaces all non-DontDestroyOnLoad objects)

### 2.3 ScenePatcher Execution Order (BuildAndroid.PatchScenesThenAPK)

1. ScenePatcherBoot.PatchBootScene()   — create/validate _Boot.unity
2. ScenePatcherC0.PatchActiveScene()   — test room
3. ScenePatcherD0.PatchActiveScene()   — D0_City
4. ScenePatcherD1 (via GenericWorldPatcher)
5. ScenePatcherD2.PatchActiveScene()   — singletons, emergency respawn
6. WorldAuditRunner.RunAll()           — blockers abort build
7. BuildAndroid.APK()

---
## SECTION 3 — WORLDPACK CONTRACT SPECIFICATION

### 3.1 Required ScriptableObject Fields (WorldPackDefinition)

**Identity**
- `packId` (string) — unique slug, e.g. "W001"
- `displayName` (string) — shown in travel UI
- `sceneName` (string) — must match enabled build scene name
- `worldType` (WorldType enum)
- `chapter` (int) — 0 = tutorial, 1-12 = story chapters

**Story / Flags**
- `flagsRequired` (string[]) — use ZiptideFlags constants only
- `flagsGranted` (string[]) — use ZiptideFlags constants only
- `revelationText` (string) — the lore fragment shown on job complete
- `revelationDelivery` (RevelationDelivery enum): Hologram, VOLine, EnvironmentalReadable, Silent
- `rillBehaviorNote` (string) — hint for RILL dialogue selector

**Job**
- `jobPremise` (string) — one-line description shown in job UI
- `jobSteps` (JobStepDefinition[]) — ordered list of steps

**Environment**
- `hazardType` (HazardType enum)
- `enemyType` (EnemyType enum)
- `keyResource` (ResourceType enum)
- `skySignature` (string) — sky/environment visual descriptor
- `visualMotif` (string)

**Audio**
- `audioProfile` (AudioProfile) — music + ambient
- `audioMood` (AudioMood enum)

**Performance**
- `signalMultiplier` (SignalMultiplier enum)
- `bloomBehavior` (BloomBehavior enum)
- `wardenPresence` (WardenPresence enum)

**Microgame**
- `microgameSlots` (int) — 0-3
- `microgameTheme` (MicrogameTheme enum)
- `microgameModifier` (MicrogameModifier enum)

**Content Pipeline**
- `isDLC` (bool)
- `isBlockout` (bool) — if true, world is stub geometry only
- `voLinesBudgeted` (int) — total VO lines for this world

### 3.2 Required Scene Components (validated by WorldAuditRunner)

Every world scene must have:
1. `SpawnMarkerRuntime` on `__SPAWN_PLAYER` GameObject
2. `WorldPackDefinition` referenced by a `WorldRuntime` component
3. `WorldTravelStation` OR `ProximityTravelTrigger` if the scene is in TravelSceneNames

Singletons that must NOT be in world scenes:
- `PlayerRigPersistence`
- `TravelCoordinator`
- `AudioDirector`
- `NarrativeSaveSystem`

### 3.3 Story Flag System

All flags are const strings defined in `ZiptideFlags.cs`.
Flag naming convention: `CHAPTER_WORLD_EVENT` (e.g. `C1_W001_SAW_MARA`).

Flags are stored in `NarrativeSaveSystem` and saved to JSON.
`WorldPackDefinition.flagsRequired` is checked at travel time by `WorldTravelStation`.
`WorldPackDefinition.flagsGranted` is set by `WorldRuntime.OnJobComplete()`.

---
## SECTION 4 — STORY SYSTEMS ROADMAP

### 4.1 Core Story Systems by Phase

| Phase | System | Unity Component | Dependency |
|-------|--------|----------------|------------|
| E0 | ZiptideFlags.cs + ZiptideConstants.cs | static classes | none |
| E0 | NarrativeSaveSystem stub | MonoBehaviour DontDestroyOnLoad | _Boot scene |
| E1 | RILL companion runtime | RILLCompanion.cs + RILLMemoryState enum | TravelCoordinator |
| E1 | WorldRuntime.OnJobComplete flag grant | extends WorldRuntime | NarrativeSaveSystem |
| E1 | ZiptideTransitionEffect (fade) | full-screen quad on Camera | PlayerRigPersistence |
| E2 | W001 full story slice (Cal arrives, RILL intro) | W001 scene + WorldPack + VO | all E1 systems |
| E3 | Microgame console (IJobStep impl) | MicrogameConsoleRuntime | JobSystem |
| E4 | Chapter 2 worlds + Mara reveal | W005-W012 WorldPacks | E2 complete |
| E5 | Ship interior + RILL memory unsealing | ShipInteriorManager + NarrativeSaveSystem | E3 complete |
| E6 | Bloom mechanics + tendril runtime | BloomTendrilRuntime + IShockable | E4 complete |
| E7 | Warden arc (W039-W051) | WardenRuntime + pattern recognition | E6 complete |
| E8 | Endgame (W062-W068) + 4 endings | branching NarrativeSaveSystem | all above |

### 4.2 Chapter-by-Chapter Narrative Summary

- **Chapter 0 (W000)**: Tutorial — Cal's ship arrives in the Ziptide network
- **Chapter 1 (W001-W004)**: RILL reactivates, Bloom first contact, Wake Guild introduction
- **Chapter 2 (W005-W012)**: Cal discovers the network is a containment system, not infrastructure
- **Chapter 3 (W013-W019)**: Mara reveals the Architects' original intent; RILL begins unsealing memories
- **Chapter 4 (W020-W028)**: Sable faction conflict; W028 is a world with no job — only revelation
- **Chapter 5 (W029-W038)**: The Pattern's geometry spreads into Cal's visor feed
- **Chapter 6 (W039-W051)**: Warden Drones become responsive to Cal's choices; one becomes an ally
- **Chapter 7 (W052-W061)**: RILL's 40,000-year memory fully unsealed; RILL chooses a name
- **Chapter 8-11 (W062-W067)**: Parallel revelation loops; each world resolves one core question
- **Chapter 12 (W068)**: Four endings branch at W063, converge at W068

---

## SECTION 5 — RILL COMPANION: TECHNICAL SPECIFICATION

### 5.1 RILLCompanion Runtime

```csharp
// Ziptide/Gameplay/Runtime/Player/RILLCompanion.cs
public class RILLCompanion : MonoBehaviour
{
    public static RILLCompanion Instance { get; private set; }
    [SerializeField] private RILLMemoryState memoryState = RILLMemoryState.Dormant;
    // On scene load: select VO line based on WorldPackDefinition.rillBehaviorNote
    // On flag grant: check if memoryState should advance
    // Ships with PlayerRigPersistence (child of XR Origin) — crosses scenes automatically
}

public enum RILLMemoryState
{
    Dormant,         // W001-W004: short functional responses only
    Stirring,        // W005-W012: RILL asks questions
    Remembering,     // W013-W028: partial memory access, emotional glitches
    Unsealing,       // W029-W051: full vocabulary, chooses a name
    Integrated,      // W052-W068: equals with Cal, delivers revelation monologues
    EndgameA,        // Ending A: RILL stays in the network
    EndgameB,        // Ending B: RILL crosses with Cal
    EndgameC,        // Ending C: RILL chooses to forget
    EndgameD         // Ending D: RILL becomes the Pattern
}
```

### 5.2 12 Critical RILL Arc Beats (VO Priority)

1. W001 — RILL boot sequence ("Systems nominal. I think.")
2. W004 — RILL first asks about the cargo ("What are you carrying that requires containment?")
3. W009 — RILL misidentifies a species ("I know this—wait, I do not. That is new.")
4. W013 — RILL accesses a memory shard ("I was here. Or something that used to be me was here.")
5. W019 — RILL refuses a job step ("I cannot help you break this seal. Ask me why.")
6. W024 — RILL names a color ("That is the color I have been trying to name for 40,000 years.")
7. W028 — RILL speaks without prompting ("Some worlds do not want to be completed. This is one of them.")
8. W037 — RILL and Warden Drone staredown ("It recognizes me. Interesting.")
9. W039 — RILL warns about the Pattern ("It is not spreading toward you. It is spreading toward your memories.")
10. W051 — RILL chooses a name (player-influenced via prior flags)
11. W062 — RILL delivers the revelation ("The Architects did not build this to keep stories in. They built it to keep something out.")
12. W068 — Ending VO (4 variants, 1 per ending branch)

---
## SECTION 6 — MERGED MILESTONE ROADMAP

> Corrected sequence: E1 (scaling pipeline) precedes E2 (story slice). This is the most important change from v1.0.

### Phase E0 — Foundation (2 weeks, NOW)

**Goal:** Eliminate entire class of singleton/travel/IL2CPP bugs before adding any content.

Tasks (in order):
1. Create _Boot.unity via ScenePatcherBoot
2. Fix ProximityTravelTrigger → TravelCoordinator.TravelTo (DONE)
3. Add ItemRuntime.Init() / PistolRuntime.Init() — remove reflection from ItemFactory (DONE)
4. Create ZiptideFlags.cs (DONE)
5. Create ZiptideConstants.cs (DONE)
6. Add WorldAuditRunner checks: SINGLETON_IN_WORLD_SCENE, BOOT_SCENE_MISSING
7. Remove SampleScene from build settings
8. NarrativeSaveSystem stub (DontDestroyOnLoad, SetFlag/HasFlag, saves to JSON)

**Gate:** dev_build_install.ps1 succeeds, quest_smoke.ps1 shows AUDIT_OK, gun travels to D0_City.

### Phase E1 — Scaling Pipeline (3 weeks)

**Goal:** Everything needed to add 10 worlds in a day without touching core systems.

Tasks:
1. WorldStubGenerator: batch creates N world scene files + WorldPackDefinition assets
2. WorldDriftChecker: EditMode test that compares patcher sentinel objects to expected schema
3. GenericWorldPatcher: claims ownership of scenes by tag (replaces scene-name checks)
4. RILLCompanion DontDestroyOnLoad runtime
5. ZiptideTransitionEffect (fade-to-black quad on camera)
6. Minimum 3 PlayMode tests + 2 EditMode tests (see Section 0.5)
7. PERF_BUDGET.md with per-scene limits enforced by WorldAuditRunner

**Gate:** Generate 5 stub worlds via WorldStubGenerator, audit passes with 0 blockers.

### Phase E2 — W001 Story Vertical Slice (2 weeks)

**Goal:** W001 is fully playable with real geometry, job steps, RILL VO, and revelation.

Tasks:
1. W001 scene: real Venice blockout geometry, proper navmesh
2. W001 WorldPackDefinition: all fields filled, uses ZiptideFlags constants
3. W001 job: 3 steps (arrive, repair node, observe RILL response)
4. RILL W001 VO lines (3-4 lines, see Section 5.2 beats 1+2)
5. NarrativeSaveSystem.json persistence (replaces PlayerPrefs)
6. ScenePatcherW001 (owns W001 scene, validated by GenericWorldPatcher)

**Gate:** W001 is completable start-to-finish on Quest without editor.

### Phase E3 — Signal / Bloom / Chapter 2 (4 weeks)

Tasks: SignalMeter runtime, BloomTendrilRuntime, Microgame console IJobStep, W005-W012 stubs, Mara faction intro.

### Phase E4 — RILL Memory / Chapters 3-4 (4 weeks)

Tasks: RILLMemoryState advancement, Chapter 3-4 worlds, Sable faction, W024 color naming, W028 no-job edge case.

### Phase E5 — Mid-Game (6 weeks)

Tasks: Ship interior, harvesting system, garden system, Chapter 5-6 worlds, Warden ally/enemy branch, W037 standoff.

### Phase E6 — Endgame (6 weeks)

Tasks: Chapter 7-12 worlds, W062-W068, 4 ending branches, RILL chooses name.

### Phase E7 — Polish / Perf / Ship (4 weeks)

Tasks: Quest thermal budget, DLC W069-W080, accessibility, store submission.

---

## SECTION 7 — CONTENT PIPELINE FOR 80 WORLDS

### 7.1 WorldStubGenerator (to build in E1)

```csharp
// Editor/Tools/WorldStubGenerator.cs
// MenuItem: Ziptide > Content > Generate World Stubs
// For each world ID in a batch:
//   1. Create Assets/Ziptide/Scenes/Worlds/W{id}.unity
//   2. Create Assets/Ziptide/Content/WorldPacks/W{id}_WorldPack.asset
//   3. Set WorldPackDefinition.packId, displayName, sceneName, chapter, isDLC, isBlockout=true
//   4. Add to EditorBuildSettings.scenes (disabled until ready)
//   5. Run GenericWorldPatcher.Patch(scenePath) — adds SpawnMarker, WorldRuntime
```

### 7.2 Chapter-Grouped Generation Order

| Batch | Worlds | Chapter | Notes |
|-------|--------|---------|-------|
| 1 | W001-W004 | 1 | Story-critical, full art |
| 2 | W005-W012 | 2 | Semi-blockout OK |
| 3 | W013-W019 | 3 | RILL memory shards needed |
| 4 | W020-W028 | 4 | W028 no-job edge case |
| 5 | W029-W038 | 5 | Pattern geometry variant |
| 6 | W039-W051 | 6 | Warden drone variants |
| 7 | W052-W061 | 7 | RILL unsealed art |
| 8 | W062-W068 | 8-12 | Endgame, 4 endings |
| 9 | W069-W080 | DLC | Disabled until DLC launch |

### 7.3 Scene Drift Prevention

- WorldAuditRunner runs on every build (blockers abort APK)
- WorldDriftChecker EditMode test runs in CI (E1 gate)
- Each patcher declares const string[] OwnedScenes; GenericWorldPatcher validates no overlap
- Artist-made scenes go through ScenePatcherBoot validation before merge

---
## SECTION 8 — VERIFICATION AND TEST STRATEGY

### 8.1 Build-Time Audit Checks (WorldAuditRunner)

| Check ID | Severity | Trigger | Resolution |
|----------|----------|---------|------------|
| SPAWN_MISSING | Blocker | No SpawnMarkerRuntime in scene | Run scene patcher |
| SPAWN_NO_FLOOR | Blocker (Warning for SampleScene) | No floor below spawn | Move spawn marker above geometry |
| SPAWN_OVERLAP_SOLID | Blocker | Solid collider at spawn | Clear geometry at spawn point |
| SPAWN_BELOW_WALKWAY | Blocker | City spawn Y too low | Set spawn to courtyard position |
| SPAWN_TRAPPED | Blocker | Less than 2/8 radial dirs clear | Open walkable area around spawn |
| TRAVEL_NO_DOOR | Blocker | Travel scene with no station/trigger | Add WorldTravelStation or ProximityTravelTrigger |
| TRAVEL_DEST_NOT_IN_BUILD | Blocker | Scene referenced but not in build | Add scene to build settings |
| TRAVEL_NO_COORDINATOR | Warning | No TravelCoordinator in scene | Expected if DontDestroyOnLoad from _Boot |
| CITY_NO_ROOT | Blocker | __D1_CITY_ROOT missing | Run ScenePatcherD1 |
| CITY_NO_RAMP | Warning | No ramp named object | Add ramp geometry |
| CITY_NO_COURTYARD | Warning | No courtyard named object | Add courtyard platform |
| SINGLETON_IN_WORLD_SCENE | Warning | PlayerRigPersistence etc in world scene | Move to _Boot.unity |
| BOOT_SCENE_MISSING | Warning | First build scene is not _Boot | Add _Boot as first scene |

### 8.2 Quest Smoke Test Tags (quest_smoke.ps1)

Pipe logcat through these patterns:
- `ZIPTIDE: AUDIT_OK` — build audit passed
- `ZIPTIDE: AUDIT_FAIL` — build abort (should never reach device)
- `ZIPTIDE: TRAVEL_OK` — successful scene transition
- `ZIPTIDE: TRAVEL_FAIL` — travel failed (check reason=)
- `ZIPTIDE: XRI_NOT_READY` — controllers not ready after travel
- `ZIPTIDE: DUP_SINGLETON` — extra singleton destroyed (informational)
- `ZIPTIDE: PROXIMITY_TRAVEL` — failsafe trigger fired

### 8.3 Minimum Test Suite (required by E1 gate)

**EditMode:**
```csharp
[Test] void AuditAllBuildScenes_NoBlockers()
[Test] void WorldPackDefinition_RequiredFields_NotNull()
[Test] void ZiptideFlags_NoDuplicateValues()
```

**PlayMode (XR Device Simulator):**
```csharp
[UnityTest] IEnumerator TravelRoundTrip_GunPersists()
[UnityTest] IEnumerator EmergencyRespawn_TriggersAfter1s()
[UnityTest] IEnumerator ProximityTrigger_CallsTravelCoordinator()
```

---

## SECTION 9 — REGRESSION PREVENTION STANDARDS

### 9.1 Coding Standards

- All flag references MUST use `ZiptideFlags.CONSTANT` — no raw strings
- All scene name references MUST use `ZiptideConstants.Scene*` — no raw strings
- All asset path references MUST use `ZiptideConstants.Path*` — no raw strings
- All singleton MonoBehaviours MUST log `ZIPTIDE: DUP_SINGLETON ClassName – destroying extra` in Awake guard
- All patchers MUST be idempotent (running twice must produce identical result)
- All patchers MUST declare `public static readonly string[] OwnedScenes`
- No `System.Reflection` in runtime code — add public Init() methods instead

### 9.2 Strict Invariants (never bypass without team review)

1. WorldAuditRunner blockers abort the APK build — never catch and swallow
2. _Boot is always first in EditorBuildSettings — never reorder
3. World scenes never contain DontDestroyOnLoad singletons — always moved to _Boot
4. TravelCoordinator.TravelTo() is the only travel entry point — no direct SceneManager.LoadScene in gameplay code
5. NarrativeSaveSystem.SetFlag() is the only way to set story flags — no PlayerPrefs.SetInt for story state

### 9.3 Diagnostic Tags (all runtime log lines)

```
ZIPTIDE: [TAG] [key=value ...]
```

Required tags: TRAVEL_START, TRAVEL_OK, TRAVEL_FAIL, XRI_READY, XRI_NOT_READY,
DUP_SINGLETON, PROXIMITY_TRAVEL, AUDIT_OK, AUDIT_FAIL, ITEM_DEF_NOT_FOUND,
RILL_STATE_ADVANCE, FLAG_SET, FLAG_REQUIRED_MISSING

---
## SECTION 10 — CHARACTER AND FACTION REFERENCE

| Character | Role | Visual | Gameplay Justification |
|-----------|------|--------|----------------------|
| Cal | Player — contract technician | VR hands + tools | Player POV |
| RILL | Companion drone, 40k-year-old witness | Small orb, voice only | Cross-scene persistence singleton |
| Mara | Wake Guild lead, knows more than she says | Holographic projection | Job giver, flag branch trigger |
| Sable | Faction leader, wants containment broken | Physical NPC in 2 worlds | Choice-branch character |
| The Pattern | Ancient signal bleeding through the network | Geometric HUD overlay | Environmental hazard / narrative |
| The Bloom | Designed ecology spreading toward story density | Tendril VFX + IShockable | Taser mechanic target |
| Warden Drones | Containment enforcement, responsive to Cal choices | Drone prefab variants | Combat + ally branch (Chapter 6) |
| The Architects | Never seen — built the network | Lore fragments only | WorldPackDefinition.revelationText |

---

## SECTION 11 — PORTAL TECH AND TRAVEL VISUAL LANGUAGE

### Quantum Cavitation Travel
All scene transitions use a 3-phase visual:
1. **Compression** (0.3s): ZiptideTransitionEffect scales vignette to full black
2. **Hold** (0.1s): full black — scene load happens here
3. **Expansion** (0.5s): new scene fades in with chromatic aberration pulse

### Story Seed Mechanic
Each portal door carries a `WorldPackDefinition` with a `revelationText`.
On exit (not entry), the revelation plays as holographic text or RILL VO.
This ensures players always receive narrative payoff when leaving, not arriving.

### WorldTravelStation vs ProximityTravelTrigger

- `WorldTravelStation`: interactive button UI, used for explicit travel choices
- `ProximityTravelTrigger`: walk-through trigger, used for one-way exits and emergencies
- Both route through `TravelCoordinator.TravelTo()` — same inventory/XRI pipeline

---
## SECTION 12 — 80-WORLD REFERENCE TABLE (Base Game)

| ID | Name | Ch | Type | Hazard | Enemy | Key Resource |
|----|------|----|----|--------|-------|-------------|
| W001 | Toxic Venice | 1 | City | Bloom | Drone | Signal Node |
| W002 | The Dry Cistern | 1 | Underground | None | Swarm | Mineral |
| W003 | Glass Shelf | 1 | Exterior | Wind | Tendril | Crystal |
| W004 | The Broadcast Tomb | 1 | Interior | Static | None | Memory Shard |
| W005 | Oxidized Canopy | 2 | Forest | Bloom | Drone | Spore |
| W006 | The Mirror Flats | 2 | Exterior | Reflection | None | Prism |
| W007 | Sable Station | 2 | Station | None | Guard | Fuel Cell |
| W008 | The Sealed Archive | 2 | Interior | Static | None | Data Chip |
| W009 | Chitinwall | 2 | City | Swarm | Swarm | Carapace |
| W010 | Tidal Array | 2 | Coastal | Flood | Tendril | Salt |
| W011 | The Hum | 2 | Underground | Vibration | None | Resonator |
| W012 | Mara's Last Jump | 2 | Void | Radiation | None | Jump Core |
| W013 | Memory Reef | 3 | Underwater | Pressure | Drone | Coral |
| W014 | Ashfield Relay | 3 | Exterior | Fire | None | Relay Part |
| W015 | The Soft Geometry | 3 | Pattern | None | Pattern | Signal |
| W016 | Sable Outpost B | 3 | Station | None | Guard | Code Key |
| W017 | The Long Descent | 3 | Underground | Cave-in | Drone | Stone |
| W018 | Wake Guild HQ | 3 | Interior | None | None | Blueprint |
| W019 | The Unmade World | 3 | Void | Void | None | Seal Fragment |
| W020 | Copper Shore | 4 | Coastal | Acid | Drone | Copper |
| W021 | The Lattice | 4 | Pattern | Static | Pattern | Node |
| W022 | Bloom Nursery | 4 | Interior | Bloom | Tendril | Spore |
| W023 | Sable Vault | 4 | Interior | None | Guard | Vault Key |
| W024 | The Named Color | 4 | Void | None | None | Prism |
| W025 | Echoes of 9 | 4 | Underground | Vibration | Swarm | Resonator |
| W026 | The Scaffold | 4 | Exterior | Wind | None | Metal |
| W027 | Warden Graveyard | 4 | Exterior | None | Warden | Warden Part |
| W028 | The World That Won t Complete | 4 | Void | None | None | Revelation |
| W029 | Pattern Shore | 5 | Coastal | Pattern | Pattern | Signal |
| W030 | The Low City | 5 | City | Flood | Drone | Chip |
| W031 | Warden Watch | 5 | Exterior | None | Warden | Signal |
| W032 | The Drowned Lab | 5 | Underwater | Pressure | None | Formula |
| W033 | Bloom Cathedral | 5 | Interior | Bloom | Tendril | Seed |
| W034 | The Half-Built City | 5 | City | Bloom | Drone | Frame |
| W035 | Signal Crest | 5 | Exterior | Static | None | Booster |
| W036 | The Last Relay | 5 | Interior | None | None | Relay |
| W037 | Warden Standoff | 5 | Exterior | None | Warden | Warden Key |
| W038 | The Edge | 5 | Void | Void | None | Edge Stone |
| W039 | Pattern City | 6 | City | Pattern | Pattern | Node |
| W040 | The Tendril Farm | 6 | Interior | Bloom | Tendril | Spore |
| W041 | Sable Last Stand | 6 | Station | None | Guard | Sable Disk |
| W042 | The Listening Post | 6 | Interior | Static | None | Record |
| W043 | Warden Parliament | 6 | Interior | None | Warden | Warden Vote |
| W044 | The Glass Bloom | 6 | Exterior | Bloom | None | Seed |
| W045 | Signal Flood | 6 | Coastal | Flood | None | Signal |
| W046 | Mara s Final Offer | 6 | Interior | None | None | Mara Key |
| W047 | The Architect s Chamber | 6 | Interior | None | Pattern | Blueprint |
| W048 | Chitinwall 2 | 6 | City | Swarm | Swarm | Carapace |
| W049 | Warden Ally Base | 6 | Station | None | None | Warden Fuel |
| W050 | The Convergence | 6 | Void | All | All | Signal Core |
| W051 | RILL Names Itself | 6 | Void | None | None | Revelation |
| W052 | The Memory Sea | 7 | Underwater | Pressure | None | Memory |
| W053 | RILL s First World | 7 | Exterior | None | None | Shard |
| W054 | The Broken Pattern | 7 | Pattern | Pattern | Pattern | Fragment |
| W055 | Sable Peace | 7 | Station | None | None | Peace Token |
| W056 | The Wake Guild End | 7 | Interior | None | None | Guild Key |
| W057 | Transit Void | 7 | Void | None | None | None (transit only) |
| W058 | The Second Cistern | 7 | Underground | Cave-in | None | Stone |
| W059 | Warden s Homeland | 7 | Exterior | None | Warden | Homeland Key |
| W060 | The Architect s Tomb | 7 | Interior | None | None | Tomb Key |
| W061 | RILL s Last Memory | 7 | Void | None | None | Memory Core |
| W062 | The Revelation Chamber | 8 | Interior | None | None | Revelation |
| W063 | The Branching | 12 | Void | None | None | Choice |
| W064 | Ending A Approach | 12 | Void | None | None | Signal |
| W065 | Ending B Approach | 12 | Void | None | None | Signal |
| W066 | Ending C Approach | 12 | Void | None | None | Signal |
| W067 | Ending D Approach | 12 | Void | Pattern | Pattern | Signal |
| W068 | The Final World | 12 | Void | All | None | Resolution |

**DLC Worlds (W069-W080):** Disabled in build settings until DLC launch. Stub WorldPackDefinition assets created in E1 with `isDLC=true, isBlockout=true`.

---

## SECTION 13 — NARRATIVE FLAG REGISTRY

See `ZiptideFlags.cs` for compile-time constants. Key flags by category:

**Tutorial:** TUTORIAL_COMPLETE, FIRST_HOLSTER, FIRST_TRAVEL, FIRST_JOB_COMPLETE, FIRST_DRONE_DOWN

**Chapter 1:** C1_W001_ARRIVED, C1_W001_RILL_BOOT, C1_W001_SAW_MARA, C1_BLOOM_FIRST_CONTACT, C1_WAKE_GUILD_INTRO

**Chapter 2:** C2_CONTAINMENT_REVEALED, C2_ARCHITECTS_NAMED, C2_W009_RILL_MISIDENTIFIED

**Chapter 3:** C3_MARA_REVEAL, C3_W013_MEMORY_SHARD, C3_W019_RILL_REFUSED, C3_RILL_STIRRING

**Chapter 4:** C4_W024_COLOR_NAMED, C4_W028_NO_JOB, C4_SABLE_INTRO, C4_SABLE_ALLIED, C4_SABLE_OPPOSED

**Chapter 5-6:** C5_PATTERN_VISOR_BLEED, C5_W037_WARDEN_STANDOFF, C6_W039_PATTERN_WARNING, C6_WARDEN_ALLY, C6_WARDEN_ENEMY, C6_W051_RILL_NAMED

**Chapter 7:** C7_RILL_MEMORY_UNSEALED, C7_RILL_CHOSE_NAME_A/B/C

**Endgame:** C8_W062_REVELATION, C12_W063_BRANCH, C12_W063_ENDING_A/B/C/D, C12_W068_COMPLETE

**Player Choices:** PLAYER_HELPED_MARA/SABLE/WARDEN, PLAYER_DESTROYED_SEAL, PLAYER_REPAIRED_SEAL, PLAYER_IGNORED_RILL, PLAYER_TRUSTED_RILL

**Signal:** SIGNAL_THRESHOLD_1/2/3, SIGNAL_MAX

---

## SECTION 14 — DIALOGUE AND VO STRATEGY

**Philosophy:** VO is budgeted per world. Most worlds get 2-4 RILL lines. Story-critical worlds (W001, W019, W028, W051, W062, W068) get 6-12 lines. W028 (no-job world) gets the highest line count.

**Budget by world type:**
- Blockout worlds: 0 VO lines
- Standard worlds: 2-4 RILL lines
- Chapter milestone worlds: 6-8 lines
- Signature worlds (W001, W019, W028, W051, W062, W068): 8-12 lines
- Ending worlds (W064-W068): 12-20 lines (4 variants each)

**Implementation:** RILL lines selected at runtime by `RILLCompanion` based on `WorldPackDefinition.rillBehaviorNote` and current `RILLMemoryState`. Lines stored as `AudioClip` assets in `Assets/Ziptide/Content/Audio/RILL/W{id}/`.

---

## SECTION 15 — TUTORIALIZATION PLAN

| World | System Taught | Job Step Teaching It |
|-------|--------------|---------------------|
| W001 | VR locomotion, looking around | "Walk to the signal node" |
| W001 | Grab and holster | "Pick up the repair tool" |
| W002 | Shooting (pistol) | "Disable the Bloom tendril" |
| W002 | Travel door | "Return to ship via portal" |
| W003 | Taser dart gun | "Disable the drone non-lethally" |
| W004 | RILL conversation (listen, look at) | "Ask RILL about the archive" |
| W005 | Harvesting resource | "Collect 3 signal spores" |

---

## DOCUMENT METADATA

- Version: 2.0
- Last Updated: 2026-02-25
- Authors: Cursor Agent (technical) + Story Bible (narrative)
- Status: CURRENT — supersedes v1.0
- Review: Send this file to GPT with prompt: "Review this as a principal engineer. What are the top 5 things most likely to break?"
- Next Action: Execute E0 tasks (see Section 6, Phase E0)
