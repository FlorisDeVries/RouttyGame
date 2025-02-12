using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameElements.Nodes
{
    public abstract class Node : MonoBehaviour
    {
        public List<Connection> Connections = new();
        
        public abstract NodeType NodeType { get; }

        public virtual bool CanConnect(List<Node> currentPath, Node previousNode)
        {
            return currentPath.All(node => node.NodeType != NodeType);
        }

        public virtual bool MatchColor(NodeColor color)
        {
            return false;
        }
        
        public virtual bool MatchShape(NodeShape shape)
        {
            return false;
        }
        
        public virtual void Connect(Connection connection)
        {
            Connections.Add(connection);
        }
    }
    
    public enum NodeType
    {
        Source,
        Color,
        Shape,
        Destination
    }
}