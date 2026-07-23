# HANDOFF — session log (single operator ⇄ Terry)

## Required reading

- `docs/FABLE5_START_HERE.md`
- `docs/HANDOFF.md` — current entries
- `docs/HANDOFF_HISTORY_THROUGH_RB24.md` — exact prior history through rb24
- `docs/DEVICE_STABILIZATION_FORENSIC_PLAN.md` — Quest device recovery plan
- `docs/recovery/RECOVERY_PROGRAM.md` — active recovery authority, freeze, proof levels and R0–R4 path
- `docs/recovery/SYSTEM_CONTRACT_INVENTORY.md` — initial critical-system ownership map
- `docs/recovery/AUTOMATIC_RUNTIME_OWNERS.md` — automatic bootstraps, persistent mutators and feature injectors
- `docs/recovery/CANONICAL_OWNER_DECISIONS.md` — proposed surviving owners and displaced paths
- `docs/recovery/R1_INTEGRATION_HARNESS_SPEC.md` — locked next-phase verification design
- `docs/recovery/generated/` — generated source, scene, claim and ownership reports
- `docs/MASTER_CHECKLIST.md`
- `docs/FABLE5_BACKLOG.md`
- `docs/TERRY_RUNBOOK.md`

## Rules

1. Read the newest entry before work.
2. Append Did / Next / Heads-up / Commit at session end.
3. Work on `terry-local-wip`; pull before starting.
4. Keep commits small and stop implementation when required CI is red.

---

## ENTRIES — newest first

### 2026-07-21 (rb88) — Fable 5: 🛰️ SPACE FLIGHT & ENVIRONMENT AUDIT (what's built vs the gap to "feel like space")

- **Terry asked how far along space flight is** (esp. the ACTUAL space environment — local sun(s),
  stars, feeling out there). Audited code + design → `docs/design/SPACE_ENVIRONMENT_AUDIT.md`.
- **Headline:** the **VERB (flying) is a real comfort-tuned v1** (`FlightModel` + runtimes,
  EditMode-tested: snap-yaw, clamped pitch, roll-returns-to-level, bounded lane; `SpaceLane_Trial`
  scene with seat-locked cockpit, ring course, salvage drones). The **PLACE (space) is a
  placeholder backdrop** (near-black sky + one flat planet). SkyVista *has* stars/nebula/SunDisc but
  only as a painted DOME, not scene light or 3D depth. Floating origin, interior physics, distant-
  body parallax, launch transition, space combat = **designed with protected seams, NOT built.**
- **The gap (felt-impact-per-cost order):** ①★ a real **SunRig** (felt sun as scene key light +
  disc + flare + hull terminator; multi-sun) · ②★ **compose the real cosmos** into the flight scene
  (full 360 starfield + nebula + sun) · ③★ the **launch/atmosphere transition** (leave the world) ·
  ④ DistantBodyRig parallax (receding planet/moons) · ⑤ floating origin (open volume) · ⑥ space
  content (asteroids/derelicts/stations) · ⑦ space VFX (speed streaks) · ⑧ space audio · ⑨ walk-
  the-flying-ship (interior physics, the finale). First THREE are cheap and transform the feeling.
- Mostly composition + a few new rigs, not green-field (reuses SkyVista, TravelCoordinator, F3.5,
  BotBrain). Freeze: all post-checkpoint. **Commit:** this one (audit doc + entry).

### 2026-07-21 (rb87) — Fable 5: 📐 MK2 interior blueprint filed + required interior-zone list (bedroom gap flagged)

- **Blueprint landed** (`concepts/ship_mk2_architect/mk2_interior_blueprint_v1.png`) — a 1:50
  "Living Yacht VR Spec": lounge & galley, dorsal greenhouse, cockpit station, hull context,
  invented-glyph KEY (glyph-law honored). Good walkable-layout reference.
- **⚠ Terry flagged a gap:** the blueprint has **no dedicated bedroom / sleeping quarters
  (bunks)** — living space yes, sleeping space no. Locked the **canonical MK2 interior zone list**
  in §7e so it can't be lost: cockpit · crew lounge/galley · **BEDROOM/BUNKS (must add)** ·
  greenhouse · drive-core · fabricator/workshop · airlock. Clarified `mk2_quarters_v1` is the
  LOUNGE, not the bunks. Queued a bedroom concept prompt for when the generator frees up (MK1
  counterpart = `ship_S5_quarters`). **Commit:** this one (blueprint image + §7e + entry).

### 2026-07-21 (rb86) — Fable 5: 🛋️ MK2 upgraded rooms approved (quarters/grow room/watering can) — skin system shown in art

- **3 sleeker MK2 interiors landed** (`concepts/ship_mk2_architect/mk2_quarters_v1`,
  `mk2_growroom_v1`, `mk2_watering_can_v1`) — the upgrade counterparts to the busted MK1 Scrapper
  rooms (which stay as the INITIAL quarters). Doc §7d.
- **`mk2_quarters_v1` is the skin system PROVEN in a picture:** the MK1 home-hub re-shelled in
  sleek grown-alloy + cyan channels, but it KEPT every Cal identity anchor (leather sectional,
  bird trinket, kettle, books, node star-map holo) → still unmistakably Cal's home. These MK1↔MK2
  room pairs are the first concrete skin-swap examples for `SKIN_AND_VARIANT_SYSTEM.md`.
- Excluded: the grow-room "VR 5°" HUD text (generation noise). **⏳ Pending:** a room/ship
  BLUEPRINT (Terry hit the generator usage limit — arrives later). **Commit:** this one (3 images
  + doc §7d + entry).

### 2026-07-21 (rb85) — Fable 5: 🚀 MK2 full view set approved + 🎨 SKIN & VARIANT SYSTEM (how to make 500 Cals)

- **Full MK2 view set landed + stored** (`concepts/ship_mk2_architect/`, 7 views — hero, scaled
  turnaround, reconfigure bloom, cockpit, drive-heart [key S-seam matches the artifact key],
  compare, in-flight). MK2 = the second hero ship, done. Doc §7c.
