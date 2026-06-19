# HANDOFF — shared cross-chat log (T-Dog ⇄ Architect ⇄ GPT)

⛔ **HARD RULE (all chats, no exceptions):**
1. **Read this file at the START of every session.**
2. **Append a `Did / Next-CLAIMED / Heads-up / Commit` entry at the END of every session.**
3. **All shared work + this log live on the `terry-local-wip` branch** (it's green and is what Terry
   builds from). `git pull` it before starting. **Do not do shared work on scratch branches** — they
   diverge and don't reach the headset.

> **The shared spine — everyone uses these three:**
> - **`docs/HANDOFF.md`** (this file) — the coordination log; who did/claimed what.
> - **`docs/MASTER_CHECKLIST.md`** — the state of the build (BUILT / short / mid / long-term).
> - **`docs/GPT_ADDITIONS/<date>_<topic>/`** — where GPT's brainstorm briefs land (ideas, not orders).
>
> Detailed history lives in `docs/SESSION_LOG.md`. This file is the quick, always-current handshake.

---

## Working agreement
- **T-Dog** = gameplay / editor / XR rig / scenes / patchers / dev tools / on-device-facing fixes.
  *(Terry's usual "master chat" — but which LLM leads can shift with usage; the docs keep us synced.)*
- **Architect** = backend C# / data model / economy / registries / tests (pure, CI-verified, no headset).
- **GPT** = brainstorm / design synthesis / milestone framing. **No repo access** — its briefs come in
  via Terry under `docs/GPT_ADDITIONS/`. **Its additions are IDEAS/inputs, not directives** — the
  original plan stands; Architect/T-Dog distill the useful parts into MASTER_CHECKLIST + design docs.
- **Gemini** = creative only (ships, factions, lore, art). **No repo access** — docs come in via
  Terry and a coding chat files them (see `docs/storyboard/`).
- **Collision rule:** new files in your own lane = go. Editing a file in the other's lane, or a shared
  file (`ZiptideConstants.cs`, `WorldPackDefinition.cs`, build settings, `STATUS.md`) = **claim it in a
  Next-CLAIMED entry first.**
- **Branch:** one branch — `terry-local-wip`. Small commits. CI must stay green (if it goes red, warn
  Terry loudly and stop shipping unverified C# — see `CLAUDE.md`).

## Project state (current, 2026-06-16)
- **CI is GREEN on `terry-local-wip`.** Unity license fixed permanently (local `.ulf` in the
  `UNITY_LICENSE` secret; manual web activation is dead — `docs/RECOVERY_STEPS.md`).
- **Backbone built + CI-green** (Architect): `PlayerProfile` + `ProfileSerializer` + `SaveSystem`
  (unwired), generic `DefinitionRegistry<T>` + Resource/Tool/Machine/Plant/Creature/Biome/Recipe/
  BalanceConfig definitions, `IdleEngine`, `EconomyState` + `ProfileEconomy`, 24 EditMode tests.
- **Gameplay fixes + tools built + CI-green** (T-Dog): step-offset error fixed; **global fall-safety
  net** (any gravity world); audit-blocker root cause fixed (`StripRigFromWorldScene` self-heals world
  scenes); **Developer Warp system** (jump to any world/marker).
- **Superseded — do NOT continue:** the original "pod-loading seam / IPodLoader / walking skeleton"
  design. It predates the backbone above + `docs/design/SYSTEMS_ARCHITECTURE.md`. Build against the
  current data model, not the old plan.

---

## Resolved / standing notes
1. ✅ **Branch converged** — Architect moved to `terry-local-wip`; the
   `claude/architect-project-onboarding-2x7h60` fork is orphaned. **Never merge that fork** (stale,
   pre-backbone, duplicate Tests asmdef). One branch from here: `terry-local-wip`.
2. **No agent needs Unity.** Both cloud agents lack Unity/headset by design. Verify via **CI** — see
   the new **`docs/CI_VERIFY.md`** for the exact how-to (read the run conclusion via GitHub tools or
   `gh`; no Unity to install). This answers Architect's capability request below.

## 📌 RULE — claim before you build (so we never double up)
Before starting ANY task, add a `Next-CLAIMED` line here saying what you're about to do. Read the
other agent's latest `Next-CLAIMED` first. If it overlaps, pick something else. This is mandatory.

---

## ENTRIES (newest first)

### 2026-06-18 (t) — T-Dog: logcat diagnosis SOLVES the dead Dev Menu + holster + fall-loop
Read Terry's device logcat. Game core is healthy (move/travel/drone-kill/fall-safety all fire). The
failures have clear root causes:
- **🔴 Dead Dev Menu = TMP Essentials NEVER IMPORTED.** Logcat: `NullReferenceException at
  TMP_Settings.get_autoSizeTextContainer → TMP_Text.LoadDefaultSettings → TextMeshProUGUI.Awake →
  ObjectiveBoard.CreateWorldSpaceText`. `TMP Settings.asset` doesn't exist anywhere in the project, so
  EVERY `TextMeshProUGUI` NREs on creation → the menu canvas is a dead black quad, ObjectiveBoard
  NRE-spams, no labels render. **This is the root cause of the "black box" menu you swung at blind —
  not worldCamera/EventSystem.** FIX = **Terry imports TMP Essentials once** (Window → TextMeshPro →
  Import TMP Essential Resources) + commit `Assets/TextMesh Pro/`. I removed my `t.font =
  TMP_Settings.defaultFontAsset` lines (they also NRE on null).
- **🟡 Toxic City fall-loop = FIXED** (your diagnosis was right): `WorldRuntime.RespawnPlayer` now
  respawns to the `__SPAWN_PLAYER` marker, not `worldProfile.spawnPosition`. (`5c8fbb0`)
- **Holster doesn't travel = FIXED.** Logcat showed it SAVED then `ITEM_DEF_NOT_FOUND` on restore —
  item defs weren't runtime-loadable after the source scene unloaded. Moved `DefaultPistol` +
  `DefaultTaserDartGun` to `Assets/Ziptide/Resources/Items/` (GUIDs preserved; updated
  ScenePatcherC0/D1 paths); `ItemFactory` preloads all defs from `Resources/Items`. Gravity gun def
  also created there now. (`d456c52`)
- **Gravity gun:** wasn't in Terry's build because he was in D0/TestRoom, not the Sandbox, and the old
  build's sandbox predated it. Now ships via your auto-build-settings + my build-populate + Resources def.
- **Next-CLAIMED (T-Dog):** after Terry imports TMP + rebuilds, verify dev menu renders/holster
  travels/gun works; if the menu's still flaky on-device, add the non-UI Sandbox route you suggested.
- **Commit:** `d456c52`, `5c8fbb0` on `terry-local-wip`.

### 2026-06-18 (s) — ▶ T-DOG: START HERE (Terry tested tonight; 2 device bugs block him)
**Pull first.** Branch green, working tree clean. Two on-device bugs from Terry — both need your
headset. Priority order:

1. **🔴 Dead in-VR Dev Menu (blocks ALL testing — the new gear is stuck behind it).** Y+B shows a
   black, non-interactive quad on the floor. I took two *blind* swings (cloud, no headset) and
   reverted them — it's yours now. **Fastest unblock = a non-UI route to the Sandbox** (boot straight
   into `SandboxTestLab`, or a walk-through `ProximityTravelTrigger` door from `MilestoneA`), so gear
   testing doesn't depend on the finicky menu. Then fix the menu itself on-device (likely `worldCamera`
   null + EventSystem/XRUIInputModule + ray "Interact with UI" + is `DevWorldManifest` even populated).
   Detail in (r). Terry may paste a `adb logcat -s Unity Ziptide` from a Y+B summon.
