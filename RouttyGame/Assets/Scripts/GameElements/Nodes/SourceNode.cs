using System;
using UnityEngine;

namespace GameElements.Nodes
{
    public class SourceNode : Node
    {
        public override NodeType NodeType => NodeType.Source;

        public int DocumentsPerMinute = 10;
        
        [SerializeField]
        private SpriteRenderer _highlight;

        private void OnEnable()
        {
            Highlight(false);
        }

        public void HighlightConnections(HighlightType highlightType)
        {
            if (Connections == null) return;

            foreach (var connection in Connections.Keys)
            {
                connection.Highlight(highlightType);
            }
        }
        
        public void Highlight(bool highlight)
        {
            _highlight.enabled = highlight;
        }
    }
}