# ZIPTIDE — Storyboard Hub

The single place where **creative** (Terry + wife + Gemini: ships, factions, lore, "why") meets
**code** (TDog + Architect: structure, data, interfaces). Creative brings designs and the *reasons*
behind them; the coding agents translate those into data assets + interfaces and slot them into the
storyboard. Established 2026-06-16.

> **Workflow (how we work together):**
> 1. **Creative** produces a concept: an AI image + the lore (what the ship/world/faction is, why
>    this planet/species built it this way, what it does beyond looking cool).
> 2. **Coding (TDog/Architect)** translates it: which `Definition`/`Blueprint` assets, which kit,
>    which interfaces/mechanics — and files it under the right sub-storyboard here.
> 3. The result is **one overall storyboard (the spine)** + **sub-storyboards per planet / mission /
>    faction**, all consistent with the architecture so either coding agent can build any piece.

---

## The spine (overall storyboard)
Cal — an **alien contract technician** (not human; Earth is earned near the end, see
`../design/STORY_AND_HOOKS.md`) — works the **Ziptide containment network**. The arc scales:

1. **On-foot slice** — Toxic Venice (W001): move, shoot, grab, first job, RILL wakes. *(building now)*
2. **First flight** — Cal's scavenger ship; a short guided hop teaser, then real travel. *(designed)*
3. **Open exploration** — many worlds, idle economy (mine/garden/salvage), each with its own biome,
   creatures, and the **faction** whose engineering that world shaped. *(designed)*
4. **Grand strategy / 4X endgame** — async "Risk on top of the game": duel for planets, leaderboards,
   no pay-to-win. *(designed, last)*

## Sub-storyboards (the index)
| Sub-storyboard | Covers | File |
|---|---|---|
| Ships & Factions | Cal's starter ship + the 3 alien faction design pillars + world-swap transitions | [`SHIPS_AND_FACTIONS.md`](SHIPS_AND_FACTIONS.md) |
| *(per-planet — add as authored)* | one planet/mission each: beats, biome, faction, ship, creatures, job chain | `worlds/<Wxxx>_<name>.md` |

**Convention for a new planet sub-storyboard** (`worlds/Wxxx_<name>.md`): vibe + why-this-world;
biome → surface family (`../ART_AUDIO_CONTENT_ARCHITECTURE.md`) + economy mechanic; the resident
faction + its ship; native creatures; the job chain (reuse the World Recipe in
`../design/LEVEL1_TOXIC_VENICE.md`); the one mystery object (story spine); RILL beats.

## Anchored to existing design (don't duplicate — cross-reference)
- Story spine + tone: `../design/STORY_AND_HOOKS.md`
- Ship system (model, cockpit, blueprint pipeline): `../design/SHIP_SYSTEM.md`
- Big systems (economy, creatures, worlds, 4X): `../design/SYSTEMS_ARCHITECTURE.md`
- Controls + flight: `../design/CONTROLS_AND_FLIGHT.md`
- Art surface families + kits: `../ART_AUDIO_CONTENT_ARCHITECTURE.md`, `../project_art_plan/`
- The reusable build process: `../design/LEVEL1_TOXIC_VENICE.md` (the World Recipe)

## Build gating (so creative knows what's "now" vs "later")
Everything ship/faction is **design-now, build-later**. Build order stays: finish the on-foot
Level-1 slice + wire the save/idle loop first (`../WORKLIST.md`), *then* the ship slice, *then*
factions/worlds at scale. Creative can race ahead on concepts; code lands them in roadmap order.
