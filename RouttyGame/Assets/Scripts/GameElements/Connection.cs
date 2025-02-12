using System.Collections;
using System.Collections.Generic;
using _Common.Extensions;
using GameElements.Nodes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameElements
{
    public class Connection : MonoBehaviour
    {
        private LineRenderer _lineRenderer;
        private List<ConnectionPoint> _connectionPoints = new();

        [Header("Document Transport Settings")]
        [Tooltip("Document prefab that will travel along the connection.")]
        public Document documentPrefab;

        [Tooltip("Calculated documents per minute for this connection (set during completion).")]
        [ReadOnly]
        public int DocumentsPerMinute = 0;
        
        public Color DefaultColor = Color.white;
        public Color PrimaryHighlightColor = Color.green;
        public Color SecondaryHighlightColor = Color.green;

        private Coroutine _documentTransportRoutine;
        private Vector3 _mousePosition;

        private void OnEnable()
        {
            _lineRenderer = GetComponent<LineRenderer>();
        }

        private void OnDisable()
        {
            if (_documentTransportRoutine != null)
            {
                StopCoroutine(_documentTransportRoutine);
            }
        }

        public void StartConnection(Vector3 position)
        {
            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPosition(0, position.FlattenOnZ());
            Highlight(HighlightType.Primary);
        }
        
        public void AddNode(Vector3 position)
        {
            _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, position.FlattenOnZ());
            _lineRenderer.positionCount++;
            _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, _mousePosition.FlattenOnZ());
        }

        public void FinishConnection(List<Node> path, int documentsPerMinute)
        {  
            DocumentsPerMinute = documentsPerMinute;
            
            _lineRenderer.positionCount--;
            _connectionPoints.Clear();
            foreach (var node in path)
            {
                _connectionPoints.Add(new ConnectionPoint(node.transform.position.FlattenOnZ(), node));
            }
            
            Highlight(HighlightType.Disabled);
            StartDocumentTransport();
        }

        public void FollowMouse(Vector3 mousePosition)
        {
            if (_lineRenderer.positionCount == 0)
                return;

            _mousePosition = mousePosition;
            _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, _mousePosition.FlattenOnZ());
        }
        
        private void StartDocumentTransport()
        {
            // Stop any previous routine.
            if (_documentTransportRoutine != null)
            {
                StopCoroutine(_documentTransportRoutine);
            }
            _documentTransportRoutine = StartCoroutine(DocumentTransportCoroutine());
        }
        
        public void Highlight(HighlightType highlightType)
        {
            switch (highlightType)
            {
                case HighlightType.Disabled:
                    _lineRenderer.startColor = DefaultColor;
                    _lineRenderer.endColor = DefaultColor;
                    break;
                case HighlightType.Primary:
                    _lineRenderer.startColor = PrimaryHighlightColor;
                    _lineRenderer.endColor = PrimaryHighlightColor;
                    break;
                case HighlightType.Secondary:
                    _lineRenderer.startColor = SecondaryHighlightColor;
                    _lineRenderer.endColor = SecondaryHighlightColor;
                    break;
            }
        }
        
        private IEnumerator DocumentTransportCoroutine()
        {
            var interval = 60f / DocumentsPerMinute;
            while (true)
            {
                SpawnDocument();
                yield return new WaitForSeconds(interval);
            }
            // ReSharper disable once IteratorNeverReturns
        }
        
        private void SpawnDocument()
        {
            var doc = Instantiate(documentPrefab, _connectionPoints[0].Position, Quaternion.identity, transform);
            doc.SetPath(_connectionPoints);
        }
    }
    
    public enum HighlightType
    {
        Disabled,
        Primary,
        Secondary
    }
}