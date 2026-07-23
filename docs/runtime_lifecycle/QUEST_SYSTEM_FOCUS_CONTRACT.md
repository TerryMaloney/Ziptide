# QUEST SYSTEM-FOCUS LIFECYCLE CONTRACT

**Lane:** Reasonbox (Fable 5), per `gpt-high-value-lanes`, 2026-07-23. **Docs + validator only — no runtime edits.** Machine-readable twin: `quest_system_focus_contract.json` (validated by `tools/quest_lifecycle_gate.py`; the JSON is the enforced truth, this file is the reasoning).
**Purpose:** one canonical owner and one exact state machine for every way the OS, headset, or player can interrupt the game — so doff/overlay/tracking-loss behavior is DESIGNED, not inherited from Unity defaults. Feeds VRC Functional.2 (Universal Menu focus) and Input.4, and `DEVICE_TEST_CHECKLIST.md` §10's interrupt matrix (rb100), which currently tests behavior nobody has specified.

---

## 1. SOURCE AUDIT — what exists today (verified 2026-07-23)

| Seam | File (exact) | Current lifecycle behavior |
|---|---|---|
| Save | `Gameplay/Runtime/Persistence/SaveSystem.cs:116-117` | **The only lifecycle-aware code in the game:** `OnApplicationPause(true) → Save()`, `OnApplicationQuit → Save()`. `Save()` is atomic (tmp → swap, `.bak` demotion — never half-written). `AutosaveNow(reason)` static guarded helper for hot paths. |
| Travel | `Gameplay/Runtime/World/TravelCoordinator.cs:31` | `IsTraveling` latch; `PlayerRigPersistence` skips wiring/restore during travel; async activation hold with 20 s never-wedge; 5 s XRI-ready wait with proceed-anyway. No pause/focus interaction — relies on Unity's whole-loop pause semantics. |
| Input session | `Gameplay/Runtime/Player/PlayerInputSessionGuard.cs` | Subordinate enforcement of `PlayerRigPersistence`'s canonical input session: one persistent `XRInteractionManager` + one `InputActionManager`; disables scene-authored duplicates after transfer; runs post-scene-load. **No suppress/resume API** — there is no way to mute gameplay input without tearing down the session. |
| Audio | `Gameplay/Runtime/Audio/AudioDirector.cs`, `AmbienceDirector.cs` | No focus/pause handling. `AudioListener.pause` unused anywhere. Overlay behavior today = whatever the OS does. |
| Haptics | 6 call sites (`BeltTileItem`, `BeltConductorRuntime`, `BeltBlueprintWandItem`, `CameraRuntime`, `WorldDiscoveryNodeRuntime`, `ZiptideGateEffect`) | Fire-and-forget impulses only; no sustained loops found. No suppression policy. |
| Repair/coupler | `Gameplay/Runtime/Story/RepairableMachine.cs:29-47` | `RepairStage` (`Panel→…→Part→Running`) is **runtime session state, not persisted**; `IsRepaired = _stage == Running`; `StageChanged` event exists. Job STEP completion flags persist via profile. |
| Jobs/economy | `WorldJobLibrary` steps → `PlayerProfile.flags` / `RewardRouter` ledger | Flag/ledger writes are in-memory until the next `Save()` — the pause-save is what makes progress durable across doff. |
| Wall-clock systems | `GardenPlotRuntime`, `MiningRigRuntime`, `BeltMinePortRuntime`, `EcologyDirector`, `WorldRuntime`, `PvpProgressionRuntime`, `ArenaLobbyBoard` (+timestamp uses in `SaveSystem`, `PlayerRigPersistence`, `PhotoCaptureCamera`) | Advance on `UtcNow` **by design** (idle-economy law: the world lives while the headset is off). None of these must pause. |
| Simulation pause | — | `Time.timeScale` unused project-wide. There is NO pause mechanism; during focus-loss-while-rendering the game keeps simulating. |
| XR presence/tracking | `Core/Runtime/VRBootDiagnostics.cs:68`, `Core/Runtime/DebugHUD.cs:79` (display listing only) | **No `userPresence` subscription, no tracking-state monitoring anywhere.** HeadsetRemoved is currently only whatever `OnApplicationPause` the OS delivers; TrackingLost is completely unhandled. |
| Network | Photon (recovery-gated `RecoveryFeatureId.NetBootstrap`); offline-first game | No lifecycle interaction; multiplayer excluded from candidate builds. |

