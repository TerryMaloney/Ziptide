#!/usr/bin/env python3
"""catalog_doc_sync_gate — keep machine catalogs and their prose docs from silently drifting.

Every machine-enforced DESIGN catalog/manifest must be REGISTERED in
docs/catalog_doc_reconcile.json alongside the prose design doc that owns its truth, so
"doc says X, catalog says Y" drift can never be forgotten (MISS_LEDGER #16 system change).

BLOCKING (exit 1):
  - a tracked catalog (matching trackedGlobs) is NOT registered  -> you added a catalog and
    forgot to register its owning doc + reconcile tracking;
  - a registered catalog or its doc file is missing;
  - a registered catalog is not valid JSON.

WARNING (non-blocking, surfaced so a periodic reconcile actually happens):
  - the doc does not back-link the catalog by filename (add a "Machine catalog: <path>" line);
  - lastReconciled is older than staleWarnDays (reconcile due).

Stdlib only. Same idiom as the other tools/*_gate.py gates; wired in fast-preflight.
"""
import datetime
import glob
import json
import os
import sys

ROOT = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))
REGISTRY = os.path.join(ROOT, "docs", "catalog_doc_reconcile.json")


def _rel(p):
    return p.replace(ROOT + os.sep, "").replace(os.sep, "/")


def evaluate(registry_path=REGISTRY):
    """Return (blocking, warnings) lists. Pure — no printing — so tests can drive it."""
    blocking = []
    warnings = []

    if not os.path.exists(registry_path):
        return (["REGISTRY_MISSING path=" + _rel(registry_path)], [])
    reg = json.load(open(registry_path))
    entries = reg.get("entries", [])
    tracked_globs = reg.get("trackedGlobs", [])
    stale_days = int(reg.get("staleWarnDays", 45))

    registered = {e["catalog"] for e in entries}

    # 1) coverage — every tracked catalog must be registered
    on_disk = set()
    for g in tracked_globs:
        for p in glob.glob(os.path.join(ROOT, g)):
            on_disk.add(_rel(p))
    for cat in sorted(on_disk - registered):
        blocking.append(f"UNREGISTERED_CATALOG catalog={cat} (add it to docs/catalog_doc_reconcile.json with its owning doc)")

    # 2) each entry — files exist, catalog parses, back-link + staleness
    today = datetime.date.today()
    for e in entries:
        cat, doc = e.get("catalog", ""), e.get("doc", "")
        cat_abs, doc_abs = os.path.join(ROOT, cat), os.path.join(ROOT, doc)
        if not os.path.exists(cat_abs):
            blocking.append(f"CATALOG_FILE_MISSING catalog={cat}")
        else:
            try:
                json.load(open(cat_abs))
            except Exception as ex:
                blocking.append(f"CATALOG_INVALID_JSON catalog={cat} err={ex}")
        if not os.path.exists(doc_abs):
            blocking.append(f"DOC_FILE_MISSING doc={doc} (for catalog {cat})")
        elif os.path.exists(cat_abs):
            base = os.path.basename(cat)
            if base not in open(doc_abs, encoding="utf-8", errors="ignore").read():
                warnings.append(f"NO_BACKLINK doc={doc} should reference {base} (add a 'Machine catalog: {cat}' line)")
        lr = e.get("lastReconciled", "")
        try:
            d = datetime.date.fromisoformat(lr)
            if (today - d).days > stale_days:
                warnings.append(f"RECONCILE_DUE catalog={cat} lastReconciled={lr} (>{stale_days}d) owner={e.get('owner','?')}")
        except ValueError:
            blocking.append(f"BAD_LASTRECONCILED catalog={cat} value={lr!r} (want YYYY-MM-DD)")

    return (blocking, warnings)


def main(registry_path=REGISTRY):
    blocking, warnings = evaluate(registry_path)
    for w in warnings:
        print("CATALOG_DOC_SYNC_WARN " + w)
    for b in blocking:
        print("CATALOG_DOC_SYNC_BLOCK " + b)
    print(f"CATALOG_DOC_SYNC summary warnings={len(warnings)} blocking={len(blocking)}")
    return 1 if blocking else 0


if __name__ == "__main__":
    sys.exit(main())
