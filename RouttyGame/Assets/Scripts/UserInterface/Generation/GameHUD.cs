using System;
using _Common.Events.Resources;
using _Common.Extensions;
using GameManagement;
using GameManagement.Resources;
using LevelManagement;
using Scores.Resources;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using UserInterface.Resources;

namespace UserInterface.Generation
{
    public class GameHUD : UIDocumentGenerator
    {
        [Header("Visual Style")]
        [SerializeField]
        private FontAsset _fontAsset;

        [Header("Pause Button Sprite")]
        [SerializeField]
        private Sprite _pauseIcon;

        [SerializeField]
        private ButtonProperties _buyButtonProperties;

        [SerializeField]
        private Transform _scoreDisplay;

        private Label _scoreLabel;
        private bool _isPurchased;

        protected override void Generate()
        {
            // Create a full-screen container with USS class "game-hud".
            _isPurchased = LevelController.RouttyIsPurchased;
            var rootContainer = root.Create("root", false);

            var headerContainer = rootContainer.Create("header-container", false);
            var headerErp = headerContainer.Create<Label>("header", false);
            headerContainer.Create("vertical-line");

            var headerFormat = headerContainer.Create<Label>("header-large", false);
            // headerContainer.Create("vertical-line");
            headerContainer.Create("vertical-line");

            var headerChannel = headerContainer.Create<Label>("header", false);

            headerErp.text = "ERP";
            headerFormat.text = "Format";
            headerChannel.text = "Channel";

            // Pause Button
            var pauseButton = rootContainer.Create<Button>("pause-button");

            var pauseIcon = pauseButton.Create("pause-icon");
            pauseIcon.style.backgroundImage = new StyleBackground(_pauseIcon.texture);

            // Pause Button Logic
            void PauseButtonCallback(ClickEvent evt) => GameManager.Instance.ChangeState(GameState.Paused);
            pauseButton.RegisterCallback<ClickEvent>(PauseButtonCallback);
            createdButtons.Add(pauseButton, PauseButtonCallback);

            // Create and add the score label.
            var hud = rootContainer.Create("hud", false);

            if (!LevelController.RouttyIsPurchased)
            {
                var buyButton = hud.Create<Button>("buy-button");
                buyButton.text = _buyButtonProperties.Text;
                buyButton.style.alignSelf = Align.Center;

                void BuyButtonCallback(ClickEvent evt)
                {
                    _buyButtonProperties.EventObject.RaiseEvent(evt);
                    buyButton.style.display = DisplayStyle.None;
                }

                buyButton.RegisterCallback<ClickEvent>(BuyButtonCallback);
                createdButtons.Add(buyButton, BuyButtonCallback);
            }

            _scoreLabel = hud.Create<Label>("score-label");
            _scoreLabel.text = "0";

            if (!_fontAsset)
                return;

            _scoreLabel.style.unityFontDefinition = new StyleFontDefinition(_fontAsset);
            headerErp.style.unityFontDefinition = new StyleFontDefinition(_fontAsset);
            headerFormat.style.unityFontDefinition = new StyleFontDefinition(_fontAsset);
            headerChannel.style.unityFontDefinition = new StyleFontDefinition(_fontAsset);
            pauseButton.style.unityFontDefinition = new StyleFontDefinition(_fontAsset);
        }

        private void FixedUpdate()
        {
            _scoreLabel.text = $"{ScoreManager.Instance.CurrentScore}";

            if (LevelController.RouttyIsPurchased != _isPurchased)
            {
                Clear();
                Generate();
            }

            if (_scoreLabel == null || _scoreDisplay == null || GameManager.Instance.MainCamera == null)
                return;

            try
            {
                var scoreRect = _scoreLabel.worldBound;
                var screenPos = new Vector2(scoreRect.center.x, scoreRect.yMax);
                screenPos.y -= 25;

                // Make sure screenPos is a real value
                if (float.IsNaN(screenPos.x) || float.IsNaN(screenPos.y))
                    return;

                var correctedScreenPos = PanelSettingsManager.Instance.ScaleToScreenSpace(screenPos);
                var worldPos =
                    PanelSettingsManager.Instance.ScreenPositionToWorld(correctedScreenPos,
                        GameManager.Instance.MainCamera);
                _scoreDisplay.position = worldPos;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}