# MISS LEDGER — every miss is a class, every class gets a system change

**The Class Law (Terry, 2026-07-19):** any acute issue is a systemic issue. Every miss — bug,
omission, planning blind spot, process failure — is logged here with the five fields, and the
entry is not CLOSED until its SYSTEM CHANGE exists. A fix without a system change is a loan.
Format per entry: **WHAT · FOUND BY · WHY MISSED · CLASS · SYSTEM CHANGE (→ status)**.
This generalizes hwr31's device-bug ratchet ("every bug dies three deaths") to everything.
Read by every lane at session start alongside HANDOFF. Full spec: `FINISHED_GAME_BENCHMARK.md` §3.

---

## OPEN / SEEDED 2026-07-19 (the benchmark exercise's own findings — entries 1–14)

1. **WHAT:** no player-facing credits roll designed. **FOUND BY:** finished-game benchmark (B3).
   **WHY MISSED:** CREDITS.md solved the LICENSING need and its existence masked the
   EXPERIENCE need — one name, two artifacts. **CLASS:** a bookkeeping artifact masking a
   player-facing artifact of the same name. **SYSTEM CHANGE:** benchmark B3 row + EXCELLENCE_MAP
   presentation row (→ pending); credits-roll design queued with title-menu family.
2. **WHAT:** no legal/boot screens (health & safety, licenses, studio identity). **FOUND BY:**
   benchmark (B3). **WHY MISSED:** cert list covered the REQUIREMENT; nobody owned the DESIGN.
   **CLASS:** compliance item with no experience owner. **SYSTEM CHANGE:** same as #1.
3. **WHAT:** player-visible error states (save-corrupt, entitlement-fail, storage-full) have no
   authored fiction. **FOUND BY:** benchmark (B3). **WHY MISSED:** error handling lived in
   engineering docs; the FICTION layer had no slot for failure. **CLASS:** engineering states
   without presentation states. **SYSTEM CHANGE:** error-state line kit added to the voice-
   formula scope (assembly GAP E) (→ pending).
4. **WHAT:** the ending EXPERIENCE (roll-credits moment, post-credits state, completion
   ceremony, replay answer) undesigned while ending STORY is fully designed. **FOUND BY:**
   benchmark (B5). **WHY MISSED:** story lane owned the endings' MEANING; no lane owned their
   STAGING; first-hour got a Director's Cut, last-hour never did. **CLASS:** asymmetric care —
   entrances polished, exits assumed. **SYSTEM CHANGE:** benchmark B5 row +
   "LAST_HOUR_DIRECTORS_CUT" queued as a design doc (→ pending); EXCELLENCE_MAP row.
5. **WHAT:** "Ziptide" never trademark/store-collision searched. **FOUND BY:** benchmark (B6).
   **WHY MISSED:** legal-compliance list was platform-shaped (what Meta asks) — nothing asked
   what the WORLD asks. **CLASS:** compliance scoped to the platform's questionnaire.
   **SYSTEM CHANGE:** B6 gains name/IP clearance row (→ pending: Terry runs the search — an
   afternoon, HIGH priority, freeze-compatible).
6. **WHAT:** no marketing campaign plan (page copy, shot list, trailer beats, press kit,
   launch calendar, demo decision). **FOUND BY:** benchmark (B7). **WHY MISSED:** DC-4 built
   capture TOOLING and read as "marketing covered." **CLASS:** tooling mistaken for the
   deliverable it enables. **SYSTEM CHANGE:** B7 row; campaign doc queued (→ pending).
7. **WHAT:** business decisions (price, discount posture, no-MTX stance) not on record.
   **FOUND BY:** benchmark (B8). **WHY MISSED:** premium/no-MTX was implied by tone-charter
   values, never DECIDED. **CLASS:** values mistaken for decisions. **SYSTEM CHANGE:** B8 ⚖
   one-pager queued for Terry (→ pending).
