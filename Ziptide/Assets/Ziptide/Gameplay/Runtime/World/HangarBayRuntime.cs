using UnityEngine;

using Ziptide.Content.Ship;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// SHIP PILLAR 2.2/2.5 — THE HANGAR BAY: the in-VR refit surface (the spec's "grab-and-attach
    /// hangar, lives in the home hub"). Self-builds beside the Quarters: tile rows for CHASSIS (all
    /// six), ENGINE and WINGS modules, and the ship's NAME; a live holo-readout shows the resolved
    /// loadout stats (pure ShipLoadoutCore — wraps can't touch these numbers). Selecting a tile
    /// equips via ShipLocker (profile-flag persistence) and re-runs ShipRefit LIVE on the berth hull —
    /// the ship you're standing next to transforms as you shop. First refit sets SHIP_REFIT (RILL
    /// notices). Tile sizing obeys the characterSize×fontSize lesson.
    /// </summary>
    public class HangarBayRuntime : MonoBehaviour
    {
        [Tooltip("The hull root ShipRefit applies to (the boarding station's ship).")]
        public GameObject shipRoot;

        private TextMesh _readout;
        private static readonly string[] NamePool =
            { "WAKESKIPPER", "SALT MARE", "THE DEBUGGER", "LOW TIDE", "PATTERN DANCER", "MARA'S DARE" };

        private void Start()
        {
            BuildPanel();
            RefreshReadout();

            // The complete ship hierarchy exists by HangarBay.Start. Attach the local presentation
            // guard here so exterior arrival views do not render cockpit/quarters/hangar UI before
            // the tracked head is physically in those spaces. ShipBoardingStation remains the sole
            // owner of boarding, teleport and travel behavior.
            if (shipRoot != null)
            {
                ShipBoardingPresentationGuard guard =
                    shipRoot.GetComponent<ShipBoardingPresentationGuard>();
                if (guard == null) guard = shipRoot.AddComponent<ShipBoardingPresentationGuard>();
                guard.RefreshNow();
            }
        }

        private void BuildPanel()
        {
            // Backboard.
            var board = GameObject.CreatePrimitive(PrimitiveType.Cube);
            board.name = "HangarBoard";
            Object.Destroy(board.GetComponent<Collider>());
            board.transform.SetParent(transform, false);
            board.transform.localScale = new Vector3(2.6f, 2.0f, 0.06f);
            board.transform.localPosition = new Vector3(0f, 1.3f, 0f);
            ItemFactory.ApplyURPColor(board, new Color(0.12f, 0.14f, 0.17f));

            var title = NewText("HANGAR — REFIT", new Vector3(0f, 2.42f, -0.06f), 0.02f);
            title.color = new Color(0.85f, 0.9f, 1f);

            // Row 1: chassis (six).
            for (int i = 0; i < ShipChassisPreset.All.Length; i++)
            {
                var c = ShipChassisPreset.All[i];
                Tile(c.DisplayName, new Vector3(-1.05f + i * 0.42f, 2.05f, -0.06f),
                     () => EquipAndRefit("chassis", c.Id));
            }
            // Row 2: engines.
            string[] engines = { "engine_stock", "engine_tide", "engine_ion" };
            for (int i = 0; i < engines.Length; i++)
            {
                var id = engines[i];
                Tile(ShipModulePreset.Find(id).DisplayName, new Vector3(-0.84f + i * 0.84f, 1.6f, -0.06f),
                     () => EquipAndRefit("engine", id));
            }
            // Row 3: wings.
            string[] wings = { "wings_stock", "wings_razor", "wings_bulwark" };
            for (int i = 0; i < wings.Length; i++)
            {
                var id = wings[i];
                Tile(ShipModulePreset.Find(id).DisplayName, new Vector3(-0.84f + i * 0.84f, 1.15f, -0.06f),
                     () => EquipAndRefit("wings", id));
            }
            // Row 4: the name.
            for (int i = 0; i < NamePool.Length; i++)
            {
                var n = NamePool[i];
                Tile(n, new Vector3(-1.05f + i * 0.42f, 0.7f, -0.06f),
                     () => EquipAndRefit("name", n));
            }

            // The holo-readout.
            _readout = NewText("", new Vector3(0f, 0.28f, -0.06f), 0.016f);
            _readout.color = new Color(0.5f, 0.95f, 0.8f);
        }

        private void EquipAndRefit(string slot, string id)
        {
            var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            if (profile == null) return;
            ShipLocker.Equip(profile, slot, id);
            profile.SetFlag("SHIP_REFIT"); // RILL notices the first refit
            if (shipRoot != null) ShipRefit.Apply(shipRoot);
            RefreshReadout();
            Debug.Log("ZIPTIDE: HANGAR_EQUIP slot=" + slot + " id=" + id);
        }

        private void RefreshReadout()
        {
            if (_readout == null) return;
            var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            var chassis = ShipChassisPreset.Find(ShipLocker.GetEquipped(profile, "chassis"));
            var mods = new System.Collections.Generic.List<ShipModulePreset>();
            foreach (var id in ShipLocker.EquippedModules(profile, new[] { "engine", "wings", "hardpoint", "cargo" }))
            {
                var m = ShipModulePreset.Find(id);
                if (m != null) mods.Add(m);
            }
            var s = ShipLoadoutCore.Resolve(chassis, mods);
            _readout.text = chassis.DisplayName.ToUpperInvariant() +
                "   SPD " + s.Speed.ToString("F0") +
                "  HDL " + s.Handling.ToString("F1") +
                "  BST " + s.Boost.ToString("F1") + "x" +
                "  CRG " + s.Cargo.ToString("F0") +
                "  ARM " + s.Armor.ToString("F0");
        }

        // ── Tile/text helpers (the lobby-board idiom, compact) ───────────────
        private void Tile(string label, Vector3 localPos, System.Action onSelect)
        {
            var tile = GameObject.CreatePrimitive(PrimitiveType.Cube);
            tile.name = "Tile_" + label;
            tile.transform.SetParent(transform, false);
            tile.transform.localPosition = localPos;
            tile.transform.localScale = new Vector3(0.38f, 0.28f, 0.04f);
            ItemFactory.ApplyURPColor(tile, new Color(0.2f, 0.24f, 0.3f));
            var grab = tile.AddComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRSimpleInteractable>(); // collider exists (primitive)
            grab.selectEntered.AddListener(_ => onSelect());

            var tm = NewText(label, localPos + new Vector3(0f, 0f, -0.05f), 0.008f);
            tm.color = new Color(0.9f, 0.93f, 1f);
        }

        private TextMesh NewText(string text, Vector3 localPos, float charSize)
        {
            var go = new GameObject("Txt_" + text);
            go.transform.SetParent(transform, false);
            go.transform.localPosition = localPos;
            var tm = go.AddComponent<TextMesh>();
            tm.text = text;
            tm.characterSize = charSize; tm.fontSize = 64;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            return tm;
        }
    }
}
