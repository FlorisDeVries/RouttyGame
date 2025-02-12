using System.Collections.Generic;
using DG.Tweening;
using GameManagement;
using GameManagement.Resources;
using Shapes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameElements
{
    public class ConnectionRequirementTimer : MonoBehaviour
    {
        [Header("Timer Settings")]
        public bool IsSource;

        public float TimeLimit = 5f;
        public List<Object> RequiredCounterparts = new();

        [Header("Shapes Ring Reference")]
        [Tooltip("Assign a Shapes Ring component here")]
        [Required]
        public Disc timerRing;

        [Header("Appearance")]
        [Tooltip("Color of the ring during countdown (at full time left)")]
        public Color ringColor = Color.green;

        [Tooltip("Color of the ring when the timer runs out (at zero time left)")]
        public Color ringExpiredColor = Color.red;

        private float spawnTime;
        private bool isComplete;

        public void Setup(List<Object> requiredCounterparts, float timeLimit)
        {
            if (requiredCounterparts.Count == 0)
            {
                enabled = false;
                timerRing.enabled = false;
                return;
            }
            
            TimeLimit = timeLimit;
            RequiredCounterparts = requiredCounterparts;
            
            isComplete = false;
            spawnTime = Time.time;

            // Initialize the disc's appearance
            timerRing.Color = ringColor;
            timerRing.AngRadiansStart = 0f;
            timerRing.AngRadiansEnd = ShapesMath.TAU; // full circle
        }

        public void RemoveRequirement(Object counterpart)
        {
            if (!RequiredCounterparts.Contains(counterpart))
                return;
            
            RequiredCounterparts.Remove(counterpart);

            if (RequiredCounterparts.Count == 0)
            {
                isComplete = true;
                Debug.Log($"{gameObject.name} connected with all required counterpart(s) within {TimeLimit} seconds.");
                
                // Scale down the disc visually, then disable it
                timerRing.transform
                    .DOScale(Vector3.zero, 0.5f)
                    .OnComplete(() => timerRing.enabled = false);
            }
        }

        private void Update()
        {
            if (isComplete)
                return;

            var elapsedTime = Time.time - spawnTime;

            // Calculate fraction of time left (clamp to 0 so we don't get negative angles)
            var timeLeft = Mathf.Max(TimeLimit - elapsedTime, 0f);
            var fraction = timeLeft / TimeLimit; // goes from 1 (start) down to 0 (end)

            // Update the ring arc: from full circle down to 0
            timerRing.AngRadiansEnd = ShapesMath.TAU * fraction;

            // Smooth color transition: when fraction=1 => ringColor, fraction=0 => ringExpiredColor
            timerRing.Color = Color.Lerp(ringExpiredColor, ringColor, fraction);

            // Once time is fully up, color is already at red, 
            // but if you wish to do anything else specifically at time 0, do so here:
            if (timeLeft <= 0f)
            {
                // Optionally set color to ensure it's fully red
                timerRing.Color = ringExpiredColor;
            }

            // If time is up and there are still unconnected counterparts, handle failure
            if (elapsedTime > TimeLimit && RequiredCounterparts.Count > 0)
            {
                Debug.LogWarning($"{gameObject.name} failed to connect with {RequiredCounterparts.Count} required counterpart(s) within {TimeLimit} seconds.");
                GameManager.Instance.ChangeState(GameState.GameOver);
            }
        }
    }
}
