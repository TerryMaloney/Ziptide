# SHIP NAVIGATION MAP TERMINAL
## A clean three-layer VR star chart for eighty worlds

**Status:** PLANNING ONLY. No runtime, scene, asset, package, workflow, save, travel, helm, profile, or certified-checkpoint change is authorized by this document.

**Clarification from Terry:** this document covers only the in-universe ship map used to visualize and select destinations. It does **not** describe or replace the title menu, Home Hub menu, pause menu, settings, profile selection, inventory, or ordinary player menu.

**Product reference direction:** combine the clean, graphic, high-contrast feel Terry likes in Destiny-style destination maps with the nested cluster → system/destination → local-site hierarchy associated with Mass Effect-style galaxy navigation, translated into an original ZIPTIDE visual language and a fixed, comfortable VR terminal.

---

## 1. Product definition

The **Ship Navigation Map Terminal** is a physical navigation surface inside the hero ship.

Its jobs are:

1. let the player understand where known destinations sit in the wider network;
2. keep roughly eighty eventual destinations browsable without displaying eighty equal buttons;
3. preview the identity and current state of a destination;
4. show available landing zones, contracts, orbital sites, stations, derelicts and space-lane activities;
5. select a destination for the canonical helm/cast-off flow;
6. make unlocking a new region feel like discovery rather than a row appearing in a developer list.

It is **not**:

- the title screen;
- a pause menu;
- a generic scene selector;
- a replacement for the contract ledger;
- a second travel system;
- a floating wrist map;
- a minimap;
- a developer warp board in story clothing.

The terminal is the visual navigation half of the hero helm. The contract ledger answers **why should I go?** The navigation map answers **where is it, what is there, and which arrival point am I selecting?**

---

## 2. Canonical ownership laws

The terminal is a projection and selector over existing owners.

- `WorldGating` remains the authority for whether a destination is known, visible, selectable or locked.
- Existing world/destination author data remains the authority for names, summaries, hazards, arrival points and visual references.
- The contract/job owners remain the authority for available work and completion state.
- The future contract ledger remains a derived view over canonical flags and may request that the terminal focus a destination.
- The consolidated hero helm owns the current selected destination.
- `TravelCoordinator` remains the only travel owner.
- `SaveSystem` remains the only profile/disk owner.
- The terminal never loads a scene, grants an unlock, spends a resource, completes a job or writes raw story state.

Required flow:

```text
canonical destination/world data
        +
WorldGating visibility/selectability
        +
contract/ledger derived status
        ↓
ShipNavigationMapModel
        ↓
three-layer terminal presentation
        ↓
SelectedDestinationId + SelectedArrivalId
        ↓
canonical hero helm/cast-off confirmation
        ↓
TravelCoordinator.TravelTo(...)
```

The terminal may select. The physical helm/cast-off control confirms and launches.

No destination tile, planet preview, mission node or launch-looking graphic may call travel directly.

---

## 3. Three-layer hierarchy

The exact final number of clusters is data-driven. Eight clusters containing approximately ten destinations each is the current planning shape, not a hardcoded runtime assumption.

### Layer 1 — Network / cluster view

The top layer shows a small number of major regions rather than all destinations.

Each cluster has:

- stable `clusterId`;
- original name and glyph;
- broad color family;
- simple stylized spatial footprint;
- discovered/unknown/locked state;
- one short identity line;
- progress summary derived from known destinations;
- optional network-route lines showing relationships, not literal astronomical scale.

The view should read in seconds. A player should see five to ten large choices, not a cloud of tiny dots.

Selecting a cluster transitions to Layer 2. Unknown clusters can appear as bounded silhouettes, signal noise or an unlabeled pulse only when the story allows that anticipation. The map must not spoil chapter-locked destinations.

### Layer 2 — destination group view

The selected cluster opens into its destinations.

This layer may contain:

- planets;
- moons;
- orbital stations;
- derelict ships;
- space-lane missions;
- anomalies;
- relay structures;
- temporary story sites.

Destinations use a common node contract but distinct type silhouettes:

- planet/moon: circular emblem;
- station: geometric ring or facility silhouette;
- derelict: broken hull profile;
- space lane: route/ring glyph;
- anomaly: unstable signal mark;
- hidden/unknown: story-approved obscured signal.

