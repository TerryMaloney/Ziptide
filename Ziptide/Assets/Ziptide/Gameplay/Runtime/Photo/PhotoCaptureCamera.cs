using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Ziptide.Content;
using Ziptide.Content.Photo;
using Ziptide.Core;

namespace Ziptide.Gameplay
{
    /// <summary>Item-owned low-rate viewfinder and bounded PNG capture pipeline.</summary>
    public sealed class PhotoCaptureCamera : MonoBehaviour
    {
        public const int CaptureWidth = 512;
        public const int CaptureHeight = 384;
        public const float MaxViewfinderHz = 10f;
        public const string PhotoFolderName = "Photos";

        private Transform _lens;
        private Renderer _screen;
        private CameraDefinition _definition;
        private Camera _camera;
        private RenderTexture _target;
        private Texture2D _readback;
        private Material _viewfinderMaterial;
        private float _nextPreview;
        private int _sequence;

        public void Initialize(Transform lens, Renderer screen, CameraDefinition definition)
        {
            _lens = lens;
            _screen = screen;
            _definition = definition;
            BuildResources();
        }

        private void Update()
        {
            if (_camera == null || Time.unscaledTime < _nextPreview) return;
            _nextPreview = Time.unscaledTime + 1f / MaxViewfinderHz;
            AlignCamera();
            _camera.Render();
        }

        public bool TryCapture(out CapturedPhoto captured, out PhotoScore score)
        {
            captured = null;
            score = default;
            if (_camera == null) BuildResources();
            if (_camera == null || _target == null || _readback == null) return false;

            string writtenFile = null;
            try
            {
                AlignCamera();
                _camera.Render();
                RenderTexture previous = RenderTexture.active;
                try
                {
                    RenderTexture.active = _target;
                    _readback.ReadPixels(new Rect(0, 0, CaptureWidth, CaptureHeight), 0, 0, false);
                    _readback.Apply(false, false);
                }
                finally { RenderTexture.active = previous; }

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
                string world = SceneManager.GetActiveScene().name;
                string file = BuildFileName(world, now, _sequence++);
                Directory.CreateDirectory(PhotoFolderPath(Application.persistentDataPath));
                File.WriteAllBytes(Path.Combine(PhotoFolderPath(Application.persistentDataPath), file), _readback.EncodeToPNG());
                writtenFile = file;

                Vector3 euler = _camera.transform.eulerAngles;
                captured = new CapturedPhoto
                {
                    worldId = world,
                    atUnix = now,
                    yaw = Mathf.DeltaAngle(0f, euler.y),
                    pitch = Mathf.DeltaAngle(0f, euler.x),
                    fov = _camera.fieldOfView,
                    rating = (int)score.Rating,
                    subjectId = detected.subjectId ?? string.Empty,
                    file = file
                };

                SaveSystem save = SaveSystem.Instance;
                if (save == null || save.Profile == null) throw new InvalidOperationException("no_live_profile");
                CapturedPhoto evicted = PhotoAlbum.Add(save.Profile.photos, captured);
                if (evicted != null) SafeDeletePhoto(Application.persistentDataPath, evicted.file);
                save.Save();
                Debug.Log("ZIPTIDE: PHOTO_CAPTURED world=" + world + " rating=" + score.Rating +
                          " score=" + score.Total + " file=" + file + " subject=" + captured.subjectId);
                return true;
            }
            catch (Exception exception)
            {
                if (!string.IsNullOrEmpty(writtenFile)) SafeDeletePhoto(Application.persistentDataPath, writtenFile);
                captured = null;
                Debug.LogWarning("ZIPTIDE: PHOTO_CAPTURE_FAIL reason=" + exception.Message);
                return false;
            }
        }

        public static string PhotoFolderPath(string persistentDataPath)
            => Path.Combine(persistentDataPath ?? string.Empty, PhotoFolderName);

        public static string BuildFileName(string worldId, long unixSeconds, int sequence)
            => "photo_" + SanitizeFilePart(worldId) + "_" + Math.Max(0L, unixSeconds) + "_" +
               Mathf.Max(0, sequence).ToString("D3") + ".png";

        public static string SanitizeFilePart(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "world";
            char[] invalid = Path.GetInvalidFileNameChars();
            var builder = new System.Text.StringBuilder(value.Length);
            foreach (char c in value)
                builder.Append(char.IsWhiteSpace(c) || Array.IndexOf(invalid, c) >= 0 || c == '/' || c == '\\'
                    ? '_' : char.ToLowerInvariant(c));
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

            var cameraObject = new GameObject("__PHOTO_CAPTURE_CAMERA");
            cameraObject.transform.SetParent(transform, false);
            _camera = cameraObject.AddComponent<Camera>();
            _camera.enabled = false;
            _camera.clearFlags = CameraClearFlags.Skybox;
            _camera.fieldOfView = Mathf.Clamp(_definition != null ? _definition.captureFov : 60f, 35f, 90f);
            _camera.nearClipPlane = 0.03f;
            _camera.farClipPlane = 1000f;
            _camera.allowHDR = false;
            _camera.allowMSAA = false;
            _camera.depthTextureMode = DepthTextureMode.None;
            _camera.cullingMask = ~(1 << 5);

            _target = new RenderTexture(CaptureWidth, CaptureHeight, 16, RenderTextureFormat.ARGB32)
            {
                name = "FieldCameraTarget", filterMode = FilterMode.Bilinear,
                wrapMode = TextureWrapMode.Clamp, antiAliasing = 1, useMipMap = false
            };
            _target.Create();
            _camera.targetTexture = _target;
            _readback = new Texture2D(CaptureWidth, CaptureHeight, TextureFormat.RGB24, false)
            { name = "FieldCameraReadback", wrapMode = TextureWrapMode.Clamp, filterMode = FilterMode.Bilinear };

            if (_screen != null)
            {
                _screen.gameObject.layer = 5;
                Shader shader = Shader.Find("Universal Render Pipeline/Unlit") ??
                                Shader.Find("Unlit/Texture") ?? Shader.Find("Sprites/Default");
                if (shader != null)
                {
                    _viewfinderMaterial = new Material(shader) { name = "FieldCameraViewfinder" };
                    if (_viewfinderMaterial.HasProperty("_BaseMap")) _viewfinderMaterial.SetTexture("_BaseMap", _target);
                    if (_viewfinderMaterial.HasProperty("_MainTex")) _viewfinderMaterial.SetTexture("_MainTex", _target);
                    if (_viewfinderMaterial.HasProperty("_BaseColor")) _viewfinderMaterial.SetColor("_BaseColor", Color.white);
                    if (_viewfinderMaterial.HasProperty("_Color")) _viewfinderMaterial.SetColor("_Color", Color.white);
                    _screen.sharedMaterial = _viewfinderMaterial;
                }
            }
            AlignCamera();
        }

        private void AlignCamera()
        {
            if (_camera != null && _lens != null)
                _camera.transform.SetPositionAndRotation(_lens.position, _lens.rotation);
        }

        private void OnDestroy()
        {
            if (_camera != null) _camera.targetTexture = null;
            if (_target != null) { _target.Release(); Destroy(_target); }
            if (_readback != null) Destroy(_readback);
            if (_viewfinderMaterial != null) Destroy(_viewfinderMaterial);
            if (_camera != null) Destroy(_camera.gameObject);
        }
    }
}
