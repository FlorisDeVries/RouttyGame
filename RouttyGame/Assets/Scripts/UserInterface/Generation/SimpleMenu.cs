using System.Collections.Generic;
using _Common.Events.Resources;
using _Common.Extensions;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UIElements;
using UserInterface.Buttons;
using UserInterface.Buttons.Resources;

namespace UserInterface.Generation
{
    public class SimpleMenu : UIDocumentGenerator
    {
        [Header("Visual Style")]
        [SerializeField]
        private FontAsset _fontAsset;
        [SerializeField]
        private string _title = "PAUSED";

        [Header("Menu Options")]
        [SerializeField]
        private List<ButtonProperties> _menuButtons = new();
        
        protected override void Generate()
        {
            // Create elements
            var rootContainer = root.Create("root", false);
            var menuContainer = rootContainer.Create("menu-container");
            var title = menuContainer.Create<Label>("title-label");
            if (_fontAsset) title.style.unityFontDefinition = new StyleFontDefinition(_fontAsset);
            title.text = _title;
            
            foreach (var buttonType in _menuButtons)
            {
                CreateButton(menuContainer, buttonType);
            }
        }

        private void CreateButton(VisualElement container, ButtonProperties buttonProperties)
        {
            var button = container.Create<Button>("button");

            button.text = buttonProperties.Text;
            if (_fontAsset) button.style.unityFontDefinition = new StyleFontDefinition(_fontAsset);

            void EventCallBack(ClickEvent evt) => buttonProperties.EventObject.RaiseEvent(evt);
            button.RegisterCallback<ClickEvent>(EventCallBack);

            createdButtons.Add(button, EventCallBack);
        }
    }
}