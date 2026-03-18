using UnityEngine;

namespace WhereFirefliesReturn.Core
{
    public enum GameState { MainMenu, Playing, Paused, GameOver }

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public GameState CurrentState { get; private set; } = GameState.MainMenu;

        public event System.Action<GameState> OnStateChanged;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        void Start() => SetState(GameState.Playing);

        public void SetState(GameState newState)
        {
            CurrentState = newState;
            Time.timeScale = newState == GameState.Paused ? 0f : 1f;
            OnStateChanged?.Invoke(newState);
        }

        public void StartGame() => SetState(GameState.Playing);
        public void PauseGame() => SetState(GameState.Paused);
        public void ResumeGame() => SetState(GameState.Playing);
        public void EndGame() => SetState(GameState.GameOver);
    }
}
