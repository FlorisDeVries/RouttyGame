using System;
using _Common.Events.Resources;
using GameManagement.Resources;
using UnityEngine;

namespace UserInterface
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField]
        private FloatingText floatingTextPrefab;

        [SerializeField]
        private Canvas uiCanvas;

        [SerializeField]
        private ScoreEventObject _scoreEventObject;
        
        [SerializeField]
        private Color _positiveColor = Color.green;
        
        [SerializeField]
        private Color _negativeColor = Color.red;

        private void OnEnable()
        {
            _scoreEventObject.RegisterListener(OnScoreEvent);
        }

        private void OnDisable()
        {
            _scoreEventObject.UnregisterListener(OnScoreEvent);
        }

        private void OnScoreEvent(ScoreEvent scoreEvent)
        {
            var worldPosition = scoreEvent.Position ?? transform.position;
            var screenPosition = GameManager.Instance.MainCamera.WorldToScreenPoint(worldPosition);
            DisplayFloatingScore(scoreEvent.Score, screenPosition);
        }

        public void DisplayFloatingScore(int amount, Vector3 screenPosition)
        {
            var instance = Instantiate(floatingTextPrefab, uiCanvas.transform);
    
            var canvasRect = uiCanvas.transform as RectTransform;
            var instanceRect = instance.GetComponent<RectTransform>();

            var uiCamera = uiCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : GameManager.Instance.MainCamera;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect,
                screenPosition,
                uiCamera,
                out var localPoint
            );
    
            instanceRect.localPosition = localPoint;
    
            if (amount > 0)
            {
                instance.Setup("+" + amount, _positiveColor, 24);
            }
            else
            {
                instance.Setup(amount.ToString(), _negativeColor, 30);
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(transform.position, 0.5f);
        }
    }
}