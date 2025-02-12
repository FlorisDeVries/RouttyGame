using _Common.Unity.Resources;
using GameManagement.Resources;
using UnityEngine;
using UnityEngine.UIElements;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

namespace UserInterface.Buttons.Resources
{
    [CreateAssetMenu(fileName = "SceneButtonProperties", menuName = "Project/UserInterface/SceneButtonProperties")]
    public class SceneButtonProperties : ScriptableObject
    {
        [SerializeField]
        private string _text;

        [SerializeField]
        private UnityScene _scene;

        public string Text => _text;

        public bool HasScene => _scene != null;

        public void RaiseEvent(ClickEvent clickEvent)
        {
            GameManager.Instance.LoadScene(_scene);
        }
    }
}