using System;
using UnityEngine;

namespace GameElements
{
    public class SpawnPoint : MonoBehaviour
    {
        public float Weight = 1;
        
        public Color Color = Color.green;
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color;
            Gizmos.DrawSphere(transform.position, 0.5f);
        }
    }
}
