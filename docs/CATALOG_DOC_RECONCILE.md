# CATALOG ↔ DOC RECONCILE — the standing machinery so drift is never forgotten
### Terry 2026-07-23: "make sure the periodic reconcile is not something that gets forgotten."

GPT's offline pass created a good pattern — design docs (prose) now have companion **machine
catalogs** (JSON) enforced by CI gates. The risk (I flagged it, Terry called it): the prose and the
JSON now BOTH hold truth, so they can drift apart, and "we'll reconcile periodically" is exactly the
kind of promise that gets forgotten. Per the Class Law, a promise isn't a fix — this is the standing
machinery that makes the reconcile un-forgettable. **MISS_LEDGER #16.**

## How it works
- **The registry:** `docs/catalog_doc_reconcile.json` pairs every design catalog/manifest with its
  owning prose doc + owner + `lastReconciled` date.
- **The gate:** `tools/catalog_doc_sync_gate.py` (CI, in `fast-preflight.yml`; covered by
  `tools/tests/test_catalog_doc_sync_gate.py`). It **BLOCKS** (fails CI) when:
  - a design catalog matching `trackedGlobs` is not registered → *you added a catalog and forgot to
    register its doc + reconcile tracking* (the core "never forgotten" guarantee);
  - a registered catalog or its doc file is missing, or the catalog isn't valid JSON;
  - a `lastReconciled` date is malformed.
  It **WARNS** (surfaced, non-blocking) when a doc doesn't back-link its catalog by filename, or when
  `lastReconciled` is older than `staleWarnDays` (45) — i.e. **"reconcile due."**

## The reconcile ritual (do this when you touch either side of a pair)
1. Change a design doc OR its catalog → update the OTHER to match.
2. Bump that entry's `lastReconciled` in `docs/catalog_doc_reconcile.json` to today.
3. Keep the doc's **`Machine catalog: <path>`** back-link line present (clears the warning).
4. A NEW design catalog → add a registry entry (else CI blocks) with its owning doc.

## Why this is enough (honest scope)
The gate proves the catalog and doc EXIST, are paired, and are actively tracked — it cannot read
prose to prove semantic equality (doc value == JSON value). That last mile is human judgment at
reconcile time. What the machinery guarantees is that the reconcile is **surfaced, owned, dated, and
un-skippable** — no pair can be silently added or left un-tracked. That's the forgettable part,
removed.

## Owners: back-links still to add (clears their warnings)
GPT-owned entries (gate/mk2/rill/concept-intake) should add a `Machine catalog: <path>` line to
their prose doc and confirm the doc pairing. The four rb space docs already carry the back-link.
