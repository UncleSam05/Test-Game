using UnityEngine;

public class RepulsionPickup : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Look for the RepulsionAbility component in the player (or its children).
            RepulsionAbility repulsionAbility = other.GetComponentInChildren<RepulsionAbility>();
            if (repulsionAbility != null)
            {
                repulsionAbility.ActivateRepulsionPowerUp();
            }
            Destroy(gameObject);
        }
    }
}