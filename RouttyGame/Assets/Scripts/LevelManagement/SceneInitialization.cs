using _Common.Unity;
using GameManagement;
using GameManagement.Resources;
using Scores.Resources;
using UnityEngine;
using UserInput.Resources;

namespace LevelManagement
{
    public class SceneInitialization : MonoBehaviour
    {
        [SerializeField]
        private int _sceneInitializationFrames = 3;

        private void OnEnable()
        {
            var mainCamera = Camera.main;
            
            ScoreManager.Instance.Authenticate();

            StartCoroutine(CoroutineHelper.DelayFixedFrames(
                () =>
                {
                    GameManager.Instance.ChangeState(GameState.Playing);
                    GameManager.Instance.SetMainCamera(mainCamera);
                    ScoreManager.Instance.ShowLeaderBoards(false);
                }, _sceneInitializationFrames)
            );
            
            GameplayInputActions.Instance.SetActive(true);
            GameplayInputActions.Instance.PauseGameAction.OnValueChanged += SetPause;
        }

        private void OnDisable()
        {
            GameManager.Instance.ChangeState(GameState.Loading);
            GameplayInputActions.Instance.PauseGameAction.OnValueChanged -= SetPause;
        }

        private void SetPause(bool clicked)
        {
            if (!clicked)
            {
                return;
            }

            GameManager.Instance.TogglePause();
        }
    }
}