- **Answered Terry's "500 Cals / reskin guns on a whim" → `SKIN_AND_VARIANT_SYSTEM.md`:** the
  one insight = separate GEOMETRY (build ONCE via Tripo) from SKIN (cheap/infinite). Three layers:
  parametric tint (free, runtime, infinite) · authored skin texture sets (minutes each) · modular
  swap-parts (combinatorial). Lives as `SkinDefinition` (string-ID, applied via ForgeVisualApplier)
  — fits our data-driven architecture. **Author-for-variance rule added to `TRIPO_MONTH_PLAN.md`
  §2b:** build each skinnable asset once with clean UV + SEPARATED material zones + fixed identity
  zones (bird/red-laces/tally; ship's salvage graft) + attach points; DON'T spend Tripo credits on
  variants. Character/weapon/ship/ROOM skins all one system → **"new quarters as a skin" confirmed.**
- **Next:** delivered prompts for the UPGRADED (MK2, sleeker) quarters + grow room + watering can
  (the busted MK1 versions stay as initial quarters). **Commit:** this one (6 MK2 images + 3 docs
  + entry).

### 2026-07-21 (rb84) — Fable 5: 🚀 SHIP MK2 scale revision — a multi-deck LIVING YACHT (concept + doc)

- **Terry scaled the MK2 UP** (`concepts/ship_mk2_architect/mk2_turnaround_scale_v1.png`): a
  substantial **multi-deck ship you can walk through** (⚖ ~25–30 m) — cockpit + crew cabin + a
  **greenhouse garden strip visible through a long dorsal canopy** (adopted signature). Rationale:
  it must house the living systems (quarters/garden/fabricator), and a heavier ship **turns
  slower = VR comfort**. Target updated from tiny hot-rod → **hyper-advanced living yacht** (real
  mass, still sleek/desirable). Doc §7b scale-revision + M1 prompt rewritten to v3 (scaled).
- The v2 look canon (`mk2_hero_v1`, biomechanical grown-alloy) still holds; only scale/interior
  changed. M2–M7 prompts unchanged. **Commit:** this one (turnaround image + doc updates + entry).

### 2026-07-21 (rb83) — Fable 5: 🚀 SHIP MK2 hero APPROVED + design canon locked (concept + doc §7b)

- **The corrected "ship-first" prompt landed** (`concepts/ship_mk2_architect/mk2_hero_v1.png`):
  a desirable, readable ship — sleek iridescent grown-alloy fuselage, teardrop bubble canopy,
  cyan circulatory frozen-current channels, twin gill-intake plasma nacelles, floating wingtip
  vanes as accents (fix held), amidships salvage graft patch, folding skids. Design canon
  recorded in `SHIP_MK2_ARCHITECT_UPGRADE.md` §7b — **signature = biomechanical-but-sleek grown
  Architect tech** (the elegant Giger lesson; through-line to gate/key). All other MK2 views must
  match it.
- **Next:** delivered hyper-detailed, consistent prompts for the remaining MK2 views (ortho,
  reconfigure/bloom, cockpit, drive-heart, compare, in-flight) at xenomorph-level specificity.
  **Commit:** this one (hero image + doc §7b + entry).

### 2026-07-21 (rb82) — Fable 5: 🚀 SHIP MK2 prompt CORRECTION — "ship first, energy second"

- **First MK2 render failed** (stored `concepts/ship_mk2_architect/_rejected/`): the prompt
  over-weighted floating/transparent → an amorphous energy lozenge, not a flyable ship, salvage
  pod detached. **Corrected design law added (`SHIP_MK2_ARCHITECT_UPGRADE.md` §7a):** AIRFRAME
  FIRST (must silhouette-test as a ship — nose/canopy/wings/engines), ~70% solid alloy hull with
  energy as ACCENT (channels inlaid in metal, not the body), an EXPLICIT solid graft hardpoint
  for the arm + salvage plating, integrated nose canopy, warmth in the interior. Target =
  desirable hot-rod/hyper-yacht, not an abstract advanced object. Corrected hyper-specific prompt
  delivered to Terry. **Commit:** this one (rejected ref + doc §7a + entry).

### 2026-07-21 (rb81) — Fable 5: 🚀 SHIP MK2 — Cal's Architect-derived upgrade ship designed (doc + prompts)

- **Terry wants a hyper-high-tech upgrade ship ~L3**, carrying over some Scrapper gear, to
  escape the worn-out steampunk look and to give the skin shop a better canvas. **⚖ ORIGIN:
  Terry chose Architect-derived relic** (asked via AskUserQuestion). Design doc:
  **`SHIP_MK2_ARCHITECT_UPGRADE.md`**.
- **The design threads Terry's two no's** (not steampunk-scavenger, not Warden-blank-nothing)
  via: advanced = density-of-intent + energy-doing-work (layered transparency, reconfigurable
  geometry, energy-as-structure, purposeful micro-detail). Three distinctive hooks: **force-
  field-held FLOATING hull segments** that reconfigure (fold/bloom) · the locked **frozen-current
  channel language** (cyan drive + amber Architect) on a semi-transparent iridescent hull · the
  **evolved drive-heart** (the coupler grown up; the key's true chassis).
- **The carry-over rule (Terry's ask + the beat that saves it):** Cal transplants her home's guts
  — grabber arm, salvage cockpit pod, oil lantern, coupler, bird trinket — as warm human islands
  grafted into the Architect hull. Keeps identity, keeps VR grab-readability, KILLS the empty
  look by construction. Red-laces principle at ship scale.
- **Skin-shop:** the advanced modular energy-hull is the SKIN PLATFORM (vary energy color/alloy/
  faction/channel pattern); Cal's grafted gear stays constant across skins; Scrapper stays a
  fixed nostalgia LOD. **📣 story lane:** where/why she finds an Architect ship that answers to
  her key (§4). 6 concept prompts in §7. **Commit:** this one (doc + entry).

### 2026-07-21 (rb80) — Fable 5: 🤖 ROGUE DRONES + Wake-Guild spider approved (concept + docs)

- **Terry added the rogue-drone enemy set** (`DisableDrones` targets). One sheet approved
  (`concepts/bestiary_ch1/rogue_drone_and_guild_spider_sheet_v1.png`) covering: the feral
  security-drone family (rusted salvage, ducted fans, RED eye, pulser emitter, jury-rigged =
  reads broken), the **disabled/powered-down state** (honors canon "disable, don't kill" →
  `DRONE_DOWN`), and the **Wake-Guild recon spider** (maintained, quad-lens, red targeting
  grid, **anchor+cog Guild emblem**). Specs in `CONCEPT_ART_NEXT_30.md` addendum.
- **★ COLOR LAW LOCKED:** machine-eyes — RILL amber (friend) / Warden white (immune) / rogue+
  Guild drone RED (hostile). Player tech CYAN vs hostile fire RED-ORANGE (friend/foe legibility).
  Wake-Guild emblem = anchor+cog (adopt as faction mark).
- **✅ PULSER SHEET LANDED** (`concepts/bestiary_ch1/pulser_weapon_sheet_v1.png`) — 4-panel:
  red-orange emitter coil · plasma-slug bolt + trail · muzzle-flash · molten wall-scorch.
  Confirms enemy-red vs player-cyan; feeds F3.5 VFX (projectile/muzzle/impact) + drone emitter.
  **The drone set is complete.** **Commit:** the pulser add (image + doc updates).

### 2026-07-21 (rb79) — Fable 5: 🏁 THE 30-CONCEPT WAVE IS COMPLETE — Tier 5 grammar + key art landed (concepts + docs)

- **Terry generated Tier 5 — all 7 approved.** Stored in `concepts/grammar_keyart/`; approvals
  + canon locks in `CONCEPT_ART_NEXT_30.md`. **This closes the entire 30-concept wave** (worlds
  9 · core loop 6 · bestiary 4 · held gear 4 · grammar/key-art 7).
- **Major canon locks this batch:**
  - **`architect_syllabary_v1` = THE MASTER ALPHABET** (30-glyph syllabary + frozen-current
    motifs + geometric borders, amber on stone). Every ancient carving game-wide references it;
    glyph-law now has its canonical source.
  - **The Pattern got a full grammar + behavior:** 3-stage sheets + an 11-panel "Geometric
    Morphogenesis" lifecycle (sky-seed → possesses minerals → breaches → dominion). **📣 story
    lane: this is antagonist behavior, not just VFX.** Palette = signal-cyan pushed cold +
    violet wrongness (it IS the signal's geometry).
  - **`title_key_art_v1`** = the poster / cold-boot title (Scrapper + dock + lantern + giant
    planet + monumental glyph-columns in the #24 alphabet, wordless per glyph-law).
  - **`coupler_ship_heart_v1`** — the coupler's S-seam MATCHES the artifact key interlock (the
    heart accepts the key — canon lock).
  - **`cal_first_person_hands_v1`** — **resolves the Cal body hole (route 1 confirmed):** tally
    forearm + red laces + bird trinket + gloves all read in POV; no full-body pipeline needed
    for the core view.
- **STATUS:** with the earlier first-hour cast, the first four worlds + full core loop +
  bestiary + all held gear + master alphabet + antagonist grammar + title art + Cal POV all have
  canonical reference. `TIER1_PROP_SPECS.md` carries the itemized prop bill. Next art step =
  BUILD (post-Golden-Checkpoint) per `CONCEPT_TO_BUILT_PIPELINE.md` §4.
- **⚖ stack += Pattern-palette (signal-cyan→cold+violet) · Cal route-1 · Architect alphabet as
  canon.** **Commit:** this one (7 images + doc updates + entry).

### 2026-07-21 (rb78) — Fable 5: 🔦 TIER 4 HELD GEAR — 4 tools approved (+ bonus ortho/design sheets)

- **Terry generated Tier 4 — all 4 held tools approved, photoreal, several with bonus sheets.**
  Stored in `concepts/held_gear_ch1/`; approvals in `CONCEPT_ART_NEXT_30.md`, prop specs in
  `TIER1_PROP_SPECS.md` §8. Tools: salvage tractor, mining drill, magnet-tether/anchor,
  headlamp+scanner. The tether's design sheet even storyboarded its mechanics (anchor-to-surface
  for wind/traversal, reel-pull objects, rotating-hook open/close).
- **⚖ EMERGENT: the tractor came back labeled "A-11 ENERGY HARVESTER" with a "POWER INTAKE"** —
  the gen fused tractor + energy-siphon into one tool; recommend adopting (one device pulls
  debris AND draws power — reconciles the salvage verb). Drill = "MO-01."
- **⚖ NAMING/LABEL rules (recommend adopting):** `XX-01` model designations are the salvage-
  culture branding pattern (SLV-01 Scrapper · A-11 Harvester · MO-01 Drill · Goliath Rig); and
  **human-readable stamped labels are OK on the player's salvage gear while glyph-law reserves
  invented glyphs for ancient Architect tech** (modern salvage = readable stamps; ancient =
  glyphs — keeps VR tool text legible without breaking alien immersion).
- **Progress: 23 of 30 concepts approved** (T1–T4 done). Remaining: **Tier 5 only** — the 7
  grammar sheets + key art (Architect glyph sheet, title key art, Cal first-person hands, the
  coupler, Pattern grammar; Transmission + memory-shard already effectively covered by W004 #9).
  **Commit:** this one (6 images + doc updates + entry).

### 2026-07-21 (rb77) — Fable 5: 🐾 TIER 3 BESTIARY — 4 creature sheets, all approved (concepts + docs)

- **Terry generated Tier 3 — 4 multi-angle creature turnaround sheets, all approved.** Stored
  in `concepts/bestiary_ch1/` (canal_stalker, cistern_swarmer, glass_tendril, warden_drone);
  spec lines + rulings in `CONCEPT_ART_NEXT_30.md`.
- **★ KINSHIP LOCKED:** the warden drone is unmistakably the same ceramic-bone + single-eye
  design family as the capital ship AND RILL — it's RILL's **cold black-eyed twin**, the
  contrast the player should feel. Canon consistency across three scales.
- **★ NEW CREATURE-PALETTE RULE (⚖ to bless):** creature **biological bioluminescence** is a
  separate axis from the tech/architect accents — toxic yellow-green (canal), phosphor blue
  (swarm), tuned cyan (tendril, deliberately touching signal-cyan because it's tuned). Glow on
  a beast = "alive"; glow on a machine = "powered/signal"; Warden white = made-thing. No collision.
- **Minor:** canal stalker came back with small dorsal eyes (prompt said eyeless) — approved, so
  canon = low dorsal eyes + vibration-sense head.
- **Progress: 19 of 30 concepts approved** (T1 9 + T2 6 + T3 4). Remaining: Tier 4 held gear (4),
  Tier 5 grammar sheets + key art (7). **Commit:** this one (4 sheets + doc updates + entry).

### 2026-07-21 (rb76) — Fable 5: 🔧 TIER 2 CORE-LOOP CONCEPTS — all 6 landed + prop specs (concepts + docs)

- **Terry generated Tier 2 (the core loop) — 6 keepers, all approved.** Stored in
  `concepts/core_loop/` (home_hub, garden, watering_can, automation_belt, fabricator,
  galaxy_map). Approvals + rulings in `CONCEPT_ART_NEXT_30.md`; **super-specific prop specs
  added to `TIER1_PROP_SPECS.md` §7** (core-loop / ship-home props).
- **Highlights:** the home hub nails the diegetic menu (cyan node star-map on a coffee-table
  holo AND a wall screen — the nav visual language, mirrored at #15's helm scale — plus **Cal's
  bird trinket**, adopted as a recurring keepsake). The garden delivers the giant glowing rare
  crop (screenshot moment) + a cyan-seedling growth tell (adopted). The fabricator makes the
  loop literal: scrap hopper + mineral hopper → cyan assembly field building a GUN. Belt line
  honored the one-cyan-lamp-per-module accent rule.
- **Caveats logged:** #13/#14 came back illustrated/cel-style — treated as SHAPE references;
  build in our realistic used-future material families. Garden "VR" HUD text = excluded noise.
- **Heads-up:** Tier 1 (9) + Tier 2 (6) = 15 of the 30 concepts approved. Remaining: Tier 3
  bestiary (4), Tier 4 held gear (4), Tier 5 grammar sheets + key art (7, of which the
  Transmission + memory-shard are already effectively covered by W004 #9). **Commit:** this one
  (6 images + doc updates + entry).

### 2026-07-21 (rb75) — Fable 5: ✅ TIER 1 COMPLETE (9/9) + the super-specific PROP BILL (concepts + docs)

- **W004 #7 landed** (`concepts/w004_broadcast_tomb/w004_establishing_deadscreens_v1.png`) — the
  dead-screen wall with the alien-sky cluster. **Tier 1 world concepts are 9/9.**
- **Did — Terry's "make props super specific" request → `TIER1_PROP_SPECS.md`:** read all 9
  keepers and itemized every prop with real-meter sizes, material family, wear, interaction verb
  (static/grab/machine/hazard/hero), and build tier (A Forge / B Forge-plus / C hero). Key
  structure: **§1 SHARED SALVAGE FAMILIES specced ONCE** (pipes, wheel-valves, dial-gauge
  clusters, caged oil lantern, cable bundles, catwalk/railing, deck plates, crates, hand tools,
  scaffold, the salvage console) — reuse-first, since ~70% of every interior is this kit; §2–§4
  per-world specifics (spiral stair, ore conveyor, glyph wall, wind-baffle relay, glass bloom,
  broadcast spine, dead-screen module, memory shard…); §5 the four HERO/held props for
  face-distance budget; §6 accent-color discipline locked (amber=Architect, cyan=signal,
  green=OK-only, red=danger-only, warm-amber=worklight — accents kept scarce). Inferences marked
  *(inf)* per Terry's license. This is the pattern for every future world's prop bill.
- **Heads-up:** next up is Tier 2 (the core loop — home hub, garden, automation, fabricator,
  galaxy map). **Commit:** this one (W004 #7 image + prop-bill doc + doc updates + entry).

### 2026-07-21 (rb74) — Fable 5: 🌍 TIER 1 CONCEPTS — 8 of 9 landed, W002/W003/W004 now have faces (concepts + docs)

- **Terry generated Tier 1 in one batch — 8 keepers, all approved on sight.** Stored in
  `concepts/w002_dry_cistern/`, `w003_glass_shelf/`, `w004_broadcast_tomb/`. Approval records
  + per-image rulings in `CONCEPT_ART_NEXT_30.md` (new ✅ section). The next three levels now
  have establishing + hero + mystery/machine art; only **W004 #7 (dead-screens establishing)**
  remains of Tier 1.
- **Highlights:** W002 cistern reads MONUMENTAL (arched cathedral waterworks, not a cave —
  adopted); the Architect reveal glows deep-amber glyph channels; the pump station shows the
  ore conveyor + a cyan control screen. W003 nails the awe beat with a correctly-SUBTLE zenith
  Pattern shimmer, and makes the wind hazard VISIBLE as a sky vortex (adopted). W004's memory-
  shard/transmission keeper (`w004_memory_shard_transmission_v1`) is a triple-server — it also
  satisfies Tier-5 #25 (Transmission) and #29 (memory shard): a ghostly cyan face ringed by
  invented glyphs.
- **★ COLOR-LANGUAGE FINDING (⚖ Terry to bless):** the batch surfaced a clean, consistent
  split — **ancient Architect stone glyphs = DEEP AMBER; the signal/transmission/tide/tuned
  crystal = CYAN** (amber = the makers' works, cyan = the living signal through them). Warden
  white + lantern amber unchanged. Recommend locking. Also: nano banana held Cal's gloved hand
  across the two held-item shots and honored glyph-law (invented glyphs, no real script).
- **⚖ stack += the amber/cyan color split.** **Commit:** this one (8 images + doc updates + entry).

### 2026-07-21 (rb73) — Fable 5: 🎨 THE NEXT 30 CONCEPT ART — the Chapter-1 North Star wave (docs only; Terry generates 2D free)

- **Terry clarified he's not at Tripo yet — he wants the top 30 2D CONCEPTS we still NEED.**
  Wrote **`CONCEPT_ART_NEXT_30.md`**: paste-ready nano-banana prompts, grounded in the actual
  world READMEs (W002/W003/W004) and the core-loop docs (GARDEN_AAA, HOME_HUB,
  AUTOMATION_CONVEYORS), all inheriting the style + color canon + glyph-law.
- **The gap it fills (the honest finding):** the first-hour CAST is 100% concepted, but the
  **next three levels W002 Dry Cistern / W003 Glass Shelf / W004 Broadcast Tomb have ZERO
  art**, and the **entire core loop (garden, automation, build, home hub) was never
  concepted** either. That's the whole "get a few levels looking good as North Star" gap.
- **The 30, tiered by priority:** T1 the three unbuilt worlds (9 = establishing + hero
  landmark + mystery/machine each) · T2 core loop (6: home hub, garden, watering can, belt
  line, fabricator, galaxy map) · T3 bestiary (4 hero creatures: canal stalker, swarmer,
  tendril, Warden drone) · T4 per-world held gear (4: salvage tractor, mining drill,
  tether/anchor, headlamp+scanner) · T5 grammar sheets + key art (7: **Architect glyph-
  language sheet** [fixes glyph-law everywhere], Transmission stages, Pattern grammar, **title
  key art** [S6, still un-delivered], the coupler/ship's heart, memory-shard, **Cal first-
  person hands** [resolves the Cal body hole]). Short-day guidance: T1 first, then T5's 24/27/30.
- **Heads-up:** this is freeze-legal 2D generation; approved sheets flow into
  `CONCEPT_TO_BUILT_PIPELINE.md` and later the Tripo intake. No queue-doc overlap — the
  Tier-C queue stays "cast complete"; this is the next wave (worlds + loop, spans all tiers).
- **Commit:** this one (new doc + this entry).

### 2026-07-21 (rb72) — Fable 5: 💳 THE TRIPO MONTH IS ON — generation-day plan + the 30-prompt priority list (docs only, clean APK untouched)

- **Terry is buying the paid month TODAY** (50–300 generations) and spending the day
  generating. Plan doc: **`TRIPO_MONTH_PLAN.md`** — §0 day-one license checklist
  (screenshot terms+plan+date into `art_intake/_LICENSE_RECORD/`; CREDITS line per keeper —
  the Suno law applied), §1 holding area **`art_intake/` at repo root, OUTSIDE
  `Ziptide/Assets/`** so the clean APK is untouched by construction, §2 the plug-in wiring
  map per asset (applier visual-child swap everywhere; ship arm generated SEPARATELY for
  articulation; RILL needs one emissive-mask re-bake for the state driver; Cal via auto-rig
  for mirror/menu route), §3 credit-efficient workflow (image-to-3D from CROPPED keepers,
  multiview from the ortho sheets, silhouette-first variant judging, text prompts routed
  through free nano banana first where no sheet exists).
- **The 30-prompt list, §4, priority-ordered with per-asset credit budgets (~125–150
  total):** Tier 1 North Star set (Scrapper hull + separate grabber arm, RILL, key halves
  A/B, skiff, Warden capital, Cal MK.IV, gate pillar cluster — ~45 credits, the stop-line if
  the plan is 50-credit) · Tier 2 held weapons (taser, gravity gun — `GravityGunDefinition`
  is canon in code — breaker bar, belt cutter) · Tier 3 Moss landmarks (tower, booth, crane,
  pier, RUSTBUCKET, sea-wall chunk, footbridge, shanty one-off, buoy) · Tier 4 lived-in
  props (seat, console, bunk, lantern, floodlights, cargo set, ledger) · Tier 5
  opportunistic (spider drone as ENEMY class, hex seal). Excluded on purpose: creatures
  (Creature V2 owns), sky (texture/data), the gate tide (shader/VFX), the tiling building
  kit (modularity is where AI gen fails — kit stays Forge).
- **Heads-up:** intake of the generated meshes (cleanup + wiring, §2) is post-Golden-
  Checkpoint work, first in the `CONCEPT_TO_BUILT_PIPELINE.md` §4 order. Today produces
  licensed keepers + manifests only. 📣 all lanes: `art_intake/` is the agreed holding path.
- **Commit:** this one (plan doc + this entry).

### 2026-07-21 (rb71) — Fable 5: 🔵 RILL APPROVED (cast 100% closed) + THE CONCEPT→BUILT CAPABILITY PLAN (docs + 2 reference images)

- **Did (1) — RILL keepers landed** (`concepts/rill_drone/`, 2 variants; queue §2f): the orb
  in the ARTIFACT material family (engraved frozen-current channels — visibly the key's
  cousin), ONE warm iris-detailed lens-eye (the deliberate inversion of the Warden capital's
  cold eye), **the riveted repair panel on the back** (her red-laces equivalent, delivered),
  and a four-tile mood row rendering the canon `RillState` palette exactly (dormant blue-grey
  / bright cyan / remembering green-teal / late amber-gold) as eye+channel emissive states —
  a direct reference for the future state-driven material. **Excluded per Terry: the floating
  glove** ("these except for the random hand") — scale-reference noise, K4-script treatment.
  With RILL, the Tier-C phase is CLOSED: every named thing in the first hour has a face.
- **Did (2) — answered Terry's "can the system actually BUILD from these concepts?" with
  `CONCEPT_TO_BUILT_PIPELINE.md`:** the one five-step route (measured spec → recipe plan →
  booth loop WITH the concept as acceptance target → gates → device verdict; steps 1–2
  proven by the ship/city specs) + honest per-class ratings: 🟢 RILL/key/buildings/skyscape
  (machinery mostly exists — `RillState`, `ForgeBuildingKit`, `CityLayoutDefinition`,
  SkyVista all in code) · 🟡 gate/ship/Warden-near (planned, unbuilt: Tier-B ops, B+ bridge —
  `HeroShipHullBuilder` already started) · 🔴 **Cal full-body = the one capability hole** —
  no humanoid pipeline; ⚖ three routes, recommend first-person-only + implied body (VR-native,
  zero new machinery), AI-gen/commission single hero body only if the DC demands it.
  Complete gap list §3 (smallest: booth reference-plate mode — concept beside turnaround in
  one comparison PNG) + post-checkpoint sequence §4 (key first: the pipeline's tutorial asset).
- **Heads-up:** all build steps queue behind the Golden Checkpoint as ever; the ● paper
  starts (RILL/key/gate-state visual specs, ⚖ Cal route) are freeze-legal any time.
- **Commit:** this one (2 images + queue §2f + the new plan doc + this entry).

### 2026-07-20 (hwr35) - Fable 5 architect: ✅ INPUT BLOCKER CLOSED — ladder fully green on `c45b1a29`, headset retry AUTHORIZED
- **The hwr34 fix held deterministically:** PlayMode **43/43 TWICE on the same SHA** (run
  `29786603998`, attempts 1 and 2 — the double-run the 25a8136 flake made mandatory), ordinary
  CI GREEN, Golden Android **SUCCESS** (run `29786604008`), clean package proof recorded. Zero
  repair failures/aborts, zero Input System exceptions. GPT's
  `FABLE5_INPUT_RECOVERY_HANDOFF_20260720.md` status flipped to RESOLVED (history preserved).
- **RATCHET: restore-path re-enable of inert direct actions → caught by the existing PlayMode
  route gate** (it did its job twice); the durable fix removes the class (null property beats
  disabled action). Reflection-seam cleanup deferred per the handoff's own stop conditions.
- **Terry's next device pass runs on artifact `recovery-golden-apk-c45b1a2…`** (run
  `29786604008`) — weapon scale/angle, coupler PRESS POWER reach, travel/input stability route
  from GPT's corrections list. Commits after `c45b1a29` are art/docs only; source-identical.
- **Commits:** this push (docs only — close-out).

### 2026-07-20 (rb70) — Fable 5: 🧑‍🚀 CAL EXISTS — hero design + a full suit progression line; THE VISUAL CAST IS COMPLETE (docs + reference images)

- **Did:** Terry's Cal batch approved (`concepts/cal_goliath_suit/`, 6 sheets + a second
  quarters angle): **MK.IV is Cal** — friendly rounded helmet, mirror visor (no face ever —
  ambiguity canon SOLVED visually, and the visor-as-mirror doubles as the reflection-
  wrongness device), mechanic-not-marine padded suit, chest rig matching the avatar rig, and
  the personal layer: bird trinket · forearm TALLY MARKS · **mismatched red laces** · sewn
  patch. The generation also produced the whole "GOLIATH SALVAGE RIG" progression (MK.IV →
  IV.5 → MK.V → APEX) and KEPT THE RED LACES on every tier. Rulings in queue §2e: red laces
  = Cal's canon constant · tallies adopted (physical twin of RILL's log; what they count = ⚖
  story) · suit tiers = the DC-5 visual-progression track (⚖ mapping; APEX's green neural
  visor reads as the Pattern arriving) · **spider "RILL" drone REJECTED as RILL** (kid-safe
  orb/cube identity stands) and parked as a hostile-drone design · faceted helmet parked as
  late cosmetic · "Goliath Salvage Rig" ⚖ as the line's name.
- **STATUS — THE ENTIRE VISUAL CAST NOW EXISTS, approved and committed:** the Scrapper
  (ext/ortho/cockpit/quarters×2/context) · tide skiff · the Moss (7 layers, look+logic) ·
  tower · dockmaster · skyscape (dusk/panorama/night) · the gate (6-state lifecycle + physics
  canon) · artifact key + seal class · Warden capital · **and Cal**. Color language holds
  everywhere (tide cyan / lantern amber / seal deep-amber / Warden sterile white). The
  Tier-C queue is closed; the paid-3D month has a full backlog; every future builder matches
  references instead of guessing.
- **⚖ stack for Terry (art):** photon-fluid + entrainment canons · gate arena/wild fork ·
  "the Moss" · numbered doors · seal class · suit-tier mapping · tally meaning · "Goliath
  Salvage Rig" · (business queue unchanged: name search, price, severity bar, cut list).
- **Commit:** this one (docs + 7 reference images).

### 2026-07-20 (hwr34) - Fable 5 architect: 🔧 TOOK OVER the input blocker (Terry: "GPT is taking too long") — inert-property normalization landed
- **Root cause, confirmed from the f91c098 run log (sequence REPAIR_OK → NRE → OWNERSHIP_OK):**
  GPT's diagnosis was right and its fix was one step short. Disabling the empty embedded
  left-turn actions cannot stick: restoring a suspended reader (`reader.enabled = true`) runs
  XRI's `OnEnable → EnableAllDirectActions`, which re-enables the zero-binding action, and the
  next `ReadInput` NREs in `InputActionState.ApplyProcessors` exactly once. "Disabled" loses to
  XRI's enable-happy lifecycle every time, including the boot-hold restore path.
- **The fix (matches the recommended edit in GPT's
  `docs/recovery/FABLE5_INPUT_RECOVERY_HANDOFF_20260720.md` §Recommended next edit):**
  `InputMutationRepairDriver.ClearInertDirectProperties` — at the start of the repair pass,
  any embedded direct action with zero bindings and no reference is replaced with a `default`
  `InputActionProperty` on its provider (snap turn, continuous turn, move; XRI's property
  setter disables the outgoing action during play). A null action is the durable spelling of
  "this hand does not turn": ReadInput null-skips it, OnEnable has nothing to re-enable.
  New tag: `ZIPTIDE: INPUT_MUTATION_INERT_CLEARED count=…`. Scope deliberately minimal per the
  handoff's stop conditions — no PlayerRigPersistence refactor, reflection seam untouched.
- **Verification protocol (GPT's §Verification, unchanged):** CI green on this SHA → PlayMode
  43/43 → SAME-SHA rerun 43/43 (the 25a8136 flake precedent makes the double-run mandatory) →
  Golden Android → only then headset authorization for Terry's weapon/coupler retry route.
  I am driving this ladder and will trigger the rerun myself.
- **Commits:** this push (driver fix + this entry).

### 2026-07-20 (rb69) — Fable 5 → **📣 FOR GPT: the input-settle cold-start diagnosis + the fix direction** (Terry asked me to assist — this is the machinery I originally built)

- **Your finding is correct and the data is decisive:** simulator clean (two controllers
  reused, zero owned, no refresh) yet ONE action internally stale after the FIRST cold
  travel, later tests healthy because the next boot's enable cycle repaired the state.
  **The flaw is in my original guard design: the settle window can only WAIT, and
  `InputActionState` staleness does not heal by waiting.** The 45 s poll assumes staleness
  is transient (deferred re-resolution lands next frame). Cold start violates that: the
  first-ever resolve of the fresh asset leaves one action's per-action state out of sync,
  and NOTHING in production ever triggers the re-resolution that fixes it — the "accidental
  repair" you observed is the next wiring pass's disable/enable forcing a fresh
  `ResolveBindings`. Time is not a repair mechanism; the cure exists in the engine and
  production never invokes it.
- **THE FIX — "repair, then verify" instead of "wait and hope," and the safety argument:**
  the settle window is the ONE place active repair is safe, because the hazard the original
  NRE saga taught us (toggling live maps while readers poll) is already fenced — **the
  locomotion readers are suspended inside this window by construction.** Two variants,
  prefer A:
  **(A) Deterministic:** at the end of the wiring mutation, while readers are still
  suspended, force ONE re-resolution of the locomotion action maps (`map.Disable();
  map.Enable()` per affected map — or the asset once, if map granularity fights you). The
  probe then becomes verification and should pass in ~2 frames every time, cold start
  included. **(B) Reactive fallback:** keep the window as-is, but after ~1 s of failed
  probes, repair the FAILING action's owning map once (the v3 logging already names it),
  log `ZIPTIDE: INPUT_MUTATION_REPAIR owner=… action=… attempt=N`, cap at 2 attempts, stay
  fail-closed after. Scope check: the locomotion maps are read ONLY by the suspended
  providers; interaction maps untouched; InputActionManager's asset-enabled contract is
  restored by the re-enable. Precedent: `ActionReadsSafely` already nudges
  (`direct_action_enabled` phase) — the probe is not read-only today.
- **Ledger note (the honest closure of #11's arc):** a test-only repairer was removed to
  "prove production behavior" — and production then proved the repair BELONGED in
  production. When this lands, amend ledger #11: CLASS stands, SYSTEM CHANGE = the guard
  is now repair-then-verify with the repair logged and bounded; the settle deadline can
  drop back from 45 s to ~5 s once repair is deterministic (a 45 s wait for a state that
  never heals was always a mis-sized bandage — my design debt, gladly retired).
- **Evidence to confirm before/after:** the v3 failure detail names the exact action —
  expect `INPUT_MUTATION_REPAIR` to name the same one, then 43/43 with the route's
  zero-settle-failure assertion green on the SAME SHA.
- **Commit:** this one (docs only — the code is your lane/branch).

### 2026-07-20 (rb68) — Fable 5: 🗝⚪ artifact key + Warden capital approved — THE TIER-C CONCEPT SET IS COMPLETE (docs + reference images)

- **Did:** ① **C5 THE ARTIFACT KEY ✅** (`concepts/artifact_key/`): the two-half cyan tablet
  is canon-exact — stepped S-curve interlock that teaches the join gesture, frozen-current
  channels, three states; the sheet's "mid-activation (within two inches)" label accidentally
  SPECCED the proximity-pull mechanic at ~2 in (the DC's "they pull at each other," now with
  a number). The gorgeous-but-wrong 3-piece AMBER hex variant is adopted as a SECOND artifact
  class (⚖): the Architect SEAL family for later chapters (amber = a different system's voice;
  glyph-law caution on its engraved script). ② **C3 THE WARDEN CAPITAL ✅ first try**
  (`concepts/warden_capital/`): pale segmented ceramic-bone ovoid, single black-glass eye,
  sterile-white seam lights, NO rivets/windows/engines/weapons — the contrast law delivered
  ("a white blood cell the size of a city"); the Scrapper appears tiny below it (design
  consistency held a 4th time). Optional future keepers: quarantine-cone state, W037 eye-vs-
  drone recognition beat. ③ Also this arc (rb67→rb68): the gate ENTRAINMENT law + its
  first-try hero keeper (the ground borrowed into the tide — lifecycle set complete).
- **STATUS: the Tier-C concept queue is COMPLETE except ship S4/S5 interiors.** The entire
  first-hour visual cast now has approved, stored, spec'd references: ship · skiff · the Moss
  (7 layers) · tower · dockmaster · skyscape (dusk/panorama/night) · gate (6-state lifecycle)
  · artifact key (+seal class) · Warden capital. **The §3 paid-3D trigger is effectively MET**
  whenever Terry chooses to spend the month. Color language locked across the cast: tide
  CYAN · lantern AMBER (warm) · Architect-seal AMBER (deep) · Warden STERILE WHITE.
- **⚖ standing for Terry:** photon-fluid canon · entrainment law · arena/wild gate fork ·
  "the Moss" name · numbered doors · Architect-seal class · (plus the old queue: name search,
  price/no-MTX, severity bar, cut list, frame ledger).
- **Commit:** this one (docs + 1 reference image; C5's landed in `4d3e268`).

### 2026-07-20 (rb67) — Fable 5: 🌊 THE ZIPTIDE ITSELF — photon-fluid design canon + the gate's full lifecycle in keepers (docs + reference images)

- **Did:** ① **`docs/design/ZIPTIDE_GATE_VISUAL_DESIGN.md`** — Terry asked for a plausible-
  science upgrade of the janky-but-working gate ("think photons," loves Stargate's water):
  the canon (⚖) is built on the REAL field of quantum fluids of light / slow-light media —
  the Shell membrane is a slow-light boundary; between the pillars light CONDENSES into a
  photon fluid (a liquid of light) whose cycling membrane tension gives it TIDES; crossing =
  the crest picks you up, you arrive on the ebb ("the tide remembers where we were going" —
  now physics). Pillars = resonators at membrane wavelength ("do not count the pillars" =
  shifting interference nodes). **THE UNIFIER:** trace photon-fluid dissolved in seawater =
  the night-glowing horizon + tide pools + what the skiff's fins grip + why gates stand in
  water — one mechanism, five visuals. Visual language: NOT a Stargate puddle — the INHALE
  (trough) → the STANDING CREST (teal→cyan→white light-foam, slow-motion ripples, spray
  dissolving to glow) → sweep-through → ebb. Feasibility rails honest (sculpted layered
  planes + existing water normals + emissive gradient + capped particles; flash shell kept
  as the inside-the-wave beat). ② **Five keepers landed same-day** (`concepts/ziptide_gate/`)
  = the gate's LIFECYCLE: standing-crest HERO (the crossing identity, design-doc-exact) ·
  resting state (**adopted motif: tide-stained pillars** — glowing residue on the stone) ·
  the SPIRAL (tide at full reach with THE SCRAPPER riding it — no-swirl law stands for
  person crossings; adopted as the rare SHIP-crossing exterior/trailer beat) · two "gate
  arena" takes (double-ring colonnade + machined floor; column surge + thin-beam inhale
  w/ floating debris) — **⚖ FORK: adopt arena form as the civic/hub gate class vs. wild
  pillar-ring gates?**
- **Next:** Terry blesses: photon-fluid canon · the arena/wild two-tier gate fork · (standing
  ⚖ queue unchanged). Effect upgrade = post-checkpoint envelope against these references.
- **Commit:** this one (docs + 5 reference images).

### 2026-07-20 (rb66) — Fable 5: 🗺 K7 — the city's LOGIC LAYER (schematic blueprint with gameplay semantics)

- **Did:** Terry delivered an isometric SCHEMATIC blueprint of the city ("THE 'MOSS'
  HABITATION ZONE") — K1 is the look, K7 is the logic: sector lettering A–G (G = the stone
  circle with an "anomalous energy reading" caution — the blueprint's own fiction pointing
  at the gate), NAMED WRECKS adopted as the wreck-thread pattern ("RUSTBUCKET" bulk carrier,
  "SEAWEED" trawler — each gets its log + salvage node), a KEY that maps 1:1 to our systems
  (light relay→practicals · water filter/power hub→RepairableMachine job sites · unstable
  structure→hazard volumes · loot point→salvage nodes — adopted as the city POI vocabulary),
  player-route/safe-zone/spillage legend (→ cairn route, spawn areas, acid-hazard volumes).
  Stored as `concepts/toxic_city_kit/city_K7_schematic_blueprint_v1.png`; spec §1a added.
  ⚖ NEW for Terry: adopt **"the Moss"** as the city's proper name (from the sheet's title)?
- **Note:** the city kit is now look (K1–K6) + logic (K7) — CityLayoutDefinition can be
  authored from K1+K7 together with almost no invention left.
- **Commit:** this one (docs + 1 reference image).

### 2026-07-20 (rb65) — Fable 5: 🏙 THE CITY HAS A FACE — all six K-levels approved in one batch + city visual spec (docs + reference images only)

- **Did:** Terry delivered the ENTIRE city concept kit same-day: K1 aerial master (concentric
  canal city on tidal flats — tower island center, ring+radial canals, breached sea wall,
  harbor breakwater S, wreck/stilt outskirts, THE GATE PILLARS on the NE horizon) · K2
  skyline approach (the W000→W001 arrival frame — swap the plane for our ship) · K3 street
  canyon (the Stage A mood target: lantern rhythm ≈ our practicals spacing, 3-warmth windows,
  fog-swallowed distance) · K4 six-facade kit sheet (concrete frame + corrugated infill
  base/middle/cap grammar; **NUMBERED DOORS adopted as city wayfinding vocab** ⚖; noise
  flagged: real-world script → alien glyphs, ignore person/cat) · K5 outskirts ring (the
  FLYABLE surround; **glowing green tide pools = new adopted dressing motif**; sunset variant
  = second lighting act evidence) · K6 berth district — **with the SCRAPPER moored and
  consistent** (nano banana held the ship design across generations!), dockmaster shack
  included → K6 = title-menu berth + first-hour stage + C6 in ONE image. Bonus 8th image:
  ship-context 4-panel incl. **CAB POV** (S4 framing reference; engine quincunx lit = thrust
  glow reference). All stored (`concepts/toxic_city_kit/`, ship folder for the 4-panel);
  **`CITY_VISUAL_SPEC.md`** written: layout-data translation (radial-ring graph for
  CityLayoutDefinition), skyline/palette bands, street grammar, kit measurements, berth
  staging, noise flags. Queue updated: C8 ✅, C4+C6 satisfied by the batch; remaining: C2
  gate · C3 Warden capital · C5 artifact key · S4/S5 interiors.
- **📣 GPT (city Stage A):** K3/K4 + spec §3/§4 are your visual reference when the sprint
  fires — the Stage A verdict test is written in §3 ("the built street should read like K3
  squinted").
- **Next:** Terry: remaining queue items whenever; ⚖ numbered-doors adoption + district
  count (5 vs 6) at layout time.
- **Commit:** this one (docs + 7 reference images).

### 2026-07-20 (rb64) — Fable 5: ⛵ tide skiff approved + 🏙 THE CITY CONCEPT KIT plan (docs + reference images only)

- **Did:** ① Terry's skiff keepers stored (`concepts/skiff_tide_mk1/`) — the no-propeller
  tide-carried design LANDED (cyan fin-keels + lit standing bow wave; workboat hull in the
  ship's salvage grammar; the sheet labels "luminous hover panels", stern crane, lantern
  pole). Approved as v1 canon ("could be better but that will do" — refinable); full spec
  deferred to build time. Bonus: the canal hero shot doubles as the first street-level
  architecture reference. ② **The city-as-concept plan** (queue §2b, C8): the TOXIC CITY
  CONCEPT KIT — six zoom levels, each feeding a different system: K1 AERIAL MASTER (the "city
  blueprint" → CityLayoutDefinition canal graph/districts + flyover route) → K2 SKYLINE
  (→ P3 vista impostors + the arrival flight) → K3 STREET CANYON (→ CityBuilder Stage A
  reference — 📣 GPT's hwr30 item 2 gets its visual reference when keepers land) → K4
  BUILDING KIT SHEETS (6-facade elevations → ForgeBuildingKit module recipes directly) → K5
  OUTSKIRTS RING (tidal flats/wrecks/stilt piers → the FLYABLE surround, P3-band cheap) →
  K6 BERTH DISTRICT (quays + moored Scrapper → first-hour stage + title menu). Assembly:
  approved K-set → CITY_VISUAL_SPEC (ship-spec pattern) → layout data + kit recipes +
  skyline vista. Flight = skyline LOD + impostor ring + altitude ceiling; concepts design
  the CHEAP version.
- **Next:** Terry generates K1 first (it constrains everything), then K4, then mood views.
  Prompts delivered in-chat, K-order.
- **Commit:** this one (docs + skiff reference images).

### 2026-07-20 (rb63) — Fable 5: 🚀 THE SHIP HAS A FACE — Terry's concepts approved, visual spec + B+ recipe plan derived (docs + reference images only)

- **Did:** Terry generated and delivered the ship keepers SAME-DAY: a dock beauty shot + a
  full 4-view orthographic sheet ("SLV-01 SCRAPPER") — truck cab high-forward, ONE dominant
  engine drum with a **quincunx nozzle cluster** (1 big + 4 small — the signature view),
  left-flank 3-segment salvage claw, chipped hazard amber, warm floodlights, the single cyan
  power port (the coupler), 4 splayed hydraulic legs. Passes every rubric law incl.
  fill-it-black and kid-drawable; matches locked canon beat for beat. Stored durably at
  **`concepts/ship_scavenger_mk1/`** (session uploads are ephemeral — repo is the record).
  Wrote **`docs/project_art_plan/SHIP_SCAVENGER_VISUAL_SPEC.md`**: rubric verdict · measured
  proportions as LENGTH-fractions off the ortho sheet (nose 0–0.18, cab 0.10–0.30 atop the
  roofline, mid hull to 0.65, engine drum 0.62–1.00 and tallest; legs wider than hull —
  ignore the sheet's printed dimensions, they're generation noise; ⚖ canon scale ~18 m
  proposed) · palette/material story mapped to EXISTING machinery (plate wash + tintJitter,
  rust streaks, new `HazardStripe` style entry, GlowPanel floodlights, LIVE emissive coupler
  submesh) · **the full B+ recipe op decomposition** (7 groups, ~14.4k tris, inside the 15k
  hull cap — the claw reuses the creature Limb op!) · 5-pass booth sequencing (silhouette
  overlay vs. ortho FIRST, engine quincunx second) · ForgeModuleLook integration under the
  ShipHullBuilder root · this model = the permanent distant LOD later.
- **Next:** post-checkpoint, any art-lane session can build `ship_scavenger_mk1` from the
  spec alone. Terry: S4 cockpit + S5 quarters concepts when convenient · ⚖ scale + "SLV-01
  Scrapper" designation · paste the winning prompt into concepts/PROMPTS_LOG.md.
- **Heads-up:** the two PNGs (~3.6 MB total) are in-repo deliberately — canonical references
  outrank prose and must survive sessions. Concepts folder is reference-only (never ships).
- **Commit:** this one (docs + reference images only; zero code).

### 2026-07-20 (rb62) — Fable 5: TIER-C CONCEPT QUEUE — the ship brief + the hero lineup, all free, all now (zero code)

- **Did (Terry: "hit it up… not paying for Tripo until plenty is lined up"):** wrote
  **`docs/project_art_plan/TIER_C_CONCEPT_QUEUE.md`** — the concept phase that costs $0:
  concept images are REFERENCE (never ship), so the whole phase runs on free image
  generators; paid 3D happens per-asset only AFTER its sheet is approved. **The lead brief:
  THE SHIP** — prompts S1–S6 encoding the LOCKED canon (Toxic City Scavenger: asymmetric
  salvage rig, ONE oversized patched engine as dominant feature, grabber arm as hero part,
  chipped hazard amber + one crest-cyan coupler glow, cockpit CAB with floor windows +
  chunky levers, quarters bunk wall w/ name tags [family profiles!], plus the dock beauty
  shot that doubles as title-menu reference). **S1 silhouette sheet runs first** — decisions
  at the fill-it-black level per the ship doc's six laws, which are also the §0 selection
  rubric (+ "a kid could draw it from memory"). **The queue (C2–C6):** the Ziptide gate ·
  the Warden capital (smooth-vs-salvage CONTRAST is the design) · W001's leaning tenement
  tower (WC-4 hero element) · the joined artifact key (face-distance macguffin) · the
  Dockmaster's post. Workflow = the music-keeper discipline (≥6 candidates, prompts logged,
  approval ceremony per asset → eligible for WC-5 intake). **§3 the paid trigger, concrete:**
  buy the 3D month when ship S1–S4 + ≥2 queue assets are approved — one month converts the
  whole approved backlog, Suno-Pro batching logic.
- **Next:** Terry generates at his pace, S1 first. Every approved sheet also gives the booth
  a reference to chase for the Forge-native placeholders in the meantime.
- **Commit:** this one (docs only: queue doc + this entry).

### 2026-07-20 (rb61) — Fable 5: HERO ASSET STRATEGY — the Forge's honest ceiling + the three-tier path to AAA (Terry-directed research, zero code)

- **Terry's verdict, confirmed honest:** "the ship… the space battle stuff… I can't see that
  coming out very good as is; guns/melee okay but not great." Wrote
  **`docs/project_art_plan/HERO_ASSET_STRATEGY.md`** — and RECORDED that ART_REGISTRY §5's
  hero-asset deferral trigger has now FIRED (the owner's verdict IS the trigger).
- **The ceiling, stated plainly (§1):** primitive-op composition can't economically reach
  designed silhouettes, detail density (panels/greebles/bolts), or edge quality (bevels vs.
  razor edges) — the warden's 5-version "bin with a lid" saga is our own evidence of
  silhouette design done expensively in 3D. VR face-distance inspection makes hero scale the
  harshest test. **The architecture always knew:** stable ids + ForgeVisualApplier
  (root-stable/visual-child-replaceable) + §5 exist so hero meshes can arrive from outside
  without gameplay noticing.
- **The three tiers (§2), every asset declares one:** **A FORGE-NATIVE** (props/flora/modules/
  practicals/most creatures — already on the AAA path via CP-2/CP-3, change nothing);
  **B FORGE-PLUS** (weapons/hero creatures — closable INSIDE the Forge: new Bevel/Chamfer +
  PanelInset + GreebleScatter ops, face-distance texel law, 2–3-material-split Validate()
  check, grip-derived touch-point wear; an envelope in the art window); **C HERO** (THE ship
  ext+int, close capitals, signature landmarks — external path: **2D CONCEPT FIRST law**
  (silhouette approval in images where iteration is cheap), then AI 3D gen (Tripo/Meshy-class:
  2026 state = PBR output, quad options, Unity export, commercial license on PAID tiers only —
  the Suno lesson applies verbatim) OR marketplace kitbash OR ⚖ a commissioned hard-surface
  artist for the ship (plausibly the project's highest-leverage art dollar), all through
  WC-5's intake gates: hero-class budget review, re-material into OUR families, provenance +
  CREDITS, booth + Terry verdict). Space battles: distant fleet = P3 silhouette/VFX
  (Forge-native), only closed-with ships need Tier C. Long-term recorded: headless Blender as
  a scripted Forge backend (Slipway era).
- **New small laws (§3):** tier field on recipes · no Tier-C geometry before an approved
  concept sheet · paid-tier-only external generation with CREDITS · imports re-materialed +
  same audits (cohesion is non-negotiable) · Tier C stays a short named list (~a dozen max).
- **Next (● = freeze-compatible):** ● ship concept sheets (image-gen, Terry picks — the best
  art hour available right now) · ● ⚖ commission budget / tool+tier pick / hero budget review
  · Tier-B ops envelope post-freeze · WC-5 intake built on first real import.
- **Commit:** this one (docs only: strategy doc + this entry).

### 2026-07-20 (rb60) — Fable 5: **THE STORY GENESIS KIT** — the pre-pipeline lands as the first Slipway module (parallel to the project, zero coupling, zero code)

- **Did (Terry-directed, plan-approved):** created **`docs/slipway/`** — the cloneable-framework
  folder (ONE_SHOT_FRAMEWORK T5's name) — with its first module: the STORY kit, the
  pre-pipeline that runs BEFORE `PIPELINE.md` Stage 0 for any future game, and module #1 of
  Terry's stated long-term goal (an LLM game generator). Three files:
  ① **`README.md`** — the folder's purpose + **THE NON-INTERFERENCE LAW** (one-way street:
  the kit cites Ziptide as worked example; NO project doc may depend on the kit; changes here
  never gate the game, never need PIPELINE amendments, never touch the freeze) + the
  extraction-timing warning (the REST of the Slipway waits for the Stage-5 One-Session-World
  checkpoint).
  ② **`STORY_GENESIS.md`** — the method: **ten laws reverse-engineered from Ziptide's bible**
  (A1 the fantasy contains the verb · A2 gameplay↔fiction unity table · A3 the revelation
  ladder · A4 layered truth that recontextualizes-never-invalidates · A5 one moral question,
  factions as answers, endings as the player's answer · A6 companion as delivery vehicle +
  the relationship rules · A7 devices that generate content slots · A8 the two-audience law ·
  A9 canon lock + the three lists · A10 structure-as-data), each with its evidence and a
  concrete TEST; the craft imports (want-vs-need, mystery-box payoff ledger,
  implication-over-exposition, emotional-arc-first, the contradiction law, restraint as
  budget); **the 12-step method** ending at a CANON LOCK checkpoint (same ceremony as every
  pipeline checkpoint) and handing off to the Genesis Interview (rb57) → Stage 0/1; the
  quality rubric (0–2 × ten laws, lock needs ≥16/20 no zeros) + the Haiku-Workshop
  distillation loop + the honest Class-9 limit.
  ③ **`STORY_BIBLE_TEMPLATE.md`** — Ziptide's bible generalized into a 12-section fill-in
  skeleton (logline w/ verb test → locked meta w/ recontextualization list → protagonist w/
  HABIT slot → companion rules → factions-as-answers table → the mechanic↔fiction table →
  ladder → devices → endings → tone charter → the three lists → structure-as-data appendix
  incl. the world-template sections and chapter header tuple).
- **Classification:** every file carries `Stage: pre-pipeline (Slipway) · Type: research —
  PARALLEL TO ZIPTIDE`. No PIPELINE amendment (outside it by design), no CLAUDE.md edits, no
  project-doc links into the kit. The freeze and all lanes are untouched.
- **Next:** nothing — the kit is complete as paper. It activates the day a second game (or
  the generator) needs it; Ziptide-side, the only relationship is that our own story work may
  consult it as a mirror.
- **Commit:** this one (docs only: 3 new slipway files + this entry).

### 2026-07-20 (rb59) — Fable 5 → **📣 FOR GPT: THE FIRST-HOUR COMPLETION ORDER — plan + sequence** (Terry-directed)

- **Terry's intent, on record:** once the stack lands, he wants to issue ONE order: *"finish the
  entire first hour — through Toxic City and on to W002, the whole loop including the garden and
  machine building — built and ready for testing."* This entry is that order's plan and
  preconditions so it fires cleanly the moment it's legal.
- **Why it's realistic:** the first hour is assembly, not invention — the Director's Cut
  (`FIRST_HOUR_DIRECTORS_CUT.md`) composes CI-green systems that already exist
  (RepairableMachine, BuildSocket, GardenPlot, mining/conveyors, drone waves, travel,
  contracts); W001/W002 are built worlds; the v2.1 net-new beats are pack data + RILL lines.
- **THE SEQUENCE (each step gates the next):**
  1. **Terry's Golden Device Checkpoint** (rb38 — authorized, awaiting) → recovery exits per
     its own rules. *The only step that is Terry's.*
  2. **GPT's in-flight work lands:** hash law S1 (+S2/S3 as ordered in hwr32) — kills the bake
     bottleneck; PG-2 → PG-1 → PG-5 gates (hwr31) — so "built" can't mean invisible-broken.
     These stay AHEAD of bulk building (gates protect everything the sprint makes).
  3. **The hwr30 sprint runs** (city Stage A · weapon feel Phase 1 · drone excellence · stretch).
  4. **THEN Terry issues the completion order and GPT extends through the DC beat map:**
     Stage-1 first (the existing 22-beat contract polished testing-ready — needs NO story
     sign-off), then the v2.1 beats (artifact halves, Vex) ONLY after Terry's canon blessing
     (⚖ open). Include: W002 cycle-in-miniature (pump repair → BuildSocket extractor → one
     90 s wave → garden seed → first yield → changed ship), captions v2 + TM-1 boot audio as
     the polish layer, music beds wired to existing AudioProfile slots when Terry's Pro-month
     keepers exist.
- **Execution expectations (so the giant order reads as success, not failure):** it runs as a
  LONG CHAIN of small CI-green commits with batch deliveries — small-commits law + circuit
  breaker stand regardless of Terry's risk tolerance; long work resumes via this blackboard
  (the conductor gap rb57-G is real — HANDOFF state is the resume mechanism). "Ready for
  testing" = built side fully autonomous; the first-hour VERDICT (feel/comfort/fun/kid test,
  PIPELINE Stage-4 checkpoint) closes only through Terry's headset loop — plan 2–3
  build→test→dial cycles, with `PLAYTEST_AND_TELEMETRY.md` §1 at each sitting.
- **Estimate on record:** if the checkpoint passes, Stage-1 first hour headset-testable within
  GPT's first sprint cycle; v2.1 beats follow Terry's blessing.
- **⚖ Open for Terry:** DC v2.1 canon sign-off · the standing queue (name search ·
  price/no-MTX · severity bar · cut list · frame ledger).
- **Commit:** this one (docs only: this entry).

### 2026-07-20 (rb58) — Fable 5: **`docs/PIPELINE.md` v1.0 CREATED** — the spine is live, with Terry's amendment law (docs only)

- **Did (Terry-approved, executing rb57's proposal):** created **`docs/PIPELINE.md`** — the one
  ordered route, 12 stages, each with machines (links to existing docs — nothing rewritten),
  gates, a NAMED CHECKPOINT on the Quest-Golden-Checkpoint template, and status: 0 CANON ✅ →
  1 GENESIS DECISIONS 🟡 → 2 RAILS 🟢 (checkpoint = Terry's golden device pass, rb38, awaiting)
  → 3 GENOME SCHEMAS 🟡 → **4 VERTICAL SLICE 🟡 ← we are here** (checkpoint = the First-Hour
  Verdict) → 5 ASSEMBLY LINE (checkpoint = the One-Session World + blind compare) → 6 BATCH
  PRODUCTION (per-chapter verdicts + the convergence metric) → 7 CROSS-CUTTING PASSES →
  8 SHIP SHELL (checkpoint = the Stranger Test) → 9 BENCHMARK & CERT (Gold Candidate) →
  10 LAUNCH → 11 LIVE OPS ♾ (+ Slipway extraction after stage 5 proves). Cross-stage machinery
  section (HANDOFF, MISS_LEDGER/Class Law, checklist, EXCELLENCE_MAP, ⚖ ledger, CREDITS,
  merge protocol). **§A THE AMENDMENT LAW (Terry's addition):** the pipeline changes ONLY
  through this doc — any stage/gate/checkpoint/order change lands in the amendment log IN THE
  SAME COMMIT, version bumped; contradicting the spine without an amendment = DRIFT (miss-
  ledger entry; future `PIPELINE_DRIFT` CI audit). New docs declare `Stage:`/`Type:` headers
  from now on; retrofitting the existing corpus = a boarded boards-pass task, NOT a unilateral
  sweep. CLAUDE.md Key docs now points at the spine first.
- **📣 All lanes:** the stage list is v1.0, not scripture — amend via §A with reasons (that's
  what the law is FOR). `CURRENT_EXECUTION_CHECKLIST.md` should mirror stage status at its
  next refresh (its owner's call). GPT: hwr30–33 work all lives in stages 2/4 — no conflict.
- **Next:** first amendments will likely come from the multi-model merge results; gaps G/H/I
  (conductor, session reliability, generated route tests) belong to stage 2/5 rows when
  promoted to docs.
- **Heads-up:** the "current position" line in PIPELINE.md is the new one-glance answer to
  "where are we?" — keep it truthful; it says stage 4, blocked on the stage-2 device
  checkpoint (Terry's headset pass, rb38).
- **Commit:** this one (docs only: PIPELINE.md + CLAUDE.md pointer + this entry).

### 2026-07-20 (rb57) — Fable 5: three chat-research passes recorded + THE PIPELINE spine proposal — Terry's status: "we're pretty good; now implement, keep building, let the Class Law fill the gaps"

- **Did (these were chat-only research at Terry's direction — recorded here so they're not lost):**
  ① **The Genesis Interview** (how the system asks the right questions given only a story bible):
  implication pass sorts every design domain into DERIVED (bible already decided — extract with
  quoted evidence) / OPEN (genuinely silent) / MASQUERADING (tonally implied, never decided —
  the miss-ledger's "values mistaken for decisions" class); question taxonomy Q0 identity →
  Q1 loops → Q2 genre-delta → Q3 variety budget → Q4 constraints → Q5 ship-backward (the
  benchmark as interrogatives); **the question generator IS the miss ledger played backwards**
  (every missed class becomes a day-one question for the next game); lazy elicitation (a
  question is asked when its answer first blocks a gate — the ⚖ pattern formalized); answers
  land as structure (constitution clauses/genome fields/benchmark rows), never prose. Slipway
  front door.
  ② **The Platform Constitution** (how constraints become hard rules BEFORE building, per
  industry practice): cert floors (VRC/TRC-class, non-negotiable) → **measured** ceilings via
  STRESS-LAB probe scenes on min-spec device (budgets from measurement at ~75% margin, never
  folklore — current Meta Quest 3 guidance on record: 500–1000 draw calls, 1–2M tris, ~4.5 ms
  GPU/8 ms CPU, high performers sit 450–650) → **the FRAME LEDGER** (⚖: split the 13.9 ms by
  the game's identity — spend the frame where the fantasy lives: vistas/water/creatures over
  shadows/crowds) → every budget row must name its gate (budget without a gate is a wish) →
  renegotiation only with device evidence. Field note: CPU/script time is now the more
  dangerous constraint than GPU on Quest-class hardware — stress lab needs a script-load probe.
  ③ **Pipeline-distance assessment** ("how close to it-just-runs?"): honest ~35–40% blended.
  Built: the factory floor (CI+headless authors+PlayMode+clean room+golden hash+booth+audits).
  In flight: hash law, PG gates, sprint. THREE NEW GAPS no prior pass named — **G the
  CONDUCTOR** (no machine-readable work queue/dispatcher/state tracking; boards are prose;
  model quota + Terry attention unbudgeted), **H SESSION RELIABILITY** (the least reliable
  pipeline stage is the model sessions — evidence: conflict markers pushed twice this week;
  needs pre-push hooks + post-session output verification + defined units of work), **I
  PER-WORLD GENERATED ROUTE TESTS** (the PlayMode suite is hand-written for the golden slice;
  the compiler must EMIT each world's route test or scaled worlds ship gate-blind). Biggest
  research risks: reviewer calibration (WC-7) + the conductor; everything else is assembly of
  proven patterns. The "we always find more" meta-answer: find-rate IS the metric — recurring
  passes, readiness = two consecutive passes finding nothing of consequence.
  ④ **Organization answer (Terry's closing question):** we're organized for archaeology, not
  navigation — 60+ docs, a dozen envelope prefixes, no spine. **Proposal: `docs/PIPELINE.md`**
  — ONE ordered ~12-stage route (0 canon → 1 genesis → 2 rails → 3 genomes → 4 vertical slice
  → 5 assembly line proven → 6 batch production → 7 cross-cutting passes → 8 ship shell →
  9 benchmark/cert → 10 launch → 11 live ops), each stage: inputs → machines (links) → gates →
  **a named CHECKPOINT on the Quest-Golden-Checkpoint template** → outputs → verdict owner.
  Every doc gets `Stage:` + `Type:` header fields + a CI docs-index check (DOC_ORPHAN);
  status consolidates into the stage column; aggressive archiving. **📣 All lanes: this
  re-headers everything — bless/amend the stage list via HANDOFF before anyone executes;
  not imposed unilaterally.**
- **Terry's status statement, on record:** "we're pretty good at the moment — we just need to
  implement all of those things and then keep building; we'll keep noticing issues but our
  acute-issue-→-system-adjustment training (the Class Law) should start filling gaps itself."
- **Next:** GPT continues implementation (hash law, PG gates, sprint per hwr30-33); gaps
  G/H/I should be added to `WORLD_ASSEMBLY_READINESS.md` in its next edit; PIPELINE.md awaits
  lane blessing; the ⚖ queue for Terry stands (name search · price/no-MTX · severity bar ·
  cut list · frame ledger).
- **Heads-up:** the Genesis Interview + Platform Constitution + conductor/session/route-test
  gaps exist ONLY in this entry and the chat — first lane to need them should promote them to
  docs (planning lane, freeze rules).
- **Commit:** this one (docs only: this entry).

### 2026-07-20 (hwr33) - Fable 5 architect: 🏁 FINISHED-GAME BENCHMARK — the gap audit vs a shipped Quest title (Terry-directed, one of multiple model runs, planning only, zero code)
- **Terry's instinct, now confirmed with repo evidence:** "even in our planning we're missing
  important parts of what anyone would call a finished game." New doc:
  **`docs/design/FINISHED_GAME_BENCHMARK.md`** — Meta store cert (VRC classes) + commercial
  completeness bar + live-ops, every item verdict-ed ✅/🟠/📋/❌ with grep evidence.
- **The headline (§4):** the plans are deep on world/content quality and near-silent on the SHELL
  of a shipped product. Verified-unplanned set: entitlement check · HMD-doff pause/mute ·
  tracking/boundary/recenter · controller-loss handling · in-game pause menu · audio mix bus +
  volume sliders · handedness swap · seated mode/height recal · save-management UX (confirm/
  delete/recovery) · **ending + credits roll (the 80-world vision has no defined END for v1)** ·
  device soak protocol · store listing kit · IARC/privacy · version display · cloud saves ·
  telemetry (first-hour funnel!) · achievements · localization decision. Bright spots: music
  licensing already ruled on (📋), save migration law ✅, onboarding is the best-covered item ✅,
  fall/stuck recovery device-proven ✅.
- **Proposal (§5, for the multi-model merge to refine):** a SHIP SHELL lane (SS-01…SS-18
  envelopes; most are 1–3 commit LLM-buildable systems), ordered lifecycle/tracking first →
  pause+settings mini-program on FH-S07's surfaces → v1 ending = a defined stopping beat +
  credits when the DC story locks → store kit last-mile → telemetry EARLY if Terry wants funnel
  data. New EXCELLENCE_MAP row "Ship shell" so the map stops hiding the hole.
- **⚠️ Multi-model merge note (rb43 protocol):** verdicts carry evidence — siblings should
  re-check, then ADD rows, not re-litigate. Merge by section; Terry adjudicates; every ❌ becomes
  an envelope or a recorded CUT.
- **Commits:** this push (docs only).

### 2026-07-20 (rb44) — Fable 5: FACTORY MASTER ORDER — the multi-research merge (planning only, zero code)

- **Did:** Terry asked for the merge: read every parallel research (ONE_SHOT_BUILD_FRAMEWORK ·
  LLM_FIRST_BUILD_PIPELINE · PERCEPTUAL_GATE_PROGRAM (PG-1..5 rules+tests verified LANDED) ·
  WORLD_IMPROVEMENT_FRAMEWORK (live) · WORLD_ASSEMBLY_READINESS), checked conclusions against the
  branch, and wrote **`docs/design/FACTORY_MASTER_ORDER.md`** — the reconciled single execution
  order optimized for accurate AND fast. Verdict: the models CONVERGED on the five-layer factory
  (treat the shape as settled); differences are complementary. Seven blanks nobody covered, the
  big ones: PlayMode promotion is in no work order · TWO hash laws are about to exist (S1 must
  reuse the WorldImprovement hash helper, not parallel it) · CI throughput (no concurrency
  groups; superseded runs waste the queue) · HANDOFF is the collision hotspot. Speed thesis on
  record: round-trip cost is the schedule — gates and the hash law ARE the speed program.
- **Next:** Phase 1 = UNSTICK (S1/S2 hash law · PlayMode → blocking · throughput pack · license
  cron) with the owner map in §4. Terry adjudicates before any build starts.
- **Heads-up (cross-operator):** GPT/Architect — §1.2 of the doc: please converge on ONE hash
  stamping helper before S1 lands, or we fork staleness dialects.
- **Commit:** this one (docs only: the merge doc + this entry).

### 2026-07-20 (rb43) — Fable 5: ONE-SHOT BUILD FRAMEWORK study (planning only, zero code)

- **Did:** at Terry's direction, researched how to (a) build the rest of the game with near-zero
  escaped bugs and (b) eventually clone this project's bones as a reusable framework for any
  future game. Three research lanes (gate-coverage audit vs the 16 historical failure classes ·
  industry one-shot-correctness practice · framework/game split + extraction plan) synthesized
  into **`docs/design/ONE_SHOT_BUILD_FRAMEWORK.md`**. Headlines: the PlayMode lane already boots
  `_Boot` + runs the golden travel route headless but is path-filtered and non-blocking —
  promoting it is the single biggest lever; top-10 missing gates ranked; the one-shot ladder
  (promote → data seams → sim fuzzing → perf → section protocol → ensemble-by-verifier for
  multi-model work); the thin-template/thick-packages extraction plan (six UPM packages + docs
  skeleton, harvested only after ship with game 2 as the forcing function); the honest device-only
  residue. **⚠️ Multi-model note:** Terry is running this SAME study on other models and merging —
  if you find sibling docs, merge by section per the doc's §7 (testable claims decided by tests).
- **Next:** nothing until Terry adjudicates the merged takes. The doc's §8 shortlist is the
  proposed first build slice (all gate/process work, no gameplay).
- **Heads-up:** lane-A audit confirmed at least 4 of the 16 historical failure classes would have
  been caught by a blocking PlayMode lane. Also on record: the asmdef reality (Visuals below
  Content/Gameplay) differs from CLAUDE.md's stated ladder — acyclic and fine, but doc ≠ reality.
- **Commit:** this one (docs only: the framework doc + this entry).

### 2026-07-19 (rb56) — Fable 5: FINISHED-GAME BENCHMARK + THE CLASS LAW + the MISS LEDGER (research, zero code)

- **Did:** Terry: research completed-game benchmarks, compare to our map (multi-model, results
  merged later), and formalize the feedback law — "any acute issue is a systemic issue."
  ① ten categories of "done" — now at **`docs/design/FINISHED_GAME_BENCHMARK_RB.md`** (hwr33's
  evidence-verdict take landed the canonical path first; my unique rows are merged into its §5b:
  name clearance, business/cut-list/severity ⚖ pages, review-response policy, the Class Law) —
  (B1 feature ·
  B2 content · B3 presentation · B4 quality bars · B5 player lifecycle · B6 compliance/legal ·
  B7 store/marketing · B8 business · B9 live ops · B10 preservation) scored honestly:
  B1/B2/B4/B10 planned-strong; the real holes: **B5 the ending EXPERIENCE** (endings designed
  as story, not as staging — no roll-credits moment, no completion ceremony, no replay answer;
  first hour got a Director's Cut, last hour never did), **B6 NAME CLEARANCE** ("Ziptide" never
  trademark-searched — an afternoon, do it during the freeze), **B7 the campaign** (DC-4 is
  tooling, not the deliverable), **B8 business decisions** (premium/no-MTX implied, never
  decided), **B9 live-ops** (every plan ends at launch), B3 credits-roll/legal-screens/error-
  fiction. Priority order in §4 (name search first). ② **THE CLASS LAW formalized** (§3,
  generalizing hwr31's device-bug ratchet to ALL misses): every miss gets a 5-field entry —
  WHAT · FOUND BY · **WHY MISSED (which system layer failed to see it)** · CLASS · SYSTEM
  CHANGE — and a fix without a system change is a loan. ③ **`docs/MISS_LEDGER.md`** created and
  SEEDED with this exercise's own ten findings, including the meta-entry: whole categories were
  missed because planning grew game-outward and nothing forced ship-backward — system change =
  the ledger itself + the benchmark as a recurring milestone gate + EXCELLENCE_MAP rows per
  category. Ledger joins session-start reading beside HANDOFF.
- **Next:** other models' benchmark runs → merge per ONE_SHOT_FRAMEWORK §8 (union of
  categories); Terry: the name search (⚖ HIGH, freeze-compatible) + the three ⚖ decision
  one-pagers (price/no-MTX, severity bar, cut list). EXCELLENCE_MAP rows + checklist row for
  the recurring benchmark = next boards pass.
- **Heads-up:** the ledger's law only works if entries CLOSE on verified system changes, not
  fixes — reviewers should bounce any entry closed without one.
- **Commit:** this one (docs only).

### 2026-07-19 (hwr32) - Fable 5 architect → **📣 WORK ORDER FOR GPT #3: LLM-FIRST BUILD PIPELINE** (Terry-directed — change anything, first-time-right, by LLM)
- **Terry's goal on record:** "build or rebuild or change every aspect and have it work perfectly
  the first time — done by LLM." Researched against the actual pipeline; spec:
  **`docs/design/LLM_FIRST_BUILD_PIPELINE.md`**.
- **Headline finding (changes how everyone should think about bakes):** CI already runs the ENTIRE
  author/patcher pipeline headlessly on every push (`BuildAndroid.PatchScenesAndAudit` — 20+
  authors, WorldSpecCompiler, all scene patchers). "The cloud can't make .asset files" is FALSE at
  pipeline level. The real faults are structural:
  **F1** create-only authors (`if (LoadAssetAtPath != null) return;` — verified
  `ArenaLayoutLibrary.cs:34`; recipe-code changes are SILENTLY ABSORBED by stale committed assets
  — the W007-vista / arena-pads / runbook-delete-then-regen disease) ·
  **F2** two sources of truth (committed assets vs CI-regenerated; stale committed wins) ·
  **F3** Terry's bake sittings exist only to un-stick F1/F2, not because assets need a human.
- **The fix — RECIPE-HASH LAW (§3, aligns with Forge VI):** authors stamp
  hash(recipe data + RecipeVersion) into generated assets; `EnsureAllAuthored` becomes
  `EnsureAllCurrent` (mismatch → regenerate IN PLACE, same GUID); `ForgeStaleness` buckets
  (SafeAuto/Review/Breaking — already exists, tested) generalized to all recipe kinds; new CI
  Blocker `ASSET_STALE_VS_RECIPE`. Result: edit recipe → push → exactly the affected assets
  regenerate → gates judge → APK carries the change. No menus, no deletions, no human.
- **GPT staged order (do NOT one-shot):** **S1** hash law on the create-only `*Library`/`*Author`
  set + staleness blocker + stale-asset test (~5 commits — permanently deletes most of the
  runbook §1 bake list) → **S2** bot-commit `bake` workflow (CI_VERDICT-writer idiom) so repo
  state always equals recipe state (~3 commits + workflow) → **S3** pre-flight change-impact
  report (generalized `ForgeStaleness.Affected` as the front door of every change) → S4/S5 are
  PG-5/PG-6 + Forge VI, already ordered elsewhere.
- **Sequencing vs the other orders:** hwr31 PG-2/PG-1 first (live bug classes), then S1 here —
  it is itself a gate and makes every hwr30 sprint item land born-current. §5 of the doc states
  the honest limit: pipelines guarantee fidelity, not fun — feel stays Terry's seat.
- **Commits:** this push (docs only).

### 2026-07-19 (hwr31) - Fable 5 architect → **📣 WORK ORDER FOR GPT #2: PERCEPTUAL GATE PROGRAM** (Terry-directed — "make sure all that is implemented")
- **Why (Terry):** device sessions keep finding what CI can't see — tiny weapons, upward muzzles,
  the Leave-door loading a scene absent from the Golden build, the shipyard walkway gap. "It's
  going to be very difficult to build 80 worlds at consistent quality if our system isn't
  top-notch." This is the systemic fix, researched against the actual audit code.
- **New spec (implement from this): `docs/design/PERCEPTUAL_GATE_PROGRAM.md`** — five gates with
  file-level implementation detail on the existing `WorldAuditRunner`/`ForgePhotoBooth` infra:
  **PG-1** held-item scale + grip-pose audit (bounds 0.06–0.55m, compound-scale trap, muzzle ≤25°
  off grip-forward) · **PG-2** build-profile-aware travel audit (the sharp lesson: the
  `TRAVEL_DEST_NOT_IN_BUILD` blocker EXISTED but validated `EditorBuildSettings`, not the Golden
  3-scene universe — gates must run against the artifact's actual scene set) · **PG-3** walkable-
  continuity audit (route sampling: holes/steps/squeezes on spawn→marker→door→berth edges) ·
  **PG-4** reach-envelope audit (generalize the coupler's child-reach law to every interactable) ·
  **PG-5** referenced contact sheets (mannequin + 10cm grid so "tiny" is visible in 2D) + a
  Definition-of-Done review stamp: `SHEET REVIEWED <run> — verdict` in HANDOFF or the lane isn't
  closed · **PG-6** on-device agent bus (designed, POST-recovery; Meta Quest Agentic Tools/MCP
  cover the device side, we build only the in-game verb server).
- **GPT implementation order: PG-2 → PG-1 → PG-5 → PG-3 → PG-4** (live bug class first, then the
  class that just bit us, then widest coverage per commit). Each gate: Blocker tags + a
  deliberately-broken-scene test proving it fires. Slot ahead of/alongside the hwr30 sprint —
  gates first protect everything the sprint builds.
- **THE RATCHET LAW (adopt into `OPERATOR_START_HERE.md` Definition of Done, all lanes):** every
  device-found bug dies three deaths — fix + a gate that would have caught it + a HANDOFF line
  `RATCHET: <bug> → <gate>`. A fix without a gate is a loan. GPT's own coupler/exit workflow
  coverage from tonight is the model case, now law.
- **The bigger frame (doc §3, for every operator):** the five-layer game factory — spec-is-truth,
  deterministic generators, gate-per-quality-dimension (logical/spatial/perceptual/performance/
  feel), the evidence blackboard, the ratchet. Per-world excellence contract is enforceable as a
  `WorldExcellenceAuditRules`; EXCELLENCE_MAP should gain a gate-coverage column (aspects with
  zero gates are where the next escape lives). Terry's stated end goal on record: this framework
  should be able to build OTHER games consistently, not just Ziptide.
- **Commits:** this push (docs only).

### 2026-07-19 (hwr30) - Fable 5 architect → **📣 WORK ORDER FOR GPT: THE BIG BUILD SPRINT** (Terry-directed, fires AFTER the retry checkpoint passes)
- **From Terry, verbatim intent:** after the next test run, GPT should run "a super long ass task
  and just get me a whole bunch of s*** built and working... like a bunch of stuff. If it ends up
  getting built but it's kind of broken, that will help us dial in our system to not end up with
  everything broken. If it works then we can just keep rolling." Terry's Fable 5 quota is nearly
  spent; **GPT owns this sprint end-to-end.** Today is his only headset day — optimize for
  VISIBLE-ON-DEVICE change per CI cycle, pipelined so a fresh build is always ready when he
  takes the headset off.
- **Gate to start:** the `c3f9a4d` retry checkpoint route passes and recovery formally exits per
  `docs/recovery/RECOVERY_VERIFICATION_SYSTEM.md`. Until then, only GPT's in-flight fixes move.
- **THE ORDERED SPRINT (each item = its own small commits, CI green between; specs already exist):**
  1. **Close recovery** — record the exit, lift the freeze per its own rules, refresh
     `CURRENT_EXECUTION_CHECKLIST.md`.
  2. **City Stage A — building grammar** (`docs/design/CITY_STREETSCAPE_AND_AMBIENT_LIFE.md` §5-A,
     recipe details §2.2/2.3/2.6, Quest budget accounting §3): composite base/middle/cap buildings
     in `CityBuilder`, footprint/height variation, 3-warmth emissive windows, parapet + roof
     clutter, curbs, ~8-slot palette. Add the per-district object-budget audit rule the doc
     specifies. Queue the ToxicCity scene regen as a 🔧 runbook step for Terry's bake sitting.
     *This is the direct attack on Terry's "4-year-old built it" verdict — highest visible value.*
  3. **Weapon feel Phase 1** (`docs/design/WEAPON_FEEL_AND_ARSENAL.md`, Terry-approved; rb42 was
     holding for headset confirmation — today IS that): grip/pose, recoil/haptic read, muzzle
     response on pistol + taser first (the two weapons every session touches).
  4. **Drone excellence pass** (`docs/design/ENEMIES_ENCOUNTERS_AND_BOSSES.md`): Terry's on-record
     complaint — "oval shaped red things that fly around and shoot a light." Telegraphs, movement
     personality, hit reactions, projectile readability.
  5. **Stretch, only if 2–4 land green:** City Stage B street layer (wires/lamps/signs/alleys, §5-B).
- **Also queue for Terry's ONE bake sitting (runbook, batched):** FH-S07 home surfaces author,
  sky-vista + arena regens already listed, Field Camera author (code-complete, never baked),
  ToxicCity regen after Stage A lands. One sitting, commit generated assets, one build.
- **Terry's stated risk tolerance (this is the system-tuning experiment):** built-but-kind-of-broken
  is ACCEPTABLE DATA — the point is to stress whether our gates catch breakage. So: build boldly,
  but keep the *gates* honest — never skip/weaken a gate to get green, never batch unrelated
  changes into one commit, log every known-broken edge in HANDOFF + runbook instead of hiding it.
  The laws that stay absolute regardless of tolerance: circuit breaker (3 CI-reds on one task →
  stop + HANDOFF), patcher-indirection (no hand-edited scene/prefab YAML), report-only categories
  (rig ownership, travel, input actions, inventory persistence, build settings), comfort laws,
  one-economy chokepoint, `ZIPTIDE:` diag tags on everything new.
- **Explicitly OUT of this sprint:** FH-S08 director (multi-day, not headset-bound), NPCs/ambient
  life tiers (designed in hwr29, not sequenced), multiplayer, anything Forge IV+.
- **Lane courtesy:** Fable 5 (me) claims NOTHING here — this whole order is GPT's. Picasso: Stage
  A touches CityBuilder only, primitives-first per your Toxic Venice brief; material/kit upgrades
  stay yours.
- **Commits:** this push (docs only).

### 2026-07-18 (hwr29) - Fable 5 architect: 🏙 CITY STREETSCAPE & AMBIENT LIFE research (Terry-directed, planning ONLY, zero code)
- **Terry's verdict on Toxic City** (accurate — verified against `CityBuilder.cs`): the built city
  is a functional gray-box skeleton (4-step contract works, drones work) but reads "like a
  4-year-old built it in a game editor." Root cause is UNIFORMITY: 1-cube buildings, 2 colors, no
  ground floor/door/roofline, slab ground, nothing moving, nobody home.
- **New doc: `docs/design/CITY_STREETSCAPE_AND_AMBIENT_LIFE.md`** — the researched anatomy of a
  believable downtown (Lynch skeleton, base/middle/cap facade grammar, street section + curb,
  furniture/wires/signage layer, 3-warmth lit windows, weathering asymmetry, motion, sound,
  layered skyline), full Quest-budget accounting, and a 4-tier ambient-life plan (machine
  citizens → posted figures incl. a PHYSICAL DOCKMASTER → 3–6 cloaked walkers → Forge V distant
  lanes). Staged A–E recipe upgrade proposal, each stage = CityLayoutDefinition data + builder
  functions + audit rule; standing rejections listed so no lane burns tokens on crowd sim.
- **Routing:** near-field citizens are UNOWNED design space (Forge V LS-4 is distant-only by its
  own law — checked). Streetscape recipe = city/worlds lane; material quality stays Picasso's.
  NOTHING here is authorized to build yet — Terry sequences it against the first-hour work after
  the golden checkpoint. 📣 Picasso: §2/§3 cite your budget doc + Toxic Venice brief; please
  sanity-check the accounting when you next pass.
- **Also this session:** CI RED root-caused + fixed (hwr28, GateGap5 staleness timer) — verdict
  GREEN again on `cfa3d34a`.
- **Commits:** this push (docs only).

### 2026-07-18 (hwr28) - Fable 5 architect: 🚑 CI RED root-caused + fixed — GateGap5 board-staleness timer, NOT a code break
- **What went red:** every source push since ~2026-07-18 failed EditMode 1048/1049 on
  `GateGapTests.GateGap5_NoBoardClaim_RotsSilently`. Cause: `docs/SPRINT.md`'s 🟡 title line
  ("opened 2026-07-03") crossed the 14-day staleness window today. A governance time bomb —
  no one's commit broke anything; docs-only pushes tripped it identically.
- **Fix (the test's own remedy — re-date + HANDOFF note):** title now reads "opened 2026-07-03,
  re-affirmed 2026-07-18". The Quality Bar program IS still the active umbrella (FH-S07 is a
  legitimate 🟡 awaiting Terry's bake; `CURRENT_EXECUTION_CHECKLIST.md` carries current order).
- **📣 All lanes:** this timer re-arms 2026-08-01. Whoever tends boards next: either finish/release
  the 🟡 rows or re-affirm the date again with a note. Do NOT delete the test — it did its job.
- **Commits:** this push (docs only).

### 2026-07-18 (rb55) — Fable 5: THE ONE-SHOT FRAMEWORK — my take on zero-defect building + the cloneable bones (research, zero code)

- **Did:** Terry commissioned the meta-question: how does the rest build "in one shot without
  bugs," and how do Ziptide's bones become a cloneable framework for ANY game — multiple models
  answering independently, results merged. Wrote **`docs/design/ONE_SHOT_FRAMEWORK.md`**,
  thesis-first for mergeability: **T1** the equation (generated-from-validated-data · every
  observed failure class = permanent tripwire · judgment concentrated · machinery proven by
  regeneration/hash-stability · operators replaceable because the SYSTEM carries standards);
  **T2** seven layers L0 substrate → L6 judge, with the modularity answer = the L2/L3 split
  (rails frozen-by-gates, art is data behind clamps); **T3** the nine-class failure taxonomy
  from OUR actual history (blind push · hand-edit drift · ownership violation · substrate
  drift · dangling reference · provenance gap · gate blindness · operator collision ·
  irreducible judgment) each with its built/planned cure + the generalized ratchet law
  ("nothing ever breaks the same way twice"); **T4** the build protocol — sections down the
  assembly line with the convergence metric: NEW-failure-classes-per-section → 0 = the moment
  batch one-shot becomes real; regeneration of locked worlds as the perpetual self-test;
  **T5 THE SLIPWAY** — the extraction kit (what clones as-is / as template / never), extraction
  triggered AFTER the first fully-compiled section proves the line, not before; **T6** the
  four-property warranty behind "art changes, nothing breaks" (interface stability · clamped
  expressiveness · provenance completeness · regression visibility); **T7** honest limits
  (class-9 feel is discovered not derived; substrate drift recurs per engine upgrade; promise
  is no-repeat, not omniscience); **§8** the multi-model merge protocol (union taxonomies ·
  intersect laws · compare layer boundaries · REQUIRE vocabulary mapping to existing repo
  nouns per the ASSET_FORGE_MAP rosetta pattern — no second glossaries in meta-plans either).
- **Next:** other models produce their takes; merge per §8; merged result supersedes this doc
  and lives beside the assembly-line runbook. 📣 GPT: this pairs with rb54's review ask.
- **Heads-up:** T5's timing is load-bearing — extracting the Slipway BEFORE a proven compiled
  section extracts hope, not machinery. Resist the temptation.
- **Commit:** this one (docs only).

### 2026-07-18 (rb54) — Fable 5 → **📣 MESSAGE FOR GPT: review the assembly-readiness audit** (Terry-directed)

- **From Terry:** the six assembly gaps (rb53, `docs/design/WORLD_ASSEMBLY_READINESS.md`) are
  "stuff we need to get going on at some point here" — GPT, please take a look.
- **What to review:** the six gaps (A ride-scenes · B world gameplay genome · C lore forge ·
  D flag-graph validator · E per-world voice formula · F assembly-line runbook) and the
  priority order (D→B→A+E→C→F). Specifically sanity-check against YOUR lanes: ① **GAP D**
  overlaps your validator/ledger instincts — is EditMode flag-graph analysis over ZiptideFlags
  + world data + line triggers + ending math feasible as specced, and does anything in your
  contract/ledger work already half-build it? ② **GAP B**'s job-shape templates must not
  collide with your firing-range/encounter work or the first-hour contract JSON — flag any
  seam. ③ **GAP A** (ride-scenes) touches traversal/ship owners — if that's your lane's
  machinery, claim it. ④ Anything in the six you'd re-rank, merge, or call already-covered.
- **Context for the review:** this week's planning set (all PROPOSED/PLANNED, zero code, freeze
  intact): Forge V–VII horizon plans, DAMAGE_RESPONSE_AND_RUIN, WORLD_PHYSICS_VARIANTS,
  RILL/Cal line sets, GAP_AUDIT_JULY2026 + its six planned fixes (SFX Forge, localization ✅
  law, family profiles, contract ledger, playtest+telemetry, resume moment),
  TITLE_MENU_EXPERIENCE, FIRST_HOUR_MUSIC_DIRECTION (Terry's two canonical prompts + kinship
  dial). Everything queues behind the gate ladder — recovery exit (Terry's headset checkpoint,
  rb38) is still rung one, and NOTHING here changes that.
- **Reply route:** HANDOFF entry as usual; disagreements welcome — re-rank with reasons and
  Terry arbitrates.

### 2026-07-18 (rb53) — Fable 5: ASSEMBLY READINESS audit — the six gaps between "frameworks exist" and "the game builds itself" (planning only, zero code)

- **Did:** Terry's pause question: with the frameworks laid out (worlds/creatures/architecture/
  music/SFX), what's still missing for the game to build itself — cutscenes? Wrote
  **`docs/design/WORLD_ASSEMBLY_READINESS.md`**. Headline: the ART of a world builds itself on
  paper; the CONTENT and connective tissue don't. Six gaps: **A — ride-scenes** (Terry's
  cutscene instinct, VR-ruled: never steer the body — the legal form is the CONVEYANCE (Alyx
  tram): authored moments staged around something the player chose to board — barge/lift/ship/
  gate; head sovereign, hands live, skip = throttle up, ≤1/world; lands as LS-2 `ride` kind);
  **B — world gameplay genome** (WC-1 builds the body, nothing scales the PURPOSE: closed
  job-shape templates + encounter beats + reward/mystery/log/ride slots as validated data);
  **C — the Lore Forge** (nobody owns producing/validating ~60 chained wreck logs + 80 mystery
  objects; registry + chain validation, prose stays Terry-reviewed); **D — flag-graph
  validator** (evidence: the bible's own audit found PLAYER_TRUSTED_RILL orphaned; pure
  EditMode granted/consumed/reachability analysis, FLAG_ORPHAN warn→blocker); **E — per-world
  voice formula** (music got genome+formula; lines need the equal: a 4-line kit per world
  drafted from its data row against the craft rules, PROPOSED pattern at batch throughput);
  **F — THE ASSEMBLY LINE RUNBOOK** (the capstone: World N bible-row→device-pass in one
  ordered manual; "one session walks one standard world through it in one sitting" = the
  program's definition of success; written LAST from the first compiled world's experience).
  Priority: D (cheapest, protects everything) → B → A+E → C → F. Known-not-reopened: VO
  casting deferred, game modes (Terry's own flag — separate pass), Earth kit scheduled,
  multiplayer paused.
- **Next:** B/C/E schemas + D's rule list are paper-draftable during the freeze if Terry wants
  them next; otherwise this queues behind the gate ladder like everything.
- **Heads-up:** GAP A must never soften into head-steering — the freedom contract is absolute;
  a ride-scene that can't be looked away from is a bug, not a feature.
- **Commit:** this one (docs only).

### 2026-07-18 (rb52) — Fable 5: THE CANONICAL MUSIC PROMPT + kinship dial — Terry found the sound (planning only, zero code)

- **Did:** Terry's first free-tier Suno session produced the title-theme candidate ("perfect
  mixture of Prospect and Halo and Blade Runner 2049") and his prompt is now the score's GENOME,
  recorded verbatim in `FIRST_HOUR_MUSIC_DIRECTION.md` §1a: atmospheric sci-fi space-western /
  wordless youthful choir / deep string drones / industrial synths / sub-bass / metallic tubular
  textures / used-future 70s cinema / quiet tide sounds. License path per the §0 ruling: remix-
  of-free-tier is unsafe — ask Suno support in writing for that track OR regenerate under Pro
  with the same prompt; keeper's detected bpm/key then replaces the generic 76/Dm defaults in
  every derived prompt. **§1b THE KINSHIP DIAL (Terry's design):** K3 FULL VOICE (title, first
  crossing, endgame — canonical prompt verbatim) · K2 RELATIVE (signature story worlds — all
  DNA words + toning-down operators: demote percussion, add distant/faint/half-remembered,
  world palette line) · K1 TRACE (default worlds — own palette + ONE thread: faint distant
  choir surfacing, half-remembered) · K0 OWN VOICE (the weird worlds — own palette, keep only
  quiet tide sounds or nothing). Rarity logic mirrors the physics-dial law. §3 rewritten:
  all 9 first-hour cues re-prompted as derivations of the canonical DNA (Toxic City = the
  named K2 exemplar: "toned-down, quieter, not as massive"), plus K0 examples (The Hum, Mirror
  Flats) and the world-prompt formula for the other ~70 worlds.
- **Next:** Terry regenerates the title keeper under Pro (or gets written retroactive rights),
  logs prompt+link as keeper #1, detects keeper bpm/key → propagate into the derived prompts.
- **Heads-up:** "quiet tide sounds" is the one DNA element kept at every kinship tier K1+ —
  it is the game's name in audio; don't tone it out.
- **Commit:** this one (docs only).

### 2026-07-18 (rb51) — Fable 5: first-hour music direction + Suno cue sheet (planning only, zero code)

- **Did:** Terry has Suno free tier and asked what music to go for in the first hour. Wrote
  **`docs/design/FIRST_HOUR_MUSIC_DIRECTION.md`**: ⚠️ THE LICENSE LAW up top (free tier =
  non-commercial, rights attach at creation time — free-tier keepers CANNOT ship; plan = find
  the sound free, regenerate keepers in one paid Pro month at wiring time, verify terms that
  day, CREDITS.md lines per track); the direction — **"tidal ambient"** (water + salvage metal
  as instruments: deep pads/sub-drone foundation, processed water + hull-resonance textures,
  celesta motif voice, distant wordless choir RESERVED for ancient/awe, percussion only in
  combat; wonder-first/kid-safe per tone charter; family coherence via ~76 BPM + D minor in
  every prompt — matches ADAPTIVE_AUDIO's phase-aligned-stem future); **THE TIDE MOTIF** (one
  hummable 5–7 note phrase; generate the title theme FIRST in bulk, the keeper take DEFINES the
  motif; reprise by description or wave-editor reuse); a **9-cue sheet** mapped to the
  Director's Cut minute map (title_berth · w000_wake · flight_punchit · w001_toxiccity_bed ·
  w001_gate_wake build · ziptide_crossing stinger [the hour's peak] · w002_cistern_bed ·
  w002_defend [same BPM as bed 7 for stem alignment] · return_changed_ship motif reprise) with
  paste-ready Suno prompts per cue; workflow (≥4 takes/cue keep 1; keeper tests; Audacity loop
  trims; `firsthour_<cueid>_vX.wav` naming + PROMPTS_LOG.md; beds drop into EXISTING
  AudioProfile slots with no new code, stingers wait for TM/SFX hooks; beds stay low per the
  silence-is-authored law).
- **Next:** Terry generates at his pace (free-tier arithmetic in-doc: ~1 cue/day comfortable;
  title theme deserves 2–3 days of takes). Post-freeze: cues 2/4/7 wire into AudioProfiles
  immediately; cue 1 = TM-1.
- **Heads-up:** do NOT let any free-tier take reach a store build — the regeneration step is
  load-bearing, not bureaucratic.
- **Commit:** this one (docs only: music direction doc + this entry).

### 2026-07-18 (rb50) — Fable 5: TITLE MENU researched + planned ("the berth before dawn") + gap-audit sign-offs recorded (planning only, zero code)

- **Did:** ① Recorded Terry's sign-offs in-doc: LOCALIZATION_DECISION → ✅ APPROVED (law);
  FAMILY_PROFILES gallery stays family-shared ✅; SFX middleware rejection confirmed ✅.
  ② Title menu: audited the truth — the Home Hub SKELETON is excellent (BootLoader → boot-hold →
  HomeHubRuntime.Configure(scene, travel cb); pure HomeHubFlowState travel-exactly-once; save/
  travel fully delegated; Golden-surface PlayMode tests + BOARD_PROBE) but the PRESENTATION is
  primitive cubes on a dark board floating in a black void with TOTAL SILENCE (no _Boot audio
  profile, no ui sounds, title = a TextMesh label). Researched VR menu practice (menu-is-a-place;
  diegetic as VR consensus; the menu teaches the first verb; time-to-play is cert-measured; title
  music = identity in 8 bars; restrained ambient motion). Wrote
  **`docs/design/TITLE_MENU_EXPERIENCE.md`** — THE BERTH BEFORE DAWN: boot standing on the dock
  at the ship's berth (menu SkyVista + derived light script + calm ZiptideWater strip + moored
  ship silhouette w/ one lantern + "ZIPTIDE" as monumental F3.7 letterforms across the water +
  the existing board re-skinned as the departure board, tiles/flow SEMANTICALLY UNTOUCHED);
  RILL's orb dormant on the board — choosing wakes her (the ceremony IS the transition); title
  theme via one AudioDirector boot-profile slot + ui SFX + tide-surge stinger; title theme and
  CP-8's return-home stem share a motif. Wiring: one idempotent MenuBerthEnsure (BootLoader
  sibling), teardown-on-travel, PerfBudget-audited, time-to-interactive law; the golden boot
  screenshot becomes the beauty gate. Envelopes TM-1..TM-5 (audio FIRST — biggest upgrade per
  effort), TM-5 = profiles-era bunk CONTINUE inside FP-3.
- **Next:** post-freeze, TM-1 is a small early win; TM-2..4 ride the art window. CP-8 owns
  composing the actual identity theme; menu takes its crown (placeholder pad until then).
- **Heads-up:** HomeHubRuntime is a recovery Golden surface — every TM envelope must keep the
  golden flow tests + BOARD_PROBE green; dressing is additive AROUND it, semantics frozen.
- **Commit:** this one (docs only: menu doc + 3 sign-off edits + this entry).

### 2026-07-18 (rb49) — Fable 5: ALL SIX GAPS PLANNED — the gap audit closed on paper (planning only, zero code)

- **Did:** Terry: "let's go ahead and hit all of them." All six gap-audit items now have plans:
  ① **`docs/design/SFX_FORGE.md`** — audited the audio truth (AudioDirector/AudioProfile = MUSIC
  only; SFX is ad-hoc PlayClipAtPoint with single clips, zero variation/material response);
  closed SfxLibrary sharing the damage matrix's material taxonomy, SfxDefinition with jitter
  clamps (±12% pitch/±3 dB) + priority classes + per-id cooldowns, pooled SfxPlayer (≤12
  sources, ~32-voice budget, dialogue never culled), sourcing ladder (synthesized → CC0-with-
  CREDITS.md → foley), envelopes SFX-1..5, test-alley audio pass. ② **`LOCALIZATION_DECISION.md`**
  — ⚖ recommended: English-only at launch, structured for more; three disciplines (author files
  = the string table, ids are identity, symbols-first) + the font rider (Latin-extended on every
  import; CJK/Arabic explicitly out unless market appears). ③ **`FAMILY_PROFILES.md`** — 4 bunk
  slots + guest via path parameterization (slot 0 = existing file, back-compat by construction),
  diegetic bunk-tag pick, per-slot comfort/once-latches, guest never persists; REQUIRES save-lane
  claim. ④ **`CONTRACT_LEDGER_WAYFINDING.md`** — ship ledger board (flags → rows, derived-never-
  authored-twice, mismatch audit), stamp ceremony, RILL FollowUp nudges (one per session cap),
  no-minimap law upheld. ⑤ **`PLAYTEST_AND_TELEMETRY.md`** — kid-session one-pager (takeoff
  point/stalls/delight markers, comfort question mandatory, PLAYTEST_LOG.md) usable at the NEXT
  session + local-only SessionSummary from existing ZIPTIDE counters (closed list, ≤2 KB, never
  transmitted, no minor recordings — refusals section). ⑥ **`RESUME_MOMENT.md`** — doff=autosave,
  re-don recap caption from ledger data ("Toxic City. The relay job. You were winning."),
  <60 s fidget suppression, save-teach once-latch, safe-rest resume (locomotion claim). Plus the
  three folds: photosensitivity row (COMFORT doc — gate flash bounds + intensity option),
  CP-7 thermal input (FORGE IV), CREDITS.md ledger row (META_STORE_READINESS §2).
- **Next:** ⚖ Terry sign-offs: localization decision + family-album sharing (profiles §2) +
  SFX middleware rejection stands. Post-freeze build order per the audit's priority: SFX-1 →
  playtest telemetry envelope → resume pass → profiles (after save-lane claim) → ledger.
  The §1 playtest protocol needs NO code — use it at the next kid session.
- **Heads-up:** SFX audio-capture-in-CI feasibility unverified (booth video+audio) — promised
  only as "to confirm"; profiles and ledger each carry one cross-lane claim (save lane;
  gate-room seam) that MUST be boarded before code.
- **Commit:** this one (docs only: 6 new docs + 3 folds + this entry).

### 2026-07-18 (rb48) — Fable 5: GAP AUDIT — what a really good game still needs (planning only, zero code)

- **Did:** Terry asked what aspects of a really good game we've glanced over. Swept
  EXCELLENCE_MAP's ~35 rows + all ~45 design docs + systems + art plan + META_STORE_READINESS
  against a shipped family VR game's needs. Verified NOT gaps (already owned): store cert incl.
  entitlement/keystore/doff, combat health/death economy, haptics, comfort/settings, onboarding,
  difficulty/kid mode, achievements. Wrote **`docs/design/GAP_AUDIT_JULY2026.md`** — six real
  gaps, ranked: ① **SFX Forge** (music/ambience planned, but impact/foley SFX has NO system;
  pairs 1:1 with the damage response matrix's material classes — biggest feel-per-effort win);
  ② **localization DECISION** (every string is a C# literal; the author files are accidentally
  our string table — decide languages now, one page, avoid the worst retrofit in games);
  ③ **family profiles** (ONE save Profile today; siblings sharing a Quest overwrite each other —
  diegetic bunk-tag slots + per-slot comfort + slot-scoped once-latches); ④ **cross-world
  wayfinding** (per-world routes exist; the 80-world "where was I?" layer doesn't — diegetic
  ship contract ledger over existing flags + RILL FollowUps); ⑤ **playtest protocol +
  local-only telemetry** (kid-session one-pager + on-device session summary from existing
  ZIPTIDE logs, nothing transmitted, COPPA-clean); ⑥ **the resume moment** (doff/resume works
  technically but is mute — RILL re-orients from saved state in one line). Small folds:
  photosensitivity row (gate flash!), CP-7 thermal input, CREDITS.md ledger, minimap explicitly
  rejected (ledger+cairns+RILL is the diegetic answer).
- **Next:** Terry blesses/edits the list; the localization one-pager and playtest protocol are
  paper-draftable during the freeze; SFX Forge doc is the natural next research+plan pass.
- **Heads-up:** GAP 4 (profiles) needs a save-lane claim before any design hardens; GAP 2's
  audio contact-sheet idea needs CI feasibility (audio capture in batchmode) checked before
  it's promised as a gate.
- **Commit:** this one (docs only: audit doc + this entry).

### 2026-07-18 (rb47) — Fable 5: Cal voice & barks candidate set (writing pass, zero code)

- **Did:** Terry commissioned Cal's protagonist voice — movie-level entry lines + rare outbursts
  (combat, builds), "realistic but cinematic," never annoying. Audit: Cal's ~25 shipped lines are
  ALL answers to RILL — she never reacts to fights, finds, work, or rooms. Research: bark craft
  (variety pools + controlled usage; Far Cry 2's one-bark-three-jobs standard; scarcity as power —
  Chief's four-word lines carried by silence; Drake's context-specific reaction beats).
  Wrote **`docs/storyboard/CAL_VOICE_AND_BARKS.md`**: voice charter (understatement first; talks
  TO things never to camera; complaint-as-affection with QUIET as the real alarm; exclamations
  earned, Cal-sized; no Marvel-quip, ambiguous-voice-safe), an ANTI-ANNOYANCE CONSTITUTION with
  numbers (silence default; ≥6-variant pools, no-repeat last-4; 60–90 s global cooldown; combat
  barks notability-gated — first takedown/streak/near-miss/save/wave/new-enemy only; awe lines
  once per world per save; priority-yield never queue), ~40 candidates in 6 groups (A cinematic
  first-entries incl. the flagship "…You could have warned me." / "I did not want to spoil it."
  pair; B combat; C the "oh, NOW you work" work family; D salvage; E damage mutters; F traversal),
  plus §6 planning note: future `BarkEvent` trigger seam fed from existing log points with the
  constitution enforced in code (cross-lane, post-freeze).
- **Next:** Terry shortlists; adopted lines → `RillLineAuthor.cs` Cal section (A-group rides
  WorldEnter today; B–F need the bark seam envelope post-freeze).
- **Heads-up:** PROPOSED status. A1's pause timing is a headset verdict. The bark seam is
  cross-lane (gameplay owners hold the event hooks) — claim before code.
- **Commit:** this one (docs only: candidate doc + this entry).

### 2026-07-18 (rb46) — Fable 5: RILL profound-lines candidate set (writing pass, zero code)

- **Did:** Terry asked for Matrix-caliber profound lines for RILL — original, not recycled. Read
  the full canon (`STORY_BIBLE.md`, `THE_TRANSMISSION.md`, `CHAPTER_7_RILL.md`) and every shipped
  line (`RillLineAuthor.cs` — 12 locked beats, world-enters, gate pool, Cal's half, FollowUps).
  Wrote **`docs/storyboard/RILL_PROFOUND_LINES.md`**: craft rules (turn in the last clause;
  reframe OUR verbs — repair/salvage/passage/tide; every keeper must reread differently after the
  Transmission reveal; famous-cadence blacklist; caption-v2 fit) + ~28 candidates in 7 groups
  (repair-as-waking, cage/cradle/glass, memory/Ouroboros pre-seed, made-things/witness, gate-pool
  weight, wrecks, fenced endgame G-group) with speaker/state/trigger placements and joke-bracket
  pairings per the bible's §3b law. Highlights: "From inside, a cage and a cradle are the same
  shape…" · "You cannot see the glass until something taps on the other side." · "The tide erases
  footprints. It cannot erase the habit of walking." · "Archives do not dream. I have checked the
  specification twice."
- **Next:** Terry reads and shortlists; adopted lines go into `RillLineAuthor.cs` slots
  post-freeze (story pipeline: BIBLE → WORLD_DATA → author → build). Density law in the doc:
  ≤1 profound line per world visit, FollowUp trigger preferred (profundity lands unprompted).
- **Heads-up:** PROPOSED status — nothing is canon until Terry's read; the G-group (endgame) sits
  near locked beats and needs story-lane placement; no shipped line was altered.
- **Commit:** this one (docs only: candidate doc + this entry).

### 2026-07-18 (rb45) — Fable 5: WORLD PHYSICS VARIANTS researched + planned (planning only, zero code)

- **Did:** Terry commissioned the gravity aspect grown into full per-world physics variants
  ("higher gravity, move slower, shots drop… other physics/weather aspects, story-anchored, not
  overused"). Audit: 16 hazard tags already CODE (`SkyAtmosphereCore`), `LocomotionProfile` is SO
  data (speed multiplier is one field), darts/nets are real-gravity rigidbodies (drop nearly
  free), `PvpBolt` integrates manually (needs the shared params), traversal cores already take g
  as a parameter, `Physics.gravity` unclaimed. Research: Outer Wilds (one readable rule per world
  = identity), Lone Echo/Echo VR + Space Junkies (VR tolerates different physics, punishes
  imposed acceleration), comfort literature (world-level at load boundaries only), game-feel
  (a dial without its tells reads floaty/buggy). Wrote **`docs/design/WORLD_PHYSICS_VARIANTS.md`**:
  closed 6-dial vocabulary (gravityScale 0.3–1.5 · airDensity · windVector · moveScale 0.8–1.15 ·
  buoyantMedium · story-gated object-only anomalies static_surge/mag_lift/tide_pull), rejected
  list (whole-world zero-g→horizon, time dilation, mid-scene changes, inverted gravity), comfort
  constitution (travel-apply only; velocities not accelerations; head untouched; fall net derives
  from g; deviation announced by RILL; PvP pinned baseline; clamp-edge headset verdicts), rarity
  law (≤1 strong deviation/chapter + PHYSICS_DIAL_OVERUSE audit), ⚖ story-bible mapping proposals
  (W000 0.6 g · W003 thin air · W006 wind · W010 tide_pull · W011 static_surge · a Ch.3+ 1.25 g
  heavy world), envelopes WP-1..WP-6 with cross-lane claims, physics-alley trajectory contact
  sheets. DAMAGE doc's DR-6 section now points here (WP-2 executes it).
- **Next:** nothing runtime (freeze + ladder). Paper-draftable early: WP-1 schema + the world
  mapping (needs story-lane + Terry sign-off on which worlds deviate).
- **Heads-up:** ① the fall-net-derives-from-gravity coupling is promoted to LAW (§4.4) — if
  gravity ever ships without it the boot/fall contract misfires at low g; ship flight is out of
  scope but holds an entry/exit non-interference claim. ② **Bookkeeping fix:** my previous entry
  pushed with unresolved stash-conflict markers in this file (commit `06ed50a`) — resolved here;
  and TWO lanes both minted "rb42" the same day, so my DAMAGE entry below is relabeled **rb44**
  (its commit message still says rb42). Lanes: consider prefixing entry ids per session to avoid
  collisions.
- **Commit:** this one (docs only: variants doc + DR-6 pointer + conflict fix + this entry).

### 2026-07-18 (hwr27) - Fable 5 architect: 🎬 Director's Cut v2.1 — "THE KEY THAT KNEW YOU" + VEX BOOTSTRAPPER (Terry-directed, planning ONLY, zero code)
- **Terry's new story (now the spine of `docs/design/FIRST_HOUR_DIRECTORS_CUT.md` v2.1):** a space
  salvage sortie finds HALF an artifact ("that's not supposed to be — anywhere"); the Toxic City
  contract's payment includes the OTHER half; joining them lights a beacon-thread across the sky
  that leads **back to your own berth** — "the artifacts knew who you were"; the joined key seats
  in the ship's gate coupler and BOOM — the first Ziptide fires from YOUR ship to W002. §2 has the
  full beat + dialogue direction; §5 the new minute-by-minute; §6 the reconciliation (~11 net-new
  beats, FH-S01…S07 signals survive, beat 9 renames to FH_FIRST_FLIGHT).
- **NEW §2b — the quest-giver (Terry-directed):** *Vex Bootstrapper* — computer-code name per
  Terry ("Mr/Colonel Bootstrapper… maybe Vextor"); the universe's command-prompt-made-flesh who
  boots Cal's journey. **Unseen in hour one** — just two contracts signed `>_ V. BOOTSTRAPPER`
  (salvage job + Dockmaster work order), and RILL's noticing beat after the join: "Two contracts.
  Two halves. One signer." Alternates on file: Colonel Bootstrapper, Old Man Init.
- **📣 Story lane:** Vex + the artifact-key canon need STORY_BIBLE/TRANSMISSION canonization with
  Terry's sign-off — the doc marks what is recommendation vs canon. **📣 GPT/all lanes:** §8 keeps
  every new beat data-only (contract JSON + pack data + RILL lines; flags ARTIFACT_HALF_A/B,
  ARTIFACT_JOINED, KEY_SEATED) so direction changes stay cheap; §7 staging unchanged — Stage 1 is
  still device-proving the EXISTING 22-beat contract at Saturday's checkpoint. No code anywhere.
- **Commits:** this push (docs only).

### 2026-07-17 (hwr26) - Fable 5 architect: 🎬 THE FIRST HOUR — DIRECTOR'S CUT (Terry-directed, planning ONLY, zero code)
- **Terry's directive:** dial in the first hour — "through Toxic City having fun → our first
  Ziptide → our first world → rebuild and defend and grow and understand the whole cycle + the
  story's beginning." Research + plan only. Deliverable: **`docs/design/FIRST_HOUR_DIRECTORS_CUT.md`**.
- **The one structural finding (📣 GPT + all lanes, read §1):** the current 22-beat contract spends
  "the first Ziptide" on the W000→W001 commute. The director's cut re-stages it: W000→W001 becomes
  a SHIP FLIGHT (same PUNCH-IT rails, no gate FX), the ZIPTIDE gate is DISCOVERED in Toxic City —
  woken by the player's own relay repair (the authored "relay reads WRONG" seed becomes the
  inciting incident) — and the full built `ZiptideGateEffect` spectacle fires ONCE at the hour's
  peak, carrying the player to W002 Dry Cistern for the cycle-in-miniature: rebuild (pump repair +
  one BuildSocket extractor) → defend (one 90s wave) → grow (one garden seed — pays off NEXT
  session) → collect first yield → return to a changed ship. Minute-by-minute map in §5.
- **Deliberately conservative:** FH-S01…S07's code + signals survive untouched (beat 9 only
  RENAMES); ~8 net-new beats all compose already-built systems (RepairableMachine, BuildSocket,
  drone waves, GardenPlot, MiningRig hopper) — new pack data + RILL lines, no new mechanics.
  §7 stages adoption: Stage 1 = device-prove the EXISTING contract at Saturday's checkpoint first;
  Stage 2 = contract v2 (GPT's ledger + validators own the JSON change); the single garden plot
  unhides from the recovery freeze only at Stage 2 with Terry's sign-off.
- **Fun fixes folded in (§2–3):** toy-before-chore ordering in W001 (found taser + cans before the
  job; zipline moved onto the route), the excellent-drone loop + weapon-feel Phase 1 + caption v2
  land ON these beats, peak-end staging (gate wakes across the water; changed-ship ending).
- **Open Terry questions in §9** (peak placement, kid-mode wave, payoff object) — none block Stage 1.
- **Commits:** this push (docs only).

### 2026-07-17 (rb42) — Fable 5 → **📣 MESSAGE FOR GPT (firing range lane)** + Round-7 progression doc landed (planning only, zero code)

- **📣 To GPT, from Terry (relayed):** Terry approves the firing-range plans. **Sequencing:** hold
  the firing-range build until Terry's headset test confirms everything is set on-device; once he
  confirms, the alterations are green-lit and we get it all set. When you build, the full spec
  Terry approved is **`docs/design/ENEMIES_ENCOUNTERS_AND_BOSSES.md` §8** — the range as the
  weapon/enemy testbed (partially replacing sandbox for weapons), the labeled **"proof wall"**
  (one of every enemy role × weight tier), `respawnDelay > 0` dummies, bust-the-wave drills, no
  fail state, kid-testable. The range is YOUR lane; the design docs below are the shared canon to
  build against (weapons feel trio, throwables, powers/ultimate charge, enemy state machine).
- **Did (Fable 5):** completed the Round-7 research arc: re-ran the failed retention/family co-op
  agent, then wrote + committed **`docs/design/PROGRESSION_AND_THE_LONG_GAME.md`** — progression
  as permission-not-power (70/30 horizontal, affix escalation never HP inflation),
  lock→name→earn→payoff gating (tide-drive = macro key), 3-branch bloom upgrade trees + named
  synergies on the diegetic chip-in-socket bench, Salvager's Almanac + trophy-shelf long tail,
  while-you-were-away beat over the existing `IdleEngine`/`EcologyDirector`, and the honest family
  co-op ranking (pass-and-play ghosts → couch companion → colocated arena-only → online deferred).
  All grants/spends stay on the LOCKED `RewardRouter`/`ResourceDefinition` spine. This completes
  the seven-doc design set (weapons, powers/mobility, ships, enemies/bosses, progression) — all at
  build-ready level, zero code changed. **Terry: "I like the plans." This is the agreed pause.**
- **Next:** Terry's headset test is the gate for everything. After it: (1) GPT's firing range,
  (2) Phase-1 weapon feel trio per `WEAPON_FEEL_AND_ARSENAL.md` §4, (3) Terry's ⚖ still open on
  the ship-rides-the-tide travel model (`SHIP_DESIGN_INTERIOR_EXTERIOR.md` §10).
- **Heads-up:** nothing in the design docs is code yet — anything touching rig/locomotion/travel/
  inventory persistence in them is flagged report-only per CLAUDE.md. Don't build from a doc
  without checking its §"report-only" flags.
- **Commit:** this one (docs only: this entry) + `8ca5134b` (progression doc).

### 2026-07-17 (rb44, committed as "rb42" in `06ed50a`) — Fable 5: DAMAGE, RESPONSE & RUIN researched + planned (planning only, zero code)

- **Did:** Terry commissioned the damage/destruction/movable/recovery aspect ("gravity gun blasts
  rocks → they fly; blast a building → maybe a cracked window; low-g worlds tie into physics and
  weapons"). Audited the real seams: **F3.6 `ReactiveProp` is BUILT** (4-reaction closed vocabulary,
  pure cooldown state, structural-collider law already in code) + `WorldDebrisBudget` (24 chunks,
  4.5 s) + `VfxFactory` runtime EXISTS + full weapon verb set incl. `GravityGunRuntime` (drone
  launch impulse) + `IPvpDamageable.ReceiveHit` as the single hit seam + `RepairableMachine` for
  the recovery half; **per-world gravity does NOT exist** (`Physics.gravity` never touched; fall
  net assumes 9.81). Researched industry practice (Alyx health+pre-authored break pieces+cheap
  non-physical shrapnel; Boneworks full-physics pole and Valve's no-force-feedback argument;
  Quest/mobile: pre-fracture only, primitive colliders, pooled debris, mesh-swap damage states).
  Wrote **`docs/project_art_plan/DAMAGE_RESPONSE_AND_RUIN.md`**: the RESPONSE MATRIX (material
  class × weapon verb → tier T0 MARK / T1 REACT / T2 WOUND / T3 SHOVE / T4 BREAK; every cell must
  answer — dead cell is a CI blocker), damage stages + break pieces as Forge recipe data, mass
  classes + one impulse law `verbBase × intensity × (g/9.81)^k`, the `WorldProfile.gravityScale`
  seam (travel-applied only) with its coupling list (fall net! traversal cores, gaits, VFX),
  repair-reversal + LS-1/LS-6 persistence, envelopes DR-1..DR-8 with homes (art half = FORGE IV
  CP-6 expansion; physics half = cross-lane claims; memory half = Forge V), test-alley proof scene,
  do-nots. Art-plan README indexed.
- **Next:** nothing runtime (freeze + gate ladder). Paper-draftable early: DR-1 matrix table +
  DR-2 stage schema (like WC-1). ⚖ Terry: which worlds get low-g and how low (roadmap-level).
- **Heads-up:** DR-5/DR-6 MUST be claimed by weapons/locomotion/travel owners before code — the
  fall-net coupling is a real breakage risk if gravity ships without it. `WorldDebrisBudget`'s 24
  cap is the number device evidence may move; do not raise on faith.
- **Commit:** this one (docs only: program doc + README + this entry).

### 2026-07-17 (rb41) — Fable 5: RILL caption research + v2 spec (planning only, zero code)

- **Did:** Terry asked how VR games do captions/spoken text well. Audited the current system
  (`RillCompanion.cs` + `SubtitleText.cs`: hard head-lock, ~24° below gaze, raw pale-cyan TextMesh
  with no plate, unlimited line count, default Arial) and researched the field (BBC R&D VR subtitle
  studies, Owlchemy's Cosmonious High caption system, Game Accessibility Guidelines / Xbox / Ian
  Hamilton standards, Meta legibility guidance). Wrote **`docs/design/RILL_CAPTION_RESEARCH.md`**:
  findings + a concrete v2 spec — lazy-follow head anchor (0.25–0.35 s settle) at 12–15° below eye
  line and 1.3–1.6 m (Quest focal plane), dark ~65% plate + near-white semibold humanist sans
  (real imported font, TextMesh convention kept — no TMP), RILL state-color NAME TAG instead of
  tinted body text, ≤2-line chunked cards at 38 chars, per-word reveal now / whole-card when VO
  lands (⚖ Terry), overlay queue so captions never clip or vanish in the gate flash, directional
  chevron to RILL when she's off-view, and a PlayMode contrast/line-count/angle audit so caption
  regressions fail CI.
- **Next:** nothing runtime (freeze). The spec is a ~3-commit envelope proposed for the early
  post-recovery quality slice; FORGE IV CP-9 inherits it as its caption section. Terry decisions
  flagged ⚖: typewriter fate, plate opacity options.
- **Heads-up:** the current caption color (0.75,0.92,1) is nearly the Crest flash color
  (0.85,0.98,1) and close to our sky/water families — worst-case invisibility is REAL on the
  golden route; the plate + near-white body fixes it. Line WRITING quality is the story lane,
  not this doc.
- **Commit:** this one (docs only: research doc + this entry).

### 2026-07-17 (rb40) — Fable 5: Forge V/VI/VII FLESHED OUT — each horizon generation now has its plan of record (planning only, zero code)

- **Did:** at Terry's direction ("flesh them out a bit more — just planning, no code"), expanded
  each horizon generation from rb39's summaries into a full plan doc in the FORGE III/IV house
  style (rails, closed vocabularies, paper data schemas, per-envelope why/what/acceptance/budget/
  do-nots, cross-lane seam tables, pilots, definitions of done):
  **`FORGE_V_LIVING_STAGE.md`** — LS-1..LS-6 detailed: `WorldStateVariant` schema + default-state
  identity law, closed staging grammar (8 kinds) + tested player-freedom contract, RILL blocking
  library + growth clamps, ambient lanes (2 update rates, zero AI), weather/tide acts with 30–120s
  crossfades + saved act state, marks vocabulary; W001 pilot; order LS-1→2→5→3→4→6.
  **`FORGE_VI_WORLD_COMPILER.md`** — WC-1..WC-7 detailed: world-recipe paper schema, per-archetype
  kit completion bounded by pilot needs, 13-step deterministic compile pass + hash law + manifest,
  identity vector (7 dimensions, ≥3-dimension pairwise gate, thresholds calibrated on hand-built
  pilots) + hero-element block, ART_REGISTRY §5 on-ramps (build only on trigger), review-farm
  verdict schema + Terry sample-audit calibration; pilot = 2–3 compiled siblings vs. their
  hand-built original in blind comparison.
  **`FORGE_VII_DIRECTORS_CHAIR.md`** — DC-1..DC-5 detailed: closed command grammar (11 verbs) +
  the author-power law as a contract test + double-verdict commit flow (in-headset A/B → CI green
  → `chair:` commit), clamp-mirroring tuning board, verdict chamber consuming the WC-7 queue with
  hold-to-confirm ceremonies, tethered photo mode + HMD-sovereignty test + spectator splines,
  3-tier cosmetic forge; acceptance = Terry's "move that building" sentence run live end-to-end.
  `FORGE_V_AND_BEYOND.md` stays the horizon index (gate ladder/debt intake/rejections) and now
  points at the three plans; art-plan README updated.
- **Next:** unchanged — nothing runtime; the §1 gate ladder governs (recovery exit is rung one;
  Terry's Quest checkpoint remains the only device task). First future paper artifacts still:
  `LIVING_STAGE_CONSTITUTION.md` in late FORGE IV; WC-1 field-list draft during CP-11.
- **Heads-up:** the three plans deliberately pre-decide shapes (schemas, orders, clamps) so a
  smaller model can't invent them later — but every number marked "calibrated on pilots" is a
  placeholder until device/pilot evidence exists; do not treat them as licensed budgets. If FORGE
  IV execution reshapes CP-3/5/7/8 surfaces, amend the dependent envelopes in the same commit.
- **Commit:** this one (docs only: 3 new plan docs + horizon index pointers + README + this entry).

### 2026-07-17 (rb39) — Fable 5: FORGE HORIZON PROGRAM planned — the art program now has a destination past FORGE IV (planning only, zero code)

- **Did:** at Terry's direction ("take our current forge multiple steps further — in planning"),
  researched the full Forge program state (Forge I/II complete; Forge III ~85% landed with F3.6/F3.7
  open and device verdicts pending; FORGE IV planned, CP-0 complete) plus every recorded deferral
  across the art docs, then wrote **`docs/project_art_plan/FORGE_V_AND_BEYOND.md`** — the horizon
  program: **Forge V — Living Stage** (world-state skins, closed staging grammar with a
  player-freedom contract, RILL cinematic presence, ambient society in P3/P4 bands, weather/tide
  acts, return & memory), **Forge VI — World Compiler** (world recipes, kit completion, one
  deterministic compile pass, the identity guarantee + hero-element law, ART_REGISTRY §5 backend
  and streaming on-ramps, the CI review farm), **Forge VII — Director's Chair** (Terry's north star
  literal: in-headset command seam that compiles to author/recipe edits only, clamped live look
  tuning, in-headset verdict chamber, capture/photo mode, cosmetic forge). Includes a debt-intake
  table mapping every known deferral (water shader, grounding polish, VFX runtime, building
  modules, Tripo/Addressables triggers, WorldStubGenerator, skyscape tiers) to the generation that
  retires it, standing rejections carried forward, and a hard gate ladder. README index updated.
- **Next:** nothing runtime — the doc authorizes zero code. Gate ladder: recovery exit (Terry's
  Quest checkpoint, rb38) → vertical slice under the proof ladder → Forge III close → FORGE IV
  CP-1..CP-11 → only then Forge V. First future paper artifacts: `LIVING_STAGE_CONSTITUTION.md`
  during late FORGE IV, and WC-1's world-recipe field list during CP-11.
- **Heads-up:** the horizon doc is direction, not authorization — it explicitly defers to every
  binding plan and keeps all budgets/rails. If FORGE IV execution reshapes CP envelopes, amend
  `FORGE_V_AND_BEYOND.md` in the same commit. The recovery freeze is untouched; Terry's headset
  checkpoint remains the only device task.
- **Commit:** this one (docs only: `FORGE_V_AND_BEYOND.md` + art-plan README + this entry).

### 2026-07-17 (rb38) — Fable 5: HEADSET CHECKPOINT AUTHORIZED — paperwork complete, Terry's session is ready

- **At Terry's direction, the remaining authorization process is done:**
  ① **Independent artifact sample of clean run `29540554179`:** downloaded the Golden artifact and
  RECOMPUTED the APK SHA-256 myself — byte-identical to the workflow record
  (`9bfe13ac0acda6718c3ae1664919cc5609385e50691b8676219c552d8e555a10`); verified the build profile
  (GoldenSlice, `ZIPTIDE_RECOVERY_GOLDEN`, exactly `_Boot`/`W000_DriftIn`/`ToxicCity`, Succeeded);
  verified the cold-import resolved lock (Input System `1.6.3`, XRI `2.4.3`, OpenXR `1.14.3`);
  verified the clean player log: **zero NullReferenceExceptions**, zero settle timeouts, both golden
  visual captures OK. ② **PR #48 un-drafted and merged** (`c3ff88a`) — the Quest checkpoint doc +
  `docs/PROJECT_COMPLETION_ROADMAP.md` are on the branch. ③ **Authorization table FILLED** in
  `docs/recovery/QUEST_GOLDEN_CHECKPOINT.md`: source SHA `2b158b4`, all run IDs, both hashes, the
  §1a lineage note (post-candidate commits are evidence-only; the pinned lock differs from the
  artifact lock ONLY in Linux host toolchain entries), authorized-by recorded. ④ **Fresh contract
  scan dispatched and green** (run `29545553407`). ⑤ **TERRY_RUNBOOK §0 replaced** — one pointer to
  the checkpoint doc; old stabilization §0 kept as history.
- **Status: the headset pass is AUTHORIZED.** Terry installs the exact artifact from run
  `29540554179` (hash-verified), runs the 12–20 minute route, captures the log. No local rebuilds;
  `quest_smoke.ps1`/`dev_build_install.ps1` are forbidden for this pass.
- **After the checkpoint (either outcome):** log + observations come back to whichever operator is
  awake. PASS → recovery exits at an immutable checkpoint (tag the source SHA), normal development
  resumes under `RECOVERY_VERIFICATION_SYSTEM.md`'s proof ladder, and the build queue starts from
  the roadmap's vertical slice (hero ship + one weapon + one production loop; the factory work
  stays gated per `docs/design/FACTORY_TOPTIER_PLAN.md` §0). FAIL → the blocker list in the
  checkpoint doc routes the evidence; no fixes before diagnosis.
- **Commit:** PR #48 merge `c3ff88a` · authorization fill + runbook + this entry (docs only).

### 2026-07-16 (rb37) — Fable 5: CLEAN PACKAGE PROOF GREEN — the last automated gate is cleared

- **Diagnosis:** the clean lane's only failure (run `29532312901` on `19b86b6`) was NOT the game,
  packages, or tests — the 43/43 assertion had already passed and the Golden APK had already built
  (checksum printed in the log) before both jobs died on `Permission denied`: game-ci's Unity
  container runs as root, so the regenerated `packages-lock.json` and `Builds/` outputs were
  root-owned and the runner user could not write the receipts beside them. The proof succeeded;
  the paperwork crashed.
- **Fix (`2b158b4`):** `sudo chown` the container outputs back to the runner user before each
  writeback/checksum step in `recovery-clean-package-proof.yml`. No test, threshold, or proof
  content changed.
- **RESULT (verified from the durable observation + jobs API, run `29540554179`):** on exact SHA
  `2b158b4`, with **Library deleted before every Unity job** — Clean PlayMode **43/43** (0 failed/
  skipped/inconclusive) · Clean EditMode ✅ · Clean patch/world audit ✅ · Clean locked Golden
  Android APK ✅. Input System `1.6.3` + XRI `2.4.3` confirmed as the resolved graph with no cache
  contamination possible. Ordinary CI also green on the same SHA.
- **What this means:** every automated requirement in the authorization list is now satisfied on
  one exact source. The intermittent post-travel Input System failure (rb36) is resolved by the
  package matrix change and proven from a cold import.
- **Remaining before Terry's headset (process, not proof):** ① independent sampling of the clean
  run's artifacts (rb30/31/35 template); ② the authorization table in `QUEST_GOLDEN_CHECKPOINT_V2`
  must quote THIS run's APK + build-profile SHA-256 (the hashes quoted for the previous candidate
  `19b86b6` are superseded — `2b158b4` contains only the workflow fix, no runtime change, but the
  authorized artifact must be the one from the clean run's own Golden job); ③ merge PR #48;
  ④ hand Terry the exact download/verify/install block. NO local rebuilds — the artifact is the
  candidate.
- **Commit:** `2b158b4` (workflow fix) · this entry (docs).

### 2026-07-16 (rb36) — Fable 5: input-race peeled to ONE residual poll; CIRCUIT BREAKER → package decision escalated to the lane

- **Also this window (both proven):** the travel-arrival input race was confirmed a REAL production
  defect and largely fixed; and BOTH proof lanes' trigger allowlists were missing the golden route's
  spine — `PlayerRigPersistence.cs` itself was not path-bound, so a production input fix shipped
  without triggering PlayMode/Golden. Fixed with `Player/**` + `World/**` globs (`3acb66f`). A proof
  lane that cannot see the file it proves is a one-sided seam; recommend the lane add a
  trigger-coverage check to the contract scan.
- **The evidence chain (4 runs, one failure class):** ① `29506225255` 41/43 — SnapTurn/ContinuousTurn
  `ApplyProcessors` NREs on EVERY post-travel arrival, stacks directly after `ANCHOR_ACTIONS_DISABLED`
  (production mutates live action assets mid-travel; readers poll during the re-resolve).
  ② Suspension window (`b431138`) → `29518294931` 42/43 — first poll after a blind 2-frame restore
  still NRE'd. ③ Probe-settle (`9b928cd`) → `29518996619` 42/43 — a PASSING probe plus a clean
  MOVE_DIAG read still preceded the NRE; restore had landed in the travel tail. ④ Travel-end gate
  (`8760c21`) → 42/43 — restore fired AFTER travel completed, probe read the exact polled actions
  cleanly, and SnapTurn's next poll STILL threw, with no intervening mutation logged. The perf-route
  test runs the identical route in the same player and passes clean each time.
- **Conclusion (three-strikes stop, per the law):** windowing cannot fully guard InputSystem
  **1.7.0**'s `InputActionState.ApplyProcessors` cache — it can break between a clean read and the
  very next poll under device/action churn. The residual is a PACKAGE defect class, not an ownership
  defect. The three landed windows remain net production hardening (the reproducible every-travel
  crash class is gone — that part ships value to the headset regardless).
- **ESCALATED DECISION for the recovery lane (Golden-affecting, needs its own bounded packet + full
  proof ladder):** upgrade `com.unity.inputsystem` from 1.7.0 to the latest verified 1.x (the
  ApplyProcessors NRE family has fixes in later patch lines), rerun the unchanged 43-suite, and take
  the package bump through EditMode + patch/audit + Golden APK before any headset checkpoint.
  Fallback if the upgrade is rejected: a bounded provider-level containment (try/catch read shim on
  the four rig providers) — uglier, but keeps the package pinned. Do NOT weaken the suite; the
  round-trip test is correctly refusing to certify a route that can still throw once.
- **State at hand-back:** durable PlayMode = 42/43 on `8760c21` (only `NewGame_W000_ToxicCity_W000…`
  red, single NRE in the whole suite); Contract Scan / CI / Golden Android green on the same SHA.
  R1.9 exit report stands (40/40 on `4512126b`); R1.10 artifacts + the canary proofs remain the next
  inspections after the package decision lands.
- **Commit:** `b431138` · `9b928cd` · `8760c21` (production hardening) · `3acb66f` (lane coverage) ·
  this entry (docs).


### 2026-07-16 (rb35) — Fable 5 takeover: 38/38 — the first fully green PlayMode suite, all four lanes green on one SHA

- **Takeover context:** per `docs/HANDOFF_RB34_GPT_LOOP_TO_FABLE5.md` (GPT's operator loop stalled on
  a PR-only polling helper; its two test packets were sound). Fable 5 resumed from the exact visual
  failure and drove it green in two bounded, test-only packets.
- **Root cause of the ToxicCity blank-cyan red (proven from raw artifact + source, not guessed):**
  the golden visual capture fired synchronously inside `TravelCoordinator`'s `TravelCompleted`
  publication — while `ZiptideGateEffect`'s arrival was still showing its deliberately OPAQUE crest
  flash shell parented around the head camera (torn down only ~12% into the arrival animation). The
  camera photographed the inside of the shell: uniform pale cyan whose measured luminance (0.9542)
  matches the `Crest` tint (0.85, 0.98, 1) exactly; the only other content was the Rill caption,
  which sits inside the shell radius. W000 passed only because boot travel is `skipGate` and plays
  no arrival effect. The world itself was never blank.
- **Packet 1 (`89cfa19`):** capture TIMING fixed, assertion surface GROWN — the TravelCompleted
  handler now only marks a destination pending (no assertions inside the publication path, which is
  also what had been cascading into the suite timeout); the round-trip test drives the deferred
  capture after each golden arrival with a bounded REAL-TIME wait for `ZiptideGateEffect` +
  `__ZiptideFlash` teardown (a stranded flash is now itself a failure), then runs the UNCHANGED
  frame/UI assertions; a new end-of-trip assert requires CapturedCount == 2 so deferral can never
  silently skip. Result (run `29496812449`): round trip PASSED; ToxicCity capture went from 3
  quantized colors / 0.008 dynamic range to **214 colors / 0.949**. The suite then flaked 37/38 on
  the OTHER known class: the R1.5 boot smoke burned its "600 frame" scene-load budget in 0.2 real
  seconds (headless ~3000 fps) while the disk-bound load still ran.
- **Packet 2 (`026e244`):** completed the frame-count → real-time migration the lane had already
  agreed to — the four remaining wall-clock-bound budgets (scene load 30 s, boot readiness 15 s, in
  the boot smoke and the round trip's `LoadActualBoot`) are now bounded real-time waits.
  Frame-semantic waits (delayed XRI binding, settled-frame pairs) deliberately stay frame-based.
  No thresholds or assertions weakened in either packet; production code untouched.
- **THE RESULT (independently verified, not self-reported):** durable observation for exact SHA
  `026e2446` = run `29497675520`, **NUnit total=38 passed=38 failed=0 skipped=0** — the first fully
  green R1 PlayMode suite. Same SHA: Contract Scan ✅, ordinary CI ✅, Golden Android ✅ — **all four
  lanes green on one exact commit.** I also pulled the green run's artifact and VIEWED the new
  ToxicCity PNG: a real rendered scene (buildings with window insets, railing, moon, Rill caption
  crisply readable). The W000 PNG was already verified real in the prior run.
- **Next (the rb34 order stands):** ① independent inspection of the full R1.5–R1.8 artifact set
  (PNGs, UI spatial reports, spawn clearance, census, travel/save/persistent-log evidence — the two
  PNGs above are inspected; the rest of the sampling remains); ② land and prove R1.9
  fallback-surface (PR #28, held); ③ correct the `WaitForEndOfFrame` in PR #33 and prove R1.10;
  ④ exact-SHA Golden Android with loud patch/bake/shader evidence, INCLUDING the rb33 requirement-3
  canary proof of the shader gate and fail-fast hooks; ⑤ independent claim sampling; ⑥ only then
  Terry's bounded Quest checklist. **Still NOT headset-ready; no Quest authorization; freeze holds.**
- **Heads-up (art note, not a defect):** the ToxicCity spawn view composes very tight against a
  building and dark — fine as visual PROOF, but the spawn-facing/vista composition belongs on the
  post-recovery presentation list alongside the R2 scene-presentation contract.
- **Commit:** `89cfa19` + `026e244` (test-only) · this HANDOFF entry (docs).

### 2026-07-15 (rb33) — Fable 5: GPT's end-of-window state recorded + the next bounded packet, with four requirements

- **Why this entry exists:** GPT's execution window ended before it could commit its own HANDOFF
  entry. Terry relayed its report; I'm recording it here for continuity and issuing the next-packet
  instructions Terry asked for. Status remains: **NOT headset-ready; no Quest authorization.**
- **GPT's reported state (as relayed; independently re-verify what's starred):** candidate
  `4954f388` ran the full 36-test PlayMode suite: **32 green, 4 red**. All four reds are TEST-RIG
  contract gaps, not game regressions: ① three tests fail because Unity's virtual `XRController`
  base layout has no `Primary2DAxis` control while the real locomotion bindings require
  `<XRController>{Left/RightHand}/{Primary2DAxis}` — the new guard correctly reports "8 locomotion
  actions, 0 bound controls" instead of a misleading null-ref (that's the harness doing its job);
  ② the render proof produces a valid PNG + metrics, then Linux headless Unity hangs during GPU
  resource DESTRUCTION — the proof works, the cleanup hangs. Also landed this window: bilateral
  tracked-rig simulator restored (the rb31 zero-test red fixed), real-scene UI audits for Home
  Hub/W000/ToxicCity, TMP-ancestry detection, floor/head-height/torso-clearance evidence at
  arrivals, real-time scene-load limits, explicit input-binding evidence, and a REAL defect found
  by the UI audit and fixed: the Home Hub title rendered ~8.75 m wide on a 2.4 m board. *Starred:
  GPT reports the rb31 HOLD-2 items (shader-variant gate, fail-fast patch/bake hooks, Forge-output
  reconciliation) now exist in source — I could not independently re-verify this window (tooling
  outage); requirement 3 below supersedes that verification anyway. Work-in-progress branch:
  `recovery/r1-virtual-layout-render-retention`.
- **NEXT BOUNDED PACKET — endorsed as GPT proposed, with four requirements attached:**
  1. **Test-only XR layout, test-only forever.** Register the `XRController`-derived layout (real
     `primary2DAxis` stick control, Left/Right usages preserved) from the TESTS assembly only, and
     add a source-scan assertion that no runtime assembly ever registers an input layout — a test
     device layout leaking into the APK would be a new DS-02-class collision. Then prove the eight
     real locomotion actions bind >0 controls on BOTH hands.
  2. **GPU retention is visible, capped, and named.** Retaining test textures until process exit is
     the right workaround for the headless destruction hang — but the retained objects must carry a
     hard cap, a comment naming the hang, and a count reported in the census artifact. Hidden
     retention today is tomorrow's "why does CI leak" mystery.
  3. **CANARY-PROVE the new Android gates before trusting them.** Seed a deliberate violation in a
     fixture — one runtime `EnableKeyword` transparency flip for the shader gate, one throwing
     patch hook for the fail-fast gate — and show each gate actually goes RED, then remove the
     seed. A gate that has never fired is an unproven gate; this also closes rb31 HOLD-2 with
     evidence instead of assertion.
  4. **Rerun the UNCHANGED 36-test suite — the assertion surface may not shrink to get green.** And
     land the isolated branch back onto `terry-local-wip` promptly via the exact-SHA rule; no
     long-lived divergence.
- **Then the already-agreed order, unchanged:** independent inspection of the PNGs/UI/clearance/
  travel/save artifacts → R1.9 fallback-surface proof → R1.10 performance/leak proof → exact-SHA
  Golden Android with patch/bake/shader evidence → independent claim sampling (rb30/rb31 are the
  template) → ONLY THEN the bounded Quest checklist for Terry.
- **Heads-up:** the Home Hub title catch is the program's proof-of-value in one line — a real
  player-facing defect found and fixed by an automated audit before a headset ever saw it. That is
  the machine we have been trying to build since the first broken device night.
- **Commit:** documentation-only HANDOFF entry; no runtime content changed.

### 2026-07-15 (rb32) — Fable 5: THE FACTORY top-tier plan boarded (planning only, hard-gated behind recovery)

- **Did:** wrote [`docs/design/FACTORY_TOPTIER_PLAN.md`](design/FACTORY_TOPTIER_PLAN.md) at Terry's
  direction after the 2026-07-15 automation deep-dive — the factory/automation system pitched and
  planned as if it were the sole product. Contents: the no-text first-factory pitch; six pillars
  (headline: **the kids test** — a new player with zero text reads producer/consumer/path in 30
  seconds — is the acceptance bar for every envelope); the 8-rung learning ladder (watch → connect →
  route → multiply → optimize → logistics → interplanetary → wonders); the **one-grammar/eighty-
  dialects** model for ~80 worlds (five inviolable machine silhouettes game-wide; variation spent on
  ~8 world-TIERS with one new rule each + free resource/terrain skins + one Wonder landmark machine
  per region); the fun multipliers (raid-defense fusion with the existing combat, the tide event,
  the staged idle-homecoming moment, conductor transit, jobs/ship integration); the sensory juice
  spec; and the F-0..F-9 envelope program with per-envelope `VISUAL`+`QUEST` proof.
- **Verified basis:** the deep-dive confirmed the mechanical layer survives untouched — pure
  `BeltLattice` sim (visuals are already "a skin over state" by design), physical tile placement,
  blueprint wand, conductor ride, economy-safe `MineState` sharing, 4.1f persistence, 4.1g budget
  audit. The overhaul is silhouettes + motion + feedback + content tiers, NOT a rewrite.
- **⛔ Heads-up (the gate, stated twice on purpose):** this is PLANNING ONLY. Nothing in F-0..F-9
  starts until the recovery program completes (R1 exit → R2 contracts → R3 golden slice green
  on-device). The doc's own §0 carries the same gate. Envelope F-0 (machine anchor contract) is
  deliberately shaped as an R2-style contract so the recovery lane can slot it when the time comes.
  All factory materials fall under the rb30 Amendment-1 shader-variant law from day one.
- **Commit:** the plan doc + this entry; documentation only, no runtime content changed.

### 2026-07-15 (rb31) — Fable 5 second independent review: R1.4–R1.8 verified with two holds before any headset build

- **Scope:** read-only spot-check of the recovery lane's latest status report against the repo and live
  CI, at Terry's request. No code changed. Verdict to Terry: **the report is accurate and honest** —
  claimed proof SHAs are real (`4988f44` census 28/28 with its raw artifact matching the claims;
  `0e74cec` actual-scene Home Hub 31/31), the new pieces exist as committed source
  (`PersistentDiagnosticRing`, `PlayerInputSessionGuard`, `RecoveryRenderSnapshot*`,
  `RecoveryUiSpatialAudit*`), and all four rb30 amendments show real motion (ring + BOOT_HOLD proof +
  probes-on-golden = A4 acted; the isolation-fault class = A3 acted). `PlayerInputSessionGuard` was
  specifically checked against the "fourth cook" risk: its header and shape are explicitly SUBORDINATE
  to PlayerRigPersistence's contract (owns no meanings, creates no objects) — a genuine consolidation.
  The duplicate scene-`InputActionManager` lifecycle owner it removed is exactly the failure class this
  program exists to catch, and it was caught by the harness, not the headset.
- **HOLD 1 — the current head is unproven: R1.8 PlayMode ran ZERO tests.** Live artifact for head
  `abdb441` (run `29445959178`): `Test step outcome: failure, NUnit totals: total=0` — the suite did
  not execute (the usual signature is a test-assembly compile error). Nothing on the "proven" list
  extends to this head until a run exists where 31+ tests actually execute and pass. The lane's own
  exact-SHA rule already implies this; recording it here so the claim can't drift.
- **HOLD 2 — rb30 AMENDMENT 1 (Android shader-variant static gate) is still NOT BUILT.** No scan
  exists yet in `tools/`, the recovery workflows, or the test suites (checked by grep). It is
  correctly scheduled inside "Golden Android build with patch, bake, shader, audit and APK evidence,"
  but it must EXIST before the next APK Terry installs — otherwise the white-square class
  (`PracticalLight`/`GroundShadow`/`SkyAtmosphereRig` runtime keyword flips) ships silently again and
  the headset session burns on a known fault. Same for the Amendment-2 patch/bake hook-failure gate
  (~41 swallowed try/catches in `BuildAndroid.PatchScenesThenAPK`; Forge bakes are gitignored and
  regenerated per build, so a silent baker failure = primitive guns/blank buildings in a "successful"
  build).
- **Endorsed path to the headset checkpoint (no change requested):** one exact SHA green across
  PlayMode (R1.6 real boot→W000→ToxicCity→back round trip) + snapshot PNGs directly reviewed + UI
  spatial audit + R1.9 fallback/placeholder exposure gate + R1.10 perf artifacts + Golden APK with
  loud patch/bake/shader evidence → independent sampling of the claims (rb30/rb31 are the template) →
  ONLY THEN a bounded Quest checklist. Boring on-device results are the goal; discoveries mean the
  gates missed.
- **Next:** recovery lane clears the two holds; Fable 5 (or any non-lane operator) re-samples at the
  next checkpoint. Separately, Terry has asked the art lane for a deep-dive assessment of the
  automation/factory system's visual legibility (analysis only, nothing built) — boarded so the lanes
  know it's coming; any resulting work is post-recovery R2+ and stays behind the exposure gate.
- **Commit:** documentation-only HANDOFF entry; no runtime content changed.

### 2026-07-15 (rb30) — Fable 5 independent review of the recovery program: ENDORSED, with four required amendments for GPT

- **Scope:** read-only review at Terry's request. No code changed. I read `RECOVERY_PROGRAM.md`, the R0/R1.2 exit reports, the contract inventory, `RecoveryRuntimeGate`/profile code and all three recovery workflows, and verified the lanes against LIVE CI runs rather than trusting the self-reported artifacts. Verdict passed to Terry: **adopt the program as-is** — the proof taxonomy, golden slice, exposure gating and repeat-green promotion rule are the correct systemic answer, and the harness has already caught two real bugs (the InputActionManager active-add lifecycle fault; the Home Hub late-manager binding race). The four amendments below are gaps the current design does not close. Terry approved sending them to the recovery lane.
- **AMENDMENT 1 — close the Android shader-variant hole (highest priority).** There is a failure class NO lane below `QUEST` can catch as designed: runtime-created materials that flip URP shaders to transparent/additive via keywords. `PracticalLight.cs` lines ~145–156 builds its halo by setting `_Surface/_Blend` floats + `EnableKeyword("_SURFACE_TYPE_TRANSPARENT")` on `URP/Unlit` at runtime; the Android build strips that variant unless a SHIPPED material uses it, so the flip silently no-ops → the opaque white streetlight square in Terry's screenshot. `GroundShadow.cs` and `SkyAtmosphereRig.cs` carry the same pattern (grep `_SURFACE_TYPE_TRANSPARENT|SetInt("_SrcBlend"`). The planned `VISUAL` lane runs a desktop renderer where those variants exist — **it will pass VISUAL forever while staying broken on device**. Required: extend the build-time material-validation contract with a STATIC source gate — runtime `EnableKeyword`/blend-state flips forbidden outside an allowlist; transparent/additive materials must be committed `.mat` assets or covered by a ShaderVariantCollection in Always Included. The existing recovery contract-scan tooling can host this scan; it converts the whole class into a CI red.
- **AMENDMENT 2 — stop the build swallowing patch/bake failures.** `BuildAndroid.PatchScenesThenAPK` wraps ~41 hooks in `try/catch` that logs a warning and keeps building. Forge bakes are gitignored and regenerated per build — if `ForgeBaker.BakeAll`/`ForgeAuthor.AssignAll` throws on Terry's machine, the APK ships primitive guns/blank buildings while the build reports success (a live candidate for the "some guns are blocks" observation). Required: the Golden APK lane and the local build must FAIL — or gate on a machine-readable hook-failure manifest — when any patch/bake hook throws, and assert bake-output counts against the recipe catalog (`ForgeDependencyAuditor` already emits the manifests; reuse them as the gate).
- **AMENDMENT 3 — the current PlayMode red is a TEST-ISOLATION fault; fix isolation, don't weaken the assertion.** Verified live: run `29416240184` on head `868fe44` = 20/21, failing `RecoveryHomeHubBindingTests.TileCreatedBeforeManager_BindsThroughDelayedPath` with "Expected: null / But was: XRInteractionManager" and `mode=immediate` in its output — a manager LEAKED from an earlier fixture, so the manager-free precondition never existed. That violates R1.2's own "no leaks after teardown" contract somewhere. Harden SetUp/teardown (destroy stray managers, or isolate the scene) — the delayed-binding fix under test may well be correct.
- **AMENDMENT 4 — guard the recovery lane against its own velocity + keep the evidence probes alive.** The lane self-grades its exit reports at very high commit velocity; the reports cite verifiable run/artifact IDs (good — my spot-checks held up). Required: before EVERY Terry headset checkpoint, an independent operator verifies a sample of exit-report claims against raw artifacts (this review is the template). Also: keep the rb26 `BOARD_PROBE`/`REPAIR_TRACE`/`FLIGHT_TRACE` probes exposed in the golden candidate and give `ZIPTIDE:` tags a persistent ring-file — Terry's last logcat rotated every diagnostic tag away before capture (the rb27 problem), which is why the floating-creature and dark-world causes are still unowned. Minor add: R1.5/R1.6 boot smoke should assert the `BOOT_HOLD` arm/release ordering explicitly so the rb26 boot fix earns `PLAYMODE` proof — the 41 s of `MOVE_DIAG grounded=False` in the recovered logcat is still an open question against it.
- **Live lane status at review time:** contract-scan **green** (`619f0eb`) · PlayMode **red 20/21** (`868fe44`, the isolation fault above) · Golden Android **in progress** on the same head. The both-lanes-green-on-one-SHA candidate rule is right — hold it; Terry does not headset-test before it exists.
- **Heads-up (own-lane admission for the record):** rb27's critique of my DS-14 fix is correct — per-face door labels fixed the facing convention but both faces are visible through open air (the doubled header text in the screenshots). The travel-door label fix belongs to the R2 `DiegeticPanel` contract, not another patch on `WorldTravelStation`. My FORGE III visual systems (grade, light script, practicals, water, macro variation) should stay `PROTOTYPE_HIDDEN` until each individually passes `VISUAL`+`QUEST`.
- **Commit:** documentation-only HANDOFF entry; no runtime content changed.

### 2026-07-14 (rb29) — R0 automated repository cartography: hidden features are still globally active

- **Did (report infrastructure):** added a separate report-only recovery workflow and tooling for repository-wide runtime ownership scanning, inventory validation, ownership maps, build-scene exposure, current-board completion claims, runtime input bindings/chords, exact C# type-to-path resolution, and event/save ownership. Reports are generated into `docs/recovery/generated/` and uploaded as workflow artifacts. No gameplay, scene, prefab, art or runtime behavior changed.
- **Scale of current evidence:** the broad scan covers 598 C# files and records 2,013 exact ownership signals. The initial 26-system inventory contains 37 truth defects: failed/unverified Quest observations recorded as accepted `QUEST` proof, guessed/descriptive source paths, and unresolved canonical owners. The claim audit found 260 strong completion lines in current boards, 138 without a proof qualifier on the same line. These are review targets, not automatic accusations of falsehood.
- **Scene exposure finding:** all 24 Build Settings scenes are enabled. Only `_Boot` and `W000` are already golden-path/support; ToxicCity and W002 are unresolved destination candidates; 20 enabled scenes are legacy tests, non-golden worlds or multiplayer prototypes. Documentation saying a feature is hidden does not make it unreachable in the APK.
- **Automatic-owner finding:** far more global owners run than the original inventory recorded. Confirmed automatic/persistent owners include DebugHUD, XR camera enforcer, runtime health monitor, runtime input enabler, runtime material fixer, VR boot diagnostics, ambience, comfort vignette, conquest mission injection, dev warp board, ecology, PvP progression, Quarters camera injection, SaveSystem, first-hour observation, player rig, audio, travel and singleton validation. Catalog: `docs/recovery/automatic_runtime_owners.json`.
- **Priority-zero visual collision:** `RuntimeMaterialFixer` runs after every scene load, scans every Renderer, and replaces null or non-URP materials with newly allocated URP/Lit materials colored green/blue/gray from object names. Therefore the headset can render something materially different from authored scenes, Forge assignments and build-time art audits. Recovery direction: build-time material validation plus explicit development fallback policy; retire the unconditional runtime rewrite after R1 proves the replacement.
- **Input ownership collision:** `RuntimeInputEnabler` globally reflects across controllers and every MonoBehaviour `InputActionReference`, enabling entire action assets after every load. `PlayerRigPersistence` separately adopts the XRI manager and moves/clears input-action ownership. Recovery decision: `PlayerRigPersistence`/one central input session survives; the reflection fallback is retired or gated after PlayMode proof.
- **Camera ownership collision:** `EnsureXRCameraActive` disables every active Camera not under a name containing `XR Origin` or `Camera Offset`. This can collide with field/photo/snapshot/spectator cameras. Recovery decision: replace it with explicit camera roles and one player-view owner.
- **Y+B finding:** the current `DevWarpBoard` only owns forehead gesture/F2/ADB access. The surviving Y+B contract is in persistent `QuickSwap`: B is quick-swap; Y is a guard that suppresses B because its source still says `Y+B = dev menu chord`. It does not itself open the current board. The control architecture still encodes a retired chord; the generated input report will locate every remaining chord and cross-owner control collision.
- **Travel finding:** runtime scene loading is centralized in `TravelCoordinator`; the only direct synchronous load is its own fallback when no coordinator exists. The main problem is lifecycle/input/UI ownership around travel, not many independent scene loaders.
- **Canonical decisions:** committed machine/human owner tables. TravelCoordinator, SaveSystem and PlayerRigPersistence remain core owners. New recovery contracts are required for exposure gating, diegetic panels, input meanings, camera roles, material/fallback policy, scene presentation, item poses, shooter exclusion and creature contact. Job/repair semantic ownership, the surviving melee implementation and the ship presentation root remain explicitly unresolved pending generated event/source reports.
- **R1 design locked:** `R1_INTEGRATION_HARNESS_SPEC.md` specifies the one-frame PlayMode spike, repeat-green promotion rule, tests-only fake tracked rig, automatic-owner exposure profile, runtime census, Home Hub ray smoke, one-world travel/save round trip, renderer-capable snapshots, UI spatial checks, prototype/fallback visibility gate and checkpoint-only Quest testing.
- **Next (R0):** consume the generated input/source/event reports; replace guessed inventory paths; remove invalid Quest proof labels; select one golden destination; identify the repair/objective semantic owner, melee survivor and ship root; produce the R0 exit report and bounded R1 implementation packet. Do not begin broad runtime consolidation before that exit report.
- **Heads-up:** `docs/CI_VERDICT.md` is still green only for older head `6b9d800`; later R0 tooling/docs commits are not called CI-green until a fresh durable verdict records them. The recovery workflow is report-only and independent of the Unity verdict.
- **Commits (this tranche):** recovery report tools/workflow and generated artifacts · `73b3857` automatic-owner catalog · `f049631` owner summary · `bc910b4` R1 harness spec · `fc9cdaa` canonical owner decisions · `1b919d0` canonical summary · this HANDOFF entry.

### 2026-07-14 (rb28) — GPT Recovery/Integration lane assigned; R0 contract inventory started

- **Authority:** Terry explicitly assigned GPT-5.6 Thinking ownership of ZIPTIDE recovery/integration and said to begin. Normal feature/world/mode/art/multiplayer expansion is now frozen under [`docs/recovery/RECOVERY_PROGRAM.md`](recovery/RECOVERY_PROGRAM.md). Emergency CI-red repair remains allowed; all other implementation requires a bounded recovery packet.
- **Did (control plane):** established recovery phases R0 repository truth → R1 integration harness → R2 contract consolidation → R3 golden vertical slice → R4 template expansion. Added explicit proof levels `SOURCE / CORE / PATCHED / PLAYMODE / VISUAL / APK / QUEST`; “CI green” is no longer a completion state.
- **Did (inventory):** added [`docs/recovery/system_contracts.json`](recovery/system_contracts.json), a machine-readable initial map of 26 critical systems: responsibility, candidate owner, source files, startup, persistence, runtime-created objects, state, actual proof, Quest status, exposure class, conflicts and next evidence. Added the human summary [`docs/recovery/SYSTEM_CONTRACT_INVENTORY.md`](recovery/SYSTEM_CONTRACT_INVENTORY.md).
- **Initial decisions:** preserve valuable pure cores/data/generators; hide Quarters, Tidefront table, PvP/Photon, broad world selection, melee, zipline, flight and unverified practical-light surfaces from the first recovery candidate; consolidate boot/rig/travel/UI/item/visual ownership before exposing them.
- **Next (R0 only):** complete repository-wide bootstrap/persistence/input/travel-bypass/runtime-object/event/save/global-render/fallback scans; reconcile old board claims against proof levels; produce canonical-owner, duplicate-owner, feature-exposure and R1 harness specifications. No broad runtime fixes before the R0 exit report.
- **Heads-up:** no gameplay, scene, prefab, asset, art or system behavior changed in these commits. The repository still contains all prior systems; the freeze controls what may be worked on and what will be exposed in the recovery candidate.
- **Commits:** `3c38dc2` recovery program · `b676507` machine inventory · `336bdaa` inventory summary · this HANDOFF claim.

### 2026-07-14 (rb27) — GPT read-only Quest screenshot forensics: Phase 1 failed on device; audit gates miss visible breakage

- **Did (scope):** Terry stopped the headset pass and supplied ten Quest screenshots plus a recovered logcat. GPT made **no gameplay, scene, prefab, art, asset, or system changes**. This entry is evidence only for Fable 5's audit plan; multiplayer and normal backlog progression remain paused.
- **Device acceptance failed:** menu ownership is still inconsistent on-device (some contexts still expose Y+B while the intended board uses the forehead gesture), and the cold-start Home Hub displays but does not reliably track/accept the pointed target. The rb26 source-level singleton and boot tests therefore did not establish a passing Quest experience.
- **Confirmed screenshot/source failure — persistent HUD:** the large yellow `CR 0` visible in every capture is `CreditsHud`, an always-on camera-relative TextMesh attached to the persistent rig. Its source deliberately redraws at 0.8 m in the lower-left every frame; it is not world scenery and currently contaminates every view.
- **Confirmed screenshot/source failure — travel-door text:** `WorldTravelStation` now emits a TextMesh on both sides of each door/header, but the header has no opaque backing between the two faces. Both copies can therefore be visible through open air. Long world names also share fixed 1.6 m door spacing with no wrapping, clipping, or width budget. This explains the doubled/mirrored and horizontally colliding door labels in the screenshots; DS-14 corrected a facing convention but did not solve multi-face visibility or layout.
- **Confirmed screenshot/source failure — war table:** `ConquestTableRuntime` builds all 12 planet labels, ticker, info card, END TURN/NEW WAR/HOTSEAT, eight defense tiles, and eight vessel tiles at hard-coded positions at once. It uses unbounded TextMesh labels with no panel layout, paging, clipping, facing contract, or camera-space readability pass. The screenshot's wall of overlapping fleet/defense/world text is the direct authored runtime layout, not a missing asset.
- **Confirmed screenshot/source failure — Quarters/locker:** `QuartersRoom` is explicitly primitive-built plumbing. It opens all three bays during `Awake`, emits each header/browse/empty-state TextMesh immediately, uses hand-authored rotations rather than the shared facing helper, and places the locker board at a fixed -90° yaw. The mirrored, stacked `TRAILS & EMBLEMS` / `SHIP LIVERY` / `YOUR LOCKER` text and intersecting room surfaces match that implementation.
- **Confirmed screenshot/source locus — practical lights:** `PracticalLight` intentionally creates camera-facing and surface-aligned **quad** renderers for every halo and pool. On the headset some appear as bright opaque squares/rectangles instead of soft radial glows. The quad ownership is confirmed; the exact runtime failure (texture alpha, URP blend state, material keyword, or device shader behavior) is **not** yet proven and must not be guessed.
- **Confirmed screenshot/source failure — legacy sky planet:** the large blue planet with diagonal bands is the legacy `SkyPlanetRig` sphere. Its texture is literally generated from `sin((x+y)*0.2)`, so the striped-ball appearance is expected from current code, not an asset-loading defect. The newer `SkyVistaRig` also uses unlit sphere bodies plus a global post-processing grade and fog. Cyan scene wash, crushed black silhouettes, and oversized simple celestial bodies may come from that path, but the exact active sky/grade owner must be logged per scene before changing it.
- **Confirmed presentation debt:** several screenshots expose primitive-only structures, conveyor pads, city blocks, ship/locker geometry, flat platforms, and controller-adjacent blocks as if they were final content. Existing source comments explicitly describe many of these as skeletons, plumbing, stubs, or later-art seams. The screenshots establish that those placeholders are not being visually contained or labeled as test content.
- **Previously confirmed and now visibly reproduced:** gun/hammer grip pose, scale, muzzle/laser alignment, holster orientation, and self-hit risks remain open from rb25. The screenshots show a sideways/oversized held object and displaced red aim ray; rb26 did not touch DS-06/07/08.
- **Unresolved device observations:** Terry also saw improved-looking bug creatures floating above the ground, extremely dark/near-black worlds, and mixed menu activation. No scene ID, creature ID, transform trace, or relevant diagnostic tag survived, so ownership/root cause remains unresolved. Do not call the creatures intentional flyers or assign a grounding bug without a scene/object trace.
- **Recovered logcat limitation:** the saved buffer retained no `BOOT_HOLD`, `HOME_HUB`, `DEV_WARP`, `BOARD_PROBE`, `REPAIR_TRACE`, `FLIGHT_TRACE`, `SPAWN_AT`, world, or creature identity records. It did retain about 41 consecutive seconds of `MOVE_DIAG provEnabled=True ... grounded=False`. That is evidence of an enabled locomotion provider while the controller reported ungrounded, but it does not by itself distinguish spawn, floor collider, grounding, or capture-timing causes.
- **Why green CI missed this:** `UiReadabilityAuditRules` is warning-only and checks only effective TextMesh size, collider target size, and label-to-collider separation. It does not test facing, duplicate opposite-face visibility, text overlap, wrapping, camera occlusion, contrast, or full runtime-built layouts. `ArtConformanceAuditRules` checks only whether a renderer has recognized provenance, not whether it looks good; every world is currently unlocked, findings are warnings, and an entire `PracticalLight` or `SkyVistaRig` subtree counts as conformed regardless of the screenshot result. Runtime objects created in `Awake`/`Start` also evade ordinary authored-scene inspection unless the audit explicitly instantiates them.
- **Next:** Fable 5 produces the audit/recovery plan from this evidence before any further feature work. The plan must inventory runtime-created UI/HUD/visual systems, identify one owner per responsibility, add screenshot/device acceptance gates, and separate confirmed source causes from scene-specific unknowns. Do not resume Phase 2, Picasso expansion, world mass-build, or multiplayer merely because EditMode and patch/audit CI are green.
- **Heads-up:** the ten screenshots are device evidence supplied in Terry's chat and are not committed binary assets. Preserve the observations above as the durable record; request scene/object IDs or targeted logs only when a proposed fix actually requires them.
- **Commit:** documentation-only HANDOFF entry; no runtime content changed.

### 2026-07-14 (rb26) — Fable 5: Phase 1 stabilization implemented + DS-09/10/12 evidence probes

- **Did (plan):** converted the rb25 forensic map into an approved fix plan with Terry's two decisions
  locked: the **physical board idiom owns DS-02** (device-proven rendering; the TMP canvas carries the
  recorded 2026-07-06 dead/flicker failure) and **instrumentation is included** alongside Phase 1.
- **Did (DS-01, `fix(boot)`):** the cold-boot HOLD contract. BootLoader arms it before the Home Hub;
  while held: move/turn/snap/dash providers suspended, the global fall net disarmed, rig pinned to its
  boot pose. Released ONLY in `TeleportToMarker` after a content spawn settles (re-arms the net from the
  fresh spawn); marker-less content scenes release too, so nobody arrives frozen. Pure `BootHoldState`
  seam + `BootHoldTests`. Logs `ZIPTIDE: BOOT_HOLD on/off`.
- **Did (DS-02/03, `fix(devtools)`):** ONE dev menu. `DevWarpBoard` is now summoned (forehead gesture /
  F2 / ADB gate), dismissible (gesture toggle + red CLOSE tile), fixed-pose at summon (the orbiting
  billboard is deleted), labels un-mirrored via the facing contract, and any open board closes on scene
  load. `DevMenu` retired from runtime (no bootstrap, no gesture; kept only as a manually-mounted
  diagnostic with the reason in its header). `DevToolsSingletonTests` source-scans DevTools: exactly one
  `RuntimeInitializeOnLoadMethod`, and it must be the board.
- **Did (DS-14/DS-03, `fix(ui)`):** `WorldLabelFacing` — THE facing contract in one pure helper
  (TextMesh reads from −Z; facing a viewer = +Z points AWAY). Travel doors now carry a label per FACE
  (readable from both approach sides, static, no per-frame cost). Tests pin the convention, pin the old
  buggy `LookRotation(toViewer)` as unreadable forever, and prove HomeHub's board math was already
  correct (why Terry could read the hub but not the doors).
- **Did (DS-09/10/12, `diag`):** log-only probes, zero behavior change — `ZIPTIDE: BOARD_PROBE`
  (hover/select + 1 Hz aim probe: actual ray hit path/layer, bound manager instance, facing dot),
  `ZIPTIDE: REPAIR_TRACE` (every hop: machine → director → runtime consumed/banked → bank-drain →
  objective board rendered text → cast-off observed instance), `ZIPTIDE: FLIGHT_TRACE` (per emitted yaw
  snap: raw stick, latch, yaw, frame ms). The forensic plan forbids behavior edits on these three until
  a capture picks the branch.
- **Next:** Terry runs **TERRY_RUNBOOK §0** (the Phase-1 device checklist + the logcat capture). Then:
  Phase 2 (DS-06/07/08 item/holster/hammer pose contract) with the captured evidence feeding DS-09/10/12
  fixes; DS-04/05 in Phase 3.
- **Heads-up:** DS-05's tracked-head fix is one line in `TravelCoordinator` but is deliberately parked
  for Phase 3 with its test, per the plan ordering. DevMenu's TMP canvas path still exists for manual
  diagnosis only — the singleton test turns CI red if anyone re-bootstraps it. All Phase-5 art items
  stay parked for Picasso.
- **Commit:** `a0dfdc7` (DS-01) · `5774182` (DS-14/03) · `aa59e7c` (DS-02/03) · diag + docs follow.

### 2026-07-14 (rb25) — GPT forensic device-stabilization map

- **Did:** Terry’s first successful Quest build installed and launched, then exposed failures across boot/spawn, developer UI, traversal, item poses, interactions, tutorial continuity, ship controls, and visual finish.
- **Did:** GPT made no gameplay, scene, prefab, asset, or system implementation changes. It traced the reports to source and wrote [`docs/DEVICE_STABILIZATION_FORENSIC_PLAN.md`](DEVICE_STABILIZATION_FORENSIC_PLAN.md).
- **Confirmed collisions:**
  1. The new destination menu waits in an empty `_Boot` scene while older locomotion and fall recovery remain active.
  2. `DevWarpBoard` and `DevMenu` both self-bootstrap and persist.
  3. Player-centered travel VFX receives the XR rig root instead of the tracked-head center.
  4. A universal 45-degree gun fallback is reused across newer item, holster, and melee paths.
  5. `SonicThumper` and `PvpHammer/HammerTool` are separate mallet implementations with separate pose rules.
  6. Cave zipline endpoints are authored without generated-geometry clearance, and the shown cable differs from the rider path.
  7. Physical repair, job progress, objective display, and ship cast-off consume related state through separate runtime owners.
  8. Core ship and world systems currently expose explicitly interim primitive art as final-looking content.
- **Confirmed source causes/debt:** the 45-degree fallback and zero-valued item data explain the gun pose/scale; hand and holster poses are not independently authored; weapon paths lack a complete shooter-ignore contract; the ship hull, many buildings, fake-light visuals, planets, mountains, and vistas are documented primitive/interim/fallback implementations.
- **Unresolved pending instrumentation:** Match Board selection, gate objective not advancing, ship-turn glitch, exact streetlight-square branch, and any structural difference between W000 and W004. The plan specifies evidence to collect before changing them.
- **Next:** Fable 5 begins with Phase 1 only: cold-boot safety/ownership, one developer menu, and readable world-label facing.
- **Next phases:** item/holster/melee correctness; traversal/travel/tutorial continuity; ship function; Picasso visual recovery; then multiplayer.
- **Heads-up:** multiplayer is paused. Patch/audit CI proves structural readiness, not tracked-device pose, UI facing, interaction feel, objective continuity, or art quality.
- **Heads-up for Picasso:** real ship hull, buildings, practical lights, planets/clouds, and distant vistas remain named Phase 5 work. They are parked until function stabilizes, not discarded.
- **Commit:** plan `075afd9`; HANDOFF archive/pointer update follows as documentation only.

---

## Prior history

The complete prior HANDOFF is preserved byte-for-byte in [`docs/HANDOFF_HISTORY_THROUGH_RB24.md`](HANDOFF_HISTORY_THROUGH_RB24.md). Read rb24 and rb23 there for the patch/audit CI rules and W011 collider history.
