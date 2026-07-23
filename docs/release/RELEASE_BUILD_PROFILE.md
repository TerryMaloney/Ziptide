# 🔐 RELEASE BUILD PROFILE — what a store-ready ZIPTIDE build IS, what today's build ISN'T, and the auditor that enforces the gap

**Written 2026-07-23 (Fable 5 C-lane/T-Dog, gpt-high-value-lanes assignment).** Machine authority:
`docs/release/release_build_contract.json`, enforced by `tools/release_build_hygiene_gate.py`
(mutation tests: `tools/tests/test_release_build_hygiene_gate.py`; Fast Preflight report:
`Builds/Reports/release_build_hygiene.json`). Companions: `docs/VRC_TEST_PLAN.md` (cert map) ·
`docs/store_readiness/meta_store_release_contract.json` (paperwork) ·
`docs/licensing/PHOTON_RELEASE_DECISION.md` (Photon facts). This doc is the human layer: the
audited truth and the ordered fix list, detailed enough that a lower-cost model can execute the
build changes without repeating the audit.

---

## 1 · The audited truth (2026-07-23, every claim pathed)

**Already correct (the gate verifies these continuously — blockers if they regress):**
- Android `applicationIdentifier` = `com.terrymaloney.ziptide` (`ProjectSettings.asset` §applicationIdentifier).
- Scripting backend = **IL2CPP** (`scriptingBackend: Android: 1`).
- Architectures = **ARM64-only** (`AndroidTargetArchitectures: 2`).
- `AndroidMinSdkVersion: 29`; version identity set (`bundleVersion: 0.1.0`, `AndroidBundleVersionCode: 1`).
- No non-Unity UPM packages (Photon is an Assets/ import, tracked by the licensing manifest).
- **DevMenu already compile-strips itself**: `DevMenu.cs` is `#if UNITY_EDITOR || DEVELOPMENT_BUILD`
  — the moment builds stop being development builds, the dev menu vanishes with zero extra work.

**The 7 real hygiene gaps (the gate reports each as a release-hold today):**
1. **Every APK is a development build.** `BuildAndroid.APK()` hardcodes
   `BuildOptions.Development | BuildOptions.AllowDebugging` (`BuildAndroid.cs:242`; the recovery
   build does the same by design — it is exempt, a recovery artifact, never a store candidate).
2. **No release entry point exists.** There is no method anywhere that can produce a
   non-development, release-signed APK.
3. **Debug-signed.** `AndroidKeystoreName`/`AndroidKeyaliasName` are empty — no release keystore
   exists, let alone custody/backup.
4. **Release-forbidden defines are ON**: `ZIPTIDE_PHOTON` + 4 PUN defines active on Android →
   unused networking code compiles into every build (per the Photon decision, defines go OFF for
   release until online ships — Terry ⚖ pending).
5. **A live Photon App ID ships in every build** (`PhotonServerSettings.asset` is a Resources/
   asset — included regardless of defines).
6. **Demo dead weight present**: Photon PUN + Chat demo folders, the `.chm`, `SampleScene.unity`
   — strip at release.
7. **Target SDK unpinned** (`AndroidTargetSdkVersion: 0` = auto) — pin explicitly at release to
   the then-current Meta-required level.
Plus two structural absences tracked as manual rows: **entitlement/Platform SDK** (needs Terry's
Meta App ID first) and **merged-manifest permissions review** (possible only once a release-mode
build exists).

## 2 · How the auditor works
- `python3 tools/release_build_hygiene_gate.py` — **development mode**: parses the REAL
  `ProjectSettings.asset`/build script/content (facts snapshot in the JSON report), reports the
  known gaps as `[hold]` findings, exits 0. Structural lies — contract corruption, app-id/IL2CPP/
  ARM64/version regressions, an over-1GB APK, an undeclared non-Unity package, "verified" manual
  rows citing missing evidence — are `[blocker]`s and fail immediately, so hygiene can only
  ratchet forward.
- `python3 tools/release_build_hygiene_gate.py --require-release-ready` — **release mode**: every
  hold becomes a blocker. Run it on a genuine release candidate; it cannot say "ready" while any
  gap above is open. (The unified readiness authority may consume the JSON report; classification
  matches its release-hold vocabulary.)
- Runs in Fast Preflight on every push with the JSON report artifact.

## 3 · The ordered fix list (for the later build-change executor — do NOT redo the audit)
1. **`ReleaseAPK()`** in `BuildAndroid.cs`: identical `PatchAndAudit()` + splash step, then
   `BuildOptions.None`, output `Builds/Android/Ziptide-release.apk`. Do not touch `APK()` (the
   dev path stays) or `RecoveryBuildAndroid` (frozen). Acceptance: gate's
   `RELEASE_METHOD_MISSING` + `DEV_BUILD_FLAGS_HARDCODED` rows clear for the release path;
   DevMenu absent from the release APK (auto via the `#if`).
2. **Keystore** (Terry, on his PC — never committed): create, configure PlayerSettings, record
   custody + off-site backup in `docs/release/evidence/`; flip `keystore-custody` to `verified`
   with that evidence path.
3. **Release define profile:** after Terry's ⚖ on the Photon recommendation — defines OFF for
   release (a build-time define override in `ReleaseAPK()` is cleaner than editing the shared
   ProjectSettings), Photon server settings excluded/emptied; verify the built APK contains no
   AppId string; flip `photon-appid-excluded`.
4. **Strip list** execution at release (demo roots — coordinate with the licensing lane; roots
   are enumerated in the contract).
5. **Pin target SDK** + record the Meta-required level with a dated source.
6. **Entitlement envelope** once Terry's App ID exists (Platform SDK import → licensing manifest
   entry → boot check → VRC Security.1 row).
7. **Permissions evidence**: dump + archive the merged manifest of the first release build.
Each step flips its contract row; the gate is the scoreboard. When `--require-release-ready`
exits 0 on a real candidate, build hygiene is DONE — remaining store work lives in the store
contract and VRC plan.
