using UnityEngine;
 
namespace WhereFirefliesReturn.Resources
{
    public enum ItemCategory { Seed, Tool, Resource }
 
    /// <summary>
    /// Create one of these per item via:
    /// Right-click in Project → Create → WhereFirefliesReturn → Item Data
    /// 
    /// Examples to create:
    ///   Seeds:     SweetPotato_Seed, Corn_Seed, WaterSpinach_Seed, Marigold_Seed
    ///   Tools:     Shovel, WateringCan
    ///   Resources: Water, CleanEnergy
    /// </summary>
    [CreateAssetMenu(fileName = "NewItem", menuName = "WhereFirefliesReturn/Item Data")]
    public class ItemData : ScriptableObject
    {
        [Header("Identity")]
        public string itemName;
        public ItemCategory category;
        public Sprite icon;
 
        [TextArea] public string description;
 
        [Header("Stack Settings")]
        /// Tools are never stackable. Seeds and Resources are.
        public bool isStackable = true;
        public int maxStack = 99;
 
        [Header("Seed Settings — only used if category == Seed")]
        public CropType cropType; // links back to companion logic
 
        [Header("Resource Settings — only used if category == Resource")]
        /// Maps this item to the ResourceManager field it affects.
        public ResourceField resourceField;
    }
 
    /// Which ResourceManager field this resource item maps to
    public enum ResourceField { None, Water, Seeds, CleanEnergy }
}
