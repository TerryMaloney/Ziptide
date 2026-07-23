# ZIPTIDE — third-party credits and licensing ledger

This file is the human-readable licensing ledger required by `docs/META_STORE_READINESS.md`.
Every external package, SDK, font, audio file, texture, mesh, model, or other third-party asset must
be recorded **when it enters the repository**, not during store submission.

Machine authority: `docs/licensing/third_party_manifest.json`
Validation: `tools/third_party_license_gate.py`

## Release rule

An entry may be present in the repository while its license research is incomplete, but it is not
approved for a release build until the manifest records its exact version/source, license evidence,
obligations, and `distributionStatus: approved`. Unknown terms are written as
`verification-required`; they are never inferred.

## Imported third-party software

### photon-unity-sdk

- **Provider:** Photon Engine / Exit Games
- **Repository path:** `Ziptide/Assets/Photon`
- **Purpose:** Optional online networking seam; current recovery and headset route do not depend on it.
- **Exact version:** **PUN 2.55** (Realtime/Chat API 4.1.8.21) — verified 2026-07-23 from
  `PhotonNetwork.cs` (`PunVersion`) + `changelog.txt`.
- **Source/download record:** Imported by git commit `21f117c1` (2026-07-06, Terry). Acquisition
  channel (photonengine.com vs Asset Store; FREE vs PLUS) still needs Terry's one-line record.
- **License and redistribution terms:** Verification required — no Photon SDK `license.txt`
  exists in the imported tree; the governing text ships inside the SDK package. Missing
  artifacts + findings: `docs/licensing/PHOTON_RELEASE_DECISION.md` and
  `docs/licensing/evidence/photon/EVIDENCE.md`.
- **Attribution or notice obligation:** Verification required (from the package license text).
- **Distribution status:** **HOLD — not approved for release distribution until verified.**
  Note: `ZIPTIDE_PHOTON` is currently ON, so Photon compiles into dev builds today; the release
  recommendation (defines off until online ships, demos stripped) is in the decision doc.
- **Owner:** Multiplayer lane.

## Unity packages

Unity-owned packages declared in `Ziptide/Packages/manifest.json` are tracked collectively as engine
and editor dependencies. The licensing gate automatically requires any future package whose ID does
not begin with `com.unity.` to receive its own manifest and credits entry before Fast Preflight can
pass.

## External art, fonts, music, audio, and models

None are currently declared as imported release assets. Concept images under `concepts/` are
reference material and do not ship in the game. When a shippable external asset is imported, add its
manifest entry and a section here in the same commit, including source, exact license, obligations,
and evidence path.

## Operator import checklist

1. Save the original source/download record and license evidence in an appropriate repository or
   durable project record.
2. Add one entry to `docs/licensing/third_party_manifest.json`.
3. Add the matching `### <creditsAnchor>` section to this file.
4. Record exact version or asset revision; never use “latest.”
5. Record attribution, notice, share-alike, modification, redistribution, and commercial-use terms.
6. Run `python3 tools/third_party_license_gate.py`.
7. Do not mark `distributionStatus` approved until the evidence supports it.
