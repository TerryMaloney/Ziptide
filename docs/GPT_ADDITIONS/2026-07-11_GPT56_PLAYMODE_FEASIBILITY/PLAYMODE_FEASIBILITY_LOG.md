# PLAYMODE / TRAVEL ROUND-TRIP FEASIBILITY LOG

**Owner:** GPT-5.6 Thinking, quality/architecture workstream  
**Authorized by:** `CURRENT_EXECUTION_CHECKLIST.md` §7 item 8, 2026-07-11  
**Branch:** `terry-local-wip`  
**Status:** ⏸️ FEASIBILITY CHECK COMPLETE — NOT ELIGIBLE IN CURRENT CI

## Question

Can the repository safely add a small PlayMode scaffold / `TravelCoordinator` round-trip test **in the existing CI environment**, without scene YAML edits, Quest hardware, timing flakiness or a second unproven test lane?

## Evidence

- `.github/workflows/ci.yml` explicitly describes and runs **EditMode tests** only.
- The Unity test job passes `testMode: editmode`; there is no PlayMode job, matrix entry or PlayMode artifact lane.
- Repository commit search found no PlayMode history to demonstrate a previously stable runner contract.
- Repository code search exposed no existing PlayMode scaffold/assembly; the public repository is not code-search indexed, so absence cannot be treated as architectural proof, but there is no positive evidence to justify changing CI.
- The checklist itself conditions this task on stability **in the existing CI environment**. Adding a new `testMode: playmode` job would change that environment rather than use an already-proven lane.
- A meaningful `TravelCoordinator` round trip would also cross scene loading, persistent rig restoration and asynchronous timing. Building that before proving a minimal PlayMode test exits cleanly would combine infrastructure and travel behavior into one noisy failure surface.

## Decision

Do **not** add the travel test or a PlayMode CI job in this pass.

This is a deliberate stop-rule result, not unfinished implementation. The independent CI-only queue is exhausted without creating an unreliable test system.

## Promotion criteria

The row becomes eligible only after a separate, intentionally approved CI-infrastructure task proves all of the following:

1. a minimal PlayMode smoke test runs on Unity `2022.3.62f3` in GameCI and exits cleanly;
2. the PlayMode job has its own cache/artifact names and does not weaken required EditMode status;
3. one green run is repeated after an unrelated branch commit to rule out one-off success;
4. the scaffold requires no checked-in scene YAML and no XR/Quest device;
5. only then is a small TravelCoordinator round-trip test added, using deterministic seams rather than real-time sleeps.

## Handoff — Did / Next / Heads-up / Commits

**Did:** inspected the live CI contract and repository evidence, applied the checklist’s conditional stop rule, and avoided introducing an unproven second Unity test lane.

**Next:** the next productive work is no longer an independent CI-only queue item. Continue with an explicitly claimed gameplay/content envelope, Picasso-independent documentation, or Terry’s consolidated PC/headset batch; do not quietly smuggle PlayMode infrastructure into an unrelated change.

**Heads-up:** EditMode remains the only required Unity test result in the durable verdict. A future PlayMode lane is useful, but it is CI infrastructure and should be reviewed as such—not treated as a tiny travel test.

**Commits:** this feasibility/stop-rule record only; no runtime, workflow, scene or test code changed.
