using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// Data-driven audio profile for a world: background music clip, volume, crossfade settings.
    /// </summary>
    [CreateAssetMenu(fileName = "NewAudioProfile", menuName = "Ziptide/Audio Profile")]
    public class AudioProfile : ScriptableObject
    {
        [Tooltip("Music clip to play.")]
        public AudioClip clip;

        [Range(0f, 1f)]
        public float volume = 0.35f;

        public bool loop = true;

        [Tooltip("Crossfade duration when switching tracks.")]
        public float crossfadeSeconds = 2f;

        [Tooltip("If false, this profile produces silence.")]
        public bool enabled = true;
    }
}