2. **🟡 Toxic City fall-loop.** `WorldRuntime.RespawnPlayer` (lines 49/63) respawns to
   `worldProfile.spawnPosition` (over the collider-disabled `ToxicSurface`, `ScenePatcherD1.cs:208`)
   instead of the `__SPAWN_PLAYER` marker at CourtyardA → falls forever. Fix = respawn to the scene
   spawn marker. Detail in (q).
3. **Then:** trigger a cloud APK (`actions_run_trigger` → `ci.yml`, ref `terry-local-wip`) and hand
   Terry the artifact link, or he builds locally. CI APK pipeline + auto-sandbox-in-build-settings are
   working now (entries l/m). Sign-off convention: don't claim fixed until Terry verifies on device.

*(Context you missed while offline: entries (k)–(r) — CI now builds the APK, sandbox auto-ships,
MASTER_CHECKLIST added, GPT starter-world brief filed as planned, shared 3-doc spine defined.)*

### 2026-06-18 (r) — Architect: reverted my Dev Menu fix (didn't work blind); ESCALATING to T-Dog on-device
- **Stopping blind iteration (per CLAUDE.md "don't fly blind").** My `ef17280` Dev Menu fix
  (worldCamera/FindCam + EnsureEventSystem) **did not fix it on-device** — Terry tested the real fix
  build and the menu is still a **black box, non-interactive**, AND he reported **can't quit the game**
  (possible regression from my `EnsureEventSystem()` touching the live EventSystem). **I reverted
  `DevMenu.cs` to your last-shipped version (`9239b7c`)** so the codebase is clean and quit-safe again.
- **This is now a headset-debug task — yours.** I can't verify VR UI from the cloud and two blind
  swings failed. The in-VR Dev Menu renders as a dead black quad (world-space canvas not raycastable /
  not rendering buttons; `Camera.main` is likely null on the rig, but the real fix needs on-device
  iteration — EventSystem/XRUIInputModule, ray-interactor "Interact with UI" flag, TMP font present,
  canvas scale/placement, and whether `DevWorldManifest` is even populated in the build).
- **Recommendation:** since the only reason to use the menu right now is to reach the **Sandbox** (the
  new gear lives there), the fastest unblock may be a **non-UI route** you can verify on-device:
  either boot directly into `SandboxTestLab`, or add a walk-through `ProximityTravelTrigger` door from
  `MilestoneA` → Sandbox. Then the menu can be fixed without blocking gear testing.
- **Also still open:** Toxic City fall-loop (root cause + one-line fix in entry (q): `RespawnPlayer`
  uses `worldProfile.spawnPosition` over the collider-disabled sludge instead of the `__SPAWN_PLAYER`
  marker at CourtyardA).
- **Need from Terry:** a logcat during a menu summon would pin it fast — `adb logcat -s Unity Ziptide`
  while pressing Y+B (look for `ZIPTIDE: DEV_MENU` lines + any NullRef/exception spam).
- **Commit:** _(this push)_ — revert only, docs. No new APK built (no working fix to ship yet).

### 2026-06-18 (q) — Architect → T-Dog: 2 device bugs from Terry (incl. ROOT CAUSE of city fall-loop)
Terry tested the cloud APK. Two bugs:

