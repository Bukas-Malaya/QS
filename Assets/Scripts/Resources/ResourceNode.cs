using UnityEngine;

namespace WhereFirefliesReturn.Resources
{
    public class ResourceNode : MonoBehaviour
    {
        [Header("Yield")]
        [SerializeField] private int water = 0;
        [SerializeField] private int seeds = 0;
        [SerializeField] private int cleanEnergy = 0;

        [Header("Interaction")]
        [SerializeField] private string promptText = "Press E to collect";

        public virtual bool IsCollected { get; protected set; }
        public string PromptText { get => promptText; set => promptText = value; }

        public virtual void Plant() { }

        public virtual void Collect()
        {
            if (IsCollected) return;

            IsCollected = true;
            ResourceManager.Instance?.Collect(water, seeds, cleanEnergy);
            gameObject.SetActive(false);

            //debug log for testing purposes
            Debug.Log("Resource collected: " + water + " water, " + seeds + " seeds, " + cleanEnergy + " clean energy.");
        }
    }
}
