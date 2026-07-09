using UnityEngine;
using Ziptide.Content;
using Ziptide.Content.Ship;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// SHIP PILLAR 2.1/2.2 — THE REFIT: turns the berth's shared hull skeleton into YOUR ship, at
    /// runtime, from the profile. Idempotent + re-runnable (the hangar calls it live on every equip):
    ///  · CHASSIS — reproportions the named ShipHullBuilder parts per ShipChassisPreset (fuselage
    ///    taper/girth, wing span+sweep, fin height, nacelle emissive) — six real silhouettes on the
    ///    same skeleton, deck/door offsets untouched (they key off the unchanged root). Picasso's
    ///    baked Forge meshes supersede this through the same parent later (ShipHullBuilder's own
    ///    invitation).
    ///  · LIVERY — closes the wired-nowhere seam: QuartersRoom equips CosmeticKind.ShipLivery but
    ///    nothing ever applied it; now the equipped livery's body/accent tint the hull renderers
    ///    (a look, never a stat — the loadout math has no wrap input, by construction).
    ///  · JOURNEY DECALS — the pure milestone list becomes small emblem plates along the port flank:
    ///    the hull is a wearable save file.
    ///  · NAMEPLATE — the ship's name (ShipLocker "name") on the bow.
    ///  · THE HUM — a per-chassis procedural engine-idle loop (the ZiptideGateEffect synthesis
    ///    precedent): every silhouette has its own voice, zero audio assets.
    /// </summary>
    public static class ShipRefit
    {
        private static readonly string[] FuselageParts = { "Fuselage_Aft", "Fuselage_Mid", "Fuselage_Bow", "Nose_Tip", "DorsalSpine" };

        /// <summary>Apply the profile's ship to this hull root. Safe to call repeatedly.</summary>
        public static void Apply(GameObject shipRoot)
        {
            if (shipRoot == null) return;
            var profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;

            string chassisId = ShipLocker.GetEquipped(profile, "chassis");
            var chassis = ShipChassisPreset.Find(chassisId);

            ApplyProportions(shipRoot.transform, chassis);
            ApplyLivery(shipRoot, profile);
            ApplyDecals(shipRoot.transform, profile);
            ApplyNameplate(shipRoot.transform, profile, chassis);
            ApplyHum(shipRoot, chassis);
            Debug.Log("ZIPTIDE: SHIP_REFIT chassis=" + chassis.Id);
        }

        // ── Chassis proportions (six silhouettes on one skeleton) ────────────
        private static void ApplyProportions(Transform root, ShipChassisPreset c)
        {
            foreach (var name in FuselageParts)
            {
                var t = root.Find(name);
                if (t == null) continue;
                var s = BaseScaleOf(t);
                t.localScale = new Vector3(s.x * c.FuselageGirth, s.y * c.FuselageGirth, s.z * c.FuselageLength);
                var p = BasePosOf(t);
                t.localPosition = new Vector3(p.x, p.y, p.z * c.FuselageLength);
            }
            foreach (Transform t in root)
            {
                if (t.name.StartsWith("Wing_") || t.name.StartsWith("WingTip_"))
                {
                    var s = BaseScaleOf(t);
                    t.localScale = new Vector3(s.x * c.WingSpan, s.y, s.z);
                    float side = t.localPosition.x >= 0f ? 1f : -1f;
                    var e = BaseEulerOf(t);
                    t.localEulerAngles = new Vector3(e.x, e.y - side * c.WingSweepDeg, e.z);
                }
                if (t.name == "TailFin")
                {
                    var s = BaseScaleOf(t);
                    t.localScale = new Vector3(s.x, s.y * c.FinHeight, s.z);
                }
                if (t.name.StartsWith("Exhaust_"))
                {
                    var r = t.GetComponent<Renderer>();
                    if (r != null)
                    {
                        var mat = r.material;
                        Color glow = EngineGlowOf(c);
                        if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", glow);
                        else mat.color = glow;
                    }
                }
            }
        }

        private static Color EngineGlowOf(ShipChassisPreset c)
        {
            switch (c.Silhouette)
            {
                case ShipSilhouette.Racer: return new Color(1f, 0.45f, 0.2f);
                case ShipSilhouette.Interceptor: return new Color(0.4f, 0.8f, 1f);
                case ShipSilhouette.Gunship: return new Color(1f, 0.3f, 0.35f);
                case ShipSilhouette.Explorer: return new Color(0.5f, 1f, 0.7f);
                case ShipSilhouette.Salvager: return new Color(1f, 0.8f, 0.35f);
                default: return new Color(0.7f, 0.7f, 0.9f); // hauler
            }
        }

        // The refit is re-runnable: remember each part's FIRST-SEEN transform so proportions always
        // multiply the ORIGINAL, never compound (chassis-swap twice ≠ ship grows twice).
        private static Vector3 BaseScaleOf(Transform t) => Remember(t).scale;
        private static Vector3 BasePosOf(Transform t) => Remember(t).pos;
        private static Vector3 BaseEulerOf(Transform t) => Remember(t).euler;

        private static ShipRefitBaseXf Remember(Transform t)
        {
            var b = t.GetComponent<ShipRefitBaseXf>();
            if (b == null)
            {
                b = t.gameObject.AddComponent<ShipRefitBaseXf>();
                b.scale = t.localScale;
                b.pos = t.localPosition;
                b.euler = t.localEulerAngles;
            }
            return b;
        }

        // ── Livery (closes the QuartersRoom seam) ────────────────────────────
        private static void ApplyLivery(GameObject shipRoot, PlayerProfile profile)
        {
            string liveryId = CosmeticLocker.GetEquipped(profile, "shiplivery");
            if (string.IsNullOrEmpty(liveryId)) return;
            var cosmetic = Resources.Load<CosmeticDefinition>("Cosmetics/" + liveryId);
            if (cosmetic == null || cosmetic.bodyColor.a <= 0.01f) return;

            foreach (var r in shipRoot.GetComponentsInChildren<Renderer>(true))
            {
                if (r == null) continue;
                string n = r.gameObject.name;
                // Body panels take bodyColor; trim/exhaust/canopy keep their identity.
                bool body = n.StartsWith("Fuselage_") || n.StartsWith("Wing_") || n == "TailFin" || n == "CargoPod";
                bool accent = n.StartsWith("WingTip_") || n == "DorsalSpine" || n == "Nose_Tip";
                if (!body && !accent) continue;
                Color c = body ? cosmetic.bodyColor
                    : (cosmetic.accentColor.a > 0.01f ? cosmetic.accentColor : cosmetic.bodyColor);
                var mat = r.material;
                if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", c);
                else mat.color = c;
            }
            Debug.Log("ZIPTIDE: SHIP_LIVERY id=" + liveryId);
        }

        // ── Journey decals (the hull remembers) ──────────────────────────────
        private static void ApplyDecals(Transform root, PlayerProfile profile)
        {
            var old = root.Find("JourneyDecals");
            if (old != null) Object.Destroy(old.gameObject);
            if (profile == null || profile.flags == null) return;
            var earned = ShipJourneyDecals.FromFlags(profile.flags);
            if (earned.Count == 0) return;

            var rail = new GameObject("JourneyDecals").transform;
            rail.SetParent(root, false);
            var mid = root.Find("Fuselage_Mid");
            float y = mid != null ? mid.localPosition.y + 0.02f : 0.4f;
            float x = mid != null ? -(mid.localScale.x * 0.5f + 0.02f) : -1.1f; // port flank
            for (int i = 0; i < earned.Count; i++)
            {
                var plate = GameObject.CreatePrimitive(PrimitiveType.Quad);
                plate.name = earned[i];
                var col = plate.GetComponent<Collider>();
                if (col != null) Object.Destroy(col);
                plate.transform.SetParent(rail, false);
                plate.transform.localPosition = new Vector3(x, y + 0.35f, 1.4f - i * 0.5f);
                plate.transform.localRotation = Quaternion.Euler(0f, 90f, 0f); // face out the port side
                plate.transform.localScale = Vector3.one * 0.3f;
                // Deterministic milestone tint (hash → hue) — reads as a row of service ribbons.
                float hue = ((earned[i].GetHashCode() & 0xFFFF) / (float)0xFFFF);
                ItemFactory.ApplyURPColor(plate, Color.HSVToRGB(hue, 0.55f, 0.9f));
            }
            Debug.Log("ZIPTIDE: SHIP_DECALS count=" + earned.Count);
        }

        // ── Nameplate ────────────────────────────────────────────────────────
        private static void ApplyNameplate(Transform root, PlayerProfile profile, ShipChassisPreset chassis)
        {
            var old = root.Find("ShipNameplate");
            if (old != null) Object.Destroy(old.gameObject);
            string shipName = ShipLocker.GetEquipped(profile, "name");
            if (string.IsNullOrEmpty(shipName)) shipName = "THE " + chassis.DisplayName.ToUpperInvariant();

            var go = new GameObject("ShipNameplate");
            go.transform.SetParent(root, false);
            var bow = root.Find("Fuselage_Bow");
            go.transform.localPosition = (bow != null ? bow.localPosition : new Vector3(0, 0, 3f))
                                         + new Vector3(0f, 1.0f, 0.5f);
            var tm = go.AddComponent<TextMesh>();
            tm.text = shipName;
            tm.characterSize = 0.02f; tm.fontSize = 64;   // the characterSize×fontSize lesson
            tm.anchor = TextAnchor.MiddleCenter;
            tm.color = new Color(0.85f, 0.9f, 1f);
        }

        // ── The hum (a voice per silhouette, zero assets) ────────────────────
        private static void ApplyHum(GameObject shipRoot, ShipChassisPreset chassis)
        {
            var src = shipRoot.GetComponent<AudioSource>();
            if (src == null) src = shipRoot.AddComponent<AudioSource>();
            src.clip = MakeHum(chassis);
            src.loop = true;
            src.volume = 0.16f;
            src.spatialBlend = 1f;      // 3D — the ship hums where it stands
            src.maxDistance = 22f;
            if (!src.isPlaying) src.Play();
        }

        /// <summary>Two-harmonic engine idle keyed to the silhouette: racers whine high, haulers
        /// throb low (base 42–96 Hz), with a slow amplitude breath so it reads as a living machine.</summary>
        private static AudioClip MakeHum(ShipChassisPreset chassis)
        {
            const int rate = 22050;
            const float seconds = 2f;
            int n = (int)(rate * seconds);
            float baseHz = 42f + chassis.BaseStats.Speed * 1.35f;   // faster chassis = higher voice
            var data = new float[n];
            for (int i = 0; i < n; i++)
            {
                float t = i / (float)rate;
                float breath = 0.8f + 0.2f * Mathf.Sin(2f * Mathf.PI * 0.5f * t);
                data[i] = breath * (0.6f * Mathf.Sin(2f * Mathf.PI * baseHz * t)
                                  + 0.3f * Mathf.Sin(2f * Mathf.PI * baseHz * 2.01f * t));
            }
            var clip = AudioClip.Create("Hum_" + chassis.Id, n, 1, rate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }

    /// <summary>Remembers a hull part's first-seen transform so the refit always multiplies the
    /// ORIGINAL proportions — chassis-swapping twice never compounds (sibling-class idiom).</summary>
    public class ShipRefitBaseXf : MonoBehaviour
    {
        public Vector3 scale, pos, euler;
    }
}
