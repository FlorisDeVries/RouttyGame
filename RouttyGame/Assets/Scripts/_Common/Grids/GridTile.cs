using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace _Common.Grids
{
    public class GridTile : MonoBehaviour
    {
        [ShowInInspector] [ReadOnly]
        private Vector2Int _position;

        public Vector2Int Position => _position;

        private void OnEnable()
        {
            Setup();
        }

        public virtual void Setup()
        {
            var position = transform.position;
            _position = new Vector2Int((int) position.x, (int) position.y);
        }

        public virtual bool IsTraversable()
        {
            return true;
        }
    }
}