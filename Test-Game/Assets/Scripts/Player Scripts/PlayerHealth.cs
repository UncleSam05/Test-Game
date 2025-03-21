using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;

    [Header("Knockback Settings")]
    [SerializeField] private float knockbackForce = 500f;    // Total force applied over the duration.
    [SerializeField] private float knockbackDuration = 0.5f;   // Duration over which knockback is applied.

    [Header("Audio Settings")]
    [SerializeField] private AudioClip hitSound;             // Sound to play on hit.
    [SerializeField] private AudioSource audioSource;        // AudioSource component.

    private int currentHealth;
    private Rigidbody2D rb;

    private void Awake()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            currentHealth--;
            Debug.Log("Player hit! Health: " + currentHealth);

            if (audioSource != null && hitSound != null)
                audioSource.PlayOneShot(hitSound);

            Vector2 knockbackDirection = (rb.position - (Vector2)collision.transform.position).normalized;
            StartCoroutine(ApplyKnockback(knockbackDirection));

            if (currentHealth <= 0)
                Die();
        }
    }

    private IEnumerator ApplyKnockback(Vector2 direction)
    {
        float timer = 0f;
        while (timer < knockbackDuration)
        {
            rb.AddForce(direction * knockbackForce * Time.fixedDeltaTime, ForceMode2D.Force);
            timer += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
    }

    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth += amount; // Also heal the player for the upgrade.
        Debug.Log("Max Health increased to: " + maxHealth);
    }

    private void Die()
    {
        Debug.Log("Player died!");
        gameObject.SetActive(false);
    }
}
