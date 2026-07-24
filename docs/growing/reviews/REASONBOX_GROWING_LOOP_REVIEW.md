# REASONBOX GROWING-LOOP REVIEW — systemic fun, progression, economy, pacing

**Reviewer:** Reasonbox (Fable 5), 2026-07-23, per `GROWING_INVENTION_LOOP_REVIEW_PACKET.md` §2. Independent first opinion; blue + red team; no runtime implementation. Disclosure: I built much of the audited garden stack (genetics, watering, species authoring, hazard-giants) — I red-team my own work below where it deserves it.

---

## 0. VERDICT IN ONE PARAGRAPH

The thesis is right and overdue: "the garden is half of an invention economy" is the best available answer to the question the current build cannot answer — *why do I garden?* — and it couples cleanly to the salvage/machine economy instead of competing with it. More of this plan already exists in tested source than the plan credits (genetics, **adjacency crossing**, wall-clock idle growth, giants, mutation, a recipe-shaped job-step grammar), which lowers the real cost substantially. The dangers are equally real: this is a fifth economy layer proposed while the certified product is two scenes; the illustrative 11-seed plot recipe is a VR chore, not a craft; "both paths required" can curdle into gate-for-gate's-sake; and sprites/multiplayer-rotation are scope gravity wearing a cute hat. My recommendation: **adopt the thesis and the grammar now, at 1/3 the proposed input scale, entering through the already-specced W000 planter beat — and park everything that isn't the loop itself.**

## 1. SOURCE AUDIT (what the plan is actually standing on)

