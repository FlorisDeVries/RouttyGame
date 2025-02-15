using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using _Common.Events;
using DG.Tweening;
using GameElements;
using GameElements.Nodes;
using GameManagement;
using GameManagement.Resources;
using Scores.Resources;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

namespace LevelManagement
{
    public class LevelController : AButtonPress
    {
        [Header("Prefabs")]
        public SourceNode SourcePrefab;

        public ColorNode ColorPrefab;
        public ShapeNode ShapePrefab;
        public DestinationNode DestinationPrefab;

        [Header("Spawn Columns (in world coordinates)")]
        [Tooltip("Spawn locations for Source nodes")]
        public List<SpawnPoint> SourceSpawnLocations;

        [Tooltip("Spawn locations for Color nodes")]
        public List<SpawnPoint> ColorSpawnLocations;

        [Tooltip("Spawn locations for Shape nodes")]
        public List<SpawnPoint> ShapeSpawnLocations;

        [Tooltip("Spawn locations for Destination nodes")]
        public List<SpawnPoint> DestinationSpawnLocations;

        [Header("Game Elements")]
        [SerializeField]
        private RouttyNode _routtyNode;

        private readonly List<SourceNode> _activeSources = new();
        private readonly List<ColorNode> _activeColors = new();
        private readonly List<ShapeNode> _activeShapes = new();
        private readonly List<DestinationNode> _activeDestinations = new();

        public static bool RouttyIsPurchased { get; private set; }

        private void Start()
        {
            ScoreManager.Instance.ResetScore();

            // Load code-defined spawn commands.
            RouttyIsPurchased = false;
            var spawnCommands = SpawnSequence.GetSpawnCommands();
            StartCoroutine(SpawnCommandsCoroutine(spawnCommands));
        }

        private IEnumerator SpawnCommandsCoroutine(List<TimedSpawnCommand> spawnCommands)
        {
            var startTime = Time.time;
            foreach (var command in spawnCommands)
            {
                var delay = command.SpawnTime - (Time.time - startTime);
                if (delay > 0)
                    yield return new WaitForSeconds(delay);

                Spawn(command);
            }
        }

        private void Spawn(TimedSpawnCommand command)
        {
            if (command.SpawnType == SpawnType.Win)
            {
                GameManager.Instance.ChangeState(GameState.Victory);
                return;
            }
            
            var spawnPos = GetSpawnPosition(command.SpawnType);

            switch (command.SpawnType)
            {
                case SpawnType.Source:
                    var source = Instantiate(SourcePrefab, spawnPos, Quaternion.identity, transform);
                    _activeSources.Add(source);
                    var sourceTimer = source.gameObject.GetComponent<ConnectionRequirementTimer>();
                    sourceTimer.Setup(new List<UnityEngine.Object>(_activeDestinations), 15f);
                    break;
                case SpawnType.Color:
                    if (RouttyIsPurchased) break;

                    var colorNode = Instantiate(ColorPrefab, spawnPos, Quaternion.identity, transform);
                    _activeColors.Add(colorNode);
                    colorNode.Setup(command.Color);
                    break;
                case SpawnType.Shape:
                    if (RouttyIsPurchased) break;

                    var shapeNode = Instantiate(ShapePrefab, spawnPos, Quaternion.identity, transform);
                    _activeShapes.Add(shapeNode);
                    shapeNode.Setup(command.Shape);
                    break;
                case SpawnType.Destination:
                    var destinationNode = Instantiate(DestinationPrefab, spawnPos, Quaternion.identity, transform);
                    destinationNode.Setup(command.Shape, command.Color);
                    _activeDestinations.Add(destinationNode);
                    var destTimer = destinationNode.gameObject.GetComponent<ConnectionRequirementTimer>();
                    destTimer.Setup(new List<UnityEngine.Object>(_activeSources), 15f);
                    break;
            }
        }

        public void RegisterConnection(SourceNode source, DestinationNode destination)
        {
            // Remove fulfilled requirement from source's timer.
            var sourceTimer = source.GetComponent<ConnectionRequirementTimer>();
            if (sourceTimer != null)
                sourceTimer.RemoveRequirement(destination);

            // Remove fulfilled requirement from destination's timer.
            var destTimer = destination.GetComponent<ConnectionRequirementTimer>();
            if (destTimer != null)
                destTimer.RemoveRequirement(source);
        }

