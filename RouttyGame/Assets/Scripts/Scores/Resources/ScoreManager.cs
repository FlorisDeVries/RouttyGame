using _Common.BaseClasses;
using UnityEngine;

namespace Scores.Resources
{
    [CreateAssetMenu(fileName = "Score Manager", menuName = "Project/Scores/Score Manager")]
    public class ScoreManager : ASingletonResource<ScoreManager>
    {
        protected override string ResourcePath => "Scores/Score Manager";

        public int CurrentScore { get; private set; }

        public void AddScore(int score)
        {
            CurrentScore += score;
        }

        public void ResetScore()
        {
            CurrentScore = 0;
        }
    }
}
