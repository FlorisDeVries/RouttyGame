using UnityEngine;

namespace _Common.Events.Resources
{
    [CreateAssetMenu(fileName = "ButtonProperties", menuName = "Project/UserInterface/ButtonProperties")]
    public class ButtonProperties : ScriptableObject
    {
        [SerializeField]
        private string _text;

        [SerializeField]
        private ClickEventObject _eventObject;
        
        public string Text => _text;
        public ClickEventObject EventObject => _eventObject;
    }
}