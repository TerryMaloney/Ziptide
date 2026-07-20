# FINISHED-GAME BENCHMARK — what "done" means for a shipped Quest title, vs where Ziptide actually is
### The completeness checklist no current plan covers, evidence-based gap verdicts, and the tiered close-out order
**Status: RESEARCH / GAP AUDIT — planning only, zero code. 2026-07-20, Terry-directed (Fable run).**
**⚠️ Multi-model protocol:** Terry is running this same study across models. Merge by section;
every gap verdict below carries repo evidence so disagreements resolve by re-checking, not by
argument (rb43's rule: testable claims decided by tests). Sibling docs should ADD rows, not
re-litigate verified ones.

---

## 0 · METHOD

Benchmark assembled from three sources: Meta's Quest store technical requirements (the VRC
classes every store submission is tested against), the de-facto commercial completeness bar for
premium VR titles (what players/reviews punish when absent), and standard live-title operations.
Then every item was checked against the repo (greps run 2026-07-20 — pause/entitlement/credits/
settings/telemetry/recenter/version probes) and against every active plan doc. Verdicts:
**✅ HAVE** (exists, evidence) · **🟠 PARTIAL** · **📋 PLANNED** (a real doc/lane owns it) ·
**❌ UNPLANNED** (no doc, no lane — the reason this study exists).

## 1 · TIER 0 — STORE-BLOCKING (cannot ship at all; Meta cert tests these)

| # | Benchmark item | Verdict | Evidence / owner |
|---|---|---|---|
| 0.1 | **Entitlement check** at launch (Platform SDK verifies purchase) | ❌ | zero `Oculus.Platform`/entitlement references in `Assets/Ziptide` |
| 0.2 | **App lifecycle**: HMD doff → immediate pause + audio mute; focus loss (universal menu) → pause; resume restores exactly | 🟠 | save-on-pause exists (`SaveSystem.OnApplicationPause`) — but no pause *state* (world keeps running), no audio duck/mute |
| 0.3 | **Tracking & boundary**: guardian intrusion, tracking loss, **recenter** handled without breaking rig/UI | ❌ | zero recenter/boundary/tracking-loss handling in code |
| 0.4 | **Controller lifecycle**: disconnect/reconnect/battery-death mid-play → prompt + graceful re-grab | ❌ | nothing handles input device loss |
| 0.5 | **Performance floor**: 72 FPS sustained, no >150 ms hitches in cert routes | 🟠 | budgets + `RuntimeHealthMonitor` + PG gates exist; needs a device soak protocol (0.5 h/2 h sessions, thermal/battery curve) — no doc owns soak |
| 0.6 | **Store listing kit**: icons (3 sizes), cover art, ≥5 screenshots, trailer, description, comfort rating declaration | ❌ | nothing exists; comfort *presets* are designed (📋) but the store's Comfortable/Moderate/Intense **rating** is undeclared |
| 0.7 | **IARC age rating** questionnaire + **privacy policy URL** + data-safety declaration | ❌ | no doc |
| 0.8 | **Audio licensing clean** for all shipped music/SFX | 📋 | the music-license law docs exist and already ruled free-tier remixes unsafe (rb-lane commits 2026-07-18) — the ONE ops item a plan owns |
| 0.9 | **Version identity**: visible build/version string in-app; update-safe saves | 🟠 | save migration law is real (`ProfileSerializer` schema versions ✅); no version display anywhere (`Application.version` unused) |

## 2 · TIER 1 — PLAYER-EXPECTED (shippable without, but reviews and refunds punish it)

