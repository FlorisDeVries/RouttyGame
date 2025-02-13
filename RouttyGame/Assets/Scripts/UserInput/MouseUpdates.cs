using _Common.Math;
using GameManagement.Resources;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UserInput.Resources;

namespace UserInput
{
    public class MouseUpdates : MonoBehaviour
    {
        [SerializeField]
        private LayerMask _interactionLayers;

        private MouseData _mouseData;
        private GameManager _gameManager;
        private Vector2 _screenPos;

        [ShowInInspector, ReadOnly]
        private GameObject _currentlyHovering;

        private void OnEnable()
        {
            _mouseData = MouseData.Instance;
            _gameManager = GameManager.Instance;
        }

        private void FixedUpdate()
        {
            if (!_gameManager.MainCamera)
                return;

            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            {
                _screenPos = Touchscreen.current.primaryTouch.position.ReadValue();
            }
            else if (Mouse.current != null)
            {
                _screenPos = Mouse.current.position.ReadValue();
            }
            else
            {
                return;
            }

            var worldPosition = _gameManager.MainCamera
                .ScreenToWorldPoint(_screenPos);

            transform.position = worldPosition;
            _mouseData.SetPosition(worldPosition);
            _mouseData.SetMouseScreenPosition(_screenPos);
            _currentlyHovering =
                MouseMath.GameObjectUnderMouse2D(_gameManager.MainCamera, _screenPos, _interactionLayers);
            _mouseData.SetCurrentlyHovering(_currentlyHovering);
        }
    }
}