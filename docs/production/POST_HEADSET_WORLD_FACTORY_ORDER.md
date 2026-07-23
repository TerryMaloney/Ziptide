# POST-HEADSET WORLD FACTORY ORDER

**Status:** Canonical execution order after the exact M0 headset recovery route passes.

**Purpose:** Move ZIPTIDE from recovery and systems construction into a repeatable world-production loop. The target is not merely to finish one world. The target is to learn from the first world, prove reuse on the second, and turn every repeated correction into data, tooling, an audit, or a reusable runtime seam so later chapter batches accelerate.

**Protected prerequisite:** The exact `c45b1a2` recovery artifact and `docs/testing/HEADSET_RETRY_C45B1A2.md` run first. This document does not authorize changes to that artifact or reinterpret its verdict.

---

## 1. The production thesis

ZIPTIDE already has most of the reusable machinery: travel, jobs, saves, economy, world generation, terrain/biomes, POIs, buildings/interiors, weapons, creature data, story data, Forge, audits, CI, and device evidence rules. The missing proof is one complete content band that uses those systems together and then a second world that proves the result was a factory rather than a one-off.

The canonical sequence is:

1. **M0 recovery proof** — establish a trustworthy Quest baseline.
2. **Model band: W000 → W001** — finish the first hour as the gold-standard assembly line.
3. **Replication world: W002** — build with the same contracts and reject special-case runtime code.
4. **Automation ratchet** — convert repeated manual work into schema/compiler/author/audit/device rules.
5. **Chapter batches** — produce worlds in controlled groups with representative device passes.
6. **Framework extraction** — separate reusable game-building machinery from ZIPTIDE-specific content only after W002 proves the boundary.

Do not jump directly from a working recovery build to authoring dozens of worlds. That would multiply unresolved content and pipeline debt.

---

## 2. Gate A — close M0 and establish the baseline

### Required result

Terry completes the locked recovery route twice with:

- correct rig, hand and controller behavior;
- usable weapon scale, angle, attachment and release;
- reachable and completable coupler flow;
- reliable PUNCH IT / gate travel;
- input restored after travel;
- no blocker crash, strand or save regression;
- lifecycle S1 producing expected `ZIPTIDE: LIFECYCLE` evidence without a new device failure.

### After the pass

1. Record every failure or discomfort note in the existing evidence/MISS_LEDGER route.
2. Classify each note as:
   - baseline blocker;
   - bounded calibration/feel issue;
   - content issue;
   - presentation issue;
   - later certification issue.
3. Fix baseline blockers before world production.
4. Preserve one known-good development APK and source SHA as the new production baseline.
5. Lift the recovery freeze only through an explicit HANDOFF entry naming the safe runtime lanes.

### Stop condition

If rig/input/travel/coupler behavior reveals another shared ownership or lifecycle conflict, remain in stabilization. Do not bury a systemic defect inside W001 content work.

---

## 3. Gate B — lock the world-production packet before building W001

Create one production packet for the W000→W001 band. It should point to existing authorities rather than duplicate them.

The packet must contain:

- world identity and emotional purpose;
- arrival image and navigation landmark;
- first-hour beat order;
- required jobs and interactions;
- RILL lines and trigger conditions;
- collectible/choice/progression flags;
- creature encounter and non-lethal resolution;
- required gear and tutorial moments;
- biome hazards and traversal verbs;
- POI/interior requirements;
- art kit and asset sockets;
- audio event IDs and caption twins;
- performance budgets;
- build-profile inclusion;
- automated acceptance checks;
- device route and subjective questions for Terry.

The production packet is the source of execution truth. Scene edits remain generated through idempotent authors/patchers; no hand-edited Unity YAML.

### WorldSpec requirement

The existing WorldSpec/compiler route must receive real committed inputs during this stage. A compiler with zero production specs is not yet a working factory. W001 is the first authoritative input; W002 later proves it generalizes.

---

## 4. Gate C — finish the model band: W000 → W001

W000 and W001 together are the first-hour product, not two unrelated scenes.

### Pass 1 — graybox truth

Use placeholders and existing procedural assets to prove:

1. Cold start / New or Continue.
2. Player orientation and first useful action.
3. RILL delivery in text/subtitle form.
4. Scan or observation step.
5. Physical grab/holster/tool use.
6. Coupler or hands-on repair sequence.
7. PUNCH IT / gate travel into W001.
8. W001 arrival image and clear route.
9. One complete job loop: arrive → inspect/scan → repair/act → collect → paid.
10. One creature encounter with a readable non-lethal resolution.
11. One physical story fragment or choice interaction.
12. Save, leave, return and resume without regression.

No final art is required to close graybox truth. Geometry, reach, pacing, ownership and progression must stabilize before expensive assets are produced.

### Pass 2 — model-world quality

