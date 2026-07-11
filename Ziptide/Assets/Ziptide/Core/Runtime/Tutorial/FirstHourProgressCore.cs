using System;
using System.Collections.Generic;

namespace Ziptide.Core
{
    /// <summary>
    /// Plain immutable input for <see cref="FirstHourProgressCore"/>. The Content assembly maps its
    /// generated FirstHourContractDefinition beats into this type; Core never references Content,
    /// Unity, files, profiles, clocks or scene state.
    /// </summary>
    public sealed class FirstHourProgressBeat
    {
        private readonly string[] _prerequisites;
        private readonly string[] _grantedFlags;

        public string Id { get; }
        public bool Required { get; }
        public IReadOnlyList<string> Prerequisites => _prerequisites;
        public string CompletionSignalId { get; }
        public IReadOnlyList<string> GrantedFlags => _grantedFlags;
        public double HintDelaySeconds { get; }
        public string HintLineId { get; }

        public FirstHourProgressBeat(
            string id,
            bool required,
            IEnumerable<string> prerequisites,
            string completionSignalId,
            IEnumerable<string> grantedFlags,
            double hintDelaySeconds,
            string hintLineId)
        {
            Id = id;
            Required = required;
            _prerequisites = Copy(prerequisites);
            CompletionSignalId = completionSignalId;
            _grantedFlags = Copy(grantedFlags);
            HintDelaySeconds = hintDelaySeconds;
            HintLineId = hintLineId;
        }

        private static string[] Copy(IEnumerable<string> source)
        {
            if (source == null) return Array.Empty<string>();
            var result = new List<string>();
            foreach (string value in source) result.Add(value);
            return result.ToArray();
        }
    }

    public enum FirstHourSignalResultCode
    {
        Accepted,
        Disabled,
        EmptySignal,
        Complete,
        Blocked,
        Duplicate,
        Early,
        Unknown
    }

    /// <summary>Immutable result from one signal attempt.</summary>
    public sealed class FirstHourSignalResult
    {
        private readonly string[] _grantedFlags;

        public FirstHourSignalResultCode Code { get; }
        public string SignalId { get; }
        public string CompletedBeatId { get; }
        public string CurrentBeatId { get; }
        public IReadOnlyList<string> GrantedFlags => _grantedFlags;
        public bool IsComplete { get; }
        public bool Advanced => Code == FirstHourSignalResultCode.Accepted;

        internal FirstHourSignalResult(
            FirstHourSignalResultCode code,
            string signalId,
            string completedBeatId,
            string currentBeatId,
            IEnumerable<string> grantedFlags,
            bool isComplete)
        {
            Code = code;
            SignalId = signalId;
            CompletedBeatId = completedBeatId;
            CurrentBeatId = currentBeatId;
            _grantedFlags = Copy(grantedFlags);
            IsComplete = isComplete;
        }

        private static string[] Copy(IEnumerable<string> source)
        {
            if (source == null) return Array.Empty<string>();
            var result = new List<string>();
            foreach (string value in source) result.Add(value);
            return result.ToArray();
        }
    }

    /// <summary>
    /// FH-X02 — deterministic first-hour progression brain.
    ///
    /// Laws:
    /// - ordered beat input + completed IDs in; no I/O, Unity, wall clock, logging or profile writes;
    /// - CurrentBeatId is the first reachable incomplete REQUIRED beat;
    /// - only the current beat's completion signal advances;
    /// - early/unknown/duplicate signals are explicit no-ops;
    /// - hints use caller-supplied elapsed time on the current beat and latch once until advancement;
    /// - invalid input disables safely with a stable reason code.
    /// </summary>
    public sealed class FirstHourProgressCore
    {
        public const string DisabledNone = "";
        public const string DisabledBeatsMissing = "FIRST_HOUR_PROGRESS_BEATS_MISSING";
        public const string DisabledBeatNull = "FIRST_HOUR_PROGRESS_BEAT_NULL";
        public const string DisabledBeatId = "FIRST_HOUR_PROGRESS_BEAT_ID_INVALID";
        public const string DisabledBeatDuplicate = "FIRST_HOUR_PROGRESS_BEAT_ID_DUPLICATE";
        public const string DisabledSignal = "FIRST_HOUR_PROGRESS_SIGNAL_INVALID";
        public const string DisabledSignalDuplicate = "FIRST_HOUR_PROGRESS_SIGNAL_DUPLICATE";
        public const string DisabledPrerequisite = "FIRST_HOUR_PROGRESS_PREREQUISITE_INVALID";
        public const string DisabledHintDelay = "FIRST_HOUR_PROGRESS_HINT_DELAY_INVALID";
        public const string DisabledFlag = "FIRST_HOUR_PROGRESS_FLAG_INVALID";

