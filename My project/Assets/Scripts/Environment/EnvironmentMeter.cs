using UnityEngine;

namespace WhereFirefliesReturn.Environment
{
    public class EnvironmentMeter : MonoBehaviour
    {
        public static EnvironmentMeter Instance { get; private set; }

        [Header("Thresholds")]
        [SerializeField] float startValue = 50f;
        [SerializeField] float collapseThreshold = 20f;
        [SerializeField] float restorationThreshold = 80f;

        public float Value { get; private set; }
        public float NormalizedValue => Value / 100f;

        public event System.Action<float> OnValueChanged;
        public event System.Action OnCollapse;
        public event System.Action OnRestoration;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            Value = startValue;
        }

        public void Modify(float delta)
        {
            float prev = Value;
            Value = Mathf.Clamp(Value + delta, 0f, 100f);

            if (Mathf.Approximately(prev, Value)) return;

            OnValueChanged?.Invoke(Value);

            if (Value <= collapseThreshold) OnCollapse?.Invoke();
            else if (Value >= restorationThreshold) OnRestoration?.Invoke();
        }
    }
}
