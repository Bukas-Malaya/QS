using UnityEngine;
using UnityEngine.UI;
using WhereFirefliesReturn.Core;

namespace WhereFirefliesReturn.UI
{
    public class MainMenuUI : MonoBehaviour
    {
        [Header("Buttons")]
        [SerializeField] private Button startButton;
        [SerializeField] private Button quitButton;

        void Start()
        {
            if (startButton != null)
                startButton.onClick.AddListener(() => SceneController.Instance.LoadScene("GameWorld"));

            if (quitButton != null)
                quitButton.onClick.AddListener(() => Application.Quit());
        }
    }
}
