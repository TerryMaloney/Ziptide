# ZIPTIDE RECOVERY AND VERIFICATION SYSTEM

**Audience:** Terry, future ChatGPT/Claude/Codex operators, and any developer taking over the repository.  
**Branch of record:** `terry-local-wip`  
**Unity editor:** `2022.3.62f3`  
**Purpose:** explain what the verification system does, where its evidence lives, which workflow to run, and when a Quest build is actually authorized.

---

## 1. The simple mental model

The system has three layers:

1. **Fast development checks** catch ordinary compile, test, contract, and scene-patching failures.
2. **Recovery route checks** launch the real game flow in Unity, interact with it using simulated VR input, travel through the real world/save systems, and inspect what the player would see.
3. **Quest checkpoint** verifies only the claims a Linux runner cannot prove: physical tracking, controller feel, comfort, device rendering, Android/Quest behavior, and actual headset performance.

The tests are guardrails. They are not the game and should not become the day-to-day development process.

Normal gameplay/content work should remain straightforward:

1. make a bounded feature change;
2. run focused tests and ordinary CI;
3. use the normal PlayMode route only when the change touches its route or owners;
4. use the clean package proof only for Unity/XR/package or foundational lifecycle changes;
5. use the headset for player-facing feel and device-only claims.

---

## 2. Read these files in this order

A new operator should start here:

1. `docs/HANDOFF.md` — current micro-step and operator handoff.
2. `docs/recovery/RECOVERY_VERIFICATION_SYSTEM.md` — this guide.
3. `docs/CI_VERDICT.md` — latest ordinary CI verdict.
4. `docs/recovery/generated/recovery_playmode_observation.md` — latest normal PlayMode result.
5. `docs/recovery/generated/recovery_clean_package_observation.md` — latest clean-package result, when applicable.
6. `docs/recovery/generated/recovery_golden_android_observation.md` — latest locked Android build result.
7. `docs/recovery/QUEST_GOLDEN_CHECKPOINT.md` — exact headset procedure and authorization table.
8. `docs/PROJECT_COMPLETION_ROADMAP.md` — project-wide path after recovery.

Do not start by reading old PR titles or `main`. Old recovery PRs may remain visible but be superseded. The current source of truth is the branch and the exact-SHA evidence above.

---

## 3. Source commits versus evidence commits

A **source candidate** is the exact commit containing runtime, test, workflow, package, scene, asset, or configuration changes being judged.

After the workflows finish, bots may add commits such as:

- `ci: record verdict ... [skip ci]`
- `ci(playmode): record observation ... [skip ci]`
- `ci(recovery): record clean package proof ... [skip ci]`
- `ci(recovery): record Golden Android observation ... [skip ci]`
- generated recovery report refreshes.

Those commits record evidence. They do not become a new runtime candidate unless they modify proof-relevant source.

When deciding whether an observation is current:

- compare its `Tested SHA` to the source candidate;
- allow later changes only under generated evidence, `docs/CI_VERDICT.md`, or `docs/HANDOFF.md`;
- any later runtime, package, scene, test, workflow, asset, or project-setting change requires a new proof result.

Never say “the branch is green” without naming the tested source SHA.

---

## 4. Canonical runtime owners

These ownership rules prevent the code collisions that started recovery:

- `TravelCoordinator` owns all scene/world travel.
- `PlayerRigPersistence` owns the persistent XR rig and physical input-session integration.
- `PlayerInputSessionGuard` consolidates duplicate input managers and input-session ownership.
- `SaveSystem` owns player profile and disk persistence.
- `BootLoader` owns cold boot and Home Hub entry.
- the recovery exposure profile controls which automatic systems are allowed in a locked Golden checkpoint.

Feature systems may request or observe these owners. They must not create competing travel, rig, input, save, or boot implementations.

For scalable content, preserve the pattern:

**definition → registry → factory → runtime owner → audit/test**

That lets worlds, weapons, creatures, plants, machines, vehicles, and story content expand without hard-wiring every instance into scene code.

---

## 5. Workflow map

### A. `CI`

File: `.github/workflows/ci.yml`

Use for ordinary source changes.

Required jobs:

- Unity EditMode tests;
- patch scenes + world audit.

The ordinary Android job is normally skipped on `terry-local-wip`; that is expected. The locked Golden workflow builds the checkpoint APK.

Durable result:

- `docs/CI_VERDICT.md`

Important rule: the project-contract report job is informational unless explicitly promoted. The generated verdict must not disguise a required Unity failure.

### B. `Recovery Contract Scan`

File: `.github/workflows/recovery-contract-scan.yml`

