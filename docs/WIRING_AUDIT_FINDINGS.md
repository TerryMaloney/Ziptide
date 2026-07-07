# WIRING AUDIT — FINDINGS LEDGER

**The both-sides proof for every seam** (companion to `WIRING_MAP.md`). Read-only audit, no code changed.
Each seam is ✅ wired both sides · ⚠ one-sided gap (queued fix) · 🔵 intentional stub (with the note that
proves it's deliberate). **Audited 2026-07-06 (Picasso/Fable 5) on `terry-local-wip`.**

**Headline:** the codebase is in good wiring health. No CRITICAL one-sided gaps found. The build-hook
chain, rig-ensure chain, forge/creature registries, appliers, and the A6 transport are all wired both
sides. All open items are either intentional stubs (🔵, boarded) or low-severity polish (⚠-minor).

---

## ✅ Verified wired (both sides present, with evidence)

- **A. Build-hook chain** — all **17** asset producers are called in `BuildAndroid.PatchScenesThenAPK`
  (cross-checked every `EnsureAllAuthored/AssignAll/BakeAll/Rebuild` in `Editor/` against the call list).
  *This is the seam that historically broke the most ("nothing got fixed" = Sandbox not in build settings /
  author not hooked). It is currently clean.*
- **B. Rig-ensure chain** — 7 ensures + XRI + input, all on the persistent rig
  (`PlayerRigPersistence.Awake` → Ensure* chain, lines ~126–198). Matches the `_Boot`-owns-all contract.
- **C. Item registry** — 5 guns mapped to shipped recipes (`ForgeAuthor` pairs → `Resources/Forge/*`):
  taser→taser_gun_mk1, pistol→pistol_scrap_mk1, StaticNet→static_net_lobber, SonicThumper→sonic_thumper_maul,
  PrismBeam→prism_beam_rifle. All 5 recipe assets present.
- **C. Creature registry** — 10 CreatureDefinitions in `Resources/Enemies`; `CityBuilder.MakeCreature`
  handles 5 by explicit case + archetype fallback for the rest. `tendril` (once suspected orphan) IS
  referenced (`PvpModes.CreatureCycle` + `CreatureVariantAuthor`).
- **D. Applier→consumer** — ForgeVisualApplier←ItemFactory; ForgeCreatureVisualApplier←CreatureBehaviorBase.Awake;
  ForgeCreatureAnimator.Bind←the applier. Every produced look has a caller.
- **E. Transport (A6)** — `NetBootstrap` sets `PvpNetHub.OnlineStarter`; `PvpNetHub.StartOnline` invokes it;
  `PhotonPvpLauncher.OnJoinedRoom`→`SetTransport`; `ArenaLobbyBoard` GO ONLINE calls StartOnline + ensures
  `PvpOnlinePresence`; presence subscribes to `Active.OnPose`. Both sides confirmed + covered by `PvpNetTests`.
- **H. Scene/boot** — boot = `W000_DriftIn`, guarded by `BootConfigTests`; 24 enabled scenes; every
  destination is enabled. *(This is where tonight's stranded-boot bug lived; now fixed + test-pinned.)*

## 🔵 Intentional stubs / envelopes (deliberate — do NOT "fix" without intent)
- **8 of 10 creatures use primitive `BuildVisuals`** (only `swarm_bug`, `light_grazer` have genomes). The
  P3 pipeline is complete; authoring the other 8 genomes is boarded successor work (HANDOFF tttt: "one
  Build* method in ForgeBodyLibrary").
- **Melee pair (BreakerBlade, TidePike) + Sandbox_GravityGun have no forge recipe** → primitive look by
  design. Forge recipes are future polish, not a wiring gap.
- **`tox_canal_stalker_01` recipe** is a FORGE-R4 proof artifact, referenced only by `ForgeRecipeLibrary`
  (not assigned to a creature — creatures use genomes now). Harmless; renders in the photo booth. Could be
  retired later.
- **Story flags for W013+** are authored (RillLineAuthor beats) but not yet granted — they fire when those
  worlds ship. `FlightModel` free-flight, PUNCH-IT fuel-cell arming, destruction v2, A6.2 combat sync,
  ADS/reload rows: all boarded envelopes, not gaps.
- **PerfBudget / WorldContent / Reachability audit rules are WARN-only** on purpose (can't baseline scene
  counts without Unity; a mistuned cap must not brick the build). Promotion is the Phase-2 item below.

## ⚠ Minor gaps (low severity — queued, not fixed here)
1. **Story-flag both-sides not exhaustively proven** (45 grants / 48 consumes — balanced, but a per-flag
   grant↔consume matrix wasn't built). *Fix:* the Phase-2 WiringValidator should enumerate `ZiptideFlags`
   and flag any constant with 0 grant OR 0 consume (excluding the known W013+ future set).
2. **`tox_canal_stalker_01` orphan recipe** — cosmetically stale (see 🔵). *Fix:* retire the recipe or note
   it "photo-booth demo only" in `ForgeRecipeLibrary` (1-line comment; do in a later cleanup, not now).
3. **DevMenu (TMP) left in place but unreliable on device** — superseded by DevWarpBoard (Terry's call).
   Not a wiring gap; flagged so no one assumes the TMP menu works.

*No ⚠ item is a build-breaker or a functional both-sides break. The high-risk seams are clean.*

---

## RECOMMENDED SAFEGUARDS → Phase 2 (SEPARATE approval; this is where "change without breaking" gets enforced)
Turn this audit into automatic guards so the one-sided-seam class can't recur:
1. **Promote the WARN-only gates to BLOCKER** once baselined: `WorldContentAuditRules`, `PerfBudgetAuditRules`,
   `WorldReachabilityAuditRules`. (Run one clean audit to capture baselines, then flip WARN→BLOCKER.)
2. **Extend `Editor/Validation/DependencyValidator` into a both-sides `WiringValidator`** that fails CI when:
   - an `*Author/*Library/*Baker` defining `EnsureAllAuthored/AssignAll/BakeAll` is NOT called in
     `BuildAndroid.PatchScenesThenAPK` (build-hook orphan);
   - an `ItemDefinition.forgeRecipeId` / `creatureId` / bot-profile id does not resolve to a shipped asset;
   - a `ForgeCreatureBody`/recipe/genome has no runtime consumer path;
   - a `ZiptideFlags` constant has 0 grant or 0 consume (minus the whitelisted W013+ future set).
3. **PR "both-sides checklist" template** — 4 boxes: producer hooked? consumer by-id? verifier (log/test/
   gate)? map+HOW_TO row added? (Mirrors `WIRING_MAP` Part 4.)
4. **Refresh cadence:** re-run this audit at each milestone close; the `WiringValidator` makes most of it
   continuous, so the manual pass shrinks to reviewing new seams.
