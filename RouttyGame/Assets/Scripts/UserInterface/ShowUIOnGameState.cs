using System.Collections.Generic;
using GameManagement;
using GameManagement.Resources;
using UnityEngine;
using UnityEngine.UIElements;

namespace UserInterface
{
    public class ShowUIOnGameState : MonoBehaviour
    {
        [SerializeField]
        private List<GameState> _visibleOnGameStates = new();

        private UIDocument _uiDocument;
        private VisualElement _root;

        private void OnEnable()
        {
            _uiDocument = GetComponent<UIDocument>();
            _root = _uiDocument.rootVisualElement;

            GameManager.Instance.OnEnterStateEvent += ToggleVisibility;

            ToggleVisibility(GameManager.Instance.CurrentGameState);
        }
        
        private void OnDisable()
        {
            GameManager.Instance.OnEnterStateEvent -= ToggleVisibility;
        }

        private void ToggleVisibility(GameState newState)
        {
            SetVisibility(_visibleOnGameStates.Contains(newState));
        }

        private void SetVisibility(bool show)
        {
            _root.style.display = show ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}