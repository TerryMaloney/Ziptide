# STORY GENESIS — how to write an excellent story that becomes a fun game
### The pre-pipeline: reverse-engineered from Ziptide's bible, fused with story craft, runnable by any model

`Stage: pre-pipeline (Slipway) · Type: research — PARALLEL TO ZIPTIDE; no project doc depends
on this; changes here never gate or alter the game.`

**What this is:** the method that runs BEFORE `PIPELINE.md` Stage 0. Input: an idea, a
feeling, a genre itch — possibly nothing more. Output: a **locked story bible** that the whole
factory downstream can eat. Ziptide's bible is the worked example throughout — not because it
is sacred, but because it demonstrably produced a game where story, map, and gameplay are one
thing. When the LLM game generator exists, this document is its first stage.

---

## PART A — THE TEN LAWS (reverse-engineered from what made Ziptide's bible work)

**A1 — The fantasy contains the verb.** The logline must embed the thing the player's hands
DO. *Ziptide:* "a contract technician takes a job repairing a derelict gate network" — repair
and salvage are IN the premise, so every mechanic is fiction and every fiction is playable.
Test: circle the protagonist's job in your logline; if it isn't a playable verb, the story is
a movie, not a game.

**A2 — Gameplay↔fiction unity.** Every core loop gets an in-world reason, written down
(Ziptide bible §5: repair = maintaining the cage; credits = the leash; non-lethal = you're a
technician and the enemies are malfunctioning, not evil). This is the single strongest defense
against ludonarrative rot — when a mechanic needs no excuse, playing IS storytelling. Test:
a two-column table, mechanic ↔ fiction line; no empty cells.

**A3 — The revelation ladder.** The big truth is dispensed one notch per chapter, and each
notch can be said in one breath (§6: *it's a job → it's a cage → someone built it → it's alive
→ it's waking → the cage has a conscience → there's an outside → we are someone's archive →
what should a made world do?*). The ladder IS the content structure — chapters, gating, and
pacing fall out of it for free. Test: write the ladder as N one-liners; each must
recontextualize the previous without contradicting it.

**A4 — Layered truth.** Under the surface spine sits an identity layer that the endgame
reveals (`THE_TRANSMISSION.md`: everything Cal "discovers" she once knew). The law within the
law: **the deep reveal recontextualizes, never invalidates** — a second playthrough must make
every earlier scene MEAN more, not make the player feel cheated. This buys replay value at the
writing desk, for free. Test: pick five early moments; write what each means before and after
the reveal; if any "after" is a shrug, the layer isn't load-bearing.

**A5 — One moral question; factions are answers; endings are the player's answer.** Ziptide:
*"Is it kinder to keep a made world asleep and safe, or wake it and risk it being unmade?"*
Every faction is a position (Guild: break out · Sable: tear it down · Wardens: enforce sleep ·
RILL: remember and choose), and the 4 endings are the question handed to the player. Theme
stops being decoration and becomes choice architecture. Test: state the question; place every
faction and every ending on it; anything unplaceable is either cuttable or a second question
(pick one question).

**A6 — The companion is the delivery vehicle.** Story arrives ambiently through a character
who is with the player's body, not through cutscenes (games are played in first person;
narration is what happens when writers give up). The Ziptide relationship rules travel with
the law: ambient lines not just plot lines · **a joke, then a real moment, then a joke** ·
a memory with its own timeline (FollowUp — the companion is still thinking about it) · the
player character gets a specific HABIT the companion can push against (Cal scolds machines;
RILL keeps the log). Test: the companion has ≥3 lines about nothing plot-critical for every 1
plot line; the protagonist has one habit a stranger could describe.

**A7 — Devices that generate content slots.** The bible declares recurring devices that
MANUFACTURE per-world story obligations: one mystery object per world (pays off later), the
wreck-log second story (read in order = a hidden narrative), the Signal meter (the world
reacting to play), environmental readables. At scale (Ziptide: 80 worlds) this is the
difference between "write 80 stories" and "fill 80 well-shaped slots." Test: each device
names its per-content-unit slot AND its payoff mechanism; a device with slots and no payoff
is homework, not story.

**A8 — The two-audience law.** A kid plays the surface ("fix robots on cool planets"); an
adult catches the depth (consciousness, captivity, what we owe the things we make). Neither
audience is condescended to; the depth is carried by implication, not content rating. Test:
describe each chapter twice — once to a 6-year-old, once to a 40-year-old; both must sound
like a game they'd want.

