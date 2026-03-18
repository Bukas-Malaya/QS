using UnityEngine;
using UnityEngine.Events;

namespace WhereFirefliesReturn.Farm
{
    [System.Serializable]
    public class Decision
    {
        public string label;
        [TextArea] public string description;
        public float environmentImpact;
        public ResourceCost cost;
    }

    [System.Serializable]
    public class ResourceCost
    {
        public int water;
        public int seeds;
        public int cleanEnergy;
    }

    public class DecisionSystem : MonoBehaviour
    {
        public static DecisionSystem Instance { get; private set; }

        public UnityEvent<Decision> OnDecisionMade;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        public void MakeDecision(Decision decision)
        {
            // Apply environment impact
            Environment.EnvironmentMeter.Instance?.Adjust(decision.environmentImpact);

            // Deduct resources
            ResourceManager.Instance?.Spend(
                decision.cost.water,
                decision.cost.seeds,
                decision.cost.cleanEnergy
            );

            OnDecisionMade?.Invoke(decision);

            // Advance game cycle after decision
            CycleManager.Instance?.AdvanceCycle();
        }
    }
}
