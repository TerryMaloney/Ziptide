using UnityEngine;

namespace Ziptide.Tests.PlayMode
{
    /// <summary>Tests-only lifecycle witness for the R1.1 PlayMode infrastructure spike.</summary>
    public sealed class PlayModeFrameProbe : MonoBehaviour
    {
        public bool AwakeObserved { get; private set; }
        public bool StartObserved { get; private set; }
        public int UpdateCount { get; private set; }

        private void Awake() => AwakeObserved = true;
        private void Start() => StartObserved = true;
        private void Update() => UpdateCount++;
    }
}
