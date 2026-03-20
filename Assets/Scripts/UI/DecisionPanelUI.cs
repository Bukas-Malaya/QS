using UnityEngine;
using UnityEngine.UI;
using TMPro;
using WhereFirefliesReturn.Farm;
using WhereFirefliesReturn.Resources;

namespace WhereFirefliesReturn.UI
{
    [System.Serializable]
    public class CycleDecisions
    {
        public Decision[] decisions;
    }

    public class DecisionPanelUI : MonoBehaviour
    {
        public static DecisionPanelUI Instance { get; private set; }

        [Header("Decisions Per Cycle")]
        [Tooltip("Index 0 = Cycle 1, Index 1 = Cycle 2, etc.")]
        [SerializeField] private CycleDecisions[] cycleDecisions;

        [Header("Panel")]
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI cyclePromptText;

        [Header("Decision Buttons (assign 2-3 in Inspector)")]
        [SerializeField] private Button[] decisionButtons;
        [SerializeField] private TextMeshProUGUI[] buttonLabels;
        [SerializeField] private TextMeshProUGUI[] buttonCostLabels;

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
            if (CycleManager.Instance != null)
                CycleManager.Instance.OnCycleStarted.AddListener(OnCycleStarted);
        }

        void OnDisable()
        {
            if (CycleManager.Instance != null)
                CycleManager.Instance.OnCycleStarted.RemoveListener(OnCycleStarted);
        }

        void Start()
        {
            Hide();
        }

        void OnCycleStarted(int cycle)
        {
            Hide();
        }

        public void Show()
        {
            int cycleIndex = (CycleManager.Instance?.CurrentCycle ?? 1) - 1;

            if (cycleDecisions == null || cycleIndex >= cycleDecisions.Length)
            {
                Debug.LogWarning($"No decisions configured for cycle {cycleIndex + 1}.");
                return;
            }

            Decision[] decisions = cycleDecisions[cycleIndex].decisions;

            if (cyclePromptText != null)
                cyclePromptText.text = $"Cycle {cycleIndex + 1} — Choose your action:";

            for (int i = 0; i < decisionButtons.Length; i++)
            {
                bool hasDecision = i < decisions.Length;
                decisionButtons[i].gameObject.SetActive(hasDecision);

                if (!hasDecision) continue;

                Decision d = decisions[i];

                if (buttonLabels != null && i < buttonLabels.Length)
                    buttonLabels[i].text = d.label;

                if (buttonCostLabels != null && i < buttonCostLabels.Length)
                    buttonCostLabels[i].text = BuildCostString(d.cost);

                bool canAfford = ResourceManager.Instance?.CanAfford(
                    d.cost.water, d.cost.seeds, d.cost.cleanEnergy) ?? true;

                decisionButtons[i].interactable = canAfford;

                int captured = i;
                decisionButtons[i].onClick.RemoveAllListeners();
                decisionButtons[i].onClick.AddListener(() => OnDecisionChosen(decisions[captured]));
            }

            panel?.SetActive(true);
        }

        void OnDecisionChosen(Decision decision)
        {
            DecisionSystem.Instance?.MakeDecision(decision);
            Hide();
        }

        void Hide()
        {
            panel?.SetActive(false);
        }

        string BuildCostString(ResourceCost cost)
        {
            var parts = new System.Collections.Generic.List<string>();
            if (cost.water > 0)       parts.Add($"Water x{cost.water}");
            if (cost.seeds > 0)       parts.Add($"Seeds x{cost.seeds}");
            if (cost.cleanEnergy > 0) parts.Add($"Energy x{cost.cleanEnergy}");
            return parts.Count > 0 ? string.Join("  ", parts) : "Free";
        }
    }
}
