using System.Collections.Generic;
using _Common.Extensions;
using Scores.Resources;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using UserInterface.Buttons;
using UserInterface.Buttons.Resources;

namespace UserInterface.Generation
{
    public class GameOverMenu : UIDocumentGenerator
    {
        [Header("Visual Style")]
        [SerializeField]
        private FontAsset _fontAsset;
        [SerializeField]
        private string _title = "GameOver";

        [Header("Menu Options")]
        [SerializeField]
        private ButtonProperties _restartButton;
        [SerializeField]
        private ButtonProperties _mainMenuButton;

        protected override void Generate()
        {
            // Create elements
            var rootContainer = root.Create("root");
            var panel = rootContainer.Create("game-over-panel");
            var title = panel.Create<Label>("game-over-title");
            title.text = _title;
            
            var highScore = panel.Create<Label>("high-score-label");
            highScore.text = $"High Score: {ScoreManager.Instance.CurrentScore}";
            
            var nameInput = panel.Create<TextField>("name-input");
            nameInput.value = "Enter your name";
            
            var submitButton = panel.Create<Button>("button");
            submitButton.text = "Submit";
            
            void SubmitScore(ClickEvent evt)
            {
                // TODO: Submit score
            }
            submitButton.RegisterCallback<ClickEvent>(SubmitScore);
            createdButtons.Add(submitButton, SubmitScore);
            
            var restartButton = panel.Create<Button>("button");
            restartButton.text = _restartButton.Text;
            
            void RestartGame(ClickEvent evt) => _restartButton.EventObject.RaiseEvent(evt);
            restartButton.RegisterCallback<ClickEvent>(RestartGame);
            createdButtons.Add(restartButton, RestartGame);
            
            var mainMenuButton = panel.Create<Button>("button");
            mainMenuButton.text = _mainMenuButton.Text;
            
            void GoToMainMenu(ClickEvent evt) => _mainMenuButton.EventObject.RaiseEvent(evt);
            mainMenuButton.RegisterCallback<ClickEvent>(GoToMainMenu);
            createdButtons.Add(mainMenuButton, GoToMainMenu);
        }
    }
}