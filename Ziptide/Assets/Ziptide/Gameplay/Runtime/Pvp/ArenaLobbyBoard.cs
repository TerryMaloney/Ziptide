using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Multiplayer.Modes;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// THE MATCH BOARD (M7a A3-scene) — a physical panel near the arena spawn: pick a MODE, a bot
    /// DIFFICULTY, a bot COUNT, then hit START. Tiles are XRSimpleInteractables (the travel-door
    /// idiom: ray-select, hover tint) built entirely at runtime, so the patcher only places this one
    /// component. Selection writes <see cref="ArenaMatchConfig"/> (static — survives restarts) and
    /// START routes through <see cref="PvpModeDirector.Restart"/>. Arena choice stays with the travel
    /// doors — this board is everything else.
    /// </summary>
    public class ArenaLobbyBoard : MonoBehaviour
    {
        private const float TileW = 0.55f, TileH = 0.28f, TileGap = 0.08f, RowGap = 0.14f;

        private static readonly Color PanelColor = new Color(0.10f, 0.12f, 0.15f);
        private static readonly Color TileColor = new Color(0.16f, 0.30f, 0.38f);
        private static readonly Color TileHover = new Color(0.24f, 0.44f, 0.55f);
        private static readonly Color TileSelected = new Color(0.90f, 0.70f, 0.18f);
        private static readonly Color StartColor = new Color(0.18f, 0.55f, 0.28f);
        private static readonly Color LabelColor = new Color(0.88f, 0.94f, 1f);

        private struct Tile { public Renderer R; public int Row; public int Col; }
        private readonly List<Tile> _tiles = new List<Tile>();
        private int _selMode, _selDiff = 1, _selBots; // defaults: Deathmatch / regular / 1 bot

        private static readonly (string label, PvpModeKind kind)[] Modes =
        {
            ("DEATHMATCH", PvpModeKind.Deathmatch),
            ("GUN GAME", PvpModeKind.GunGame),
            ("KOTH", PvpModeKind.KingOfTheHill),
            ("FRAGMENT", PvpModeKind.FragmentRush),
            ("HORDE", PvpModeKind.Horde),
        };
        private static readonly string[] Difficulties = { "rookie", "regular", "veteran", "nightmare" };
        private static readonly string[] BotCounts = { "1 BOT", "2 BOTS", "3 BOTS" };

        private void Start()
        {
            Build();
        }

        private void Build()
        {
            // Backboard
            float w = Modes.Length * (TileW + TileGap) + TileGap;
            float h = 4f * (TileH + RowGap) + 0.5f;
            var panel = GameObject.CreatePrimitive(PrimitiveType.Cube);
            panel.name = "BoardPanel";
            panel.transform.SetParent(transform, false);
            panel.transform.localPosition = new Vector3(0f, 1.5f, 0.03f);
            panel.transform.localScale = new Vector3(w, h, 0.05f);
            panel.GetComponent<Collider>().enabled = false;
            Paint(panel, PanelColor);

            MakeLabel("MATCH BOARD", new Vector3(0f, 1.5f + h / 2f + 0.12f, 0f), 0.09f);

            BuildRow(0, Row(0), ModeLabels());
            BuildRow(1, Row(1), UpperAll(Difficulties));
            BuildRow(2, Row(2), BotCounts);
            BuildStart(new Vector3(0f, Row(3), 0f));

            RefreshTints();
        }

        private static float Row(int i) => 2.35f - i * (TileH + RowGap);

        private static string[] ModeLabels()
        {
            var l = new string[Modes.Length];
            for (int i = 0; i < Modes.Length; i++) l[i] = Modes[i].label;
            return l;
        }

        private static string[] UpperAll(string[] src)
        {
            var l = new string[src.Length];
            for (int i = 0; i < src.Length; i++) l[i] = src[i].ToUpperInvariant();
            return l;
        }

        private void BuildRow(int row, float y, string[] labels)
        {
            float startX = -(labels.Length - 1) * (TileW + TileGap) * 0.5f;
            for (int col = 0; col < labels.Length; col++)
            {
                int r = row, c = col;
                var tile = MakeTile(labels[col], new Vector3(startX + col * (TileW + TileGap), y, 0f),
                    TileColor, () => OnPick(r, c));
                _tiles.Add(new Tile { R = tile, Row = row, Col = col });
            }
        }

        private void BuildStart(Vector3 pos)
        {
            MakeTile("START MATCH", pos, StartColor, () =>
            {
                ArenaMatchConfig.Mode = Modes[_selMode].kind;
                ArenaMatchConfig.Difficulty = Difficulties[_selDiff];
                ArenaMatchConfig.BotCount = _selBots + 1;
                Debug.Log("ZIPTIDE: LOBBY_START mode=" + ArenaMatchConfig.Mode
                    + " difficulty=" + ArenaMatchConfig.Difficulty + " bots=" + ArenaMatchConfig.BotCount);
                PvpModeDirector.Instance?.Restart();
            }, wide: true);
        }

        private void OnPick(int row, int col)
        {
            if (row == 0) _selMode = col;
            else if (row == 1) _selDiff = col;
            else if (row == 2) _selBots = col;
            RefreshTints();
        }

        private void RefreshTints()
        {
            foreach (var t in _tiles)
            {
                bool sel = (t.Row == 0 && t.Col == _selMode)
                        || (t.Row == 1 && t.Col == _selDiff)
                        || (t.Row == 2 && t.Col == _selBots);
                if (t.R != null) Paint(t.R.gameObject, sel ? TileSelected : TileColor);
            }
        }

        // ── Construction helpers (the travel-door idiom) ─────────────────────────────────────────
        private Renderer MakeTile(string label, Vector3 localPos, Color color, System.Action onSelect, bool wide = false)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = "Tile_" + label.Replace(' ', '_');
            go.transform.SetParent(transform, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = new Vector3(wide ? TileW * 2f : TileW, TileH, 0.06f);
            Paint(go, color);

            var interactable = go.AddComponent<XRSimpleInteractable>();
            var mgr = FindObjectOfType<XRInteractionManager>();
            if (mgr != null) interactable.interactionManager = mgr;
            var r = go.GetComponent<Renderer>();
            interactable.selectEntered.AddListener(_ => onSelect());
            interactable.hoverEntered.AddListener(_ => Paint(go, TileHover));
            interactable.hoverExited.AddListener(_ => RefreshTintsAndStart(go, color));

            MakeLabel(label, localPos + new Vector3(0f, 0f, -0.05f), 0.045f);
            return r;
        }

        private void RefreshTintsAndStart(GameObject go, Color baseColor)
        {
            // Hover-exit restores: selected tiles re-tint gold via RefreshTints; START keeps its green.
            if (go.name == "Tile_START_MATCH") Paint(go, baseColor);
            else RefreshTints();
        }

        private GameObject MakeLabel(string text, Vector3 localPos, float size)
        {
            var go = new GameObject("Label_" + text.Replace(' ', '_'));
            go.transform.SetParent(transform, false);
            go.transform.localPosition = localPos;
            go.transform.localScale = Vector3.one; // TextMesh scale stays neutral (stretch gotcha)
            var tm = go.AddComponent<TextMesh>();
            tm.text = text;
            tm.characterSize = size;
            tm.fontSize = 64;
            tm.anchor = TextAnchor.MiddleCenter;
            tm.alignment = TextAlignment.Center;
            tm.color = LabelColor;
            return go;
        }

        private static void Paint(GameObject go, Color c)
        {
            ItemFactory.ApplyURPColor(go, c);
            var r = go.GetComponent<Renderer>();
            if (r != null) r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        }
    }
}