8. **WHAT:** no live-ops plan (patch cadence, triage route, support channel, review policy).
   **FOUND BY:** benchmark (B9). **WHY MISSED:** every plan ends at "device pass"/launch;
   nothing forced the ship-backward view past day one. **CLASS:** planning horizon ends at
   launch. **SYSTEM CHANGE:** B9 row + live-ops one-pager queued (→ pending).
9. **WHAT:** no written bug-severity ship bar / cut list. **FOUND BY:** benchmark (B4/B1).
   **WHY MISSED:** gates enforce QUALITY continuously, so nobody defined the discrete SHIP
   BAR; cuts were deferred implicitly, never declared. **CLASS:** continuous discipline
   masking a discrete decision. **SYSTEM CHANGE:** severity-bar + CUT_LIST.md ⚖ pages queued
   (→ pending).
10. **WHAT (meta):** whole benchmark categories (B5/B7/B8/B9) were absent from ALL planning
    passes including six prior audits. **FOUND BY:** this exercise. **WHY MISSED:** planning
    grew game-outward (mechanics→art→content); nothing forced ship-backward. **CLASS:**
    inside-out planning blindness. **SYSTEM CHANGE:** ✅ THIS LEDGER + the benchmark as a
    recurring milestone gate (checklist row) + EXCELLENCE_MAP rows per category — the
    ship-backward view now has standing machinery.
11. **WHAT:** the recovery PlayMode performance route repeatedly failed input settle during
    cold `_Boot` and first W000 arrival. **FOUND BY:** 42/43 on `0f01fc5`, `be4a6b98`,
    `dfeddc15`, `c766c24`, `77870ebc`, `f177b1a`, and `a3bdfd5`. **WHY MISSED:** early fixes
    correctly repaired test-device ordering and proved all eight canonical Move/Turn actions were
    bound and readable at simulator-ready and every `XRI_WIRING` boundary (`repaired=0`). The
    production fail-closed probe was nevertheless hard-coded to call `ReadValue<Vector2>()` on
    every `InputActionProperty`. A valid scalar/Button/Axis action therefore produced the same
    `InvalidOperationException` classification as a genuinely stale `InputActionState`, making the
    settle predicate impossible while later steady-state route tests appeared healthy. **CLASS:** a
    safety probe whose observation is not semantically equivalent to the consumer contract; value-
    type mismatch conflated with state corruption. **SYSTEM CHANGE:** `InputActionReadKindCore`
    resolves scalar/Vector2/object reads from declared and runtime value types with EditMode coverage;
    `PlayerRigPersistence.ActionReadsSafely` now reads accordingly and records exact provider, hand,
    action path, expected/runtime types and exception on a true failure. The two unsuccessful test-
    only repair owners were removed so the next route proves production behavior. The two-second
    deadline, mutation ownership, boot handoff and fail-closed restoration remain unchanged. Exact
    43/43 Recovery PlayMode is still required before closure (→ pending CI verification).
12. **WHAT:** adding a new improvement round created duplicate matching recipes because the
    resolver assumed history would be destructively replaced. **FOUND BY:** Round 3 recipe
    integration review. **WHY MISSED:** Round 2 proved currentness but not successive-round
    provenance. **CLASS:** current-state resolver with no version-history law. **SYSTEM CHANGE:**
    resolver selects highest round then highest recipeVersion and rejects only same-version ties;
    older recipes remain auditable history (→ pending CI/generated-scene verification).
13. **WHAT:** Golden Android's independent verifier encoded Round 2 IDs and module counts, so the
    build could not distinguish a correct new round from stale proof expectations. **FOUND BY:**
    Round 3 proof-contract review. **WHY MISSED:** evidence schema was treated as a one-time build
    assertion rather than a versioned framework consumer. **CLASS:** verifier hard-coded to one
    generation of the producer contract. **SYSTEM CHANGE:** compile-report schema/compiler v2 plus
    Golden verification of Round 3 IDs, seven modules, positive counts, nine aspect scores and
    weakest-aspect output (→ pending exact-SHA Golden verification).
