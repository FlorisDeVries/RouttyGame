using _Common.Events.Resources;
using _Common.Extensions;
using Scores.Resources;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

namespace UserInterface.Generation
{
    public class ScoreMenu : UIDocumentGenerator
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

        private bool _scoreSubmitted;

        private async void Start()
        {
            await UnityServices.InitializeAsync();
            _scoreSubmitted = false;
        }

        protected override void Generate()
        {
            // Create elements
            var rootContainer = root.Create("root", false);
            var panel = rootContainer.Create("menu-container");
            var title = panel.Create<Label>("title-label");
            if (_fontAsset) title.style.unityFontDefinition = new StyleFontDefinition(_fontAsset);
            title.text = _title;

            var highScore = panel.Create<Label>("high-score-label");
            highScore.text = $"High Score: {ScoreManager.Instance.CurrentScore}";

            var nameInput = panel.Create<TextField>("name-input");
            nameInput.value = ScoreManager.Instance.PlayerName;

            // Submit button
            var submitButton = panel.Create<Button>("button");
            submitButton.text = "Submit";
            if (_scoreSubmitted) submitButton.SetEnabled(false);


            void SubmitScore(ClickEvent evt)
            {
                if (string.IsNullOrEmpty(nameInput.value) || nameInput.value == "Enter your name")
                {
                    Debug.Log("Please enter a name!");
                    return;
                }

                var playerName = nameInput.value;
                var score = ScoreManager.Instance.CurrentScore;
                UploadScore(playerName, score);
            }

            submitButton.RegisterCallback<ClickEvent>(SubmitScore);
            createdButtons.Add(submitButton, SubmitScore);
            
            // Leaderboard button
            var leaderboardButton = panel.Create<Button>("button");
            leaderboardButton.text = "Leaderboard";
            
            void ShowLeaderboard(ClickEvent evt) => ScoreManager.Instance.ShowLeaderBoards(true);
            leaderboardButton.RegisterCallback<ClickEvent>(ShowLeaderboard);
            createdButtons.Add(leaderboardButton, ShowLeaderboard);

            // Restart button
            var restartButton = panel.Create<Button>("button");
            restartButton.text = _restartButton.Text;

            void RestartGame(ClickEvent evt) => _restartButton.EventObject.RaiseEvent(evt);
            restartButton.RegisterCallback<ClickEvent>(RestartGame);
            createdButtons.Add(restartButton, RestartGame);

            // Main menu button
            var mainMenuButton = panel.Create<Button>("button");
            mainMenuButton.text = _mainMenuButton.Text;

            void GoToMainMenu(ClickEvent evt) => _mainMenuButton.EventObject.RaiseEvent(evt);
            mainMenuButton.RegisterCallback<ClickEvent>(GoToMainMenu);
            createdButtons.Add(mainMenuButton, GoToMainMenu);
        }

        public async void UploadScore(string playerName, int score)
        {
            const string leaderboardId = "Highscore";
            if (playerName != ScoreManager.Instance.PlayerName)
            {
                await AuthenticationService.Instance.UpdatePlayerNameAsync(playerName);
                ScoreManager.Instance.UpdatePlayerName();
            }

            await LeaderboardsService.Instance.AddPlayerScoreAsync(leaderboardId, score);
            ScoreManager.Instance.FetchLeaderboard();
            _scoreSubmitted = true;
            
            Clear();
            Generate();
        }
    }
}