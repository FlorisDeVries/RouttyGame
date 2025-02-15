using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GameElements.Nodes
{
    public abstract class Node : MonoBehaviour
    {
        public Dictionary<Connection, ConnectionPoint> Connections;
        
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
            Connections ??= new Dictionary<Connection, ConnectionPoint>();

            var connectionPoint = connection.GetConnectionPoint(this);
            Connections.Add(connection, connectionPoint);
        }

        private void Update()
        {
            if (Connections == null)
                return;

            foreach (var (connection, point) in Connections)
            {
                connection.UpdateConnectionPointPosition(point, transform.position);
            }
        }
    }
    
    public enum NodeType
    {
        Source,
        Color,
        Shape,
        Destination,
        Routty
    }
}