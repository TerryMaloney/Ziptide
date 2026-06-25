# W004 — The Broadcast Tomb · Story & Build README

> Inherits meta from `../STORY_BIBLE.md`; honors `MASTER_BUILD_PLAN` §12. Template: `../_WORLD_TEMPLATE.md`.

**Status:** designed  ·  **Chapter:** 1 (capstone)  ·  **Biome/Type:** Interior · Static
**Faction:** none  ·  **One-line identity:** *a dead broadcast station full of dark screens — where the drone on your shoulder asks the question that breaks the game open.*

## 1. Role in the arc
**Chapter 1 capstone.** A horror-quiet interior; no combat, all dread. Restoring the broadcast spine
unknowingly amplifies the **containment signal** — and triggers RILL's first real, unprompted question.
**RILL beat (locked):** *"What are you carrying that requires containment?"* **RILL:** Dormant → first stir.
**Signal:** → Threshold 1 locked.

## 2. Premise (local stakes)
The last paid stop before Cal can leave the district: reboot a derelict broadcast station so cargo signals
route again. The station died mid-transmission decades ago; every screen is dark but one, which flickers an
image of **a sky that isn't from any world Cal has seen**.

## 3. The mystery object
The **final broadcast**, recoverable from the spine: it's **addressed to "the watchers"** — not to anyone
inside the network. The first time the outside is named, before anyone understands it.
*→ This is also the **first Transmission fragment** — it reads as garbled junk now and only de-garbles
later (see `../THE_TRANSMISSION.md` §3). What's hidden in the static is Cal's own voice.*

## 4. Planet physics & biome → `BiomeDefinition`
- `hazardType`: **static** (discharge arcs damage/scramble gear near live panels; forces careful pathing).
  `id`: `broadcast_tomb`. Feel: sealed, dark, silent, claustrophobic; flashlight + static-dodge.
- `nativeResourceIds`: `memory_shard`. `nativeCreatureIds`: none (dread, not combat). `ambientTint`: dead grey.

## 5. Sky & atmosphere → `VisualThemeProfile`
- No sky — the **wall of dead screens** is the vista. One flickers an alien sky (foreshadow). `groundTint`:
  ash-grey; heavy fog/dark; static-blue flashes as the only motion.

## 6. Machines & build/repair → `MachineDefinition`
- Signature: the **broadcast spine** — a multi-step repair (route power through static-dangerous junctions).
  *In-fiction it amplifies the containment signal* — the player thinks they're just turning the lights on.

## 7. Crops & resources → `ResourceNodeDefinition`
- `keyResource`: **Memory Shard** (first of RILL's memory-shard items; ties to Ch.3's `C3_W013_MEMORY_SHARD`).
  No mining/crops — this is a story/atmosphere world.

## 8. Weapons / gear found
- **Signal-dampener mod** (reduces static damage; later useful vs Pattern bleed). No new weapon — this world
  withholds combat on purpose.

## 9. Abandoned ship / station
- **The station *is* the wreck.** Logs = the staff's final broadcast as they realized what they were
  transmitting. (Waker-thread #4 — first time a log mentions "the watchers.")

## 10. Alien enemies → none
- Deliberately **no enemies** — the tension is the point (a pacing valley after W001–W003 combat). Static
  arcs are the only "threat."

## 11. Missions → `JobDefinition`
| # | Beat | Learns | Step | Marker/target |
|---|------|--------|------|---------------|
| 1 | Enter the dark station | dread / flashlight | GoToMarker | `tomb_entry` |
| 2 | Route power through the static junctions | static hazard | GoToMarker (×2 nodes) | `junction_a/b` |
| 3 | Recover the final broadcast (the Memory Shard) | the "watchers" hint | Collect (memory_shard) | spine |
| 4 | Restore the spine — RILL asks its question | the screw turns | GoToMarker | `broadcast_core` |

- **Reward:** credits + Memory Shard. **Completion flags:** `C1_WAKE_GUILD_INTRO`, `W004_COMPLETE`,
  `SIGNAL_THRESHOLD_1`. **RILL beat (6–8 lines, signature-lite):** building to *"What are you carrying that
  requires containment?"*

## 12. Map & placement → world-kit (interior)
Linear-with-branches: entry → two junction rooms (static puzzle) → the screen wall → broadcast core. Spawn
at entry; the one flickering screen as a deliberate sightline draw. Tight, dark, readable.

## 13. Audio → `AudioProfile`
Near-silence + room tone; static crackle; a dead-carrier hum that resolves into RILL's voice for the
capstone line. No music until the question lands, then a single low note.

## 14. Build recipe & cross-links
Build via `../../systems/WORLD_BLUEPRINT.md` (interior kit). Canon §12 W004 · Meta `../STORY_BIBLE.md` ·
VO: the locked W004 RILL beat (`MASTER_BUILD_PLAN` §5.2 #2). Changeable-without-code: layout/jobs/RILL/screen
content = data; static-hazard feel is a small patcher/runtime tune.