14. **WHAT:** `WorldDiscoveryNodeRuntime` allocated a `MaterialPropertyBlock` in a MonoBehaviour
    instance-field initializer, causing Unity to throw during EditMode `AddComponent` and stopping
    ordinary CI before scene audit. **FOUND BY:** authoritative EditMode run `29754045806` on
    `dfeddc15`. **WHY MISSED:** runtime review treated `MaterialPropertyBlock` as ordinary managed
    state, while Unity constructs MonoBehaviours through a restricted serialization path that forbids
    Unity API object creation in constructors and field initializers. **CLASS:** UnityEngine object
    allocation before `Awake`/explicit initialization. **SYSTEM CHANGE:** discovery nodes now lazily
    allocate their property block inside `Awake`/first use; the real Round 3 EditMode integration test
    continues to add the component through Unity and must pass before generated-scene audit can run
    (→ pending exact CI verification).

15. **WHAT:** the space-flight domain was designed cool-path-first — flying, the vista, the launch
    veil, taking hits, landing all thorough — while the connective tissue (how you pilot/aim/
    navigate), the failure/edge states (fuel, disabled, **stranded**), and the "why you're out
    here" LOOP were left thin or undesigned. **FOUND BY:** `SPACE_FLIGHT_GAP_AUDIT.md` (2026-07-21).
    **WHY MISSED:** exciting mechanics pull design attention; verbs/loop/failure/onboarding get
    assumed. Same asymmetry as benchmark #4 (entrances polished, exits assumed) — recurring.
    **CLASS:** a DOMAIN designed cool-path-first; its verbs, loop, failure states, wayfinding,
    narrative delivery, onboarding and accessibility assumed rather than designed. **SYSTEM CHANGE:**
    the **DOMAIN-COMPLETENESS CHECKLIST** — every domain audit must score eight columns
    (VERB · PLACE · LOOP · FAILURE · NAV/WAYFINDING · NARRATIVE · ONBOARDING · ACCESSIBILITY/
    COMFORT) and no domain is "designed" until all eight are addressed. (→ pending: adopt the
    checklist into the audit/benchmark machinery + re-run it on the existing designed domains, not
    just space.)

16. **WHAT:** design docs (prose) and the new machine catalogs (JSON, GPT's offline pass) now BOTH
    hold truth, so they can drift; "we'll reconcile periodically" is a forgettable promise, not a
    fix. **FOUND BY:** rb review of GPT's offline pass (2026-07-23) — Terry: "make sure the periodic
    reconcile is not something that gets forgotten." **WHY MISSED:** the machine-catalog pattern was
    added fast for CI enforcement; the docs got a companion source-of-truth but nobody OWNED keeping
    the two in sync. **CLASS:** dual source-of-truth with no sync owner — a standing promise standing
    in for standing machinery. **SYSTEM CHANGE:** `docs/catalog_doc_reconcile.json` registry +
    `tools/catalog_doc_sync_gate.py` (CI in `fast-preflight.yml`; `test_catalog_doc_sync_gate.py`) —
    BLOCKS on an unregistered/missing/invalid design catalog, WARNS on a missing doc back-link or a
    `lastReconciled` older than 45 days ("reconcile due"); ritual in `docs/CATALOG_DOC_RECONCILE.md`.
    The forgettable part is removed: a pair can't be silently added or left untracked. (→ machinery
    in place + gate green locally; closes when the GPT-owned back-links are added and the gate has run
    green in CI.)