        private Vector3 GetValidSpawnPosition(List<SpawnArea> areas)
        {
            const int maxAttempts = 10;
            const float collisionRadius = 2f;
            var position = Vector3.zero;
            for (var i = 0; i < maxAttempts; i++)
            {
                position = areas[Random.Range(0, areas.Count)].GetRandomPosition();
                if (Physics2D.OverlapCircleAll(position, collisionRadius).Length == 0)
                    return position;
            }

            Debug.LogWarning($"Failed to find a valid spawn position after {maxAttempts} attempts.");
            return position;
        }

        private Vector3 GetSpawnPosition(SpawnType type)
        {
            switch (type)
            {
                case SpawnType.Destination:
                {
                    var spawnPoint = GetWeightedSpawnPoint(DestinationSpawnLocations);
                    DestinationSpawnLocations.Remove(spawnPoint);
                    return spawnPoint.transform.position;
                }
                case SpawnType.Source:
                {
                    var spawnPoint = GetWeightedSpawnPoint(SourceSpawnLocations);
                    SourceSpawnLocations.Remove(spawnPoint);
                    return spawnPoint.transform.position;
                }
                case SpawnType.Color:
                {
                    var spawnPoint = GetWeightedSpawnPoint(ColorSpawnLocations);
                    ColorSpawnLocations.Remove(spawnPoint);
                    return spawnPoint.transform.position;
                }
                case SpawnType.Shape:
                {
                    var spawnPoint = GetWeightedSpawnPoint(ShapeSpawnLocations);
                    ShapeSpawnLocations.Remove(spawnPoint);
                    return spawnPoint.transform.position;
                }
                default:
                    return Vector3.zero;
            }
        }

        private SpawnPoint GetWeightedSpawnPoint(List<SpawnPoint> spawnPoints)
        {
            var totalWeight = spawnPoints.Sum(spawnPoint => spawnPoint.Weight);

            var randomValue = Random.Range(0f, totalWeight);
            foreach (var spawnPoint in spawnPoints)
            {
                randomValue -= spawnPoint.Weight;
                if (randomValue <= 0)
                    return spawnPoint;
            }

            // Fallback (shouldn't happen if list isn't empty)
            return spawnPoints[^1];
        }

        /// <summary>
        /// Called on Routty bought click
        /// </summary>
        /// <param name="clickEvent"></param>
        /// <exception cref="NotImplementedException"></exception>
        protected override void OnButtonClicked(ClickEvent clickEvent)
        {
            RouttyIsPurchased = true;
            _routtyNode.Enable();

            // Tweak these parameters as needed.
            var targetPos = _routtyNode.transform.position;
            var overshootDistance = 1f; // How far the node moves in the first phase.
            var phaseOneDuration = 0.3f; // Duration for the overshoot.
            var phaseTwoDuration = 0.7f; // Duration for the final move to target.

            // Animate color nodes.
            foreach (var colorNode in _activeColors)
            {
                var seq = DOTween.Sequence();

                // Determine direction from the target.
                var dir = (colorNode.transform.position - targetPos).normalized;
                // Calculate an overshoot position further away in that direction.
                var overshootPos = colorNode.transform.position + dir * overshootDistance;

                seq.Append(
                    colorNode.transform.DOMove(overshootPos, phaseOneDuration)
                        .SetEase(Ease.OutQuad)
                );

                seq.Append(
                    colorNode.transform.DOMove(targetPos, phaseTwoDuration)
                        .SetEase(Ease.InQuad)
                );

                // Optional: add a slight rotation for a playful effect.
                seq.Join(
                    colorNode.transform.DORotate(
                        new Vector3(0, 0, Random.Range(-90f, 90f)),
                        phaseOneDuration + phaseTwoDuration,
                        RotateMode.FastBeyond360
                    )
                );
            }

            // Animate shape nodes.
            foreach (var shapeNode in _activeShapes)
            {
                var seq = DOTween.Sequence();

                var dir = (shapeNode.transform.position - targetPos).normalized;
                var overshootPos = shapeNode.transform.position + dir * overshootDistance;

                seq.Append(
                    shapeNode.transform.DOMove(overshootPos, phaseOneDuration)
                        .SetEase(Ease.OutQuad)
                );

                seq.Append(
                    shapeNode.transform.DOMove(targetPos, phaseTwoDuration)
                        .SetEase(Ease.InQuad)
                );

                // Optional rotation for playfulness.
                seq.Join(
                    shapeNode.transform.DORotate(
                        new Vector3(0, 0, Random.Range(-90f, 90f)),
                        phaseOneDuration + phaseTwoDuration,
                        RotateMode.FastBeyond360
                    )
                );
            }
        }
    }
}