using UnityEngine;
using UnityEngine.Events;

public class ScrapPickup : MonoBehaviour
{
    [SerializeField] private UnityEvent onPickup;

    public void Pickup()
    {
        // Check if InventoryManager exists and if there's room.
        if (InventoryManager.Instance != null && !InventoryManager.Instance.IsFull())
        {
            bool added = InventoryManager.Instance.AddScrap();
            if (added)
            {
                // Optionally invoke any extra events (sound, score update, etc.)
                if (onPickup != null)
                    onPickup.Invoke();
                // Destroy the scrap object after picking it up.
                Destroy(gameObject);
            }
        }
        else
        {
            // Optionally, you can give feedback that the inventory is full.
            Debug.Log("Inventory is full!");
        }
    }
}