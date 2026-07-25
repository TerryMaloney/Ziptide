# KEEPER SIDECARS

**Status:** canonical machine-readable companion data for concept keepers. Sidecars describe visual authority and downstream intake boundaries; they do not authorize runtime, scenes, assets, recipes, IDs, saves, or APK changes.

## Purpose

Each proposed keeper receives one JSON sidecar so later Forge, audit, comparison, provenance, and meta-pipeline tools can consume:

- stable keeper and proposed asset IDs;
- asset class and role;
- scale band;
- family/machine grammar;
- palette slots;
- authority split;
- controlling documents;
- source image archive/provenance state;
- measured-spec target;
- shipping-use/license boundary;
- runtime authorization state.

## Rules

1. JSON must remain valid and comment-free.
2. `image_records[].path` stays `null` until a durable binary archive exists.
3. Conversation mounts and attachment IDs are provenance clues, not durable file paths.
4. Generated labels, glyphs, dimensions, brands, and readings are not canon unless separately approved.
5. A sidecar may describe a composite keeper with several controlling image classes.
6. Later images do not supersede a sidecar automatically; update the authority split explicitly.
7. `internal_reference` generated art does not need a third-party shipping ledger entry unless it is later proposed for store/in-game/public shipping use.
8. Any generated image proposed to ship must move to `shipping_use: proposed` and receive current commercial-use/license review plus the third-party licensing ledger entry before approval.
9. `runtime_authority` remains `none` until the recovery/freeze and implementation gates explicitly clear.
10. Numerical dimensions remain unresolved until measured-spec and headset proxy work.

## Required top-level fields

- `schema_version`
- `keeper_id`
- `asset_id_proposed`
- `display_name_working`
- `status`
- `asset_class`
- `role`
- `scale_band`
- `visual_grammar`
- `palette_slots`
- `authority_split`
- `keeper_documents`
- `image_records`
- `measured_spec`
- `provenance_status`
- `shipping_use`
- `license_review_status`
- `runtime_authority`
- `known_unresolved`

## Validation target

A future report-only validator should verify:

- valid JSON;
- unique `keeper_id` and `asset_id_proposed`;
- cited documents exist;
- measured-spec path exists where non-null;
- `shipping_use != internal_reference` requires licensing metadata;
- archived image records include hash, generator, date, prompt record, and durable path;
- no `runtime_authority` value silently changes through concept work.
