using UnityEngine;
using UnityEngine.Events;

namespace WhereFirefliesReturn.Resources
{
    public class ResourceManager : MonoBehaviour
    {
        public static ResourceManager Instance { get; private set; }

        [Header("Starting Resources")]
        [SerializeField] private int startWater = 5;
        [SerializeField] private int startSeeds = 5;
        [SerializeField] private int startCleanEnergy = 3;

        public int Water { get; private set; }
        public int Seeds { get; private set; }
        public int CleanEnergy { get; private set; }

        public UnityEvent OnResourcesChanged;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            Water = startWater;
            Seeds = startSeeds;
            CleanEnergy = startCleanEnergy;
        }

        public void Collect(int water = 0, int seeds = 0, int cleanEnergy = 0)
        {
            Water += water;
            Seeds += seeds;
            CleanEnergy += cleanEnergy;
            OnResourcesChanged?.Invoke();
        }

        public bool CanAfford(int water, int seeds, int cleanEnergy)
        {
            return Water >= water && Seeds >= seeds && CleanEnergy >= cleanEnergy;
        }

        public void Spend(int water, int seeds, int cleanEnergy)
        {
            if (!CanAfford(water, seeds, cleanEnergy))
            {
                Debug.LogWarning("Not enough resources to spend.");
                return;
            }
            Water -= water;
            Seeds -= seeds;
            CleanEnergy -= cleanEnergy;
            OnResourcesChanged?.Invoke();
        }
    }
}
