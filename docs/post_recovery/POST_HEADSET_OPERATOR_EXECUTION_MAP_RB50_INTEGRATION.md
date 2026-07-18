# POST-HEADSET OPERATOR MAP — RB46–RB50 INTEGRATION

**Status:** PLANNING ONLY. Mandatory companion to `POST_HEADSET_OPERATOR_EXECUTION_MAP.md` until Q0 folds these decisions into that map. This is not a second execution authority and authorizes no runtime, scene, asset, package, workflow, save, travel, locomotion, audio, profile, or certified-checkpoint change before Terry records the exact Quest verdict.

**Inputs reviewed:**

- `docs/design/GAP_AUDIT_JULY2026.md`
- `docs/design/SFX_FORGE.md`
- `docs/design/LOCALIZATION_DECISION.md`
- `docs/design/FAMILY_PROFILES.md`
- `docs/design/CONTRACT_LEDGER_WAYFINDING.md`
- `docs/design/PLAYTEST_AND_TELEMETRY.md`
- `docs/design/RESUME_MOMENT.md`
- `docs/design/TITLE_MENU_EXPERIENCE.md`
- `docs/storyboard/CAL_VOICE_AND_BARKS.md`
- `docs/storyboard/RILL_PROFOUND_LINES.md`
- the photosensitivity, thermal and licensing folds in the comfort, Forge IV and store-readiness documents.

The exact certified checkpoint remains source `2b158b498e421f3e8e3dd9b1b2f90bd6ffd58295`, clean run `29540554179`, APK SHA-256 `9bfe13ac0acda6718c3ae1664919cc5609385e50691b8676219c552d8e555a10`.

---

## 1. Verdict

These additions close genuine gaps. They should be retained.

They do **not** change today's gate or displace the first-hour program. Their correct role is:

1. strengthen the first thirty seconds and the first-hour quality bar;
2. provide the family/save/return scaffolding required before broad content scaling;
3. establish text, sound, privacy and licensing disciplines before content volume makes them expensive;
4. keep Cal and RILL writing as curated candidate material rather than automatic canon.

The plans are strong at concept level. The corrections below are required before any implementation packet may claim them.

---

## 2. Accepted decisions

The following are accepted planning direction:

- English-only at launch, structured for future localization.
- Stable IDs are identity; player-facing text belongs in author/library data, not scattered runtime literals.
- Imported fonts and third-party audio/assets require a `CREDITS.md` entry at import time.
- One SFX vocabulary and pooled playback path under the existing audio owner; no FMOD/Wwise parallel stack.
- A diegetic title environment may dress the proven Home Hub without changing its flow semantics.
- Four family profile slots plus a guest concept are appropriate for the family thesis.
- The family photo album is shared, as Terry approved, but its storage/migration owner still requires specification.
- Cross-world wayfinding belongs on the ship through a contract ledger, not a floating minimap.
- Playtesting should use observed stalls, delight markers, headset-off point and a mandatory comfort question.
- Telemetry, if implemented, remains local-only, fixed-field and never transmitted.
- Resume/reorientation should use one short data-derived RILL recap and never a modal interruption.
- Cal barks are scarce, context-specific and governed by an anti-annoyance constitution.
- RILL's profound-line candidates remain sparse, state-aware and spoiler-fenced.
- Gate-flash photosensitivity, long-session thermal behavior and licensing are required release concerns.

---

## 3. Required reconciliation before implementation

### 3.1 Title-menu RILL versus the first W000 RILL beat

`TITLE_MENU_EXPERIENCE.md` visually wakes RILL during menu selection, while first-hour v2.1 currently makes `FH_LOOK_AT_RILL` in W000 the first canonical RILL introduction.

**Binding decision:**

- The menu may show a dormant orb brighten as a non-speaking transition motif.
- It sets no RILL relationship, memory-tier, tutorial or story flag.
- It does not deliver Cal/RILL dialogue.
- `FH_LOOK_AT_RILL` in W000 remains the first canonical player-to-RILL interaction and first spoken greeting unless Terry explicitly versions the first-hour contract again.

This preserves the title ceremony without firing the story beat twice.

### 3.2 Profile selection must happen before loading a profile-owned world

The family-profile document uses physical bunks as the metaphor, but a profile cannot be selected after loading W000 under an unknown or wrong profile.

**Binding decision:**

