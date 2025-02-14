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
        private MouseData _mouseData;

        private void OnEnable()
        {
            _mouseData = MouseData.Instance;
        }

        private void FixedUpdate()
        {
            _mouseData.UpdateData();
            transform.position = _mouseData.WorldPosition;
        }
    }
}