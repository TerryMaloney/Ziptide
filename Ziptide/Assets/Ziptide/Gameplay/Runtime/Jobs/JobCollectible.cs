using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ziptide.Content;

namespace Ziptide.Gameplay
{
    /// <summary>
    /// Place on a grabbable item. When the player grabs it, reports collect to JobDirector for CollectItemIdCountStep.
    /// </summary>
    [RequireComponent(typeof(ItemRuntime))]
    [RequireComponent(typeof(XRGrabInteractable))]
    public class JobCollectible : MonoBehaviour
    {
        private JobDirector _director;
        private ItemRuntime _item;

        private void Awake()
        {
            _item = GetComponent<ItemRuntime>();
        }

        private void Start()
        {
            if (_director == null)
                _director = FindObjectOfType<JobDirector>();
            var grab = GetComponent<XRGrabInteractable>();
            if (grab != null)
                grab.selectEntered.AddListener(OnGrabbed);
        }

        private void OnDestroy()
        {
            var grab = GetComponent<XRGrabInteractable>();
            if (grab != null)
                grab.selectEntered.RemoveListener(OnGrabbed);
        }

        public void Bind(JobDirector director)
        {
            _director = director;
        }

        private void OnGrabbed(SelectEnterEventArgs args)
        {
            if (_director == null || _item == null || _item.Definition == null) return;
            _director.ReportCollect(_item.Definition.itemId);
        }
    }
}
