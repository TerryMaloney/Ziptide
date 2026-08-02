using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Shared Quest rail for short-lived non-lethal world debris. Breakable walls and reactive props
    /// register through the same queue, so adding shootable dressing never creates a second physics
    /// budget. Destroyed entries are pruned lazily and the oldest live chunk is retired first.
    /// </summary>
    public static class WorldDebrisBudget
    {
        public const int MaxLiveDebris = 24;
        public const float DefaultLifetime = 4.5f;

        private static readonly Queue<GameObject> Live = new Queue<GameObject>();

        public static int LiveCount
        {
            get
            {
                PruneDestroyed();
                return Live.Count;
            }
        }

        /// <summary>Register an existing non-lethal debris object under the shared 24-live rail.</summary>
        public static void Register(GameObject debris)
        {
            if (debris == null) return;
            PruneDestroyed();
            Live.Enqueue(debris);
            while (Live.Count > MaxLiveDebris)
                DestroyObject(Live.Dequeue());
        }

        /// <summary>
        /// Spawn one cube chunk using an existing shared material. No material is allocated and the
        /// chunk carries only a Rigidbody plus the existing WallChunkDebris lifetime/shrink behavior.
        /// </summary>
        public static GameObject SpawnChunk(
            Vector3 position,
            Quaternion rotation,
            Vector3 scale,
            Vector3 velocity,
            Vector3 angularVelocity,
            Material sharedMaterial,
            float lifetime = DefaultLifetime)
        {
            var chunk = GameObject.CreatePrimitive(PrimitiveType.Cube);
            chunk.name = "WorldDebrisChunk";
            chunk.transform.SetPositionAndRotation(position, rotation);
            chunk.transform.localScale = scale;

            Renderer renderer = chunk.GetComponent<Renderer>();
            if (renderer != null)
            {
                if (sharedMaterial != null) renderer.sharedMaterial = sharedMaterial;
                renderer.shadowCastingMode = ShadowCastingMode.Off;
                renderer.receiveShadows = false;
                renderer.lightProbeUsage = LightProbeUsage.Off;
                renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
            }

            Rigidbody body = chunk.AddComponent<Rigidbody>();
            body.mass = 1.25f;
            body.linearVelocity = velocity;
            body.angularVelocity = angularVelocity;

            var debris = chunk.AddComponent<WallChunkDebris>();
            debris.lifetime = Mathf.Max(0.1f, lifetime);
            Register(chunk);
            return chunk;
        }

        /// <summary>EditMode verification/scene teardown seam. Runtime callers should not need it.</summary>
        public static void ClearAll()
        {
            while (Live.Count > 0)
                DestroyObject(Live.Dequeue());
        }

        private static void PruneDestroyed()
        {
            if (Live.Count == 0) return;
            int count = Live.Count;
            for (int i = 0; i < count; i++)
            {
                GameObject value = Live.Dequeue();
                if (value != null) Live.Enqueue(value);
            }
        }

        private static void DestroyObject(Object value)
        {
            if (value == null) return;
            if (Application.isPlaying) Object.Destroy(value);
            else Object.DestroyImmediate(value);
        }
    }
}
