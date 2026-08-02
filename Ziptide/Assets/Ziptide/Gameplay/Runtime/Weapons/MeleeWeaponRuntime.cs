using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;
using Ziptide.Core;
using Ziptide.Multiplayer;
using Ziptide.Multiplayer.Augments;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Semantic weapon-axis math shared by hand grips and holsters. A pose is defined by the actual
    /// handle-to-tip direction, never by guessed Euler angles. This keeps imported replacement meshes
    /// swappable as long as they preserve the Muzzle/tip socket.
    /// </summary>
    public static class WeaponPoseCore
    {
        public static Quaternion BuildLocalBasis(Vector3 localAxis, Vector3 localUpHint)
        {
            Vector3 axis = localAxis.sqrMagnitude > 1e-6f ? localAxis.normalized : Vector3.forward;
            Vector3 up = Vector3.ProjectOnPlane(localUpHint, axis);
            if (up.sqrMagnitude < 1e-6f)
            {
                Vector3 fallback = Mathf.Abs(Vector3.Dot(axis, Vector3.up)) < 0.9f
                    ? Vector3.up : Vector3.forward;
                up = Vector3.ProjectOnPlane(fallback, axis);
            }
            return Quaternion.LookRotation(axis, up.normalized);
        }

        public static Vector3 ResolveAxisLocal(Transform root, Transform tip, Vector3 localGripPosition)
        {
            if (root == null || tip == null) return Vector3.forward;
            Vector3 tipLocal = root.InverseTransformPoint(tip.position);
            Vector3 axis = tipLocal - localGripPosition;
            return axis.sqrMagnitude > 1e-6f ? axis.normalized : Vector3.forward;
        }

        public static Vector3 ResolveUpHintLocal(Vector3 localAxis)
        {
            Vector3 preferred = Mathf.Abs(Vector3.Dot(localAxis.normalized, Vector3.up)) < 0.9f
                ? Vector3.up : Vector3.forward;
            Vector3 projected = Vector3.ProjectOnPlane(preferred, localAxis);
            return projected.sqrMagnitude > 1e-6f ? projected.normalized : Vector3.right;
        }

        public static Quaternion MapLocalBasisToWorld(
            Vector3 localAxis, Vector3 localUp, Vector3 desiredWorldAxis, Vector3 desiredWorldUp)
        {
            Quaternion localBasis = BuildLocalBasis(localAxis, localUp);
            Quaternion worldBasis = BuildLocalBasis(desiredWorldAxis, desiredWorldUp);
            return worldBasis * Quaternion.Inverse(localBasis);
        }
    }

    /// <summary>
    /// Contact melee pair. Breaker Blade uses swing-speed contact; Tide Pike uses a committed forward
    /// thrust. Socket selection is not treated as being held in a hand, so a sheathed weapon cannot
    /// attack merely because the belt moved.
    /// </summary>
    [RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
    public class MeleeWeaponRuntime : MonoBehaviour
    {
        private const string SemanticGripName = "HandGripAttach";

        private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable _grab;
        private Transform _tip;
        private Vector3 _lastTipPos;
        private float _nextThrustAt;
        private float _nextWallHitAt;
        private readonly Dictionary<Transform, float> _nextHitAt = new Dictionary<Transform, float>();

        private ArenaWeaponDefinition Def
        {
            get
            {
                ItemRuntime item = GetComponent<ItemRuntime>();
                return item != null ? item.Definition as ArenaWeaponDefinition : null;
            }
        }

        private bool IsPike => Def != null && Def.kind == ArenaWeaponKind.TidePike;

        private void Awake()
        {
            _grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            _tip = transform.Find("Muzzle");
            if (_tip == null) _tip = transform;
            _lastTipPos = _tip.position;
        }

        private void Start()
        {
            ArenaWeaponDefinition def = Def;
            if (def == null || _grab == null) return;
            if (def.kind != ArenaWeaponKind.BreakerBlade && def.kind != ArenaWeaponKind.TidePike) return;
            InstallSemanticHandGrip(def.kind == ArenaWeaponKind.TidePike);
        }

        /// <summary>
        /// THE SWORD POSE. Two halves, and shipping only the first half is what made this wrong ten
        /// times running:
        ///
        ///   1. SEMANTIC BASIS (was already here, and is correct): solve the handle-to-tip axis from
        ///      the real Muzzle socket so a replacement mesh with different model axes still holds
        ///      right. Aligning that axis with the controller's +Z is correct because XRI drives the
        ///      controller from OpenXR's GRIP pose, whose forward axis is the direction a grasped
        ///      cylinder points.
        ///
        ///   2. RAKE (was missing, and was actively DELETED here): a hand around a hilt carries the
        ///      blade above the grip line, not along it. ItemFactory authored exactly that intent
        ///      -- gripEuler (70,0,0), "rides ABOVE the fist - a raised blade, not an aimed barrel"
        ///      -- and this method then overwrote it with the bare basis on the very next frame,
        ///      every spawn. So the shipped pose was always the barrel one, no matter how many
        ///      times the authored angle was corrected upstream.
        ///
        /// MeleeGripCore owns the angle and the reasoning; this is the translator.
        /// </summary>
        private void InstallSemanticHandGrip(bool thrustWeapon)
        {
            Transform previous = _grab.attachTransform;
            Transform grip = transform.Find(SemanticGripName);
            if (grip == null)
            {
                GameObject go = new GameObject(SemanticGripName);
                go.transform.SetParent(transform, false);
                grip = go.transform;
            }

            Vector3 localGrip = previous != null && previous != transform
                ? transform.InverseTransformPoint(previous.position) : Vector3.zero;

            Vector3 axisLocal = WeaponPoseCore.ResolveAxisLocal(transform, _tip, localGrip);
            Vector3 upLocal = WeaponPoseCore.ResolveUpHintLocal(axisLocal);

            // Keep some haft behind the fist. Without this the hand can end up on the butt of the
            // weapon (or, when no grip was authored, in the middle of the blade).
            if (_tip != null && _tip != transform)
            {
                float halfLength = transform.InverseTransformPoint(_tip.position).magnitude;
                float along = MeleeGripCore.GripOffsetAlongAxis(halfLength);
                if (localGrip.sqrMagnitude < 1e-6f) localGrip = axisLocal * along;
            }
            grip.localPosition = localGrip;

            float rake = MeleeGripCore.RakeFor(thrustWeapon);
            grip.localRotation = MeleeGripCore.GripLocalRotation(axisLocal, upLocal, rake);
            _grab.attachTransform = grip;

            Debug.Log("ZIPTIDE: MELEE_GRIP_SEMANTIC weapon=" + (thrustWeapon ? "tide_pike" : "breaker_blade")
                + " axisLocal=" + axisLocal.ToString("F3") + " upLocal=" + upLocal.ToString("F3")
                + " rake=" + rake.ToString("F0") + " grip=" + localGrip.ToString("F3")
                + " tip=" + (_tip != null ? _tip.name : "NONE"));
        }

        private void Update()
        {
            Vector3 tipPos = _tip.position;
            Vector3 tipVel = Time.deltaTime > 1e-4f ? (tipPos - _lastTipPos) / Time.deltaTime : Vector3.zero;
            _lastTipPos = tipPos;

            if (!IsHeldByController()) return;
            if (IsPike) TickPike(tipPos, tipVel);
            else TickBlade(tipPos, tipVel);
        }

        private bool IsHeldByController()
        {
            if (_grab == null || !_grab.isSelected) return false;
            foreach (UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor interactor in _grab.interactorsSelecting)
                if (interactor is UnityEngine.XR.Interaction.Toolkit.Interactors.XRBaseInputInteractor) return true;
            return false;
        }

        private void TickBlade(Vector3 tipPos, Vector3 tipVel)
        {
            if (tipVel.magnitude < (float)PvpRules.MeleeSwingSpeed) return;
            PvpNoise.Report(tipPos);

            Collider[] hits = Physics.OverlapSphere(tipPos, (float)PvpRules.BladeReach,
                ~0, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < hits.Length; i++)
            {
                Collider h = hits[i];
                if (h == null || h.transform.root == transform.root) continue;

                BreakableWall wall = h.GetComponentInParent<BreakableWall>();
                if (wall != null && Time.time >= _nextWallHitAt)
                {
                    wall.HitFromHammer(h.ClosestPoint(tipPos));
                    _nextWallHitAt = Time.time + (float)PvpRules.BladeContactDebounce;
                    continue;
                }

                IPvpDamageable pvp = h.GetComponentInParent<IPvpDamageable>();
                if (pvp == null || pvp.PlayerIndex == 0) continue;
                Transform key = h.transform.root;
                if (_nextHitAt.TryGetValue(key, out float nextAt) && Time.time < nextAt) continue;
                _nextHitAt[key] = Time.time + (float)PvpRules.BladeContactDebounce
                    * AugmentEffects.WeaponCooldownScale;

                Vector3 dir = tipVel.sqrMagnitude > 0.01f ? tipVel.normalized : transform.forward;
                PvpHitSource.Report(0);
                pvp.ReceiveHit(PvpWeapon.BreakerBlade, h.ClosestPoint(tipPos), dir);
                HitFlash(h.ClosestPoint(tipPos), new Color(0.55f, 0.9f, 1f));
                Debug.Log("ZIPTIDE: MELEE_HIT weapon=breaker_blade");
            }
        }

        private void TickPike(Vector3 tipPos, Vector3 tipVel)
        {
            if (Time.time < _nextThrustAt) return;
            float forwardSpeed = Vector3.Dot(tipVel, transform.forward);
            if (forwardSpeed < (float)PvpRules.PikeThrustSpeed) return;

            _nextThrustAt = Time.time + (float)PvpRules.PikeThrustDebounce
                * AugmentEffects.WeaponCooldownScale;
            PvpNoise.Report(tipPos);

            RaycastHit[] rays = Physics.RaycastAll(tipPos, transform.forward, (float)PvpRules.PikeReach,
                ~0, QueryTriggerInteraction.Ignore);
            float best = float.MaxValue;
            RaycastHit bestHit = default;
            IPvpDamageable bestPvp = null;
            for (int i = 0; i < rays.Length; i++)
            {
                if (rays[i].transform.root == transform.root) continue;
                IPvpDamageable pvp = rays[i].collider.GetComponentInParent<IPvpDamageable>();
                if (pvp == null || pvp.PlayerIndex == 0) continue;
                if (rays[i].distance < best)
                {
                    best = rays[i].distance;
                    bestHit = rays[i];
                    bestPvp = pvp;
                }
            }
            if (bestPvp == null) return;

            PvpHitSource.Report(0);
            bestPvp.ReceiveHit(PvpWeapon.TidePike, bestHit.point, transform.forward);
            PvpBot bot = bestHit.collider.GetComponentInParent<PvpBot>();
            if (bot != null) bot.transform.position += transform.forward * 0.75f;
            HitFlash(bestHit.point, new Color(0.35f, 0.75f, 0.8f));
            Debug.Log("ZIPTIDE: MELEE_HIT weapon=tide_pike dist=" + best.ToString("0.0"));
        }

        private static void HitFlash(Vector3 at, Color color)
        {
            GameObject s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            s.name = "MeleeHitFlash";
            Collider col = s.GetComponent<Collider>();
            if (col != null) Destroy(col);
            s.transform.position = at;
            s.transform.localScale = Vector3.one * 0.12f;
            ItemFactory.ApplyURPColor(s, color);
            s.AddComponent<MeleeFlashVisual>();
        }
    }

    public class MeleeFlashVisual : MonoBehaviour
    {
        private float _t;

        private void Update()
        {
            _t += Time.deltaTime / 0.18f;
            transform.localScale = Vector3.one * Mathf.Lerp(0.12f, 0.4f, Mathf.Clamp01(_t));
            if (_t >= 1f) Destroy(gameObject);
        }
    }
}
