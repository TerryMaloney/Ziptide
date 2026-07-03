using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>Asset lifecycle (reconciliation R2). Proxy = playable production contract;
    /// Locked = pinned behind a human-baked content hash — it cannot silently change.</summary>
    public enum ForgeQualityState
    {
        Proxy, ProxyPlus, ProductionCandidate, ProductionReady, NeedsRegen, Deprecated, Locked
    }

    /// <summary>Closed op set — the whole shape vocabulary of the Forge. schemaVersion guards it:
    /// an LLM inventing a new op fails Validate(), never silently no-ops.</summary>
    public enum ForgeOp
    {
        BeveledBox,     // size = full extents; bevel = chamfer width (0 = plain box)
        Cylinder,       // size.x = diameter, size.y = height; segments radial
        Tube,           // Cylinder with wallThickness; open-ended ring walls capped
        Lathe,          // profile = (radius, height01) points revolved around local Y; size.x = radius scale, size.y = height
        SphereSection,  // size = full extents of the ellipsoid; bevel = latitude fraction kept from the top (1 = full, 0.5 = dome)
        Wedge,          // right triangular prism: size.x wide, size.y tall (slope from +Z top to -Z bottom), size.z deep
        GreebleStrip    // segments small deterministic boxes in a row along local Z inside the size envelope
    }

    /// <summary>One placed shape. Class (not struct) so field initializers give LLM-safe defaults.</summary>
    [System.Serializable]
    public class ForgePart
    {
        public string name = "part";
        public ForgeOp op = ForgeOp.BeveledBox;
        [Tooltip("Full extents in meters (Lathe: x = radius scale, y = height).")]
        public Vector3 size = new Vector3(0.1f, 0.1f, 0.1f);
        [Tooltip("BeveledBox: chamfer width. SphereSection: latitude fraction from top (0..1].")]
        public float bevel = 0f;
        [Tooltip("Radial segments (Cylinder/Tube/Lathe/SphereSection) or box count (GreebleStrip). 3..16.")]
        public int segments = 8;
        [Tooltip("Tube only: wall thickness in meters.")]
        public float wallThickness = 0.01f;
        [Tooltip("Lathe only: (radius01, height01) points, 2..8, bottom to top.")]
        public Vector2[] profile;
        public Vector3 position = Vector3.zero;
        public Vector3 eulerRotation = Vector3.zero;
        public Vector3 scale = Vector3.one;
        [Tooltip("Emit a second copy mirrored across local X (winding corrected).")]
        public bool mirrorX = false;
        [Tooltip("Index into the recipe palette (0..5).")]
        public int paletteSlot = 0;
        [Tooltip("false = flat-shaded (crisp low-poly read, the default); true = smooth normals.")]
        public bool smooth = false;
    }

    /// <summary>Named attach point (Grip/Muzzle/Seat/Door/...). Consumers snap existing children here —
    /// for handhelds the Quest controller forward-tilt (~+45° X on Grip) is baked into localEuler,
    /// per docs/systems/ASSET_SWAP_PIPELINE.md.</summary>
    [System.Serializable]
    public class ForgeSocket
    {
        public string name = "Socket";
        public Vector3 localPosition = Vector3.zero;
        public Vector3 localEuler = Vector3.zero;
    }

    /// <summary>
    /// A complete Forge asset as data: parts + palette + sockets + budget + story tags. Written by LLM
    /// sessions in ForgeRecipeLibrary (create-only code authoring), built deterministically by
    /// <see cref="ForgeMesh"/>, photographed by ForgePhotoBooth, consumed at runtime via
    /// ForgeVisualApplier. The asset under Resources/Forge is the live, editable truth.
    /// </summary>
    [CreateAssetMenu(fileName = "ForgeRecipe", menuName = "Ziptide/Forge Recipe")]
    public class ForgeRecipeDefinition : ScriptableObject
    {
        public const int MaxParts = 24;
        public const int MaxPaletteSlots = 6;
        public const int CurrentSchemaVersion = 2; // v2 = FORGE II slotStyles; v1 assets stay valid
        public const float MaxPartExtent = 4f;

        [Tooltip("Stable id, lowercase snake_case (e.g. 'taser_gun_mk1'). Unique across Resources/Forge.")]
        public string recipeId = "recipe";
        public int schemaVersion = CurrentSchemaVersion;
        [Tooltip("Surface family key from ART_DIRECTION_MASTER_PLAN.md — palette conformance is tested.")]
        public string surfaceFamily = ForgePalettes.FamilySalvage;
        public string[] storyTags;
        [Tooltip("Up to 6 colors; parts reference slots by index. Fewer colors = fewer draw calls.")]
        public Color[] palette = { Color.gray };
        [Tooltip("FORGE II: material style per palette slot (parallel to palette; missing/short = " +
                 "PaintedMetal defaults). Drives the texture bake: wear/grime/panels/cells/emissive.")]
        public ForgeStyleSpec[] slotStyles;
        [Tooltip("Hard triangle budget — over it, tests and the audit fail the build.")]
        public int budgetTris = 3000;
        public ForgePart[] parts;
        public ForgeSocket[] sockets;

        [Header("Lifecycle (reconciliation R2 — see ASSET_FORGE_MAP.md)")]
        [Tooltip("Proxy → … → ProductionReady. Locked = pinned behind lockedContentHash (audit-enforced). " +
                 "Deprecated = story no longer uses it; never auto-deleted.")]
        public ForgeQualityState qualityState = ForgeQualityState.Proxy;
        [Tooltip("Human-baked approval baseline (Ziptide → Art → Lock Selected Forge Recipe). Locked " +
                 "assets whose current content hash differs = FORGE_LOCKED_DRIFT build blocker.")]
        public string lockedContentHash = "";
        [Tooltip("Prose for humans — the dependency auditor NEVER parses this.")]
        [TextArea] public string storyRole = "";
        [Tooltip("Structured canon anchors (e.g. W001, rill_teal, bloom) — staleness detection input.")]
        public string[] storyRefs;
        [Tooltip("World/scene ids whose rules this asset depends on (e.g. ToxicCity, W002_DryCistern).")]
        public string[] worldRuleRefs;
        [Tooltip("Style/family tokens beyond surfaceFamily (e.g. glow_teal, rusted_metal).")]
        public string[] tokenRefs;

        /// <summary>
        /// Deterministic content hash over everything that defines the LOOK (parts, palette, styles,
        /// sockets, budget, family). Pure + platform-stable (FNV-1a over a canonical string; fixed
        /// number formatting). Lifecycle fields themselves are excluded so locking doesn't change
        /// the hash it pins.
        /// </summary>
        public string ComputeContentHash()
        {
            var sb = new StringBuilder(1024);
            sb.Append(recipeId).Append('|').Append(surfaceFamily).Append('|').Append(budgetTris).Append('|');
            if (palette != null)
                foreach (var c in palette) AppendColor(sb, c);
            if (slotStyles != null)
                foreach (var s in slotStyles)
                {
                    if (s == null) { sb.Append("~|"); continue; }
                    sb.Append((int)s.style).Append(',').Append(F(s.wear)).Append(',').Append(F(s.grime))
                      .Append(',').Append(F(s.panelDensity)).Append(',').Append(F(s.cellSize)).Append(',');
                    AppendColor(sb, s.emissive);
                    sb.Append(F(s.emissiveIntensity)).Append('|');
                }
            if (parts != null)
                foreach (var p in parts)
                {
                    if (p == null) { sb.Append("~|"); continue; }
                    sb.Append(p.name).Append(',').Append((int)p.op).Append(',');
                    AppendVec(sb, p.size); sb.Append(F(p.bevel)).Append(',').Append(p.segments).Append(',')
                      .Append(F(p.wallThickness)).Append(',');
                    if (p.profile != null)
                        foreach (var pt in p.profile) sb.Append(F(pt.x)).Append(':').Append(F(pt.y)).Append(';');
                    AppendVec(sb, p.position); AppendVec(sb, p.eulerRotation); AppendVec(sb, p.scale);
                    sb.Append(p.mirrorX ? 1 : 0).Append(',').Append(p.paletteSlot).Append(',')
                      .Append(p.smooth ? 1 : 0).Append('|');
                }
            if (sockets != null)
                foreach (var s in sockets)
                {
                    if (s == null) { sb.Append("~|"); continue; }
                    sb.Append(s.name).Append(',');
                    AppendVec(sb, s.localPosition); AppendVec(sb, s.localEuler); sb.Append('|');
                }
            return Fnv1a64(sb.ToString());
        }

        private static string F(float v) => v.ToString("F4", System.Globalization.CultureInfo.InvariantCulture);
        private static void AppendVec(StringBuilder sb, Vector3 v)
        { sb.Append(F(v.x)).Append(',').Append(F(v.y)).Append(',').Append(F(v.z)).Append(','); }
        private static void AppendColor(StringBuilder sb, Color c)
        { sb.Append(F(c.r)).Append(',').Append(F(c.g)).Append(',').Append(F(c.b)).Append(','); }

        private static string Fnv1a64(string s)
        {
            unchecked
            {
                ulong h = 14695981039346656037UL;
                foreach (char c in s) { h ^= c; h *= 1099511628211UL; }
                return h.ToString("x16");
            }
        }

        /// <summary>Pure validation; empty list = well-formed. Mirrors the SkyVista convention.</summary>
        public List<string> Validate()
        {
            var issues = new List<string>();
            if (string.IsNullOrEmpty(recipeId)) issues.Add("empty recipeId");
            if (schemaVersion < 1 || schemaVersion > CurrentSchemaVersion)
                issues.Add("unknown schemaVersion " + schemaVersion);
            if (!ForgePalettes.IsKnownFamily(surfaceFamily)) issues.Add("unknown surfaceFamily '" + surfaceFamily + "'");
            if (palette == null || palette.Length < 1 || palette.Length > MaxPaletteSlots)
                issues.Add("palette must have 1.." + MaxPaletteSlots + " colors");
            if (parts == null || parts.Length == 0) issues.Add("no parts");
            else if (parts.Length > MaxParts) issues.Add("more than " + MaxParts + " parts");
            if (budgetTris <= 0) issues.Add("budgetTris must be positive");
            if (qualityState == ForgeQualityState.Locked && string.IsNullOrEmpty(lockedContentHash))
                issues.Add("Locked without a lockedContentHash — bake it via Ziptide > Art > Lock Selected Forge Recipe");

            if (parts != null)
                for (int i = 0; i < parts.Length; i++)
                {
                    var p = parts[i];
                    string tag = "part " + i + " (" + (p == null ? "null" : p.name) + "): ";
                    if (p == null) { issues.Add(tag + "null"); continue; }
                    if (palette != null && (p.paletteSlot < 0 || p.paletteSlot >= palette.Length))
                        issues.Add(tag + "paletteSlot " + p.paletteSlot + " out of range");
                    if (p.segments < 3 || p.segments > 16) issues.Add(tag + "segments out of 3..16");
                    if (p.size.x <= 0f || p.size.y <= 0f || p.size.z <= 0f) issues.Add(tag + "non-positive size");
                    if (Mathf.Max(p.size.x, Mathf.Max(p.size.y, p.size.z)) > MaxPartExtent)
                        issues.Add(tag + "size exceeds " + MaxPartExtent + "m");
                    if (p.op == ForgeOp.Lathe && (p.profile == null || p.profile.Length < 2 || p.profile.Length > 8))
                        issues.Add(tag + "Lathe profile needs 2..8 points");
                    if (p.op == ForgeOp.Lathe && p.profile != null)
                        foreach (var pt in p.profile)
                            if (pt.x < 0f) issues.Add(tag + "Lathe profile radius < 0");
                    if (p.op == ForgeOp.SphereSection && (p.bevel <= 0f || p.bevel > 1f))
                        issues.Add(tag + "SphereSection latitude fraction (bevel) must be in (0,1]");
                    if (p.op == ForgeOp.Tube && (p.wallThickness <= 0f || p.wallThickness * 2f >= p.size.x))
                        issues.Add(tag + "Tube wallThickness must be >0 and < radius");
                }

            if (sockets != null)
            {
                var names = new HashSet<string>();
                foreach (var s in sockets)
                {
                    if (s == null || string.IsNullOrEmpty(s.name)) { issues.Add("socket with empty name"); continue; }
                    if (!names.Add(s.name)) issues.Add("duplicate socket '" + s.name + "'");
                }
                if (names.Contains("Grip") && !names.Contains("Muzzle"))
                    issues.Add("handheld contract: a Grip socket requires a Muzzle socket");
                if (storyTags != null && System.Array.IndexOf(storyTags, "creature") >= 0
                    && !names.Contains("WeakPoint"))
                    issues.Add("creature contract (PROXY_CONTRACTS.md): a WeakPoint socket is required");
            }
            else if (storyTags != null && System.Array.IndexOf(storyTags, "creature") >= 0)
                issues.Add("creature contract (PROXY_CONTRACTS.md): a WeakPoint socket is required");

            return issues;
        }
    }
}
