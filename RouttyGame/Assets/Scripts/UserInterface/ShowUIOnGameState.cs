using System.Collections.Generic;
using GameManagement;
using GameManagement.Resources;
using Scores.Resources;
using UnityEngine;
using UserInterface.Generation;

namespace UserInterface
{
    public class ShowUIOnGameState : MonoBehaviour
    {
        [SerializeField]
        private List<GameState> _visibleOnGameStates = new();

        private UIDocumentGenerator _uiDocumentGenerator;
        
        private bool _leaderboardShown;

        private void OnEnable()
        {
            _uiDocumentGenerator = GetComponent<UIDocumentGenerator>();

            GameManager.Instance.OnEnterStateEvent += ToggleVisibility;
            ScoreManager.Instance.OnShowLeaderBoardsEvent += OnShowLeaderBoards;

            ToggleVisibility(GameManager.Instance.CurrentGameState);
        }
        
        private void OnDisable()
        {
            GameManager.Instance.OnEnterStateEvent -= ToggleVisibility;
            ScoreManager.Instance.OnShowLeaderBoardsEvent -= OnShowLeaderBoards;
        }

        private void ToggleVisibility(GameState newState)
        {
            SetVisibility(_visibleOnGameStates.Contains(newState));
        }
        
        private void OnShowLeaderBoards(bool show)
        {
            _leaderboardShown = show;
            ToggleVisibility(GameManager.Instance.CurrentGameState);
        }

        private void SetVisibility(bool show)
        {
            if (!_uiDocumentGenerator)
            {
                return;
            }
            
            if (show && !_leaderboardShown)
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