        private readonly List<FirstHourProgressBeat> _beats = new List<FirstHourProgressBeat>();
        private readonly Dictionary<string, FirstHourProgressBeat> _beatsById =
            new Dictionary<string, FirstHourProgressBeat>(StringComparer.Ordinal);
        private readonly Dictionary<string, FirstHourProgressBeat> _beatsBySignal =
            new Dictionary<string, FirstHourProgressBeat>(StringComparer.Ordinal);
        private readonly HashSet<string> _completed = new HashSet<string>(StringComparer.Ordinal);
        private readonly HashSet<string> _grantedFlagSet = new HashSet<string>(StringComparer.Ordinal);
        private readonly List<string> _grantedFlagOrder = new List<string>();

        private bool _hintDelivered;

        public bool IsEnabled => DisabledReasonCode.Length == 0;
        public string DisabledReasonCode { get; private set; } = DisabledNone;
        public string CurrentBeatId
        {
            get
            {
                FirstHourProgressBeat beat = FindCurrentBeat();
                return beat != null ? beat.Id : null;
            }
        }
        public string CurrentSignalId
        {
            get
            {
                FirstHourProgressBeat beat = FindCurrentBeat();
                return beat != null ? beat.CompletionSignalId : null;
            }
        }
        public string CurrentHintLineId
        {
            get
            {
                FirstHourProgressBeat beat = FindCurrentBeat();
                return beat != null ? beat.HintLineId : null;
            }
        }
        public double CurrentHintDelaySeconds
        {
            get
            {
                FirstHourProgressBeat beat = FindCurrentBeat();
                return beat != null ? beat.HintDelaySeconds : 0d;
            }
        }
        public bool IsComplete => IsEnabled && AllRequiredBeatsComplete();
        public bool IsBlocked => IsEnabled && !IsComplete && FindCurrentBeat() == null;
        public IReadOnlyList<string> GrantedFlags => _grantedFlagOrder.ToArray();
        public IReadOnlyList<string> CompletedBeatIds => OrderedCompletedBeatIds();

        public FirstHourProgressCore(
            IEnumerable<FirstHourProgressBeat> beats,
            IEnumerable<string> completedBeatIds = null)
        {
            if (!TryLoadBeats(beats)) return;
            LoadCompleted(completedBeatIds);
            RebuildGrantedFlags();
        }

        public FirstHourSignalResult AcceptSignal(string signalId)
        {
            if (!IsEnabled)
                return Result(FirstHourSignalResultCode.Disabled, signalId, null, Array.Empty<string>());
            if (string.IsNullOrWhiteSpace(signalId))
                return Result(FirstHourSignalResultCode.EmptySignal, signalId, null, Array.Empty<string>());
            if (IsComplete)
                return Result(FirstHourSignalResultCode.Complete, signalId, null, Array.Empty<string>());

            FirstHourProgressBeat current = FindCurrentBeat();
            if (current == null)
                return Result(FirstHourSignalResultCode.Blocked, signalId, null, Array.Empty<string>());

            FirstHourProgressBeat signaledBeat;
            if (!_beatsBySignal.TryGetValue(signalId, out signaledBeat))
                return Result(FirstHourSignalResultCode.Unknown, signalId, null, Array.Empty<string>());
            if (_completed.Contains(signaledBeat.Id))
                return Result(FirstHourSignalResultCode.Duplicate, signalId, null, Array.Empty<string>());
            if (!string.Equals(current.Id, signaledBeat.Id, StringComparison.Ordinal))
                return Result(FirstHourSignalResultCode.Early, signalId, null, Array.Empty<string>());

            _completed.Add(current.Id);
            string[] newlyGranted = AddFlags(current.GrantedFlags);
            _hintDelivered = false;
            return Result(FirstHourSignalResultCode.Accepted, signalId, current.Id, newlyGranted);
        }

        /// <summary>
        /// True when the current beat has a hint line, the caller reports that its configured delay
        /// has elapsed on this beat, and that hint has not yet been delivered. This method does not
        /// mutate the latch; call <see cref="MarkHintDelivered"/> only after presentation succeeds.
        /// </summary>
        public bool ShouldOfferHint(double elapsedOnCurrentBeatSeconds)
        {
            if (!IsEnabled || IsComplete || _hintDelivered) return false;
            FirstHourProgressBeat current = FindCurrentBeat();
            if (current == null || string.IsNullOrWhiteSpace(current.HintLineId)) return false;
            if (double.IsNaN(elapsedOnCurrentBeatSeconds) || double.IsInfinity(elapsedOnCurrentBeatSeconds)) return false;
            return elapsedOnCurrentBeatSeconds >= current.HintDelaySeconds;
        }

        /// <summary>Latch the current beat's hint once. Advancing to another beat resets the latch.</summary>
        public bool MarkHintDelivered()
        {
            if (!IsEnabled || IsComplete || _hintDelivered) return false;
            FirstHourProgressBeat current = FindCurrentBeat();
            if (current == null || string.IsNullOrWhiteSpace(current.HintLineId)) return false;
            _hintDelivered = true;
            return true;
        }

