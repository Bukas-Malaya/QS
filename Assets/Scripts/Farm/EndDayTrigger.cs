using UnityEngine;
using WhereFirefliesReturn.UI;

namespace WhereFirefliesReturn.Farm
{
    /// <summary>
    /// Place this on a trigger collider (e.g. barn entrance).
    /// When the player walks in, it opens the DecisionPanelUI for the current cycle.
    /// </summary>
    public class EndDayTrigger : MonoBehaviour
    {
        [Tooltip("Optional prompt shown above the trigger zone")]
        [SerializeField] private string promptText = "Enter to end the day";

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            if (DecisionPanelUI.Instance != null)
                DecisionPanelUI.Instance.Show();
            else
                Debug.LogWarning("[EndDayTrigger] DecisionPanelUI.Instance not found in scene.");
        }
    }
}
