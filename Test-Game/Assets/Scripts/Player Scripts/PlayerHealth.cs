using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackForce = 500f;    // Total force magnitude applied over the duration.
    [SerializeField] private float knockbackDuration = 0.5f;   // Duration over which knockback is applied.

    [Header("Audio Settings")]
    [SerializeField] private AudioClip hitSound;             // Sound effect for when the player is hit.
    [SerializeField] private AudioSource audioSource;        // AudioSource component to play the sound.

    private int currentHealth;
    private Rigidbody2D rb;

    private void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();

        // If you don't assign an AudioSource in the Inspector, try to get one from the GameObject.
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player collided with an enemy.
        if (collision.gameObject.CompareTag("Enemy"))
        {
            currentHealth--;
            Debug.Log("Player hit! Health: " + currentHealth);

            // Play the hit sound.
            if (audioSource != null && hitSound != null)
            {
                audioSource.PlayOneShot(hitSound);
            }

            // Calculate knockback direction (away from the enemy).
            Vector2 knockbackDirection = (rb.position - (Vector2)collision.transform.position).normalized;
            StartCoroutine(ApplyKnockback(knockbackDirection));

            // If health reaches zero or less, handle player death.
            if (currentHealth <= 0)
            {
                Die();
            }
        }
    }

    private IEnumerator ApplyKnockback(Vector2 direction)
    {
        float timer = 0f;
        while (timer < knockbackDuration)
        {
            Vector2 forceToApply = direction * knockbackForce * Time.fixedDeltaTime;
            rb.AddForce(forceToApply, ForceMode2D.Force);
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    private void Die()
    {
        Debug.Log("Player died!");
        
        gameObject.SetActive(false);
    }
}