17. **WHAT:** the audience-certification chain for a family title — Meta's "Mixed Ages"
    age-group self-certification (separate from IARC), without which the app is INVISIBLE to
    parent-managed preteen (10–12) accounts, plus its obligations (no ads · COPPA-clean data ·
    Get Age Category API within 30 days of self-cert or SDK cut/store removal) — appeared in NO
    store planning, though the 10–12 family audience is the project's stated core. **FOUND BY:**
    finished-game benchmark, third run (F5C, 2026-07-23). **WHY MISSED:** store planning was
    shaped by Meta's generic submission checklist (what EVERY app needs); nothing asked what
    THIS AUDIENCE needs — the family goal lived in design docs, compliance lived in platform
    docs, and no artifact crossed them. **CLASS:** audience-specific platform requirements
    invisible to generic platform checklists. **SYSTEM CHANGE:** canonical benchmark §5c rows +
    `META_STORE_READINESS.md` must gain a "Mixed Ages / family access" section whose items ride
    the entitlement/Platform-SDK envelope; the recurring benchmark re-score gains an "audience
    access" column so audience-conditional requirements are re-checked per milestone.
    (→ pending: META_STORE_READINESS section + ⚖ Terry confirms Mixed Ages as the target
    designation.)

18. **WHAT:** an unplayable build passed every gate — from the actual launch state a player had
    NO available action (boot hold suspended move/turn in a floorless `_Boot`, zero ACTIVE ray
    interactors, the only choice stranded 2.9-4.4 m away). **FOUND BY:** Terry's M0 headset
    session, 2026-07-25, in the first sixty seconds. **WHY MISSED:** the PlayMode harness
    *repaired the failing state before observing it* — `RecoveryActualRigControllerSimulation`
    disables the production `XRInputModalityManager` (the component that decides ray activation),
    force-activates the ray hierarchies, and fakes the head pose; the same artifact had already
    logged `rays=0(active=0)` + `NO_RAY_INTERACTORS` and treated it as setup, not a blocker.
    Amplifiers: findings are graded individually so a combination that is jointly fatal stayed
    warning-only, and every UI/reach audit inspects AUTHORED scenes while the Home Hub is created
    at runtime. **CLASS:** component-green / goal-dead — every owner satisfied its local contract
    and no gate asked whether a real player could execute the next required verb from the
    unmutated production state; plus the harness-heals-then-certifies pattern that hides it.
    **SYSTEM CHANGE:** `docs/recovery/BOOT_LIVENESS_GATE_PLAN.md` G1-G6 — production-state-first
    liveness assertion before any synthetic activation (G1) · a mechanical harness-honesty gate
    forbidding mutation-before-assertion and separating synthetic evidence classes (G2) ·
    contextual fatal combinations emitting a blocking `BOOT_DEADLOCK` (G3) · a runtime-created
    interactable census measured from the actual tracked pose (G4) · the first-actionable-verb
    delivery gate before any APK reaches Terry (G5) · standing/seated/child reach proxies (G6).
    (→ pending: fix shipped rb110; the entry CLOSES only when G1-G4 exist and are green.)

