using UnityEngine;
using UnityEngine.Events;

namespace WhereFirefliesReturn.Farm
{
    public class CycleManager : MonoBehaviour
    {
        public static CycleManager Instance { get; private set; }

        [Header("Settings")]
        [SerializeField] private int totalCycles = 5;

        public int CurrentCycle { get; private set; } = 1;
        public bool IsLastCycle => CurrentCycle >= totalCycles;

        public UnityEvent<int> OnCycleStarted;
        public UnityEvent OnAllCyclesComplete;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        void Start()
        {
            OnCycleStarted?.Invoke(CurrentCycle);
        }

        public void AdvanceCycle()
        {
            if (IsLastCycle)
            {
                OnAllCyclesComplete?.Invoke();
                return;
            }

            CurrentCycle++;
            OnCycleStarted?.Invoke(CurrentCycle);
        }
    }
}
