using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Visuals
{
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
        public const int CurrentSchemaVersion = 1;
        public const float MaxPartExtent = 4f;

        [Tooltip("Stable id, lowercase snake_case (e.g. 'taser_gun_mk1'). Unique across Resources/Forge.")]
        public string recipeId = "recipe";
        public int schemaVersion = CurrentSchemaVersion;
        [Tooltip("Surface family key from ART_DIRECTION_MASTER_PLAN.md — palette conformance is tested.")]
        public string surfaceFamily = ForgePalettes.FamilySalvage;
        public string[] storyTags;
        [Tooltip("Up to 6 colors; parts reference slots by index. Fewer colors = fewer draw calls.")]
        public Color[] palette = { Color.gray };
        [Tooltip("Hard triangle budget — over it, tests and the audit fail the build.")]
        public int budgetTris = 3000;
        public ForgePart[] parts;
        public ForgeSocket[] sockets;

        /// <summary>Pure validation; empty list = well-formed. Mirrors the SkyVista convention.</summary>
        public List<string> Validate()
        {
            var issues = new List<string>();
            if (string.IsNullOrEmpty(recipeId)) issues.Add("empty recipeId");
            if (schemaVersion != CurrentSchemaVersion) issues.Add("unknown schemaVersion " + schemaVersion);
            if (!ForgePalettes.IsKnownFamily(surfaceFamily)) issues.Add("unknown surfaceFamily '" + surfaceFamily + "'");
            if (palette == null || palette.Length < 1 || palette.Length > MaxPaletteSlots)
                issues.Add("palette must have 1.." + MaxPaletteSlots + " colors");
            if (parts == null || parts.Length == 0) issues.Add("no parts");
            else if (parts.Length > MaxParts) issues.Add("more than " + MaxParts + " parts");
            if (budgetTris <= 0) issues.Add("budgetTris must be positive");

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
            }

            return issues;
        }
    }
}