19. **WHAT:** a false "the city has NO level inside it" claim (rb126: `ToxicCityLayout.asset` has
    "EMPTY districts:/canals:/creatureZones:/droneZones:/shipyard:/experience: blocks") shaped a
    session of planning — the asset actually carries 5 districts, 7 connections, 2 canals, 3 drone
    zones, 4 creature zones, hazards, and a berth; only `pois:[]` was empty. **FOUND BY:** the
    plan-mode exploration's full-file read (2026-07-29), before any code acted on the claim.
    **WHY MISSED:** the state was measured with `grep -E "^  [a-zA-Z]+:"` — on multi-line Unity
    YAML that pattern matches only top-level keys, so a populated list (items on subsequent,
    deeper-indented lines) is indistinguishable from an empty one; the tool's output shape was
    mistaken for the data's shape. **CLASS:** measurement-by-grep on multi-line structured data —
    a line-oriented probe answering a structure-shaped question. **SYSTEM CHANGE:** asset/scene
    content claims ("empty", "missing", "N of X") must come from a full read of the block or a
    structure-aware parse, never a line-regex; any doc claim that cites grep as its evidence is
    treated as UNVERIFIED. Corrected in the docs by rb127 (HANDOFF) — `LEVEL1_SPATIAL_SCRIPT`'s
    affected rows are re-marked as the spec supersedes them. (→ pending: closes when the spatial
    script's §3 [∅] rows are reconciled against the compiled spec.)

20. **WHAT:** I told Terry the space-leg batch was "CI GREEN on `94e363a4`". The **EditMode job**
    was green; the **"Patch scenes + world audit" job was RED on that same commit** and stayed red
    through `c42b1311` — one blocker, `RESOURCE_ID_UNREGISTERED: 'scrap'`. So for three commits the
    project was flying with a broken verification workflow while a session report said otherwise,
    which is precisely the condition CLAUDE.md §WORKFLOW INTEGRITY says must be announced LOUDLY.
    **FOUND BY:** reading the run's per-job conclusions (2026-07-29) instead of the run's headline.
    **WHY MISSED:** I checked the job I cared about (tests) and generalised it to "CI". A CI run is
    a set of jobs with independent conclusions; "the tests passed" and "CI is green" are different
    claims, and the audit job is the one that catches content-law breaks like an unregistered
    resource id — exactly the class of thing that reaches the headset as a broken build.
    **CLASS:** part-for-whole verification — reporting a subsystem's status as the system's status.
    **SYSTEM CHANGE:** a CI-green claim must cite the RUN's conclusion (or every job's), never one
    job's; when a run is red, the reply leads with the workflow-integrity warning before any
    feature report. Fixed in `scrap` registration; the honesty rule is the durable half.

