using UnityEngine;
using UnityEngine.Events;

public class ScrapPickup : MonoBehaviour
{
    [SerializeField] private UnityEvent onPickup;

    public void Pickup()
    {
        // Fire any additional events (like updating inventory or playing a sound)
        if (onPickup != null)
            onPickup.Invoke();

        // Destroy the scrap object.
        Destroy(gameObject);
    }
}