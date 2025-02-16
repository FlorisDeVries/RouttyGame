using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace GameElements.Nodes
{
    public class RouttyNode : Node
    {
        public override NodeType NodeType => NodeType.Routty;

        private void Start()
        {
            transform.localScale = Vector3.zero;
        }

        public void Enable()
        {
            // Reset the scale to zero before animating.
            transform.localScale = Vector3.zero;
            
            Sequence seq = DOTween.Sequence();
    
            seq.Append(
                transform.DOScale(Vector3.one, 1f)
                    .SetEase(Ease.OutElastic)
            );
            
            transform.DORotate(new Vector3(0, 0, 720), .7f, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear);
        }

        public override bool MatchColor(NodeColor color)
        {
            return true;
        }

        public override bool MatchShape(NodeShape shape)
        {
            return true;
        }

        public override bool CanConnect(List<Node> currentPath, Node startNode)
        {
            return true;
        }
    }
}