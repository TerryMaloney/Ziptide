using UnityEngine;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// A ship's window onto the Moss system: a framed pane showing the baked starfield from
    /// <see cref="PortholeStarfieldCore"/>, with the ringed giant hanging in the corner of the
    /// view. Built from primitives with a generated texture — no assets, so it can be authored
    /// into any interior scene and re-skinned by the Forge later.
    ///
    /// It exists because the first hour opened in a ship you could not see out of. Cheap by
    /// construction: one 128×128 texture, unlit, no update loop.
    /// </summary>
    public class PortholeRuntime : MonoBehaviour
    {
        [SerializeField] private int seed = 909;
        [SerializeField, Range(0f, 1f)] private float starDensity = 0.85f;
        [SerializeField] private float paneWidth = 0.9f;
        [SerializeField] private float paneHeight = 0.7f;
        [SerializeField] private bool showGiant = true;

        private const int Resolution = 128;

        private void Start()
        {
            if (transform.Find("Pane") != null) return; // authored twice / re-entered — leave it be
            Build();
        }

        private void Build()
        {
            var pane = GameObject.CreatePrimitive(PrimitiveType.Quad);
            pane.name = "Pane";
            pane.transform.SetParent(transform, false);
            pane.transform.localScale = new Vector3(paneWidth, paneHeight, 1f);
            var col = pane.GetComponent<Collider>();
            if (col != null) Destroy(col);

            var tex = new Texture2D(Resolution, Resolution, TextureFormat.RGBA32, false)
            {
                name = "PortholeStarfield",
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp,
            };
            var buffer = new byte[Resolution * Resolution * 4];
            PortholeStarfieldCore.Bake(buffer, Resolution, Resolution, seed, starDensity);
            tex.LoadRawTextureData(buffer);
            tex.Apply(false, false);

            var shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null) shader = Shader.Find("Sprites/Default");
            var mat = new Material(shader);
            if (mat.HasProperty("_BaseMap")) mat.SetTexture("_BaseMap", tex);
            mat.mainTexture = tex;
            if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", Color.white);
            pane.GetComponent<Renderer>().sharedMaterial = mat;

            // The parent giant, low in the frame — the same body the sky shows outside, so the
            // view through the window agrees with the view from the deck.
            if (showGiant)
            {
                var giant = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                giant.name = "GiantThroughGlass";
                giant.transform.SetParent(pane.transform, false);
                giant.transform.localPosition = new Vector3(0.28f, -0.22f, -0.02f);
                giant.transform.localScale = Vector3.one * 0.30f;
                var gc = giant.GetComponent<Collider>();
                if (gc != null) Destroy(gc);
                var gm = new Material(shader);
                if (gm.HasProperty("_BaseColor")) gm.SetColor("_BaseColor", new Color(0.46f, 0.56f, 0.76f));
                else gm.color = new Color(0.46f, 0.56f, 0.76f);
                giant.GetComponent<Renderer>().sharedMaterial = gm;
            }

            BuildFrame();
            Debug.Log("ZIPTIDE: PORTHOLE built seed=" + seed + " density=" + starDensity.ToString("F2"));
        }

        /// <summary>Four bars around the pane — the window has to read as part of the hull.</summary>
        private void BuildFrame()
        {
            const float bar = 0.06f;
            Bar("Frame_T", new Vector3(0f, paneHeight * 0.5f, 0f), new Vector3(paneWidth + bar, bar, bar));
            Bar("Frame_B", new Vector3(0f, -paneHeight * 0.5f, 0f), new Vector3(paneWidth + bar, bar, bar));
            Bar("Frame_L", new Vector3(-paneWidth * 0.5f, 0f, 0f), new Vector3(bar, paneHeight, bar));
            Bar("Frame_R", new Vector3(paneWidth * 0.5f, 0f, 0f), new Vector3(bar, paneHeight, bar));
        }

        private void Bar(string name, Vector3 localPos, Vector3 scale)
        {
            var b = GameObject.CreatePrimitive(PrimitiveType.Cube);
            b.name = name;
            b.transform.SetParent(transform, false);
            b.transform.localPosition = localPos;
            b.transform.localScale = scale;
            var col = b.GetComponent<Collider>();
            if (col != null) Destroy(col);
            ItemFactory.ApplyURPColor(b, new Color(0.22f, 0.24f, 0.28f));
        }
    }
}
