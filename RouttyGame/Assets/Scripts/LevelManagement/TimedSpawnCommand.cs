using System;
using GameElements.Nodes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LevelManagement
{
    public enum SpawnType
    {
        Source,
        Color,
        Shape,
        Destination,
        Win
    }
    
    [Serializable]
    public class TimedSpawnCommand
    {
        [Tooltip("Time (in seconds) after level start to spawn this node.")]
        public float SpawnTime;

        [Tooltip("The type of node to spawn.")]
        public SpawnType SpawnType;
        
        public bool ShouldShowColor => SpawnType is SpawnType.Color or SpawnType.Destination;
        public bool ShouldShowShape => SpawnType is SpawnType.Shape or SpawnType.Destination;

        [Tooltip("Optional: For Color nodes or Destination nodes, select the desired color.")]
        [ShowIf(nameof(ShouldShowColor))]
        public NodeColor Color;

        [Tooltip("Optional: For Shape nodes or Destination nodes, select the desired shape.")]
        [ShowIf(nameof(ShouldShowShape))]
        public NodeShape Shape;
    }
}
