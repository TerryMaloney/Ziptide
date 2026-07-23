# ✅ VRC TEST PLAN — Meta Horizon Store certification, run as a checklist before we ever submit

**What this is:** every Virtual Reality Check (VRC) Meta tests a store submission against, as a
living pass/fail plan mapped onto OUR systems, gates and diagnostics — so cert is a checklist we
ratchet green over time, never a surprise rejection.
**Source of truth:** verified against the official requirement list at
`developers.meta.com/horizon/resources/publish-quest-req/` on **2026-07-23** (IDs + required/
recommended status below are from that fetch). Meta revises VRCs — **re-verify against the live
page at each milestone re-score and before submission.** Meta also publishes a downloadable VRC
test-plan spreadsheet and the **VRC Validator** tool; adopt both in the release lane (M8).
**How to use:** each row = precondition → check → our evidence. Mark ⬜ untested · ✅ pass ·
❌ fail (❌ = MISS_LEDGER entry + ratchet per the Class Law). Companion docs:
`DEVICE_TEST_CHECKLIST.md` §10 (the on-device interrupt matrix) · `META_STORE_READINESS.md`
(paperwork/assets) · `FINISHED_GAME_BENCHMARK.md` (the full ship bar).

Legend: **[R]** = Required (cert-blocking) · **[rec]** = Recommended (quality-rated, not
blocking). Retired IDs (Performance.2, Functional.8/11, Input.6) omitted.

---

## 1 · Packaging — all [R]
| ⬜ | ID | Check | Our status / evidence |
|---|---|---|---|
| ⬜ | Packaging.1 | Release-build manifest conforms (no debug flags, correct intent filters) | 🕳️ release build-flag audit is a named EXCELLENCE_MAP gap — build it in the release lane |
| ⬜ | Packaging.2 | APK signature scheme v2 | ⚠ currently DEBUG-signed; release keystore NOT SET UP (`META_STORE_READINESS.md`) — keystore + backup rule first |
| ⬜ | Packaging.3 | No unsupported Android feature requirements | verify manifest at release audit |
| ⬜ | Packaging.4 | Supported engine version | Unity 2022.3 LTS ✅ today — re-check support window at submission |
| ⬜ | Packaging.5 | APK < 1 GB (OBB < 4 GB) | fine today (procedural art); watch when art/audio land — add size print to build log |
| ⬜ | Packaging.6 | 64-bit only | ARM64/IL2CPP already our build config ✅ |

## 2 · Performance
| ⬜ | ID | Check | Our status / evidence |
|---|---|---|---|
| ⬜ | Performance.1 [R] | Runs at specified refresh rate (72 Hz) sustained — **release build, on device, under thermal load** | `RuntimeHealthMonitor` `HEALTH_SLOW`/1%-low + DEVICE_TEST_CHECKLIST §10 thermal row; never editor numbers |
| ⬜ | Performance.3 [R] | Head-tracked graphics or a VR loading indicator **within 4 s of launch** | boot currently lands in Sandbox dev-bypass; measure cold-boot-to-first-tracked-frame (§10 row 1); async-travel crest is the in-session pattern ✅ |
| ⬜ | Performance.4 [rec] | ≥85% render scaling | check render scale setting at release |

## 3 · Functional
| ⬜ | ID | Check | Our status / evidence |
|---|---|---|---|
| ⬜ | Functional.1 [R] | Installs + runs, no crashes/freezes | golden APK + PlayMode routes + §10 soak |
| ⬜ | Functional.2 [R] | **Single-player app PAUSES when OS requests** (doff, Universal Menu) | ⚠ save-on-pause exists (`SaveSystem.OnApplicationPause`) but NO pause state — world keeps running, audio keeps playing. Known gap (benchmark row 0.2); a top real-world cert failure |
| ⬜ | Functional.3 [R] | User can never get STUCK (always progress or restart) | fall-safety + BOOT_HOLD ✅; §10 adds stuck-state probes |
| ⬜ | Functional.4 [R] | Never loses user data | atomic saves + backups + migrations ✅ (`SaveFileStore`, `ProfileSerializer`); §10 low-storage + update-over-install rows extend proof |
| ⬜ | Functional.5 [R] | Responds to positional tracking/orientation correctly | XR rig ✅; §10 Guardian/tracking-loss rows |
| ⬜ | Functional.6 [R] | Assets show only Meta headsets/controllers | check store assets at creation |
| ⬜ | Functional.7 [rec] | Notify when internet is required | game is offline-first ✅; add one authored offline notice if any online feature ships |
| ⬜ | Functional.9 [R] | Forward-orientation reset supported (Local tracking) | ⚠ no recenter handling in code — known ❌ (benchmark 0.3); pair with height-recal in the settings program |
| ⬜ | Functional.10 [rec] | Avoid headlocked UI | RILL caption v2 spec is lazy-follow (not headlocked) ✅ by design; wrist HUD diegetic ✅ |
| ⬜ | Functional.12 [R] | Works for multiple entitled users on one headset | ties to FAMILY_PROFILES plan — test once profiles land |
| ⬜ | Functional.13 [rec] | Localized apps default sensibly | English-only decision ✅ on record (`LOCALIZATION_DECISION.md`) |
| ⬜ | Functional.14 [R] | Passthrough apps show passthrough loading | N/A unless we declare passthrough; correct launch-from-passthrough-home still tested in §10 |