Nodes may be arranged along elegant authored arcs, routes or constellation geometry. They are not required to simulate physically accurate orbits.

Focusing a node shows only a compact card:

- destination name;
- type;
- one-line identity;
- current contract stamp/state;
- key hazard or comfort glyphs;
- discovered arrival-site count;
- lock reason when that reason is intentionally player-readable.

Selecting the node transitions to Layer 3.

### Layer 3 — destination / local-site view

The destination becomes the focal object.

For a planet or moon:

- a high-quality stylized globe, disk or shallow-parallax preview occupies the hero area;
- known arrival regions appear as a small authored node list or mapped marks;
- weather/atmosphere/helmet/physics/hazard information appears through symbols first;
- current story, contract and return-state changes may subtly alter the preview.

For a station, derelict or orbital site:

- the focal preview is the site's silhouette, station diagram or orbit card;
- nodes represent docking collars, exterior battle approaches, breached sections or mission variants;
- unexplored interior structure remains hidden rather than spoiled by a complete floor plan.

For a space lane:

- the focal preview shows the route, major hazards, objective type and known exit/return condition.

Layer 3 chooses an `arrivalId` or mission-site identity. It does not launch immediately.

After selection, the terminal returns to or highlights the physical helm confirmation state:

- selected destination clearly named;
- selected arrival/site clearly shown;
- required coupler/key/ship readiness visible;
- one deliberate physical launch action such as the canonical `PUNCH IT` control.

---

## 4. VR interaction model

The player stands or sits at a fixed physical terminal. The map moves inside the terminal; the player's head and rig do not.

### Required input paths

- ray point + trigger select;
- left- or right-hand menu control according to the selected profile preference;
- seated reach at every required control;
- explicit Back/Breadcrumb control on every nested layer;
- physical launch confirmation separate from selection;
- optional direct-touch or grab/pan interaction only after the ray path is proven.

### Focus behavior

Aiming at a node:

- increases its scale slightly within a strict clamp;
- adds a clean ring/outline;
- produces one restrained focus tick after focus stabilizes;
- reveals the compact information card;
- does not select through dwell alone.

This avoids accidental travel and ray-jitter audio chatter.

### Fake seamless zoom

The apparent camera zoom is a content transition inside a fixed terminal volume:

1. selected layer scales and fades toward its chosen node;
2. the next layer fades/scales in from the same visual anchor;
3. breadcrumb/title changes;
4. input transfers only after the next layer is stable.

No world camera, HMD, ship interior or player rig moves. The implementation may use separate pooled layer roots rather than one enormous map object.

Back performs the exact reverse semantic transition and restores the previous focus when practical.

---

## 5. Visual language

The map is clean graphic design embedded in a physical ship instrument.

### Core style

- dark, low-noise field;
- thin architectural lines;
- highly legible silhouettes;
- restrained emissive accents;
- consistent typography and icon language;
- broad negative space;
- no dense metallic frame consuming the screen;
- no photorealistic 3D solar-system simulation;
- no raw developer identifiers;
- no miniature unreadable text clouds.

### ZIPTIDE identity

This cannot be a direct Destiny or Mass Effect replica. ZIPTIDE's original identity comes from:

- tide-route curves rather than conventional star-chart grids;
- crest and gate motifs;
- Architect geometry where story-appropriate;
- salvage-tech irregularities in the Rustbucket's hardware frame;
- RILL's state color used sparingly for focus/guidance;
- destination emblems derived from each world's Forge visual identity;
- route animation suggesting remembered passage rather than literal fuel-line plotting.

### World preview assets

Use a quality ladder:

1. authored flat emblem for every destination;
2. authored hero preview for selected destinations;
3. shallow-parallax or slowly rotating focal piece where it adds value;
4. later story-state variants for important worlds.

Only the selected Layer-3 destination receives the expensive focal treatment. Do not load eighty rotating 3D planets.

The focal preview may be generated from the same canonical art/world recipe that defines the destination, so the map cannot promise a visual identity that the actual world contradicts.

---

## 6. Information architecture

### Always visible

- current hierarchy/breadcrumb;
- Back control when nested;
- current focus/selection;
- selected destination summary;
- clear locked/unavailable/readiness state;
- physical relation to the launch control.

### Layer-2 compact card

Maximum information:

