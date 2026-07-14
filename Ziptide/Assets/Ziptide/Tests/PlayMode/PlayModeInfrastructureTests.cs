using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace Ziptide.Tests.PlayMode
{
    /// <summary>
    /// R1.1 infrastructure spike only. This test proves the PlayMode runner can enter play,
    /// execute Awake/Start on a tests-only component, yield a frame, destroy its object and exit.
    /// It deliberately loads no project scene and starts no XR loader.
    /// </summary>
    public sealed class PlayModeInfrastructureTests
    {
        [UnityTest]
        public IEnumerator OneFrameRunner_ExecutesLifecycleAndCleansUp()
        {
            Assert.IsTrue(Application.isPlaying, "PlayMode test must execute while the player loop is running.");

            var host = new GameObject("__RECOVERY_PLAYMODE_FRAME_PROBE");
            var probe = host.AddComponent<PlayModeFrameProbe>();

            Assert.IsTrue(probe.AwakeObserved, "AddComponent must execute Awake immediately.");
            Assert.IsFalse(probe.StartObserved, "Start must not be assumed before the player loop advances.");

            yield return null;

            Assert.IsTrue(probe.StartObserved, "The PlayMode runner did not advance the lifecycle by one frame.");
            Assert.GreaterOrEqual(probe.UpdateCount, 1, "The tests-only probe did not receive an Update tick.");

            Object.Destroy(host);
            yield return null;

            Assert.IsTrue(host == null, "The tests-only object leaked past teardown.");
        }
    }
}
