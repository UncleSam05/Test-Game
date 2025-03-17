using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private GameInput gameInput;      // Reference to your GameInput component.
    [SerializeField] private float interactRadius = 2f;   // Radius within which scrap can be picked up.
    [SerializeField] private LayerMask scrapLayer;        // (Optional) Layer mask for scrap objects.

    void Update()
    {
        // Check if the interact action (E key) is pressed.
        if (gameInput.InteractPressed)
        {
            // Check for scrap objects within the interact radius.
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, interactRadius, scrapLayer);
            foreach (Collider2D hit in hits)
            {
                ScrapPickup scrap = hit.GetComponent<ScrapPickup>();
                if (scrap != null)
                {
                    scrap.Pickup();
                    break; // Pick up only one scrap per press.
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize the interact radius in the Scene view.
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
