using UnityEngine;
using UnityEngine.Events;

namespace _Common.Events.Resources
{
    [CreateAssetMenu(fileName = "ScoreEventObject", menuName = "Project/Events/ScoreEventObject")]
    public class ScoreEventObject : ScriptableObject
    {
        private readonly UnityEvent<ScoreEvent> _event = new();

        public void RegisterListener(UnityAction<ScoreEvent> listener)
        {
            _event.AddListener(listener);
        }

        public void UnregisterListener(UnityAction<ScoreEvent> listener)
        {
            _event.RemoveListener(listener);
        }

        public void RaiseEvent(ScoreEvent score)
        {
            _event.Invoke(score);
        }
    }

    public class ScoreEvent
    {
        public int Score { get; }
        public Vector3? Position { get; }
        
        public ScoreEvent(int score, Vector3? position)
        {
            Score = score;
            Position = position;
        }
    }
}