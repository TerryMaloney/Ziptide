using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>Six newest device-local captures, with owned texture/material cleanup.</summary>
    public sealed class QuartersPhotoWall : MonoBehaviour
    {
        public const int MaxDisplayedPhotos = 6;
        public const string RootName = "CapturedPhotoWall";
        private readonly List<Texture2D> _textures = new List<Texture2D>();
        private readonly List<Material> _materials = new List<Material>();
        private Transform _root;

        private void Start() => Rebuild();

        public void Rebuild()
        {
            ClearBuiltResources();
            _root = new GameObject(RootName).transform;
            _root.SetParent(transform, false);
            PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            List<CapturedPhoto> photos = profile != null ? profile.photos : null;
            int count = Mathf.Min(MaxDisplayedPhotos, photos != null ? photos.Count : 0);
            if (count == 0) { Label("NO CAPTURES YET\nTAKE THE FIELD CAMERA", new Vector3(0f, 1.3f, 0.02f)); return; }
            for (int slot = 0; slot < count; slot++) BuildFrame(slot, photos[photos.Count - 1 - slot]);
        }

        public static Vector3 SlotLocalPosition(int slot)
        {
            slot = Mathf.Clamp(slot, 0, MaxDisplayedPhotos - 1);
            return new Vector3(-1.3f + (slot % 3) * 1.3f, 1.62f - (slot / 3) * 0.76f, 0.025f);
        }

        public static Color FrameColorForRating(int rating)
            => rating >= 2 ? new Color(0.92f, 0.68f, 0.24f)
             : rating == 1 ? new Color(0.27f, 0.64f, 0.70f)
             : new Color(0.34f, 0.37f, 0.42f);

        private void BuildFrame(int slot, CapturedPhoto photo)
        {
            Transform holder = new GameObject("Photo_" + slot).transform;
            holder.SetParent(_root, false);
            holder.localPosition = SlotLocalPosition(slot);
            GameObject frame = Primitive(PrimitiveType.Cube, holder, "Frame", new Vector3(1.08f, 0.64f, 0.035f));
            SetMaterial(frame.GetComponent<Renderer>(), ColorMaterial("Frame_" + slot, FrameColorForRating(photo.rating)));
            GameObject image = Primitive(PrimitiveType.Quad, holder, "Image", new Vector3(0.98f, 0.54f, 1f));
            image.transform.localPosition = new Vector3(0f, 0f, -0.021f);
            image.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            SetMaterial(image.GetComponent<Renderer>(), PhotoMaterial(photo));
        }

        private Material PhotoMaterial(CapturedPhoto photo)
        {
            if (photo != null && Path.GetFileName(photo.file) == photo.file)
            {
                string path = Path.Combine(PhotoCaptureCamera.PhotoFolderPath(Application.persistentDataPath), photo.file);
                if (File.Exists(path))
                {
                    var texture = new Texture2D(2, 2, TextureFormat.RGB24, false);
                    if (texture.LoadImage(File.ReadAllBytes(path), false))
                    {
                        _textures.Add(texture);
                        Material material = BaseMaterial("Photo_" + photo.file);
                        if (material != null)
                        {
                            if (material.HasProperty("_BaseMap")) material.SetTexture("_BaseMap", texture);
                            if (material.HasProperty("_MainTex")) material.SetTexture("_MainTex", texture);
                        }
                        return material;
                    }
                    Destroy(texture);
                }
            }
            return ColorMaterial("MissingPhoto", new Color(0.08f, 0.10f, 0.12f));
        }

        private Material ColorMaterial(string name, Color color)
        {
            Material material = BaseMaterial(name);
            if (material != null)
            {
                if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
                if (material.HasProperty("_Color")) material.SetColor("_Color", color);
            }
            return material;
        }

        private Material BaseMaterial(string name)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Unlit") ?? Shader.Find("Unlit/Texture") ?? Shader.Find("Sprites/Default");
            if (shader == null) return null;
            var material = new Material(shader) { name = name };
            _materials.Add(material);
            return material;
        }

        private static GameObject Primitive(PrimitiveType type, Transform parent, string name, Vector3 scale)
        {
            GameObject go = GameObject.CreatePrimitive(type);
            go.name = name; go.transform.SetParent(parent, false); go.transform.localScale = scale;
            Collider collider = go.GetComponent<Collider>(); if (collider != null) Destroy(collider);
            return go;
        }

        private static void SetMaterial(Renderer renderer, Material material)
        {
            if (renderer == null) return;
            renderer.sharedMaterial = material; renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false; renderer.lightProbeUsage = LightProbeUsage.Off;
        }

        private void Label(string value, Vector3 position)
        {
            var go = new GameObject("EmptyNotice"); go.transform.SetParent(_root, false); go.transform.localPosition = position;
            TextMesh text = go.AddComponent<TextMesh>(); text.text = value; text.anchor = TextAnchor.MiddleCenter;
            text.alignment = TextAlignment.Center; text.fontSize = 48; text.characterSize = 0.028f;
        }

        private void ClearBuiltResources()
        {
            if (_root != null) Destroy(_root.gameObject); _root = null;
            foreach (Material material in _materials) if (material != null) Destroy(material); _materials.Clear();
            foreach (Texture2D texture in _textures) if (texture != null) Destroy(texture); _textures.Clear();
        }

        private void OnDestroy() => ClearBuiltResources();
    }
}
