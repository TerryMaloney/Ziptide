# SPACE POI SPAWN PACKETS — deterministic placement inputs for the Moss orbital loop

**Status:** 🔵 DESIGN + DATA CONTRACT — zero runtime spawning. This packet converts the six
families in `space_poi_catalog.json` into stable future spawner inputs without authorizing a
scene injector or changing the reserved recovery route.

> **Machine catalog:** `docs/design/space_poi_spawn_catalog.json` — CI-enforced; keep in sync (`docs/CATALOG_DOC_RECONCILE.md`).

## 1. Boundary

- `runtimeSpawningAuthorized` remains `false` until the exact c45b1a2 headset verdict and the
  post-verdict implementation gate.
- The catalog describes **what may be spawned and how it is identified**, not a new mission,
  world, salvage, or story engine.
- Mission truth remains in `space_mission_catalog.json`; POI visual/gameplay truth remains in
  `space_poi_catalog.json`; this packet only adds placement identity, deterministic seeds, and caps.
- No device time, frame count, network state, or nondeterministic random source may affect placement.
- Authored landmarks such as the Ziptide gate approach are never procedurally duplicated.

## 2. Stable seed contract

A deterministic sector spawn derives from:

`systemSeed + sectorId + poiFamilyId + slotIndex`

The future runtime adapter may choose the hash implementation, but must log the resulting seed and
must produce the same selection for the same inputs across reloads. The authoritative source of the
Moss `systemSeed` and sector-grid dimensions remains author-required; the packet does not invent them.

## 3. Packet fields

Each packet declares:

- stable `id` and source `poiFamilyId`;
- `spawnMode`: authored landmark, deterministic sector, or either;
- proposed per-scene cap and whether only one instance may exist per sector;
- exact seed inputs for deterministic modes;
- source budget class, distance band, and near/mid/far LOD plan;
- stable sockets consumed by scan, tractor, tow, repair, distress, gate, and mission adapters;
- compatible mission IDs and salvage outputs copied from the source POI catalog;
- exclusions that prevent overlap with arrival, gate, dock, or protected story volumes;
- `adapterStatus: unassigned-post-checkpoint` until a runtime owner is approved.

## 4. Six Moss packet families

1. **Prior-waker smallcraft** — sparse deterministic wrecks, maximum two in a scene; ordered-log
   thread preserved; scan/salvage/log sockets required.
2. **Disabled salvage tug** — authored or deterministic distress site, maximum one; tow, repair,
   distress, and escort anchors required.
3. **Debris ribbon** — deterministic field, maximum two; pooled fragments and safe lanes; never
   overlaps gate alignment or arrival safety volumes.
4. **Orbital repair relay** — authored or deterministic infrastructure, maximum one; the
   `signal_hook` and Warden-response origin remain mandatory.
5. **Stranded civilian ship** — authored or deterministic distress site, maximum one; distress,
   tow, repair, and passenger/cargo transfer sockets required.
6. **Ziptide gate approach lane** — authored landmark only, exactly one; alignment, traversal, and
   safe-arrival volumes may not be procedurally moved or duplicated.

## 5. Later runtime adapter acceptance

Implementation begins only after the device checkpoint. The adapter must:

- consume these packets rather than hardcode Moss POI families;
- resolve packet sockets to existing mission/job steps without creating a second mission engine;
- route salvage through the existing inventory/economy authority;
- route lore flags through the existing profile/flag authority;
- log family, packet, seed, slot, cap decision, and rejected exclusions;
- remain idempotent when patchers or scene preparation run more than once;
- stop and report when a required authored landmark or socket cannot resolve.

The first runtime proof slice should contain one prior-waker wreck, one relay, one distress ship, and
the authored gate approach. Debris density and repeated variants come only after the small slice passes
Quest presence, comfort, navigation, and budget review.
