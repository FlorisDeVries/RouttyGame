using GameElements.Nodes;
using UnityEngine;

namespace GameElements
{
    public class ConnectionPoint
    {
        public Vector3 Position;
        public Node Node;

        public ConnectionPoint(Vector3 position, Node node)
        {
            Position = position;
            Node = node;
        }
    }
}