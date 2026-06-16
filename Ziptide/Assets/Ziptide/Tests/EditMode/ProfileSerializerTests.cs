using NUnit.Framework;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// Headless verification of the save/load layer — runs in CI, no headset needed.
    /// This is the "blind-build safety net" in action: persistence is proven before it ships.
    /// </summary>
    public class ProfileSerializerTests
    {
        [Test]
        public void RoundTrip_PreservesData()
        {
            var p = ProfileSerializer.NewProfile();
            p.displayName = "Cal";
            p.SetFlag("met_rill");
            p.AddResource("scrap", 42.5);
            p.AddResource("scrap", 7.5);            // accumulate -> 50
            var w = p.GetWorld("d0_city", createIfMissing: true);
            w.discovered = true;
            w.lastResolvedAtUnix = 1234567890L;

            string json = ProfileSerializer.Serialize(p);
            var r = ProfileSerializer.Deserialize(json);

            Assert.AreEqual(p.playerId, r.playerId);
            Assert.AreEqual("Cal", r.displayName);
            Assert.IsTrue(r.HasFlag("met_rill"));
            Assert.AreEqual(50.0, r.GetResource("scrap"), 1e-9);

            var rw = r.GetWorld("d0_city");
            Assert.IsNotNull(rw);
            Assert.IsTrue(rw.discovered);
            Assert.AreEqual(1234567890L, rw.lastResolvedAtUnix);
        }

        [Test]
        public void Deserialize_NullOrBlank_ReturnsFreshProfile()
        {
            var a = ProfileSerializer.Deserialize(null);
            var b = ProfileSerializer.Deserialize("   ");

            Assert.IsNotNull(a);
            Assert.IsNotNull(b);
            Assert.AreEqual(PlayerProfile.CurrentSchemaVersion, a.schemaVersion);
            Assert.IsFalse(string.IsNullOrEmpty(a.playerId));
        }

        [Test]
        public void Deserialize_Garbage_DoesNotThrow_ReturnsFreshProfile()
        {
            var p = ProfileSerializer.Deserialize("this is not json {{{");
            Assert.IsNotNull(p);
            Assert.AreEqual(PlayerProfile.CurrentSchemaVersion, p.schemaVersion);
        }

        [Test]
        public void NewProfile_HasUniqueIdsAndTimestamps()
        {
            var a = ProfileSerializer.NewProfile();
            var b = ProfileSerializer.NewProfile();
            Assert.AreNotEqual(a.playerId, b.playerId);
            Assert.Greater(a.createdAtUnix, 0L);
        }
    }
}
