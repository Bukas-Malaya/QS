using UnityEngine;

namespace WhereFirefliesReturn.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Game State")]
        public GameState CurrentState { get; private set; } = GameState.MainMenu;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void StartGame()
        {
            ChangeState(GameState.Playing);
            SceneController.Instance.LoadScene("GameWorld");
        }

        public void GameOver(bool restored)
        {
            ChangeState(GameState.GameOver);
            SceneController.Instance.LoadScene("EndScreen");
        }

        // Parameterless wrappers — use these in UnityEvent Inspector wiring
        public void EndGame()
        {
            bool restored = WhereFirefliesReturn.Environment.EnvironmentMeter.Instance != null
                && WhereFirefliesReturn.Environment.EnvironmentMeter.Instance.CurrentValue >= 80f;
            GameOver(restored);
        }

        public void ReturnToMenu()
        {
            ChangeState(GameState.MainMenu);
            SceneController.Instance.LoadScene("MainMenu");
        }

        void ChangeState(GameState newState)
        {
            CurrentState = newState;
        }
    }

    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver
    }
}
