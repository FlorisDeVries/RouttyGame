using _Common.Resources;
using UnityEngine;

namespace UserInterface.Buttons.Resources
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