| # | Benchmark item | Verdict | Evidence / owner |
|---|---|---|---|
| 1.1 | **Title/home flow**: New / Continue / Settings from boot | 📋 | FH-S07 — code CI-green, bake pending |
| 1.2 | **In-game pause/system menu** on menu button: resume, settings, return-to-ship, quit-safely | ❌ | no pause menu exists (DevMenu is a dev tool, not this) |
| 1.3 | **Settings — audio**: master/music/SFX/voice sliders | ❌ | `AudioDirector`/`AudioProfile` have no mix bus or user volumes |
| 1.4 | **Settings — comfort**: vignette, snap-turn angle, smooth-loco toggle, seated mode, **height recalibration**, kid mode | 📋/❌ | presets designed (`COMFORT_AND_ACCESSIBILITY.md`, FH-S07 surfaces); seated mode + height recal + in-session recenter are in NO plan |
| 1.5 | **Settings — handedness**: full left-hand swap (belt, holsters, dominant ray) | ❌ | nothing; VR-critical (~10% of players) |
| 1.6 | **Subtitles**: toggle + size options; all spoken/radio audio captioned | 🟠 | caption v2 spec exists (RILL lane); no toggle/size setting surface |
| 1.7 | **Save management**: confirm-overwrite on NEW GAME, delete/reset save, corrupt-save recovery | 🟠 | robust single-profile overlay saves ✅; no confirm/delete/recovery UX |
| 1.8 | **Onboarding** that teaches every verb without reading | 📋 | the entire first-hour program — best-covered item in the game |
| 1.9 | **A completable arc**: win state, **ending scene, credits roll**, "what's next" hook | ❌ | no ending, no credits roll (`CreditsHud` is currency), no completion state — the 80-world vision has no END defined for v1 |
| 1.10 | **Difficulty handling**: at minimum an assist/kid mode and a failure policy that never soft-locks | 🟠 | PvP ladder scales (✅ tested); campaign difficulty/assist unowned beyond the DC §9 kid-wave idea |
| 1.11 | **Haptics coverage law**: every grab/hit/shot/UI press has a haptic + audio response | 🟠 | strong on weapons/belt lanes; no coverage *law*/audit like the diag-tag law |
| 1.12 | **Loading states**: no dead black screens; travel covered by crest hold | ✅ | ASYNC travel lane, device-verified pending |
| 1.13 | **Failure recovery**: fall-out-of-world net, stuck rescue | ✅ | BOOT_HOLD/fall recovery + spawn safety, device-proven in recovery |

## 3 · TIER 2 — COMPETITIVE POLISH (what separates "good" from "store-featured")

| # | Item | Verdict | Note |
|---|---|---|---|
| 2.1 | Achievements (platform) + in-game stats page (playtime, worlds, photos) | ❌ | photo album + PvP stats are natural feeders; no platform hookup |
| 2.2 | Cloud backup of saves (platform cloud save API) | ❌ | one headset re-flash from losing everything |
| 2.3 | Crash/ANR telemetry + opt-in analytics funnel (first-hour completion %!) | ❌ | `RuntimeHealthMonitor` is local-only; the first-hour contract DESERVES funnel data |
| 2.4 | Localization decision (even "English-only v1" is a decision with a doc) | ❌ | undecided; affects every authored string (RILL lines are data ✅ — good position) |
| 2.5 | Soundtrack completeness: music per world/state + stingers + mix pass | 📋 | music genome + Suno lanes (rb51/52) — composition covered, mix bus is 1.3's gap |
| 2.6 | Social proof: photo sharing (Field Camera → Quest gallery export), trailer capture path | 🟠 | Field Camera ships photos to disk; no export/share; trailer needs 0.6 anyway |

## 4 · THE HEADLINE — what NO current plan covers (Terry's instinct, confirmed)

The project's plans are deep on **world/content quality** (forge, worlds, first hour, factory,
gates) and near-silent on the **shell of a shipped product**. The unowned set, consolidated:
**entitlement · lifecycle pause/mute · tracking/boundary/recenter · controller loss · pause menu
· audio mix bus + sliders · handedness · seated/height · save-management UX · ending + credits ·
soak protocol · store kit · IARC/privacy · version display · cloud saves · telemetry ·
achievements · localization decision.**

