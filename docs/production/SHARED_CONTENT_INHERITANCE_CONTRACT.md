# SHARED CONTENT INHERITANCE CONTRACT

**Status:** CANONICAL TARGET · enforcement implementation gated by the current M0 Quest verdict
**Written:** 2026-07-27
**Purpose:** ensure every eligible campaign upgrade automatically reaches Arena, Horde, Tidefront, future co-op and later side games without duplicate content or parallel systems

## 0. The law

> **The campaign owns content identity. Side games own rules, arrangements and balance adapters.**

A main-game weapon, creature, biome, architecture kit, hazard, ship, cosmetic, audio cue or VFX vocabulary must not be copied into a side mode as a second asset family. The side mode references the same stable identity and supplies only the context it legitimately owns.

Examples:

- the Taser has one item identity, one visual root, one grip, one feedback stack and one skin family; Arena may use a PvP balance profile, but not a copied Taser asset;
- the W002 swarmer has one creature identity/body/tell/animation family; Horde supplies wave and difficulty data, not a second swarmer;
- the ToxicCity material/architecture/sky family is the same family used by any ToxicCity-themed arena or Tidefront mission;
- Tidefront’s W010 node and mission use the actual W010 identity, hazard and scene, not a strategy-only duplicate world;
- a weapon skin, ship livery or room skin is owned/equipped once and appears everywhere that content appears.

A side-mode exception must be explicit, narrow and justified. Silence means inherit.

---

## 1. Surviving shared owners

| Content responsibility | Canonical owner | Side-mode adapter may own | Side mode may not own |
|---|---|---|---|
| Item identity and physical shell | `ItemDefinition` / `ItemFactory` / `ItemRuntime` | availability, spawn/pad rules, mode balance profile | copied mesh, grip, collider, muzzle, moving parts, skin or feedback implementation |
| Weapon/tool effect | existing runtime modules and shared target interfaces | PvP outcome numbers, friendly-fire/team restrictions | second firing/effect runtime for the same item |
| Creature identity | `CreatureDefinition`, behavior owner, Forge/import visual applier | spawn wave, difficulty, allegiance, objective role | duplicate creature body or divergent tell/animation |
| World/biome identity | WorldSpec/WorldPack, layout, theme, kit and hazard definitions | arena crop/layout, conquest node/mission context | copied world scene, copied palette/material family or alternate lore truth |
| Ship identity | ship definitions/loadout/flight owners | Tidefront strategic score/token representation | parallel flight model, copied hull visual, alternate module stats |
| Powers/augments | shared effect registry + equip/save owner | mode cooldown/magnitude profile when declared | copied effect code or separate inventory |
| Economy/reward | existing ledger/profile/reward chokepoints | mode payout formula and ledger source | second wallet or duplicate unlock record |
| Cosmetics/skins | `CosmeticDefinition`/locker and future shared skin application | mode-specific availability restrictions if required | separate ownership/equip state |
| Audio/VFX | stable event/effect ids and shared services | context mix/priority | copied clips/effects attached directly to side-mode objects |
| Save/progression | `SaveSystem`/profile/world overlays | mode session state | alternate permanent profile truth |

---

## 2. Weapon and tool inheritance

### Shared layers

Every eligible mode receives the same:

- stable `itemId`;
- base Definition fields;
- visual child and material zones;
- grip/attach/muzzle/sight/moving-part sockets;
- one-hand/two-hand/throw/deploy interaction module;
- recoil/weight/smoothing behavior where applicable;
- haptic/audio/VFX event identities;
- target-effect tags;
- cosmetic/skin ids;
- holster/travel/save identity.

### Context adapters

A mode may add a data-only context profile such as:

- campaign charge/resource cost;
- Arena damage/armor/charge/cooldown;
- Horde crowd-control duration or wave scaling;
- Tidefront mission modifier contribution;
- bot preference/usage weights;
- allowed/disabled status for a particular mode.

The adapter references `itemId`. It never recreates presentation or interaction data.

### Required reconciliation

The current `ArenaWeaponDefinition` path is useful for arena-specific kinds and balance, but it must not become a visual/interaction fork. The production target is:

