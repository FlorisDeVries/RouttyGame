using _Common.Events;
using GameManagement.Resources;
using UnityEngine.UIElements;

namespace UserInterface.Buttons
{
    public class QuitGameOnButtonPress : AButtonPress
    {
        protected override void OnButtonClicked(ClickEvent clickEvent)
        {
            GameManager.Instance.QuitGame();
        }
    }
}