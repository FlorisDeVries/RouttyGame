using GameManagement;
using GameManagement.Resources;
using UnityEngine.UIElements;

namespace UserInterface.Buttons
{
    public class UnpauseOnButtonPress : AButtonPress
    {
        protected override void OnButtonClicked(ClickEvent clickEvent)
        {
            GameManager.Instance.ChangeState(GameState.Playing);
        }
    }
}