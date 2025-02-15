using System.Collections.Generic;
using _Common.Events.Resources;
using DG.Tweening;
using GameElements.Nodes;
using GameManagement;
using GameManagement.Resources;
using Scores.Resources;
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
        
        [SerializeField]
        private ScoreEventObject _scoreEventObject;

        private List<ConnectionPoint> _path;
        private SpriteRenderer _spriteRenderer;
        public SpriteRenderer StampSpriteRenderer;

        private Tween _moveTween;

        private List<float> _cumulativeProgress;
        private List<float> _segmentDurations;

        private int _nextNodeEffectIndex = 1;

        private void OnEnable()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            transform.localScale = Vector3.zero;

            GameManager.Instance.OnEnterStateEvent += OnEnterState;
        }

        private void OnEnterState(GameState newState)
        {
            if (newState == GameState.GameOver)
            {
                Destroy(gameObject);
            }
        }

        private void OnDisable()
        {
            _moveTween?.Kill();
            GameManager.Instance.OnEnterStateEvent -= OnEnterState;
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
            transform.position = _path[0].Position;

            var segments = _path.Count - 1;
            var totalDistance = 0f;
            _segmentDurations = new List<float>();
            for (var i = 0; i < segments; i++)
            {
                var d = Vector3.Distance(_path[i].Position, _path[i + 1].Position);
                _segmentDurations.Add(d / moveSpeed);
                totalDistance += d;
            }

            _cumulativeProgress = new List<float> { 0f };
            var accum = 0f;
            for (var i = 0; i < segments; i++)
            {
                accum += (_segmentDurations[i] * moveSpeed) / totalDistance; // note: (_segmentDuration*moveSpeed) equals the original segment distance.
                _cumulativeProgress.Add(accum);
            }
            _cumulativeProgress[^1] = 1f;

            // Total duration based on total distance and moveSpeed.
            var totalDuration = totalDistance / moveSpeed;
            var progress = 0f;

            _moveTween = DOTween.To(() => progress, x => progress = x, 1f, totalDuration)
                .SetEase(Ease.Linear)
                .OnUpdate(() =>
                {
                    if (_nextNodeEffectIndex < _cumulativeProgress.Count &&
                        progress >= _cumulativeProgress[_nextNodeEffectIndex])
                    {
                        ApplyNodeEffect(_path[_nextNodeEffectIndex]);
                        _nextNodeEffectIndex++;
                    }

                    var seg = 0;
                    for (var i = 0; i < _cumulativeProgress.Count - 1; i++)
                    {
                        if (progress >= _cumulativeProgress[i] && progress <= _cumulativeProgress[i + 1])
                        {
                            seg = i;
                            break;
                        }
                    }

                    var segStart = _cumulativeProgress[seg];
                    var segEnd = _cumulativeProgress[seg + 1];
                    var segProgress = (progress - segStart) / (segEnd - segStart);

                    var startPos = _path[seg].Position;
                    var endPos = _path[seg + 1].Position;
                    transform.position = Vector3.Lerp(startPos, endPos, segProgress);
                })
                .OnComplete(OnFinalizePath);

            var scaleSeq = DOTween.Sequence();
            scaleSeq.Append(
                transform.DOScale(Vector3.one, spawnAnimationDuration)
                    .SetEase(Ease.OutBack)
            );

            var cumulativeTime = 0f;
            var bounceDuration = 0.5f;
            
            for (var i = 1; i < _path.Count - 1; i++)
            {
                cumulativeTime += _segmentDurations[i - 1];
                scaleSeq.Insert(cumulativeTime - bounceDuration,
                    transform.DOScale(0.5f, bounceDuration)
                        .SetEase(Ease.InSine)
                );

                scaleSeq.Insert(cumulativeTime,
                    transform.DOScale(1f, bounceDuration)
                        .SetEase(Ease.OutSine)
                );
            }

            cumulativeTime += _segmentDurations[^1];
            scaleSeq.Insert(cumulativeTime - despawnAnimationDuration,
                transform.DOScale(Vector3.zero, despawnAnimationDuration)
                    .SetEase(Ease.InBack)
            );

            _moveTween.Play();
            scaleSeq.Play();
        }
        
        private void OnFinalizePath()
        {
            // Score double if path contains routty node
            var score = _path.Exists(cp => cp.Node is RouttyNode) ? 2 : 1;
            _scoreEventObject.RaiseEvent(new ScoreEvent(score, null));
            ScoreManager.Instance.AddScore(score);
            Destroy(gameObject);
        }

        /// <summary>
        /// Applies the node-specific effect (color, shape stamp, etc.)
        /// </summary>
        private void ApplyNodeEffect(ConnectionPoint cp)
        {
            if (cp.Node is ColorNode colorNode)
            {
                _spriteRenderer.color = NodeCollection.Instance.ColorToColorDictionary[colorNode.Color];
            }

            if (cp.Node is ShapeNode shapeNode)
            {
                StampSpriteRenderer.enabled = true;
                StampSpriteRenderer.sprite = NodeCollection.Instance.ShapeToSpriteDictionary[shapeNode.Shape];
            }

            if (cp.Node is RouttyNode)
            {
                var destinationNode = _path[^1].Node as DestinationNode;
                if (!destinationNode) return;

                _spriteRenderer.color = NodeCollection.Instance.ColorToColorDictionary[destinationNode.Color];
                StampSpriteRenderer.enabled = true;
                StampSpriteRenderer.sprite = NodeCollection.Instance.ShapeToSpriteDictionary[destinationNode.Shape];
            }
        }
    }
}
