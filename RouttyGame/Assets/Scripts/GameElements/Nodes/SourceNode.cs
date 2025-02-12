using System.Collections.Generic;
using UnityEngine;

namespace GameElements.Nodes
{
    public class SourceNode : Node
    {
        public override NodeType NodeType => NodeType.Source;

        public int DocumentsPerMinute = 10;

        public void HighlightConnections(HighlightType highlightType)
        {
            foreach (var connection in Connections)
            {
                connection.Highlight(highlightType);
            }
        }
    }
}