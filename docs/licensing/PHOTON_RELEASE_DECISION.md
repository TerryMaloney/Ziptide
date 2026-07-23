# PHOTON RELEASE DECISION — provenance, dependency map, and the keep/strip/replace call

**Status:** RESEARCHED 2026-07-23 (Fable 5 C-lane/T-Dog, per the gpt-first-hour work order).
Evidence file: `docs/licensing/evidence/photon/EVIDENCE.md` (every claim there carries a repo
path or a dated official-source fetch). Machine entry: `photon-unity-sdk` in
`docs/licensing/third_party_manifest.json`. **Distribution remains `hold-verification` — this
document does NOT approve release distribution.**

## 1 · Exact findings
- **What it is:** Photon Unity Networking 2 (**PUN 2.55**, Realtime/Chat API 4.1.8.21), the full
  distribution: PUN + Realtime + Chat + native libs + demos + `.chm` docs.
- **How it got here:** git commit `21f117c1` (Terry, 2026-07-06 — "PUN2 imported + App ID +
  ZIPTIDE_PHOTON define on"). Acquisition channel (photonengine.com vs Asset Store; FREE vs
  PLUS $95/100-CCU) is **unrecorded** — only Terry can state it.
- **License:** **unprovable from the repo** — no Photon SDK `license.txt` exists in the tree
  (official pages confirm the license text ships inside SDK packages and is not published on
  the site). Product status verified official: PUN in ongoing-support mode; Fusion is the
  successor for new projects.
- **It ships TODAY:** `ZIPTIDE_PHOTON` + the PUN defines are ON for Android/Standalone, so
  Photon code and the `Assets/ZiptideNet` adapter compile into current dev builds, and the
  committed **live App ID** (`PhotonServerSettings.asset`, a `Resources/` asset) ships in every
  build regardless of defines.

## 2 · Runtime dependency map
```
Gameplay (PvP lobby/HUD/match)                 [no Photon references]
   └── IPvpTransport / PvpNetHub seam          Ziptide.Multiplayer (LoopbackPvpTransport default)
         └── Assets/ZiptideNet adapter          ONLY under ZIPTIDE_PHOTON (currently ON)
               └── Assets/Photon (PUN 2.55)     required by the adapter alone
Demos + .chm                                    referenced by NOTHING (no build-settings scene,
                                                no Ziptide code) — pure dead weight
Recovery/golden route                           independent (RecoveryRuntimeArtifactGuard)
```
**Nothing in the shippable game requires any Photon demo content.** The only real consumer is
the optional online adapter behind the seam — exactly the architecture that makes every option
below cheap.

## 3 · Recommendation: **KEEP (gated) · STRIP demos at release · GATE the define · VERIFY before any store build**
1. **KEEP the SDK in-repo.** M7b (online, two-headset) is a planned milestone; the seam is
   built; removal now would churn a working lane for no licensing gain (holding unverified
   software in a private repo is not distribution — DISTRIBUTION is what's on hold).
2. **STRIP at release (do not delete today):** `PhotonUnityNetworking/Demos/**`,
   `PhotonChat/Demos/**`, `PhotonNetworking-Documentation.chm` — zero runtime references;
   removing them shrinks the repo/import surface and deletes the demo fonts/assets with their
   own license tails. Execute as a release-lane step (or earlier with Terry's nod); forbidden
   in this docs-only lane.
3. **Turn `ZIPTIDE_PHOTON` (and the PUN defines) OFF for RELEASE builds until online actually
   ships.** Today the dev builds carry networking code + a live App ID that no shipped feature
   uses. Benefits: smaller APK, no un-needed INTERNET-permission pressure (VRC Security.2
   minimal-permissions), the Functional.7 offline-notice question stays N/A, and the license
   question stops blocking every release build (with defines off + demos stripped, whether any
   Photon bits still land in the APK must be re-verified — `Resources/PhotonServerSettings.asset`
   ships regardless of defines unless excluded). ⚖ Terry decision; execution belongs to the
   release-hygiene lane, coordinated with the multiplayer owner.
4. **App ID hygiene:** Photon App IDs are client-visible by design (not a secret), but ours is
   committed and shipping in builds that don't use it. Recommend Terry: check the Photon
   dashboard for unexpected usage, and consider rotating the App ID before any public release
   build. Not a license blocker.
5. **REPLACE later, maybe — decide at M7b, not now.** Official guidance: new projects → Fusion.
   When online work actually starts, spend one session on PUN-vs-Fusion (the seam means the
   gameplay side barely cares). Until then, replacing would be speculative churn.

## 4 · What closes `hold-verification` (the precise missing artifacts)
1. `license.txt` from the exact PUN 2.55 package (Terry's download or a same-version
   re-download) → copy to `docs/licensing/evidence/photon/`.
2. Terry's one-line acquisition record (channel, FREE vs PLUS, order/account ref).
3. If applicable, the linked "Photon License Terms" document captured with date.
Then: update `licenseId`/`obligations`/`licenseEvidencePath` in the manifest from the actual
text, and only then consider `distributionStatus: approved` (the gate enforces evidence-first).

## 5 · Manifest/credits changes made with this decision (proven-only)
- `exactVersion` → "PUN 2.55 (Realtime/Chat API 4.1.8.21)" (proven, §1 evidence).
- `sourceRecord` → the git import record incl. the unrecorded-channel caveat (proven half).
- `licenseId` / `licenseEvidencePath` / `obligations` remain `verification-required`;
  `distributionStatus` remains `hold-verification`. Nothing was approved.