After graybox truth is stable:

- remove confusing routes and dead space;
- lock interaction heights for children, seated play and standing play;
- establish the W001 kit vocabulary;
- establish arrival, mid-route and payoff landmarks;
- calibrate job duration and first-hour pacing;
- validate 72 Hz and memory behavior;
- produce device screenshots/contact sheets for perceptual review;
- convert every objective lesson into a reusable rule where appropriate.

### Model-band exit gate

W000→W001 is complete enough to teach the factory only when:

- the full first-hour route can be completed without developer intervention;
- all required story/job/progression beats fire exactly once or as designed;
- save/return behavior works;
- no open blocker is disguised as future polish;
- the production packet, specs, generated outputs and evidence agree;
- Terry can describe desired content changes without requiring a foundational runtime rewrite.

---

## 5. The automation ratchet

Every correction discovered while finishing W001 must be classified before implementation.

### Class 1 — one-world authored content

Examples: a unique landmark, a story prop, a specific route adjustment. Keep it in W001 data/recipe/spec.

### Class 2 — reusable schema field

Examples: arrival landmark role, child-reach interaction height, hazard zone, story trigger, audio-event binding. Add the smallest stable field to the authoritative data model.

### Class 3 — compiler/author/factory behavior

Examples: repeated socket placement, collider setup, LOD policy, POI path generation, standard interaction wiring. Implement once in the generator or author.

### Class 4 — audit or automated test

Examples: missing spawn, unreachable objective, absent trigger, wrong profile inclusion, unlicensed external asset, stale generated output. A machine-detectable failure gets a machine check.

### Class 5 — device-only acceptance rule

Examples: weapon feel, child reach, visual clarity, motion comfort, audio balance, landmark readability. Add an exact device row and expected evidence.

### Repetition law

- First occurrence: classify and fix deliberately.
- Second occurrence in another world: default assumption is that the framework is missing a reusable rule.
- Third occurrence: one-off repair is prohibited unless a written exception proves the cases are genuinely unrelated.

This is how world two becomes faster than world one and chapter batches become faster than world two.

---

## 6. Gate D — W002 as the replication test

W002 is not simply “the next level.” It is the proof that W001 did not become a special-case monument.

### Rules

1. Start from the same production packet template and WorldSpec schema.
2. Use a meaningfully different layout/biome/content combination.
3. Reuse canonical job, story, interaction, audio and art sockets.
4. No new generic runtime system is added merely to make W002 work unless the need is proven reusable.
5. Any W001-only hard-coded world ID, scene path or content assumption discovered here must be removed from the generic layer.
6. Track production effort by stage: spec, generation, wiring, machine verification, device verification, and subjective polish.

### W002 exit gate

The process is ready to scale when:

- W002 reaches a complete playable loop substantially faster than W001;
- most corrections are data/recipe changes rather than runtime surgery;
- the same validators and device packet apply;
- no new duplicate owner is introduced;
- the build can regenerate both worlds deterministically;
- a third operator could follow the packet without reconstructing project history.

If W002 is not faster, stop and repair the factory before W003.

---

## 7. Paid 3D asset month — start at the W002 gate, not before pipeline lock

Terry’s proposed timing is correct with one refinement: subscribe after W001 has locked scale, sockets, rigs, collider expectations, LOD budgets, materials and import provenance, but early enough to use the month across both W001 and W002.

### Before subscribing

Prove one complete import lane using a test asset:

concept/reference → generated mesh → source archive → license/provenance record → cleanup → scale/pivot/orientation → rig if needed → animation compatibility → materials → LODs → colliders → Forge/registry trace → Quest budget check → device review.

### Month-one production batch

Prioritize shared, high-reuse assets:

1. one hero player/NPC body pipeline;
2. RILL or another recurring companion representation if applicable;
3. one signature creature using the full body/rig/animation/LOD path;
4. shared hands/gear attachment standards;
5. W001/W002 modular environment kit pieces;
6. high-frequency props and interactable shells;
7. one ship exterior/interior proof asset only after its socket and boarding contracts are stable.

Do not spend the paid month generating dozens of unvalidated meshes. Batch only from approved briefs, and archive prompts, source files, receipts, license terms and generation dates with each asset.

### Commercial-use checkpoint

At purchase time, re-verify the selected service’s current commercial-use, ownership, training-data, redistribution and subscription-expiration terms. The repository licensing manifest and CREDITS workflow must receive evidence at import, not months later.

---

## 8. Paid audio/SFX month — after the audio rails and first-hour event list are stable

Do not start paid sound production while call sites and IDs are still scattered.

### Required rails first

- canonical mixer/bus structure;
- `AudioEvents` or equivalent stable event seam;
- volume/settings persistence;
- ducking behavior;
- 2D/3D and mono/stereo rules;
- loop and duration specifications;
- caption-twin path for progress/safety cues;
- licensing-at-import evidence;
- Quest memory and voice limits.

