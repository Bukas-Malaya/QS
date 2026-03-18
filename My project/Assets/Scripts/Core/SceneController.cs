using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace WhereFirefliesReturn.Core
{
    public class SceneController : MonoBehaviour
    {
        public static SceneController Instance { get; private set; }

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void LoadScene(string sceneName) => StartCoroutine(LoadAsync(sceneName));

        public void LoadMainMenu() => LoadScene("MainMenu");
        public void LoadGameWorld() => LoadScene("GameWorld");
        public void LoadEndScreen() => LoadScene("EndScreen");

        IEnumerator LoadAsync(string sceneName)
        {
            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
            while (!op.isDone) yield return null;
        }
    }
}
