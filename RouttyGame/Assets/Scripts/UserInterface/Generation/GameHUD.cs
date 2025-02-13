using System;
using _Common.Extensions;
using LevelManagement;
using Scores.Resources;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;

namespace UserInterface.Generation
{
    public class GameHUD : UIDocumentGenerator
    {
        [Header("Visual Style")]
        [SerializeField]
        private FontAsset _fontAsset;
        
        private Label _scoreLabel;
        
        private Label _extraLabel;
        private Label _extraLabel2;
        private Label _extraLabel3;

        protected override void Generate()
        {
            // Create a full-screen container with USS class "game-hud".
            var rootContainer = root.Create("root", false);
            var hud = rootContainer.Create("hud", false);

            // Create and add the score label.
            _scoreLabel = hud.Create<Label>("score-label");
            if (_fontAsset != null)
                _scoreLabel.style.unityFontDefinition = new StyleFontDefinition(_fontAsset);
            _scoreLabel.text = "0";
            
            _extraLabel = hud.Create<Label>("score-label");
            if (_fontAsset != null)
                _extraLabel.style.unityFontDefinition = new StyleFontDefinition(_fontAsset);
            _extraLabel.text = "0";
            
            _extraLabel2 = hud.Create<Label>("score-label");
            if (_fontAsset != null)
                _extraLabel2.style.unityFontDefinition = new StyleFontDefinition(_fontAsset);
            _extraLabel2.text = "0";
            
            _extraLabel3 = hud.Create<Label>("score-label");
            if (_fontAsset != null)
                _extraLabel3.style.unityFontDefinition = new StyleFontDefinition(_fontAsset);
            _extraLabel3.text = "0";
        }

        private void FixedUpdate()
        {
            // Update the score label.
            _scoreLabel.text = $"{ScoreManager.Instance.CurrentScore}";
            _extraLabel.text = mouseData.CurrentlyHovering.name;
            _extraLabel2.text = ConnectionController.PrimaryInputAction.ToString();
            _extraLabel3.text = ConnectionController.Held.ToString();
        }
    }
}