using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameElements.Nodes
{
    public class ColorNode : Node
    {
        public override NodeType NodeType => NodeType.Color;

        public NodeColor Color;
        private SpriteRenderer _spriteRenderer;

        public override bool MatchColor(NodeColor color)
        {
            return Color == color;
        }

        private void OnEnable()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        
        public void Setup(NodeColor color)
        {
            Color = color;
            _spriteRenderer.color = NodeCollection.Instance.ColorToColorDictionary[Color];
        }
        
        public override bool CanConnect(List<Node> currentPath, Node previousNode)
        {
            var routtyFound = currentPath.Exists(node => node.NodeType == NodeType.Routty);
            return !routtyFound && base.CanConnect(currentPath, previousNode);
        }
    }

    public enum NodeColor
    {
        Red,
        Green,
        Blue
    }
}