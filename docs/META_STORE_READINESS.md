# META STORE READINESS — governed release contract

This document covers everything between “the game works” and “the game may be submitted.” It is not a claim that ZIPTIDE is release-ready.

- Machine authority: `docs/store_readiness/meta_store_release_contract.json`
- Validator: `tools/meta_store_readiness_gate.py`
- Certification test plan: `docs/VRC_TEST_PLAN.md`
- Device evidence matrix: `docs/DEVICE_TEST_CHECKLIST.md` §10
- Licensing authority: `docs/licensing/third_party_manifest.json`
- Last requirement verification: 2026-07-23 against Meta’s official Quest publishing requirements.

## Gate policy

Ordinary development runs:

```text
python3 tools/meta_store_readiness_gate.py
```

This validates ownership, evidence paths, status vocabulary, URLs, waivers, annual deadlines, and truthfulness. Open future work remains a visible **release hold** without making unrelated development CI permanently red.

A submission candidate must run:

```text
python3 tools/meta_store_readiness_gate.py --require-release-ready
```

That mode blocks unless every required row is `verified`, `waived` with explicit approval, or genuinely `not-applicable` with rationale—and the candidate itself is marked `ready`.

## Current release verdict

**HOLD.** The reserved `c45b1a2` APK is a recovery/headset-test artifact, not a store candidate. The recovery freeze and exact headset verdict remain first priority.

No operator may convert an unknown requirement into a pass. `verification-required` is the correct value until evidence exists.

## A · Terry-owned administration and public URLs

- [ ] Meta developer organization verified.
- [ ] App record created and App ID handled through the project’s secret/configuration process.
- [ ] Stable **support page URL** published and checked.
- [ ] Stable **Terms of Service URL** published and checked.
- [ ] Hosted privacy policy matching the actual shipped data behavior, with contact/deletion path.
- [ ] Data Use Checkup completed and its next annual due date recorded.
- [ ] Age-group self-certification path locked; Mixed Ages is the current planned path, but the manifest keeps it decision-required until Terry confirms it.
- [ ] IARC questionnaire completed from shipped content, not plans.

Never commit private dashboard credentials, keystore secrets, or other authentication material as evidence.

## B · Release packaging and security

- [ ] Release keystore created outside the repository, backed up, and custody documented.
- [ ] APK signature scheme v2 proven.
- [ ] ARM64/IL2CPP, package size, supported Android requirements, and release manifest audited.
- [ ] Android permissions reduced to only what the shipping feature set needs.
- [ ] DevMenu and diagnostics locked or stripped through release flags—not deleted.
- [ ] Visible version identifier matches the build.
- [ ] Meta entitlement check completes in the required launch window after the App ID exists.

### Photon boundary

T-Dog’s evidence establishes PUN 2.55 and shows networking defines plus a live App ID currently enter development builds. Release remains on hold until the package license text/acquisition record are stored and the release pipeline deliberately decides:

- whether Photon is enabled for that release;
- whether demo content is stripped;
- whether the App ID is retained or rotated;
- whether all licensing obligations are fulfilled.

Authority: `docs/licensing/PHOTON_RELEASE_DECISION.md`.

## C · Required runtime behavior after recovery freeze

These are known implementation gaps, not paperwork passes:

- [ ] **System-focus envelope:** pause simulation when required, keep compliant rendering under system UI, hide hands/controllers, ignore gameplay input, and handle audio correctly.
- [ ] Forward-orientation reset/recenter.
- [ ] Truthful seated/standing/room-scale and controller-only metadata.
- [ ] Head-tracked graphics or VR loading indicator within four seconds of cold launch.
- [ ] Sustained 72 Hz on the oldest supported device under thermal load.
- [ ] No data loss across doff/resume, Guardian, controller sleep, low storage, and update-over-install.
- [ ] Player-facing title/save flow, comfort settings, onboarding, credits/licenses, and release-safe diagnostics.

Runtime implementation stays frozen until Terry completes the exact recovery retry card and reports the headset verdict.

## D · Store presentation

- [ ] Logo/icon/cover assets built to the current Meta specifications.
- [ ] Genuine in-headset screenshots.
- [ ] Trailer within current duration/content rules.
- [ ] Store name, descriptions, keywords, website, support, and Terms links validated.
- [ ] Comfort rating and metadata match the actual game.
- [ ] Content declarations and IARC answers match the shipped build.

This is Picasso/art plus publishing work after the visual vertical slice is representative. Placeholder visuals must not be submitted as final store evidence.

## E · Evidence rules

A row can become `verified` only when:

1. the requirement was checked against the current official rule;
2. the implementation or dashboard action is complete;
3. a durable repository evidence path exists;
4. `verifiedAt` is recorded;
5. URL rows contain a reachable HTTPS URL;
6. annual rows include a future `nextDue` date.

A waiver requires both `waiverReason` and `approvedBy`. “Not applicable” requires a specific applicability rationale. Neither status is a shortcut for unfinished work.

## F · Submission sequence

1. Complete the exact recovery headset test before changing protected runtime systems.
2. Close runtime certification gaps and run `DEVICE_TEST_CHECKLIST.md` §10.
3. Produce a release-keystore build with the release flag/permission audit.
4. Run `meta_store_readiness_gate.py --require-release-ready`.
5. Upload to a private release channel and run Meta’s validator/VRC tooling.
6. Convert every failure into a tracked blocker; fix and repeat.
7. Submit only when the machine contract and the actual dashboard agree.
