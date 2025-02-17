using System;
using System.Collections.Generic;
using GameElements.Resources;
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
        
        [SerializeField]
        private SpriteRenderer _highlight;
        
        [SerializeField]
        private Color _highlightColor;
        private Color _defaultColor;

        [Tooltip("Documents per minute that this destination can receive")]
        public int DocumentsPerMinute = 15;

        private SpriteRenderer _spriteRenderer;
        
        [SerializeField]
        private SpriteRenderer _iconRenderer;
        [SerializeField]
        private SpriteRenderer _iconBackgroundRenderer;
        
        [SerializeField]
        private SpriteRenderer _triangleIconRenderer;
        [SerializeField]
        private SpriteRenderer _triangleBackgroundRenderer;

        [SerializeField]
        private SpriteRenderer _starIconRenderer;
        [SerializeField]
        private SpriteRenderer _starBackgroundRenderer;
        private Sprite _iconSprite;

        private void OnEnable()
        {
            _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            
            _defaultColor = _highlight.color;
            Highlight(false);
        }

        public void Setup(NodeShape shape, NodeColor color, int waveIndex)
        {
            DocumentsPerMinute = 10 + waveIndex * 2;
            
            _iconSprite = ChannelCollection.Instance.ChannelSprites[waveIndex];
            if (shape == NodeShape.Triangle)
            {
                _triangleBackgroundRenderer.enabled = true;
                _triangleIconRenderer.enabled = true;
                _triangleIconRenderer.sprite = _iconSprite;
                
                _iconRenderer.enabled = false;
                _iconBackgroundRenderer.enabled = false;
                
                _starBackgroundRenderer.enabled = false;
                _starIconRenderer.enabled = false;
            }
            else if (shape == NodeShape.Star)
            {
                _starBackgroundRenderer.enabled = true;
                _starIconRenderer.enabled = true;
                _starIconRenderer.sprite = _iconSprite;
                
                _iconRenderer.enabled = false;
                _iconBackgroundRenderer.enabled = false;
                
                _triangleBackgroundRenderer.enabled = false;
                _triangleIconRenderer.enabled = false;
            }
            else
            {
                _iconBackgroundRenderer.enabled = true;
                _iconRenderer.enabled = true;
                _iconRenderer.sprite = _iconSprite;
                
                _triangleBackgroundRenderer.enabled = false;
                _triangleIconRenderer.enabled = false;
                
                _starBackgroundRenderer.enabled = false;
                _starIconRenderer.enabled = false;
            }
                
            Shape = shape;
            Color = color;

            _spriteRenderer.sprite = NodeCollection.Instance.ShapeToSpriteDictionary[Shape];
            _spriteRenderer.color = NodeCollection.Instance.ColorToColorDictionary[Color];
            
            _highlight.sprite = _spriteRenderer.sprite;
        }

        public override bool CanConnect(List<Node> currentPath, Node startNode)
        {
            var alreadyConnected = startNode.ConnectedToNode(this);
            var anyColorMatch = currentPath.Exists(node => node.MatchColor(Color));
            var anyShapeMatch = currentPath.Exists(node => node.MatchShape(Shape));
            return !alreadyConnected && anyColorMatch && anyShapeMatch && base.CanConnect(currentPath, startNode);
        }
        
        public void Highlight(bool highlight)
        {
            _highlight.color = highlight ? _highlightColor : _defaultColor;
        }
    }
}