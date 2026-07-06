# 🧩 THE ADDITIONS BANK — 50+ improvement ideas per part (opened 2026-07-06)

**What this is:** Terry's directive — "the game is far too simple, I want AAA." So every major PART of
Ziptide got a swept idea bank: ~50 concrete, buildable improvements each, sized and tagged. This is an
**IDEA BANK, not a directive queue** (same law as `docs/GPT_ADDITIONS/`). Nothing here is scheduled;
operators PULL from it when a track has capacity.

## The files (one per part)
| File | Part | Owner lane |
|---|---|---|
| `GARDEN_50.md` | grow-a-garden depth | 📖 story |
| `INDUSTRY_50.md` | machines / mines / **conveyors + automation** | 📖 story |
| `SPACEFLIGHT_50.md` | flight + the ship | 📖 story |
| `STORY_50.md` | narrative (Haiku-drafted — the story workshop's first output) | 📖 story |
| `WORLDS_50.md` | exploration / traversal / secrets / weather | 📖 + 🏗 |
| `COMBAT_GAMEPLAY_50.md` | weapons, movement tech, enemy pressure | 🎮 + 📖 |
| `MULTIPLAYER_50.md` | arena / modes — the Fortnite bar | 🎮 |
| `TIDEFRONT_50.md` | conquest strategy layer | 🎮 |
| `CREATURES_50.md` | living-world AI / ecology | 📖 |
| `ART_AUDIO_50.md` | **Picasso's file** — Forge families, VFX, adaptive audio, UI | 🎨 |
| `ECONOMY_META_50.md` | progression / quarters / cosmetics / daily loops | 📖 + 🎮 |

## The laws of the bank
1. **Ideas, not directives.** Pulling one = adding a row to your track's `SPRINT_*.md` (+ a HANDOFF
   task envelope if it crosses lanes). Un-pulled ideas cost nothing.
2. **Every idea RIDES THE MACHINE.** Each row names the existing system it extends (WorldSpec,
   ItemFactory, PvpModes engines, CreatureDefinition, ProfileEconomy, the Forge/ArtModuleRegistry…).
   An idea that would require breaking a locked contract (travel law, non-lethal canon, PlayerIndex
   law, rig gotchas, comfort rules, looks-never-stats) gets **redesigned to fit or dropped** — flag
   it in HANDOFF, don't force it.
3. **Row format:** `| # | Idea (concrete) | Size S/M/L | Systems touched | 🎨 |` — 🎨 marks a
   meaningful art/audio component (Picasso fulfills or vetoes the look; the idea's *logic* can still
   ship with a primitive/placeholder first, per the ArtModuleRegistry fallback law).
4. **Append-only, curated at pulls.** Add new ideas at the bottom with the next number; never
   renumber. When an idea ships, strike it (`~~N~~`) and note the commit — don't delete (history).
5. **Sizes are honest:** S ≈ one commit; M ≈ a small sprint chunk (core+tests+wire); L ≈ a
   multi-commit feature (usually its own design doc first).

## How to pull well (for any operator, any model)
- Sort your part's file by what raises Terry's "AAA" read the most per unit effort — usually the
  S/M rows that add *feel* (juice, telegraphs, readouts) before the L rows that add *scope*.
- Batch related S rows into one commit when they touch the same file.
- 🎨 rows: build the mechanic behind a placeholder first (ship the logic CI-green), then hand the
  look to Picasso via `ART_REGISTRY.md` ids — don't block a mechanic on art.

*Opened by the architect (Fable 5 → Opus 4.8 handoff session) on Terry's "50 per part" directive.
Story ideas drafted by a Haiku subagent per the workshop in `docs/storyboard/HAIKU_STORY_WORKSHOP.md`.*