**Headline:** the game has exactly ONE deliberate lifecycle behavior (save-on-pause) and it is correct. Everything else is Unity/OS default. Two of the six contract states (`SystemOverlay`, `TrackingLost`) have **zero handling** — and those are the two where the app keeps running.

## 2. THE CANONICAL OWNER

**One new owner: `SystemFocusLifecycle`** (Gameplay/Runtime/Player beside the rig owners; persistent via the existing `_Boot` ensure path — NO new bootstrap pattern beyond the established ensure idiom). It is the single subscriber to `Application.focusChanged`, `OnApplicationPause`, `XRInputSubsystem` tracking state, and (if available on-device) OpenXR user-presence; it derives exactly one `LifecycleState` and broadcasts `StateChanged(prev, next)`. Consumers (audio duck, input suppression, haptics, HUD badge) SUBSCRIBE — they never read Unity callbacks directly again.

**Preserved owners (unchanged duties):** `SaveSystem` keeps its own `OnApplicationPause` save — it is the last-resort safety net and stays even after the owner exists (idempotent, atomic, proven). `PlayerRigPersistence` keeps the input session; the owner REQUESTS suppression via a new small API on the session owner (dependency D1, cross-lane). `TravelCoordinator`, `BootLoader` untouched. This adds one owner for a responsibility that verifiably has none — the same justification class as `FirstHourDirector`.

## 3. THE STATE MACHINE (six states; exact signals; JSON is normative)

```
            focus=false, pause=false                pause=true
  ACTIVE ─────────────────────────► SYSTEM_OVERLAY ─────────► HEADSET_REMOVED/BACKGROUNDED
    │  ▲                                  │  ▲                        │
    │  └──── RESUMING ◄───────────────────┘  └── pause=true ──────────┘
    │            ▲                    (focus=true)      (pause=false → RESUMING)
    │            │
    └── trackingValid=false ──► TRACKING_LOST ── trackingValid=true ──► RESUMING
```

- **Active** — everything normal. Entry only via Resuming's completed checklist.
- **SystemOverlay** — `focusChanged(false)` while not paused (Universal Menu, Guardian setup, notifications; **rendering and the player loop CONTINUE**). This is today's most dangerous gap: creatures keep acting, machines keep interacting, controller input may leak.
- **HeadsetRemoved** — `OnApplicationPause(true)` (Quest proximity sensor → OS pause; the player loop halts, coroutines freeze, audio stops at OS level). Almost everything here is enforced by the OS; our obligations are the ordered pre-pause instant (save) and the resume path.
- **Backgrounded** — pause with intent-to-return-unknown (OS app switch / long doff). On Quest, indistinguishable from HeadsetRemoved at the signal level; modeled separately because resume policy differs (stale-time classification, §5 timers) and other platforms distinguish them.
- **TrackingLost** — 6DoF invalid (covered sensors, darkness, boundary confusion) while the app runs. Comfort-critical: the world must NOT swim.
- **Resuming** — the ONLY path back to Active; an ordered checklist, not an instant (the rb36 travel-arrival input race proved re-entry ordering is a real defect class).

## 4. BEHAVIOR CONTRACT PER STATE (normative; JSON `states[*].behaviors` mirrors this)

> **ADJUDICATED 2026-07-23 (Terry delegated the call):** ① **Overlay simulation CONTINUES** —
> non-lethal canon caps the harm, pausing would freeze travel coroutines mid-crest, and input
> suppression removes exploitation. **Named tripwire:** if device testing shows creatures
> harassing a menu-docked player (stun/shove while in the Universal Menu), escalate to
> "hostile actions targeting the player are held during SystemOverlay" — a bounded follow-up,
> not a redesign. ② **Audio DUCKS (−10 dB), never mutes** — presence under the overlay is
> comforting, the OS already attenuates, and hard mute/unmute pops are worse than quiet
> continuity. Both decisions ride the MISS_LEDGER if the tripwire fires.

