using UnityEngine;

namespace Ziptide.Content
{
    /// <summary>
    /// Data-driven pistol settings. Model can be swapped via ItemDefinition.modelPrefab.
    /// </summary>
    public class PistolDefinition : ItemDefinition
    {
        [Header("Pistol")]
        public float fireRate = 0.2f;
        public float range = 50f;
        public float hitForce = 5f;
        public float recoilKick = 0.02f;
        [Range(0f, 1f)] public float hapticAmplitude = 0.5f;
        public float hapticDuration = 0.05f;
        public GameObject muzzleFlashPrefab;
        public AudioClip fireClip;
    }
}