- Slot selection occurs in `_Boot`, before `StartNewProfile`, `Load`, profile-bound comfort application and W000 travel.
- The title berth/departure board may present bunk tags or a miniature bunk manifest using the same visual language.
- The physical bunk wall inside the ship is the later management/personalization surface, not the first technical slot-selection location.
- No profile is loaded merely to enter quarters and choose another profile.
- No hot slot switching exists mid-session.

### 3.3 Comfort settings need a two-level ownership model

`COMFORT_AND_ACCESSIBILITY.md` currently says comfort is device-global. `FAMILY_PROFILES.md` correctly observes that siblings may require different presets.

**Binding decision:**

- Before slot selection, the game applies a safe device default, migrated from today's global `PlayerPrefs` values.
- After slot selection, body/accessibility preferences are slot-scoped: comfort preset, seated/standing preference, subtitle size/toggle, handedness/menu hand and haptic preference.
- Truly device-shared settings remain shared: master/device volume, platform-owned settings and any hardware-only calibration.
- A new slot inherits the current safe device/default values once; later edits are slot-specific.
- Deleting a slot does not delete the device-safe default.
- Q0 must update the comfort law text before FP-2 implementation so two documents do not remain authoritative simultaneously.

### 3.4 Family-profile storage contract is incomplete

Before FP-1, close these details:

1. **Legacy slot 0:** either the legacy save path remains slot 0 permanently, or an atomic one-time migration copies it to a slot path. Pick one and test it. Do not describe both.
2. **Guest:** guest is an ephemeral profile owned by `SaveSystem` with disk writes disabled/no-op for that session. It must still satisfy live gameplay reads. It resets after clean exit, crash or relaunch and may not bypass canonical save ownership.
3. **Backups:** backups are per slot. Deletion creates a deliberate tombstone/undo state so normal corruption recovery cannot silently resurrect a profile the player intentionally deleted.
4. **Shared photo album:** current photo metadata/storage must be audited. Define one device-level gallery owner and a migration from any profile-scoped metadata. Shared gallery cannot be declared without this data-flow packet.
5. **Names:** slot identifiers stored in diagnostics/telemetry are anonymous slot indexes or local IDs, never a child's typed name.
6. **Concurrency:** all paths, backups, temporary files and recovery checks must be isolated by slot.

### 3.5 Ledger stamp acknowledgement requires state

`CONTRACT_LEDGER_WAYFINDING.md` says the board adds no persistence, but a once-only stamp ceremony needs to know whether the player has already seen it.

**Binding decision:**

- Ledger status rows remain purely derived from canonical world/job flags.
- A stamp ceremony may use the existing once-latch/profile-flag convention for acknowledgement, or be explicitly session-only.
- It may not repeat on every ship visit accidentally.
- Any acknowledgement flag is presentation state only and never becomes a second contract-completion truth.
- Ledger selection highlights the canonical hero helm/gate destination model after helm consolidation; it never calls travel or uses a raw developer scene list.

### 3.6 SFX Forge and Material Audio must be one stack

`SFX_FORGE.md` and `HORROR_MICROWORLDS_AND_MATERIAL_AUDIO.md` must not create separate material taxonomies or playback managers.

**Binding decision:**

- MA-0 performs repository audio archaeology and identifies the surviving mixer/audio ownership.
- One semantic surface model resolves base material + optional overlay + contact energy/verb.
- The Damage Response matrix, Forge/world mappings, footsteps/landings, object contacts and SFX library reference that same model.
- `SfxDefinition` and `SfxPlayer` are the one-shot vocabulary/playback layer under `AudioDirector`; they do not replace ambience or VO.
- Dialogue priority is a global mix/voice-budget law, not a requirement to put dialogue clips inside the SFX library.
- No per-leaf, per-object-pair or per-prefab bespoke audio systems.

**Vertical sequence:**

1. MA-0 archaeology + semantic material vocabulary.
2. SFX-1 definitions/library/validation.
3. SFX-2 pooled playback and priority logic.
4. Q4 integrates the hero Taser's first excellent SFX family.
5. Q2/Q8/Q11 add selected repair, UI, machine, impact and world-response sounds as those player beats become real.
6. Q13 completes the broader mix, footsteps/landings, contact routing and clamp-volume Quest verdicts.

SFX is therefore not postponed wholesale until Q13; it grows vertically with the first hour.

### 3.7 Playtest privacy and repository handling

The protocol is useful immediately, but actual notes about minors must not become public repository content by default.

**Binding decision:**

