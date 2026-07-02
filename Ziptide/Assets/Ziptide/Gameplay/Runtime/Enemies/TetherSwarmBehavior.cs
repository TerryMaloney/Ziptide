using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Tether-swarm (CREATURE_DESIGN novel #4): many bodies sharing ONE life via a glowing tether cord —
    /// shooting the bugs does nothing, because the bugs have no colliders at all. The only hittable
    /// thing is the bright TETHER NODE strung between the two anchor clusters: cut the cord (taser or
    /// gravity hit on the node — the whole CreatureRuntime health lives there) and the entire colony
    /// drops. Evolution: a colony organism with a shared nervous cord (the chitin worlds). The failed
    /// shots at the bodies are the lesson; the glowing cord is the tell.
    /// </summary>
    public class TetherSwarmBehavior : CreatureBehaviorBase
    {
        public int bodiesPerCluster = 4;
        public float clusterSeparation = 2.6f;
        public float weaveSpeed = 2.0f;

        private Transform _clusterA, _clusterB, _node;
        private Transform[] _bodies;
        private float[] _phase;
        private LineRenderer _cord;
        private static readonly Color BodyColor = new Color(0.50f, 0.38f, 0.20f);
        private static readonly Color CordColor = new Color(0.95f, 0.75f, 0.25f);

        protected override void BuildVisuals()
        {
            // Two colliderless clusters of bugs…
            _clusterA = new GameObject("ClusterA").transform;
            _clusterA.SetParent(transform, false);
            _clusterA.localPosition = new Vector3(-clusterSeparation * 0.5f, 0.4f, 0f);
            _clusterB = new GameObject("ClusterB").transform;
            _clusterB.SetParent(transform, false);
            _clusterB.localPosition = new Vector3(clusterSeparation * 0.5f, 0.4f, 0f);

            _bodies = new Transform[bodiesPerCluster * 2];
            _phase = new float[_bodies.Length];
            for (int i = 0; i < _bodies.Length; i++)
            {
                var parent = i < bodiesPerCluster ? _clusterA : _clusterB;
                var b = MakePart("Bug_" + i, PrimitiveType.Sphere, Vector3.zero,
                    new Vector3(0.12f, 0.08f, 0.16f), BodyColor);
                b.transform.SetParent(parent, false);
                _bodies[i] = b.transform;
                _phase[i] = Random.value * 10f;
            }

            // …and the ONE vulnerable thing: the glowing tether node mid-cord (the only collider).
            var node = MakePart("TetherNode", PrimitiveType.Sphere, new Vector3(0f, 0.4f, 0f),
                Vector3.one * 0.22f, CordColor, keepCollider: true);
            _node = node.transform;

            _cord = gameObject.AddComponent<LineRenderer>();
            _cord.positionCount = 3;
            _cord.startWidth = 0.04f;
            _cord.endWidth = 0.04f;
            _cord.useWorldSpace = true;
            var shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null) shader = Shader.Find("Sprites/Default");
            if (shader != null)
            {
                var mat = new Material(shader);
                mat.color = CordColor;
                if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", CordColor);
                _cord.material = mat;
            }
        }

        protected override void Tick(float dt, float dist)
        {
            // The colony weaves as a pair around the player at standoff, clusters counter-orbiting.
            if (Player != null && dist <= detectRange)
            {
                Vector3 target = Vector3.Lerp(transform.position,
                    new Vector3(Player.position.x, Runtime.HomePos.y, Player.position.z)
                    + (transform.position - Player.position).normalized * 3f, 0.5f);
                CollideMove(Leashed(Vector3.MoveTowards(transform.position, target, weaveSpeed * dt)));
                FaceToward(Player.position, dt, 3f);
            }

            float t = Time.time;
            if (_clusterA != null)
                _clusterA.localPosition = new Vector3(-clusterSeparation * 0.5f, 0.4f + Mathf.Sin(t * 1.8f) * 0.2f, Mathf.Sin(t * 1.1f) * 0.5f);
            if (_clusterB != null)
                _clusterB.localPosition = new Vector3(clusterSeparation * 0.5f, 0.4f + Mathf.Sin(t * 1.8f + 2f) * 0.2f, Mathf.Sin(t * 1.1f + 2f) * 0.5f);
            for (int i = 0; i < _bodies.Length; i++)
                if (_bodies[i] != null)
                    _bodies[i].localPosition = new Vector3(
                        Mathf.Sin(_phase[i] + t * 5f) * 0.35f,
                        Mathf.Sin(_phase[i] + t * 3.3f) * 0.2f,
                        Mathf.Cos(_phase[i] + t * 4.1f) * 0.35f);

            // The cord sags between clusters through the node — the visible "cut here."
            if (_cord != null && _clusterA != null && _clusterB != null && _node != null)
            {
                _node.localPosition = new Vector3(0f, 0.35f + Mathf.Sin(t * 2.5f) * 0.05f, 0f);
                _cord.SetPosition(0, _clusterA.position);
                _cord.SetPosition(1, _node.position);
                _cord.SetPosition(2, _clusterB.position);
            }
        }
    }
}
