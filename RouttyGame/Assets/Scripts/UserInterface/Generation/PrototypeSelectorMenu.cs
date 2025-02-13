using System.Collections.Generic;
using _Common.Extensions;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using UserInterface.Buttons.Resources;

namespace UserInterface.Generation
{
    public class PrototypeSelectorMenu : UIDocumentGenerator
    {
        [Header("Visual Style")]
        [SerializeField]
        private FontAsset _fontAsset;

        [SerializeField]
        private Sprite _routtyLogo;
        [SerializeField]
        private Sprite _routtyBackground;
        
        [Header("Menu Options")]
        [SerializeField]
        private SceneButtonProperties _prototype;
        
        protected override void Generate()
        {
            // Create a full-screen container with USS class "main-menu".
            var mainMenu = root.Create("main-menu");
            mainMenu.style.backgroundImage = new StyleBackground(_routtyBackground.texture);
            mainMenu.RegisterCallback<ClickEvent>(evt => _prototype.RaiseEvent(evt));
            
            // Create and add the subscript.
            var subscript = mainMenu.Create<Label>("subscript");
            subscript.text = "Click anywhere to start the game";
            if (_fontAsset != null)
                subscript.style.unityFontDefinition = new StyleFontDefinition(_fontAsset);
        }
    }
}