- one stable campaign/shared item identity where the same named item exists in multiple modes;
- mode-specific rule profiles or thin adapters;
- explicit arena-only identity only when the item is genuinely not a campaign item.

Static Net, Sonic Thumper, Prism Beam, Breaker Blade and Tide Pike are campaign-story items and therefore should inherit campaign upgrades into Arena/Horde automatically.

---

## 3. Creature inheritance

### Shared creature package

A creature family owns:

- stable creature/species id;
- body/skin/LOD package;
- skeleton and animations;
- gameplay tells;
- target interfaces and non-lethal states;
- disable/capture/salvage drops;
- sound/VFX events;
- ecology/nest data;
- surface and space eligibility where relevant.

### Mode use

- **Campaign:** authored ecology, encounter, story and world-state role.
- **Horde:** wave composition and difficulty modifiers over the same package.
- **Arena mutators:** optional creature hazard/objective using the same package.
- **Tidefront:** defense/attack mission role and strategic trait reference.
- **Future co-op:** networked state adapter over the same canonical behavior outcomes.

A new imported hero creature must upgrade every eligible use because the root species id remains unchanged.

---

## 4. World, architecture and hazard inheritance

### Campaign owns

- world id and lore;
- biome and architecture-family composition;
- material/surface families;
- sky/vista identity;
- hazard definition;
- POI/prop kits;
- creature roster;
- resource identity;
- soundtrack/ambience identity.

### Arena owns

- competitive layout and sightline decisions;
- spawn pairs;
- objective zones;
- weapon/augment pads;
- breakable-wall and mutator placement.

Arena references campaign biome/kit/sky/hazard ids. It does not rebuild the art family. When W002’s cavern kit or W010’s flood presentation improves, `Arena_Cistern` and `Arena_TidalArray` inherit that family on regeneration.

### Tidefront owns

- node ownership, adjacency, production and strategic modifiers;
- battle/fleet/defense state;
- mission offer and outcome tilt.

Tidefront references the actual story-world identity and travels to the actual target world or its canonical space route. It does not clone a simplified mission scene unless a declared low-cost tactical variant is a separate content identity.

---

## 5. Ship and space inheritance

The Scrapper, MK2, faction ships, Warden vessels and derelict families should have one visual/identity source across:

- campaign flight;
- space salvage/combat;
- Tidefront vessels and mission encounters;
- hangar/fleet display;
- future co-op presence;
- cinematics/arrival silhouettes.

Tidefront’s pure vessel catalog may use strategic stats that are not the player flight stats, but the vessel entry should reference a canonical ship/faction/visual family id. A “Pulse Frigate” token and a close-range Pulse Frigate cannot drift into unrelated designs.

Space enemies use the same disable/salvage vocabulary as campaign and side missions. Mode-specific AI/quantity is an adapter.

---

## 6. Powers and augments

One shared augment/effect identity supplies:

- effect id and tested effect rules;
- visual/audio/haptic events;
- equip slot and save ownership;
- cooldown/duration defaults;
- valid targets and comfort rules;
- skin/visual identity.

Context profiles may vary cooldown or magnitude within explicit bounds:

- campaign progression/crafting;
- Arena pickup/drop-on-down;
- Horde wave reward;
- Tidefront temporary mission buff;
- future co-op team rules.

Active powers never receive a campaign implementation and a separate PvP implementation. Shared effect logic first, translators/adapters second.

---

## 7. Art, Tripo and skin inheritance

External or Tripo geometry enters once under a stable visual id. All modes receive it through the same applier/registry path.

Every skinnable base declares:

- clean UVs;
- material zones;
- fixed identity zones;
- attach points;
- default/fallback visual;
- LOD package;
- skin compatibility.

A skin is owned once. The same Taser skin must appear in campaign, Arena, Horde and co-op. The same ship livery must appear in flight, hangar and Tidefront close-up presentations. Room skins affect every instance of the same room shell unless the skin explicitly targets one variant id.

Side modes may use lower LODs or impostors at distance. That is the same asset package, not a different design.

