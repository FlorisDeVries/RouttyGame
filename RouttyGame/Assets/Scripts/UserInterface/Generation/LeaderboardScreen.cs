using _Common.Extensions;
using Scores.Resources;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using UserInterface.Buttons.Resources;

namespace UserInterface.Generation
{
    public class LeaderboardScreen : UIDocumentGenerator
    {
        [Header("Visual Style")]
        [SerializeField]
        private FontAsset _fontAsset;

        [SerializeField]
        private string _title = "Leaderboard";

        [FormerlySerializedAs("_mainMenuButton")]
        [Header("Menu Options")]
        [SerializeField]
        private ButtonProperties _returnButton;

        private async void Start()
        {
            await UnityServices.InitializeAsync();
        }

        protected override void Generate()
        {
            // Root container
            var rootContainer = root.Create("leaderboard-root", false);
            var panel = rootContainer.Create("leaderboard-panel");

            // Title
            var title = panel.Create<Label>("title-label");
            if (_fontAsset != null)
            {
                title.style.unityFontDefinition = new StyleFontDefinition(_fontAsset);
            }
            title.text = _title;

            // Leaderboard Table
            var table = panel.Create("leaderboard-table");

            // Column Titles
            var headerRow = table.Create("leaderboard-row");

            var rankHeader = headerRow.Create<Label>("rank-label");
            rankHeader.text = "Rank";
            if (_fontAsset != null)
            {
                rankHeader.style.unityFontDefinition = new StyleFontDefinition(_fontAsset);
            }

            var nameHeader = headerRow.Create<Label>("name-label");
            nameHeader.text = "Player";
            if (_fontAsset != null)
            {
                nameHeader.style.unityFontDefinition = new StyleFontDefinition(_fontAsset);
            }

            var scoreHeader = headerRow.Create<Label>("score-label");
            scoreHeader.text = "Score";
            if (_fontAsset != null)
            {
                scoreHeader.style.unityFontDefinition = new StyleFontDefinition(_fontAsset);
            }

            foreach (var entry in ScoreManager.Instance.LeaderboardEntries)
            {
                var row = table.Create("leaderboard-row");

                var rankLabel = row.Create<Label>("rank-label");
                rankLabel.text = entry.Rank.ToString();

                var nameLabel = row.Create<Label>("name-label");
                nameLabel.text = entry.PlayerName;

                var scoreLabel = row.Create<Label>("score-label");
                scoreLabel.text = entry.Score.ToString();
            }

            // Back to Main Menu Button
            var returnButton = panel.Create<Button>("button");
            returnButton.text = _returnButton.Text;
            void Return(ClickEvent evt) => _returnButton.EventObject.RaiseEvent(evt);
            returnButton.RegisterCallback<ClickEvent>(Return);
            createdButtons.Add(returnButton, Return); 
        }

        public override void Show()
        {
            ScoreManager.Instance.FetchLeaderboard();
            base.Show();
        }
    }
}