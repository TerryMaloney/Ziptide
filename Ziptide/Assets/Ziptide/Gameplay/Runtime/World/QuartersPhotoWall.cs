using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// FIELD CAMERA — six newest captured photos on the Quarters back wall. Files are device-local;
    /// missing/corrupt files degrade to a neutral panel. Every loaded Texture2D/Material is owned and
    /// destroyed by this scene-local wall.
    /// </summary>
    public sealed class QuartersPhotoWall : MonoBehaviour
    {
        public const int MaxDisplayedPhotos = 6;
        public const string RootName = "CapturedPhotoWall";
        private const float WallZ = 2.225f;

        private readonly List<Texture2D> _textures = new List<Texture2D>();
        private readonly List<Material> _materials = new List<Material>();
        private Transform _builtRoot;

        private static readonly Color SnapshotFrame = new Color(0.34f, 0.37f, 0.42f);
        private static readonly Color PostcardFrame = new Color(0.27f, 0.64f, 0.70f);
        private static readonly Color MasterpieceFrame = new Color(0.92f, 0.68f, 0.24f);
        private static readonly Color MissingColor = new Color(0.08f, 0.10f, 0.12f);

        private void Start()
        {
            Rebuild();
        }

        public void Rebuild()
        {
            ClearBuiltResources();

            var root = new GameObject(RootName);
            _builtRoot = root.transform;
            _builtRoot.SetParent(transform, false);

            BuildTitle(_builtRoot);
            PlayerProfile profile = SaveSystem.Instance != null ? SaveSystem.Instance.Profile : null;
            List<CapturedPhoto> photos = profile != null ? profile.photos : null;
            int count = Mathf.Min(MaxDisplayedPhotos, photos != null ? photos.Count : 0);

            if (count == 0)
            {
                BuildEmptyNotice(_builtRoot);
                return;
            }

            for (int slot = 0; slot < count; slot++)
            {
                CapturedPhoto photo = photos[photos.Count - 1 - slot];
                BuildFrame(_builtRoot, slot, photo);
            }
        }

        public static Vector3 SlotLocalPosition(int slot)
        {
            slot = Mathf.Clamp(slot, 0, MaxDisplayedPhotos - 1);
            int column = slot % 3;
            int row = slot / 3;
            return new Vector3(-1.30f + column * 1.30f, 1.62f - row * 0.76f, WallZ);
        }

        public static Color FrameColorForRating(int rating)
        {
            if (rating >= 2) return MasterpieceFrame;
            if (rating == 1) return PostcardFrame;
            return SnapshotFrame;
        }

        private void BuildFrame(Transform parent, int slot, CapturedPhoto photo)
        {
            var holder = new GameObject("Photo_" + slot);
            holder.transform.SetParent(parent, false);
            holder.transform.localPosition = SlotLocalPosition(slot);
            holder.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);

            GameObject frame = GameObject.CreatePrimitive(PrimitiveType.Cube);
            frame.name = "Frame";
            frame.transform.SetParent(holder.transform, false);
            frame.transform.localScale = new Vector3(1.08f, 0.64f, 0.035f);
            DisablePhysical(frame);
            AssignMaterial(frame.GetComponent<Renderer>(), CreateColorMaterial(
                "PhotoFrame_" + slot,
                FrameColorForRating(photo != null ? photo.rating : 0)));

            GameObject image = GameObject.CreatePrimitive(PrimitiveType.Quad);
            image.name = "Image";
            image.transform.SetParent(holder.transform, false);
            image.transform.localPosition = new Vector3(0f, 0f, -0.021f);
            image.transform.localScale = new Vector3(0.98f, 0.54f, 1f);
            DisablePhysical(image);

            Material imageMaterial = LoadPhotoMaterial(photo);
            AssignMaterial(image.GetComponent<Renderer>(), imageMaterial);
        }

        private Material LoadPhotoMaterial(CapturedPhoto photo)
        {
            if (photo != null && !string.IsNullOrEmpty(photo.file) && Path.GetFileName(photo.file) == photo.file)
            {
                string path = Path.Combine(
                    PhotoCaptureCamera.PhotoFolderPath(Application.persistentDataPath),
                    photo.file);
                try
                {
                    if (File.Exists(path))
                    {
                        byte[] bytes = File.ReadAllBytes(path);
                        var texture = new Texture2D(2, 2, TextureFormat.RGB24, false, false)
                        {
                            name = "Captured_" + photo.file,
                            wrapMode = TextureWrapMode.Clamp,
                            filterMode = FilterMode.Bilinear
                        };
                        if (texture.LoadImage(bytes, false))
                        {
                            _textures.Add(texture);
                            return CreateTextureMaterial("PhotoMat_" + photo.file, texture);
                        }
                        Destroy(texture);
                    }
                }
                catch (System.Exception exception)
                {
                    Debug.LogWarning("ZIPTIDE: PHOTO_WALL_LOAD_FAIL file=" + photo.file + " " + exception.Message);
                }
            }

            return CreateColorMaterial("PhotoMissing", MissingColor);
        }

        private Material CreateTextureMaterial(string name, Texture texture)
        {
            Material material = CreateBaseMaterial(name);
            if (material == null) return null;
            if (material.HasProperty("_BaseMap")) material.SetTexture("_BaseMap", texture);
            if (material.HasProperty("_MainTex")) material.SetTexture("_MainTex", texture);
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", Color.white);
            if (material.HasProperty("_Color")) material.SetColor("_Color", Color.white);
            return material;
        }

        private Material CreateColorMaterial(string name, Color color)
        {
            Material material = CreateBaseMaterial(name);
            if (material == null) return null;
            if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
            if (material.HasProperty("_Color")) material.SetColor("_Color", color);
            return material;
        }

        private Material CreateBaseMaterial(string name)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null) shader = Shader.Find("Unlit/Texture");
            if (shader == null) shader = Shader.Find("Sprites/Default");
            if (shader == null) return null;
            var material = new Material(shader) { name = name };
            _materials.Add(material);
            return material;
        }

        private static void AssignMaterial(Renderer renderer, Material material)
        {
            if (renderer == null) return;
            renderer.sharedMaterial = material;
            renderer.shadowCastingMode = ShadowCastingMode.Off;
            renderer.receiveShadows = false;
            renderer.lightProbeUsage = LightProbeUsage.Off;
            renderer.reflectionProbeUsage = ReflectionProbeUsage.Off;
        }

        private static void DisablePhysical(GameObject gameObject)
        {
            Collider collider = gameObject.GetComponent<Collider>();
            if (collider != null) Object.Destroy(collider);
            gameObject.isStatic = true;
        }

        private static void BuildTitle(Transform parent)
        {
            var title = new GameObject("Title");
            title.transform.SetParent(parent, false);
            title.transform.localPosition = new Vector3(0f, 2.08f, WallZ - 0.03f);
            title.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            TextMesh text = title.AddComponent<TextMesh>();
            text.text = "FIELD NOTES";
            text.anchor = TextAnchor.MiddleCenter;
            text.alignment = TextAlignment.Center;
            text.fontSize = 48;
            text.characterSize = 0.035f;
            text.color = new Color(0.82f, 0.88f, 0.92f);
        }

        private static void BuildEmptyNotice(Transform parent)
        {
            var notice = new GameObject("EmptyNotice");
            notice.transform.SetParent(parent, false);
            notice.transform.localPosition = new Vector3(0f, 1.25f, WallZ - 0.03f);
            notice.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            TextMesh text = notice.AddComponent<TextMesh>();
            text.text = "NO CAPTURES YET\n\nTAKE THE FIELD CAMERA\nAND BRING A WORLD HOME";
            text.anchor = TextAnchor.MiddleCenter;
            text.alignment = TextAlignment.Center;
            text.fontSize = 48;
            text.characterSize = 0.026f;
            text.color = new Color(0.50f, 0.58f, 0.64f);
        }

        private void ClearBuiltResources()
        {
            if (_builtRoot != null) Destroy(_builtRoot.gameObject);
            _builtRoot = null;

            for (int i = 0; i < _materials.Count; i++)
                if (_materials[i] != null) Destroy(_materials[i]);
            _materials.Clear();

            for (int i = 0; i < _textures.Count; i++)
                if (_textures[i] != null) Destroy(_textures[i]);
            _textures.Clear();
        }

        private void OnDestroy()
        {
            ClearBuiltResources();
        }
    }
}
