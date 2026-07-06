using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// THE PLAYER'S SKIN (Terry: "our main character doesn't have a skin yet"). VR presence is
    /// hands-first: each controller wears a built-up salvage glove (leather palm, plated knuckles,
    /// articulated finger block, cuff, glowing teal wrist band — the taser's family), and a chest
    /// rig floats below the camera yaw-only (the BeltRig pattern) so looking down shows a BODY,
    /// not a void. All primitive-built (zero assets), palette matches ToxicIndustrial, and the
    /// glow strips carry the RILL teal so the player reads as part of this world. Rig-ensured;
    /// the QUARTERS cosmetics seam recolors it later (one material set per part group).
    /// </summary>
    public class PlayerAvatarRig : MonoBehaviour
    {
        private static readonly Color Leather = new Color(0.17f, 0.15f, 0.14f);
        private static readonly Color Plate = new Color(0.36f, 0.34f, 0.31f);
        private static readonly Color Accent = new Color(0.85f, 0.70f, 0.30f); // cabin gold trim
        private static readonly Color Glow = new Color(0.30f, 0.80f, 0.95f);   // RILL teal

        private Transform _cam;
        private Transform _torso;

        private void Start()
        {
            _cam = GetComponentInChildren<Camera>(true)?.transform;
            foreach (var c in GetComponentsInChildren<ActionBasedController>(true))
            {
                string n = c.name.ToLowerInvariant();
                if (n.Contains("left")) BuildGlove(c.transform, true);
                else if (n.Contains("right")) BuildGlove(c.transform, false);
            }
            BuildTorso();
            Debug.Log("ZIPTIDE: AVATAR_READY gloves+torso");
        }

        private void LateUpdate()
        {
            // Chest follows the head yaw-only at chest height — never pitches/rolls with the head.
            if (_torso == null || _cam == null) return;
            _torso.position = _cam.position + Vector3.up * -0.42f
                + Quaternion.Euler(0f, _cam.eulerAngles.y, 0f) * new Vector3(0f, 0f, -0.06f);
            _torso.rotation = Quaternion.Euler(0f, _cam.eulerAngles.y, 0f);
        }

        private void BuildGlove(Transform hand, bool left)
        {
            if (hand.Find("__Glove") != null) return;
            var root = new GameObject("__Glove");
            root.transform.SetParent(hand, false);
            float mirror = left ? -1f : 1f;

            Part(root.transform, "Palm", new Vector3(0f, -0.028f, -0.045f), Vector3.zero,
                new Vector3(0.074f, 0.03f, 0.095f), Leather);
            Part(root.transform, "KnucklePlate", new Vector3(0f, -0.008f, -0.015f), new Vector3(-8f, 0f, 0f),
                new Vector3(0.078f, 0.016f, 0.05f), Plate);
            Part(root.transform, "Fingers", new Vector3(0f, -0.03f, 0.012f), new Vector3(-22f, 0f, 0f),
                new Vector3(0.07f, 0.024f, 0.055f), Leather);
            Part(root.transform, "Thumb", new Vector3(mirror * 0.042f, -0.035f, -0.03f), new Vector3(0f, mirror * 35f, mirror * -20f),
                new Vector3(0.02f, 0.02f, 0.05f), Leather);
            Part(root.transform, "Cuff", new Vector3(0f, -0.02f, -0.105f), Vector3.zero,
                new Vector3(0.085f, 0.05f, 0.035f), Plate);
            Part(root.transform, "CuffTrim", new Vector3(0f, -0.02f, -0.086f), Vector3.zero,
                new Vector3(0.088f, 0.052f, 0.006f), Accent);
            var band = Part(root.transform, "WristGlow", new Vector3(0f, 0.006f, -0.105f), Vector3.zero,
                new Vector3(0.05f, 0.008f, 0.02f), Glow);
            MakeEmissive(band, Glow);
        }

        private void BuildTorso()
        {
            if (_cam == null) return;
            var torso = new GameObject("__AvatarTorso");
            torso.transform.SetParent(transform, false); // rig space, positioned in LateUpdate
            _torso = torso.transform;

            Part(_torso, "ChestPlate", new Vector3(0f, 0f, 0.02f), new Vector3(8f, 0f, 0f),
                new Vector3(0.30f, 0.20f, 0.11f), Leather);
            Part(_torso, "ChestArmor", new Vector3(0f, 0.05f, 0.075f), new Vector3(12f, 0f, 0f),
                new Vector3(0.24f, 0.10f, 0.02f), Plate);
            Part(_torso, "ShoulderL", new Vector3(-0.185f, 0.09f, 0f), new Vector3(0f, 0f, 12f),
                new Vector3(0.09f, 0.06f, 0.12f), Plate);
            Part(_torso, "ShoulderR", new Vector3(0.185f, 0.09f, 0f), new Vector3(0f, 0f, -12f),
                new Vector3(0.09f, 0.06f, 0.12f), Plate);
            var strip = Part(_torso, "CollarGlow", new Vector3(0f, 0.105f, 0.055f), new Vector3(15f, 0f, 0f),
                new Vector3(0.16f, 0.012f, 0.01f), Glow);
            MakeEmissive(strip, Glow);
        }

        private static GameObject Part(Transform parent, string name, Vector3 pos, Vector3 euler, Vector3 size, Color color)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
            go.name = name;
            var col = go.GetComponent<Collider>();
            if (col != null) Destroy(col); // pure look — never collides, never grabs
            go.transform.SetParent(parent, false);
            go.transform.localPosition = pos;
            go.transform.localRotation = Quaternion.Euler(euler);
            go.transform.localScale = size;
            ItemFactory.ApplyURPColor(go, color);
            var r = go.GetComponent<Renderer>();
            if (r != null) r.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            return go;
        }

        private static void MakeEmissive(GameObject go, Color color)
        {
            var r = go.GetComponent<Renderer>();
            if (r == null || r.sharedMaterial == null) return;
            r.sharedMaterial.EnableKeyword("_EMISSION");
            r.sharedMaterial.SetColor("_EmissionColor", color * 1.6f);
        }
    }
}
