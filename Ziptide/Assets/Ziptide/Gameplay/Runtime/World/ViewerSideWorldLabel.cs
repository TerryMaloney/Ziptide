using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// One side of a two-sided world label pair. Legacy TextMesh font materials render from both
    /// directions, so two always-enabled labels produce one readable face plus one mirrored face.
    /// This component enables only the label on the tracked viewer's side and re-applies the single
    /// WorldLabelFacing convention in the parent's local space.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ViewerSideWorldLabel : MonoBehaviour
    {
        private Vector3 _localCenter;
        private float _faceOffset;
        private int _sideSign;
        private Renderer _renderer;
        private Camera _viewer;
        private bool _configured;

        public void Configure(Vector3 localCenter, float faceOffset, int sideSign)
        {
            _localCenter = localCenter;
            _faceOffset = Mathf.Abs(faceOffset);
            _sideSign = sideSign < 0 ? -1 : 1;
            _renderer = GetComponent<Renderer>();
            _configured = true;
            UpdateFacing();
        }

        private void LateUpdate()
        {
            UpdateFacing();
        }

        private void UpdateFacing()
        {
            if (!_configured || transform.parent == null) return;
            Camera viewer = ResolveViewer();
            if (viewer == null) return;

            Vector3 viewerLocal = transform.parent.InverseTransformPoint(viewer.transform.position);
            float sideDelta = viewerLocal.z - _localCenter.z;
            bool visible = _sideSign < 0 ? sideDelta <= 0f : sideDelta > 0f;
            if (_renderer != null) _renderer.enabled = visible;
            if (!visible) return;

            Vector3 localPosition = _localCenter + Vector3.forward * (_faceOffset * _sideSign);
            transform.localPosition = localPosition;
            transform.localRotation = WorldLabelFacing.FaceViewer(
                localPosition,
                viewerLocal,
                yawOnly: false);
        }

        private Camera ResolveViewer()
        {
            if (_viewer != null && _viewer.isActiveAndEnabled) return _viewer;
            PlayerRigPersistence rig = FindObjectOfType<PlayerRigPersistence>();
            if (rig != null) _viewer = rig.GetComponentInChildren<Camera>(true);
            if (_viewer == null) _viewer = Camera.main;
            return _viewer;
        }
    }
}