None of this is glamorous; all of it is why players say a game "feels finished." Almost all of
it is also **exactly one system each, pure-core-friendly, LLM-buildable, and testable in CI** —
the same factory that builds worlds can build the shell.

## 5 · PROPOSED CLOSE-OUT STRUCTURE (for the multi-model merge to refine)

- **New lane: "SHIP SHELL"** (SPRINT_SHIP_SHELL.md when adjudicated) owning §1 + §2 items as
  numbered envelopes (SS-01 entitlement … SS-18 localization decision). Most are 1–3 commit
  systems; the pause/system menu (1.2) + settings surfaces (1.3–1.6) form one coherent
  mini-program on top of FH-S07's surfaces and should land together.
- **Order logic:** (a) 0.2/0.3/0.4 lifecycle+tracking first — they're *correctness* and get
  cheaper before more systems exist; (b) pause+settings mini-program next — every device session
  benefits; (c) ending/credits when the first-hour v2.1 story locks (the DC already defines the
  hour's end — v1 "ending" = a defined stopping beat + credits, not 80 worlds); (d) store
  kit/IARC/privacy last-mile with a real ship date; (e) telemetry EARLY if Terry wants first-hour
  funnel data from testers — it changes what we learn from every session after it lands.
- **Gate it like everything else:** each SS envelope gets EditMode tests where pure (mix math,
  handedness mapping, save-slot state machine) + a PG-5 sheet where visual + a runbook device
  line. Add an EXCELLENCE_MAP row **"Ship shell"** so the map stops hiding this hole.
- **Benchmark ratchet:** this table becomes a checked list; a lane isn't "done" adding shell
  features — the game is done when §1 is all ✅ and §2 has explicit ship/cut decisions.

## 5b · SIBLING ADDITIONS (Fable rb-lane run, `FINISHED_GAME_BENCHMARK_RB.md` — ADD-only per §0)

The rb-lane run scored ten coarser categories; most overlap rows above. Its rows NOT covered
here, added per the merge protocol:
- **NAME/IP CLEARANCE:** "Ziptide" has never been trademark/store-collision searched. An
  afternoon, freeze-compatible, and every day of delay raises the cost of a bad answer. (⚖
  Terry runs it — HIGH.)
- **Business decisions on record:** price point, launch-discount posture, explicit no-MTX
  stance — implied by the tone charter everywhere, DECIDED nowhere. One ⚖ page.
- **Cut list + bug-severity ship bar as artifacts:** we defer well but never DECLARE cuts, and
  gates enforce continuous quality without a discrete ship bar (zero S1/S2, ≤N S3). Two ⚖
  pages.
- **Review-response/community policy** (beyond telemetry): support contact, FAQ, review-reply
  posture — the human half of §2.3's live-ops.
- **THE CLASS LAW + `docs/MISS_LEDGER.md`** (rb-lane's process contribution, now live): every
  miss — bug, omission, blind spot — gets a 5-field ledger entry (WHAT · FOUND BY · **WHY
  MISSED** · CLASS · SYSTEM CHANGE); a fix without a system change is a loan; entries close
  only on verified system changes. The ledger is seeded with both benchmark runs' meta-finding:
  categories were missed because planning grew game-outward and nothing forced ship-backward —
  system change = the ledger + this benchmark as a recurring milestone gate + the
  EXCELLENCE_MAP rows (§5's "Ship shell" row satisfies this).

## 6 · ACCEPTANCE FOR THIS STUDY
Merged-doc version adjudicated by Terry → SHIP SHELL lane exists with owners → every ❌ above is
either an envelope or an explicit CUT decision recorded here. The finished-game test, stated
once: *a stranger buys it, plays one hour, doffs mid-fight, resumes at dinner, adjusts volume,
swaps hands, finishes the arc, sees credits, and never once meets a developer artifact.*
