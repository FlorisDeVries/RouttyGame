using UnityEngine;

namespace GameElements.Nodes
{
    public class ShapeNode : Node
    {
        public override NodeType NodeType => NodeType.Shape;

        public NodeShape Shape;
        private SpriteRenderer _spriteRenderer;

        public override bool MatchShape(NodeShape shape)
        {
            return Shape == shape;
        }

        private void OnEnable()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
        
        public void Setup(NodeShape shape)
        {
            Shape = shape;
            _spriteRenderer.sprite = NodeCollection.Instance.ShapeToSpriteDictionary[Shape];
        }
    }
    
    public enum NodeShape
    {
        Circle,
        Square,
        Triangle
    }
}