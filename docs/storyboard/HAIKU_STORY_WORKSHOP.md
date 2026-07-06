# ✍️ THE HAIKU STORY WORKSHOP — how Ziptide's narrative gets good (opened 2026-07-06)

Terry's directive: "start working with **Haiku** on making the story actually really good — right now
it's super basic." This is the process. Story is the one part where a cheaper, faster model (Haiku)
run as a *drafting* subagent is the right tool: high volume of candidate lines/beats, curated down by
the operator against locked canon.

## The loop
1. **Seed a Haiku subagent** (`Agent`, `model: haiku`) with the canon it must respect, READ-ONLY:
   `docs/storyboard/STORY_BIBLE.md` (locked meta, factions, four endings, gradual-hint ladder),
   `THE_TRANSMISSION.md` (the identity layer), the relevant `CHAPTER_*.md`, and `WORLD_DATA.md`
   (which worlds exist, their fiction, the flag schema). Tell it the delivery systems it may write
   FOR (below) and the tone charter (Halo wonder + Fallout depth; non-lethal, all-ages; dread by
   implication, never gore).
2. **Ask for candidates, not commits:** RILL lines by trigger, per-world micro-arcs (beginning/turn/
   echo), Transmission tier text, ChoiceStation branches, collectible log text. Volume is fine —
   curation is the point.
3. **The operator curates** (this is NOT Haiku's job): cut anything that contradicts the bible, fix
   invented names/world-numbers to canon, keep the best 20-30%, and land it through the real pipeline
   — `RillLineAuthor` (RILL lines → `Resources/Story/RillLines.asset`), `WORLD_DATA.md` + the world
   packs (collectibles/choices/flags), `TransmissionText`/`TransmissionConsole` (tier text).
4. **Canon stays locked.** Haiku PROPOSES; it never edits `STORY_BIBLE.md` or `THE_TRANSMISSION.md`.
   New canon is a Terry decision. Candidate ideas that want new canon go into `docs/additions/STORY_50.md`
   flagged for his call, not into the bible.

## The delivery systems (what story ideas ride — don't invent new ones without a design doc)
- **RILL lines:** `RillLineLibrary` / `RillLineAuthor`, triggers `WorldEnter` / `FlagSet` (new trigger
  kinds are a small ⚙CI task if a beat needs one). ~34 lines exist today — the floor, not the ceiling.
- **Transmission:** `TransmissionProgress` (clarity tiers) + `TransmissionText` + `TransmissionConsole`
  playback; fidelity/decode worsens per tier by design.
- **Choices:** `ChoiceStation` two-option set-pieces + world flags for consequence.
- **Collectibles:** pack `CollectibleSpawnDefinition` with `flagOnCollect` — physical story objects.
- **Environmental:** the world builders (POIs incl. `StoryAnchor`, lighting/sky via VisualThemeProfile)
  — what a place SAYS without text. This is where AAA story lives in VR.
- **VO (art track):** RILL/Transmission voice production is Picasso's `ART_AUDIO_50.md` #8/#9 — text
  first here, voice there.

## First workshop output (this session)
`docs/additions/STORY_50.md` — 50 story ideas drafted by a Haiku subagent, seeded as above. **Curator
note on that file:** it is a raw draft; several rows invent placeholder world numbers (W015, W031…)
and faction names not in the bible. KEEP THE IDEA, FIX THE CANON when pulling — reconcile every world/
faction/ending reference against `STORY_BIBLE.md` + `WORLD_DATA.md` before it touches an asset. The
strongest rows to pull first are the RILL-memory/callback lines (S, cheap, high felt-impact) and the
per-world micro-arc beats.

## When to run it again
Any story-heavy chunk: a new chapter's worlds (draft its RILL beats + collectibles + Transmission tier),
a punch-up pass on flat existing lines, or a faction's texture. One Haiku subagent per chunk, curated,
landed through the pipeline, CI-green.

*Owner: the story track. Opened alongside the Additions Bank on Terry's "work with Haiku" directive.*