| Claimed-new concept | Repository truth |
|---|---|
| Genetics with speed/yield/size | **Built + tested**: `Core/Runtime/Economy/PlantGenetics.cs` — speed/yield/size/generation, `RollWild/Cross`, 12% mutation kick, `GiantThreshold 0.85`, clamps (SpeedMax 2.2 / YieldMax 2.5). |
| Adjacency/reproduction (§6) | **Already implemented**: `GardenService.CrossPlots(PlotState a, PlotState b, seed, nowUnix)` — deterministic per seed. The plan's same/different-family split is a DATA extension of an existing verb, not a new system. |
| Return-to-ship growth rhythm | **Built**: growth is wall-clock (`UtcNow`), classified BY-DESIGN in the lifecycle contract — plants grow while you run a job. The macro loop's "prepare → return" cadence already has its clock. |
| Species breadth | **Built**: 24 authored species (`GardenAuthor`), hand tools (watering can + pour physics, prune snips), hazard-kick planting, ★ GIANT presentation. |
| Recipe/step grammar | **Adjacent system exists**: `WorldJobLibrary`'s spec-table (`.Collect/.Pickup/.Machine/.Repair/.Reward`) is a tested step grammar; recipes are the same shape pointed at items instead of jobs. |
| Industrial path | **Built**: salvage/disable economy through `RewardRouter`/`LedgerSource` (one-economy audit-gated), `MiningRigRuntime`, `RepairableMachine` stages, 💎 deterministic belts. |
| Loadout surface | **Built, physical**: holsters (`allowedItemIds`) + the belt's fixed sockets ARE a visible, child-readable carry limit. No menu needed. |
| Known gaps the plan correctly names | Seed SURFACING (EXCELLENCE_MAP gardens row: "most seed surfacing… remain" — the species exist but worlds don't hand you seeds); no invention catalog; no recipe states; no narration layer (my audio program §5 provides the caption/narration seams it needs); first-hour contract (22 locked beats) has NO garden beat — my slice packet's optional PR-8 planter is the sanctioned entry. |

Bottom line: the plan's §5–§7 core is ~60% standing code and data. The genuinely new builds are the **recipe grammar + invention catalog + trait language + surfacing**, not cultivation itself.

## 2. RECOMMENDED CORE FANTASY (one paragraph, as required)

You are the tinkerer-botanist of a living workshop-ship. Every world hands you something strange — a spore that glows, a fiber that grips, an ore that hums — and you understand it just well enough to have an idea. You grow it, refine it, and fuse it into a tool nobody gave you; you choose which three ideas ride your belt; and when you step back onto a world that beat you yesterday, it answers to you differently today. The celebration beat is not "number went up" — it's *"I made this, I chose this, and it worked."*

## 3. BLUE TEAM

- **The loop that carries the emotion** is the plan's §4 loop with one edit: the *identify* beat must produce an IDEA, not a stat sheet — the scan says "sticky roots… good for climbing gear," and the catalog immediately shows the silhouette card it could become. Anticipation is the fuel: the player should usually be one-ingredient-short of something they want.
- **Recipe grammar (child-learnable):** adopt base/trait/catalyst but at **1 base + 1 trait + optional 1 catalyst** (see red team). The grammar IS the tutorial: trait words are verbs kids can say (glow, grip, float, filter, zap), and every recipe reads as a sentence — "grippy fiber + metal frame = climbing gloves." Never ship a recipe that can't be spoken as one sentence; that's a testable authoring law.
- **Useful-not-decorative, honestly coupled:** couple the two paths by FUNCTION, not by rule. Industrial makes the *body* (what it is); biological makes the *behavior* (what it does). A static tether is a machined frame that does nothing until conductive fiber gives it its verb. That reads as logic, not as a gate — and it survives a child asking "why do I need both?"
- **Cadence across worlds (recommended):** each world contributes exactly ONE of: a seed family, a catalyst, or a machine tier — never several. At 80 worlds that's a deep catalog with no single world overwhelming anyone; it also drops straight into the world-factory declaration (§11) and the post-M0 production order's per-world packet.
- **Wonder & co-discovery:** giants (built) are already the screenshot moment; mutations (built) are the "DAD LOOK" moment. Family co-discovery wants exactly one thing added: the catalog card's silhouette state — a child recognizing "that's the shadow of the thing we almost can make" is shared anticipation with zero new systems.
- **Sprites:** the emotional job they'd do (the garden feels alive and responsive) is partially served free by existing behavior (plants that visibly grow/react, ecology life outside). As FEATURES they are art+AI+save scope. Blue-team verdict: the *feeling* now, the *creatures* later.

## 4. RED TEAM (the failure modes I'd bet on, and their laws)

1. **Scope gravity vs the slice.** The certified product is `_Boot→W000→ToxicCity`. A fifth economy cannot be allowed to delay the slice. Law: the garden enters the first hour ONLY as the already-specced optional planter beat (slice packet PR-8: plant before first departure, find it grown on return) plus ONE visible catalog card. Everything else lands post-slice.
2. **The 8+2+1 plot recipe is a VR chore.** Eleven physical seed insertions per craft = dropped seeds, tired arms, bored kids. Cut to **1+1+1 (3 physical actions max)**; earn more only with device evidence. The plan says numbers are illustrative — make the CAP the law, not the example.
3. **Artificial mutual necessity.** "High-value equipment should often require both paths" becomes resentment when the player is 1 boring ingredient short for the tenth time. The function-coupling rule (body/behavior) plus the anti-grind laws mostly defuse this; add one more: **no recipe may require more than one SCARCE ingredient** — scarcity budget of one per recipe.
4. **Breeding RNG bleeding into progression.** The fairness law needs a bright line: anything on a REQUIRED path has **zero RNG anywhere in its chain** (the existing 12% mutation kick belongs exclusively to the optional tier, where it already lives); optional rarities get **visible bounded pity** ("guaranteed within 5 crosses," counter shown). Children experience unexplained randomness as unfairness; a visible counter converts it to anticipation.
5. **Loadout backtracking.** Limited loadout is expression only while the anti-lockout law holds; the moment a world demands a tool you left home, it's punishment. Two rules: the belt's physical sockets ARE the loadout UI (no abstract menu — capacity is what fits on your body, visible and narratable for free); and every world offers ONE mid-world swap opportunity (a supply-drop/locker at the spawn POI) so a wrong guess costs a walk, not a travel loop.
6. **Distraction from story/jobs/combat.** The garden must never be the *only* thing that advanced this session. Pacing law: garden actions are BOOKENDS (before departure, after return) — the wall-clock growth already enforces this shape; keep every mandatory garden interaction under ~90 seconds so it stays a rhythm, not a mode.
7. **Multiplayer rotation.** FOMO pressure on a family audience + Photon's licensing hold + live-ops burden. Red-team verdict: PARK entirely; campaign rotation can be simulated later with deterministic weekly seeds if ever wanted, with no scarcity of REQUIRED anything (the plan already says this — I'd simply not build any of it now).
8. **First-hour size.** The §12 introductory band is still too big for the locked 22-beat contract: "one plot, a very small number of families, one recipe, one nurture action, one output" is FIVE new teachings. The PR-8 planter (plant → travel → grown) teaches cultivation's whole promise in TWO actions with zero recipe UI. The first RECIPE belongs in W001/second visit, not the first hour.

