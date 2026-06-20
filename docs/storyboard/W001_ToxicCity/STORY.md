# W001 — Toxic City: Story & Job (the "first contract")

**World:** ToxicCity (the new dedicated blueprint scene). **Role in the arc:** onboarding / first
contract. Self-contained — once Cal ships out, she does not return here. Keep all the big lore
*light* (see `docs/design/STORY_AND_HOOKS.md` for the spine: Cal is not human; the "containment
network" is secretly a prison; Earth is the end-game reveal; RILL is waking).

> Everything here is built to be **changed without code**: the beats are `JobDefinition` step assets,
> all spoken/printed lines live in data (ObjectiveBoard text + a RILL line block), and story state is
> `PlayerProfile.flags`. Rewrite the story by editing assets, not C#.

---

## Premise (local, low-stakes)

**Cal** is a contract repair-tech and small-time salvager. Her blue-collar rig is **berthed at the
Toxic City shipyard**, and she's short on the **passage credits** she needs to undock and move on.

Toxic City is the flooded lower tier of a worker world: rusted mid-level walkways over **green toxic
canals**, oligarch glass towers gleaming above, alien glyphs on everything you can't read. People
live and work here; you feel the class divide without a word of exposition.

Her handler, the **Wake Guild**, pushes a contract to the dock's **Dispatch** kiosk. A local fixer —
the **Dockmaster** — is paying a bounty: the district's **maintenance drones have gone feral** around
the canals and a **signal-relay node** has dropped offline, which has frozen cargo clearance (Cal's
ship can't leave until the relay is back up). 

**Why Cal takes it:** money, and she physically can't leave until the relay's fixed. Mercenary,
ordinary, no destiny required. That's the point.

The **one mystery seed** (pays off worlds later, ignore for now): when Cal re-seats the relay it comes
up *wrong* — a signature that doesn't match local tech. RILL notices. Cal shrugs it off and ships out.

---

## Cast

- **Cal** — the player. Never call her human; never call her alien. Let it sit. (Reflection/hands/
  how NPCs address her are future seeds — not in W001.)
- **RILL** — Cal's half-dormant companion AI. Dry, glitchy, occasionally too perceptive. Boots with
  *"Systems nominal. I think."* In W001 it's **Dormant→Stirring**: short functional lines + one
  unprompted observation at the relay.
- **The Dockmaster** — local fixer / quest-giver flavor on the Dispatch kiosk. Gruff, transactional.
  Not important; a face for the bounty. (No dialogue system yet — represented by the DispatchKiosk +
  ObjectiveBoard text.)
- **The Wake Guild** — Cal's employer/handler. Trust them now; doubt them later (Chapter 2).

---

## Beat sheet → maps 1:1 onto existing Job step types

| # | Beat | Player learns | Job step type | Marker / target |
|---|------|---------------|---------------|-----------------|
| 1 | Spawn at the shipyard plaza; orient; head to Dispatch | look / move | `GoToMarkerStep` | Dispatch plaza |
| 2 | Take the Dockmaster's contract at the kiosk | interact / accept | (DispatchKiosk → StartJob) | DispatchHall |
| 3 | Clear the feral drones along the boulevard/canals | aim / fire / dodge stun bolts | `DisableDronesCountStep` | Patrol_Market, Patrol_Canal |
| 4 | Enter the relay building; re-seat the node (it comes up wrong) | interact | `DeliverToSocketStep` or interact | `relay_node` (RelayVault interior) |
| 5 | Return to the shipyard; get paid; ship out (never return) | travel | `GoToMarkerStep` + travel door | Shipyard berth / travel door |

**RILL lines (data — edit freely):**
- Boot (beat 1): *"Systems nominal. I think."*
- Beat 3 first kill: *"Maintenance units shouldn't bite back. Noted."*
- Beat 4 relay (the seed): *"That's not a maintenance handshake. That signature's older than this
  district."*
- Beat 5 ship-out: *"Berth's clear. Whatever that node was talking to — it wasn't us."*

**Story flags set (PlayerProfile.flags):** `toxiccity_contract_taken`, `toxiccity_relay_repaired`,
`rill_first_doubt`, `toxiccity_complete`.

**Bounty payout:** on `toxiccity_complete`, grant passage credits → `PlayerProfile.resources`
(requires a reward field on `JobDefinition` — Architect lane; V1 can ship narrative-only if that
slips, see HANDOFF).

---

## How this maps to what's already built (no new systems for the layout)

- **Scene/layout:** `ToxicCity` is generated from `ToxicCityLayout.asset` (`CityLayoutDefinition`) by
  `ScenePatcherToxicCity`. Districts: **Shipyard** (berth + static ship), **Dispatch** (spawn +
  DispatchHall job-giver interior), **Plaza** (oligarch + garden towers), **Market** (combat drones),
  **CanalRow** (RelayVault mission interior, over the canals).
- **Combat (beat 3):** combat drones in the `Patrol_Market` / `Patrol_Canal` zones (Drone Combat V1 —
  non-lethal stun bolts). Tutorial trio near Dispatch stays passive.
- **Jobs (beats 1–5):** `JobDirector` + `DispatchKiosk` + `ObjectiveBoard` are placed by the patcher;
  the 5-step `JobDefinition` is authored as data (see "Author checklist" below).
- **Exit (beat 5):** `WorldTravelStation` by the shipyard berth (the eventual "leave on your ship"
  spot once flight lands; for now it's the travel door).

## Author checklist (to finish the playable contract)
1. **(Architect lane — claim first)** Author `ToxicCity_Contract` `JobDefinition` with the 5 steps
   above + a reward (passage credits). Add it to `ToxicCity_WorldPack.jobs`.
2. **(T-Dog)** Point `DispatchKiosk.jobIndex` at it; fill `ObjectiveBoard`/RILL text from the lines
   above. Wire the `relay_node` interior interactable to the `DeliverToSocket`/interact step.
3. **Verify on-device** the full beat 1→5 loop, then set `toxiccity_complete`.

## Changeability notes (so this is a template, not a one-off)
- Reorder/retune beats → edit the `JobDefinition` steps.
- Rewrite the narrative → edit this doc + the ObjectiveBoard/RILL text data.
- Re-layout the city → edit `ToxicCityLayout.asset` (districts/connections/canals/drone zones).
- Cut Toxic City entirely → nothing else depends on it; flags are namespaced `toxiccity_*`.