- name;
- type glyph;
- one-line identity;
- status stamp;
- two or three critical hazard/requirement symbols.

### Layer-3 full card

May include:

- destination and arrival name;
- current contract state;
- atmosphere/helmet requirement;
- dominant environmental hazard;
- unusual world-physics indicator when implemented;
- recommended equipment category without hard-locking player creativity;
- known salvage/ecology identity;
- discovered landing sites;
- one concise RILL note or contract-ledger summary.

Do not reduce every destination to a numeric difficulty rating if the game can communicate the actual problem through role and hazard symbols. A danger tier may exist, but it is not the whole identity.

---

## 7. Relationship to other interfaces

### Title/Home Hub

Completely separate. The title experience gets the player into a profile and starts or continues play. It does not expose the eighty-world navigation terminal.

### Contract ledger

The ledger and map share data, not presentation.

- ledger: unfinished work, progress, why return;
- map: spatial hierarchy, destination preview, where to arrive;
- selecting a ledger row may focus the appropriate map destination;
- selecting a map destination may show its derived contract status;
- neither duplicates contract truth.

### Developer warp board

Separate and hidden. The story-facing terminal never exposes raw scene names, test worlds or arbitrary bypasses. Development mode can use the same destination model only through an explicit hidden filter, without leaking into public progression.

### Pause/settings/inventory

Unrelated. Those surfaces remain reachable through their existing player-menu paths and must not be folded into the navigation terminal.

---

## 8. First-hour use

The first hour deliberately exposes only a tiny subset of the final hierarchy.

### Before the artifact key

- the terminal can show the local known network in a restricted state;
- normal first flight/Space Lane content uses only destinations legally available at that moment;
- Toxic City remains the clear selectable work destination;
- future clusters are absent or represented only by approved non-spoiler signals.

### Artifact/key payoff

When the joined key seats in the coupler:

1. the terminal detects a canonical unlock through `WorldGating`/story state;
2. an unknown signal resolves into a real destination or cluster path;
3. the hierarchy visually opens one level beyond what the player previously understood;
4. the first true Ziptide becomes the physical launch into that newly revealed path.

This is one of the first hour's strongest payoff opportunities: the player's universe visibly becomes larger because of something they physically repaired, found, joined and installed.

### W002 return

After the first W002 cycle, the terminal can show:

- W002's settled/current output state;
- the next unresolved signal;
- a clean visual distinction between completed, active, newly available and unknown.

The first hour must not require the player to understand the final eighty-world hierarchy. It teaches one cluster, one selection, one arrival point and one launch confirmation.

---

## 9. Data contract proposal

Names are illustrative until repository archaeology confirms the surviving destination model.

```text
NavigationClusterDefinition
- clusterId
- displayNameId
- summaryLineId
- glyphId
- visualThemeId
- authoredOrder
- visibilityRequirementId
- destinationIds[]

NavigationDestinationView
- destinationId                  // references canonical destination/world identity
- destinationType               // planet, moon, station, derelict, lane, anomaly...
- clusterId
- displayNameId
- summaryLineId
- emblemId
- heroPreviewId
- hazardGlyphIds[]
- requirementGlyphIds[]
- arrivalSiteIds[]
- derivedVisibility
- derivedSelectability
- derivedContractState
- derivedWorldStateVariant

NavigationArrivalView
- arrivalId                     // references canonical arrival/mission identity
- displayNameId
- summaryLineId
- selectionRequirementId
- derivedAvailability
```

These are view-model or author-data concepts, not permission to duplicate existing destination definitions. MAP-0 begins by auditing and adapting the current world/ship helm data.

Stable IDs are identity. All displayed text follows `LOCALIZATION_DECISION.md` and comes from author/library data.

---

## 10. Audio and motion

Starter SFX vocabulary:

- `map_focus_tick`;
- `map_cluster_enter`;
- `map_layer_back`;
- `map_destination_select`;
- `map_locked_deny`;
- `map_route_resolve`;
- `map_launch_ready`.

Rules:

- focus sound fires only after stable focus and respects a cooldown;
- layer transitions are short, quiet and spatially anchored to the terminal;
- locked nodes use restrained feedback rather than an error alarm;
- unlocking a major cluster can earn a larger story stinger;
- no continuous beeping wallpaper;
- all sounds route through the accepted SFX/audio ownership tree;
- visual response always accompanies meaningful audio.