## 5. REQUIRED DELIVERABLES (the numbers)

- **Campaign progression curve (W000→mid-game):** W000: plant one seed (PR-8 beat), see one silhouette card. W001: first trait seed found in-world (SURFACING is the real W001 work), first spoken known-recipe (glow cap + salvage frame = lantern), craft at one assembler. W002: adjacency lesson (CrossPlots surfaced), first combined bio+industrial recipe, first real loadout choice (belt full, pick). W003–W005: one family OR catalyst OR machine tier each; first sidegrade; first world-revisit payoff ("the tether opens the W002 grate"). Mid-game: hybridization, extractor+assembler tiers, automation of MASTERED recipes only, planet-signature catalysts.
- **Biological/industrial value split:** 60% machine/salvage (frames, structural progression, major machinery — the battle economy stays primary), 25% garden (traits, consumables, catalysts, adaptations), 15% combined (the memorable capability items). Measured in the ledger by `LedgerSource`, auditable via the existing economy audit.
- **Deterministic-vs-random law:** required chain = zero RNG end-to-end; optional = bounded visible pity (≤5 crosses) + the existing 12% mutation as pure bonus; cosmetics = free RNG. Compatibility of a pairing is always STATED before commit (the plan's own rule — keep it).
- **Initial counts:** 6 seed families (regroup the existing 24 species into families rather than authoring new ones), 8–10 catalog cards visible by W002, 4–5 actually craftable in the first two worlds, exactly 1 catalyst type per world, recipe inputs capped at 1+1+1.
- **Loadout principle:** capacity = the physical belt + holsters, no abstract slots; heavy items occupy two sockets physically (visible, self-explaining); one mid-world swap point per world; ship = the full collection, always.
- **Kid-understanding test ("name it, say it, use it"):** a six-year-old can (a) point at any catalog card and hear its name, (b) after one use, say in their own words what a trait DOES ("sticky = climbing"), (c) complete a KNOWN recipe unaided on the second attempt. Run as a family device session per the DEVICE_TEST_CHECKLIST idiom; failures are MISS_LEDGER entries against the authoring laws, not against the child.
- **Example early loop:** find glow-cap spores in a dark W001 alley → scan speaks "Glow Cap — it makes light" and a lantern silhouette appears in the catalog → plant it on the ship before the next job (2 actions) → it grew while you repaired the coupler → harvest, craft lantern (glow cap + salvage frame, one assembler action) → the next dark alley is yours → RILL comments. Total new UI: one catalog card, one assembler.
- **Example mid-game loop:** W004's signature is the static eel-fern (catalyst, memorable discovery, not a drop) → breed conductivity into a fast-growth base (adjacency, pity counter visible, 2 crosses) → extractor refines conductive fiber → assembler: fiber + alloy frame + eel-fern = **Static Tether** (new verb: pull metal, stun-chain) → belt choice: tether replaces grapple this trip → revisit W002: the sealed machinery room answers to the tether through the grate → a mutation during breeding threw a violet variant: cosmetic, brag-worthy, zero progression weight.

## 6. KEEP NOW / KEEP LATER / CUT OR PARK

| Item | Verdict | Why |
|---|---|---|
| Thesis + pillars §2–§3 | **KEEP NOW** | The missing "why" for two built systems; costs a doc, pays everywhere. |
| Recipe grammar (at 1+1+1) | **KEEP NOW** | The learnable language; existing job-step spec-table is its implementation shape. |
| Invention catalog v1 (ONE ship board, cards + silhouettes + spoken names) | **KEEP NOW** | The anticipation engine; rides my audio program's narration/caption seams (D-ref: audio rails R2/R4). |
| Deterministic-required law + scarcity-budget-of-one + visible pity | **KEEP NOW** | Cheap to legislate now, miserable to retrofit. |
| W000 planter beat (slice PR-8) + W001 seed surfacing | **KEEP NOW** | Surfacing is the real unlock — the 24 species finally reachable; the contract amendment is already drafted. |
| Trait language (verb-nameable traits) | **KEEP NOW** | Authoring law from day one; renaming traits later breaks the child's learned language. |
| Belt-as-loadout + mid-world swap point | **KEEP NOW** (principle only) | Zero new UI; slot-weights and counts wait for device evidence. |
| Machine tiers beyond extractor+assembler | **KEEP LATER** | Two machines cover the whole early curve. |
| Hybridization + higher-tier seeds | **KEEP LATER** | CrossPlots is ready underneath; surface after the basic rhythm is device-proven. |
| Automation of mastered recipes (belt integration) | **KEEP LATER** | Belts are 💎 and waiting; "mastered-only" law is right; sequence after manual loop is loved. |
| Sprites / garden helpers | **KEEP LATER** | Emotional job partially served free today; as features they're art+AI+save scope with no loop dependency. |
| Six-year-old pass formalization | **KEEP LATER** (test NOW informally) | The test definition above suffices until the catalog exists to test. |
| 8+2+1 input counts | **CUT** | Replace with the 1+1+1 cap law. |
| "Both paths required" as a blanket rule | **CUT** (replace with function-coupling) | Body/behavior coupling delivers the intent without the resentment. |
| Garden-support entities as plot OUTPUTS | **CUT from v1** | Output types should all be inventory-shaped in v1; living outputs multiply persistence scope. |
| Multiplayer/rotation layer | **PARK** | FOMO + Photon hold + live-ops burden; deterministic weekly seeds can resurrect it later without scarcity. |

## 7. RECONCILIATION NOTES (for the merge)

- First hour: this review binds to the locked 22-beat contract + slice packet PR-8; the §12 introductory band as written is over-scope for hour one (red-team #8).
- Audio/accessibility: the plan's three-channel identity contract (§17) is the same law as my audio program's caption-twin + spoken-label rules — one narration system serves both; the reusable `AccessibleCatalogItem` question the plan raises should be answered YES, project-wide, by Architect.
- Economy: value-split enforcement belongs in the existing `LedgerSource`/economy audit, not a new tracker.
- Lifecycle: plot commits are the exact "confirmation before spending rare inputs" case — wire commit confirmation into the same interaction family as the §17 rules; wall-clock growth is already classified in the lifecycle contract.
- World factory: the one-contribution-per-world cadence is the garden's row in the per-world production packet (gpt-post-m0-order's ratchet applies: a second world needing the same correction = framework work).

**Handoff:** this file is my complete independent first opinion; no runtime touched; Architect and T-Dog reviews should land independently before merge per the packet's rules.
