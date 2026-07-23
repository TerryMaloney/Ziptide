# Photon — durable evidence notes (collected 2026-07-23, Fable 5 C-lane/T-Dog)

Read-only findings backing `docs/licensing/PHOTON_RELEASE_DECISION.md` and the
`photon-unity-sdk` entry in `docs/licensing/third_party_manifest.json`. Every claim below is
either a repo path you can open or an official-source fetch with its date. Nothing is inferred
from folder names.

## 1 · Product + exact version (PROVEN, repo)
- Product: **Photon Unity Networking 2 (PUN 2)** — full distribution layout at
  `Ziptide/Assets/Photon/`: `PhotonUnityNetworking/`, `PhotonRealtime/`, `PhotonChat/`,
  `PhotonLibs/`, plus `PhotonNetworking-Documentation.chm`.
- Exact version: **PUN 2.55** —
  `Ziptide/Assets/Photon/PhotonUnityNetworking/Code/PhotonNetwork.cs:67`
  (`public const string PunVersion = "2.55";`) and
  `Ziptide/Assets/Photon/PhotonUnityNetworking/changelog.txt` head:
  "v2.55 (3. July 2026) … Updated: Library, Realtime and Chat API to **4.1.8.21**."

## 2 · Import provenance (PARTIALLY PROVEN, git)
- Imported by commit **`21f117c1`**, 2026-07-06, author Terry, message:
  *"PUN2 imported + App ID + ZIPTIDE_PHOTON define on."*
- **Acquisition channel NOT recorded** (photonengine.com account download vs Unity Asset Store;
  FREE vs PLUS purchase). This is a fact only Terry can supply.

## 3 · License texts present in the tree (PROVEN, repo)
- **No Photon SDK license file exists anywhere under `Ziptide/Assets/Photon/`** (searched
  `*license*`, `*eula*`, `*.pdf`). The only license files found are third-party items INSIDE the
  distribution:
  - `PhotonUnityNetworking/Code/Editor/ReordableList/LICENSE.txt` (an editor UI helper's own
    license);
  - `PhotonChat/Demos/Demo Chat/Resources/OpenSans/Apache License.txt` (a demo font).
- Consequence: the governing Photon SDK license terms are **not provable from the repo**.

## 4 · Official-source checks (fetched 2026-07-23)
- `photonengine.com/pun`: PUN is in **ongoing support mode** ("PUN will continue to receive
  support…"); **Fusion** is the recommended successor for new projects; tiers listed: **PUN
  FREE** and **PUN PLUS ($95, 100 CCU Photon Cloud plan, 12 months)**. No license text or
  attribution terms on the page.
- `photonengine.com/sdks`: SDK download packages are described as containing
  **"license.txt - the license terms"** — the license text itself is not published on the page.
- `photonengine.com/terms`: an index page; states *"See Photon License Terms for Photon
  Multiplayer Backends' license terms"* (separate linked document; SDK redistribution specifics
  not on the index).
- Conclusion: exact license/redistribution/attribution obligations remain **verification-
  required**; the authoritative artifact is the `license.txt` of the exact distribution used.

## 5 · Build/runtime facts (PROVEN, repo)
- Scripting defines (`Ziptide/ProjectSettings/ProjectSettings.asset` lines 815/825): Android and
  Standalone both carry `PHOTON_UNITY_NETWORKING;PUN_2_0_OR_NEWER;PUN_2_OR_NEWER;
  PUN_2_19_OR_NEWER;ZIPTIDE_PHOTON` → **Photon assemblies AND the Ziptide adapter compile into
  current device builds.**
- Adapter: `Ziptide/Assets/ZiptideNet/` (`NetBootstrap.cs`, `PhotonPvpTransport.cs`), compiled
  only under `ZIPTIDE_PHOTON`. Gameplay reaches networking ONLY through the
  `IPvpTransport`/`PvpNetHub` seam (`Ziptide/Assets/Ziptide/Multiplayer/Runtime/Net/PvpNetHub.cs`
  — no direct Photon references in Ziptide assemblies; loopback transport is the default).
- Committed **live App ID**: `AppIdRealtime: 7b6d9f0f-f1f8-4ee3-9f90-367e32f5b268` in
  `Ziptide/Assets/Photon/PhotonUnityNetworking/Resources/PhotonServerSettings.asset` — lives in
  a `Resources/` folder, so it ships in ANY build regardless of defines.
- Demo content: `PhotonUnityNetworking/Demos/**` and `PhotonChat/Demos/**` have their own
  asmdefs; **no Ziptide code references them and no demo scene is in EditorBuildSettings**
  (census 2026-07-23). The `.chm` documentation file is inert.
- Recovery independence: `Ziptide/Assets/Ziptide/Tests/PlayMode/RecoveryRuntimeArtifactGuard.cs`
  guards that the recovery route does not depend on Photon; the manifest already records
  `shipsInCurrentRecoveryCandidate: false` for the c45b1a2 candidate.

## 6 · The precise missing artifacts (what closes verification)
1. **`license.txt` from the exact PUN 2.55 package** Terry downloaded (or a fresh download of
   the same version from the same channel) — copy into this folder.
2. **Terry's acquisition record**: channel (photonengine.com vs Asset Store), FREE vs PLUS,
   account/order reference if any.
3. The **"Photon License Terms"** document linked from photonengine.com/terms, captured with
   date, IF it (rather than the package license.txt) governs SDK redistribution.
