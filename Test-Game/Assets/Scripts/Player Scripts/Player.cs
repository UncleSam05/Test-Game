using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    // You can remove waterSurfaceY if it's no longer needed, or keep it for other logic.
    [SerializeField] private float waterSurfaceY = 10f;

    [Header("Input")]
    [SerializeField] private GameInput gameInput;

    [Header("Visual Settings")]
    // Reference to the SpriteRenderer on your "Player Visual" child.
    [SerializeField] private SpriteRenderer visualSprite;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        // Get movement input from GameInput.
        Vector2 moveInput = gameInput.MoveInput;

        // Calculate new position based on input.
        Vector2 newPos = rb.position + moveInput * moveSpeed * Time.fixedDeltaTime;

        // Clamp the y value so that the player never goes above y = 0.
        newPos.y = Mathf.Min(newPos.y, 0f);

        rb.MovePosition(newPos);
    }

    private void Update()
    {
        // Flip sprite based on horizontal input.
        float horizontalInput = gameInput.MoveInput.x;
        if (horizontalInput > 0)
        {
            visualSprite.flipX = true;
        }
        else if (horizontalInput < 0)
        {
            visualSprite.flipX = false;
        }
    }
}