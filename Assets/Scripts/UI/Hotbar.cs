using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


namespace WhereFirefliesReturn.Resources
{
    public class Hotbar : MonoBehaviour
    {
        public static Hotbar Instance { get; private set; }
 
        [System.Serializable]
        public class HotbarSlot
        {
            public GameObject root;
            public Image icon;
            public TextMeshProUGUI countLabel;
            public GameObject selectedHighlight;
            [HideInInspector] public ItemData boundItem;
        }
 
        [SerializeField] private HotbarSlot[] slots;
        public ItemData SelectedItem { get; private set; }
        public CropType SelectedCrop => SelectedItem != null && SelectedItem.category == ItemCategory.Seed
            ? SelectedItem.cropType : CropType.None;
 
        private int selectedIndex = -1;
 
        void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
        }
        void OnEnable()  { if (InventoryManager.Instance != null) InventoryManager.Instance.OnInventoryChanged.AddListener(Refresh); }
        void OnDisable() { 
            if (InventoryManager.Instance != null) 
            InventoryManager.Instance.OnInventoryChanged.RemoveListener(Refresh); 
            }
        void Start()
        {
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.OnInventoryChanged.AddListener(Refresh);
            }
             Refresh();
        }
 
        void Update()
        {
            if (Input.anyKeyDown)
                Debug.Log("Key pressed: " + Input.inputString);

            
            for (int i = 0; i < slots.Length && i < 9; i++)
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    SelectSlot(i);
                    Debug.Log("Selected slot: " + (SelectedItem != null ? SelectedItem.name : "None"));
                }
        }
 
        void Refresh()
        {
            if (InventoryManager.Instance == null) return;
 
            var slottable = new List<InventoryManager.ItemStack>();
            foreach (var stack in InventoryManager.Instance.Stacks)
                if (stack.data.category == ItemCategory.Seed || stack.data.category == ItemCategory.Tool)
                    slottable.Add(stack);
 
            ItemData previousSelected = SelectedItem;
 
            for (int i = 0; i < slots.Length; i++)
            {
                if (i < slottable.Count) BindSlot(slots[i], slottable[i]);
                else ClearSlot(slots[i]);
            }
 
            bool reselected = false;
            if (previousSelected != null)
                for (int i = 0; i < slots.Length; i++)
                    if (slots[i].boundItem == previousSelected) { SelectSlot(i); reselected = true; break; }
 
            if (!reselected)
                for (int i = 0; i < slots.Length; i++)
                    if (slots[i].boundItem != null) { SelectSlot(i); break; }
        }
 
        void BindSlot(HotbarSlot slot, InventoryManager.ItemStack stack)
        {
            slot.boundItem = stack.data;
            if (slot.icon != null)
            {
                slot.icon.sprite = stack.data.icon;
                slot.icon.enabled = true; // always show, even without sprite
            }
            if (slot.countLabel != null)
            {
                slot.countLabel.text = stack.data.isStackable ? stack.quantity.ToString() : "";
                slot.countLabel.gameObject.SetActive(stack.data.isStackable);
            }
        }
 
        void ClearSlot(HotbarSlot slot)
        {
            slot.boundItem = null;
            if (slot.icon != null) { slot.icon.sprite = null; slot.icon.enabled = false; }
            if (slot.countLabel != null) slot.countLabel.gameObject.SetActive(false);
            if (slot.selectedHighlight != null) slot.selectedHighlight.SetActive(false);
            
        }
 
        public void SelectSlot(int index)
        {
            if (index < 0 || index >= slots.Length || slots[index].boundItem == null) return;
            selectedIndex = index;
            SelectedItem = slots[index].boundItem;
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].selectedHighlight != null) slots[i].selectedHighlight.SetActive(i == index);
            }
        }
 
        public bool TrySpendSelected()
        {
            if (SelectedItem == null) return false;
            return InventoryManager.Instance.Spend(SelectedItem, 1);
        }
        public bool HasSelected() => SelectedItem != null && InventoryManager.Instance.Has(SelectedItem);
    }
}