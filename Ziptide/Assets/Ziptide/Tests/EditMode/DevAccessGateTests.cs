#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System.IO;
using NUnit.Framework;
using Ziptide.Gameplay.DevTools;

namespace Ziptide.Tests.EditMode
{
    public class DevAccessGateTests
    {
        private string _root;

        [SetUp]
        public void SetUp()
        {
            _root = Path.Combine(Path.GetTempPath(), "ziptide-dev-access-" + System.Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(_root);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_root)) Directory.Delete(_root, true);
        }

        [Test]
        public void LockedWithoutAccessMarker()
        {
            Assert.IsFalse(DevAccessGate.IsUnlockedAt(_root));
        }

        [Test]
        public void AccessMarkerUnlocks()
        {
            File.WriteAllText(DevAccessGate.AccessMarkerPath(_root), "developer\n");
            Assert.IsTrue(DevAccessGate.IsUnlockedAt(_root));
        }

        [Test]
        public void OpenRequestNeverBypassesLock()
        {
            string open = DevAccessGate.OpenMarkerPath(_root);
            File.WriteAllText(open, "open\n");

            Assert.IsFalse(DevAccessGate.TryConsumeOpenRequestAt(_root));
            Assert.IsTrue(File.Exists(open), "locked request remains pending until access is granted");
        }

        [Test]
        public void UnlockedOpenRequestConsumesExactlyOnce()
        {
            File.WriteAllText(DevAccessGate.AccessMarkerPath(_root), "developer\n");
            File.WriteAllText(DevAccessGate.OpenMarkerPath(_root), "open\n");

            Assert.IsTrue(DevAccessGate.TryConsumeOpenRequestAt(_root));
            Assert.IsFalse(File.Exists(DevAccessGate.OpenMarkerPath(_root)));
            Assert.IsTrue(File.Exists(DevAccessGate.AccessMarkerPath(_root)), "persistent unlock survives one-shot open");
            Assert.IsFalse(DevAccessGate.TryConsumeOpenRequestAt(_root), "request cannot replay");
        }

        [Test]
        public void EmptyRootFailsClosed()
        {
            Assert.IsFalse(DevAccessGate.IsUnlockedAt(null));
            Assert.IsFalse(DevAccessGate.IsUnlockedAt(""));
            Assert.IsFalse(DevAccessGate.TryConsumeOpenRequestAt(null));
        }

        [Test]
        public void MarkerNamesAreStableAndHidden()
        {
            StringAssert.EndsWith(DevAccessGate.AccessMarkerName, DevAccessGate.AccessMarkerPath(_root));
            StringAssert.EndsWith(DevAccessGate.OpenMarkerName, DevAccessGate.OpenMarkerPath(_root));
            StringAssert.StartsWith(".", DevAccessGate.AccessMarkerName);
            StringAssert.StartsWith(".", DevAccessGate.OpenMarkerName);
        }
    }
}
#endif
