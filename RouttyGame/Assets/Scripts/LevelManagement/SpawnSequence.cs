using System.Collections.Generic;
using System.Linq;
using _Common.Extensions;
using GameElements.Nodes;
using UnityEngine;

namespace LevelManagement
{
    public static class SpawnSequence
    {
        public static List<TimedSpawnCommand> GetSpawnCommands()
        {
            var commands = new List<TimedSpawnCommand>();
            var cumulativeTime = 0f;
            var spawnOffset = 1f;
            var connectTime = 10f;


            var colors = new[] { NodeColor.Red, NodeColor.Green, NodeColor.Blue, NodeColor.Yellow };
            var shapes = new[] { NodeShape.Circle, NodeShape.Square, NodeShape.Triangle, NodeShape.Star };

            var combinations = new List<(NodeColor color, NodeShape shape)>();
            foreach (var color in colors)
            {
                combinations.AddRange(shapes.Select(shape => (color, shape)));
            }

            combinations.Shuffle();

            var totalDestinations = 15;

            var spawnedColors = new HashSet<NodeColor>();
            var spawnedShapes = new HashSet<NodeShape>();

            cumulativeTime += spawnOffset;
            commands.Add(new TimedSpawnCommand
                { SpawnTime = cumulativeTime, SpawnType = SpawnType.Source, WaveIndex = 0 });
            for (var wave = 0; wave < totalDestinations; wave++)
            {
                if (wave is 4 or 10)
                {
                    cumulativeTime += spawnOffset;
                    commands.Add(new TimedSpawnCommand
                    {
                        SpawnTime = cumulativeTime,
                        SpawnType = SpawnType.Source,
                        WaveIndex = wave
                    });
                    cumulativeTime += connectTime;
                }

                var (waveColor, waveShape) = combinations[wave];

                cumulativeTime += spawnOffset;
                if (!spawnedColors.Contains(waveColor))
                {
                    commands.Add(new TimedSpawnCommand
                    {
                        SpawnTime = cumulativeTime,
                        SpawnType = SpawnType.Color,
                        Color = waveColor,
                        WaveIndex = wave
                    });
                    spawnedColors.Add(waveColor);
                }

                cumulativeTime += spawnOffset;
                if (!spawnedShapes.Contains(waveShape))
                {
                    commands.Add(new TimedSpawnCommand
                    {
                        SpawnTime = cumulativeTime,
                        SpawnType = SpawnType.Shape,
                        Shape = waveShape,
                        WaveIndex = wave
                    });
                    spawnedShapes.Add(waveShape);
                }

                cumulativeTime += spawnOffset;
                commands.Add(new TimedSpawnCommand
                {
                    SpawnTime = cumulativeTime,
                    SpawnType = SpawnType.Destination,
                    Color = waveColor,
                    Shape = waveShape,
                    WaveIndex = wave
                });

                cumulativeTime += connectTime;
            }

            cumulativeTime += 30f;
            commands.Add(new TimedSpawnCommand { SpawnTime = cumulativeTime, SpawnType = SpawnType.Win });

            Debug.Log($"Total game time: {cumulativeTime} seconds");

            return commands;
        }
    }
}