using _Common.Events;
using _Common.Unity.Resources;
using GameManagement.Resources;
using UnityEngine;
using UnityEngine.UIElements;
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed

namespace UserInterface.Buttons
{
    public class LoadSceneOnButtonPress : AButtonPress
    {
        [SerializeField]
        private UnityScene _sceneReference;

        protected override void OnButtonClicked(ClickEvent clickEvent)
        {
            GameManager.Instance.LoadScene(_sceneReference);
        }
    }
}