using System;
using System.Collections;
using System.Collections.Generic;
using GameElements;
using GameElements.Nodes;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace LevelManagement
{
    public class LevelController : MonoBehaviour
    {
        [Header("Prefabs")]
        public SourceNode SourcePrefab;
        public ColorNode ColorPrefab;
        public ShapeNode ShapePrefab;
        public DestinationNode DestinationPrefab;

        [Header("Spawn Columns (in world coordinates)")]
        [Tooltip("Spawn areas for Source nodes")]
        public List<SpawnArea> SourceSpawnAreas;
        [Tooltip("Spawn areas for Color nodes")]
        public List<SpawnArea> ColorSpawnAreas;
        [Tooltip("Spawn areas for Shape nodes")]
        public List<SpawnArea> ShapeSpawnAreas;
        [Tooltip("Spawn areas for Destination nodes")]
        public List<SpawnArea> DestinationSpawnAreas;

        // New fields for connection tracking.
        private List<SourceNode> activeSources = new();
        private List<DestinationNode> activeDestinations = new();
        private int connectionCount = 0;

        private void Start()
        {
            // Load code-defined spawn commands.
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
            List<SpawnArea> areas = command.SpawnType switch
            {
                SpawnType.Source => SourceSpawnAreas,
                SpawnType.Color => ColorSpawnAreas,
                SpawnType.Shape => ShapeSpawnAreas,
                SpawnType.Destination => DestinationSpawnAreas,
                _ => throw new ArgumentOutOfRangeException()
            };

            // Use new helper to get a non-overlapping spawn position.
            Vector3 spawnPos = GetValidSpawnPosition(areas);

            switch (command.SpawnType)
            {
                case SpawnType.Source:
                    var source = Instantiate(SourcePrefab, spawnPos, Quaternion.identity, transform);
                    activeSources.Add(source);
                    var sourceTimer = source.gameObject.GetComponent<ConnectionRequirementTimer>();
                    sourceTimer.Setup(new List<UnityEngine.Object>(activeDestinations), 15f);
                    break;
                case SpawnType.Color:
                    var colorNode = Instantiate(ColorPrefab, spawnPos, Quaternion.identity, transform);
                    colorNode.Setup(command.Color);
                    break;
                case SpawnType.Shape:
                    var shapeNode = Instantiate(ShapePrefab, spawnPos, Quaternion.identity, transform);
                    shapeNode.Setup(command.Shape);
                    break;
                case SpawnType.Destination:
                    var destinationNode = Instantiate(DestinationPrefab, spawnPos, Quaternion.identity, transform);
                    destinationNode.Setup(command.Shape, command.Color);
                    activeDestinations.Add(destinationNode);
                    var destTimer = destinationNode.gameObject.GetComponent<ConnectionRequirementTimer>();
                    destTimer.Setup(new List<UnityEngine.Object>(activeSources), 15f);
                    break;
            }
        }

        public void RegisterConnection(SourceNode source, DestinationNode destination)
        {
            connectionCount++;
            int required = activeSources.Count * activeDestinations.Count;
            Debug.Log($"Connection registered: {connectionCount}/{required}");

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
            Vector3 position = Vector3.zero;
            for (int i = 0; i < maxAttempts; i++)
            {
                position = areas[Random.Range(0, areas.Count)].GetRandomPosition();
                if (Physics2D.OverlapCircleAll(position, collisionRadius).Length == 0)
                    return position;
            }
            Debug.LogWarning($"Failed to find a valid spawn position after {maxAttempts} attempts.");
            return position;
        }
    }
}