        public bool IsBeatComplete(string beatId)
        {
            return !string.IsNullOrWhiteSpace(beatId) && _completed.Contains(beatId);
        }

        private bool TryLoadBeats(IEnumerable<FirstHourProgressBeat> beats)
        {
            if (beats == null)
            {
                Disable(DisabledBeatsMissing);
                return false;
            }

            foreach (FirstHourProgressBeat beat in beats)
            {
                if (beat == null)
                {
                    Disable(DisabledBeatNull);
                    return false;
                }
                if (string.IsNullOrWhiteSpace(beat.Id))
                {
                    Disable(DisabledBeatId);
                    return false;
                }
                if (_beatsById.ContainsKey(beat.Id))
                {
                    Disable(DisabledBeatDuplicate);
                    return false;
                }
                if (string.IsNullOrWhiteSpace(beat.CompletionSignalId))
                {
                    Disable(DisabledSignal);
                    return false;
                }
                if (_beatsBySignal.ContainsKey(beat.CompletionSignalId))
                {
                    Disable(DisabledSignalDuplicate);
                    return false;
                }
                if (beat.HintDelaySeconds < 0d || double.IsNaN(beat.HintDelaySeconds) ||
                    double.IsInfinity(beat.HintDelaySeconds))
                {
                    Disable(DisabledHintDelay);
                    return false;
                }

                foreach (string prerequisite in beat.Prerequisites)
                {
                    if (string.IsNullOrWhiteSpace(prerequisite) || !_beatsById.ContainsKey(prerequisite))
                    {
                        Disable(DisabledPrerequisite);
                        return false;
                    }
                }
                foreach (string flag in beat.GrantedFlags)
                {
                    if (string.IsNullOrWhiteSpace(flag))
                    {
                        Disable(DisabledFlag);
                        return false;
                    }
                }

                _beats.Add(beat);
                _beatsById.Add(beat.Id, beat);
                _beatsBySignal.Add(beat.CompletionSignalId, beat);
            }

            if (_beats.Count == 0)
            {
                Disable(DisabledBeatsMissing);
                return false;
            }

            return true;
        }

        private void LoadCompleted(IEnumerable<string> completedBeatIds)
        {
            if (completedBeatIds == null) return;
            foreach (string beatId in completedBeatIds)
            {
                if (!string.IsNullOrWhiteSpace(beatId) && _beatsById.ContainsKey(beatId))
                    _completed.Add(beatId);
            }
        }

        private void RebuildGrantedFlags()
        {
            _grantedFlagSet.Clear();
            _grantedFlagOrder.Clear();
            foreach (FirstHourProgressBeat beat in _beats)
            {
                if (!_completed.Contains(beat.Id)) continue;
                AddFlags(beat.GrantedFlags);
            }
        }

        private string[] AddFlags(IReadOnlyList<string> flags)
        {
            if (flags == null || flags.Count == 0) return Array.Empty<string>();
            var newlyGranted = new List<string>();
            for (int i = 0; i < flags.Count; i++)
            {
                string flag = flags[i];
                if (!_grantedFlagSet.Add(flag)) continue;
                _grantedFlagOrder.Add(flag);
                newlyGranted.Add(flag);
            }
            return newlyGranted.ToArray();
        }

        private FirstHourProgressBeat FindCurrentBeat()
        {
            if (!IsEnabled) return null;
            foreach (FirstHourProgressBeat beat in _beats)
            {
                if (!beat.Required || _completed.Contains(beat.Id)) continue;
                if (PrerequisitesComplete(beat)) return beat;
            }
            return null;
        }

        private bool PrerequisitesComplete(FirstHourProgressBeat beat)
        {
            for (int i = 0; i < beat.Prerequisites.Count; i++)
                if (!_completed.Contains(beat.Prerequisites[i])) return false;
            return true;
        }

        private bool AllRequiredBeatsComplete()
        {
            foreach (FirstHourProgressBeat beat in _beats)
                if (beat.Required && !_completed.Contains(beat.Id)) return false;
            return true;
        }

        private string[] OrderedCompletedBeatIds()
        {
            var result = new List<string>();
            foreach (FirstHourProgressBeat beat in _beats)
                if (_completed.Contains(beat.Id)) result.Add(beat.Id);
            return result.ToArray();
        }

        private FirstHourSignalResult Result(
            FirstHourSignalResultCode code,
            string signalId,
            string completedBeatId,
            IEnumerable<string> grantedFlags)
        {
            return new FirstHourSignalResult(
                code,
                signalId,
                completedBeatId,
                CurrentBeatId,
                grantedFlags,
                IsComplete);
        }

        private void Disable(string reasonCode)
        {
            DisabledReasonCode = reasonCode ?? DisabledBeatsMissing;
            _beats.Clear();
            _beatsById.Clear();
            _beatsBySignal.Clear();
            _completed.Clear();
            _grantedFlagSet.Clear();
            _grantedFlagOrder.Clear();
            _hintDelivered = false;
        }
    }
}
