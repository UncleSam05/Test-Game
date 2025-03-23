using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    private enum EnemyState { Roam, Chase }
    private EnemyState currentState = EnemyState.Roam;

    [Header("Player Settings")]
    [SerializeField] private Transform playerTransform; // Reference to the player's transform.
    [SerializeField] private float chaseRange = 5f;       // Distance within which the enemy chases the player.
    [SerializeField] private float chaseSpeed = 3f;       // Movement speed when chasing.

    [Header("Roam Settings")]
    [SerializeField] private float roamSpeed = 2f;        // Movement speed when roaming.
    [SerializeField] private Vector2 roamAreaMin = new Vector2(-5f, -5f); // Bottom-left corner of roam area.
    [SerializeField] private Vector2 roamAreaMax = new Vector2(5f, 5f);    // Top-right corner of roam area.
    [SerializeField] private float newTargetThreshold = 0.1f; // When close to current roam target, pick a new one.

    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 5f;      // Smoothing factor for rotation.
    [SerializeField] private float maxTiltAngle = 30f;      // Maximum tilt (in degrees) for the sprite.
    [SerializeField] private float tiltFactor = 30f;        // Factor to convert vertical difference to tilt angle.

    private Vector2 roamTarget; // The current roaming destination.
    private Rigidbody2D rb;
    private Vector3 originalScale;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        originalScale = transform.localScale;
    }

    private void Start()
    {
        PickNewRoamTarget();
    }

    private void FixedUpdate()
    {
        // Determine state: chase if player is within chaseRange; otherwise roam.
        float distanceToPlayer = Vector2.Distance(rb.position, playerTransform.position);
        currentState = (distanceToPlayer <= chaseRange) ? EnemyState.Chase : EnemyState.Roam;

        if (currentState == EnemyState.Chase)
            ChasePlayer();
        else
            Roam();
    }

    private void ChasePlayer()
    {
        // Calculate direction from enemy to player.
        Vector2 direction = ((Vector2)playerTransform.position - rb.position).normalized;
        rb.MovePosition(rb.position + direction * chaseSpeed * Time.fixedDeltaTime);

        // Flip sprite based on horizontal direction.
        if (direction.x >= 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
        else
        {
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }

        // Calculate tilt angle based on vertical component.
        float tiltAngle = Mathf.Clamp(direction.y * tiltFactor, -maxTiltAngle, maxTiltAngle);
        // Invert tilt if enemy is facing left.
        if (direction.x < 0)
        {
            tiltAngle = -tiltAngle;
        }
        float newRotation = Mathf.LerpAngle(rb.rotation, tiltAngle, rotationSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(newRotation);
    }

    private void Roam()
    {
        // Calculate direction toward roam target.
        Vector2 direction = roamTarget - rb.position;
        if (direction.magnitude < newTargetThreshold)
        {
            PickNewRoamTarget();
            direction = roamTarget - rb.position;
        }
        direction.Normalize();
        rb.MovePosition(rb.position + direction * roamSpeed * Time.fixedDeltaTime);

        // Flip sprite based on horizontal direction.
        if (direction.x >= 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
        else
        {
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }

        // Calculate tilt angle based on vertical component.
        float tiltAngle = Mathf.Clamp(direction.y * tiltFactor, -maxTiltAngle, maxTiltAngle);
        if (direction.x < 0)
        {
            tiltAngle = -tiltAngle;
        }
        float newRotation = Mathf.LerpAngle(rb.rotation, tiltAngle, rotationSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(newRotation);
    }

    private void PickNewRoamTarget()
    {
        float randomX = Random.Range(roamAreaMin.x, roamAreaMax.x);
        float randomY = Random.Range(roamAreaMin.y, roamAreaMax.y);
        roamTarget = new Vector2(randomX, randomY);
    }
}