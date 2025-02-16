using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameElements.Nodes
{
    public class DestinationNode : Node
    {
        public override NodeType NodeType => NodeType.Destination;

        [ShowInInspector] 
        public NodeShape Shape { get; private set; }

        [ShowInInspector] 
        public NodeColor Color { get; private set; }
        
        [SerializeField]
        private SpriteRenderer _highlight;
        
        [SerializeField]
        private Color _highlightColor;
        private Color _defaultColor;

        [Tooltip("Documents per minute that this destination can receive")]
        public int DocumentsPerMinute = 15;

        private SpriteRenderer _spriteRenderer;

        private void OnEnable()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            
            _defaultColor = _highlight.color;
            Highlight(false);
        }

        public void Setup(NodeShape shape, NodeColor color, int waveIndex)
        {
            DocumentsPerMinute = 10 + waveIndex * 2;
                
            Shape = shape;
            Color = color;

            _spriteRenderer.sprite = NodeCollection.Instance.ShapeToSpriteDictionary[Shape];
            _spriteRenderer.color = NodeCollection.Instance.ColorToColorDictionary[Color];
            
            _highlight.sprite = _spriteRenderer.sprite;
        }

        public override bool CanConnect(List<Node> currentPath, Node startNode)
        {
            var alreadyConnected = startNode.ConnectedToNode(this);
            var anyColorMatch = currentPath.Exists(node => node.MatchColor(Color));
            var anyShapeMatch = currentPath.Exists(node => node.MatchShape(Shape));
            return !alreadyConnected && anyColorMatch && anyShapeMatch && base.CanConnect(currentPath, startNode);
        }
        
        public void Highlight(bool highlight)
        {
            _highlight.color = highlight ? _highlightColor : _defaultColor;
        }
    }
}