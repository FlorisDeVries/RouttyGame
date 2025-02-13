using System.Collections.Generic;
using GameElements;
using GameElements.Nodes;
using Scores.Resources;
using UnityEngine;
using UserInput.Resources;

namespace LevelManagement
{
    public class ConnectionController : MonoBehaviour
    {
        public Connection ConnectionPrefab;
        private Connection _currentConnection;
        
        public static bool Held { get; private set; }
        public static int PrimaryInputAction { get; private set; }
        
        [SerializeField]
        private LevelController _levelController;
        
        private readonly List<Node> _selectedNodes = new();
        private SourceNode _sourceNode = new();
        private bool _isDrawing;

        private MouseData _mouseData;
        private GameplayInputActions _gameplayInputActions;

        private void OnEnable()
        {
            _gameplayInputActions = GameplayInputActions.Instance;
            _mouseData = MouseData.Instance;
            
            _gameplayInputActions.PrimaryInputAction.OnValueChanged += OnPrimaryInputAction;
            PrimaryInputAction = 0;
        }

        private void OnDisable()
        {
            _gameplayInputActions.PrimaryInputAction.OnValueChanged -= OnPrimaryInputAction;
        }

        private void OnPrimaryInputAction(bool held)
        {
            PrimaryInputAction++;
            Held = held;

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
            
            if (!hitNode.CanConnect(_selectedNodes, _selectedNodes[^1]))
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
            
            _sourceNode = node as SourceNode;
            if (!_sourceNode)
            {
                return;
            }
            
            _isDrawing = true;
            _selectedNodes.Clear();
            _selectedNodes.Add(node);
            _sourceNode.HighlightConnections(HighlightType.Secondary);
            
            // Spawn a line renderer
            _currentConnection = Instantiate(ConnectionPrefab, transform);
            _currentConnection.StartConnection(node.transform.position);
        }
        
        private void FinishDraw()
        {
            if (!_isDrawing)
                return;
            
            _isDrawing = false;
            _sourceNode.HighlightConnections(HighlightType.Disabled);
            
            if (_selectedNodes.Count > 1)
            {
                var endNode = _selectedNodes[^1];
                if (endNode.NodeType == NodeType.Destination)
                {
                    var sourceNode = _selectedNodes[0] as SourceNode;
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