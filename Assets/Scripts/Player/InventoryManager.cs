
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace WhereFirefliesReturn.Resources
{
    /// <summary>
    /// Single source of truth for what the player is carrying.
    /// - Pickups call InventoryManager.Instance.Add(itemData, amount)
    /// - Hotbar reads from this and displays slots
    /// - ResourceManager stays in sync for Water/Seeds/CleanEnergy
    /// 
    /// No setup needed beyond placing this on a persistent GameObject.
    /// </summary>
    public class InventoryManager : MonoBehaviour
    {
        
        public static InventoryManager Instance { get; private set; }
        [System.Serializable]
        
        public class ItemStack
        {
            public ItemData data;
            public int quantity; // always 1 for tools

            public ItemStack(ItemData data, int qty)
            {
                this.data = data;
                this.quantity = data.isStackable ? Mathf.Clamp(qty, 1, data.maxStack) : 1;
            }
        }


        // All stacks the player currently holds
        private List<ItemStack> stacks = new();

        public IReadOnlyList<ItemStack> Stacks => stacks;

        // Fired whenever inventory changes — hotbar listens to this
        public UnityEvent OnInventoryChanged = new();

        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            if (transform.parent == null)
            {
                DontDestroyOnLoad(gameObject);
            }
            
        }
        void Start()
        {
        }

        /// <summary>Add an item (e.g. from a pickup). Auto-stacks if stackable.</summary>
        public void Add(ItemData item, int amount = 1)
        {
            if (item == null) return;

            if (item.isStackable)
            {
                var existing = stacks.Find(s => s.data == item);
                if (existing != null)
                {
                    existing.quantity = Mathf.Min(existing.quantity + amount, item.maxStack);
                }
                else
                {
                    stacks.Add(new ItemStack(item, amount));
                }
            }
            else
            {
                Debug.Log($"Adding non-stackable item: {item.itemName}");
                if (!stacks.Exists(s => s.data == item))
                    stacks.Add(new ItemStack(item, 1));
            }
            Debug.Log($"[Inventory] Added {item.itemName} x{amount}");
            SyncToResourceManager(item, amount);
            OnInventoryChanged?.Invoke();
        }
        public bool Spend(ItemData item, int amount = 1)
        {
            if (item == null) return false;

            var stack = stacks.Find(s => s.data == item);
            if (stack == null || stack.quantity < amount) return false;

            stack.quantity -= amount;
            if (stack.quantity <= 0) stacks.Remove(stack);

            SyncToResourceManager(item, -amount);
            OnInventoryChanged?.Invoke();
            return true;
        }

        public bool Has(ItemData item, int amount = 1)
        {
            var stack = stacks.Find(s => s.data == item);
            return stack != null && stack.quantity >= amount;
        }

        public int GetQuantity(ItemData item)
        {
            var stack = stacks.Find(s => s.data == item);
            return stack?.quantity ?? 0;
        }

        /// <summary>Used by farm_bed: find a seed item by CropType.</summary>
        public ItemStack GetSeedForCrop(CropType cropType)
        {
            return stacks.Find(s =>
                s.data.category == ItemCategory.Seed &&
                s.data.cropType == cropType &&
                s.quantity > 0);
        }
        // Keep ResourceManager's Water/Seeds/CleanEnergy in sync.
        void SyncToResourceManager(ItemData item, int delta)
        {
            if (item.category != ItemCategory.Resource) return;
            if (ResourceManager.Instance == null) return;

            switch (item.resourceField)
            {
                case ResourceField.Water:
                    if (delta > 0) ResourceManager.Instance.Collect(water: delta);
                    else ResourceManager.Instance.Spend(water: -delta, seeds: 0, cleanEnergy: 0);
                    break;
                case ResourceField.Seeds:
                    if (delta > 0) ResourceManager.Instance.Collect(seeds: delta);
                    else ResourceManager.Instance.Spend(water: 0, seeds: -delta, cleanEnergy: 0);
                    break;
                case ResourceField.CleanEnergy:
                    if (delta > 0) ResourceManager.Instance.Collect(cleanEnergy: delta);
                    else ResourceManager.Instance.Spend(water: 0, seeds: 0, cleanEnergy: -delta);
                    break;
            }
        }
    }
}