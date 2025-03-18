using UnityEngine;

public class RepulsionAbility : MonoBehaviour
{
    [Header("Repulsion Settings")]
    [SerializeField] private float repulsionRadius = 5f;
    [SerializeField] private float repulsionForce = 500f;
    [SerializeField] private float powerUpDuration = 10f; // Duration the repulsion ability remains active.

    [Header("Effects")]
    [SerializeField] private GameObject repulsionEffectPrefab; // Visual effect prefab.

    [Header("UI")]
    [SerializeField] private GameObject repulsionUIIndicator; // The UI element that indicates the repulsion power-up is active.

    // Reference to the GameInput component (assign via Inspector or find at runtime).
    [SerializeField] private GameInput gameInput;

    private bool hasRepulsion = false;
    private float repulsionTimer = 0f;

    private void Update()
    {
        if (hasRepulsion && gameInput.RepulsionPressed)
        {
            UseRepulsion();
        }

        // If active, count down the power-up duration.
        if (hasRepulsion)
        {
            repulsionTimer -= Time.deltaTime;
            if (repulsionTimer <= 0)
            {
                DeactivateRepulsion();
            }
        }
    }

    public void ActivateRepulsionPowerUp()
    {
        hasRepulsion = true;
        repulsionTimer = powerUpDuration;
        if (repulsionUIIndicator != null)
            repulsionUIIndicator.SetActive(true);
    }

    private void DeactivateRepulsion()
    {
        hasRepulsion = false;
        if (repulsionUIIndicator != null)
            repulsionUIIndicator.SetActive(false);
    }

    private void UseRepulsion()
    {
        // Spawn the repulsion visual effect at the player's position.
        if (repulsionEffectPrefab != null)
        {
            Instantiate(repulsionEffectPrefab, transform.position, Quaternion.identity);
        }

        // Use OverlapCircleAll to find all colliders in the repulsion radius.
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, repulsionRadius);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                Rigidbody2D enemyRb = hit.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    // Apply a force away from the player.
                    Vector2 direction = (enemyRb.position - (Vector2)transform.position).normalized;
                    enemyRb.AddForce(direction * repulsionForce);
                }
            }
        }
        // Optionally, consume the repulsion power-up immediately after use.
        DeactivateRepulsion();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, repulsionRadius);
    }
}
