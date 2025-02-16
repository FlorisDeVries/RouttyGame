using System.Collections.Generic;
using UnityEngine;

namespace GameElements.Nodes
{
    public class SourceNode : Node
    {
        public override NodeType NodeType => NodeType.Source;

        public int DocumentsPerMinute = 10;
        
        [SerializeField]
        private SpriteRenderer _highlight;
        
        [SerializeField]
        private Color _highlightColor;
        private Color _defaultColor;

        public override bool CanConnect(List<Node> currentPath, Node startNode)
        {
            var destinationNode = startNode as DestinationNode;
            if (!destinationNode) return false;
            
            var alreadyConnected = startNode.ConnectedToNode(this);
            var anyColorMatch = currentPath.Exists(node => node.MatchColor(destinationNode.Color));
            var anyShapeMatch = currentPath.Exists(node => node.MatchShape(destinationNode.Shape));
            return !alreadyConnected && anyColorMatch && anyShapeMatch && base.CanConnect(currentPath, startNode);
        }

        private void OnEnable()
        {
            _defaultColor = _highlight.color;
            Highlight(false);
        }
        
        public void Highlight(bool highlight)
        {
            _highlight.color = highlight ? _highlightColor : _defaultColor;
        }
    }
}