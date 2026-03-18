using UnityEngine;

namespace WhereFirefliesReturn.Resources
{
    public enum ResourceType { Water, Seeds, CleanEnergy }

    public class ResourceManager : MonoBehaviour
    {
        public static ResourceManager Instance { get; private set; }

        public int Water { get; private set; }
        public int Seeds { get; private set; }
        public int CleanEnergy { get; private set; }

        public event System.Action<ResourceType, int> OnResourceChanged;

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }

        public void Collect(ResourceType type, int amount)
        {
            Add(type, amount);
        }

        public bool Spend(ResourceType type, int amount)
        {
            if (Get(type) < amount) return false;
            Add(type, -amount);
            return true;
        }

        public int Get(ResourceType type) => type switch
        {
            ResourceType.Water => Water,
            ResourceType.Seeds => Seeds,
            ResourceType.CleanEnergy => CleanEnergy,
            _ => 0
        };

        void Add(ResourceType type, int delta)
        {
            switch (type)
            {
                case ResourceType.Water: Water = Mathf.Max(0, Water + delta); break;
                case ResourceType.Seeds: Seeds = Mathf.Max(0, Seeds + delta); break;
                case ResourceType.CleanEnergy: CleanEnergy = Mathf.Max(0, CleanEnergy + delta); break;
            }
            OnResourceChanged?.Invoke(type, Get(type));
        }
    }
}