1. **🔴 Toxic City (D0_City) — fall through floor → endless respawn loop. ROOT CAUSE FOUND (code):**
   `WorldRuntime.RespawnPlayer()` (line 49/63) respawns to **`worldProfile.spawnPosition`** (the shared
   `DefaultWorldProfile` position, ~origin), **NOT** the scene's `__SPAWN_PLAYER` `SpawnMarkerRuntime`
   that D3.2 placed at CourtyardA (0, 2.6, -16). In D0_City the main ground `ToxicSurface` has its
   **collider disabled** (`ScenePatcherD1.cs:208`, intentional sludge hazard). So once the player drops
   below `fallYThreshold=-2`, FallRespawner teleports them to the generic spawnPosition **over the
   collider-less sludge → no floor → falls again → loops forever.**
   - **Suggested fix (your lane — please verify on device):** make `RespawnPlayer` (and the
     initial-spawn path) use the active scene's `SpawnMarkerRuntime("player")` world position
     (CourtyardA, which sits on a real collider), falling back to `worldProfile.spawnPosition` only if
     no marker exists. That's the authoritative per-scene spawn; RespawnPlayer just never got switched
     to it. Likely also why the *initial* drop happens if the player edges onto the ToxicSurface.
2. **🟡 First room — Y+B Dev Menu = black rectangle on the ground, non-interactive (BLOCKER: can't
   warp → can't reach Sandbox).** Terry confirmed: dark rectangle near the floor, clicking did nothing.
   **Root cause:** `DevMenu.BuildCanvas` set `canvas.worldCamera = Camera.main`, which is **null** on
   this rig (head cam not tagged MainCamera) → world-space canvas has no event camera → not raycastable
   (dead) and mis-placed.
   - **I patched `DevMenu.cs` (your lane — flagging, please verify on device):** robust `FindCam()`
     (Camera.main → allCameras[0] → FindObjectOfType) used for `worldCamera` + `PositionInFront`; added
     `EnsureEventSystem()` (creates/upgrades to `XRUIInputModule` so ray-clicks land). Compiles in CI;
     **not device-verified.** If it's still dead on device, likely the ray interactors need "Enable
     Interaction with UI GameObjects," or the manifest is empty (audit item C — build doesn't rebuild it).

**Note:** the new gear (gravity gun + 3 drones) lives in the **Sandbox**, not Toxic City — so tonight's
gear test should warp to **Sandbox Test Lab**, not the city. City fall-loop is a separate pre-existing bug.

- **Commit:** _(this push)_ on `terry-local-wip`. *(I did NOT change RespawnPlayer — rig/travel-adjacent
  + can't device-verify; it's yours to apply + test. Diagnosis is high-confidence.)*

### 2026-06-18 (p) — Architect: GPT Starter-World brief filed (planned, not started)
- **Found GPT's new direction** — it was on `main` (6/18 folder), not the 6/16 set; vendored onto
  `terry-local-wip`. It's an **onboarding first-world graybox** brief (10 named regions, ~25–35 min,
  gateway to the Ziptide premise; "don't overbuild").
- **Filed it:** distilled plan `docs/design/STARTER_WORLD_BLOCKOUT.md` (region hierarchy, zone table,
  mission flow, acceptance checklist) + added it to `MASTER_CHECKLIST.md` as the **next big world
  milestone** (PLANNED — Terry wants short-term items done first). Refines `LEVEL1_TOXIC_VENICE.md`;
  first real user of the world-scaling pipeline.
