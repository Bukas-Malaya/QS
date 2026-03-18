using UnityEngine;
using UnityEngine.Events;

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
    }
}
