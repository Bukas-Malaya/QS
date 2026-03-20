using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace WhereFirefliesReturn.Environment
{
    public class EnvironmentMeter : MonoBehaviour
    {
        public static EnvironmentMeter Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private float maxValue = 100f;
        [SerializeField] private float startValue = 50f;
        [SerializeField] private float collapseThreshold = 20f;
        [SerializeField] private float restorationThreshold = 80f;

        public float CurrentValue { get; private set; }
        public float NormalizedValue => CurrentValue / maxValue;

        [Header("Events")]
        public UnityEvent<float> OnValueChanged;
        public UnityEvent OnEcosystemCollapse;
        public UnityEvent OnEcosystemRestored;

        [Header("Progress Bar")]
        [SerializeField] private GameObject progressFill;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            CurrentValue = startValue;
        }

        public void Adjust(float delta)
        {
            CurrentValue = Mathf.Clamp(CurrentValue + delta, 0f, maxValue);
            OnValueChanged?.Invoke(CurrentValue);

            if (CurrentValue <= collapseThreshold)
                OnEcosystemCollapse?.Invoke();
            else if (CurrentValue >= restorationThreshold)
                OnEcosystemRestored?.Invoke();
        }

        public void GetCurrentFill() {
            if (progressFill != null)
            {
                progressFill.GetComponent<Image>().fillAmount = NormalizedValue;
            }
        }

        public void ResetMeter() {
            CurrentValue = startValue;
            OnValueChanged?.Invoke(CurrentValue);
        }

        void Update() {
            GetCurrentFill();
        }
    }
}
