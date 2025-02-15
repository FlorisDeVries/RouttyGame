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

        [Tooltip("Documents per minute that this destination can receive")]
        public int DocumentsPerMinute = 15;

        private SpriteRenderer _spriteRenderer;

        private void OnEnable()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        public void Setup(NodeShape shape, NodeColor color)
        {
            Shape = shape;
            Color = color;

            _spriteRenderer.sprite = NodeCollection.Instance.ShapeToSpriteDictionary[Shape];
            _spriteRenderer.color = NodeCollection.Instance.ColorToColorDictionary[Color];
        }

        public override bool CanConnect(List<Node> currentPath, Node previousNode)
        {
            var anyColorMatch = currentPath.Exists(node => node.MatchColor(Color));
            var anyShapeMatch = currentPath.Exists(node => node.MatchShape(Shape));
            return anyColorMatch && anyShapeMatch && base.CanConnect(currentPath, previousNode);
        }
    }
}