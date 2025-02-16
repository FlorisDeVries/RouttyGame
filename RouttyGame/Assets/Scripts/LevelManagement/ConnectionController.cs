using System.Collections.Generic;
using GameElements;
using GameElements.Nodes;
using UnityEngine;
using UserInput.Resources;

namespace LevelManagement
{
    public class ConnectionController : MonoBehaviour
    {
        public Connection ConnectionPrefab;
        private Connection _currentConnection;
        
        public static bool Held { get; private set; }
        
        [SerializeField]
        private LevelController _levelController;
        
        private readonly List<Node> _selectedNodes = new();
        private Node _startNode;
        private bool _isDrawing;

        private MouseData _mouseData;
        private GameplayInputActions _gameplayInputActions;

        private void OnEnable()
        {
            _gameplayInputActions = GameplayInputActions.Instance;
            _mouseData = MouseData.Instance;
            
            _gameplayInputActions.PrimaryInputAction.OnValueChanged += OnPrimaryInputAction;
        }

        private void OnDisable()
        {
            _gameplayInputActions.PrimaryInputAction.OnValueChanged -= OnPrimaryInputAction;
        }

        private void OnPrimaryInputAction(bool held)
        {
            Held = held;
            _mouseData.UpdateData();

            if (held)
            {
                StartDraw();
            }
            else
            {
                FinishDraw();
            }
        }

        private void Update()
        {
            if (!_isDrawing) return;
            if (!_currentConnection) return;
            
            _currentConnection.FollowMouse(_mouseData.WorldPosition);
            
            if (!_mouseData.CurrentlyHovering)
            {
                return;
            }
            
            if (!_mouseData.CurrentlyHovering.TryGetComponent<Node>(out var hitNode))
            {
                return;
            }
            
            if (_selectedNodes.Contains(hitNode))
            {
                return;
            }
            
            if (!hitNode.CanConnect(_selectedNodes, _startNode))
            {
                return;
            }
            
            _currentConnection.AddNode(hitNode.transform.position);
            _selectedNodes.Add(hitNode);
        }

        private void StartDraw()
        {
            if (!_mouseData.CurrentlyHovering)
            {
                return;
            }
            
            if (!_mouseData.CurrentlyHovering.TryGetComponent<Node>(out var node))
            {
                return;
            }
            
            
            // Allow starting from either a Source or Destination.
            if (node is not (SourceNode or DestinationNode)) return;
            
            _isDrawing = true;
            _selectedNodes.Clear();
            _selectedNodes.Add(node);
            _startNode = node;
                
            _startNode.HighlightConnections(HighlightType.Secondary);
                
            switch (_startNode)
            {
                case SourceNode sourceNode:
                    _levelController.HighlightMissingDestinations(sourceNode);
                    break;
                case DestinationNode destinationNode:
                    _levelController.HighlightMissingSources(destinationNode);
                    break;
            }
                
            // Spawn the connection line.
            _currentConnection = Instantiate(ConnectionPrefab, transform);
            _currentConnection.StartConnection(node.transform.position);
        }
        
        private void FinishDraw()
        {
            if (!_isDrawing)
                return;
            
            _isDrawing = false;
            _startNode.HighlightConnections(HighlightType.Disabled);
            _levelController.DisableAllNodeHighlights();
            
            if (_selectedNodes.Count > 1)
            {
                var endNode = _selectedNodes[^1];
                if (_startNode is SourceNode sourceNode && endNode.NodeType == NodeType.Destination)
                {
                    var destinationNode = endNode as DestinationNode;
                    var rate = destinationNode!.DocumentsPerMinute * sourceNode!.DocumentsPerMinute;

                    _currentConnection.FinishConnection(_selectedNodes, rate);

                    foreach (var selectedNode in _selectedNodes)
                    {
                        selectedNode.Connect(_currentConnection);
                    }
                    
                    _selectedNodes.Clear();
                    _currentConnection = null;
                    _levelController.RegisterConnection(sourceNode, destinationNode);
                }
                else if (_startNode is DestinationNode destinationNode && endNode.NodeType == NodeType.Source)
                {
                    sourceNode = endNode as SourceNode;
                    var rate = destinationNode!.DocumentsPerMinute * sourceNode!.DocumentsPerMinute;

                    _currentConnection.FinishConnection(_selectedNodes, rate, true);

                    foreach (var selectedNode in _selectedNodes)
                    {
                        selectedNode.Connect(_currentConnection);
                    }
                    
                    _selectedNodes.Clear();
                    _currentConnection = null;
                    // Note: We always register with source first and destination second.
                    _levelController.RegisterConnection(sourceNode, destinationNode);
                }
                else
                {
                    ClearLine();
                }
            }
            else
            {
                ClearLine();
            }
        }
    
        private void ClearLine()
        {
            _selectedNodes.Clear();
            Destroy(_currentConnection.gameObject);
            _currentConnection = null;
        }
    }
}