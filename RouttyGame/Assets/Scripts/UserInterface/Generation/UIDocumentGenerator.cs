using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using UserInput.Resources;

namespace UserInterface.Generation
{
    [RequireComponent(typeof(UIDocument))]
    public abstract class UIDocumentGenerator : MonoBehaviour
    {
        [SerializeField]
        private bool _showInEditor;

        [SerializeField]
        private List<StyleSheet> _styleSheets = new();

        protected UIDocument document;
        protected VisualElement root;
        protected MouseData mouseData;
        private bool _hidden;

        protected Dictionary<Button, EventCallback<ClickEvent>> createdButtons = new();

        private void OnEnable()
        {
            Clear();

            if (root != null)
                Generate();

            mouseData = MouseData.Instance;
        }

        private void OnDisable()
        {
            ResetEvents();
            mouseData.SetCursorOverUIState(document, false);
        }

        private void ResetEvents()
        {
            if (createdButtons == null)
            {
                return;
            }

            foreach (var (button, eventCallback) in createdButtons)
            {
                button.UnregisterCallback(eventCallback);
            }

            createdButtons.Clear();
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
                return;

            createdButtons ??= new Dictionary<Button, EventCallback<ClickEvent>>();
            Clear();

            if (!_showInEditor)
                return;

            if (root != null)
                Generate();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        protected void Clear()
        {
            if (document == null) document = GetComponent<UIDocument>();

            root = document.rootVisualElement;

            if (root == null) return;

            root.Clear();

            foreach (var styleSheet in _styleSheets)
            {
                root.styleSheets.Add(styleSheet);
            }

            ResetEvents();
        }

        protected abstract void Generate();

        protected virtual void Update()
        {
            if (!document)
                return;

            if (root.panel == null ||
                root.style.display == DisplayStyle.None)
            {
                mouseData.SetCursorOverUIState(document, false);
                return;
            }

            var top = root.panel.Pick(mouseData.ScreenPositionYInverted);
            mouseData.SetCursorOverUIState(document, top != null && !top.name.Contains("hidden"));
        }

        public virtual void Show()
        {
            if (!_hidden)
                return;

            // Not yet initialized
            if (root == null)
                return;
            
            _hidden = false;
            root.style.display = DisplayStyle.Flex;
            Clear();
            Generate();
        }

        public virtual void Hide()
        {
            if (_hidden)
                return;

            // Not yet initialized
            if (root == null)
                return;

            _hidden = true;
            root.style.display = DisplayStyle.None;
        }
    }
}