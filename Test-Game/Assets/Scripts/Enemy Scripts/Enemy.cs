using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    private enum EnemyState { Roam, Chase }
    private EnemyState currentState = EnemyState.Roam;

    [Header("Player Settings")]
    [SerializeField] private Transform playerTransform; // Assign the player's transform in the Inspector.
    [SerializeField] private float chaseRange = 5f;       // When the player is within this distance, chase them.
    [SerializeField] private float chaseSpeed = 3f;       // Speed when chasing the player.

    [Header("Roam Settings")]
    [SerializeField] private float roamSpeed = 2f;        // Speed when roaming.
    [SerializeField] private Vector2 roamAreaMin = new Vector2(-5f, -5f); // Bottom-left corner of roam area.
    [SerializeField] private Vector2 roamAreaMax = new Vector2(5f, 5f);    // Top-right corner of roam area.
    [SerializeField] private float newTargetThreshold = 0.1f; // When close enough to the roam target, pick a new one.

    [Header("Rotation Smoothing")]
    [SerializeField] private float rotationSpeed = 5f;    // How fast the enemy rotates to face the target.

    private Vector2 roamTarget;   // The current roam destination.
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        PickNewRoamTarget();
    }

    private void FixedUpdate()
    {
        // Determine state based on the player's distance.
        float distanceToPlayer = Vector2.Distance(rb.position, playerTransform.position);
        if (distanceToPlayer <= chaseRange)
            currentState = EnemyState.Chase;
        else
            currentState = EnemyState.Roam;

        if (currentState == EnemyState.Chase)
            ChasePlayer();
        else
            Roam();
    }

    private void ChasePlayer()
    {
        // Compute direction to the player.
        Vector2 direction = ((Vector2)playerTransform.position - rb.position).normalized;
        // Move towards the player.
        rb.MovePosition(rb.position + direction * chaseSpeed * Time.fixedDeltaTime);

        // Smoothly rotate towards the player.
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float smoothAngle = Mathf.LerpAngle(rb.rotation, targetAngle, rotationSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(smoothAngle);
    }

    private void Roam()
    {
        // Calculate direction toward the roam target.
        Vector2 direction = roamTarget - rb.position;
        // If the enemy is close enough to its roam target, pick a new one.
        if (direction.magnitude < newTargetThreshold)
        {
            PickNewRoamTarget();
            direction = roamTarget - rb.position;
        }
        direction.Normalize();
        rb.MovePosition(rb.position + direction * roamSpeed * Time.fixedDeltaTime);

        // Smoothly rotate to face movement direction.
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        float smoothAngle = Mathf.LerpAngle(rb.rotation, targetAngle, rotationSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(smoothAngle);
    }

    private void PickNewRoamTarget()
    {
        // Pick a random point within the defined roam area.
        float randomX = Random.Range(roamAreaMin.x, roamAreaMax.x);
        float randomY = Random.Range(roamAreaMin.y, roamAreaMax.y);
        roamTarget = new Vector2(randomX, randomY);
    }
}
