using UnityEngine;

namespace WhereFirefliesReturn.Farm
{
    [System.Serializable]
    public class Decision
    {
        public string label;
        [TextArea] public string description;
        public float environmentImpact;
        public Resources.ResourceType resourceCost;
        public int resourceCostAmount;
    }

    public class DecisionSystem : MonoBehaviour
    {
        public static DecisionSystem Instance { get; private set; }

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public bool Apply(Decision decision)
        {
            var rm = Resources.ResourceManager.Instance;
            if (rm == null) return false;

            if (decision.resourceCostAmount > 0 && !rm.Spend(decision.resourceCost, decision.resourceCostAmount))
            {
                Debug.Log($"Not enough {decision.resourceCost} for decision: {decision.label}");
                return false;
            }

            Environment.EnvironmentMeter.Instance?.Modify(decision.environmentImpact);
            CycleManager.Instance?.CompleteCurrentCycle();
            return true;
        }
    }
}
