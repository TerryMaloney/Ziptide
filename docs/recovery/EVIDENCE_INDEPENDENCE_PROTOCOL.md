# EVIDENCE INDEPENDENCE PROTOCOL — what a second green is worth
### Correlated repeats, the classify-before-rerun rule, and red-cause labeling
**Status: BINDING amendment to `RECOVERY_VERIFICATION_SYSTEM.md` §7 and §9.**
**Authored 2026-07-20 by Fable 5 (Architect) · Terry-authorized ("go ahead and fix it").**
Applies to every lane, every operator, every model. Supersedes the ad-hoc
"require 43/43 twice on the same SHA" rule introduced during the input-blocker saga.

---

## 0 · WHY THIS EXISTS (the mistake, stated plainly)

During the 2026-07-20 input blocker, the recovery PlayMode gate returned **43/43 on one run and
42/43 on a later run of the same source SHA** (`25a8136`). The response was a new authorization
rule: *require 43/43 twice on the same SHA.* I (Fable 5) then executed exactly that on the fix
candidate `c45b1a29` — using `rerun_workflow_run`, which re-runs the **same run** — and reported
the double green as strong evidence.

**That reasoning was wrong, and it is the kind of wrong that ships bugs.** A same-run rerun
holds constant: the SHA, the **cached Unity `Library`** (`key: Library-playmode-r1-v3-…`), the
runner image, the package graph, and the deterministic NUnit execution order. It is the most
*correlated* repeat obtainable. It is not a second sample; it is the same sample observed twice.

The audit that found this also measured the suite's coupling: all 43 PlayMode tests run in **one
Unity process** (`testMode: playmode`), sharing one global `InputSystem` seeded by a single
`[SetUpFixture]`/`[OneTimeSetUp]` holding **static** virtual devices
(`RecoveryPlayModeInputEnvironment.cs`). The tests are not 43 independent trials, and repeats of
the whole suite in an identical environment are not independent trials either.

**Correction of record:** the `c45b1a29` authorization was nevertheless SOUND — but for reasons
never stated at the time. See §3.

---

## 1 · THE CLASSIFY-BEFORE-RERUN RULE (the core law)

A gate that returns different verdicts for identical source has exactly two possible diseases.
They look identical and require opposite responses. **Name the disease before choosing the cure.**

| Disease | Meaning | Correct response | Wrong response |
|---|---|---|---|
| **Noisy measurement** | The system is fine; the observation is unreliable (runner flake, network, infra timeout, harness bug) | **Sample more** — and sample *independently* (§2) | Changing product code to chase a harness fault |
| **Racy system** | The system genuinely contains a timing/order/state-dependent defect | **Remove the degree of freedom** — make the property structural, not probabilistic | **Re-running until green.** This manufactures confidence: you can pass N times and still ship the race |

**The law:** *re-running is only ever valid evidence for noisy measurement. For a race, the only
valid green is one produced by a candidate in which the timing window no longer exists.*

Worked example (the input blocker, done right):
- Symptom: same SHA, 43/43 then 42/43. Disease: **racy system** — XRI's
  `OnEnable → EnableAllDirectActions` re-enabled a deliberately-empty action, and whether the
  next `ReadInput` landed inside that window depended on frame timing.
- Wrong cure: "run it twice, require two greens." The race simply may not fire twice.
- Right cure (what shipped): replace the empty action with a `default` (null) property. A null
  action **cannot** be re-enabled — the window is gone by construction. The evidence for the fix
  is the **structural argument plus a decorrelated green**, not the count of greens.

Every operator writing a fix for a nondeterministic gate must state, in the commit body or
HANDOFF entry: **`DISEASE: noisy-measurement | racy-system`**, and justify the cure accordingly.

## 2 · THE DECORRELATION RULE (what counts as a second sample)

A repeat is only evidence to the extent it **varies the axis that could be hiding the defect.**
Ranked from worthless to strong:

| Repeat type | Varies | Evidence value |
|---|---|---|
| `rerun_workflow_run` on the same run | wall-clock only | ⛔ **Not a second sample.** Do not cite it as one |
| New run, same workflow, same cached Library | timing, runner instance | 🟡 Weak — catches only coarse infra noise |
| **Clean package proof lane** (`Library` deleted, cold import) | Library state, resolve order, asset-import order, timing | ✅ **Strong — this is the canonical second sample** |
| Randomized test order (`--order=random` / NUnit seed) | intra-suite coupling | ✅ Strong for shared-state suites; **not yet wired** (§5) |
| Different runner OS/image | platform assumptions | ⚪ Rarely needed here |

