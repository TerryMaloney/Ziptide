# SPACE DESIGN — what comparable games do, and what fits ZIPTIDE
### Terry 2026-07-21: "look up what other games do for these scenarios; figure out what fits our game best, best judgment." Research-grounded recommendations for the gap-audit gaps.

**Status:** 🔵 RESEARCH + RECOMMENDATION — zero code (freeze). Resolves the open ⚖ from
`SPACE_FLIGHT_GAP_AUDIT.md` using real comparables (sources §5). Filtered through Ziptide's pillars.

---

## §0 — THE LENS (Ziptide's pillars — every choice must pass these)

**VR comfort-first · Quest motion controllers (no assumed HOTAS) · all-ages · NON-LETHAL ·
cozy-but-epic · used-future · one-ship-is-HOME · single-player-first · EXPLORATION + SALVAGE over
dogfighting.** Any mechanic that fights these loses, however cool elsewhere.

## §1 — THE NORTH-STAR BLEND (who we borrow from, and for what)

Ziptide's space = a deliberate blend, all comfort-capped:
- **No Man's Sky** — the PACE + posture: seated, cockpit-anchored, **exploration-forward, combat
  light/secondary** (NMS's own combat is "functional but mediocre" and nobody minds — it's not a
  dogfighter). *Our lesson: don't build a twitch dogfighter; build a place to explore and salvage.*
- **Everspace 2** — **freedom to STOP, breathe, navigate at your own pace**; handcrafted persistent
  POIs over endless proc-gen; strafe + elevation for fine control near docks/wrecks. *Our cozy,
  no-timer, handcrafted-world instinct, validated.*
- **Rebel Galaxy Outlaw** — **soft-lock combat**: lock a target, the ship soft-follows and matches
  its pace, but YOU still aim and fire — "generous enough to remove the hassle, not so much it
  plays itself." *The exact comfort/all-ages combat model we want.*
- **Vox Machinae** — **grab the virtual controls**: hold grip to lock your hand to a physical
  throttle/stick in the cockpit; players find it MORE immersive than a HOTAS because every control
  is represented in the cockpit. *Our diegetic piloting answer, and it fits the levered cockpit we
  concepted.*

**One line:** *NMS's pace + Vox's hands + Rebel Galaxy's soft-lock + Everspace's handcrafted POIs.*

## §2 — RECOMMENDATIONS PER GAP (rec · why it fits · comparable)

- **A1 Piloting interaction → GRAB THE VIRTUAL THROTTLE + STICK (Vox Machinae model).** Grip to
  lock your hand to the console throttle (push = accelerate) and a stick (pitch/snap-yaw); release
  to let go and use your hands for other cockpit controls. Base input = virtual grab on Quest
  controllers; **optional HOTAS support later** (Elite/VTOL VR pattern) but never required. *Most
  immersive, uses the cockpit we designed, no peripheral assumed.*
- **A2 Instruments → DIEGETIC gauges on the console** (speed/altitude/armor/nav/target), physical,
  not a head-locked HUD. *NMS's fixed-forward HUD is tolerable but panel-diegetic is better for our
  presence bar.*
- **A3 Hover / station-keeping → YES, a hold mode** (throttle to zero holds position; strafe +
  elevation for fine moves). *Everspace's "stop and breathe" — essential for salvage, inspection,
  and our un-rushed pace.*
- **A4 Flight model → ARCADE, not Newtonian.** Speed you set holds; gentle assisted turns; a boost
  ability; no drift/inertia sim. *Comfort + all-ages; NMS/Rebel Galaxy/Everspace-default are all
  arcade. Full Newtonian is a nausea + skill-wall we don't want.*
- **B1 Wayfinding → DIEGETIC nav beacon.** Pick a target on the system/galaxy map → a soft
  holographic waypoint + RILL bearings ("two-seven-zero, Cal"), not a cluttered HUD. *NMS objective
  marker, diegetic-ized.*
- **B2 THE LOOP / missions → EXPLORATION + SALVAGE forward, no timers.** Mission types that fit:
  **salvage run** (find→disable→tractor→economy) · **discovery** (a derelict/POI to explore for
  lore/story) · **ferry/delivery** (cargo between docks/worlds) · **assist/escort** (protect a slow
  craft — non-lethal) · **distress call** (a rescue — warmth) · **gate-run** (travel to a new
  system). **Combat is SPICE, not the meal.** *NMS pillars minus survival-grind + Everspace's
  handcrafted encounters; maps straight onto our disable+salvage economy.*
