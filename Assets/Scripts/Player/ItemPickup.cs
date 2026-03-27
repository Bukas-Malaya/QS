using UnityEngine;

namespace WhereFirefliesReturn.Resources
{
    /// <summary>
    /// Place on any world pickup object (seed bag, tool, water canteen, etc.)
    /// Player walks into trigger → item added to InventoryManager → hotbar updates.
    /// 
    /// Setup:
    ///   - Add a Collider with Is Trigger = true
    ///   - Assign itemData (your ScriptableObject)
    ///   - Set quantity (ignored for Tools)
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ItemPickup : MonoBehaviour
    {
        [SerializeField] private ItemData itemData;
        [SerializeField] private int quantity = 1;

        [Header("Optional")]
        [SerializeField] private GameObject pickupVFX;
        [SerializeField] private AudioClip pickupSound;

        private bool pickedUp = false;

        void OnTriggerEnter(Collider other)
        {
            if (pickedUp) return;
            if (!other.CompareTag("Player")) return;

            pickedUp = true;
            InventoryManager.Instance?.Add(itemData, quantity);

            if (pickupVFX != null)
                Instantiate(pickupVFX, transform.position, Quaternion.identity);

            if (pickupSound != null)
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);

            Destroy(gameObject);
        }
    }
}