| Axis | Active | SystemOverlay | HeadsetRemoved / Backgrounded | TrackingLost | Resuming |
|---|---|---|---|---|---|
| **Save** | autosave on travel + flag grants (existing) | `AutosaveNow("system_overlay")` on entry | `SaveSystem.OnApplicationPause` fires (existing, KEEP); owner adds nothing — double save is harmless/atomic | none required | none (verify SAVE_OK was logged; never re-save during checklist) |
| **Simulation** | runs | **continues** (v1 decision: non-lethal canon makes overlay-time simulation safe; creatures can't kill; revisit only with device evidence) | OS-halted (coroutines/Update frozen — free correctness) | continues (player is present, just untracked) | runs |
| **Audio** | mix per `docs/audio` plan | duck Master −10 dB via mixer param (rails row R1 dependency D2); no mute — presence is comforting under the overlay | OS silences; on resume NO catch-up burst: one-shots scheduled while paused are dropped, not queued | unchanged | unduck over 400 ms AFTER input re-enabled |
| **Rendering** | normal | continues (OS composites overlay above) | OS-owned | **comfort fade to neutral within 250 ms** (existing vignette/fade family), hold; never render a swimming world | fade back in ≤ 500 ms as the LAST checklist step |
| **Controllers/hands** | normal | hide ray/hand visuals (they poke through overlays confusingly) | n/a | show controller models if trackable, hide hands | restore visuals before fade-in |
| **Gameplay input** | enabled | **suppressed** (gameplay action maps disabled via D1 API; system/recenter untouched) | OS-blocked | suppressed except recenter | re-enable ONLY after input session re-consolidation (PlayerInputSessionGuard pass) — the rb36 class |
| **Haptics** | normal | **zero impulses** (owner-gated at the 6 call sites via one static predicate — slice S3) | OS-blocked | zero | zero until Active |
| **Timers** | wall-clock systems advance (BY DESIGN: gardens/mining/ecology/pvp-seasons); session timers (repair interactions, telegraphs, tween coroutines) run | both continue (v1) | wall-clock advances (the FEATURE: offline progress); session timers frozen by OS — correct | both continue | wall-clock deltas resolve via each system's existing catch-up (they already do this on scene load); no special code |
| **Travel/coupler/job atomicity** | `IsTraveling` latch guards re-entry | entering overlay mid-travel: coroutine continues (loop runs); crest already covers visuals — acceptable | pause mid-travel: coroutine freezes inside the crest, resumes intact (Unity semantics); pause-save during travel is safe because profile scene/position writes happen post-arrival (verify in S1 test). Coupler: `RepairStage` survives pause (memory intact); process DEATH resets stage but persisted job-step flags survive — **decided and accepted**: the machine re-presents at `Panel` stage, completed JOB steps never regress | no interaction | Resuming during `IsTraveling` defers checklist completion until travel's own XRI-ready wait finishes — one ordering, not two |
| **Network/offline** | offline-first; Photon recovery-gated OFF | no new obligations; any future session layer must treat all six states as disconnect-safe (dependency note for the multiplayer lane) | same | same | same |

## 5. DISPLACED/DUPLICATE-OWNERSHIP RISKS (found by this audit)

1. **`SaveSystem`'s pause hook vs the new owner** — NOT a duplicate: layered defense, both idempotent+atomic. Contract records it as intentional double-coverage. Risk only if the owner ever *reorders* saving after suppression work — the JSON pins save as first-on-entry.
2. **Input re-enable racing scene wiring** — the rb36 class. Two re-entry paths exist (scene load → `PlayerInputSessionGuard`; resume → owner). The contract serializes them: resume checklist CALLS the guard's consolidation rather than duplicating it. D1's API must live on the session owner's side to keep one owner.
3. **Future pause menu** (benchmark SS lane) will want SystemOverlay-like suppression — it must CONSUME this owner's state (a `MenuOpen` synthetic input to the same suppression path), not grow a second suppression system. Recorded as a forward dependency for that lane.
4. **`ComfortVignette` vs TrackingLost fade** — same visual family; the fade must reuse the existing comfort fade surface, not add a second full-screen effect owner.
5. **Haptics call sites are unowned** — six independent `SendHapticImpulse` sites; the gating predicate (S3) is the minimal single chokepoint short of a full haptics service (which stays out of scope; EXCELLENCE_MAP gap #7 owns that future).

## 6. ORDERED IMPLEMENTATION SLICES (post-adjudication; each is one bounded PR a lower-cost model can execute from this doc + JSON)

- **S1 — The owner + state derivation + logging (no behavior changes).** New `SystemFocusLifecycle` class; subscribes signals; derives states; logs `ZIPTIDE: LIFECYCLE state=<prev>-><next> reason=<signal>`; `AutosaveNow("system_overlay")` on overlay entry (the one behavior safe to ship immediately). EditMode: state-derivation table tests (signal sequences → expected states, incl. overlay-during-travel, pause-during-overlay). PlayMode: scripted focus/pause pulses on the golden route assert the log sequence + a SAVE_OK on overlay. Device: checklist §10 doff/overlay rows now have expected log lines.
- **S2 — Input suppression (D1).** Suppress/resume API on the input-session owner (`PlayerRigPersistence`/guard side, cross-lane file — this slice is the recorded dependency handoff); owner drives it per §4. PlayMode: input events during simulated overlay do not reach gameplay actions; resume order asserts guard-consolidation-before-enable. Device: §10 Universal-Menu row — no grabs/fires leak through the overlay.
- **S3 — Haptics + audio duck + visuals.** One static gate predicate consumed by the 6 haptic sites; mixer duck param (depends on audio rails R1 — recorded dependency D2, no ordering conflict: S3 lands after audio P0); ray/hand visual hide. Device: overlay entry audibly ducks; zero buzzes while docked in menu.
- **S4 — TrackingLost.** `XRInputSubsystem.trackingOriginUpdated`/device-validity polling (0.5 s cadence, the InteriorCull idiom); comfort fade reuse; recenter-only input. Device: cover the sensors — world fades, no swim, recovery clean.
- **S5 — Resume checklist unification.** Formal `Resuming` state gating re-entry (S1 logged it; S5 enforces ordering: guard pass → visuals → audio → input → Active); travel-deferral rule. PlayMode: resume-during-travel scenario; the rb36 regression suite extends here. Device: §10 doff-mid-travel and doff-mid-repair rows.
- **S6 — Contract promotion.** `quest_system_focus_contract.json` gains `implementedBy` fields per state as slices land; the gate then requires them (ratchet, WARN→BLOCK pattern), making contract-vs-source drift a CI failure.

## 7. ACCEPTANCE EVIDENCE MAP

- **EditMode:** S1 derivation tables; gate-predicate truth tables (S3).
- **PlayMode:** focus/pause pulse scripts on the golden route (overlay save, suppression, resume ordering, resume-during-travel); extends the existing 43-test suite's evidence classes, path-bound so lifecycle files trigger it.
- **Device (authoritative):** `DEVICE_TEST_CHECKLIST.md` §10 rows map 1:1 to states — each row now has expected `ZIPTIDE: LIFECYCLE` lines; every ❌ = MISS_LEDGER five-field entry.
- **Validator (this lane, shipped now):** `tools/quest_lifecycle_gate.py` + `tools/tests/test_quest_lifecycle_gate.py` — enforces the JSON's objective truth: all six states present, every behavior axis specified per state, every transition names valid states and a real Unity/XR signal, every referenced source path exists in the repo (auditor-verified paths can't rot silently), slices ordered and dependency-named. Mutation tests prove each enforcement bites. The test file matches the `test_*_gate.py` discovery pattern, so the existing CI continuity job runs it with **zero workflow edits**.

## 8. DEPENDENCIES RECORDED (not edited — other lanes own the files)

- **D1:** suppress/resume API on the canonical input session (PlayerRigPersistence/guard side) — required by S2.
- **D2:** `ZiptideMix` mixer + duck param — audio lane P0 rails (`docs/audio/AUDIO_PRODUCTION_MASTER_PLAN.md` §3) — required by S3's duck only.
- **D3:** future pause menu (benchmark ship-shell lane) must consume this owner's suppression path (§5.3).
- **D4:** any multiplayer resume semantics route through the same states (§4 network row).
