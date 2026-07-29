using NUnit.Framework;
using UnityEngine;
using Ziptide.Gameplay;

namespace Ziptide.Tests.EditMode
{
    /// <summary>
    /// ToxicCity shipped with one pack-declared marker (<c>player</c>) while its contract's steps
    /// pointed at <c>dispatch_inside</c>, <c>relay_node</c> and <c>shipyard_office</c> — objects the
    /// city builder authors straight into the scene. The old lookup searched only pack markers, so
    /// three of the first level's four steps could never complete and <c>toxiccity_complete</c> was
    /// never granted, which left W002 permanently locked. These tests pin the scene fallback.
    /// </summary>
    public class JobDirectorMarkerResolutionTests
    {
        private GameObject _director;
        private GameObject _authored;

        [SetUp]
        public void SetUp()
        {
            _director = new GameObject("JobDirector");
            _director.AddComponent<JobDirector>();
        }

        [TearDown]
        public void TearDown()
        {
            if (_director != null) Object.DestroyImmediate(_director);
            if (_authored != null) Object.DestroyImmediate(_authored);
        }

        [Test]
        public void ResolveMarker_FindsAMarkerAuthoredIntoTheScene()
        {
            _authored = new GameObject("Hero_DispatchHall");
            var marker = new GameObject("Marker_dispatch_inside");
            marker.transform.SetParent(_authored.transform);
            marker.transform.position = new Vector3(3f, 0f, 7f);

            var resolved = _director.GetComponent<JobDirector>().ResolveMarker("dispatch_inside");

            Assert.IsNotNull(resolved, "an authored scene marker must resolve even when the pack omits it");
            Assert.AreEqual(marker.transform, resolved);
        }

        [Test]
        public void ResolveMarker_FindsMarkersNestedSeveralLevelsDeep()
        {
            _authored = new GameObject("__TOXIC_CITY_ROOT");
            var district = new GameObject("District_Shipyard");
            district.transform.SetParent(_authored.transform);
            var hero = new GameObject("Hero_ShipyardOffice");
            hero.transform.SetParent(district.transform);
            var marker = new GameObject("Marker_shipyard_office");
            marker.transform.SetParent(hero.transform);

            var resolved = _director.GetComponent<JobDirector>().ResolveMarker("shipyard_office");

            Assert.AreEqual(marker.transform, resolved);
        }

        [Test]
        public void ResolveMarker_CachesSoRepeatedTicksDoNotRescanTheScene()
        {
            _authored = new GameObject("Marker_relay_node");

            var director = _director.GetComponent<JobDirector>();
            var first = director.ResolveMarker("relay_node");
            var second = director.ResolveMarker("relay_node");

            Assert.IsNotNull(first);
            Assert.AreSame(first, second);
        }

        [Test]
        public void ResolveMarker_ReturnsNullForAnUnknownIdWithoutThrowing()
        {
            Assert.IsNull(_director.GetComponent<JobDirector>().ResolveMarker("no_such_marker"));
        }

        [Test]
        public void ResolveMarker_IgnoresNullAndEmptyIds()
        {
            var director = _director.GetComponent<JobDirector>();
            Assert.IsNull(director.ResolveMarker(null));
            Assert.IsNull(director.ResolveMarker(string.Empty));
        }
    }
}
