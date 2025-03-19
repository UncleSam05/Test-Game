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
    [SerializeField] private float rotationSpeed = 5f;    // How fast the enemy rotates (smoothing factor).
    // Reference to the SpriteRenderer for flipping.
    [SerializeField] private SpriteRenderer spriteRenderer;

    private Vector2 roamTarget;   // Current roaming destination.
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
        // Determine whether to chase the player or roam.
        float distanceToPlayer = Vector2.Distance(rb.position, playerTransform.position);
        currentState = (distanceToPlayer <= chaseRange) ? EnemyState.Chase : EnemyState.Roam;

        if (currentState == EnemyState.Chase)
            ChasePlayer();
        else
            Roam();
    }

    private void ChasePlayer()
    {
        // Compute the direction toward the player.
        Vector2 direction = ((Vector2)playerTransform.position - rb.position).normalized;
        rb.MovePosition(rb.position + direction * chaseSpeed * Time.fixedDeltaTime);

        // Calculate target rotation in degrees.
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        // Adjust angle and determine if sprite should be flipped.
        targetAngle = AdjustAngleForSprite(targetAngle);
        // Smoothly interpolate the current rotation toward the target.
        float smoothAngle = Mathf.LerpAngle(rb.rotation, targetAngle, rotationSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(smoothAngle);
    }

    private void Roam()
    {
        // Compute direction toward the current roam target.
        Vector2 direction = roamTarget - rb.position;
        if (direction.magnitude < newTargetThreshold)
        {
            PickNewRoamTarget();
            direction = roamTarget - rb.position;
        }
        direction.Normalize();
        rb.MovePosition(rb.position + direction * roamSpeed * Time.fixedDeltaTime);

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        targetAngle = AdjustAngleForSprite(targetAngle);
        float smoothAngle = Mathf.LerpAngle(rb.rotation, targetAngle, rotationSpeed * Time.fixedDeltaTime);
        rb.MoveRotation(smoothAngle);
    }

    private void PickNewRoamTarget()
    {
        float randomX = Random.Range(roamAreaMin.x, roamAreaMax.x);
        float randomY = Random.Range(roamAreaMin.y, roamAreaMax.y);
        roamTarget = new Vector2(randomX, randomY);
    }

    /// <summary>
    /// Adjusts the target angle so that the enemy's sprite remains upright in a side-view game.
    /// If the angle exceeds 90° (or is below -90°), subtracts or adds 180° and sets the sprite to flip.
    /// </summary>
    /// <param name="angle">The original target angle in degrees.</param>
    /// <returns>The adjusted angle.</returns>
    private float AdjustAngleForSprite(float angle)
    {
        bool flip = false;
        if (angle > 90f)
        {
            angle -= 180f;
            flip = true;
        }
        else if (angle < -90f)
        {
            angle += 180f;
            flip = true;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = flip;
        }
        return angle;
    }
}
