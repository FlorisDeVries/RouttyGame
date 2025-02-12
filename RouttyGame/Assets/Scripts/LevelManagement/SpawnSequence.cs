using System.Collections.Generic;
using GameElements.Nodes;

namespace LevelManagement
{
    public static class SpawnSequence
    {
        public static List<TimedSpawnCommand> GetSpawnCommands()
        {
            var commands = new List<TimedSpawnCommand>();
            var cumulativeTime = 0f;
            var spawnOffset = 0.5f; // time between spawns in a connection
            var connectionGap = 1f; // gap between connections

            // Wave 1: Red & Circle
            cumulativeTime += spawnOffset;
            commands.Add(new TimedSpawnCommand { SpawnTime = cumulativeTime, SpawnType = SpawnType.Source });
            
            cumulativeTime += spawnOffset;
            commands.Add(new TimedSpawnCommand { SpawnTime = cumulativeTime, SpawnType = SpawnType.Color, Color = NodeColor.Red });
            
            cumulativeTime += spawnOffset;
            commands.Add(new TimedSpawnCommand { SpawnTime = cumulativeTime, SpawnType = SpawnType.Shape, Shape = NodeShape.Circle });
            
            cumulativeTime += spawnOffset;
            commands.Add(new TimedSpawnCommand { SpawnTime = cumulativeTime, SpawnType = SpawnType.Destination, Color = NodeColor.Red, Shape = NodeShape.Circle });

            // Gap before next connection.
            cumulativeTime += connectionGap;

            // Wave 2: Green & Square            
            cumulativeTime += spawnOffset;
            commands.Add(new TimedSpawnCommand { SpawnTime = cumulativeTime, SpawnType = SpawnType.Color, Color = NodeColor.Green });
            
            cumulativeTime += spawnOffset;
            commands.Add(new TimedSpawnCommand { SpawnTime = cumulativeTime, SpawnType = SpawnType.Shape, Shape = NodeShape.Square });
            
            cumulativeTime += spawnOffset;
            commands.Add(new TimedSpawnCommand { SpawnTime = cumulativeTime, SpawnType = SpawnType.Destination, Color = NodeColor.Green, Shape = NodeShape.Square });

            // Gap before next connection.
            cumulativeTime += connectionGap;

            // Wave 3: Blue & Triangle            
            cumulativeTime += spawnOffset;
            commands.Add(new TimedSpawnCommand { SpawnTime = cumulativeTime, SpawnType = SpawnType.Color, Color = NodeColor.Blue });
            
            cumulativeTime += spawnOffset;
            commands.Add(new TimedSpawnCommand { SpawnTime = cumulativeTime, SpawnType = SpawnType.Shape, Shape = NodeShape.Triangle });
            
            cumulativeTime += spawnOffset;
            commands.Add(new TimedSpawnCommand { SpawnTime = cumulativeTime, SpawnType = SpawnType.Destination, Color = NodeColor.Blue, Shape = NodeShape.Triangle });

            // Gap before next connection.
            cumulativeTime += connectionGap;

            // Wave 4: An additional source
            cumulativeTime += spawnOffset;
            commands.Add(new TimedSpawnCommand { SpawnTime = cumulativeTime, SpawnType = SpawnType.Source });
            
            return commands;
        }
    }
}
