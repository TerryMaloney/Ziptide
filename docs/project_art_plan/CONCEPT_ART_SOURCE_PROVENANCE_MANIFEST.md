# CONCEPT-ART SOURCE PROVENANCE MANIFEST

**Status:** canonical metadata ledger for concept-art source images used by the ZIPTIDE prompt pipeline. This document does not store image binaries. Binary archival remains required in a durable project-controlled location before measured-spec, paid 3D, external sharing, or production intake.

## 1. Purpose

For every concept image considered as a keeper, exploration reference, refinement source, or rejection example, preserve:

- stable source record ID;
- proposed asset/family ID;
- conversation date;
- generator/service/model when known;
- prompt author/operator when known;
- text-to-image or image-to-image mode;
- original prompt and refinement prompt document;
- source image/reference IDs;
- original filename;
- byte size;
- SHA-256;
- keeper/reject verdict;
- controlling decision classes;
- known drift;
- supersession relation;
- commercial-use/license evidence when required;
- durable binary archive location;
- measured-spec destination.

A prompt or verdict document is not a substitute for preserving the unedited binary.

## 2. Record schema

```yaml
source_record_id: CA-SRC-<number>
asset_id_proposed:
family_or_system:
date_generated_or_received:
generator_service:
generator_model_version:
prompt_author_operator:
mode: text-to-image | image-to-image | unknown
conversation_attachment_id:
conversation_filename:
ephemeral_review_path:
byte_size:
sha256:
source_reference_ids: []
prompt_document:
verdict_document:
verdict: keeper | composite-keeper | refine | exploration-reference | reject
controls: []
known_drift: []
supersedes: []
superseded_by: []
license_status: not-checked | current-terms-verified | restricted | unknown
license_evidence:
durable_binary_archive:
measured_spec_destination:
notes:
```

## 3. Current records

### CA-SRC-0001 — Circuit Bridge flush-docked production sheet

```yaml
source_record_id: CA-SRC-0001
asset_id_proposed: equipment.circuit_bridge.mk1
family_or_system: Conduct family / field equipment
date_generated_or_received: 2026-07-24
generator_service: Nano Banana (reported project workflow; exact endpoint unresolved)
generator_model_version: unknown
prompt_author_operator: current GPT lane; possible external operator involvement not evidenced in HANDOFF
mode: image-to-image
conversation_attachment_id: file_000000002a3481fb82394e46446d31bc
conversation_filename: 1000001608.png
ephemeral_review_path: /mnt/data/1000001608.png
byte_size: 354382
sha256: 9a6f608b21a50b89e13e935a4da17febe6a010aa764f6bab53647a8ce9005a22
source_reference_ids:
  - prior Circuit Bridge first-pass production sheet
prompt_document: docs/project_art_plan/PROMPT_TEST_08_FIRST_PASS_VERDICT_CIRCUIT_BRIDGE_MK1.md
verdict_document: docs/project_art_plan/PROMPT_TEST_08_FINAL_COMPOSITE_KEEPER_VERDICT_CIRCUIT_BRIDGE_MK1.md
verdict: composite-keeper
controls:
  - equipment body
  - guarded Conductive Vein channel
  - belt role
  - service/open view
  - rear docking face
  - short side clamps
  - fold-over bridge lever
  - flush-docked state
known_drift:
  - generated glyphs and labels are non-authoritative
  - embedded active-view pose may not define final player hand layout
  - exact dimensions and contact geometry unresolved
supersedes:
  - first-pass pistol-like Circuit Bridge body for silhouette and docking design
superseded_by: []
license_status: not-checked
license_evidence: pending current service-term capture
durable_binary_archive: pending
measured_spec_destination: GI-MSPEC-equipment-circuit-bridge-mk1
notes: Production sheet is keeper authority only under the written composite-reference split.
```

### CA-SRC-0002 — Circuit Bridge second player-eye field-use image

```yaml
source_record_id: CA-SRC-0002
asset_id_proposed: equipment.circuit_bridge.mk1
family_or_system: Conduct family / field equipment
date_generated_or_received: 2026-07-24
generator_service: Nano Banana (reported project workflow; exact endpoint unresolved)
generator_model_version: unknown
prompt_author_operator: current GPT lane; possible external operator involvement not evidenced in HANDOFF
mode: image-to-image
conversation_attachment_id: file_00000000899481fbb55b54210501852b
conversation_filename: 1000001609.png
ephemeral_review_path: /mnt/data/1000001609.png
byte_size: 292448
sha256: 71b4d12c7efd2b2cf19f18368e79c50d7c3b513b5d37e0b60db735012e731e3b
source_reference_ids:
  - prior handheld Circuit Bridge field-use scene
prompt_document: docs/project_art_plan/PROMPT_TEST_08_FIRST_PASS_VERDICT_CIRCUIT_BRIDGE_MK1.md
verdict_document: docs/project_art_plan/PROMPT_TEST_08_FINAL_COMPOSITE_KEEPER_VERDICT_CIRCUIT_BRIDGE_MK1.md
verdict: reject
controls:
  - rain-wet industrial environment mood only
known_drift:
  - handheld pistol-like body
  - one-hand aiming pose
  - forward contact/muzzle read
  - open-air electrical arc
  - no flush docking or hands-free persistence
supersedes: []
superseded_by:
  - measured interaction plate required; no further generator pass authorized
license_status: not-checked
license_evidence: pending current service-term capture
durable_binary_archive: pending
measured_spec_destination: none for equipment interaction; environment mood may feed provisional world plate
notes: Must never be used as interaction, equipment, pose, scale, or electrical-effect authority.
```

## 4. Backfill queue

The following keeper groups require source-image metadata and binary backfill from the conversation/file library or original generator downloads:

1. Moss Filter lifecycle sheet;
2. Filter Organ component reference;
3. Moss Filter nursery departure/return pair;
4. compact Moss Filter Rig sheet;
5. invention station keeper;
6. Cistern Gripvine lifecycle sheet;
7. Dry Cistern discovery scene;
8. Grip Pad/Grip Fiber component sheet;
9. Grip nursery pair;
10. BioRefiner keeper sheets and installed scene;
11. Assembler keeper sheets and installed scene;
12. Ore Processor keeper sheets;
13. Equipment Frame Blank integration sheet;
14. Cistern Grip Clamp sheet, field-use scene, chain logic, and card;
15. Relay Reed lifecycle and discovery sheets;
16. Conductive Vein/Charge Nodule sheet;
17. Relay Reed nursery pair;
18. rejected first-pass Circuit Bridge production and field-use images.

Do not invent hashes, source IDs, service versions, or license records during backfill.

## 5. Durable archive requirements

A keeper is ready for paid 3D or measured visual-spec intake only when:

- unedited source binary is stored durably;
- SHA-256 matches this ledger;
- original and refinement prompts are linked;
- service/model and generation date are recorded as accurately as available;
- current commercial-use terms have been reviewed where external use is planned;
- verdict and authority split are linked;
- known drift is documented;
- supersession history is clear.

## 6. Tooling limitation record

The current GitHub text-content action can preserve metadata but cannot upload these PNG binaries directly. Conversation mount paths are ephemeral and must not be treated as repository archive locations. A future operator with binary-upload capability should archive the files without recompression or editing and update `durable_binary_archive`.
