using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>Which motion program (FORGE II P4 ForgeMotor) drives a limb's bones.</summary>
    public enum GaitRole { None, Leg, Tentacle, Wing, Tail, Antenna }

    /// <summary>One rigid segment of a limb chain — a beveled box between two joints.</summary>
    [System.Serializable]
    public class ForgeLimbSegment
    {
        [Tooltip("x = width, y = LENGTH along the chain, z = depth.")]
        public Vector3 size = new Vector3(0.06f, 0.3f, 0.07f);
        public int paletteSlot;
    }

    /// <summary>
    /// A bone chain hanging off the creature core: attach point + direction + segments.
    /// Each segment gets its own bone so the P4 motor can drive knees/waves per joint.
    /// </summary>
    [System.Serializable]
    public class ForgeLimb
    {
        public string name = "Limb";
        [Tooltip("Chain start, local to the body root.")]
        public Vector3 attachLocal;
        [Tooltip("Chain direction at build pose (normalized at build). Down-and-out reads as a stance.")]
        public Vector3 chainDirection = Vector3.down;
        public GaitRole role = GaitRole.Leg;
        [Tooltip("Mirror across X: builds a second chain (and bones) at -x.")]
        public bool mirrorX;
        public ForgeLimbSegment[] segments;
    }

    /// <summary>
    /// FORGE II P3 — the creature GENOME: core parts (torso/head/shell — rigid, bone 0) + limb
    /// chains (one bone per segment) + palette/styles + the emissive eye. ForgeSkinnedBuilder
    /// turns this into ONE rigid-weighted SkinnedMeshRenderer (1 draw call). Same laws as
    /// ForgeRecipeDefinition: pure data, deterministic build, Validate() gates in tests/audit.
    /// </summary>
    [CreateAssetMenu(fileName = "ForgeCreatureBody", menuName = "Ziptide/Forge Creature Body")]
    public class ForgeCreatureBody : ScriptableObject
    {
        public const int MaxBones = 12;

        public string bodyId;
        [Tooltip("Rigid core (torso/head/shell) — all weighted to the ROOT bone.")]
        public ForgePart[] coreParts;
        public ForgeLimb[] limbs;

        public Color[] palette;
        public ForgeStyleSpec[] slotStyles;
        public int budgetTris = 10000; // creature class budget (FORGE II P2 table)

        [Header("The eye (emissive tell — every creature has one)")]
        public Vector3 eyeLocal = new Vector3(0f, 0.3f, 0.4f);
        public float eyeRadius = 0.05f;
        public int eyePaletteSlot = 3;

        /// <summary>Structural issues that make the body unbuildable or over-budget. Empty = good.</summary>
        public System.Collections.Generic.List<string> Validate()
        {
            var issues = new System.Collections.Generic.List<string>();
            if (string.IsNullOrEmpty(bodyId)) issues.Add("bodyId is empty");
            if (coreParts == null || coreParts.Length == 0) issues.Add("no core parts");
            if (palette == null || palette.Length == 0) issues.Add("no palette");
            int bones = BoneCount();
            if (bones > MaxBones)
                issues.Add("bone count " + bones + " exceeds MaxBones " + MaxBones + " (Quest skinning budget)");
            if (limbs != null)
                foreach (var l in limbs)
                {
                    if (l == null || l.segments == null || l.segments.Length == 0)
                        issues.Add("limb '" + (l != null ? l.name : "null") + "' has no segments");
                    else if (l.chainDirection.sqrMagnitude < 1e-6f)
                        issues.Add("limb '" + l.name + "' has a zero chain direction");
                }
            return issues;
        }

        /// <summary>Root + one bone per limb segment (mirrored limbs double their bones).</summary>
        public int BoneCount()
        {
            int n = 1;
            if (limbs != null)
                foreach (var l in limbs)
                    if (l != null && l.segments != null)
                        n += l.segments.Length * (l.mirrorX ? 2 : 1);
            return n;
        }
    }
}