21. **WHAT:** ToxicCity's contract step 4 asks you to repair machine `signal_relay`, and the
    world pack spawned **no machines at all** — so the first level's contract stopped dead at step
    4 of 6. Steps 5 (drive to the flats for half B) and 6 (return to the berth) sat behind a step
    that could not complete, and `toxiccity_complete` — the flag gating W002 — was ungrantable.
    **FOUND BY:** auditing every contract step's marker/machine against a real producer before the
    2026-07-29 headset session (rb131), not by playing it. **WHY MISSED:** generated worlds pair
    the two halves automatically (`WorldJobLibrary.Repair()` is documented "pair with a Machine()
    entry", and the same spec writes `pack.machines`). ToxicCity is the ONE world that hand-writes
    its contract and its pack in two different files, so the pairing was a convention no mechanism
    enforced. **CLASS:** a cross-file invariant that is automatic on the generated path and manual
    on the bespoke path — the bespoke path inherits the assumption without the enforcement.
    **SYSTEM CHANGE:** ① the wiring gate's 22nd feature binds the contract step to the pack machine
    (mutation-tested: removing either half reds CI) — DONE. ② `WorldPackValidator` already contains
    the rule that predicts this exact defect in words ("Repair 'X' but the pack spawns no such
    machine — likely un-completable"). Wire it into `WorldAuditRunner` as a project-wide pack report
    so every world is checked, not just the one someone remembered — WARN first per the
    `PerfBudgetAuditRules` ratchet, promote to blocker after one clean run — **DONE 2026-07-31
    (`WorldPackAuditRules`)**.
    **CORRECTION (2026-07-31):** I wrote above that the validator "is called from NOTHING but its
    own unit tests". That was wrong — `JobDirector.cs:37` calls it at world entry. The gap is
    narrower than I stated and still real: that call is a RUNTIME check, on device, after the player
    has already travelled into the broken world, and only for the world they entered. I read the
    test call sites and generalised; the honest claim was "nothing checks packs at BUILD time".
    (→ pending: closes when a green-run citation is part of the standard session report format.)

22. **WHAT:** The `InputActionState.ApplyProcessors` NRE was diagnosed correctly on 2026-07-20 and
    the fix shipped (`ClearInertDirectProperties` — null the zero-binding embedded action instead of
    disabling it), yet the same crash kept landing intermittently in PlayMode eleven days later. The
    mutation was right; its **trigger** was wrong. It lived inside `InputMutationRepairDriver`'s
    post-travel repair, and that method's `Update()` returns early until it has seen a travel — so
    cold boot, and every PlayMode test that merely loads a scene, still met the enabled placeholder
    and crashed on the provider's first read.
    **FOUND BY:** reading `Update()`'s early-out while chasing the last PlayMode red (2026-07-31),
    not from the stack — the stack looked identical to the one already "fixed", which is why three
    sessions read it as flaky.
    **WHY MISSED:** the bug was found on the recovery route, so the fix was installed on the recovery
    route. The route that exhibits a defect is the route that gets the guard; nobody asked which
    *other* paths reach the same read. VR_RIG_GOTCHAS #9's verification rule even demanded a
    same-SHA rerun — and got it — because both runs travelled.
    **CLASS:** a correct repair installed on one trigger path, leaving the identical defect live on
    every other path to the same call site. The acute fix looks complete because its own reproduction
    goes green.
    **SYSTEM CHANGE:** ① the mutation is extracted to `LocomotionInertActionSweep` (one
    implementation) and driven from install, every `sceneLoaded`, and the post-travel repair, with
    `reason=` on the log so which trigger fired is visible — DONE. ② the standing rule, recorded in
    VR_RIG_GOTCHAS #9: **a repair is not accepted until every path that can reach the guarded call
    has been enumerated and shown to run it** — "the reproduction is green" is not that enumeration.
    (→ **CLOSED 2026-07-31**: three consecutive PlayMode dispatches on byte-identical Unity source
    returned 43/43 — runs `30624847856` / `30625474855` / `30626042812`. The same lane read 41/43,
    40/43 and 42/43 before the fix, with the failure moving between tests each run.)

23. **WHAT:** Twice in one session my own new test failed CI because **the test's premise was wrong,
    not the code** — the berth guard asserted on the wrong axis (`017e0f17`), and
    `LocomotionInertActionSweepTests` asserted "one inert property per provider" when a freshly
    added XRI provider actually carries **two** (Unity's serializer instantiates the embedded
    `[SerializeField] InputAction`, so both hands start as zero-binding actions). The sweep was
    behaving correctly in both cases.
    **FOUND BY:** CI, both times — which is the system working, but it costs a full red cycle each
    time and a red cycle is the thing the circuit breaker counts.
    **WHY MISSED:** both assertions were **counts**, and a count encodes an assumption about the
    default state of a type whose source I cannot read (VR_RIG_GOTCHAS #8: the cloud container has
    the XRI/Input System DLLs but not their source). I asserted on my model of the package instead
    of on the behaviour I actually care about.
    **CLASS:** asserting on a number derived from unreadable third-party default state, rather than
    on the observable property the feature exists to guarantee.
    **SYSTEM CHANGE:** when a package type's defaults are not readable in this container, assert the
    **property** ("this hand's action is null", "the real stick's action survived"), not the count.
    Where a count is genuinely the contract, pin the default in its own named test first so the
    number has a stated source — done here as
    `AnUnconfiguredProvider_HasBothHandsInert_AndBothAreCleared`.
    (→ pending: closes when this rule is in VR_RIG_GOTCHAS #8 alongside the "just push and let CI
    catch the name" advice, which is what nudged me toward guessing in the first place.)

24. **WHAT:** `COMBAT_HEALTH_PLAN.md` sat unbuilt for three weeks with the header "PROPOSAL awaiting
    Terry's two design decisions (§2)" while **§2 of the same document says "DECIDED (Terry,
    2026-07-07)"**. The consequence is the largest gameplay hole in the game: the campaign player
    cannot be hurt, and every creature's authored `damage` is applied to nobody.
    **FOUND BY:** hunting for holes at Terry's request (2026-07-31), by reading §2 after the header
    said not to bother.
    **WHY MISSED:** a decision was recorded in the body of a document whose *status field* was never
    updated to match. Every operator who checked "is this actionable?" read the header, believed it
    was blocked on Terry, and moved on — including several who had capacity to build it.
    **CLASS:** a stale status field outranking the document's own content. The cheapest possible lie,
    in the most-read line of the file, and it silently converts buildable work into blocked work.
    **SYSTEM CHANGE:** ① this plan's header now carries a verified-against-the-tree state table
    rather than a claim — DONE. ② the durable rule: **a doc whose body records a decision may not
    keep a status that contradicts it**, and any operator who reads a "blocked/awaiting" status must
    confirm it against the section it points at before believing it. A status line is a claim about
    the document, and claims get checked like any other.
    (→ pending: closes when Phase B ships, which is the proof the status was the only thing stopping it.)

25. **WHAT:** I wrote a complete second `ArmorMeter` in `Core` — the pure type plus twelve tests —
    before discovering one already existed in `Ziptide.Multiplayer` with `PlayerCombatState` and its
    own tests. Deleted before commit.
    **FOUND BY:** the file-already-exists error when writing the test file, not by searching first.
    **WHY MISSED:** I read the plan's Phase A.1 (*"New pure type `ArmorMeter` … put it beside
    `PvpCombatant` in `Multiplayer`, or in `Core` if `Gameplay` needs it"*) as a spec to implement,
    and the phrase "New pure type" as proof it did not exist. The plan was written before A.1 was
    built and never updated — the same stale-status class as #24, one heading down.
    **CLASS:** trusting a plan's tense as evidence of current state. "New type X" in a design doc says
    nothing about whether X exists today.
    **SYSTEM CHANGE:** before implementing any named type from a plan, grep for the type name first —
    one command, and it is the difference between a refactor and a duplicate combat system. Recorded
    as an explicit ⚠ in COMBAT_HEALTH_PLAN's header so the next reader is warned at the point of use.
    (→ pending: closes when the grep-first step is in OPERATOR_START_HERE's Definition of Done.)

26. **WHAT:** Three code commits today passed CI (`adc108f4`, `d655ac0c`, `911ccd1f`) and only the
    first has a **durable** green record. `docs/CI_VERDICT.md` still names `52fee309` as the last
    GREEN, so anyone reading the committed record — the thing CLAUDE.md's workflow-integrity rule
    points at — under-reports what is actually verified.
    **FOUND BY:** noticing the verdict file never updated past `52fee309` while the API said two
    later runs succeeded (2026-07-31).
    **WHY MISSED:** the verdict job refuses a stale write, and correctly so: it tolerates later
    commits only to generated evidence, `CI_VERDICT.md`, `HANDOFF.md` and `handoff_queue/**`. I
    pushed docs touching `TONIGHT_TEST_CARD.md`, `MISS_LEDGER.md`, `EXCELLENCE_MAP.md` and
    `CURRENT_EXECUTION_CHECKLIST.md` within a minute of each code push, so by the time the run
    finished the head had moved to a path the writer does not tolerate. **The mechanism is right;
    my sequencing was wrong.** Every one of those doc pushes cost the code commit its record.
    **CLASS:** a correct conservative guard defeated by push ordering — the evidence is lost not
    because verification failed but because the author raced it. Nothing is red, so nothing warns.
    **SYSTEM CHANGE:** ① sequence: land docs BEFORE the code commit they describe, or batch them
    into it; never push unrelated docs while a run is in flight on the previous commit. ② when a
    verdict is knowingly lost this way, the HANDOFF entry must cite the RUN ID and conclusion
    directly, so the durable record exists somewhere even when the file is stale — done for today's
    three in rb134.
    (→ **CLOSED same day**: applied on the very next code push. I held the docs for `4bd58b18` until
    its run finished instead of pushing immediately, and `CI_VERDICT.md` now reads **GREEN for
    `4bd58b185b2a`** — the file names the session's own last code commit, which is exactly the
    closure condition.)

## CLOSED

*(entries move here when their SYSTEM CHANGE is verified in place — the fix alone never closes
an entry)*
