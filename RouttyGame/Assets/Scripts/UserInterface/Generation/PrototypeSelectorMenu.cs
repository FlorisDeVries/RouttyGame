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
        
        [Header("Menu Options")]
        [SerializeField]
        private SceneButtonProperties _prototype;
        
        protected override void Generate()
        {
            // Create a full-screen container with USS class "main-menu".
            var mainMenu = new VisualElement();
            mainMenu.name = "main-menu";
            mainMenu.AddToClassList("main-menu");
            mainMenu.style.width = Length.Percent(100);
            mainMenu.style.height = Length.Percent(100);
            mainMenu.RegisterCallback<ClickEvent>(evt => _prototype.RaiseEvent(evt));

            // Create and add the logo.
            var logo = new VisualElement();
            logo.name = "logo";
            logo.AddToClassList("logo");
            // Set the background image from the sprite.
            if (_routtyLogo)
                logo.style.backgroundImage = new StyleBackground(_routtyLogo.texture);
            
            mainMenu.Add(logo);
            
            // Create and add the subscript.
            var subscript = new Label("Click anywhere to start the game");
            subscript.name = "subscript";
            subscript.AddToClassList("subscript");
            if (_fontAsset != null)
                subscript.style.unityFontDefinition = new StyleFontDefinition(_fontAsset);
            
            mainMenu.Add(subscript);
            
            // Set the root element.
            root.Clear();
            root.Add(mainMenu);
        }
    }
}