**A9 — Canon lock + amendment discipline.** The bible ends OPEN questions on purpose (listed,
owned) and locks everything else with a dated sign-off; changes go through the same gate,
never silent drift; an honor list protects prior canon. Story is treated like code: versioned,
audited (§9's hook→payoff cohesion checklist), and immune to enthusiasm-driven mutation at
3 a.m. Test: the bible's last section is three lists — LOCKED / HONORED / DELIBERATELY OPEN —
and nothing exists outside them.

**A10 — Structure-as-data.** The story is WRITTEN in shapes a pipeline can eat: every world
carries a header tuple (`Biome · hazard · Faction · CompanionState`), every world doc fills
the same template sections mapping fiction → buildable assets (machines, crops, enemies,
missions, audio), flags have names, beats have ids. This is what makes "the game builds
itself" reachable: the bible is the first genome, not a PDF the team admires. Test: a script
could parse world count, per-world hazard, and every named flag from the bible corpus without
a human explaining it.

## PART B — THE CRAFT IMPORTS (established story knowledge, game-adapted)

- **Want vs. need, under the surface.** The protagonist's stated want is small and mercenary
  (money, passage, get unstuck — Cal); the need is the theme's answer. The gap between them
  IS the arc, and keeping the want blue-collar keeps the player's early goals honest.
- **Mystery-box discipline.** Plant freely, but every hook lives on a payoff ledger (the
  bible's cohesion checklist) — and the audit runs before lock. An unpaid hook found at the
  audit is either paid, cut, or explicitly moved to the OPEN list. Mystery without a ledger
  is debt.
- **Implication over exposition — the believability trick.** The world proves its history by
  wear, not by speeches (STORY_AND_HOOKS: the opening world reads believable because things
  are half-broken, repurposed, signed by absent people). One rule of thumb: if a lore fact
  can be a PROP, it must not be a paragraph.
- **The emotional arc is authored first.** Before beats, write what the player should FEEL
  per act of the opening (curiosity → competence → unease → awe). Beats serve feelings, never
  the reverse — this is why "a joke, then a real moment, then a joke" works: it's an
  emotional-arc rule, not a dialogue tip.
- **The contradiction law.** Every named character who appears twice contradicts themselves
  at least once (believes the mission AND fears its cost). A character who is only ever
  consistent with their faction is a mechanic wearing a nameplate.
- **Restraint as a feature.** Villains who never appear (the Observers watch and do nothing —
  once), silence that is authored, the biggest line saved for the biggest moment. Dread and
  awe are budgeted resources exactly like draw calls.

## PART C — THE METHOD (ordered; each step ends at a checkpoint test; pipeline style)

1. **The seed → premise + verb.** Write the logline until A1 passes. *(Test: A1.)*
2. **World logic.** One page: how this world works, why it's in trouble, why the
   protagonist's VERB is the answer to that trouble. *(Test: A2 table drafts itself from
   this page or the logic is decorative.)*
3. **The moral question.** One sentence, genuinely two-sided (a question with an obvious
   answer is a lecture). *(Test: A5 placement works for ≥3 factions you can imagine.)*
4. **The truth layers.** Surface spine paragraph + identity-layer paragraph + the
   recontextualization list. *(Test: A4's five-moments exercise.)*
5. **The revelation ladder.** N notches (one per planned chapter). *(Test: A3 monotonic,
   one-breath each.)*
6. **The devices.** 3–5 recurring devices with slots + payoffs. *(Test: A7.)*
7. **Characters.** Protagonist (want, need, HABIT) · companion (arc states, relationship
   rules) · faction leads (position on the question + their contradiction). *(Tests: A6,
   B-contradiction.)*
8. **Tone charter.** The two-audience descriptions + content bars + what humor sounds like
   here. *(Test: A8.)*
9. **The bible draft.** Fill `STORY_BIBLE_TEMPLATE.md` — at this point it is assembly, not
   invention; every section's content already exists from steps 1–8.
10. **The cohesion audit.** Run the payoff ledger; place every mechanic in the A2 table;
    parse-test the data shapes (A10); run the whole rubric (Part D). Fix or explicitly OPEN
    every failure.
11. **CANON LOCK.** Dated sign-off by the game's owner; the three lists (A9) written; from
    here, amendments only. **CHECKPOINT: Canon Lock** — the same ceremony as every pipeline
    checkpoint: entry criteria (audit clean), evidence (rubric scores), verdict, signature.
12. **Handoff.** The locked bible feeds the Genesis Interview (derived / open / masquerading
    passes — HANDOFF rb57) → `PIPELINE.md` Stage 0/1. The pre-pipeline ends here.

## PART D — THE QUALITY GATES (how you know it's good BEFORE building)

**The rubric (score each 0–2; the lock needs no zeros and ≥16/20):** A1 verb-in-logline ·
A2 no-empty-cells · A3 ladder monotonic/one-breath · A4 recontextualization holds · A5
everyone placeable on the question · A6 companion ratios + habit · A7 devices have payoffs ·
A8 both audiences want it · A9 three lists exist · A10 parse test passes.
**The iteration loop** (imported from Ziptide's Haiku Story Workshop): distill each chapter
to ONE line of essence; if the distillation is boring, the chapter is boring — revise at the
distillation level FIRST, then re-expand. Cheap passes beat precious drafts.
**The honest limit (Class-9):** these gates prevent every known failure class of game
stories — verb-less premises, unpaid hooks, mouthpiece characters, exposition dumps,
theme-as-decoration. They cannot prove the story is MOVING. That is discovered the way
Ziptide discovered RILL: by playing it, watching a kid meet it, and listening for the quiet.
The gates buy you the right to that test with everything structural already sound.