Reasonbox’s audio production plan and vertical-slice queue are the authority for the first batch.

### First paid batch

Produce the first-hour/high-reuse sounds first:

- boot/home presence;
- RILL identity cues;
- UI/comfort interactions;
- grab, holster and release;
- coupler repair stages;
- PUNCH IT / gate travel;
- W001 ambience and stingers;
- scanner and job feedback;
- weapon disable/salvage vocabulary;
- creature presence and warning cues;
- ship alerts/flight foundations when the ship lane is ready.

SFX may begin before final VO. Do not generate final character VO until the relevant lines and delivery timing are stable. At purchase time, compare currently available providers and verify commercial rights from their live terms before committing the batch.

---

## 9. Art and sound integration cadence

Use three fidelity layers rather than waiting for an all-at-once art pass:

1. **Functional graybox:** correct scale, sockets, interactions, visibility and performance.
2. **Representative vertical-slice assets:** one complete example per asset class proves the pipeline.
3. **Batch replacement:** swap placeholders through registries/recipes without changing gameplay ownership.

Every real asset must preserve the graybox contract. Art may improve silhouette and presentation but must not silently move interaction points, break reach, change required colliders, invalidate save IDs or exceed budgets.

---

## 10. Chapter-batch scaling

After W002 proves replication:

1. Group worlds by chapter and shared kit/mechanic requirements.
2. Add each chapter’s new reusable mechanic before generating all worlds that depend on it.
3. Generate a small batch.
4. Run all deterministic audits.
5. Device-test one high-risk representative and one ordinary representative.
6. Correct the recipe/compiler, then regenerate the batch.
7. Run a periodic full-route soak across all shipped worlds.

Do not device-test every trivial data change independently, but never let an entire chapter accumulate without representative Quest evidence.

Suggested scale sequence:

- W000→W001 model band;
- W002 replication world;
- W003–W005 mini-batch proving three-world throughput;
- complete Chapter 1 band;
- later chapter batches with explicit new-system gates;
- full 80-world generation/audit only after batch throughput is stable.

---

## 11. Framework/product extraction

The reusable framework may become valuable beyond ZIPTIDE, but extraction must follow proof rather than distract from the game.

### Keep generic boundaries now

Generic layer candidates:

- world specification schema and compiler;
- generated-scene authoring/patching system;
- jobs/contracts/economy interfaces;
- save/profile overlays and migrations;
- interaction/socket/reach contracts;
- world travel/build-profile validation;
- content registries and provenance;
- performance budgets and audits;
- first-hour/vertical-slice evidence framework;
- device-test packet and MISS_LEDGER ratchet;
- CI verdict and release-readiness tooling.

ZIPTIDE-specific layer:

- lore, characters and dialogue;
- world IDs and chapter data;
- Ziptide travel fiction/effects;
- specific weapons, creatures, plants and ships;
- art/audio identity;
- story flags and endings.

### Extraction gate

Do not package or market the framework after W001 alone. W002 must prove reuse, and the W003–W005 mini-batch should prove throughput. Then create:

- a clean framework/content dependency map;
- a minimal sample game using different names and content;
- a template repository or Unity package strategy;
- setup, authoring and verification documentation;
- a licensing boundary for Unity, Photon, generated assets and third-party tools.

The product is not “ZIPTIDE with names removed.” The product is the proven production method that built multiple distinct worlds without hidden project history.

---

## 12. Immediate post-headset order

When M0 passes, the next work is:

1. Record and close baseline blocker findings.
2. Explicitly lift the recovery freeze for named lanes.
3. Reconcile Reasonbox lifecycle S1 against device evidence; schedule S2–S5 by dependency.
4. Create the W000→W001 production packet and commit the first real WorldSpec inputs.
5. Finish first-hour graybox truth before real asset volume.
6. Promote recurring W001 fixes through the automation ratchet.
7. Lock the 3D import contract and one representative asset pipeline.
8. Build W002 using the same packet and measure whether production accelerates.
9. Begin one paid 3D generation month for shared W001/W002 assets after the import gate passes.
10. Build audio rails, then purchase one SFX-production month and execute the approved first-hour batch.
11. Prove W003–W005 mini-batch throughput.
12. Begin chapter-scale production and later framework extraction.

---

## 13. Success definition

The project has entered true production mode when Terry can request a new world primarily by describing:

- its identity and feeling;
- required story beat;
- job/interaction loop;
- creature/tool/hazard combination;
- landmarks and traversal;
- art/audio kit;

…and the team can turn that into specs, generated scenes, audits, an APK and a focused headset route without reopening foundational ownership questions.

That is the point where ZIPTIDE begins moving quickly—and where the underlying method starts becoming a reusable game-production framework.