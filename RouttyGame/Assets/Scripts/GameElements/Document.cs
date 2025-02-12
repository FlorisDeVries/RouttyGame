using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace GameElements
{
    public class Document : MonoBehaviour
    {
        [Tooltip("Movement speed in units per second along the path.")]
        public float moveSpeed = 5f;

        [Tooltip("Duration for the spawn (scale up) animation.")]
        public float spawnAnimationDuration = 0.3f;

        [Tooltip("Duration for the despawn (scale down) animation.")]
        public float despawnAnimationDuration = 0.3f;

        [Tooltip("Extra scale factor for the 'bounce' on intermediary nodes (1 = no change).")]
        public float bounceScale = 1.2f;

        private List<ConnectionPoint> _path;
        private SpriteRenderer _spriteRenderer;
        public SpriteRenderer StampSpriteRenderer;

        private void OnEnable()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            transform.localScale = Vector3.zero;
        }

        public void SetPath(List<ConnectionPoint> path)
        {
            _path = path;
            if (_path == null || _path.Count < 2)
            {
                Debug.LogWarning("Document path is null or has fewer than 2 points.");
                return;
            }

            SpawnAndMove();
        }

        private void SpawnAndMove()
        {
            // Set initial position
            transform.position = _path[0].Position;
            var segmentDurations = new List<float>();

            // Build the movement sequence.
            var moveSeq = DOTween.Sequence();
            for (int i = 0; i < _path.Count - 1; i++)
            {
                var currentCp = _path[i];
                var nextCp = _path[i + 1];
                var segmentDuration = Vector3.Distance(currentCp.Position, nextCp.Position) / moveSpeed;
                segmentDurations.Add(segmentDuration);

                moveSeq.Append(
                    transform.DOMove(nextCp.Position, segmentDuration)
                             .SetEase(Ease.Linear)
                );
                moveSeq.AppendCallback(() => ApplyNodeEffect(nextCp));
            }
            moveSeq.OnComplete(() => Destroy(gameObject));

            // Build the scaling sequence.
            var scaleSeq = DOTween.Sequence();
            scaleSeq.Append(
                transform.DOScale(Vector3.one, spawnAnimationDuration)
                         .SetEase(Ease.OutBack)
            );

            var cumulativeTime = 0f;
            var bounceDuration = .5f;

            for (int i = 1; i < _path.Count - 1; i++)
            {
                cumulativeTime += segmentDurations[i - 1];

                scaleSeq.Insert(cumulativeTime - bounceDuration,
                    transform.DOScale(0.5f, bounceDuration)
                             .SetEase(Ease.InSine)
                );

                scaleSeq.Insert(cumulativeTime,
                    transform.DOScale(1f, bounceDuration)
                             .SetEase(Ease.OutSine)
                );
            }
            
            // Add the last segment duration.
            cumulativeTime += segmentDurations[segmentDurations.Count - 1];

            // 3. Finally, scale down over despawnAnimationDuration so that scale reaches 0 at the very end.
            scaleSeq.Insert(cumulativeTime - despawnAnimationDuration,
                transform.DOScale(Vector3.zero, despawnAnimationDuration)
                         .SetEase(Ease.InBack)
            );

            // Start both sequences concurrently.
            moveSeq.Play();
            scaleSeq.Play();
        }
        
        /// <summary>
        /// Applies the node-specific effect (color, shape stamp, etc.)
        /// </summary>
        private void ApplyNodeEffect(ConnectionPoint cp)
        {
            if (cp.Node is Nodes.ColorNode colorNode)
            {
                _spriteRenderer.color = NodeCollection.Instance.ColorToColorDictionary[colorNode.Color];
            }

            if (cp.Node is Nodes.ShapeNode shapeNode)
            {
                StampSpriteRenderer.enabled = true;
                StampSpriteRenderer.sprite = NodeCollection.Instance.ShapeToSpriteDictionary[shapeNode.Shape];
            }
        }
    }
}