- The repository may contain a blank `PLAYTEST_LOG_TEMPLATE.md` and sanitized issue summaries.
- Raw session notes, exact ages, verbatim quotes, headset-off timestamps and any names remain local/private unless Terry deliberately sanitizes them.
- Local SessionSummary uses anonymous slot index only; never a typed profile/bunk name.
- Nothing is transmitted automatically.
- Do not label the design legally “COPPA-clean” as a substitute for store/legal review. The defensible claim is privacy-minimized, local-only and no third-party analytics.
- Telemetry counters identify where to investigate; observation remains the explanation.

Implementation must count through explicit neutral diagnostics/events or a small diagnostics seam. It must not parse `Debug.Log` text as its runtime source of truth.

### 3.8 Resume cannot blindly save unstable transient state

“Doff = autosave” is only safe when the state being saved has a defined restoration contract.

**Binding decision:**

- Platform/lifecycle owner emits idempotent doff/re-don events.
- SaveSystem remains the only profile/disk owner.
- Doff requests a guarded canonical save; it does not serialize arbitrary transform, physics or half-completed animation state.
- Each transient owner declares a resume policy:
  - ordinary grounded play: resume after physics/input settle;
  - zipline/traversal: safe anchor/restart policy;
  - flight: neutral/autopilot or authored restart policy;
  - repair/artifact join: restore the last canonical stage, never a half-transition;
  - defense wave: explicitly resume, restart or safely resolve according to the first-hour save matrix.
- No global head/rig teleport is introduced to solve resume.
- Resume/world-entry/RILL lines pass through one priority/courtesy path so only one line speaks.
- The pure recap composer can be built early; doff-save and safe-rest behavior wait for their owner-specific contracts and Quest checks.

### 3.9 Title environment must be non-blocking and non-canonical

The Berth Before Dawn concept is accepted with these rails:

- Home Hub board semantics, `HomeHubFlowState`, boot hold, save choices and travel callback remain frozen.
- Board interactivity is available before decoration. Sky, water, silhouette and title may fade in/degrade independently.
- Decoration has an explicit low-memory/performance fallback.
- Menu content is `_Boot` presentation, not a travel destination or story world.
- The ship silhouette cannot claim a final hero-ship design before Q2 establishes it; use the current approved silhouette provider or a deliberately generic shadow proxy.
- TM-1 may land after the SFX/audio-owner foundation is known.
- TM-2/TM-3 should ride the Q2 hero-ship/art window.
- TM-4 waits for the canonical transition motif and device timing.
- TM-5 is subordinate to the corrected family-profile flow.
- A beautiful boot capture and unchanged time-to-interactive are both required; neither substitutes for the other.

### 3.10 Cal and RILL candidate writing is not automatic canon

`CAL_VOICE_AND_BARKS.md` and `RILL_PROFOUND_LINES.md` remain candidate libraries.

Before adoption, every line needs:

- Terry/story-lane shortlist approval;
- exact story state, world/flag trigger and spoiler classification;
- once-latch/repeat policy;
- caption-v2 chunking check;
- priority/courtesy behavior;
- headset timing/read where the pause or delivery is important.

One dialogue scheduling/presentation authority must arbitrate RILL plot lines, RILL FollowUps, resume recaps, Cal scripted replies and rare Cal barks. Do not create an independent always-running bark singleton or second subtitle queue.

The anti-annoyance laws are accepted: silence default, rare notable triggers, no late queued bark, no per-kill/per-pickup chatter, and no profound-line density greater than one per world visit.

These writing passes enrich Q3 and later story work. They do not block Q0–Q2 and they do not authorize broad VO production.

---

## 4. Revised placement in the post-PASS queue

This refines, but does not replace, Q0–Q13.

### Q0 — governance

Add:

- adopt `LOCALIZATION_DECISION.md` as a cross-cutting law;
- create/index `CREDITS.md` before importing the caption font or licensed audio;
- record the privacy rule: raw minor playtest notes remain private/local;
- index the rb46–rb50 plans by roadmap tier;
- resolve the stale sprint claim truthfully; current docs head `76519f4` still has 1,048/1,049 EditMode tests passing with the sole failure `GateGap5_NoBoardClaim_RotsSilently`.

### Q1 — contracts and author discipline

Add:

- stable player-facing text IDs and author/library ownership to the PR template;
- localization/font/license checks to text-touching packets;
- a repository playtest template only, not raw family notes;
- the exact title/profile/RILL conflict decisions from §3.

### Q2 — W000, hero ship and boot presentation foundation