Animation remains inside the terminal. No fast near-face particles, aggressive scanline flicker or full-view flash.

---

## 11. Performance and scaling laws

- Never instantiate all eighty destinations as detailed 3D objects.
- Pool/reuse node visuals and information cards.
- Load only one hero preview at Layer 3.
- Cluster and destination emblems use atlased or otherwise audited low-cost presentation.
- Background routes are bounded line/mesh elements, not particle storms.
- Text count is capped per layer.
- Layer transitions allocate no large transient object graph during interaction.
- A missing preview falls back to the destination emblem and never blocks selection.
- Terminal interactivity appears before optional hero art finishes loading.
- The map must remain usable at the project's slowest supported Quest performance tier.

---

## 12. Bounded implementation sequence

All packets remain post-checkpoint and subordinate to the operator map.

### MAP-0 — repository archaeology and pure hierarchy

- identify the current canonical destination/world/arrival data;
- identify the current two helm surfaces and consolidation target;
- define adapters/view models rather than duplicate definitions;
- pure tests for cluster membership, visibility, ordering, selection and breadcrumb behavior;
- no scene, travel or save changes.

### MAP-1 — three-layer graybox terminal

- fixed physical terminal volume;
- Layer 1/2/3 transitions;
- ray focus/select/back;
- placeholder cluster/destination/arrival visuals;
- no travel call;
- seated/standing and handedness checks;
- PlayMode and visual capture proof.

### MAP-2 — canonical helm and ledger integration

- terminal writes only the consolidated helm's selected destination/arrival state;
- first-hour restriction mode;
- ledger focus request through one explicit selection seam;
- WorldGating-derived hidden/locked/selectable states;
- no duplicate unlock or contract truth;
- full W000 → Toxic City golden-route regression proof.

### MAP-3 — first-hour presentation and launch handoff

- initial cluster/destination art;
- Toxic City and W002 focal previews;
- artifact-key route reveal;
- selected destination/readiness on the physical helm;
- SFX family;
- canonical `PUNCH IT` confirmation remains the only launch action;
- Quest comprehension and comfort campaign.

### MAP-4 — scalable content authoring

- authoring/audit path for clusters, destination types and arrival sites;
- orbital station, derelict, space-lane and anomaly node variants;
- missing/mismatched data audits;
- proof with two clusters and multiple destination types before broad scaling;
- no mass authoring until blind comprehension and performance tests pass.

### MAP-5 — later stateful richness

- world-state preview variants;
- return marks and outputs;
- optional route-history visualization;
- Forge VI integration only after authored worlds establish the quality bar.

---

## 13. Acceptance contract

The navigation map is not done because it can display nodes.

It is accepted when:

1. a first-time player can select Toxic City without instruction;
2. a returning child can find an unfinished known world from the ledger and reach its map selection within thirty seconds;
3. the player can distinguish cluster, destination and arrival layers without reading a manual;
4. Back always restores a valid previous layer/focus;
5. no map interaction travels accidentally;
6. only the physical helm/cast-off confirmation can begin travel;
7. locked and unknown states do not spoil future content;
8. planet, station, derelict, lane and anomaly types are visually distinct;
9. selected focal art resembles the actual destination identity;
10. left/right hand and seated/standing paths work;
11. the terminal remains interactive while optional preview art loads or fails;
12. the full first-hour and recovery golden routes remain green;
13. the map holds the Quest performance budget with a representative full cluster;
14. eighty-destination scale is proven through data/audits, not by rendering eighty detailed objects at once.

---

## 14. Explicit exclusions

This plan does not authorize:

- changing the title-menu design;
- moving profile selection into the map;
- adding settings, inventory or pause functions to the map;
- replacing the contract ledger;
- direct travel from destination nodes;
- a free-floating map attached to the player's face;
- exact simulation of galaxy scale or orbital mechanics;
- loading every destination preview simultaneously;
- exposing developer scenes in public mode;
- a new save owner, travel owner, world-gating owner or mission owner;
- copying Destiny, Mass Effect or any other game's protected art, terminology, layouts or assets.

The target is an original ZIPTIDE navigation instrument with a clean graphic hierarchy and the logistical clarity Terry identified in those references.
