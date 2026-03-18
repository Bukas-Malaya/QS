using UnityEngine;

namespace WhereFirefliesReturn.Farm
{
    public class CycleManager : MonoBehaviour
    {
        public static CycleManager Instance { get; private set; }

        [SerializeField] int totalCycles = 5;

        public int CurrentCycle { get; private set; } = 1;
        public int TotalCycles => totalCycles;
        public bool IsLastCycle => CurrentCycle >= totalCycles;

        public event System.Action<int> OnCycleStart;
        public event System.Action<int> OnCycleComplete;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        void Start() => OnCycleStart?.Invoke(CurrentCycle);

        public void CompleteCurrentCycle()
        {
            OnCycleComplete?.Invoke(CurrentCycle);

            if (IsLastCycle)
            {
                Core.GameManager.Instance?.EndGame();
                return;
            }

            CurrentCycle++;
            OnCycleStart?.Invoke(CurrentCycle);
        }
    }
}