- **C1 Combat targeting → SOFT-LOCK (Rebel Galaxy Outlaw).** Lock the nearest threat, ship
  soft-follows + matches pace, generous aim-assist, YOU fire the non-lethal disable weapons. *The
  proven comfort/all-ages answer; kills VR-dogfight nausea; keeps agency.*
- **C2 Salvage interaction → TRACTOR FROM THE COCKPIT.** Disable a wreck → tractor-beam grab → reel
  it in → converts to salvage/economy. *Reuses our grabber-arm + ground salvage grammar; no EVA
  needed (see §3).*
- **D1 Fuel/energy → NO punitive fuel; a self-recharging DRIVE.** *NMS's launch-fuel grind was
  widely disliked; we're cozy, not survival. Maybe a boost-energy that recharges — never a "stranded
  because you ran dry" chore.*
- **D2 STRANDED/DISABLED → a RESCUE, never a harsh game-over (our non-lethal canon).** Armor breaks/
  drive dies → ship goes dead-in-space → **RILL fires a distress beacon** → a tense drift beat → an
  **auto-recovery TOW back to the last dock** (or a friendly salvager hauls you in). You lose time/
  maybe some cargo, not the game. *The warm inverse of Endless Sky's punishing "disabled and
  floating"; ties to RILL + the repair loop + restore-not-destroy.*
- **F1 Narrative in flight → RILL/Cal banter + INCOMING TRANSMISSIONS during travel.** The
  de-garbling Transmission delivered on quiet flights. *Every space game does comms-in-flight; this
  finally uses travel as the story-delivery channel Terry wanted (the "COD cutscene in transit").*
- **G1/G2 Onboarding + accessibility → seated-first + an AUTOPILOT/assist-fly option.** Auto-cruise
  to a selected target for players who can't/don't want to hand-fly; the `SpaceLane_Trial` becomes
  the teaching level. *NMS seated posture + Everspace/Elite autopilot-to-POI; huge for all-ages +
  accessibility.*

## §3 — THE ⚖ SCOPE DECISIONS (my recommendation, for Terry to bless)

1. **Arcade flight, NOT Newtonian** — comfort + all-ages. ✅ recommend.
2. **No EVA in v1** — you stay in the ship; salvage is tractor-from-cockpit. Keeps scope + comfort;
   revisit EVA much later. ✅ recommend.
3. **No punitive fuel** — recharging drive/boost only. ✅ recommend.
4. **Soft-lock combat, not free-aim dogfighting** — comfort + agency balance. ✅ recommend.
5. **HOTAS optional, virtual-grab base** — never require a peripheral. ✅ recommend.
6. **Combat is optional spice** — the game is beatable exploration/salvage-forward; a pacifist can
   largely avoid fights (matches the ground "disable, don't kill / pacifists still finish"). ✅ recommend.

## §4 — HOW THIS CLOSES THE GAP AUDIT (the failed columns)

`SPACE_FLIGHT_GAP_AUDIT` §3 flagged LOOP/FAILURE/NAV/NARRATIVE/ONBOARDING. This doc answers each:
**LOOP** = §2 B2 mission set · **FAILURE** = §2 D2 rescue-not-death · **NAV** = §2 B1 diegetic
beacon · **NARRATIVE** = §2 F1 transmissions-in-flight · **ONBOARDING** = §2 G1/G2 trial + assist-
fly. VERB/PLACE/ACCESSIBILITY/COMFORT were already covered. → the domain-completeness checklist
(MISS_LEDGER #15) now passes for space, pending Terry's ⚖.

## §5 — Sources
- No Man's Sky (VR posture, explore-forward, combat-light): Wikipedia; Time; RoadToVR/community.
- Everspace 2 (at-your-own-pace, POIs, strafe): TheGamer review; Everspace wiki/FAQ.
- Rebel Galaxy Outlaw (soft-lock combat): Kotaku; Game Developer "Designing the satisfying space
  combat of Rebel Galaxy Outlaw".
- Vox Machinae (grab virtual controls): UploadVR hands-on; Steam community; DCS/Elite VR cockpit
  discussions.
- Disabled/stranded patterns: Endless Sky (GitHub auto-recovery request); Space Engineers community.
- VR flight comfort (cockpit anchor, frame consistency, seated): virtual-flight.com guide; NMS VR wiki.
