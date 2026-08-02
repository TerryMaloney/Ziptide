using UnityEngine;

namespace Ziptide.Core
{
    /// <summary>
    /// HOW A SWORD SITS IN A QUEST HAND. The numbers, in one place, with the reasoning attached —
    /// because this has been "fixed" repeatedly at the wrong layer and kept coming back wrong.
    ///
    /// WHAT THE PLATFORM GIVES US.
    /// Unity's XR Interaction Toolkit poses a grabbed object so its attachTransform lands on the
    /// interactor's attach transform. On this project's stack (OpenXR + XRI 2.5.4) the controller
    /// transform is driven by devicePosition/deviceRotation, which is OpenXR's GRIP pose — the pose
    /// whose documented purpose is "placing a model, such as a sword or a stick, in the player's
    /// hand". Its forward axis is defined as the ray through the tube formed by your non-thumb
    /// fingers when you close your hand: literally the direction a grasped cylinder points.
    ///
    /// So aligning the blade's handle-to-tip axis with the controller's +Z is the correct BASIS, and
    /// the existing WeaponPoseCore already solves that from the real Muzzle socket rather than from
    /// guessed Eulers. That part was never the bug.
    ///
    /// WHAT WAS MISSING: A SWORD IS NOT A TORCH.
    /// A hand closed around a hilt does not hold the blade along the finger tube. The wrist is
    /// slightly extended and the blade rides ABOVE that axis, which is why every VR melee game that
    /// feels right rakes the blade up off the grip line rather than shipping it barrel-straight:
    ///   · at rest, an arm hanging at your side leaves the blade pointing forward and up, not into
    ///     the floor — the single biggest "this feels wrong" tell;
    ///   · a natural downward swing tracks THROUGH the target instead of past it;
    ///   · the tip stays inside your view cone without extending the wrist to find it.
    ///
    /// This project already knew that. ItemFactory authors the breaker blade with a 70 degree grip
    /// tilt and the comment "rides ABOVE the fist - a raised blade, not an aimed barrel". Then
    /// MeleeWeaponRuntime.InstallSemanticHandGrip ran on the next frame and replaced that pose with a
    /// pure axis-aligned basis, every single spawn. The authored intent never survived to the
    /// headset once. Hence "this is like the tenth time".
    ///
    /// THE NUMBER. 70 degrees is too far — that is a blade held like a flag. The working range
    /// across VR melee is roughly 25-55 degrees off the grip axis; 40 is the middle of it and the
    /// angle at which the blade reads as continuing the forearm. The pike is a committed two-handed
    /// thrust weapon, so it stays much flatter: the tip has to LEAD.
    /// </summary>
    public static class MeleeGripCore
    {
        /// <summary>Blade rake above the controller's grip axis, in degrees.</summary>
        public const float BladeRakeDegrees = 40f;

        /// <summary>Thrust weapons stay near the grip line so the point leads the motion.</summary>
        public const float ThrustRakeDegrees = 12f;

        /// <summary>Below this the weapon reads as an aimed barrel.</summary>
        public const float MinUsefulRake = 25f;

        /// <summary>Above this the weapon reads as a flagpole and the tip leaves your view.</summary>
        public const float MaxUsefulRake = 55f;

        /// <summary>
        /// Metres of haft that must sit BEHIND the fist. Zero pommel means the hand appears welded to
        /// the butt of the weapon, which reads as a prop rather than a held object.
        /// </summary>
        public const float PommelClearance = 0.05f;

        /// <summary>The rake this weapon wants. One switch, so blades and pikes can never drift.</summary>
        public static float RakeFor(bool thrustWeapon) => thrustWeapon ? ThrustRakeDegrees : BladeRakeDegrees;

        /// <summary>A rake is only sane inside the band that keeps the tip in view and off the floor.</summary>
        public static bool IsUsefulBladeRake(float degrees) =>
            degrees >= MinUsefulRake && degrees <= MaxUsefulRake;

        /// <summary>
        /// The attach transform's local rotation for a weapon whose handle-to-tip axis is
        /// <paramref name="localAxis"/>. Post-multiplying the semantic basis by a pitch is what
        /// raises the blade: XRI aligns the attach transform to the hand, so a pitch baked into the
        /// attach pose comes out as the weapon pitching the OTHER way relative to the hand — which is
        /// exactly the raised blade we want, and the reason this cannot be done by rotating the mesh.
        /// </summary>
        public static Quaternion GripLocalRotation(Vector3 localAxis, Vector3 localUp, float rakeDegrees)
        {
            return BasisOf(localAxis, localUp) * Quaternion.Euler(rakeDegrees, 0f, 0f);
        }

        /// <summary>
        /// Where the blade ends up pointing, in hand space, for a given rake. Pure, so the tests can
        /// assert the thing that actually matters — that the tip rises above the grip line — instead
        /// of asserting an Euler triple nobody can picture.
        /// </summary>
        public static Vector3 BladeDirectionInHandSpace(float rakeDegrees)
        {
            return Quaternion.Euler(-rakeDegrees, 0f, 0f) * Vector3.forward;
        }

        /// <summary>
        /// Grip point along the weapon's own axis: far enough from the tip to be a handle, with
        /// PommelClearance left behind the fist. `halfLength` is the distance from the model origin
        /// to the tip.
        /// </summary>
        public static float GripOffsetAlongAxis(float halfLength)
        {
            float fromButt = Mathf.Max(0f, halfLength - PommelClearance);
            return -fromButt;
        }

        // Same construction as WeaponPoseCore.BuildLocalBasis; duplicated here so the pure grip math
        // has no dependency on the Gameplay assembly and can be unit-tested on its own.
        private static Quaternion BasisOf(Vector3 axis, Vector3 upHint)
        {
            Vector3 a = axis.sqrMagnitude > 1e-6f ? axis.normalized : Vector3.forward;
            Vector3 up = Vector3.ProjectOnPlane(upHint, a);
            if (up.sqrMagnitude < 1e-6f)
            {
                Vector3 fallback = Mathf.Abs(Vector3.Dot(a, Vector3.up)) < 0.9f ? Vector3.up : Vector3.forward;
                up = Vector3.ProjectOnPlane(fallback, a);
            }
            return Quaternion.LookRotation(a, up.normalized);
        }
    }
}
