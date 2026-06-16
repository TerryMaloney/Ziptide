using System.Collections.Generic;
using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Content
{
    /// <summary>
    /// Authoring-time registry of pods: a concrete <see cref="IPodLoader"/> backed by a hand-authored
    /// list of <see cref="PodNarrative"/> assets. This is the "one source of truth for what pods exist"
    /// (see 06_SCHEMAS). Swap it for a JSON / Addressables loader later without changing callers.
    /// </summary>
    [CreateAssetMenu(fileName = "PodRegistry", menuName = "Ziptide/Pod Registry", order = 21)]
    public class PodRegistry : ScriptableObject, IPodLoader
    {
        [Tooltip("All pods available in this build. Ids must be unique and non-empty.")]
        [SerializeField] private List<PodNarrative> pods = new List<PodNarrative>();

        public IReadOnlyList<string> AvailablePodIds
        {
            get
            {
                var ids = new List<string>(pods.Count);
                foreach (var pod in pods)
                    if (pod != null && !string.IsNullOrEmpty(pod.podId))
                        ids.Add(pod.podId);
                return ids;
            }
        }

        public PodNarrative Load(string podId)
        {
            if (string.IsNullOrEmpty(podId)) return null;
            foreach (var pod in pods)
                if (pod != null && pod.podId == podId)
                    return pod;
            return null;
        }

        public bool Has(string podId) => Load(podId) != null;
    }
}
