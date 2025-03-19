using System.Collections;
using UnityEngine;

public class RepulsionAbility : MonoBehaviour
{
    [Header("Repulsion Settings")]
    [SerializeField] private float repulsionRadius = 5f;
    [SerializeField] private float repulsionForce = 500f;
    [SerializeField] private float repulsionDuration = 0.5f; // How long the force is applied.

    [SerializeField] private float powerUpDuration = 10f; // How long the repulsion ability remains active.

    [Header("Effects")]
    [SerializeField] private GameObject repulsionEffectPrefab; // Visual effect prefab.

    [Header("UI")]
    [SerializeField] private GameObject repulsionUIIndicator; // The UI element indicating the repulsion power-up is active.

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

        // Count down the active power-up timer.
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
        // Instantiate the visual effect at the player's position.
        if (repulsionEffectPrefab != null)
        {
            Instantiate(repulsionEffectPrefab, transform.position, Quaternion.identity);
        }

        // Get all colliders in the repulsion radius.
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, repulsionRadius);
        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                Rigidbody2D enemyRb = hit.GetComponent<Rigidbody2D>();
                if (enemyRb != null)
                {
                    Vector2 direction = (enemyRb.position - (Vector2)transform.position).normalized;
                    // Start coroutine to apply repulsion force over time.
                    StartCoroutine(ApplyRepulsionForce(enemyRb, direction));
                }
            }
        }
        // Optionally, deactivate the repulsion power-up after use.
        DeactivateRepulsion();
    }

    private IEnumerator ApplyRepulsionForce(Rigidbody2D enemyRb, Vector2 direction)
    {
        float timer = repulsionDuration;
        // Loop for the duration, applying a small amount of force each fixed update.
        while (timer > 0)
        {
            enemyRb.AddForce(direction * repulsionForce * Time.fixedDeltaTime, ForceMode2D.Force);
            timer -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, repulsionRadius);
    }
}