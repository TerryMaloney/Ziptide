# CREATURE ROSTER & PROMPT QUEUE — every living/hostile thing, what art exists, and the next prompts

**Written 2026-07-26 (T-Dog lane) for Terry's Tripo run-up.** Uses the canonical framework in
`CONCEPT_ART_PROMPT_PLAYBOOK.md` (§3 template, §5.1 species archetype, §4 visual constants).
Companions: `CONCEPT_ART_NEXT_30.md` (approvals) · `systems/CREATURE_DESIGN.md` (behaviour canon)
· `design/ENEMIES_ENCOUNTERS_AND_BOSSES.md` (the combat state machine).

---

## 0. THE HEADLINE FINDING

**The body pipeline is in better shape than the art pipeline.** `ForgeBodyLibrary` defines forged
bodies for **six** of the seven shipped creatures (only two are committed as `.asset` files —
`light_grazer`, `swarm_bug`; the rest are generated at build time by the create-only author, which
is why a disk listing under-reports them). The real gap is **concept art**: four shipped creatures
the game already spawns have no approved sheet, so their forged bodies were built blind — the exact
condition the pipeline doc blames for the Warden taking five rounds.

| Shipped `CreatureDefinition` | Concept art? | Forged body? |
|---|---|---|
| `witness_mite` | ❌ **none — prompt now** | ✅ defined |
| `light_grazer` | ❌ **none — prompt now** | ✅ defined |
| `husk_molter` | ❌ **none — prompt next** | ✅ defined |
| `tether_swarm` | ❌ **none — prompt now** | ⚪ intentionally none — it is a cluster + cord, not one body |
| `swarm_bug` | ✅ as *cistern_swarmer* (ID MISMATCH) | ✅ defined |
| `tendril` | ✅ as *glass_tendril* (ID MISMATCH) | ✅ defined |
| `warden` | ✅ as *warden_drone* | ✅ defined |

**⚠️ ID drift to resolve before Tripo:** concept sheets use `creature_cistern_swarmer` /
`creature_glass_tendril`; the shipped assets use `swarm_bug` / `tendril`. Pick one set of IDs now —
retrofitting after models are imported is the expensive version.

## 1. THE LOCKED LAWS (obey in every prompt — these are already blessed)

- **Two glow languages, never mixed.** Creatures carry **biological bioluminescence**: toxic
  yellow-green (canal), phosphor blue (cave swarm), tuned cyan (things tuned to the signal).
  Machines carry **powered/signal** light. A glow on a creature reads *alive*; on a machine it
  reads *powered*.
- **Machine-eye colour law:** RILL **amber** (friend) · Warden **sterile white** (immune system) ·
  rogue + Wake-Guild drones **RED** (hostile). Player tech **cyan** vs hostile fire **red-orange**.
- **Warden kinship:** capital ship ↔ warden drone ↔ RILL are ONE family — smooth ceramic-bone,
  single eye. The warden drone is RILL's *cold-eyed* twin (black glass) against her warm iris.
- **Wake-Guild faction mark:** anchor + cog.
- **World tone:** used-future 1970s sci-fi, salvage-built human tech, monumental Architect tech,
  smooth sterile Warden tech, family-readable silhouettes, restrained emissives.
- **Non-lethal canon:** every hostile needs a believable **disabled/powered-down state** — that is
  a required panel, not a bonus.

## 2. WHAT WE ALREADY HAVE (5 sheets, `concepts/bestiary_ch1/` — approved 2026-07-21)

1. **Canal Stalker** (`tox_canal_stalker_01`, W001) — ~3.5 m crocodilian-salamander, grey-green
   flesh + dorsal plates, yellow-green hazard stripes, gill-fringed maw. Canon: low-set dorsal
   eyes + vibration-sense head.
2. **Cistern Swarmer** (W002) — cat-sized isopod/silverfish, pale chitin, phosphor-blue underside,
   nests in clusters.
