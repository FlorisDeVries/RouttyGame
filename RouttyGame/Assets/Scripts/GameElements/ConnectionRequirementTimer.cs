using System.Collections.Generic;
using _Common.Events.Resources;
using DG.Tweening;
using GameManagement;
using GameManagement.Resources;
using Scores.Resources;
using Shapes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameElements
{
    public class ConnectionRequirementTimer : MonoBehaviour
    {
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
        
        [Header("Score Fine")]
        [SerializeField]
        private int fineAmount = 50;
        
        [SerializeField]
        private ScoreEventObject _scoreEventObject;

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
            var timeLeft = Mathf.Max(TimeLimit - elapsedTime, 0f);
            var fraction = timeLeft / TimeLimit;

            timerRing.AngRadiansEnd = ShapesMath.TAU * fraction;

            timerRing.Color = Color.Lerp(ringExpiredColor, ringColor, fraction);

            if (timeLeft <= 0f)
            {
                timerRing.Color = ringExpiredColor;
            }

            if (elapsedTime > TimeLimit && RequiredCounterparts.Count > 0)
            {
                Debug.LogWarning($"{gameObject.name} failed to connect with {RequiredCounterparts.Count} required counterpart(s) within {TimeLimit} seconds.");
                
                spawnTime = Time.time;
                _scoreEventObject.RaiseEvent(new ScoreEvent(-fineAmount, transform.position));
                ScoreManager.Instance.AddFine(fineAmount);
            }
        }
    }
}
