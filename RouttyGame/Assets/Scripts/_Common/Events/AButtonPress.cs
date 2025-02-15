using _Common.Events.Resources;
using UnityEngine;
using UnityEngine.UIElements;

namespace _Common.Events
{
    public abstract class AButtonPress : MonoBehaviour
    {
        [SerializeField]
        private ClickEventObject _eventObject;

        private void OnEnable()
        {
            Setup();

            if (_eventObject)
            {
                _eventObject.RegisterListener(OnButtonClicked);
            }
        }

        private void OnDisable()
        {
            if (_eventObject)
            {
                _eventObject.UnregisterListener(OnButtonClicked);
            }
        }

        protected virtual void Setup()
        {
        }

        protected abstract void OnButtonClicked(ClickEvent clickEvent);
    }
}