This is the static repository/architecture lane. It checks ownership contracts, inventories, manifests, continuity rules, first-hour bindings, and report-generation tools.

It does not prove runtime behavior or headset quality.

Generated evidence lives under:

- `docs/recovery/generated/`

### C. `Recovery PlayMode Observation`

File: `.github/workflows/recovery-playmode.yml`

This is the normal runtime regression lane. It uses a package-compatible Unity Library cache and runs the canonical PlayMode suite.

The route currently proves, among other things:

- real boot and Home Hub readiness;
- simulated VR selection of NEW GAME;
- W000 arrival;
- W000 → ToxicCity → W000 travel through `TravelCoordinator`;
- save/profile round trip;
- persistent rig and input ownership;
- snapshots from the real player camera;
- UI spatial/readability checks;
- spawn clearance;
- runtime-owner census and duplicate detection;
- fallback/material visibility;
- Linux reference performance samples.

Durable results:

- `docs/recovery/generated/recovery_playmode_observation.md`
- `docs/recovery/generated/recovery_playmode_counts.json`

Artifact name:

- `recovery-playmode-r1-results-<SHA>`

A successful checkpoint result must include real NUnit XML and the expected total. A green job with missing or contradictory XML is not valid proof.

### D. `Recovery Golden Android`

File: `.github/workflows/recovery-golden-android.yml`

This builds the exact locked Android candidate using:

- build method `Ziptide.Build.RecoveryBuildAndroid.PatchScenesThenGoldenAPK`;
- compile define `ZIPTIDE_RECOVERY_GOLDEN`;
- exposure profile `GoldenSlice`;
- scenes `_Boot`, `W000_DriftIn`, and `ToxicCity` only.

It must run the patch, authoring, Forge bake, shader-safety, required-hook, scene-lock, and audit chain before producing the APK.

Durable result:

- `docs/recovery/generated/recovery_golden_android_observation.md`

Artifacts:

- `recovery-golden-apk-<SHA>`
- `recovery-golden-audit-<SHA>`

A green icon alone is not authorization. Inspect the build profile, audit evidence, APK existence, and checksums.

### E. `Recovery Clean Package Proof`

File: `.github/workflows/recovery-clean-package-proof.yml`

This is the slow authoritative lane for:

- `Ziptide/Packages/**` changes;
- Unity Input System or XR Interaction Toolkit changes;
- foundational player-rig/input lifecycle changes;
- foundational travel/world lifecycle changes;
- suspected Unity Library/cache contamination.

Every Unity job deletes `Ziptide/Library` before launch. It independently runs:

- exact 43-test PlayMode route;
- full EditMode suite;
- patch/world audit;
- locked Golden Android build.

It records the UPM-resolved `packages-lock.json` and requires the PlayMode result to be exactly:

- total 43;
- passed 43;
- failed 0;
- skipped 0;
- inconclusive 0.

Durable results:

- `docs/recovery/generated/recovery_clean_package_observation.md`
- `docs/recovery/generated/recovery_clean_package_counts.json`
- `Ziptide/Packages/packages-lock.json`

Artifacts:

- `clean-playmode-<SHA>`
- `clean-editmode-<SHA>`
- `clean-patch-audit-<SHA>`
- `clean-golden-apk-<SHA>`

Do not create another temporary cache-patching workflow. Change this lane directly through a bounded PR if its behavior needs correction.

---

## 6. Which lane should a change use?

| Change type | Minimum proof |
|---|---|
| Documentation only | contract/static checks as relevant; no Unity run unless docs are executable contracts |
| Model, texture, prop, sound, dialogue, plant spec, weapon tuning outside the Golden route | focused tests + ordinary CI; targeted Quest review when player-facing |
| Golden-route scene/UI/material/weapon presentation | ordinary CI + normal PlayMode + Golden Android as relevant |
| Travel, save, boot, persistent rig, input-session ownership | ordinary CI + normal PlayMode + Golden Android; use clean package proof when lifecycle/cache uncertainty exists |
| `Packages/**`, Unity Input System, XRI, OpenXR, XR Management | clean package proof is mandatory |
| Release or headset checkpoint candidate | all current required lanes on one exact source SHA + artifact review + Quest checklist |

Do not run every heavyweight lane for every small content addition. Do not skip the heavyweight lane for foundational ownership or package changes.

---

## 7. Quest authorization rule

A Quest checkpoint is authorized only when one exact source SHA has:

1. normal PlayMode at the required total with zero failure/skip/inconclusive;
2. clean package proof green when the candidate contains package or foundational lifecycle changes;
3. ordinary CI green: EditMode and patch/world audit;
4. contract scan current and green;
5. Golden Android green on the same source;
6. raw artifacts independently inspected;
7. exact APK and build-report SHA-256 recorded;
8. authorization table completed in `docs/recovery/QUEST_GOLDEN_CHECKPOINT.md`.

