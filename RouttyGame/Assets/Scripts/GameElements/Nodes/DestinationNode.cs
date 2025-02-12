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
        private NodeShape _shape;
        
        [ShowInInspector]
        private NodeColor _color;

        [Tooltip("Documents per minute that this destination can receive")]
        public int DocumentsPerMinute = 15;

        private SpriteRenderer _spriteRenderer;

        private void OnEnable()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        public void Setup(NodeShape shape, NodeColor color)
        {
            _shape = shape;
            _color = color;
            
            _spriteRenderer.sprite = NodeCollection.Instance.ShapeToSpriteDictionary[_shape];
            _spriteRenderer.color = NodeCollection.Instance.ColorToColorDictionary[_color];
        }

        public override bool CanConnect(List<Node> currentPath, Node previousNode)
        {
            var anyColorMatch = currentPath.Exists(node => node.MatchColor(_color));
            var anyShapeMatch = currentPath.Exists(node => node.MatchShape(_shape));
            return anyColorMatch && anyShapeMatch && base.CanConnect(currentPath, previousNode);
        }
    }
}