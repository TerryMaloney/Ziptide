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

## SAFEGUARDS — Phase 2 (STATUS: ✅ SHIPPED the automatic gate; gate-promotion deferred by design)

**✅ SHIPPED — `Editor/Validation/WiringValidator.cs` + `Tests/EditMode/WiringValidatorTests.cs`** (runs
every CI push, fails the build on a one-sided seam — the enforcement Terry asked for). It checks the
deterministic, asset-based seams:
- **Build-hook completeness** — every Editor type with a public-static `EnsureAllAuthored/EnsureAuthored/
  AssignAll/BakeAll` is referenced in `BuildAndroid.PatchScenesThenAPK` (reflection + source scan).
- **Item→recipe** — every `ItemDefinition.forgeRecipeId` resolves to a `Resources/Forge/<id>` recipe.
- **Genome→creature** — every `ForgeCreatureBody` has a matching `Resources/Enemies/<id>` CreatureDefinition.
Run manually via menu **Ziptide → Validate wiring**. Extend it with new deterministic checks (add a method,
call it in `Validate`).

**⏳ DEFERRED (needs a device/Unity baseline — correctly NOT flipped blind):**
- **Promote WARN gates → BLOCKER** (`WorldContent`, `PerfBudget`, `Reachability`). These need a clean audit
  on the REAL scenes to capture baselines; flipping them without that risks bricking every build (the exact
  reason they're WARN). Procedure: Terry runs the world audit once → confirm 0 warnings → flip each gate's
  severity. The WiringValidator already covers the *asset-wiring* class of check safely in CI, so this
  deferral loses no wiring coverage — it's a perf/navigation concern, not a one-sided-seam concern.

**NOT NEEDED (this repo pushes direct to `terry-local-wip`, no PR flow):** a PR-template checklist — the
both-sides law lives in `WIRING_MAP.md` Part 4 instead.

**Queued future checks for the validator** (deterministic, add when useful): `creatureId`/bot-profile id
resolution; `ZiptideFlags` grant↔consume coverage (minus the whitelisted W013+ future set).

**Refresh cadence:** the validator makes the wiring pass continuous; re-run the full manual audit only at
milestone closes to catch non-deterministic seams (flags, scene content).
