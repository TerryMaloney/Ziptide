using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Content
{
    [Serializable]
    public sealed class FirstHourContractReference
    {
        public string id;
        public string status;

        public FirstHourContractReference Clone()
        {
            return new FirstHourContractReference { id = id, status = status };
        }
    }

    [Serializable]
    public sealed class FirstHourBeatDefinition
    {
        public int sequence;
        public string id;
        public string phase;
        public string world;
        public bool required;
        public List<string> prerequisites = new List<string>();
        public string openCondition;
        public FirstHourContractReference completionSignal;
        public List<string> setsFlags = new List<string>();
        public string persistence;
        public List<string> evidence = new List<string>();
        public bool allowsInputLock;
        public bool movesPlayerRig;
        public bool autoComplete;
        public string notes;
        public string teachesVerb;
        public float hesitationSeconds;
        public FirstHourContractReference rillLine;

        public FirstHourBeatDefinition Clone()
        {
            return new FirstHourBeatDefinition
            {
                sequence = sequence,
                id = id,
                phase = phase,
                world = world,
                required = required,
                prerequisites = Copy(prerequisites),
                openCondition = openCondition,
                completionSignal = completionSignal != null ? completionSignal.Clone() : null,
                setsFlags = Copy(setsFlags),
                persistence = persistence,
                evidence = Copy(evidence),
                allowsInputLock = allowsInputLock,
                movesPlayerRig = movesPlayerRig,
                autoComplete = autoComplete,
                notes = notes,
                teachesVerb = teachesVerb,
                hesitationSeconds = hesitationSeconds,
                rillLine = rillLine != null ? rillLine.Clone() : null
            };
        }

        private static List<string> Copy(List<string> source)
        {
            return source != null ? new List<string>(source) : new List<string>();
        }
    }

    /// <summary>
    /// Plain serializable mirror of docs/first_hour/first_hour_beats.json. Editor code parses JSON
    /// into this DTO; runtime code never parses JSON.
    /// </summary>
    [Serializable]
    public sealed class FirstHourContractData
    {
        public int schemaVersion;
        public string contractId;
        public string title;
        public string status;
        public List<string> sourceDocuments = new List<string>();
        public List<string> completionFlags = new List<string>();
        public List<string> coreVerbs = new List<string>();
        public List<string> allowedEvidence = new List<string>();
        public List<string> bindingStatuses = new List<string>();
        public List<FirstHourBeatDefinition> beats = new List<FirstHourBeatDefinition>();

        public FirstHourContractData Clone()
        {
            var clone = new FirstHourContractData
            {
                schemaVersion = schemaVersion,
                contractId = contractId,
                title = title,
                status = status,
                sourceDocuments = Copy(sourceDocuments),
                completionFlags = Copy(completionFlags),
                coreVerbs = Copy(coreVerbs),
                allowedEvidence = Copy(allowedEvidence),
                bindingStatuses = Copy(bindingStatuses),
                beats = new List<FirstHourBeatDefinition>()
            };

            if (beats != null)
            {
                foreach (FirstHourBeatDefinition beat in beats)
                    clone.beats.Add(beat != null ? beat.Clone() : null);
            }

            return clone;
        }

        private static List<string> Copy(List<string> source)
        {
            return source != null ? new List<string>(source) : new List<string>();
        }
    }

    /// <summary>
    /// Runtime first-hour contract. Generated deterministically by FirstHourContractAuthor from the
    /// approved repository JSON. Missing data disables tutorial orchestration only; base gameplay
    /// must never depend on a fabricated fallback list.
    /// </summary>
    [CreateAssetMenu(menuName = "Ziptide/Tutorial/First Hour Contract", fileName = "FirstHourContract")]
    public sealed class FirstHourContractDefinition : ScriptableObject
    {
        public const string ResourcesPath = "Tutorial/FirstHourContract";

        public string sourceHash;
        public int schemaVersion;
        public string contractId;
        public string title;
        public string status;
        public List<string> sourceDocuments = new List<string>();
        public List<string> completionFlags = new List<string>();
        public List<string> coreVerbs = new List<string>();
        public List<string> allowedEvidence = new List<string>();
        public List<string> bindingStatuses = new List<string>();
        public List<FirstHourBeatDefinition> beats = new List<FirstHourBeatDefinition>();

        public int TeachingBeatCount
        {
            get
            {
                int total = 0;
                if (beats == null) return total;
                foreach (FirstHourBeatDefinition beat in beats)
                    if (beat != null && !string.IsNullOrWhiteSpace(beat.teachesVerb)) total++;
                return total;
            }
        }

        public int TeachingLineCount
        {
            get
            {
                int total = 0;
                if (beats == null) return total;
                foreach (FirstHourBeatDefinition beat in beats)
                    if (beat != null && beat.rillLine != null && !string.IsNullOrWhiteSpace(beat.rillLine.id)) total++;
                return total;
            }
        }

        public static FirstHourContractDefinition Load()
        {
            return Resources.Load<FirstHourContractDefinition>(ResourcesPath);
        }

        public void ReplaceWith(FirstHourContractData source, string hash)
        {
            FirstHourContractData copy = source != null ? source.Clone() : new FirstHourContractData();
            sourceHash = hash ?? string.Empty;
            schemaVersion = copy.schemaVersion;
            contractId = copy.contractId;
            title = copy.title;
            status = copy.status;
            sourceDocuments = copy.sourceDocuments;
            completionFlags = copy.completionFlags;
            coreVerbs = copy.coreVerbs;
            allowedEvidence = copy.allowedEvidence;
            bindingStatuses = copy.bindingStatuses;
            beats = copy.beats;
        }

        public FirstHourContractData ToData()
        {
            return new FirstHourContractData
            {
                schemaVersion = schemaVersion,
                contractId = contractId,
                title = title,
                status = status,
                sourceDocuments = sourceDocuments != null ? new List<string>(sourceDocuments) : new List<string>(),
                completionFlags = completionFlags != null ? new List<string>(completionFlags) : new List<string>(),
                coreVerbs = coreVerbs != null ? new List<string>(coreVerbs) : new List<string>(),
                allowedEvidence = allowedEvidence != null ? new List<string>(allowedEvidence) : new List<string>(),
                bindingStatuses = bindingStatuses != null ? new List<string>(bindingStatuses) : new List<string>(),
                beats = CloneBeats(beats)
            };
        }

        private static List<FirstHourBeatDefinition> CloneBeats(List<FirstHourBeatDefinition> source)
        {
            var result = new List<FirstHourBeatDefinition>();
            if (source == null) return result;
            foreach (FirstHourBeatDefinition beat in source)
                result.Add(beat != null ? beat.Clone() : null);
            return result;
        }
    }
}
