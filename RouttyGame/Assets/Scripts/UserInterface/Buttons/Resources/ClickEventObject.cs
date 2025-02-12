using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace UserInterface.Buttons.Resources
{
    [CreateAssetMenu(fileName = "ClickEventObject", menuName = "Project/Events/ClickEventObject")]
    public class ClickEventObject : ScriptableObject
    {
        private readonly UnityEvent<ClickEvent> _event = new();
        
        public void RegisterListener(UnityAction<ClickEvent> listener)
        {
            _event.AddListener(listener);
        }
        
        public void UnregisterListener(UnityAction<ClickEvent> listener)
        {
            _event.RemoveListener(listener);
        }
        
        public void RaiseEvent(ClickEvent clickEvent)
        {
            _event.Invoke(clickEvent);
        }

        [Button]
        public void ButtonPressed()
        {
            RaiseEvent(new ClickEvent());
        }
    }
}