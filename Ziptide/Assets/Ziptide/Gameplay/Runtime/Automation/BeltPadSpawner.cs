using UnityEngine;
using Ziptide.Content;
using Ziptide.Content.Automation;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// HARDWIRING 4.1i — belts leave the sandbox: materializes a world pack's
    /// <see cref="BeltFloorSpawnDefinition"/> entries at scene start (the JobDirector
    /// mines/gardens idiom — pure pack data in, runtime objects out). Each pad is a buildable
    /// <see cref="BeltFloorRuntime"/>: optional intake port bound to a pack mine's OWN hopper
    /// (shared MineState — economy-safe by construction), optional payout sink, a tile dispenser,
    /// and a conductor post (4.1h). Player edits persist under the pad's id (4.1f). Grid size is
    /// clamped to the 4.1g budget — runtime pads must never dodge the gate the audit enforces on
    /// patched scenes.
    /// </summary>
    public static class BeltPadSpawner
    {
        private const int MaxCellsPerSide = 16; // 16×16 = the 4.1g floor-area cap

        public static void CreateAll(WorldPackDefinition pack, Transform parent, string worldId)
        {
            if (pack == null || pack.beltFloors == null || pack.beltFloors.Count == 0) return;
            var root = new GameObject("BeltPads");
            root.transform.SetParent(parent);
            root.transform.localPosition = Vector3.zero;
            foreach (var def in pack.beltFloors)
            {
                if (def == null || string.IsNullOrEmpty(def.id)) continue;
                Create(def, pack, root.transform, worldId);
            }
        }

        private static void Create(BeltFloorSpawnDefinition def, WorldPackDefinition pack,
                                   Transform parent, string worldId)
        {
            var go = new GameObject("BeltPad_" + def.id);
            go.transform.SetParent(parent);
            go.transform.localPosition = def.localPosition;

            var floor = go.AddComponent<BeltFloorRuntime>();
            floor.width = Mathf.Clamp(def.width, 1, MaxCellsPerSide);
            floor.depth = Mathf.Clamp(def.depth, 1, MaxCellsPerSide);
            floor.cellSize = def.cellSize > 0f ? def.cellSize : 0.8f;
            floor.floorId = def.id;

            int mid = floor.depth / 2;
            if (!string.IsNullOrEmpty(def.feedMineId))
            {
                floor.AuthorPort(0, mid, BeltDir.East);
                var intakeGo = new GameObject("BeltPadIntake_" + def.feedMineId);
                intakeGo.transform.SetParent(go.transform);
                intakeGo.transform.position = floor.GridToWorld(-1f, mid);
                var intake = intakeGo.AddComponent<BeltMinePortRuntime>();
                intake.floor = floor;
                intake.portX = 0; intake.portZ = mid;
                intake.worldId = worldId;
                intake.machineId = def.feedMineId; // the pack mine's OWN MineState — one hopper
                var mineDef = pack.mines != null
                    ? pack.mines.Find(m => m != null && m.id == def.feedMineId) : null;
                if (mineDef != null)
                {
                    // Mirror the mine's definition truth so Bind() doesn't rewrite the shared state
                    // with intake defaults (rate/cap/resource live in definitions, saves keep progress).
                    intake.resourceId = mineDef.resourceId;
                    intake.ratePerSecond = (float)mineDef.ratePerSecond;
                    intake.storageCap = (float)mineDef.storageCap;
                }
            }
            if (!string.IsNullOrEmpty(def.payoutResourceId))
                floor.AuthorSink(floor.width - 1, mid, def.payoutResourceId);

            if (def.dispenser)
            {
                var disp = new GameObject("BeltPadDispenser_" + def.id);
                disp.transform.SetParent(go.transform);
                disp.transform.position = floor.GridToWorld(-1.2f, floor.depth - 0.5f);
                disp.AddComponent<BeltDispenserRuntime>();
            }
            if (def.conductor)
            {
                var cond = new GameObject("BeltPadConductor_" + def.id);
                cond.transform.SetParent(go.transform);
                cond.transform.position = floor.GridToWorld(0f, -1.2f);
                var conductor = cond.AddComponent<BeltConductorRuntime>();
                conductor.floor = floor;
                conductor.startX = 0; conductor.startZ = mid;
            }
            Debug.Log("ZIPTIDE: BELT_PAD id=" + def.id + " feed=" + def.feedMineId +
                      " payout=" + def.payoutResourceId + " size=" + floor.width + "x" + floor.depth);
        }
    }
}
