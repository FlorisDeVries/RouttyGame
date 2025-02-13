using Scores.Resources;
using UnityEngine;
using UserInterface.Generation;

namespace UserInterface
{
    public class ShowOnLeaderboards : MonoBehaviour
    {
        private UIDocumentGenerator _uiDocumentGenerator;
        
        private void OnEnable()
        {
            _uiDocumentGenerator = GetComponent<UIDocumentGenerator>();

            ScoreManager.Instance.OnShowLeaderBoardsEvent += ToggleVisibility;
        }
        
        private void OnDisable()
        {
            ScoreManager.Instance.OnShowLeaderBoardsEvent -= ToggleVisibility;
        }

        private void ToggleVisibility(bool newState)
        {
            SetVisibility(newState);
        }

        private void SetVisibility(bool show)
        {
            if (!_uiDocumentGenerator)
            {
                return;
            }
            
            if (show)
            {
                _uiDocumentGenerator.Show();
            }
            else
            {
                _uiDocumentGenerator.Hide();
            }
        }
    }
}