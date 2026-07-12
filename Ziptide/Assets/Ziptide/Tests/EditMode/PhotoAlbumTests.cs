using NUnit.Framework;
using System.Collections.Generic;
using Ziptide.Core;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// FIELD CAMERA — pins the album ring + the save story: appends keep order, the cap evicts and
    /// RETURNS the oldest (so its PNG can be deleted), removal returns the record, and the album
    /// round-trips through the profile JSON with a pre-v3 save defaulting to an empty album (the
    /// neutral-default law — old saves untouched).
    /// </summary>
    public class PhotoAlbumTests
    {
        private static CapturedPhoto Photo(string id, long at)
            => new CapturedPhoto { worldId = id, atUnix = at, file = id + ".png", rating = 1 };

        [Test]
        public void Add_AppendsNewest_UnderCap_EvictsNothing()
        {
            var album = new List<CapturedPhoto>();
            Assert.IsNull(PhotoAlbum.Add(album, Photo("a", 1), cap: 3));
            Assert.IsNull(PhotoAlbum.Add(album, Photo("b", 2), cap: 3));
            Assert.AreEqual(2, album.Count);
            Assert.AreEqual("a", album[0].worldId, "oldest at index 0");
            Assert.AreEqual("b", album[1].worldId, "newest appended");
        }

        [Test]
        public void Add_OverCap_DropsAndReturnsOldest()
        {
            var album = new List<CapturedPhoto>();
            PhotoAlbum.Add(album, Photo("a", 1), cap: 2);
            PhotoAlbum.Add(album, Photo("b", 2), cap: 2);
            var dropped = PhotoAlbum.Add(album, Photo("c", 3), cap: 2);
            Assert.IsNotNull(dropped, "the oldest is evicted so the caller can delete its file");
            Assert.AreEqual("a.png", dropped.file);
            Assert.AreEqual(2, album.Count);
            Assert.AreEqual("b", album[0].worldId);
            Assert.AreEqual("c", album[1].worldId);
        }

        [Test]
        public void Add_IsNullSafe_AndCapNeverBelowOne()
        {
            Assert.IsNull(PhotoAlbum.Add(null, Photo("a", 1)));
            var album = new List<CapturedPhoto>();
            Assert.IsNull(PhotoAlbum.Add(album, null));
            Assert.AreEqual(0, album.Count);
            PhotoAlbum.Add(album, Photo("a", 1), cap: 0); // clamped to 1
            var dropped = PhotoAlbum.Add(album, Photo("b", 2), cap: 0);
            Assert.AreEqual("a.png", dropped.file);
            Assert.AreEqual(1, album.Count);
        }

        [Test]
        public void RemoveAt_ReturnsRecord_OutOfRangeIsSafe()
        {
            var album = new List<CapturedPhoto> { Photo("a", 1), Photo("b", 2) };
            var removed = PhotoAlbum.RemoveAt(album, 0);
            Assert.AreEqual("a.png", removed.file);
            Assert.AreEqual(1, album.Count);
            Assert.IsNull(PhotoAlbum.RemoveAt(album, 9));
            Assert.IsNull(PhotoAlbum.RemoveAt(null, 0));
        }

        [Test]
        public void Album_RoundTripsThroughProfileJson()
        {
            var p = ProfileSerializer.NewProfile();
            PhotoAlbum.Add(p.photos, new CapturedPhoto
            {
                worldId = "W005", atUnix = 20278, yaw = 90f, pitch = -10f, fov = 60f,
                rating = 2, subjectId = "signature_moon", file = "shot1.png"
            });
            var back = ProfileSerializer.Deserialize(ProfileSerializer.Serialize(p));
            Assert.AreEqual(1, back.photos.Count);
            var photo = back.photos[0];
            Assert.AreEqual("W005", photo.worldId);
            Assert.AreEqual(2, photo.rating);
            Assert.AreEqual("signature_moon", photo.subjectId);
            Assert.AreEqual("shot1.png", photo.file);
            Assert.AreEqual(90f, photo.yaw, 1e-4f);
        }

        [Test]
        public void OldSave_WithoutPhotos_MigratesToEmptyAlbum_AtSchema3()
        {
            var p = ProfileSerializer.Deserialize("{\"schemaVersion\":2,\"playerId\":\"p\"}");
            Assert.IsNotNull(p.photos, "pre-v3 saves get an empty album, never null");
            Assert.AreEqual(0, p.photos.Count);
            Assert.AreEqual(PlayerProfile.CurrentSchemaVersion, p.schemaVersion, "migrated forward to v3");
            Assert.AreEqual(3, PlayerProfile.CurrentSchemaVersion);
        }
    }
}