---

## 8. Audio and VFX inheritance

Every reusable action references stable events, for example:

- item draw/holster;
- fire/charge/reload/mechanical cycle;
- hit-confirm/material impact;
- disable/capture/salvage;
- creature idle/warning/disable;
- hazard enter/escalate/clear;
- ship engine/boost/impact/alert;
- Ziptide departure/arrival;
- job accept/progress/complete;
- augment activate/expire.

Campaign and side modes use the same source events. A mode may change mix priority, ducking or crowd limits; it may not attach an older one-off clip and bypass the event layer.

---

## 9. Proposed enforcement manifest

A small source-controlled manifest should declare each shared content id and its eligible consumers. It is a verification/index artifact, not a new runtime manager.

Example shape:

```json
{
  "contentId": "taser_dart_gun",
  "kind": "item",
  "consumers": ["campaign", "arena", "horde", "tidefront_mission", "coop"],
  "canonicalDefinition": "taser_dart_gun",
  "canonicalVisual": "taser_gun_mk1",
  "contextProfiles": {
    "arena": "pvp_taser",
    "horde": "horde_taser"
  },
  "exceptions": []
}
```

The manifest does not duplicate stats or content. It proves that every declared consumer resolves through the canonical ids.

---

## 10. Required audits

Names below are target audit tags; implementation may fit them into existing validators rather than creating unnecessary classes.

1. **`SHARED_CONTENT_MISSING_CONSUMER`** — declared eligible mode cannot resolve the canonical id.
2. **`SIDE_MODE_DUPLICATE_VISUAL`** — side-mode asset copies or substitutes a shared visual without an exception.
3. **`SIDE_MODE_DUPLICATE_EFFECT`** — same named item/power has a parallel effect runtime.
4. **`SIDE_MODE_STALE_PRESENTATION`** — consumer resolves an older visual/audio/VFX version than canonical content.
5. **`ARENA_STORY_KIT_DRIFT`** — themed arena does not reference the campaign kit/theme/hazard family it claims.
6. **`HORDE_CREATURE_DRIFT`** — Horde creature id/body/tell does not match the campaign species.
7. **`TIDEFRONT_WORLD_DRIFT`** — conquest node/mission world identity, biome or resource conflicts with WorldPack truth.
8. **`COSMETIC_MODE_DRIFT`** — ownership/equip differs across modes for the same cosmetic.
9. **`CONTEXT_PROFILE_ORPHAN`** — a mode profile references missing canonical content.
10. **`SHARED_CONTENT_EXCEPTION_STALE`** — an exception remains after its reason disappears.

Every audit must have a deliberately broken fixture proving it fires.

---

## 11. Legitimate exceptions

An exception is valid only when the content is actually different, such as:

- an Arena-only abstract training weapon with no campaign counterpart;
- a miniature Tidefront table token using a deliberately simplified LOD package;
- a campaign story prop that has no combat or side-mode meaning;
- an ending-only world state not exposed to replay modes;
- a competitive balance rule that must differ from campaign progression.

The exception record must state:

- canonical id involved;
- consumer excluded or overridden;
- why inheritance would be incorrect;
- what is still shared;
- owner and review trigger.

“Easier to copy it” is not a valid exception.

---

## 12. Post-headset implementation order

1. Reconcile Taser, Gravity, Pistol, Static Net, Thumper, Prism, Blade and Pike identities across campaign/Arena/Horde.
2. Add the source-only inheritance manifest and read-only validator.
3. Prove one weapon visual/feel upgrade appears in campaign and Arena from the same source.
4. Prove one creature visual/tell upgrade appears in campaign and Horde.
5. Prove one W002/W010 kit or hazard upgrade regenerates its themed Arena.
6. Prove one campaign-world change updates the Tidefront node/mission source without duplicate authoring.
7. Extend to augments, ships, cosmetics, audio and VFX.
8. Make inheritance validation part of every content-family Definition of Done.

No step changes tomorrow’s certified recovery artifact. This contract becomes executable only after the M0 verdict authorizes the affected lanes.
