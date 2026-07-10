using UnityEngine;

namespace Ziptide.Visuals
{
    /// <summary>
    /// THE TELL BRIDGE (FORGE II P3 close): creature behaviors signal their gameplay READS — the
    /// warden's eye states, the witness-mite's stone-freeze, the husk-molter's shed decoy — through
    /// THIS component instead of recoloring their own primitive parts, so a forged skinned body
    /// carries every tell the primitives did. Added by <see cref="ForgeCreatureVisualApplier"/> next
    /// to the SkinnedMeshRenderer. Every static is graceful: no forged body → false/null and the
    /// behavior falls back to its primitive path (unforged creatures keep working unchanged).
    /// Materials are instanced lazily on first use, so one creature's tell never leaks to its family.
    /// Tell STATE is tracked in plain fields (EyeColor / IsBodyTinted) so the contract is CI-testable
    /// without shaders. A look, never a stat.
    /// </summary>
    public class ForgeBodyTell : MonoBehaviour
    {
        private SkinnedMeshRenderer _smr;
        private int _eyeIndex = -1;
        private Color[] _baseColors;
        private Material[] _instanced;

        /// <summary>Last eye color a behavior set (also the initial palette eye color).</summary>
        public Color EyeColor { get; private set; }
        public bool HasEye => _eyeIndex >= 0;
        public bool IsBodyTinted { get; private set; }
        public Color BodyTint { get; private set; }

        /// <summary>Applier wiring: the body's renderer, which material index is the EYE (-1 = none),
        /// and the palette base color per material (for ClearBodyTint restore).</summary>
        public void Init(SkinnedMeshRenderer smr, int eyeIndex, Color[] baseColors)
        {
            _smr = smr;
            _eyeIndex = eyeIndex;
            _baseColors = baseColors;
            if (eyeIndex >= 0 && baseColors != null && eyeIndex < baseColors.Length)
                EyeColor = baseColors[eyeIndex];
        }

        /// <summary>Drive the eye (base + emission) — the warden-class tell channel.</summary>
        public void SetEye(Color c)
        {
            EyeColor = c;
            var mats = Mats();
            if (!HasEye || mats == null || _eyeIndex >= mats.Length || mats[_eyeIndex] == null) return;
            Apply(mats[_eyeIndex], c);
            if (mats[_eyeIndex].HasProperty("_EmissionColor"))
                mats[_eyeIndex].SetColor("_EmissionColor", c * 2.2f);
        }

        /// <summary>Tint every non-eye material toward one color — the stone-freeze-class tell.
        /// Idempotent per color (gaze checks call this every frame).</summary>
        public void SetBodyTint(Color c)
        {
            if (IsBodyTinted && BodyTint == c) return;
            IsBodyTinted = true;
            BodyTint = c;
            var mats = Mats();
            if (mats == null) return;
            for (int i = 0; i < mats.Length; i++)
                if (i != _eyeIndex && mats[i] != null) Apply(mats[i], c);
        }

        /// <summary>Restore the palette base colors (the tint's counterpart).</summary>
        public void ClearBodyTint()
        {
            if (!IsBodyTinted) return;
            IsBodyTinted = false;
            var mats = Mats();
            if (mats == null || _baseColors == null) return;
            for (int i = 0; i < mats.Length && i < _baseColors.Length; i++)
                if (i != _eyeIndex && mats[i] != null) Apply(mats[i], _baseColors[i]);
        }

        private Material[] Mats()
        {
            if (_instanced != null) return _instanced;
            if (_smr == null) return null;
            _instanced = _smr.materials; // instantiates — shared palette materials stay shared elsewhere
            return _instanced;
        }

        private static void Apply(Material m, Color c)
        {
            if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
            else if (m.HasProperty("_Color")) m.color = c;
        }

        // ── The graceful statics behaviors call (false/null = keep your primitive path) ──

        public static bool TrySetEye(GameObject host, Color c)
        {
            var tell = Find(host);
            if (tell == null || !tell.HasEye) return false;
            tell.SetEye(c);
            return true;
        }

        public static bool TrySetBodyTint(GameObject host, Color c)
        {
            var tell = Find(host);
            if (tell == null) return false;
            tell.SetBodyTint(c);
            return true;
        }

        public static bool TryClearBodyTint(GameObject host)
        {
            var tell = Find(host);
            if (tell == null) return false;
            tell.ClearBodyTint();
            return true;
        }

        /// <summary>
        /// The husk-molter's decoy done right: a FROZEN, de-animated, tinted clone of the forged
        /// body — the shed skin looks like the creature, exactly as the trick demands. Null when the
        /// host isn't forged (spawn the primitive decoy instead). Caller owns placement + lifetime.
        /// </summary>
        public static GameObject TryCloneStatue(GameObject host, Color tint)
        {
            if (host == null) return null;
            var vis = host.transform.Find(ForgeCreatureVisualApplier.VisualChildName);
            if (vis == null) return null;

            var clone = Object.Instantiate(vis.gameObject);
            clone.name = "__ForgeHusk";
            var anim = clone.GetComponentInChildren<ForgeCreatureAnimator>();
            if (anim != null) SafeDestroy(anim);         // a husk never moves
            var cloneTell = clone.GetComponentInChildren<ForgeBodyTell>();
            if (cloneTell != null) SafeDestroy(cloneTell);
            var smr = clone.GetComponentInChildren<SkinnedMeshRenderer>();
            if (smr != null)
                foreach (var m in smr.materials)
                {
                    if (m == null) continue;
                    Apply(m, tint);
                    if (m.HasProperty("_EmissionColor")) m.SetColor("_EmissionColor", Color.black); // dead eye
                }
            return clone;
        }

        private static ForgeBodyTell Find(GameObject host)
            => host != null ? host.GetComponentInChildren<ForgeBodyTell>() : null;

        private static void SafeDestroy(Object o)
        {
            if (Application.isPlaying) Object.Destroy(o);
            else Object.DestroyImmediate(o); // EditMode tests exercise the clone path headless
        }
    }
}
