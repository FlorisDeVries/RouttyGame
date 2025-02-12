using System;
using System.Threading.Tasks;
using _Common.BaseClasses;
using _Common.Unity.Resources;
using Sirenix.OdinInspector;
using UnityEngine;

namespace GameManagement.Resources
{
    [CreateAssetMenu(fileName = "Game Manager", menuName = "Project/GameManagement/Game Manager")]
    public class GameManager : ASingletonResource<GameManager>
    {
        protected override string ResourcePath => "GameManagement/Game Manager";

        [ShowInInspector] [ReadOnly] public GameState CurrentGameState { get; private set; }

        public Camera MainCamera { get; private set; }

        public bool IsPaused => CurrentGameState != GameState.Playing;

        public event Action<GameState> OnExitStateEvent = delegate { };
        public event Action<GameState> OnEnterStateEvent = delegate { };

        public void SetMainCamera(Camera camera)
        {
            MainCamera = camera;
        }

        public void ChangeState(GameState newState)
        {
            OnExitStateEvent.Invoke(CurrentGameState);
            CurrentGameState = newState;

            switch (newState)
            {
                case GameState.Playing:
                    UnpauseGame();
                    break;
                case GameState.Victory:
                case GameState.GameOver:
                case GameState.Paused:
                    PauseGame();
                    break;
                case GameState.Loading:
                default:
                    break;
            }

            OnEnterStateEvent.Invoke(newState);
        }

        public void TogglePause()
        {
            switch (CurrentGameState)
            {
                case GameState.Playing:
                    ChangeState(GameState.Paused);
                    break;
                case GameState.Paused:
                    ChangeState(GameState.Playing);
                    break;
                case GameState.Loading:
                case GameState.Victory:
                case GameState.GameOver:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task LoadScene(UnityScene scene)
        {
            ChangeState(GameState.Loading);
            await scene.LoadSceneAsync();
            ChangeState(GameState.Playing);
        }

        private void PauseGame()
        {
            Time.timeScale = 0;
        }

        private void UnpauseGame()
        {
            Time.timeScale = 1;
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}