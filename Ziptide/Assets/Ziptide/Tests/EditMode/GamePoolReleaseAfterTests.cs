using NUnit.Framework;
using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// HARDWIRING 0.5 — GamePool.ReleaseAfter, the pooled Destroy(go, t). The timed branch needs
    /// PlayMode (WaitForSeconds); EditMode pins the synchronous contracts the weapon call-sites
    /// rely on: null is a safe no-op, and seconds<=0 releases immediately (deactivate + park).
    /// </summary>
    public class GamePoolReleaseAfterTests
    {
        [Test]
        public void ReleaseAfter_NullObject_IsSafeNoOp()
        {
            Assert.DoesNotThrow(() => GamePool.ReleaseAfter("hw_test_null", null, 0.5f));
        }

        [Test]
        public void ReleaseAfter_ZeroSeconds_ReleasesImmediately()
        {
            var go = GamePool.Get("hw_test_zero", () => new GameObject("hw_test_zero"), Vector3.one);
            Assert.IsNotNull(go);
            Assert.IsTrue(go.activeSelf, "Get hands out an active instance");
            Assert.AreEqual(Vector3.one, go.transform.position);

            GamePool.ReleaseAfter("hw_test_zero", go, 0f);
            Assert.IsFalse(go.activeSelf, "seconds<=0 must release synchronously (deactivated + parked)");

            // Round-trip: the parked instance is reused, not rebuilt.
            var again = GamePool.Get("hw_test_zero", () => new GameObject("hw_test_zero_SHOULD_NOT_BUILD"), Vector3.zero);
            Assert.AreSame(go, again, "pooled instance must be reused on the next Get");
        }
    }
}
