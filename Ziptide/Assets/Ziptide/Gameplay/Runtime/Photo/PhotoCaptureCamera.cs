using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Ziptide.Content;
using Ziptide.Content.Photo;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// FIELD CAMERA — camera-owned live viewfinder + PNG capture. This is deliberately NOT a
    /// persistent singleton: one held item owns one hidden Camera, RenderTexture, readback texture,
    /// and viewfinder material, then destroys every created resource with the item.
    /// </summary>
    public sealed class PhotoCaptureCamera : MonoBehaviour
    {
        public const int CaptureWidth = 512;
        public const int CaptureHeight = 384;
        public const float MaxViewfinderHz = 10f;
        public const string PhotoFolderName = "Photos";
        private const string CaptureChildName = "__PHOTO_CAPTURE_CAMERA";

        private Transform _lens;
        private Renderer _screen;
        private CameraDefinition _definition;
        private Camera _camera;
        private RenderTexture _target;
        private Texture2D _readback;
        private Material _viewfinderMaterial;
        private float _nextPreview;
        private int _sequence;
        private bool _initialized;

        public Camera CaptureCamera => _camera;
        public RenderTexture Target => _target;
        public Renderer Screen => _screen;

        public void Initialize(Transform lens, Renderer screen, CameraDefinition definition)
        {
            _lens = lens;
            _screen = screen;
            _definition = definition;
            if (_initialized) return;
            _initialized = true;
            BuildResources();
        }

        private void Update()
        {
            if (!_initialized || _camera == null || _target == null) return;
            if (Time.unscaledTime < _nextPreview) return;

            float interval = 1f / MaxViewfinderHz;
            _nextPreview = Time.unscaledTime + interval;
            AlignCamera();
            _camera.Render();
        }

        /// <summary>Render, score, append to the bounded album, evict/delete oldest, and save.</summary>
        public bool TryCapture(out CapturedPhoto captured, out PhotoScore score)
        {
            captured = null;
            score = default;
            if (!_initialized) BuildResources();
            if (_camera == null || _target == null || _readback == null) return false;

            try
            {
                AlignCamera();
                _camera.Render();

                RenderTexture previous = RenderTexture.active;
                RenderTexture.active = _target;
                _readback.ReadPixels(new Rect(0, 0, CaptureWidth, CaptureHeight), 0, 0, false);
                _readback.Apply(false, false);
                RenderTexture.active = previous;

                PhotoSubjectDetector.Detection detected = PhotoSubjectDetector.Detect(_camera);
                score = PhotoComposition.Evaluate(new PhotoSubjects
                {
                    SkyTier = detected.skyTier,
                    HorizonInFrame = detected.horizonInFrame,
                    OccludedBodyInFrame = detected.occludedBodyInFrame,
                    LandmarkInFrame = detected.landmarkInFrame,
                    CreatureInFrame = detected.creatureInFrame,
                    FramingCentered = detected.framingCentered
                });

                long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                string fileName = BuildFileName(SceneManager.GetActiveScene().name, now, _sequence++);
                string folder = PhotoFolderPath(Application.persistentDataPath);
                Directory.CreateDirectory(folder);
                File.WriteAllBytes(Path.Combine(folder, fileName), _readback.EncodeToPNG());

                Vector3 euler = _camera.transform.eulerAngles;
                captured = new CapturedPhoto
                {
                    worldId = SceneManager.GetActiveScene().name,
                    atUnix = now,
                    yaw = NormalizeSignedAngle(euler.y),
                    pitch = NormalizeSignedAngle(euler.x),
                    fov = _camera.fieldOfView,
                    rating = (int)score.Rating,
                    subjectId = detected.subjectId ?? string.Empty,
                    file = fileName
                };

                SaveSystem save = SaveSystem.Instance;
                if (save == null || save.Profile == null)
                {
                    SafeDeletePhoto(Application.persistentDataPath, fileName);
                    captured = null;
                    Debug.LogWarning("ZIPTIDE: PHOTO_CAPTURE_FAIL reason=no_live_profile");
                    return false;
                }

                CapturedPhoto evicted = PhotoAlbum.Add(save.Profile.photos, captured);
                if (evicted != null && !string.IsNullOrEmpty(evicted.file))
                    SafeDeletePhoto(Application.persistentDataPath, evicted.file);
                save.Save();

                Debug.Log("ZIPTIDE: PHOTO_CAPTURED world=" + captured.worldId +
                          " rating=" + score.Rating +
                          " score=" + score.Total +
                          " file=" + captured.file +
                          " subject=" + captured.subjectId);
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogWarning("ZIPTIDE: PHOTO_CAPTURE_FAIL reason=" + exception.Message);
                captured = null;
                return false;
            }
        }

        public static string PhotoFolderPath(string persistentDataPath)
        {
            return Path.Combine(persistentDataPath ?? string.Empty, PhotoFolderName);
        }

        public static string BuildFileName(string worldId, long unixSeconds, int sequence)
        {
            string safeWorld = SanitizeFilePart(worldId);
            return "photo_" + safeWorld + "_" + Math.Max(0L, unixSeconds) + "_" +
                   Mathf.Max(0, sequence).ToString("D3") + ".png";
        }

        public static string SanitizeFilePart(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "world";
            char[] invalid = Path.GetInvalidFileNameChars();
            var builder = new System.Text.StringBuilder(value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                char c = value[i];
                bool bad = char.IsWhiteSpace(c) || Array.IndexOf(invalid, c) >= 0;
                builder.Append(bad ? '_' : char.ToLowerInvariant(c));
            }
            string result = builder.ToString().Trim('_');
            return string.IsNullOrEmpty(result) ? "world" : result;
        }

        public static bool SafeDeletePhoto(string persistentDataPath, string fileName)
        {
            if (string.IsNullOrEmpty(fileName) || Path.GetFileName(fileName) != fileName) return false;
            try
            {
                string path = Path.Combine(PhotoFolderPath(persistentDataPath), fileName);
                if (!File.Exists(path)) return false;
                File.Delete(path);
                Debug.Log("ZIPTIDE: PHOTO_EVICTED file=" + fileName);
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogWarning("ZIPTIDE: PHOTO_DELETE_FAIL file=" + fileName + " " + exception.Message);
                return false;
            }
        }

        private void BuildResources()
        {
            if (_camera != null) return;
            if (_lens == null) _lens = transform.Find("Lens");
            if (_lens == null) _lens = transform;

            var cameraObject = new GameObject(CaptureChildName);
            cameraObject.transform.SetParent(transform, false);
            _camera = cameraObject.AddComponent<Camera>();
            _camera.enabled = false;
            _camera.clearFlags = CameraClearFlags.Skybox;
            _camera.fieldOfView = Mathf.Clamp(
                _definition != null ? _definition.captureFov : 60f,
                35f,
                90f);
            _camera.nearClipPlane = 0.03f;
            _camera.farClipPlane = 1000f;
            _camera.allowHDR = false;
            _camera.allowMSAA = false;
            _camera.useOcclusionCulling = true;
            _camera.depthTextureMode = DepthTextureMode.None;
            _camera.cullingMask = ~(1 << 5); // exclude UI/viewfinder to prevent recursive capture

            _target = new RenderTexture(CaptureWidth, CaptureHeight, 16, RenderTextureFormat.ARGB32)
            {
                name = "FieldCameraTarget",
                filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp,
                antiAliasing = 1,
                useMipMap = false,
                autoGenerateMips = false
            };
            _target.Create();
            _camera.targetTexture = _target;

            _readback = new Texture2D(CaptureWidth, CaptureHeight, TextureFormat.RGB24, false, false)
            {
                name = "FieldCameraReadback",
                wrapMode = TextureWrapMode.Clamp,
                filterMode = FilterMode.Bilinear
            };

            if (_screen != null)
            {
                _screen.gameObject.layer = 5;
                Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
                if (shader == null) shader = Shader.Find("Unlit/Texture");
                if (shader == null) shader = Shader.Find("Sprites/Default");
                if (shader != null)
                {
                    _viewfinderMaterial = new Material(shader)
                    {
                        name = "FieldCameraViewfinder",
                        enableInstancing = false
                    };
                    if (_viewfinderMaterial.HasProperty("_BaseMap"))
                        _viewfinderMaterial.SetTexture("_BaseMap", _target);
                    if (_viewfinderMaterial.HasProperty("_MainTex"))
                        _viewfinderMaterial.SetTexture("_MainTex", _target);
                    if (_viewfinderMaterial.HasProperty("_BaseColor"))
                        _viewfinderMaterial.SetColor("_BaseColor", Color.white);
                    if (_viewfinderMaterial.HasProperty("_Color"))
                        _viewfinderMaterial.SetColor("_Color", Color.white);
                    _screen.sharedMaterial = _viewfinderMaterial;
                }
            }

            AlignCamera();
        }

        private void AlignCamera()
        {
            if (_camera == null || _lens == null) return;
            _camera.transform.SetPositionAndRotation(_lens.position, _lens.rotation);
        }

        private static float NormalizeSignedAngle(float degrees)
        {
            return Mathf.DeltaAngle(0f, degrees);
        }

        private void OnDestroy()
        {
            if (_camera != null) _camera.targetTexture = null;
            if (_target != null)
            {
                _target.Release();
                Destroy(_target);
            }
            if (_readback != null) Destroy(_readback);
            if (_viewfinderMaterial != null) Destroy(_viewfinderMaterial);
            if (_camera != null) Destroy(_camera.gameObject);
            _target = null;
            _readback = null;
            _viewfinderMaterial = null;
            _camera = null;
        }
    }
}
