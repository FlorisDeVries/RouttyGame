using System;
using System.Collections.Generic;
using _Common.BaseClasses;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using UnityEngine;
using UnityEngine.Serialization;

namespace Scores.Resources
{
    [CreateAssetMenu(fileName = "Score Manager", menuName = "Project/Scores/Score Manager")]
    public class ScoreManager : ASingletonResource<ScoreManager>
    {
        protected override string ResourcePath => "Scores/Score Manager";

        public int CurrentScore { get; private set; }

        public List<LeaderboardEntry> LeaderboardEntries { get; private set; } = new();
        public event Action<bool> OnShowLeaderBoardsEvent = delegate { };
        public string PlayerName { get; private set; }

        public async void Authenticate()
        {
            await UnityServices.InitializeAsync();

            if (!AuthenticationService.Instance.IsAuthorized)
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            
            UpdatePlayerName();
            FetchLeaderboard();
        }

        public void AddScore(int score)
        {
            CurrentScore += score;
        }

        public void ResetScore()
        {
            CurrentScore = 0;
        }
        
        public async void FetchLeaderboard()
        {
            const string leaderboardId = "Highscore";
            var scores = await LeaderboardsService.Instance.GetScoresAsync(leaderboardId, new GetScoresOptions { Limit = 10 });

            LeaderboardEntries.Clear();
            
            foreach (var entry in scores.Results)
            {
                LeaderboardEntries.Add(new LeaderboardEntry
                {
                    Rank = entry.Rank + 1,
                    PlayerName = entry.PlayerName,
                    Score = (int)entry.Score
                });
            }
        }
        
        public async void UpdatePlayerName()
        {
            var playerName = await AuthenticationService.Instance.GetPlayerNameAsync();
            PlayerName = playerName[..^5];
        }

        public class LeaderboardEntry
        {
            public int Rank;
            public string PlayerName;
            public int Score;
        }

        public void ShowLeaderBoards(bool show)
        {
            OnShowLeaderBoardsEvent.Invoke(show);
        }
    }
}
