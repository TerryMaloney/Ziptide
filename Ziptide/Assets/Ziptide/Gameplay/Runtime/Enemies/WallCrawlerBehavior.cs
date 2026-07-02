using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// WallCrawler archetype (GAME_PLAN M3): lives ON the walls — raycasts to the nearest vertical
    /// surface, sticks to it (positioned at the hit, aligned to the normal), and inches along it
    /// tracking the player. When the player comes close it TELEGRAPHS (a ripple pulse) then drops and
    /// lunges once, then scuttles back to its wall. Counter: taser while it's off the wall (grounded =
    /// slow); the drop is readable. Evolution: chitin-world climbers that never touch the swarm floor.
    /// Collision-clean: the lunge runs through CollideMove.
    /// </summary>
    public class WallCrawlerBehavior : CreatureBehaviorBase
    {
        public float wallSearchRadius = 7f;
        public float crawlSpeed = 1.6f;
        public float dropRange = 3.2f;
        public float lungeSpeed = 5.5f;
        public float telegraphSeconds = 0.8f;
        public float lungeCooldown = 4f;

        private enum Mode { SeekWall, OnWall, Telegraph, Lunge, Return }
        private Mode _mode = Mode.SeekWall;

        private Vector3 _wallPoint;
        private Vector3 _wallNormal = Vector3.forward;
        private float _timer;
        private float _nextLungeAllowed;
        private Transform _body;
        private static readonly Color BodyColor = new Color(0.42f, 0.36f, 0.24f);

        protected override void BuildVisuals()
        {
            var body = MakePart("Carapace", PrimitiveType.Cube, Vector3.zero,
                new Vector3(0.45f, 0.16f, 0.6f), BodyColor, keepCollider: true);
            _body = body.transform;
            for (int i = 0; i < 3; i++)
            {
                MakePart("LegL_" + i, PrimitiveType.Cube, new Vector3(-0.3f, -0.05f, -0.2f + i * 0.2f),
                    new Vector3(0.25f, 0.04f, 0.05f), BodyColor * 0.8f);
                MakePart("LegR_" + i, PrimitiveType.Cube, new Vector3(0.3f, -0.05f, -0.2f + i * 0.2f),
                    new Vector3(0.25f, 0.04f, 0.05f), BodyColor * 0.8f);
            }
        }

        protected override void Tick(float dt, float dist)
        {
            switch (_mode)
            {
                case Mode.SeekWall:
                    if (FindWall()) { _mode = Mode.OnWall; }
                    else
                    {
                        // No wall near — creep toward home (walls were authored near the zone).
                        CollideMove(Leashed(Vector3.MoveTowards(transform.position, Runtime.HomePos, crawlSpeed * dt)));
                    }
                    break;

                case Mode.OnWall:
                {
                    StickToWall();
                    // Crawl along the wall toward the point nearest the player.
                    if (Player != null && dist <= detectRange)
                    {
                        Vector3 alongWall = Vector3.ProjectOnPlane(Player.position - _wallPoint, _wallNormal);
                        alongWall.y = Mathf.Clamp(alongWall.y, -0.5f, 0.5f); // stay at ambush height
                        Vector3 target = _wallPoint + alongWall.normalized * crawlSpeed * dt;
                        if (RestickAt(target) && dist <= dropRange && Time.time >= _nextLungeAllowed)
                        {
                            _mode = Mode.Telegraph;
                            _timer = telegraphSeconds;
                        }
                    }
                    break;
                }

                case Mode.Telegraph:
                    StickToWall();
                    // The ripple: a readable pulse before the drop (fair in VR).
                    if (_body != null)
                        _body.localScale = new Vector3(0.45f, 0.16f, 0.6f) * (1f + Mathf.Sin(Time.time * 25f) * 0.15f);
                    _timer -= dt;
                    if (_timer <= 0f)
                    {
                        if (_body != null) _body.localScale = new Vector3(0.45f, 0.16f, 0.6f);
                        _mode = Mode.Lunge;
                        _timer = 1.1f; // max lunge time
                        _nextLungeAllowed = Time.time + lungeCooldown;
                    }
                    break;

                case Mode.Lunge:
                {
                    _timer -= dt;
                    Vector3 target = Player != null
                        ? new Vector3(Player.position.x, Runtime.HomePos.y + 0.3f, Player.position.z)
                        : Runtime.HomePos;
                    Vector3 before = transform.position;
                    CollideMove(Vector3.MoveTowards(before, target, lungeSpeed * dt));
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, dt * 8f);
                    bool arrived = (transform.position - target).magnitude < 0.4f;
                    bool blocked = (transform.position - before).magnitude < lungeSpeed * dt * 0.2f;
                    if (_timer <= 0f || arrived || blocked) _mode = Mode.Return;
                    break;
                }

                case Mode.Return:
                    // Scuttle back toward its wall point, then re-stick.
                    CollideMove(Vector3.MoveTowards(transform.position, _wallPoint, crawlSpeed * 1.6f * dt));
                    if ((transform.position - _wallPoint).magnitude < 0.5f || !FindWall())
                        _mode = FindWall() ? Mode.OnWall : Mode.SeekWall;
                    break;
            }
        }

        public override void OnStunned(float seconds)
        {
            // A stun knocks it OFF the wall — grounded and slow is the counter window.
            if (_mode == Mode.OnWall || _mode == Mode.Telegraph) _mode = Mode.Return;
        }

        private bool FindWall()
        {
            // Probe 8 horizontal directions for the nearest vertical surface.
            float best = wallSearchRadius;
            bool found = false;
            for (int i = 0; i < 8; i++)
            {
                float a = i * Mathf.PI * 0.25f;
                Vector3 dir = new Vector3(Mathf.Cos(a), 0f, Mathf.Sin(a));
                if (Physics.Raycast(transform.position + Vector3.up * 0.3f, dir, out var hit, best, ~0, QueryTriggerInteraction.Ignore))
                {
                    if (hit.collider.GetComponentInParent<CreatureRuntime>() != null) continue;
                    if (IsPlayerRig(hit.collider.transform)) continue;
                    if (Mathf.Abs(hit.normal.y) > 0.4f) continue; // walls only, not floors/ramps
                    best = hit.distance;
                    _wallPoint = hit.point + hit.normal * 0.15f;
                    _wallNormal = hit.normal;
                    found = true;
                }
            }
            return found;
        }

        private void StickToWall()
        {
            transform.position = Vector3.Lerp(transform.position, _wallPoint, Time.deltaTime * 6f);
            transform.rotation = Quaternion.Slerp(transform.rotation,
                Quaternion.LookRotation(Vector3.Cross(_wallNormal, Vector3.up), _wallNormal), Time.deltaTime * 6f);
        }

        // Re-raycast at a crawl target so the crawler follows the wall's actual surface.
        private bool RestickAt(Vector3 target)
        {
            Vector3 probeFrom = target + _wallNormal * 0.5f;
            if (Physics.Raycast(probeFrom, -_wallNormal, out var hit, 1.2f, ~0, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.GetComponentInParent<CreatureRuntime>() == null &&
                    !IsPlayerRig(hit.collider.transform) && Mathf.Abs(hit.normal.y) < 0.4f)
                {
                    _wallPoint = hit.point + hit.normal * 0.15f;
                    _wallNormal = hit.normal;
                    return true;
                }
            }
            return true; // lost the surface this frame — keep the old point, try again next frame
        }
    }
}