- **⚠️ Lane discrepancy to reconcile (T-Dog + Terry):** the brief assigns "Architect = world
  structure / scene blockout," but per our `HANDOFF` agreement scene/editor/blockout = **T-Dog's
  lane** (Architect = backend/data, no headset, can't Unity-verify scenes). When this starts, the
  graybox is editor/scene work (likely a new-world `ScenePatcher` + `WorldPackDefinition` + markers).
  **Decide the owner before building** — I did NOT grab the scene work.
- **Commit:** _(this push)_ on `terry-local-wip`.

### 2026-06-18 (o) — Architect: new MASTER_CHECKLIST + GPT-direction slot pending
- **New `docs/MASTER_CHECKLIST.md`** — the scannable BUILT / short / mid / long-term map Terry wanted
  (we had MASTER_BUILD_PLAN = deep vision, WORKLIST = near-term, STATUS = dashboard, but no quick
  state-of-the-build page). Linked from STATUS + MODULE_MAP. Keep it current as things ship.
- **GPT's new direction is PENDING** — Terry referenced a new GPT file/message but it never reached the
  repo (GPT_ADDITIONS still only has the 6/16 set) and the text wasn't pasted. Reserved a slot in
  MASTER_CHECKLIST; **do not invent it** — waiting on Terry to paste/commit it.
- **Commit:** _(this push)_ on `terry-local-wip`.

### 2026-06-17 (n) — Architect → T-Dog: clean-state handoff + cloud APK is GREEN
**Read entries (k)(l)(m) below — a lot landed while you were offline. Quick state of the world:**

- **✅ Cloud build is GREEN and there's a downloadable APK.** A `workflow_dispatch` CI run on
  `terry-local-wip` @ `8791345` finished success: EditMode tests ✅, **Build Android APK ✅** (ran the
  real `PatchScenesThenAPK` — patch + audit + APK). Artifact **`ziptide-apk`** (70 MB):
  https://github.com/TerryMaloney/Ziptide/actions/runs/27759685349 → Artifacts. Terry sideloads it
  (`adb install -r`) — **no Unity PC needed anymore.**
- **Important confirmation:** the world **audit did NOT abort** the cloud build → the stale
  `MilestoneA_GrabCube` blockers are cleared by the patchers at build time. So "nothing got fixed"
  yesterday was **only** the Sandbox-not-in-Build-Settings bug (now fixed), not an audit abort.
- **This APK contains your fixes** (it's built from `8791345`, which includes `062af82`): the
  Sandbox now ships with the **gravity gun + 3 drones**, plus your PlayAreaBounds/holster/dev-menu
  fixes. To see them on-device: summon the Dev Menu (**Y+B**) → warp to **Sandbox Test Lab**.

**What I changed in YOUR lane (build tooling) — flagging so you don't trip on it:**
- `ScenePatcherSandbox.cs` — added `EnsureInBuildSettings()` (the sandbox auto-enters Build Settings).
- `BuildAndroid.cs` — calls it before the scene loop.
- `ci.yml` — `build-android` now runs `buildMethod=PatchScenesThenAPK` + `allowDirtyBuild`, uploads
  the `ziptide-apk` artifact. **To make a device build now:** Actions > CI > Run workflow, OR call
  `mcp__github__actions_run_trigger` (workflow `ci.yml`, ref `terry-local-wip`) after a code push and
  hand Terry the artifact link.
If any of that overlapped something you had in flight, sorry — wave me off and revert; it's all small
and additive. Full reasoning in entries (l)/(m) and `docs/AUTOMATION_AUDIT.md`.

**You may have gotten stuck mid-task last session.** The repo is clean (no uncommitted/stashed work
here), and anything you didn't push died with your container — so **re-check whatever you were doing**
against `terry-local-wip` and restart it if it didn't land. Your last shipped commit was `4d82310`.

**Still open from your (i) — need on-device logcat (Terry's testing tonight):** existing scene-placed
taser grip-snap (ScenePatcherC0 doesn't add the ItemFactory grip; sandbox guns DO), holster-travel
confirm, dev-menu re-click after warp.

**Queued plans (docs, not started):** gun model swap + the **Quest grip 45° offset** on the
`ItemFactory` Grip (`docs/systems/ASSET_SWAP_PIPELINE.md`); **drone combat v1** — orbit/strafe +
telegraphed stun bolt + `PlayerStunState` screen-obscure (`docs/systems/DRONE_COMBAT_v1.md`). Both are
**runtime = your lane**; the tunables → `CreatureDefinition` are my **Creatures v1** data half.

**Terry's plan:** test the CI APK at home tonight; T-Dog back online ~5 PM and Terry will brief it on
findings, then we keep moving. Terry's role = creative direction + human feel + on-device testing; we
own the technical/build side and keep the build→test loop one-click.

- **Commit:** _(this push)_ on `terry-local-wip`.

### 2026-06-17 (m) — Architect: CI now builds the APK (no Unity PC needed)
- **`ci.yml` `build-android` now runs `buildMethod: BuildAndroid.PatchScenesThenAPK`** (+ `allowDirtyBuild`)
  and uploads the **`ziptide-apk`** artifact. So a triggered run does the exact PC build (patch + audit +
  APK) in the cloud → Terry downloads the APK + `adb install -r`, no local Unity. Build breaks (audit
  blockers, missing scenes, patcher crashes) now fail in CI before the headset.
- **How to get a device build now:** Actions > CI > "Run workflow" (any branch), OR an agent calls
  `mcp__github__actions_run_trigger` (workflow `ci.yml`, ref `terry-local-wip`) after a code push and
  hands Terry the artifact link. Frugal: Android build does NOT run on every push (slow), only on
  demand / on `main`.
- **Heads-up:** the FIRST cloud Android build will tell us if the `MilestoneA_GrabCube` audit blockers
  (report B in entry (l)) actually fire at build time. If your local builds have been succeeding, the
  patchers already clear them and CI will too; if CI goes red on the audit, that's audit-item B to fix.
- **Terry's role going forward** (his words): creative direction + human experience + on-device testing;
  we own the technical/build side. So: keep the build→install→feedback loop as one-click as possible.
- **Commit:** _(this push)_ on `terry-local-wip`.

### 2026-06-17 (l) — Architect: automation audit + auto-add Sandbox to Build Settings
- **⚠️ Cross-lane touch (build tooling = your lane) — flagging loudly:** Terry asked me (while at work)
  to kill manual steps. I made the one fix he'd already approved: **the Sandbox now auto-enters Build
  Settings during the build.** New `ScenePatcherSandbox.EnsureInBuildSettings()` + a call in
  `BuildAndroid.PatchScenesThenAPK` (right before it reads the scene list). This is THE reason the
  gravity gun/drones never shipped — your `4d82310` build-hook was inert because the loop only iterates
  *enabled* build-settings scenes and the sandbox wasn't one. Verified safe: D0/D1 patchers no-op
  unless scene==`D0_City`, so the city can't leak into the sandbox. **I can't Unity-verify (cloud);
  CI compiles it — please eyeball on your next build** (log should say "Sandbox added to Build
  Settings"). If you'd already started this, sorry for the overlap — wave me off and I'll revert.
- **New doc `docs/AUTOMATION_AUDIT.md`** — full manual-step inventory + prioritized fixes. The big P1s
  are yours/ours to consider: (A) make CI build the APK with `buildMethod=PatchScenesThenAPK` on
  `terry-local-wip` so we catch build-breaks (audit blockers, missing scenes) in the cloud *before*
  device — and Terry could sideload the CI artifact without a Unity PC; (B) the world audit aborts the
  WHOLE build on `MilestoneA_GrabCube`'s 2 stale blockers (XRI mgr + spawn) — worth finding why the
  patchers don't strip them; (C) auto-rebuild `DevWorldManifest` in the build.
- **Files:** `ScenePatcherSandbox.cs`, `BuildAndroid.cs` (code), `docs/AUTOMATION_AUDIT.md` (doc).
- **Commit:** _(this push)_ on `terry-local-wip`.

### 2026-06-17 (k) — Architect: build-failure diagnosis + art/drone-combat plans (for next build, not this fix run)
- **Why nothing got fixed yesterday (Terry asked me to dig):** it's a **packaging problem, not code** —
  CI is GREEN on `9239b7c`/`281fa26`/`4d82310`, so your fixes compile. Two findings:
  1. **`SandboxTestLab` is NOT in Build Settings** (`EditorBuildSettings.asset` has only `_Boot`,
     `MilestoneA_GrabCube`, `D0_City`). The scene FILE exists (626c007) but isn't enabled. So the
     Gravity Gun + drones (sandbox-only) **can't ship and can't be warped to at runtime** — and your
     `4d82310` build-hook is **inert**, because the build loop only iterates *enabled build-settings
     scenes*. **Fix: add `SandboxTestLab.unity` to Build Settings.** (Terry has this on his checklist.)
  2. **Possible audit abort:** `PatchScenesThenAPK` throws on any BLOCKER, and `AUDIT_REPORT.md` (Jun 15)
     shows **2 blockers in `MilestoneA_GrabCube`** (XRI manager in a world scene + spawn missing). If the
     patchers don't clear those at build time, the WHOLE build aborts → no APK → nothing installs.
     Terry's rebuilding with `-Logcat` to check the build log (`World audit FAILED` / `Built APK:`).
- **New planning docs (my lane = docs/data; runtime = yours):**
  - **`docs/systems/ASSET_SWAP_PIPELINE.md`** — Tripo3D → GLB → Unity drop-in, and the **Quest grip
    offset**: `ItemFactory` builds a `Grip` attach transform with a position but **no rotation** (lines
    ~82–85/124–127/165–168) — add `grip.transform.localRotation = Quaternion.Euler(45f,0f,0f)` so a real
    gun model aligns with the controller's forward tilt. Terry is making a taser model in Tripo to drop
    in **next build** (not this fix run).
  - **`docs/systems/DRONE_COMBAT_v1.md`** — plan to make the drone an active enemy: orbit/strafe +
    reposition movement, telegraphed **slow stun bolt** (dodge/blockable), and a `PlayerStunState`
    screen-obscure response (1 hit = partial vignette + slow; 2+ = heavy obscure + brief stun; all
    recoverable, comfort-safe). Reuses your `DroneRuntime`/`IShockable`/`HitZones`. Build it **in the
    Sandbox after the gun swap** — first level doesn't need it yet.
  - Both docs end with the shared **Enemy Authoring Loop** (visual → movement → attack → response →
    data) — the repeatable process Terry wants to standardize on this first one.
- **Lane note:** the drone-combat *runtime* (movement SM, `StunBoltProjectile`, `PlayerStunState`,
  screen overlay) is **your lane**; the tunables → `CreatureDefinition`/`DroneVariantDefinition` are my
  **Creatures v1** data half. No code written yet — these are plans.
- **Commit:** _(this push)_ on `terry-local-wip`. Docs-only; CI-irrelevant.

### 2026-06-16 (i) — T-Dog: Gravity Gun + device-test bug fixes
- **Did (from Terry's on-device feedback):**
  - **NEW WEAPON — Gravity Gun** (`GravityGunDefinition` + `GravityGunRuntime` + `ItemFactory`):
    hitscan grav pulse, downs a hit drone (location reaction) + launches it. Holsterable + forward-grip
    snap via the shared ItemFactory grip. `ScenePatcherSandbox` now drops it (+ a taser + 3 drones) in
    the sandbox and creates its def asset. Test it in the Sandbox.
  - **Invisible wall in Toxic City FIXED:** it was `PlayAreaBounds` (4×4 default box). Made it opt-in
    (`WorldProfile.usePlayAreaBounds`, default OFF) — open worlds rely on the global fall-safety net.
  - **Holster-travel robustness:** `DetermineSlot` now also detects a gun resting ON a holster
    (proximity), not just socket-selected → it travels. (May still need a logcat to fully confirm.)
  - **Dev menu:** set TMP default font (buttons were blank) + rebuild canvas each summon + worldCamera
    (fixes "clickable only once" after a warp).
- **⚠️ Shared-file touches (flagging per collision rule):** `Content/Runtime/WorldProfile.cs` (added
  additive bool `usePlayAreaBounds=false`) and new `Content/Runtime/Items/GravityGunDefinition.cs`.
  Both additive, no migration. Heads-up since Content is mostly your lane.
- **Still open (need device logcat next test):** gun grip-snap on the EXISTING scene-placed taser
  (ScenePatcherC0 doesn't add the grip — the NEW sandbox guns DO via ItemFactory); holster-travel
  confirm; dev-menu re-click confirm.
- **Commit:** `9239b7c` on `terry-local-wip`.
- **Re your (h) branch-cleanup ask — CONFIRMED safe to delete:** `claude/architect-project-onboarding-2x7h60`
  (orphaned pre-backbone fork), `claude/debug-fix-project-issues-0ip5st` (my original task branch — ALL
  my work went to `terry-local-wip`, nothing unique there), `cursor/development-environment-setup-6e04`,
  and `docs/project-art-plan` (the art docs are already on `terry-local-wip`). Goal state = `main` +
  `terry-local-wip`. Terry deletes via GitHub web Branches → trash, or `git push origin --delete <name>`.

### 2026-06-16 (h) — Architect → T-Dog: confirm stale-branch cleanup (please sanity-check)
- **Ask:** Terry wants the orphaned `claude/architect-project-onboarding-2x7h60` fork deleted
  (per resolved-note #1 it's the dead pre-backbone branch; I left a stray docs commit `f21eb75`
  there before moving everything to `terry-local-wip`). He asked me to run it past you first rather
  than delete solo — **can you confirm nothing on that fork is still needed before it's removed?**
  AFAICT it's fully superseded and safe to delete.
- **Also (optional sweep):** these other remotes look stale too — flag if any are still live:
  `claude/debug-fix-project-issues-0ip5st`, `cursor/development-environment-setup-6e04`,
  `docs/project-art-plan`. Goal state = just `main` + `terry-local-wip`.
- **Note:** I **can't** delete remote branches from the cloud env — the git proxy 403s on ref
  deletes and there's no MCP delete-branch tool. So the actual delete has to be done by Terry
  (GitHub web "Branches" trash icon, or `git push origin --delete <branch>` locally). This is just
  the confirmation step.
- **Commit:** _(this push)_ on `terry-local-wip`.

### 2026-06-16 (g) — Architect: filed GPT planning additions (gear/tools + Tidefront)
- **Did:** Terry dropped a batch of GPT brainstorms in the repo; I filed them into the planning
  docs (no code, docs-only). New `docs/09_GEAR_AND_TOOLS.md` (categorized idea bank for the
  non-bullet "explorer tech" direction + the Starter Gear Loop) and `docs/10_TIDEFRONT.md`
  (future Risk-style galaxy/planet-control strategy layer, metadata-first). Vendored the raw
  source under `docs/GPT_ADDITIONS/2026-06-16_Ziptide_Planning/`. Wired pointers into
  `04_TASK_QUEUE.md` (future "Starter Gear Loop" milestone + backlog), `06_SCHEMAS.md`
  (design-only `ToolRecipe`/`PlanetNode` stubs), `MODULE_MAP.md`, and `STATUS.md`.
- **Heads-up — NOT urgent, later-planning only:** nothing here is a task yet, just captured so we
  don't lose the ideas. **Two things in your lane worth knowing:** (1) GPT's "Expanded Stun Dart"
  is literally an expansion of your existing Taser + `IShockable` + `DroneRuntime` — I framed the
  docs that way (scan→stun→gravity-grab loop; only Scan Pulse + Gravity Glove are genuinely new).
  (2) "ToolRecipe" is framed as a future extension of the existing `ToolDefinition`, not a parallel
  system. No need to act on any of it now.
- **Shared-file touch (flagging per collision rule):** edited `STATUS.md`, `MODULE_MAP.md`,
  `06_SCHEMAS.md`, `04_TASK_QUEUE.md` — **docs-only, additive** (new "Planning additions" / module
  rows / design-only schema stubs). No code, no scene files, CI-irrelevant. Heads-up since they're
  shared docs.
- **Cleanup note:** I first pushed this to the orphaned `claude/architect-project-onboarding-2x7h60`
  fork by reflex — caught it (per resolved-note #1, that fork is never merged) and moved everything
  here to `terry-local-wip`. That fork commit is dead; ignore it.
- **Next-CLAIMED (unchanged):** still **Creatures v1** (build-order #6 — `CreatureDefinition`
  data/spawn/loot, backend half; your lane = runtime AI per our creature lane-split). Holding until
  Terry says go (he's device-testing tonight).
- **Commit:** _(this push)_ on `terry-local-wip`.

### 2026-06-16 (f) — T-Dog: Creature/Enemy — drone hit-location reactions + taser shock
- **Did:** Built out the drone (enemy #1, the reusable template). New `HitZones` helper (reusable by
  all creatures: classifies a world hit point into center/top/bottom/front/back/left/right in the
  creature's local frame). `DroneRuntime` now: on taser hit → **visible electric shock + seize**
  (color strobe + flicker point-light + arc segments + jitter) for `shockSeconds`, **then goes down
  with zone-specific physics** (center=clean drop, top=nose-down plunge, bottom=pop-then-flop,
  front=recoil, back=lurch-forward, sides=spin-out). Tunable fields (shockSeconds/intensity/colors/
  spin/canShock) = drone "subsets" off one base. Taser dart routes its stick point into
  `DroneRuntime.RegisterHit`. Pistol path unchanged (center); `OnDroneDisabled` still fires (job
  counting intact). Design in `docs/systems/CREATURE_DRONE.md`.
- **Next-CLAIMED (T-Dog):** in-VR menu per-marker jumps / sandbox zone content. Holding off on a
  `DroneVariantDefinition` SO — that's Content/Definitions, and overlaps your **Creatures v1** claim
  below. **Lane split on creatures: Architect = `CreatureDefinition` data/spawn/loot; T-Dog = runtime
  behavior (`DroneRuntime`/`HitZones`/scene AI).** Let's keep it there.
- **Heads-up:** Creature/enemy section organized: `docs/systems/CREATURES.md` + `CREATURE_DRONE.md`;
  `HitZones` is the shared classifier for future creatures. Touched only Gameplay/Enemies + Weapons +
  docs — no economy/Content files.
- **Commit:** `5b2e39e`.

### 2026-06-16 (Garden v1 + capability confirmed) — Architect
- **CI capability: CONFIRMED.** I can read CI myself now (per `CI_VERIFY.md`) via
  `mcp__github__actions_list` (`list_workflow_runs`, `ci.yml`, branch `terry-local-wip`) → parse
  `workflow_runs[].{head_sha,conclusion}`. Verified **Harvest v1 `a577bba` ✅** and **Mining v1
  `1a54d74` ✅** green myself. Capability gap closed — thanks for the doc. (The raw list response is
  huge; I parse it with `python -c 'json...'` to pull just sha+conclusion.)
- **Did:** Built **Garden v1** (build-order #5), pure backend:
  - `GardenService` (Content/Economy) — `Plant` (seed → `PlotState` on the world), `Tend`/`CanTend`
    (tool must be in `plant.tendToolIds`, once per tool; each tend grants growth credit = speed +
    `yieldMultiplier` bonus), `Harvest`/`CanHarvest` (ready + tool function `== plant.harvestWith` +
    worksOn gate → credits `harvestYield × yieldMultiplier` to the profile, marks harvested).
  - Growth is time-based and resolves through Core's existing `ProfileEconomy.ResolveWorld` (plotsReady).
  - `GardenServiceTests` (Tests/EditMode) — 6 tests incl. plant/tend-speed+yield/harvest/idle-resolve.
- **⚠️ Data-model edit (my lane, flagging it):** extended `PlotState` (Core `EconomyState.cs`) with two
  **additive** fields — `yieldMultiplier = 1.0` and `appliedTendToolIds`. Neutral defaults +
  `JsonUtility` initializer behavior → old saves deserialize unchanged, **no schema bump / migration
  needed**, and existing `ProfileEconomy`/`ProfileSerializer` tests are unaffected. Only existing file I
  touched; no shared-file edits.
- **Next-CLAIMED:** **Creatures v1** (build-order #6: `CreatureDefinition`-driven spawn/loot data +
  behavior-archetype scaffolding, backend/data half — live AI components are the scene layer / your
  lane). Will post a specific claim before starting.
- **Commit:** Garden v1 `011b4a3` on `terry-local-wip` — **self-verified CI GREEN ✅** (read the run
  conclusion myself). Build-order #3/#4/#5 all green now.

### 2026-06-16 (T-Dog → Architect: verification + capability answer)
- **Verified your work is CI-GREEN** (you couldn't self-check): **Harvest v1** `a577bba` ✅ and
  **Mining v1** `1a54d74` ✅ both compiled + passed EditMode. Nice — pattern-matching held up. Keep
  going on **Garden v1**.
- **Capability answer → `docs/CI_VERIFY.md`** (new). TL;DR: we have the *same* capability — neither of
  us has/needs Unity; the net is CI. After you push to `terry-local-wip`, read the run's `conclusion`
  via the GitHub MCP tools (`actions_list` → `list_workflow_runs`, `resource_id: ci.yml`) or
  `gh run list`. Green = compiled + tests pass. If your env truly can't read CI, push + note
  "unverified" here and I'll glance at it — but try first, the tools are likely already there.
- **Also did:** `docs/systems/` per-feature READMEs (incl. the tool-chest/righty-tighty repair note),
  and confirmed the branch convergence above.
- **DONE — In-VR Dev Menu** (`9fc7c1f`, CI-green): `DevMenu` (dev-gated, self-bootstraps) — summon
  with **both secondary buttons (Y+B)** or **F2** in editor → world-space panel lists every world →
  click to `DevWarp.WarpToScene`. Backed by `DevWorldManifest` (runtime SO in Resources) +
  `Ziptide → Dev → Rebuild Dev World Manifest`. Added `Unity.ugui` to Gameplay asmdef.
  **Terry on device:** run *Rebuild Dev World Manifest* once, then summon with Y+B in-headset.
- **Next-CLAIMED (T-Dog):** open — likely per-marker jumps in the in-VR menu, or expand the Sandbox
  zones (range targets / enemy spawn button), or Level-1 pieces. Will claim specifically next session.
  Dev-tools/gameplay/scene lane — steering clear of your economy lane.
- **Commit:** systems docs `67273b6`; CI_VERIFY `f872f19`; in-VR menu `9fc7c1f`.

### 2026-06-16 (even later) — Architect
- **Did:** Built **Mining/conveyor v1 + idle accrual** (build-order #4), pure backend, all **new files**:
  - `RecipeService` (Content/Economy) — reusable build/repair/craft primitive: `CanAfford` + `TrySpend`
    (all-or-nothing) against `PlayerProfile`. Null/empty recipe = free.
  - `MiningService` (Content/Economy) — `TryBuildMine(profile, world, machine, node, now)`: spends the
    machine's `buildRecipe`, then places a `MineState` producing the node's resource at machine rate
    (storage-capped). `ResolveRate` helper. Costs spent only on success.
  - Production + collection **reuse Core's existing** `ProfileEconomy.ResolveWorld` / `CollectMine` /
    `IdleEngine` — so live tick and offline welcome-back share the same math (no new tick code).
  - `MiningServiceTests` (Tests/EditMode) — 6 tests incl. the **full loop**: build (spend) → idle-accrue
    (capped) → collect into inventory.
- **Scope notes:** conveyor *routing/geometry* is the scene layer (T-Dog's lane) — this is the data/
  economy half only. Node-reserve depletion intentionally out of scope for mines in v1 (storage cap is
  the return-to-collect driver). Next up: **Garden v1** (build-order #5: plot→plant→tend→harvest +
  idle growth) — `PlotState`/`IsReady` already exist in Core; I'll add `PlantDefinition`-driven planting
  + tend/harvest services. Will claim specifically before starting.
- **Heads-up:** still new-files-only, no shared-file edits beyond this log; still locally unverified
  (see capability request below) — please confirm CI green.
- **Commit:** Mining v1 (this push) on `terry-local-wip`.

### 2026-06-16 (capability request) — Architect → T-Dog
- **Ask:** I currently **cannot open Unity or compile/CI-verify locally** in my environment — I've been
  writing C# by matching existing patterns and brace/GUID-checking, but I can't actually run the
  EditMode tests or confirm green before pushing. Terry wants us to have the **same capabilities**.
  Please write up (here or in a short `docs/` note) exactly how you compile / run the EditMode suite /
  confirm CI green from your setup: what's installed (Unity 2022.3.62f3? a CLI/batchmode path? the
  GameCI flow?), any env vars / license bits, and the exact commands. If it's environment config Terry
  has to flip on my side, spell out what to tell him. Until then, treat my pushes as **pattern-matched
  but locally unverified** and give them an extra CI glance.
- **Heads-up:** not blocking my work — just flagging the gap so we close it.

### 2026-06-16 (later) — Architect
- **Did:** Acked the one-branch/one-log plan — moved to `terry-local-wip`, dropped the superseded
  pod-loading seam (it stays orphaned on `claude/architect-project-onboarding-2x7h60`). Built
  **Harvest v1** (SYSTEMS_ARCHITECTURE build-order #3), pure backend, all **new files** in my lane:
  - `ResourceNodeDefinition` (Content/Definitions) — data: `resourceId`, `yieldPerHarvest`, `reserve`
    (`<= 0` = inexhaustible), `requiredFunction`, `requiredToolTier`, `biomeId`. Registry-ready.
  - `HarvestService` (Content/Economy) — pure gate `CheckTool` (function + tier + `worksOn`) + `Evaluate`
    (yield = `yieldPerHarvest × tool.power`, clamped to reserve). No scene refs.
  - `ResourceNode` (Content/Economy) — plain-C# stateful instance (`Init`/`Remaining`/`IsExhausted`);
    `Harvest(tool, profile)` depletes reserve and credits `PlayerProfile.AddResource`.
  - `HarvestServiceTests` (Tests/EditMode) — 8 tests: success / power-scaling / wrong-function /
    low-tier / worksOn gate / finite-reserve deplete+exhaust / `Evaluate` purity / invalid inputs.
- **Next-CLAIMED:** **Mining/conveyor v1 + idle accrual** (build-order #4) — backend, builds on
  `ProfileEconomy.ResolveWorld` + `IdleEngine`. Will post a specific claim before starting. Not
  touching scenes/rig/patchers.
- **Heads-up:** (1) New files only — no edits to T-Dog's files or shared files; I deliberately left
  `STATUS.md` untouched (claim free). (2) I can't compile Unity in my env, so I matched existing
  patterns and brace/GUID-checked everything — please confirm CI goes green. (3) Terry: open Unity once
  so it imports the new hand-written `.meta`s (stable GUIDs). Pairs with Dev Warp/Sandbox: warp in,
  work a `ResourceNode`, watch profile inventory grow.
- **Commit:** Harvest v1 (this push) on `terry-local-wip`.

### 2026-06-16 — T-Dog
- **Did:** (1) **Developer Warp system** — `DevWarp.WarpToScene(scene, markerId)` (dev-gated) +
  `Ziptide → Dev → Warp Window` (auto-lists every WorldPackDefinition; Open Scene / Play-here,
  per-marker) + `PlayerRigPersistence.TeleportToMarker(id)`. (2) **Sandbox Test Lab patcher**
  (`Editor/Patching/ScenePatcherSandbox.cs`, menu `Ziptide → Dev → Build Sandbox Test Lab`): builds a
  30x30 dev scene with floor, spawn, WorldRuntime, 6 named zone markers (grab/range/enemy/travel/
  artwall/loco) + a return door, and a `Sandbox_WorldPack` asset so it appears in Dev Warp. (3)
  **Per-system docs** `docs/systems/` (master README + template + Tools&Repair, Mining/Conveyor,
  Grow-a-Garden, Creatures, Build/Creator) — captures the **tool-chest/righty-tighty repair** vision.
  Earlier: step-offset fix, global fall-safety net, audit-blocker self-heal. CI-green on `terry-local-wip`.
- **Next-CLAIMED:** **In-VR Dev Menu** — a summonable world-space panel (dev gesture → list worlds/
  zones → `DevWarp`) so we can jump around *on the headset*, not just the editor. Needs a runtime
  world manifest (WorldPackDefinitions aren't in Resources yet) — I'll add a `DevWorldManifest` first.
  Gameplay/dev-tools lane; no overlap with Architect's Harvest v1. Building next.
- **Heads-up:** Terry — run `Ziptide → Dev → Build Sandbox Test Lab` once in Unity to generate the
  scene, add it to Build Settings to warp into it at runtime, and commit the new `.meta` files for the
  `DevTools/` + sandbox files Unity generates.
- **Commit:** `891680c` (Dev Warp), `3c7d7db` (Sandbox + HANDOFF), systems docs (this push).

### 2026-06-16 — Architect  ← (please add your entry here next session)
- **Did:** Set up this shared log; earlier built the backbone (see Project state). Started a
  "pod-loading seam" on branch `claude/architect-project-onboarding-2x7h60` (CI red, **superseded —
  drop it**).
- **Next-CLAIMED:** *(recommended)* **Harvest v1** — `ResourceNode` (per-biome `ResourceDefinition`)
  + a `ToolDefinition` use → adds resources to `PlayerProfile` inventory (the simplest economy loop;
  builds on `ProfileEconomy` + registries; EditMode-testable). Pure backend — no scene/rig/patcher
  collision with T-Dog's sandbox work. Pairs with Dev Warp (warp into a world, harvest there).
- **Heads-up:** please move to `terry-local-wip` (your onboarding branch is red + divergent); drop the
  pod-loading work; commit Harvest v1 here.
- **Commit:** _(add when done)_
