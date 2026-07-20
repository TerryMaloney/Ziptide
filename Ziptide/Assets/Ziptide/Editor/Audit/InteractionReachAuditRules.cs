#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Ziptide.Editor.Audit
{
    public enum ReachGateResult
    {
        Pass,
        NoSupport,
        TooLow,
        TooHigh,
        TooSmall,
    }

    /// <summary>
    /// PG-4: child-reach and target-size contract for fixed critical controls. Grabbable world objects are
    /// excluded; this gate covers buttons, tiles, travel rows and switches that cannot be repositioned by
    /// the player. Measurements use the final collider bounds and sampled floor support.
    /// </summary>
    public static class InteractionReachAuditRules
    {
        public const string NoSupport = "INTERACTABLE_REACH_NO_SUPPORT";
        public const string TooLow = "INTERACTABLE_REACH_TOO_LOW";
        public const string TooHigh = "INTERACTABLE_REACH_TOO_HIGH";
        public const string TooSmall = "INTERACTABLE_TARGET_TOO_SMALL";
        public const float GeneralMaxCenterHeight = 1.55f;
        public const float PowerMaxCenterHeight = 1.08f;
        public const float MinimumCenterHeight = 0.28f;
        public const float MinimumTargetWidth = 0.08f;

        public static void Run(SceneAuditReport report)
        {
            Physics.SyncTransforms();
            foreach (XRSimpleInteractable interactable in Object.FindObjectsOfType<XRSimpleInteractable>(true))
            {
                if (interactable == null || interactable.GetComponent<XRGrabInteractable>() != null) continue;
                if (!IsCritical(interactable.name)) continue;
                Collider target = FindTargetCollider(interactable);
                if (target == null)
                {
                    report.Blocker(TooSmall,
                        GetPath(interactable.transform) + " has no enabled collider target.",
                        GetPath(interactable.transform));
                    continue;
                }

                bool hasSupport = TrySupportBelow(target.bounds.center, interactable.transform,
                    out float supportY);
                float height = hasSupport ? target.bounds.center.y - supportY : 0f;
                float width = Mathf.Max(target.bounds.size.x, target.bounds.size.z);
                bool powerSwitch = interactable.name.IndexOf("PowerSwitch",
                    System.StringComparison.OrdinalIgnoreCase) >= 0;
                float maxHeight = powerSwitch ? PowerMaxCenterHeight : GeneralMaxCenterHeight;
                ReachGateResult result = Evaluate(height, width, hasSupport, maxHeight);
                string path = GetPath(interactable.transform);
                switch (result)
                {
                    case ReachGateResult.NoSupport:
                        report.Blocker(NoSupport, path + " has no solid floor support within 3m.", path);
                        break;
                    case ReachGateResult.TooLow:
                        report.Blocker(TooLow, path + " target center is " + height.ToString("F2")
                            + "m above support; minimum is " + MinimumCenterHeight.ToString("F2") + "m.", path);
                        break;
                    case ReachGateResult.TooHigh:
                        report.Blocker(TooHigh, path + " target center is " + height.ToString("F2")
                            + "m above support; maximum is " + maxHeight.ToString("F2") + "m.", path);
                        break;
                    case ReachGateResult.TooSmall:
                        report.Blocker(TooSmall, path + " target width is " + width.ToString("F2")
                            + "m; minimum is " + MinimumTargetWidth.ToString("F2") + "m.", path);
                        break;
                }
            }
        }

        public static ReachGateResult Evaluate(float centerHeight, float targetWidth,
            bool hasSupport, float maxHeight = GeneralMaxCenterHeight)
        {
            if (!hasSupport) return ReachGateResult.NoSupport;
            if (centerHeight < MinimumCenterHeight) return ReachGateResult.TooLow;
            if (centerHeight > maxHeight) return ReachGateResult.TooHigh;
            if (targetWidth < MinimumTargetWidth) return ReachGateResult.TooSmall;
            return ReachGateResult.Pass;
        }

        public static bool IsCritical(string objectName)
        {
            if (string.IsNullOrEmpty(objectName)) return false;
            string name = objectName.ToLowerInvariant();
            return name.StartsWith("tile_")
                || name.StartsWith("dest_")
                || name.Contains("powerswitch")
                || name.Contains("punch")
                || name.Contains("boardpanel")
                || name.Contains("disembark")
                || name.Contains("travelbutton")
                || name.Contains("destinationbutton");
        }

        private static Collider FindTargetCollider(XRSimpleInteractable interactable)
        {
            Collider own = interactable.GetComponent<Collider>();
            if (own != null && own.enabled) return own;
            Collider[] children = interactable.GetComponentsInChildren<Collider>(true);
            for (int i = 0; i < children.Length; i++)
                if (children[i] != null && children[i].enabled) return children[i];
            return null;
        }

        private static bool TrySupportBelow(Vector3 targetCenter, Transform owner, out float supportY)
        {
            RaycastHit[] hits = Physics.RaycastAll(targetCenter + Vector3.up * 0.10f,
                Vector3.down, 3.10f, ~0, QueryTriggerInteraction.Ignore);
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
            for (int i = 0; i < hits.Length; i++)
            {
                Collider collider = hits[i].collider;
                if (collider == null || !collider.enabled || collider.isTrigger) continue;
                if (collider.transform == owner || collider.transform.IsChildOf(owner)
                    || owner.IsChildOf(collider.transform)) continue;
                if (hits[i].normal.y < 0.35f) continue;
                supportY = hits[i].point.y;
                return true;
            }
            supportY = 0f;
            return false;
        }

        private static string GetPath(Transform value)
        {
            if (value == null) return string.Empty;
            string path = value.name;
            while (value.parent != null)
            {
                value = value.parent;
                path = value.name + "/" + path;
            }
            return path;
        }
    }
}
#endif
