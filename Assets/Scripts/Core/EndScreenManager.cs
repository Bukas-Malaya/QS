using UnityEngine;
using TMPro;
using WhereFirefliesReturn.Environment;

namespace WhereFirefliesReturn.Core
{
    public class EndScreenManager : MonoBehaviour
    {
        [Header("Ending Panels")]
        [SerializeField] private GameObject restorationPanel;
        [SerializeField] private GameObject collapsePanel;
        [SerializeField] private GameObject neutralPanel;

        [Header("Score Display")]
        [SerializeField] private TextMeshProUGUI scoreText;

        [Header("Buttons")]
        [SerializeField] private UnityEngine.UI.Button playAgainButton;
        [SerializeField] private UnityEngine.UI.Button mainMenuButton;

        void Start()
        {
            float finalValue = EnvironmentMeter.Instance != null
                ? EnvironmentMeter.Instance.CurrentValue
                : 50f;

            ShowEnding(finalValue);

            if (playAgainButton != null)
                playAgainButton.onClick.AddListener(() => GameManager.Instance?.StartGame());

            if (mainMenuButton != null)
                mainMenuButton.onClick.AddListener(() => GameManager.Instance?.ReturnToMenu());
        }

        void ShowEnding(float value)
        {
            restorationPanel?.SetActive(false);
            collapsePanel?.SetActive(false);
            neutralPanel?.SetActive(false);

            if (value >= 80f)
                restorationPanel?.SetActive(true);
            else if (value <= 20f)
                collapsePanel?.SetActive(true);
            else
                neutralPanel?.SetActive(true);

            if (scoreText != null)
                scoreText.text = $"Final Health: {value:F0} / 100";
        }
    }
}
