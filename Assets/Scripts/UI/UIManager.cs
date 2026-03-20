using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WhereFirefliesReturn.Environment;
using WhereFirefliesReturn.Resources;
using WhereFirefliesReturn.Farm;

namespace WhereFirefliesReturn.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Environment Meter")]
        [SerializeField] private Slider envMeterSlider;
        [SerializeField] private Image envMeterFill;
        [SerializeField] private Color collapseColor = new Color(0.8f, 0.2f, 0.1f);
        [SerializeField] private Color neutralColor  = new Color(0.9f, 0.7f, 0.1f);
        [SerializeField] private Color restoredColor = new Color(0.2f, 0.8f, 0.3f);

        [Header("Resources")]
        [SerializeField] private TextMeshProUGUI waterText;
        [SerializeField] private TextMeshProUGUI seedsText;
        [SerializeField] private TextMeshProUGUI cleanEnergyText;

        [Header("Cycle")]
        [SerializeField] private TextMeshProUGUI cycleText;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        void OnEnable()
        {
            if (EnvironmentMeter.Instance != null)
                EnvironmentMeter.Instance.OnValueChanged.AddListener(UpdateEnvMeter);

            if (ResourceManager.Instance != null)
                ResourceManager.Instance.OnResourcesChanged.AddListener(UpdateResources);

            if (CycleManager.Instance != null)
                CycleManager.Instance.OnCycleStarted.AddListener(UpdateCycle);
        }

        void OnDisable()
        {
            if (EnvironmentMeter.Instance != null)
                EnvironmentMeter.Instance.OnValueChanged.RemoveListener(UpdateEnvMeter);

            if (ResourceManager.Instance != null)
                ResourceManager.Instance.OnResourcesChanged.RemoveListener(UpdateResources);

            if (CycleManager.Instance != null)
                CycleManager.Instance.OnCycleStarted.RemoveListener(UpdateCycle);
        }

        void Start()
        {
            // Initial sync
            if (EnvironmentMeter.Instance != null)
                UpdateEnvMeter(EnvironmentMeter.Instance.CurrentValue);

            if (ResourceManager.Instance != null)
                UpdateResources();

            if (CycleManager.Instance != null)
                UpdateCycle(CycleManager.Instance.CurrentCycle);
        }

        void UpdateEnvMeter(float value)
        {
            if (envMeterSlider != null)
            {
                envMeterSlider.maxValue = 100f;
                envMeterSlider.value = value;
            }

            if (envMeterFill != null)
            {
                if (value <= 20f)
                    envMeterFill.color = collapseColor;
                else if (value >= 80f)
                    envMeterFill.color = restoredColor;
                else
                    envMeterFill.color = Color.Lerp(collapseColor, restoredColor, (value - 20f) / 60f);
            }
        }

        void UpdateResources()
        {
            if (waterText != null)
                waterText.text = $"Water: {ResourceManager.Instance.Water}";

            if (seedsText != null)
                seedsText.text = $"Seeds: {ResourceManager.Instance.Seeds}";

            if (cleanEnergyText != null)
                cleanEnergyText.text = $"Energy: {ResourceManager.Instance.CleanEnergy}";
        }

        void UpdateCycle(int cycle)
        {
            if (cycleText != null)
                cycleText.text = $"Cycle {cycle} / {5}";
        }
    }
}