## 4 · Input
| ⬜ | ID | Check | Our status / evidence |
|---|---|---|---|
| ⬜ | Input.4 [R] | **Focus-aware**: keep rendering while system UI is up, hide hands/controllers, ignore input | ⚠ unimplemented — the OTHER top real-world failure; build with Functional.2 as one "system-focus" envelope |
| ⬜ | Input.7 [R] | Clean input switching controllers ↔ hands (if hand tracking declared) | simplest v1: do NOT declare hand tracking; row becomes N/A |
| ⬜ | Input.8 [R] | System gesture reserved | verify no binding collides |
| ⬜ | Input.1/2/3 [rec] | Menu on menu-button · grip-button grab · virtual hands align | grip-grab ✅ (XRI); menu-button → pause menu when the shell lands |

## 5 · Tracking — all [R]
| ⬜ | ID | Check | Our status / evidence |
|---|---|---|---|
| ⬜ | Tracking.1 | Metadata declares sitting/standing/roomscale truthfully | decide with the comfort/seated program (⚖ with device-target page) |
| ⬜ | Tracking.2 | Metadata matches supported input modes | with Tracking.1 |

## 6 · Security & Privacy
| ⬜ | ID | Check | Our status / evidence |
|---|---|---|---|
| ⬜ | Security.1 [rec-but-treat-as-R] | Entitlement check ≤10 s of launch | ⚠ zero Platform SDK in project — needs App ID (`META_STORE_READINESS.md`); postmortems treat absence as a failure |
| ⬜ | Security.2 [R] | Minimum Android permissions | audit manifest at release (pairs Packaging.1) |
| ⬜ | Privacy.1–4 [R] | Hosted privacy policy URL; what's collected, why, deletion path | policy not written; our local-only telemetry stance makes it SHORT — write with store paperwork |
| ⬜ | Privacy.5 [R] | Data Use Checkup clears (annual!) | Terry dashboard task; set a recurring reminder — lapse = store removal |

## 7 · Audio & Accessibility [all rec — quality-rated]
| ⬜ | ID | Check | Our status / evidence |
|---|---|---|---|
| ⬜ | Audio.1 | 3D spatialization | spatialize when the SFX Forge lands |
| ⬜ | Accessibility.1 | Playable without audio | mostly true (captions carry story) — keep it a design law |
| ⬜ | Accessibility.2 | Text/controls legible | `UiReadabilityAuditRules` ✅ exists — our gate predates the VRC |
| ⬜ | Accessibility.3 | Visual+audio+haptic redundancy | HAPTIC_COVERAGE + "redundancy not modes" law ✅ designed |
| ⬜ | Accessibility.4 | One-handed play consideration | known ❌ (benchmark); revisit at settings program |
| ⬜ | Accessibility.5/6 | Brightness/contrast · colorblind options | known ❌; colorblind = never-color-only signaling first |
| ⬜ | Accessibility.7/8 | Turn without head movement (snap ✅) · multiple locomotion styles (✅ presets) | largely ✅ already |
| ⬜ | Accessibility.9 | Sitting/standing: all interactions reachable from fixed position | seated mode + PG-4 reach audit — the kid-height program doubles as this row |

## 8 · Assets & Publishing — all at store-page creation
| ⬜ | ID | Check | Note |
|---|---|---|---|
| ⬜ | Asset.1–10 | Transparent logo · clean cover art (no extraneous text; [rec] none in top/bottom 20%) · genuine unretouched screenshots · trailer ≤ 2 min · no other platform logos · ≥24pt art text | the #1 real-world first-submission failure class — build assets TO this list, don't retrofit |
| ⬜ | Publishing.1–8 | Valid website URL · **valid support page URL** · **valid Terms of Service URL** · name/descriptions/keywords within guidelines · Meta brand compliance | NOTE: ToS + support page are REQUIRED — raises benchmark's "EULA [SHOULD]" to cert-blocking; add both to META_STORE_READINESS |
| ⬜ | Content.1–2 [R] | Content guidelines; metadata matches in-app content | with IARC honesty rule |
| — | Content.3–4 | UGC reporting/hiding | N/A — no UGC |
| — | Ads.1–7 · Streaming.1–4 | Ads / cloud-streaming rules | N/A — no ads ever (Mixed Ages requires it); no streaming |

## 9 · Beyond VRC (same submission, different checklists)
IARC questionnaire (honest; E10 expected) · **age-group self-certification = Mixed Ages** (+ Get
Age Category API within 30 days if Platform SDK features ship) · store metadata comfort rating
matching reality · Data Use Checkup annual renewal. Details: `FINISHED_GAME_BENCHMARK.md` §5c +
`FINISHED_GAME_BENCHMARK_F5C.md` §2a.

## 10 · The standing gaps this plan exposes (consolidated, build-order suggestion)
1. **System-focus envelope** (Functional.2 + Input.4 together): pause state + audio mute + focus-
   aware rendering + input ignore. One coherent runtime feature; the top two real-world failures.
2. **Recenter/orientation reset** (Functional.9) — with the settings/height program.
3. **Entitlement + Platform SDK** (Security.1) — needs Terry's App ID; carries Mixed Ages API.
4. **Release build hygiene** (Packaging.1/2 + Security.2): keystore + flag audit + permission
   audit — the named 🕳️ EXCELLENCE_MAP row.
5. **ToS + support page URLs** (Publishing.2/3) — newly discovered as REQUIRED; paperwork lane.
6. Store asset kit built to Asset.1–10 from day one.
