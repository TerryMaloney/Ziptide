# ⚡ THE FAST LANE — how a three-line fix stops costing six hours

**Written 2026-07-28 after Terry's velocity complaint: three trivial fixes (rotate a sword, two
small things) plus install instructions took SIX HOURS.** That is a project-ending rate and this
document exists to end it. It does not remove any safety that protects the shipped game; it stops
charging 80-world governance to a ten-line change.

---

## 1. The measured diagnosis

| Cost | Evidence |
|---|---|
| **Read-in tax, paid EVERY session** | `docs/HANDOFF.md` is **3,155 lines / 138 entries**, and the laws require reading it plus `CLAUDE.md` (108) + `OPERATOR_START_HERE.md` (147) + `CURRENT_EXECUTION_CHECKLIST.md` (215) = **~3,600 lines before any code** |
| **Ceremony per change** | HANDOFF entry · MISS_LEDGER check · ratchet law (every fix needs a gate) · board re-dating · catalog reconcile · EXCELLENCE_MAP row — all applied equally to a sword angle and to a new subsystem |
| **CI latency** | median **60 min** verdict-to-verdict; p75 **176 min** (measured over the last 20 verdicts) |
| **Red-tree tax** | one continuous **27-hour** RED streak (2026-07-25 17:18 → 07-26 20:19) caused by a **stale board row**, not code — discovered only by manual bisect |
| **Serialised workflows** | ordinary CI → Recovery PlayMode → Recovery Golden Android, three separate runs before an installable APK exists |
| **Hand-written install steps** | regenerated from scratch per build instead of being a committed script |

**Root cause in one line: we built a factory for eighty worlds and we are running single screws
through it.**

---

## 2. THE THREE RULES (adopt immediately)

### RULE 1 — Terry never waits on an operator for a build
`recovery-golden-android.yml` already has `workflow_dispatch`. **From the GitHub mobile app:
Actions → Recovery Golden Android → Run workflow → terry-local-wip.** ~12 minutes later the APK
artifact is there. No operator, no session, no read-in.

### RULE 2 — One permanent installer, never regenerated
`tools/install_latest.ps1`. Finds the newest APK, prints its SHA-256, **uninstalls first** (the
signature law), clears logcat, installs, and prints the logging command. Works with one headset or
two (`-Serial`, `-Both`). Nobody writes install instructions by hand again.

```powershell
.\tools\install_latest.ps1
```

### RULE 3 — Batch the fixes, never the builds
An APK costs ~1 hour of machine time whether it carries one fix or ten. **Never spend a build on a
single minor fix.** Fixes accumulate in the fast lane until either (a) five are queued, or (b) Terry
asks for a build. One build, one device session, one verdict covering everything.

---

## 3. THE FAST LANE — what it is and when it applies

A change qualifies for the fast lane when **ALL** of these are true:

- touches **≤3 files and ≤30 lines**;
- introduces **no new system, contract, schema, or gate**;
- does **not** touch: XR rig ownership · travel/`TravelCoordinator` · save/persistence · input
  actions · build config/signing · recovery artifacts;
- is **revertible in a single `git revert`**;
- its correctness is decided by **compile + existing tests + Terry's eyes on device** — not by new
  automated proof.

Rotating a weapon 180°, nudging a spawn position, changing a colour, adjusting a scale value, fixing
a label: all fast lane.

### What the fast lane WAIVES (explicitly, by this document's authority)

| Normally required | In the fast lane |
|---|---|
| A full HANDOFF entry | **One line** appended to `docs/FAST_LANE_LOG.md` — `date · what · files · why` |
| MISS_LEDGER entry | **Waived** unless the same class has now happened twice |
| The ratchet law (fix + gate) | **Deferred.** Log it as debt in the fast-lane log; a gate is only required when the class repeats |
| EXCELLENCE_MAP row update | **Waived** |
| Board re-dating / catalog reconcile | **Waived** |
| A dedicated build | **Waived** — batched per RULE 3 |

### What the fast lane NEVER waives
Compile must pass. Existing tests must pass. The change must be revertible. And the standing
report-only law still holds: rig, travel, input, persistence and build config are **never** fast
lane, no matter how small the diff looks.

---

## 4. THE MECHANICAL FIXES (each removes minutes from every future cycle)

Ordered by minutes saved per unit of work. None of these change what the gates *check* — only how
fast they report.

1. **Archive `HANDOFF.md`.** Keep the newest ~10 entries; move the rest to
   `docs/HANDOFF_ARCHIVE_2026-07.md`. This is a **10× cut to the per-session read-in tax**, paid
   back on every session by every operator forever. Highest-value single action on this list.
2. **Move doc/board staleness checks OUT of the Unity EditMode suite.**
   `GateGapTests.GateGap5_NoBoardClaim_RotsSilently` is an EditMode test, so a 15-day-old board row
   costs a full Unity boot and a 1,171-test run to report — and blocks the pipeline while it does.
   The identical check already exists in `tools/factory_governance_gate.py` and runs in ~1 second.
   **Any check that only reads `docs/**` belongs in the python preflight, never in Unity.**
   This one change would have prevented the 27-hour red streak.
3. **Add a `concurrency` group to `ci.yml`** (`recovery-golden-android.yml` already has one).
   Superseded pushes currently burn full Unity runs nobody will read.
4. **Make the durable verdict name the failing test.** The current verdict says only RED; finding
   which of 1,171 tests failed required downloading and parsing the NUnit XML by hand. Printing the
   failed test name into `CI_VERDICT.md` turns a 20-minute investigation into a glance.
5. **Sequence the device chain behind one dispatch.** A single `workflow_dispatch` that runs
   CI → PlayMode → Golden APK unattended and reports the artifact + hash, so the human waits once
   rather than shepherding three runs.

---

## 5. The standard that replaces "how long will this take?"

- A **fast-lane fix**: authored in minutes, no ceremony, waits for the next batch.
- A **batch build**: Terry dispatches it himself, ~12 minutes, installer script, one device session.
- A **structural change** (new system, contract, or anything on the report-only list): full
  ceremony, full gates, full evidence. That machinery is *correct* — it just must stop being
  charged to trivia.

**The test of whether this worked:** the next time Terry asks for three small fixes, the answer is
"they're in the batch, dispatch a build whenever you want one" — measured in minutes of operator
time, not hours.
