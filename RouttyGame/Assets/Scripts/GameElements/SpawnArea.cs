using UnityEngine;

namespace GameElements
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class SpawnArea : MonoBehaviour
    {
        private BoxCollider2D _collider;
        
        public Color Color = Color.green;

        private void Awake()
        {
            _collider = GetComponent<BoxCollider2D>();
            _collider.isTrigger = true;
        }

        /// <summary>
        /// Returns a random point within the collider's bounds.
        /// </summary>
        public Vector3 GetRandomPosition()
        {
            var bounds = _collider.bounds;
            var x = Random.Range(bounds.min.x, bounds.max.x);
            var y = Random.Range(bounds.min.y, bounds.max.y);
            return new Vector3(x, y, transform.position.z);
        }

        // Optional: Draw the spawn area in the Scene view for easy design.
        private void OnDrawGizmos()
        {
            Gizmos.color = Color;
            var box = GetComponent<BoxCollider2D>();
            if (box == null) return;

            var bounds = box.bounds;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }
    }
}