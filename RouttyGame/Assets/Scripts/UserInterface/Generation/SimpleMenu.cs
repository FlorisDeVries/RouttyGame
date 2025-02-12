using System.Collections.Generic;
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
        
        private Dictionary<Button, EventCallback<ClickEvent>> _createdButtons = new();

        protected override void Generate()
        {
            // Create elements
            ResetEvents();

            var rootContainer = root.Create("root");
            var backdrop = rootContainer.Create("backdrop");
            var menuContainer = backdrop.Create("menu-container");
            var titleContainer = menuContainer.Create("title-container");
            
            var title = titleContainer.Create<Label>("title-label");
            if (_fontAsset != null)
            {
                title.style.unityFontDefinition = new StyleFontDefinition(_fontAsset);
            }
            title.text = _title;
            
            foreach (var buttonType in _menuButtons)
            {
                CreateButton(menuContainer, buttonType);
            }
        }

        private void CreateButton(VisualElement container, ButtonProperties buttonProperties)
        {
            var buttonContainer = container.Create("button-container");
            var button = buttonContainer.Create<Button>("button");

            button.text = buttonProperties.Text;
            if (_fontAsset != null)
            {
                button.style.unityFontDefinition = new StyleFontDefinition(_fontAsset);
            }

            void EventCallBack(ClickEvent evt) => buttonProperties.EventObject.RaiseEvent(evt);
            button.RegisterCallback<ClickEvent>(EventCallBack);

            _createdButtons.Add(button, EventCallBack);
        }

        private void OnDisable()
        {
            ResetEvents();
        }

        private void ResetEvents()
        {
            foreach (var (button, eventCallback) in _createdButtons)
            {
                button.UnregisterCallback(eventCallback);
            }

            _createdButtons.Clear();
        }
    }
}