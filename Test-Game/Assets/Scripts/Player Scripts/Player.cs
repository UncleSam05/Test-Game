using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float waterSurfaceY = 10f;
    // Multiplier for movement speed: horizontal movement will be faster.
    [SerializeField] private float horizontalMultiplier = 1.5f;
    [SerializeField] private float verticalMultiplier = 1f;

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
        // Get raw movement input from GameInput.
        Vector2 moveInput = gameInput.MoveInput;

        // Scale the input: horizontal faster than vertical.
        Vector2 scaledInput = new Vector2(moveInput.x * horizontalMultiplier, moveInput.y * verticalMultiplier);

        // Prevent upward movement when at or above waterSurfaceY.
        if (rb.position.y >= waterSurfaceY && scaledInput.y > 0)
            scaledInput.y = 0;

        Vector2 newPos = rb.position + scaledInput * moveSpeed * Time.fixedDeltaTime;
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
        // If horizontal input is zero, retain the previous flip state.
    }
}