**Binding rule — "warm + cold, not warm + warm":**
> When a candidate must clear a nondeterminism concern, the required pair is **one ordinary
> (warm-Library) green plus one clean-package-proof (cold-Library) green on the same source
> SHA.** A same-run rerun does not satisfy it and must not be recorded as satisfying it.

This costs nothing new: **the project already owns the cold lane.** `Recovery Clean Package
Proof` deletes `Ziptide/Library` before every Unity job and independently runs the exact 43-test
route, EditMode, patch/audit and the Golden APK. It has been treated as a heavyweight *special
case*; under this protocol it is also **the statistically valuable repeat**, and it is the one to
reach for whenever "is this green real?" is the question.

## 3 · RE-ADJUDICATION OF `c45b1a29` (the current headset candidate)

Re-checked against the corrected rule — **the authorization stands, on better evidence than was
originally cited:**

| Requirement | Evidence |
|---|---|
| Warm-Library PlayMode green | run `29786603998` — 43/43 |
| **Cold-Library PlayMode green (the real second sample)** | **clean package proof run `29786604080` — 43/43, `Library` deleted before every Unity job, `packages-lock` recorded, Input System `1.6.3` / XRI `2.4.3` resolved from cold import** |
| Ordinary CI | GREEN (EditMode + patch/world audit) |
| Golden Android | SUCCESS, run `29786604008` |
| Disease classification | **racy-system** — cured structurally (null property cannot be re-enabled), not by repetition |

The same-run rerun (attempt 2) is **struck from the evidence record** as non-probative. It did no
harm; it simply proved nothing. Nothing about the build Terry installed changes.

## 4 · RED-CAUSE LABELING (measuring the safety net itself)

Motivating measurements (2026-07-20): the recovery PlayMode gate returned **8 success / 39
failure / 13 cancelled across its last 60 runs — a 17% green rate among decisive runs**; and the
`GateGap5` board-staleness timer once red-lighted *documentation-only* pushes while the game was
healthy. The project measures the product's failure rate obsessively and the **gates'** failure
rate not at all.

**Binding, zero-cost rule:** every HANDOFF entry that reports or resolves a CI red must carry one
tag:

- **`RED-CAUSE: product`** — the game/runtime/content was genuinely wrong. The gate worked.
- **`RED-CAUSE: net`** — the gate, harness, workflow, license, timer, cache or observation
  machinery was wrong. The product was fine.
- **`RED-CAUSE: mixed`** — both, with a sentence on the split.

Purpose: make the net-versus-product ratio *visible over time*. It is the only number that can
tell us whether the safety net is paying for itself, and it bounds the **ratchet law**
(`PERCEPTUAL_GATE_PROGRAM.md` §2): gates are code, gates fail, and "add a gate per bug" therefore
has a cost curve. When `net` reds dominate a period, gate hardening outranks new gates.

## 5 · DELIBERATELY NOT DONE NOW (and why)

**Randomized test order is not being wired in this change.** It is the correct third
decorrelation axis for a shared-state suite, and it belongs in the backlog — but the PlayMode
lane is at this moment the gate authorizing an in-flight headset checkpoint, and modifying a
live authorization gate to study it would violate the project's own change discipline. Sequence:
Terry's device pass closes → recovery exits → **then** add NUnit order randomization behind its
own bounded PR, with the first randomized run treated as a *new* baseline, not a regression.

Likewise, splitting the 43-test suite into isolated processes would decouple the tests properly,
but it changes run cost and evidence shape; propose it after recovery exit.

## 6 · WHAT CHANGES IN PRACTICE (operator quick card)

1. A gate flips green/red on identical source → **classify first** (`racy-system` vs
   `noisy-measurement`). Write the classification down.
2. Racy → **fix the structure.** Never authorize on repetition alone.
3. Need a second sample → **run the cold lane.** Never cite a same-run rerun.
4. Report any red with **`RED-CAUSE:`**.
5. When you cite evidence, cite *which axis was varied* — not just how many greens you got.
