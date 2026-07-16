using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Keeps the boardable ship's dense interior UI out of the exterior arrival view. The cockpit,
    /// helm and hangar surfaces become active only while the tracked head is on the cockpit deck;
    /// Quarters browsing surfaces become active only while the head is inside the room. This is a
    /// local ship component, not a bootstrap or alternate state owner: ShipBoardingStation continues
    /// to own every teleport, selection and travel callback.
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class ShipBoardingPresentationGuard : MonoBehaviour
    {
        private const float DeckRevealRadius = 4.5f;
        private const float QuartersRevealRadius = 4.0f;
        private const float HelmCharacterSize = 0.016f;
        private const float HierarchyRetrySeconds = 0.5f;
        private const int FastHierarchyRetryScans = 12;

        private static readonly string[] DeckUiNames =
        {
            "HelmRows",
            "DisembarkPanel",
            "QuartersPanel",
            "HangarBay"
        };

        private readonly List<GameObject> _deckUi = new List<GameObject>();
        private readonly List<GameObject> _quartersUi = new List<GameObject>();

        private Transform _cockpitDeck;
        private Transform _quarters;
        private Transform _helmRows;
        private Transform _disembarkPanel;
        private Transform _quartersPanel;
        private Transform _hangarBay;
        private Camera _viewer;
        private float _nextHierarchyRetryAt;
        private bool _hierarchyResolved;
        private int _hierarchyScanCount;

        public void RefreshNow()
        {
            ResolveHierarchy();
            UpdatePresentation();
        }

        private void Start()
        {
            RefreshNow();
        }

        private void LateUpdate()
        {
            UpdatePresentation();
        }

        private void UpdatePresentation()
        {
            EnsureHierarchy();
            Camera viewer = ResolveViewer();
            if (viewer == null) return;

            bool showDeck = _cockpitDeck != null &&
                HorizontalDistance(viewer.transform.position, _cockpitDeck.position) <= DeckRevealRadius;
            bool showQuarters = _quarters != null &&
                HorizontalDistance(viewer.transform.position, _quarters.position) <= QuartersRevealRadius;

            SetActive(_deckUi, showDeck);
            SetActive(_quartersUi, showQuarters);

            if (showDeck)
            {
                FacePanel(_disembarkPanel, viewer.transform.position);
                FacePanel(_quartersPanel, viewer.transform.position);
                FacePanel(_hangarBay, viewer.transform.position);
            }
        }

        private void EnsureHierarchy()
        {
            if (_hierarchyResolved)
            {
                if (CachedHierarchyIsValid()) return;
                _hierarchyResolved = false;
                _nextHierarchyRetryAt = 0f;
            }
            if (Time.unscaledTime < _nextHierarchyRetryAt) return;
            ResolveHierarchy();
        }

        private void ResolveHierarchy()
        {
            _hierarchyScanCount++;

            Transform[] all = GetComponentsInChildren<Transform>(true);
            _cockpitDeck = FindByName(all, "CockpitDeck");
            _quarters = FindByName(all, "Quarters");
            _helmRows = FindByName(all, "HelmRows");
            _disembarkPanel = FindByName(all, "DisembarkPanel");
            _quartersPanel = FindByName(all, "QuartersPanel");
            _hangarBay = FindByName(all, "HangarBay");

            _deckUi.Clear();
            for (int i = 0; i < DeckUiNames.Length; i++)
            {
                Transform value = FindByName(all, DeckUiNames[i]);
                if (value != null) _deckUi.Add(value.gameObject);
            }

            _quartersUi.Clear();
            if (_quarters != null)
            {
                Transform[] room = _quarters.GetComponentsInChildren<Transform>(true);
                for (int i = 0; i < room.Length; i++)
                {
                    Transform value = room[i];
                    if (value == null || value == _quarters) continue;
                    string name = value.name ?? string.Empty;
                    if (name.StartsWith("Bay_", StringComparison.Ordinal) ||
                        name == "LockerBoard" ||
                        name == "QuartersReturn" ||
                        name.StartsWith("Display_", StringComparison.Ordinal))
                    {
                        AddRootOnly(_quartersUi, value.gameObject);
                    }
                }
            }

            NormalizeHelmLabels();
            _hierarchyResolved = CachedHierarchyIsValid();
            _nextHierarchyRetryAt = _hierarchyResolved
                ? float.PositiveInfinity
                : Time.unscaledTime + (_hierarchyScanCount < FastHierarchyRetryScans
                    ? 0f
                    : HierarchyRetrySeconds);
        }

        private bool CachedHierarchyIsValid()
        {
            return _cockpitDeck != null &&
                   _quarters != null &&
                   _helmRows != null &&
                   _disembarkPanel != null &&
                   _quartersPanel != null &&
                   _hangarBay != null &&
                   _deckUi.Count == DeckUiNames.Length &&
                   _quartersUi.Count > 0;
        }

        private Camera ResolveViewer()
        {
            if (_viewer != null && _viewer.isActiveAndEnabled) return _viewer;

            PlayerRigPersistence rig = FindObjectOfType<PlayerRigPersistence>();
            if (rig != null)
                _viewer = rig.GetComponentInChildren<Camera>(true);
            if (_viewer == null) _viewer = Camera.main;
            return _viewer;
        }

        private static void FacePanel(Transform value, Vector3 viewerPosition)
        {
            if (value == null || !value.gameObject.activeInHierarchy) return;
            value.rotation = WorldLabelFacing.FaceViewer(
                value.position,
                viewerPosition,
                yawOnly: true);
        }

        private void NormalizeHelmLabels()
        {
            if (_helmRows == null) return;
            TextMesh[] labels = _helmRows.GetComponentsInChildren<TextMesh>(true);
            for (int i = 0; i < labels.Length; i++)
            {
                TextMesh label = labels[i];
                if (label != null && label.characterSize > HelmCharacterSize)
                    label.characterSize = HelmCharacterSize;
            }
        }

        private void SetActive(List<GameObject> values, bool active)
        {
            for (int i = values.Count - 1; i >= 0; i--)
            {
                GameObject value = values[i];
                if (value == null)
                {
                    values.RemoveAt(i);
                    _hierarchyResolved = false;
                    _nextHierarchyRetryAt = 0f;
                    continue;
                }
                if (value.activeSelf != active) value.SetActive(active);
            }
        }

        private static void AddRootOnly(List<GameObject> values, GameObject candidate)
        {
            if (candidate == null) return;
            for (int i = 0; i < values.Count; i++)
            {
                GameObject existing = values[i];
                if (existing == candidate) return;
                if (existing != null && candidate.transform.IsChildOf(existing.transform)) return;
            }
            for (int i = values.Count - 1; i >= 0; i--)
            {
                GameObject existing = values[i];
                if (existing != null && existing.transform.IsChildOf(candidate.transform))
                    values.RemoveAt(i);
            }
            values.Add(candidate);
        }

        private static Transform FindByName(Transform[] values, string name)
        {
            for (int i = 0; i < values.Length; i++)
            {
                Transform value = values[i];
                if (value != null && string.Equals(value.name, name, StringComparison.Ordinal))
                    return value;
            }
            return null;
        }

        private static float HorizontalDistance(Vector3 a, Vector3 b)
        {
            Vector2 axz = new Vector2(a.x, a.z);
            Vector2 bxz = new Vector2(b.x, b.z);
            return Vector2.Distance(axz, bxz);
        }
    }
}