The Quest pass itself then proves:

- tracking and controllers;
- physical post-travel movement/turn behavior;
- comfort;
- device rendering and UI readability;
- Android/Quest runtime behavior;
- device frame pacing;
- save/relaunch behavior on hardware.

A headset pass does not prove flight, factories, gardens, multiplayer, every world, or final art quality unless those were explicitly included in that campaign.

---

## 8. Artifact inspection checklist

For a recovery candidate, inspect rather than merely list:

### PlayMode

- NUnit XML totals and named failures;
- full `playmode.log`;
- W000 and ToxicCity screenshots;
- UI spatial reports;
- spawn-clearance reports;
- runtime census and duplicate-owner reports;
- travel/save evidence;
- fallback/material reports;
- R1.10 performance JSON/Markdown.

### Golden Android

- nonempty `Ziptide.apk`;
- `recovery_golden_build_profile.json`;
- exact tested SHA;
- `GoldenSlice` profile;
- `ZIPTIDE_RECOVERY_GOLDEN` define;
- three-scene lock;
- Forge bake report;
- shader-safety report and seeded canary proof;
- required build-hook canary proof;
- scene/world audit;
- APK and build-report SHA-256.

### Clean package proof

- resolved `packages-lock.json` matches the manifest and was committed;
- no stale Unity Library was restored;
- all four jobs belong to the same workflow run and SHA;
- clean PlayMode is exactly 43/43;
- clean Golden hashes are preserved.

---

## 9. GitHub and workflow traps

### Path-filter omissions

A workflow may not start if the changed file is absent from its `paths` list. Confirm trigger coverage before waiting. The earlier `PlayerRigPersistence.cs` fix was skipped because the file was not covered.

### GitHub-token recursion suppression

Commits pushed by a workflow using `GITHUB_TOKEN` generally do not start another workflow. Generated writeback commits are evidence, not a retrigger mechanism. Use a normal human/app PR merge when a new proof run is required.

### Stale observations

Generated files may describe an older SHA. Always read `Tested SHA`; never infer freshness from the branch modification time.

### Cache contamination

Never restore a Unity `Library` across incompatible package graphs. Package changes use the clean package lane, which deletes `Library`.

### Old PR clutter

An open, conflicted recovery PR may be superseded. Check the current handoff and branch before resolving or merging it. Do not merge old R1 packets solely because they remain open.

### `main` is not the recovery branch

`terry-local-wip` is the source of truth. PR #3 is a historical branch comparison into an obsolete `main`; it is not the current sprint queue.

### Green icon versus evidence

A green GitHub check proves only that workflow’s exit status. Promotion still requires the expected artifacts and internally consistent counts/hashes.

---

## 10. Local PC and Quest tools

Primary scripts:

- `tools/dev_build_install.ps1` — local build/install loop where appropriate;
- `tools/quest_smoke.ps1` — bounded Quest/logcat smoke evidence.

Normal device procedure:

1. use the authorized artifact; do not silently rebuild another SHA;
2. verify APK SHA-256;
3. connect Quest and verify `adb devices` shows `device`;
4. clear/start logcat before launch;
5. execute `docs/recovery/QUEST_GOLDEN_CHECKPOINT.md` in order;
6. preserve the full log and PASS/FAIL notes;
7. stop on a blocker instead of touring unrelated systems.

---

## 11. Handoff template for future operators

Every recovery handoff should state:

- branch of record;
- exact source candidate SHA;
- latest normal PlayMode run and totals;
- latest clean package run and totals, when applicable;
- latest ordinary CI run and required job results;
- latest Golden Android run and artifact names;
- APK/build-report hashes if authorized;
- what raw artifacts were inspected;
- the single remaining blocker or next bounded step;
- held PRs that must not be merged blindly;
- whether Quest testing is authorized.

Never write “everything is green” without the exact SHA and evidence source.

---

## 12. Recovery exit and normal development

Recovery exits only after the bounded Quest checkpoint passes and an exit report is recorded.

After exit:

- the architecture owners remain protected;
- normal feature development resumes from `docs/PROJECT_COMPLETION_ROADMAP.md`;
- focused tests and ordinary CI handle most work;
- normal PlayMode protects the Golden route;
- clean package proof is reserved for packages and foundational lifecycle changes;
- separate Quest campaigns validate ship/flight, shooter combat, creatures, gardens/factories, worlds/skyscapes, vehicles, multiplayer, accessibility, and release quality.

The verification system should stay in the background. Its job is to let the team build faster with confidence, not to become the product.
