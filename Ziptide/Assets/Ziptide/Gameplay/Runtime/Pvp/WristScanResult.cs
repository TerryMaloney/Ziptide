using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Immutable pulse-time view of one scannable. Identity and transform are preserved while kind,
    /// position and distance are copied at the moment the scanner pulse is evaluated.
    /// </summary>
    public readonly struct WristScanTarget
    {
        public WristScanTarget(
            IScannable source,
            Transform scanTransform,
            ScanKind kind,
            Vector3 position,
            float distance)
        {
            Source = source;
            ScanTransform = scanTransform;
            Kind = kind;
            Position = position;
            Distance = distance;
        }

        public IScannable Source { get; }
        public Transform ScanTransform { get; }
        public ScanKind Kind { get; }
        public Vector3 Position { get; }
        public float Distance { get; }
    }

    /// <summary>
    /// Immutable exact result of one real wrist-scanner pulse. The membership/order and copied
    /// pulse-time values cannot be changed after construction.
    /// </summary>
    public sealed class WristScanResult
    {
        private static readonly WristScanTarget[] NoTargets = new WristScanTarget[0];
        private static readonly ReadOnlyCollection<WristScanTarget> NoTargetsView =
            Array.AsReadOnly(NoTargets);

        public static WristScanResult Empty { get; } = new WristScanResult(NoTargetsView, copy: false);

        private readonly ReadOnlyCollection<WristScanTarget> _targets;

        public WristScanResult(IEnumerable<WristScanTarget> targets)
        {
            if (targets == null)
            {
                _targets = NoTargetsView;
                return;
            }

            var copy = new List<WristScanTarget>(targets).ToArray();
            _targets = copy.Length == 0 ? NoTargetsView : Array.AsReadOnly(copy);
        }

        private WristScanResult(ReadOnlyCollection<WristScanTarget> targets, bool copy)
        {
            _targets = targets ?? NoTargetsView;
        }

        public ReadOnlyCollection<WristScanTarget> Targets => _targets;
        public int Count => _targets.Count;

        /// <summary>
        /// Applies the scanner's existing filter without reordering candidates: active, non-null
        /// transform, and distance less than or equal to range.
        /// </summary>
        public static WristScanResult Capture(
            IEnumerable<IScannable> candidates,
            Vector3 origin,
            float range)
        {
            if (candidates == null) return Empty;

            var captured = new List<WristScanTarget>();
            foreach (var candidate in candidates)
            {
                if (candidate == null || !candidate.ScanActive) continue;

                Transform scanTransform = candidate.ScanTransform;
                if (scanTransform == null) continue;

                float distance = Vector3.Distance(scanTransform.position, origin);
                if (distance > range) continue;

                captured.Add(new WristScanTarget(
                    candidate,
                    scanTransform,
                    candidate.ScanKind,
                    scanTransform.position,
                    distance));
            }

            return captured.Count == 0 ? Empty : new WristScanResult(captured);
        }

        /// <summary>
        /// Invokes every subscriber independently. One failing subscriber cannot block later
        /// subscribers or propagate back into the scanner pulse path.
        /// </summary>
        public static int PublishSafely(
            Action<WristScanResult> subscribers,
            WristScanResult result,
            Action<Exception> onSubscriberFailure = null)
        {
            if (subscribers == null) return 0;
            if (result == null) result = Empty;

            int successful = 0;
            foreach (Action<WristScanResult> subscriber in subscribers.GetInvocationList())
            {
                try
                {
                    subscriber(result);
                    successful++;
                }
                catch (Exception ex)
                {
                    onSubscriberFailure?.Invoke(ex);
                }
            }

            return successful;
        }

        public string KindSummary()
        {
            if (_targets.Count == 0) return "none";

            int enemy = 0;
            int objective = 0;
            int loot = 0;
            int node = 0;

            for (int i = 0; i < _targets.Count; i++)
            {
                switch (_targets[i].Kind)
                {
                    case ScanKind.Enemy: enemy++; break;
                    case ScanKind.Objective: objective++; break;
                    case ScanKind.Loot: loot++; break;
                    case ScanKind.Node: node++; break;
                }
            }

            var summary = new StringBuilder();
            AppendKind(summary, "enemy", enemy);
            AppendKind(summary, "objective", objective);
            AppendKind(summary, "loot", loot);
            AppendKind(summary, "node", node);
            return summary.Length == 0 ? "none" : summary.ToString();
        }

        private static void AppendKind(StringBuilder summary, string name, int count)
        {
            if (count <= 0) return;
            if (summary.Length > 0) summary.Append(',');
            summary.Append(name).Append(':').Append(count);
        }
    }
}
