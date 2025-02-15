using _Common.Events;
using Scores.Resources;
using UnityEngine.UIElements;

namespace UserInterface.Buttons
{
    public class ShowLeaderboardsOnButtonPress : AButtonPress
    {
        public bool Show;
        
        protected override void OnButtonClicked(ClickEvent clickEvent)
        {
            ScoreManager.Instance.ShowLeaderBoards(Show);
        }
    }
}