Add:

- title environment may begin only as additive presentation around the proven Home Hub;
- TM-2/TM-3 coordinate with the hero-ship silhouette and existing Forge providers;
- no family-profile slot implementation yet unless a separately approved save-lane packet is claimed.

### Q3 — RILL, captions and writing

Add:

- pure resume-recap composition using the same one-line objective data the future ledger will render;
- Terry shortlist/placement pass for only the first-hour Cal/RILL candidates;
- one dialogue courtesy/priority contract covering story, resume and barks;
- no broad VO requirement.

### Q4 — SFX core + hero Taser

Before or inside Q4:

- MA-0 archaeology;
- SFX-1 and SFX-2 as bounded foundation packets;
- first complete Taser fire/charge/impact/holster family;
- one target/material response set.

At most two invisible audio-foundation PRs may precede the visible Taser result.

### Q5–Q9

- The range becomes the first reusable SFX/material-response proof surface.
- Flight, salvage, Toxic City, artifact and key interactions each receive their selected sound family as they are built.
- Player-facing strings remain data/ID-authored.

### Q10 — first true Ziptide

Add a blocking photosensitivity campaign:

- bounded flash rise/peak duration;
- no strobing/repeated full-view pulses;
- flash-intensity option behavior;
- caption visibility through the effect;
- Cozy/Standard/Bold and clamp-edge Quest verdicts.

### Q11–Q12 — W002, save and family playtest

Add:

- explicit doff/re-don behavior for build, wave, plant and yield states;
- guarded save/resume policies from §3.8;
- use the paper kid-session protocol during the full first-hour family acceptance;
- sanitized issues only; raw notes remain private;
- local SessionSummary may be added only after its neutral counter seams are specified.

### Q13 — commercial first-hour quality ratchet

Add:

- title menu TM-1..TM-4 complete and device-proven;
- final SFX/material mix, footsteps/landings and hero contact set;
- long 60–70 minute thermal/performance run, including post-throttle quality behavior;
- licensing ledger complete for every imported font/audio/art asset;
- resume line/courtesy behavior and no duplicate world-enter delivery;
- final privacy review of any diagnostic artifacts.

### Immediately after the complete first hour, before broad world scaling

**F1 — Family profiles**

- FP-1 legacy/slot/guest/backups/tombstone contract;
- FP-2 corrected per-slot versus device-shared preference model;
- FP-3 title-board bunk selection + physical ship bunk management;
- FP-4 RILL/profile awareness;
- shared gallery migration as its own named data packet if the audit shows profile-scoped metadata.

**F2 — Cross-world ledger and returning-player system**

- CL-1 pure derived ledger model;
- CL-2 ship board/stamp presentation;
- CL-3 canonical destination highlight;
- CL-4 capped RILL follow-up;
- full cold-return recap uses the same objective summary data.

These land before the project grows beyond the first small cluster, when cross-world memory and multiple family saves become operational requirements rather than future polish.

---

## 5. Additional operator-template fields

Every future packet touching these plans must also state:

1. **Player-facing text source:** stable IDs and exact author/library owner.
2. **License impact:** `CREDITS.md` entry or “none.”
3. **Privacy impact:** what is stored, where, retention, and whether any raw family note leaves Terry's private storage.
4. **Profile scope:** device-shared, slot-scoped, session-only or world/profile state.
5. **Audio/material source:** canonical semantic material and SFX owner.
6. **Interruption behavior:** doff, re-don, quit/relaunch and transient-state policy.
7. **Dialogue arbitration:** presenter/priority/courtesy and suppression behavior.
8. **Long-session proof:** when thermal, memory, cumulative audio or battery behavior could matter.

---

## 6. Definition of integrated

The rb46–rb50 additions are integrated when:

- the exact Quest checkpoint has passed and Q0 is complete;
- the main operator map incorporates this annex and this file becomes historical;
- localization, licensing and privacy are standing review laws;
- the title menu does not duplicate the first RILL beat or slow interaction;
- family profiles have one exact boot/save/gallery/delete data flow;
- SFX and material audio share one taxonomy and one playback ownership tree;
- ledger status is derived and stamp acknowledgement cannot repeat incorrectly;
- resume never saves or restores undefined transient state;
- raw playtest notes about minors are not committed publicly;
- Cal/RILL candidates are curated rather than bulk-imported;
- every item has a dependency position, owner, proof lane and rollback.

Until then, the documents are approved planning inputs, not independent implementation commands.