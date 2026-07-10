# 🏪 META STORE READINESS — everything between "the game works" and "the game ships"

**Why this exists (Terry, 2026-07-10):** "we got to make sure it's actually ready to go for the meta
store... I don't know what all goes into that." This is that list. Written from known Meta Quest
Store/App Lab requirements as of early 2026 — **an operator MUST re-verify each item against the
live Meta developer docs (developers.meta.com) at submission time; requirements change.** Each item
is a checkbox: nothing here is optional for a store release.

## 1 · Technical requirements (the VRC-class checks Meta actually tests)
- [ ] **Performance:** sustained 72Hz minimum on the lowest supported device; no long hitches.
      *Ours: RuntimeHealthMonitor's `HEALTH_SLOW` + Terry's soak test = the pre-submission evidence.*
- [ ] **64-bit ARM64 IL2CPP** build (already our build config) targeting the Android API level Meta
      currently mandates — CHECK the current minimum at submission (it ratchets ~yearly).
- [ ] **App must never crash/hang** in normal flows: cold boot, pause/resume (headset doffed mid-
      anything — mid-travel, mid-mission, mid-save), Guardian interrupts, controller sleep/wake.
      *Ours: crash-proofing sweep covers save/travel; ADD doff-mid-travel to the device checklist.*
- [ ] **Boot to interactive fast** (Meta tests time-to-first-frame and time-to-interaction).
- [ ] **Guardian/boundary respected**; player height/recenter handled; app usable seated AND standing
      (our comfort row 0.4 — the presets UI is a store-facing requirement, not a nicety).
- [ ] **Controller + hand-tracking declarations correct** in the manifest (we're controller-only
      today — declare exactly that, nothing aspirational).
- [ ] **Permissions minimal:** the Android manifest must request ONLY what's used. Audit before
      submission — Unity templates love to sneak in microphone/storage.
- [ ] **Entitlement check** (Platform SDK `GetEntitlement`) in the first seconds of boot — Meta
      REJECTS builds without it. **NOT BUILT — needs the Meta Platform SDK package + app ID.**
- [ ] **Signed with a release keystore** Terry owns and has backed up (losing it = losing update
      rights). **NOT SET UP — currently debug-signed dev builds.**

## 2 · Store administration (Terry's accounts + paperwork, can start NOW)
- [ ] Meta developer organization verified (needs a payment method / phone verification).
- [ ] App created in the developer dashboard → yields the **App ID** the entitlement check needs.
- [ ] **Privacy Policy URL** — REQUIRED even if the game collects nothing. One page, hosted
      anywhere stable. Ours genuinely collects nothing (saves are local; Photon sessions relay
      transient state) — the policy just has to SAY that. *(If/when Photon online ships: name the
      relay, what transits it, and that nothing is stored.)*
- [ ] **Data Use Checkup** questionnaire in the dashboard (what data, why — ours: none/local).
- [ ] **IARC age-rating questionnaire** (free, in-dashboard). Our answers: non-lethal stylized
      combat (nothing dies on screen — the disable+salvage law is suddenly a ratings asset),
      no gore, no gambling, no user chat (hotseat is same-room).
- [ ] **Store assets:** icon set, cover art (landscape/portrait/square at Meta's exact sizes),
      5+ screenshots (in-headset capture), a trailer video, store description + keywords.
      *Flag for Picasso's lane — these are ART deliverables with hard size specs.*

## 3 · In-game store-facing gaps (rows that become REQUIRED for submission)
- [ ] Cold-boot **title screen + save slots** (HOME_HUB 2.5 remainder) — a store build can't drop
      you into the Sandbox.
- [ ] **Comfort settings player-visible** (0.4) — vignette/snap/smooth/seated; Meta comfort rating
      questionnaire asks for these explicitly.
- [ ] **Tutorial/onboarding** (map row, still ⬜) — store reviewers play cold; W000→W001 must teach.
- [ ] **Credits + licenses screen** — Unity/asset/package attributions where licenses require it.
- [ ] **Version string visible somewhere diegetic** (support/debugging of shipped builds).
- [ ] The **DevMenu and diagnostics must be locked or stripped** in release builds (a store build
      with a warp-anywhere debug menu fails review) — build-flag it, don't delete it.

## 4 · Comfort rating honesty
Meta labels apps Comfortable/Moderate/Intense. Free locomotion + flight + ziplines + jump pads =
**Moderate at best, likely Intense** unless the comfort presets (0.4) ship and default sensibly.
This affects discoverability — worth the 0.4 investment before submission, not after.

## 5 · The submission dry-run (when the above is green)
1. Release-keystore ARM64 build through the normal pipeline → upload to a **private App Lab
   channel** → Meta's automated VRC scan gives a FREE pre-review report. Fix, repeat.
2. Full device pass on the OLDEST supported Quest with the runbook checklist + the soak test.
3. Submit for review with realistic expectations: first submissions bounce on paperwork
   (privacy URL, data checkup, rating) more often than on the game itself — which is why §2
   can and should start NOW, in parallel with development.

*Map row: EXCELLENCE_MAP §8 "SHIP & STORE". Owner of §2: Terry (accounts). Owners of §1/§3: any
operator, each item is board-row sized.*
