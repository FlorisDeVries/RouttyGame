using UnityEngine;
using UserInput.Resources;
using Cinemachine;

namespace Cameras
{
    public class CameraZoom : MonoBehaviour
    {
        [Header("Zoom Settings")]
        [SerializeField]
        private float _lerpMultiplier = 5f;

        [SerializeField]
        private float _zoomMultiplier = 2f;

        [SerializeField]
        private Vector2 _minMaxCameraDistance = new(10, 20);

        private float _targetDistance;
        private CinemachineVirtualCamera _virtualCamera;

        private void OnEnable()
        {
            _virtualCamera = GetComponent<CinemachineVirtualCamera>();
            _targetDistance = _virtualCamera.m_Lens.OrthographicSize;
            
            GameplayInputActions.Instance.ZoomAction.OnValueChanged += Zoom;
        }

        private void OnDisable()
        {
            GameplayInputActions.Instance.ZoomAction.OnValueChanged -= Zoom;
        }

        private void Zoom(float zoomValue)
        {
            _targetDistance -= Mathf.Clamp(zoomValue, -1, 1) * _zoomMultiplier;
            _targetDistance = Mathf.Clamp(_targetDistance, _minMaxCameraDistance.x, _minMaxCameraDistance.y);
        }

        private void Update()
        {
            _virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(_virtualCamera.m_Lens.OrthographicSize, _targetDistance,
                _lerpMultiplier * Time.deltaTime);
        }
    }
}