3. **Glass Tendril** (W003) — ~1.5 m anchored flora-fauna, brown chitin pod on root-claws, cyan-
   veined whipping tendrils, rooted (doesn't chase).
4. **Warden Drone** — human-sized hovering ovoid, ceramic-bone plating, ONE black cyclopean eye.
5. **Rogue Drone family + Wake-Guild Recon Spider** — feral security drones (rusted salvage,
   ducted fans, RED eye, pulser emitter, includes the **powered-down state**) and the maintained
   quad-lens Guild spider with the anchor+cog emblem. Plus `pulser_weapon_sheet_v1` for their fire.

*Also existing (not enemies): `rill_drone/` (companion), `warden_capital/` (ship).*

## 3. THE FULL EXPECTED ROSTER

### 3.1 Feral biologicals — the 12 novel behaviours (canon: `CREATURE_DESIGN.md` §25)
Each is a *mechanic first*, which is exactly what makes them worth modelling.

| # | Creature | Mechanic | Art |
|---|---|---|---|
| 1 | **Witness-mite** | freezes when observed/scanned; moves only unwatched | ❌ **prompt now** |
| 2 | **Rewinder** | afterimage trail; snaps back 2 s when hit | ❌ |
| 3 | **Sound-walker** | only traverses *vibrating* surfaces; inert in silence | ❌ |
| 4 | **Tether-swarm** | many bodies, ONE health pool via a glowing tether | ❌ **prompt now** |
| 5 | **Orbit-grazer** | orbits a gravity well; break the orbit, not the body | ❌ |
| 6 | **Inverter** | local gravity flip — fight it on the ceiling | ❌ |
| 7 | **Mimic-echo** | copies your last movement on a delay | ❌ |
| 8 | **Husk-molter** | sheds a decoy husk when stunned | ❌ **prompt next** |
| 9 | **Tide-phase** | solid only at low tide/pressure | ❌ |
| 10 | **Light-grazer** | grows in darkness, shrinks in light | ❌ **prompt now** |
| 11 | **Fractal-splitter** | splits into weaker copies when hit | ❌ |
| 12 | **Bridge-former** | spliced (not killed) it becomes a bridge — combat→traversal | ❌ |

### 3.2 The four base archetypes (`GAME_PLAN` M3)
Swarmer ✅ (cistern swarmer) · WallCrawler 🟡 (husk-molter, no art) · Flyer ✅ (rogue drone) ·
**Bruiser ❌ — we have no heavy at all.** That is a real roster hole; a Bruiser is the enemy that
makes an encounter feel dangerous rather than busy.

### 3.3 Machines & factions
- **Rogue drone family** ✅ (light/standard/veteran map to `drone_easy/standard/veteran`).
- **Wake-Guild** ✅ recon spider — the faction needs at least a second unit later.
- **Wardens** ✅ drone · ❌ heavier Warden classes (they escalate with Signal level, can become an
  ALLY in Ch.6, and one *recognises RILL* — that's a hero moment needing its own model).
- **Architect constructs** ❌ — monumental, precise, deep-amber sealed. None designed yet.
- **Bosses** ❌ — the enemies doc names Salvage Titan, Hive-Warden, Mimic Vault. All unarted.

## 4. THE PROMPT PACK — first three, ready to paste into Gemini

Each follows playbook §3 + §5.1. **Every one requires the disabled state** and the pose set the
animator needs (see §5). Generate breadth first (playbook §6 three-pass method), then converge.

---
### PROMPT 1 — WITNESS-MITE (`witness_mite`)
> Multi-angle creature turnaround concept sheet for a video game bestiary, neutral pale grey
> studio background, orthographic-style side / front / three-quarter / top views plus two state
> insets, consistent lighting, no text labels.
>
> **Subject:** a small alien organism the size of a house cat, called a Witness-mite. Its body is
> a low, wide, armoured disc of overlapping chalky plates — like a trilobite crossed with a
> barnacle — carried on six short jointed legs tucked close beneath it. The upper shell is studded
> with dozens of tiny glassy dark nodules arranged in rings, reading unmistakably as **eyes that
> are also being looked at**. No face, no mouth on top; the underside has a small radial feeding
> aperture.
>
> **Its defining behaviour, which the design must telegraph:** it freezes absolutely solid when it
> is observed, and only moves when nobody is watching. So the creature must look *convincingly
> statue-like* in its frozen state — plates clamped flat, legs locked, glassy nodules dull and
> stone-like — and visibly *alive* in its moving state: plates lifted and fanned slightly apart on
> soft pale membrane, legs extended, the nodules gone wet and glossy with a faint **cold blue-white
> bioluminescence** in the seams between plates.
>
> **Required panels:** (a) FROZEN state, plates clamped, reads as a rock; (b) MOVING state, plates
> fanned, legs out, seams glowing faintly; (c) DISABLED/powered-down state — plates half-open and
> slack, glow extinguished, harmlessly inert, clearly not dead but switched off; (d) an underside
> detail showing the feeding aperture and leg attachment; (e) a scale comparison beside a human
> hand.
>
> **Material language:** chalky mineral shell like weathered limestone and mother-of-pearl, matte
> not glossy, with fine sediment caked in the plate seams. Colour: bone-grey and pale ochre body,
> cold blue-white biological glow only in the seams.
>
> **Style:** grounded used-future 1970s science-fiction creature design, believable biology,
> readable silhouette a child could sketch after one look. Photoreal concept-art rendering.
>
> **Avoid:** cute cartoon eyes, humanoid face, insect mandibles, teeth, gore, glowing cyan
> (reserved for machines), heavy asymmetry, tentacles, spikes added for decoration.

---
### PROMPT 2 — LIGHT-GRAZER (`light_grazer`)
> Multi-angle creature turnaround concept sheet for a video game bestiary, neutral pale grey
> studio background, side / front / three-quarter views at TWO different sizes, plus state insets,
> consistent lighting, no text labels.
>
> **Subject:** a Light-grazer — a soft-bodied cave organism that **grows in darkness and shrinks
> away from light**. Its body is a translucent, gently inflated sac slung under a fan of thin
> ribbed vanes, like a cross between a jellyfish and a moth's wing, walking on four long spindly
> legs that fold. Through the translucent skin you can see a dark dense core and branching
> filaments — clearly the thing that swells or contracts.
>
> **Its defining behaviour:** the SAME animal appears at two sizes. Show a small one (knee-high,
> tightly furled, vanes collapsed, skin nearly opaque and wrinkled) and a large one (two and a half
> metres, hugely swollen, vanes spread wide and translucent, filaments glowing a soft **phosphor
> blue-green** through the skin). They must read as unmistakably the same species at different
> stages, sharing leg count, vane structure and core anatomy.
>
> **Required panels:** (a) SMALL furled form; (b) LARGE swollen form; (c) mid-shrink state, vanes
> half-collapsing and skin puckering, caught in the act of recoiling from light; (d)
> DISABLED/powered-down state — deflated and slack on the ground, glow gone, still intact;
> (e) close detail of the translucent skin showing the core and filaments beneath.
>
> **Material language:** wet translucent membrane like frosted silicone over a dark organic core,
> matte ribbed vanes with fine dust clinging to them, no armour. Colour: pale bruise-violet and
> grey membrane, phosphor blue-green internal glow only.
>
> **Style:** grounded used-future 1970s science-fiction creature design, believable soft-tissue
> biology, family-readable silhouette. Photoreal concept-art rendering.
>
> **Avoid:** teeth, faces, aggression cues, hard armour plating, cyan glow, cartoon proportions,
> mushroom clichés, gore.

---
### PROMPT 3 — TETHER-SWARM (`tether_swarm`)
> Concept sheet for a video game bestiary showing a colony creature, neutral pale grey studio
> background, group composition plus individual turnaround and detail insets, no text labels.
>
> **Subject:** a Tether-swarm — five to seven small flying bodies that share **one life through a
> single glowing cord**. Each individual is a hand-sized, eyeless, teardrop-shaped flier with a
> pair of stiff translucent wing-fins and a trailing filament; the filaments all run back and knot
> into one thick braided **nerve cord** that hangs between the group like a shared umbilical.
> The cord is the creature — the bodies are just its hands.
>
> **The design must make the cord unmissable**, because the player is meant to attack the cord and
> not the bodies: it should be the brightest, thickest, most deliberate element in the image, a
> braided living cable with a warm amber-gold glow pulsing along its length, visibly feeding each
> body where the filament enters.
>
> **Required panels:** (a) the full swarm in formation, cord taut and glowing, bodies fanned out;
> (b) a single individual turnaround, front and side, showing the filament root; (c) the cord SEVERED
> — bodies drifting apart, filaments dark and limp, glow draining outward from the cut; (d)
> DISABLED state, whole colony settled and inert but intact; (e) a close detail of the braided cord
> texture and where a filament joins a body.
>
> **Material language:** bodies of dull waxy chitin like old candle wax, wing-fins of stiff
> translucent membrane, cord of braided sinew with a wet fibrous surface. Colour: dust-grey and
> bone bodies, warm amber-gold light ONLY in the cord and filaments.
>
> **Style:** grounded used-future 1970s science-fiction creature design, unsettling but not gory,
> readable at a distance as "one creature made of many." Photoreal concept-art rendering.
>
> **Avoid:** insect faces, mandibles, stingers, red glow (reserved for hostile machines), swarm
> that reads as unrelated individuals, blood, horror-movie body horror.

---
**Next in queue after these:** Husk-molter (needs its shed husk as a separate modelled prop) ·
**a Bruiser heavy** (the roster hole) · Fractal-splitter (needs 3 nested sizes) · Bridge-former
(needs its hostile AND its spliced-bridge form).

## 5. THE CREATURE CARD (v2 — corrected after research, 2026-07-26)

**The earlier six-line card was the right idea, the wrong shape.** Research verdict: it is adequate
as a pre-*animation* gate but **insufficient as a pre-*modelling* gate**, because only two of its six
lines constrain the mesh — and it **omitted the COUNTER state, which our own CI already requires**
(`CreatureBehaviorReadabilityCatalog` enforces `ActiveStates` ≥3 + `TelegraphState` + `CounterState`
+ disabled — verified in source). It was also entirely flatscreen-derived: nothing about angle,
distance, height, sound, hands, or personal space — the five things that decide whether a VR enemy
works at all.

Industry sizing for reference: "feels dead" is 3–5 clips, **"feels alive" is 10–14**, solid indie
20–35, AAA 60–150+. **Our target is 12 clips per Standard creature** — which is exactly what
`CREATURE_QUALITY_V2_LIFE_LEAP.md` §V2.6's ten-state motion vocabulary already implies. Block A
below is essentially §V2.2's socket passport pulled forward to concept time, where it belongs.

### BLOCK A — BODY CONTRACT *(write BEFORE pressing Generate; irreversible at mesh time)*
1. **Scale** — metres; threat-height band; must read at **both 1.0 m (kid/seated) and 1.7 m** eye height.
2. **Locomotion mode** — limb count, ground/wall/fly/burrow, gait role.
3. **Silhouette triad** — idle / telegraph-apex / disabled, drawn as three thumbnails on the sheet.
4. **Attack anatomy** — the part that hits, therefore the joints required (jaw? N-segment whip? emitter? inflating bladder = blendshape).
5. **Socket map** — gaze · mouth/emitter · **weak-point ×n** · **grab handle(s)** (controller-grip sized, positioned OFF the face) · tether point · feet/anchors · VFX mount · audio mount · **carry handle for the downed body**.
6. **Separable parts** — shed husk, severed cord, salvage parts: each needs its own mesh, sockets and **cap geometry at the break** (you cannot invent an interior later), plus a missing-part silhouette that still reads as the species.
7. **Tell channel** — which surface region carries the state light, as its own material region/mask (decided before UV/bake).
8. **Physicality** — grabbable / pushable / ragdoll / static + collider proxy shapes (mesh colliders are banned for gameplay).
9. **Variants** — sizes/stages and the mechanism: blendshape, bone scale, or separate mesh.

### BLOCK B — BEHAVIOUR *(before rig/animation — the six lines, corrected and completed)*
10. **IDLE** — the 80% behaviour + one idle break.
11. **NOTICE tell** — pose change **plus its sound**; grace window before it commits.
12. **APPROACH** — how it closes · **closest approach to the headset in metres** · enters arm's reach y/n.
13. **ATTACK** — telegraph pose · wind-up duration · **apex hold (~0.2–0.3 s)** · **the player's answer and how long that physically takes** · whiff/recovery window.
14. **COUNTER** — the fair, taught answer. **CI-REQUIRED — omitting it fails the audit.**
15. **HIT REACTION** — generic additive flinch + one weak-point-specific reaction. Usually only front + grabbed-side matter, not four directions.
16. **WEAKENED tell** — the limp / one-limb-down gait. Monster Hunter's lesson: the gait change *is* the "capturable now" signal, and it reads with no HUD.
17. **STUNNED** — grabbable window · flail performance (it must perform urgency, not just freeze) · recover timer · resist tier.
18. **DOWNED** — pose · recoverable y/n · carryable y/n · which parts detach and in what order · what it leaves behind.

### BLOCK C — VR CONTRACT *(the block a flatscreen list would never contain)*
19. **Any-angle readability** — the telegraph must read as a **silhouette-scale change** (never a facial/detail change) from 3 registered views, at **0.6 m and 10 m**. There is no camera to frame it for you.
20. **Off-FOV channel** — Quest FOV is ~90–110° of a ~200° human field, so **two-thirds of the sphere has zero visual telegraph**. One spatialised sound per state *transition*; haptics if it can touch you; state how it announces itself when below or behind the gaze cone.
21. **Personal space** — minimum head distance **≥0.6 m, prefer ≥0.8 m** for face-height creatures (below ~0.5 m the eye cannot comfortably fixate); approach-vector rule; never materialise behind the head or inside guardian.
22. **Hand answer** — what happens when the player grabs / blocks / pushes / covers it, **including mid-attack**. If grabbable, the grapple is **IK-lock + offset blendspace + release-stagger matched to hand direction** (the *Saints & Sinners* pattern), never free physics — VR hands feel no resistance, so simulated resistance reads as mush.
23. **Comfort & kid rails** — no head-grab, no strobing tell, no fast near-face pass, no attack requiring physical retreat outside guardian, readable at both eye heights.

**Timing correction that matters:** 0.4 s was a controller-era number. A *hand* answer (raise shield, shove, swat) fits ~0.4–0.5 s; a **whole-body answer (duck, sidestep, back off) needs 0.6–0.9 s**, and a guardian-bound player often cannot retreat at all. Both *Until You Fall* and *Half-Life: Alyx* independently concluded VR combat needs slower, louder, more explicit intent than its flatscreen equivalent — Alyx deliberately made the Combine *less* reactive than in HL2.

**Explicitly deferred to animation** (say so on the card so nobody blocks on it): frame counts and curves, extra idle breaks, directional flinch expansion, turn-in-place, taunts, attack variants, spring-bone tuning, get-up timing, LOD behaviour.

### The non-lethal states lethal games never need
Our disable-and-salvage thesis adds: **weakened/partial disable** (the highest-value addition — MH's limping tell) · **capture gating** (cannot capture until sufficiently weakened) · **struggle while tethered** (Ghostbusters' wrangle — needs a tether socket and a strain pose) · **flee-to-hide and recover** · **reactivation/reboot** (constrains the downed pose: it must be get-up-able) · **carried/dragged** (the downed body is a physical prop — carry handle, held volume that does not blind the player) · **harvest/part-out** (detach order, cap geometry, per-part sockets) · **released, wakes up, wanders off** — one clip that buys the entire kid-friendly fantasy.

**Recommended clip budget per Standard creature: 12** — idle, idle-break, locomotion, turn-in-place, notice, approach, telegraph, apex, whiff-recover, additive flinch, stun-loop, downed. Signature tier adds get-up and one ecology action.

**Tripo-month workflow:** approve the sheet → fill Block A → generate → confirm the silhouette still reads in the telegraph pose → fill Blocks B/C → Forge/import per `CONCEPT_TO_BUILT_PIPELINE.md` steps 3–5 → device verdict.
