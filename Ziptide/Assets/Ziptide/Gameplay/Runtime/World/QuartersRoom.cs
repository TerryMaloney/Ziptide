using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// THE QUARTERS (GAME_PLAN M4 / docs/systems/QUARTERS.md): the customization room — your locker.
    /// Three display bays (WEAPON SKINS / SHIP LIVERY / TRAILS &amp; EMBLEMS) browse
    /// <see cref="CosmeticDefinition"/> assets from Resources/Cosmetics, filtered by kind and ownership;
    /// selecting a row equips it through the pure <see cref="CosmeticLocker"/> (profile flags — saves
    /// and crosses modes for free). No cosmetics are authored yet, so every bay shows the supply-drop
    /// stub — the ROOM and its plumbing are the deliverable.
    ///
    /// DELIBERATELY HOST-AGNOSTIC: no ship references. The ship parks one aft of the cockpit; the PvP
    /// pre-round locker (architect's crossover — see QUARTERS.md) can spawn another in the arena and
    /// get identical behavior, because equip state lives in the profile, not the room.
    /// Logs ZIPTIDE: QUARTERS_BROWSE kind=… / COSMETIC_EQUIPPED id=… target=…
    /// </summary>
    public class QuartersRoom : MonoBehaviour
    {
        private static readonly Color WallColor = new Color(0.14f, 0.15f, 0.19f);
        private static readonly Color FloorColor = new Color(0.20f, 0.21f, 0.25f);
        private static readonly Color TrimColor = new Color(0.85f, 0.70f, 0.30f); // warm cabin gold
        private static readonly Color BayColor = new Color(0.16f, 0.40f, 0.50f);
        private static readonly Color BayHot = new Color(0.25f, 0.62f, 0.75f);

        private const float RoomW = 4.6f, RoomD = 4.6f, WallH = 2.6f;

        private readonly List<GameObject> _browseRows = new List<GameObject>();
        private TextMesh _lockerBoard;

        private void Awake()
        {
            BuildShell();
            BuildBay("WEAPON SKINS", CosmeticKind.WeaponSkin, new Vector3(-1.55f, 0f, 1.6f), 35f);
            BuildBay("SHIP LIVERY", CosmeticKind.ShipLivery, new Vector3(0f, 0f, 2.0f), 0f);
            BuildBay("TRAILS & EMBLEMS", CosmeticKind.Trail, new Vector3(1.55f, 0f, 1.6f), -35f);
            BuildLockerBoard();
        }

        // ── The shell: floor, walls with a doorway, warm trim ────────────────

        private void BuildShell()
        {
            Cube("Floor", new Vector3(0f, -0.05f, 0f), new Vector3(RoomW, 0.1f, RoomD), FloorColor, true);
            Cube("Ceiling", new Vector3(0f, WallH, 0f), new Vector3(RoomW, 0.1f, RoomD), WallColor, false);
            Cube("WallBack", new Vector3(0f, WallH * 0.5f, RoomD * 0.5f), new Vector3(RoomW, WallH, 0.12f), WallColor, true);
            Cube("WallL", new Vector3(-RoomW * 0.5f, WallH * 0.5f, 0f), new Vector3(0.12f, WallH, RoomD), WallColor, true);
            Cube("WallR", new Vector3(RoomW * 0.5f, WallH * 0.5f, 0f), new Vector3(0.12f, WallH, RoomD), WallColor, true);
            // Front wall with a doorway gap (the host places its entrance/teleport on this side).
            Cube("WallFrontL", new Vector3(-RoomW * 0.5f + 0.9f, WallH * 0.5f, -RoomD * 0.5f), new Vector3(1.8f, WallH, 0.12f), WallColor, true);
            Cube("WallFrontR", new Vector3(RoomW * 0.5f - 0.9f, WallH * 0.5f, -RoomD * 0.5f), new Vector3(1.8f, WallH, 0.12f), WallColor, true);
            Cube("DoorHeader", new Vector3(0f, WallH - 0.25f, -RoomD * 0.5f), new Vector3(1.0f, 0.5f, 0.12f), WallColor, true);
            // Warm trim strip so the cabin reads lived-in, not a cell.
            Cube("TrimBack", new Vector3(0f, 1.9f, RoomD * 0.5f - 0.08f), new Vector3(RoomW - 0.3f, 0.06f, 0.03f), TrimColor, false);
            Cube("TrimL", new Vector3(-RoomW * 0.5f + 0.08f, 1.9f, 0f), new Vector3(0.03f, 0.06f, RoomD - 0.3f), TrimColor, false);
            Cube("TrimR", new Vector3(RoomW * 0.5f - 0.08f, 1.9f, 0f), new Vector3(0.03f, 0.06f, RoomD - 0.3f), TrimColor, false);
        }

        // ── Display bays ─────────────────────────────────────────────────────

        private void BuildBay(string title, CosmeticKind kind, Vector3 localPos, float yaw)
        {
            var bay = new GameObject("Bay_" + kind);
            bay.transform.SetParent(transform, false);
            bay.transform.localPosition = localPos;
            bay.transform.localRotation = Quaternion.Euler(0f, yaw, 0f);

            // Pedestal + display plinth (where an equipped preview model stands at the M6 art pass).
            CubeUnder(bay.transform, "Pedestal", new Vector3(0f, 0.45f, 0f), new Vector3(0.7f, 0.9f, 0.7f), FloorColor, true);
            CubeUnder(bay.transform, "Plinth", new Vector3(0f, 0.95f, 0f), new Vector3(0.5f, 0.1f, 0.5f), TrimColor, false);

            var header = new GameObject("Header");
            var htm = header.AddComponent<TextMesh>();
            htm.text = title;
            htm.characterSize = 0.045f;
            htm.fontSize = 48;
            htm.anchor = TextAnchor.MiddleCenter;
            htm.alignment = TextAlignment.Center;
            htm.color = TrimColor;
            header.transform.SetParent(bay.transform, false);
            header.transform.localPosition = new Vector3(0f, 2.05f, 0f);

            // The browse panel: select to (re)list this bay's cosmetics.
            MakePanelUnder(bay.transform, "Browse", new Vector3(0f, 1.45f, 0f),
                "BROWSE", BayColor, () => Browse(bay.transform, kind));

            // Open every bay once on build so the stub state is visible without a press.
            Browse(bay.transform, kind);
        }

        private void Browse(Transform bay, CosmeticKind kind)
        {
            // Clear this bay's old rows.
            for (int i = _browseRows.Count - 1; i >= 0; i--)
            {
                if (_browseRows[i] == null) { _browseRows.RemoveAt(i); continue; }
                if (_browseRows[i].transform.IsChildOf(bay)) { Destroy(_browseRows[i]); _browseRows.RemoveAt(i); }
            }

            var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            var all = Resources.LoadAll<CosmeticDefinition>("Cosmetics");
            var owned = new List<CosmeticDefinition>();
            foreach (var c in all)
            {
                if (c == null) continue;
                // The Trail bay also lists Emblems (they share the third display).
                bool inBay = c.kind == kind || (kind == CosmeticKind.Trail && c.kind == CosmeticKind.Emblem);
                if (!inBay) continue;
                bool isOwned = string.IsNullOrEmpty(c.ownedFlag) || (profile != null && profile.HasFlag(c.ownedFlag));
                if (isOwned) owned.Add(c);
            }

            Debug.Log("ZIPTIDE: QUARTERS_BROWSE kind=" + kind + " owned=" + owned.Count);

            if (owned.Count == 0)
            {
                // THE STUB: the room exists before its stock does.
                var empty = new GameObject("EmptyNotice");
                var tm = empty.AddComponent<TextMesh>();
                tm.text = "NO ITEMS AVAILABLE\n\n- check back after -\n- the next supply drop -";
                tm.characterSize = 0.03f;
                tm.fontSize = 48;
                tm.anchor = TextAnchor.MiddleCenter;
                tm.alignment = TextAlignment.Center;
                tm.color = new Color(0.6f, 0.65f, 0.7f);
                empty.transform.SetParent(bay, false);
                empty.transform.localPosition = new Vector3(0f, 1.15f, -0.35f);
                _browseRows.Add(empty);
                return;
            }

            for (int i = 0; i < owned.Count && i < 6; i++)
            {
                var c = owned[i];
                string targetKey = c.kind == CosmeticKind.WeaponSkin && !string.IsNullOrEmpty(c.targetItemId)
                    ? c.targetItemId
                    : c.kind.ToString().ToLowerInvariant();
                string label = string.IsNullOrEmpty(c.displayName) ? c.cosmeticId : c.displayName;
                bool equipped = CosmeticLocker.GetEquipped(profile, targetKey) == c.cosmeticId;
                var cRef = c;

                var row = MakePanelUnder(bay, "Cos_" + c.cosmeticId, new Vector3(0f, 1.2f - i * 0.22f, -0.3f),
                    (equipped ? "* " : "") + label, equipped ? TrimColor : BayColor, () =>
                    {
                        var prof = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
                        CosmeticLocker.Equip(prof, targetKey, cRef.cosmeticId);
                        Debug.Log("ZIPTIDE: COSMETIC_EQUIPPED id=" + cRef.cosmeticId + " target=" + targetKey);
                        Browse(bay, kind); // refresh the equipped marker
                        RefreshLockerBoard();
                    }, small: true);
                _browseRows.Add(row);
            }
        }

        // ── The locker board: what you have equipped, at a glance ────────────

        private void BuildLockerBoard()
        {
            Cube("LockerFrame", new Vector3(-RoomW * 0.5f + 0.2f, 1.5f, -0.8f), new Vector3(0.06f, 1.2f, 1.3f), FloorColor, false);
            var board = new GameObject("LockerBoard");
            _lockerBoard = board.AddComponent<TextMesh>();
            _lockerBoard.characterSize = 0.028f;
            _lockerBoard.fontSize = 48;
            _lockerBoard.anchor = TextAnchor.MiddleCenter;
            _lockerBoard.alignment = TextAlignment.Center;
            _lockerBoard.color = new Color(0.8f, 0.9f, 1f);
            board.transform.SetParent(transform, false);
            board.transform.localPosition = new Vector3(-RoomW * 0.5f + 0.26f, 1.5f, -0.8f);
            board.transform.localRotation = Quaternion.Euler(0f, -90f, 0f);
            RefreshLockerBoard();
        }

        private void RefreshLockerBoard()
        {
            if (_lockerBoard == null) return;
            var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            string taser = CosmeticLocker.GetEquipped(profile, "taser_dart_gun") ?? "default";
            string grav = CosmeticLocker.GetEquipped(profile, "gravity_gun") ?? "default";
            string livery = CosmeticLocker.GetEquipped(profile, "shiplivery") ?? "default";
            _lockerBoard.text = "== YOUR LOCKER ==\n\ntaser: " + taser + "\ngravity: " + grav +
                                "\nlivery: " + livery + "\n\n(everything here is a LOOK,\nnever a stat)";
        }

        // ── Primitive helpers ────────────────────────────────────────────────

        private GameObject Cube(string name, Vector3 localPos, Vector3 scale, Color color, bool collider)
            => CubeUnder(transform, name, localPos, scale, color, collider);

        private static GameObject CubeUnder(Transform parent, string name, Vector3 localPos, Vector3 scale, Color color, bool collider)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = scale;
            var c = go.GetComponent<Collider>();
            if (c != null) c.enabled = collider;
            Paint(go, color);
            return go;
        }

        private GameObject MakePanelUnder(Transform parent, string name, Vector3 localPos, string label,
                                          Color color, System.Action onSelect, bool small = false)
        {
            var root = new GameObject(name);
            root.transform.SetParent(parent, false);
            root.transform.localPosition = localPos;

            var plate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            plate.name = "Plate";
            plate.transform.SetParent(root.transform, false);
            plate.transform.localScale = small ? new Vector3(1.0f, 0.18f, 0.04f) : new Vector3(0.7f, 0.3f, 0.05f);
            Paint(plate, color);
            var renderer = plate.GetComponent<Renderer>();

            var interactable = plate.AddComponent<XRSimpleInteractable>();
            var mgr = Object.FindObjectOfType<XRInteractionManager>();
            if (mgr != null) interactable.interactionManager = mgr;
            interactable.selectEntered.AddListener(_ => onSelect?.Invoke());
            interactable.hoverEntered.AddListener(_ => Tint(renderer, BayHot));
            interactable.hoverExited.AddListener(_ => Tint(renderer, color));

            var textGo = new GameObject("Label");
            var tm = textGo.AddComponent<TextMesh>();
            tm.text = label;
            tm.characterSize = small ? 0.024f : 0.04f;
            tm.fontSize = 48;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.color = Color.white;
            textGo.transform.SetParent(root.transform, false); // unit-scale parent — no stretch
            textGo.transform.localPosition = new Vector3(0f, 0f, -0.05f);

            return root;
        }

        private static void Paint(GameObject go, Color color)
        {
            var r = go.GetComponent<Renderer>();
            if (r == null) return;
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            if (shader == null) return;
            var mat = new Material(shader);
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", color);
            else if (mat.HasProperty("_Color")) mat.SetColor("_Color", color);
            r.sharedMaterial = mat;
            r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }

        private static void Tint(Renderer r, Color color)
        {
            if (r == null || r.material == null) return;
            if (r.material.HasProperty("_BaseColor")) r.material.SetColor("_BaseColor", color);
            else if (r.material.HasProperty("_Color")) r.material.color = color;
        }
    }
}
