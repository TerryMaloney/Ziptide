using System.Collections.Generic;

namespace Ziptide.Core
{
    /// <summary>
    /// FIELD CAMERA — the album is a ring: the newest capture is appended, and when the album
    /// exceeds its cap the OLDEST is dropped and RETURNED so the scene layer can delete its PNG file
    /// (the profile never grows unbounded and no orphan images pile up on disk). Pure — no
    /// UnityEngine, no I/O, no clock — so it is fully EditMode-tested; the translator owns the file
    /// side. Mirrors the BeltFloorSave overlay idiom.
    /// </summary>
    public static class PhotoAlbum
    {
        /// <summary>Max photos kept (Quest disk + the Quarters wall are both finite). The wall shows
        /// newest-first; storage is append-order (oldest at index 0).</summary>
        public const int DefaultCap = 24;

        /// <summary>Append a capture. If that pushes the album over <paramref name="cap"/>, drop the
        /// oldest and return it (caller deletes its file). Returns null when nothing was evicted.</summary>
        public static CapturedPhoto Add(List<CapturedPhoto> album, CapturedPhoto photo, int cap = DefaultCap)
        {
            if (album == null || photo == null) return null;
            album.Add(photo);
            if (cap < 1) cap = 1;
            if (album.Count <= cap) return null;
            var dropped = album[0];
            album.RemoveAt(0);
            return dropped;
        }

        /// <summary>Remove one photo by index, returning it (so the caller can delete its file).
        /// Out-of-range is a safe no-op returning null.</summary>
        public static CapturedPhoto RemoveAt(List<CapturedPhoto> album, int index)
        {
            if (album == null || index < 0 || index >= album.Count) return null;
            var removed = album[index];
            album.RemoveAt(index);
            return removed;
        }
    }
}
