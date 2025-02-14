using System.Collections.Generic;
using System.Linq;
using _Common.BaseClasses;
using _Common.Math;
using GameManagement.Resources;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

namespace UserInput.Resources
{
    [CreateAssetMenu(fileName = "MouseData", menuName = "Project/Input/MouseData")]
    public class MouseData : ASingletonResource<MouseData>
    {
        protected override string ResourcePath => "Input/MouseData";

        [SerializeField]
        private LayerMask _interactionLayers;

        public Vector3 WorldPosition { get; private set; }
        public Vector2 ScreenPosition { get; private set; }
        public Vector2 ScreenPositionYInverted { get; private set; }

        [ShowInInspector] [ReadOnly] public GameObject CurrentlyHovering { get; private set; }

        private Dictionary<UIDocument, bool> _overUIStates = new();

        [ShowInInspector] [ReadOnly] public bool CursorOverUI => _overUIStates?.Any(x => x.Value) ?? false;

        public void SetCursorOverUIState(UIDocument document, bool overUI)
        {
            _overUIStates ??= new Dictionary<UIDocument, bool>();
            _overUIStates.TryAdd(document, overUI);
            _overUIStates[document] = overUI;
        }

        public void UpdateData()
        {
            var gameManager = GameManager.Instance;
            if (!gameManager.MainCamera)
                return;

            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            {
                ScreenPosition = Touchscreen.current.position.ReadValue();
            }
            else if (Mouse.current != null)
            {
                ScreenPosition = Mouse.current.position.ReadValue();
            }
            else
            {
                return;
            }

            WorldPosition = gameManager.MainCamera.ScreenToWorldPoint(ScreenPosition);
            CurrentlyHovering =
                MouseMath.GameObjectUnderMouse2D(gameManager.MainCamera, ScreenPosition, _interactionLayers);
        }
    }
}