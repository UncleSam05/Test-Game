using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    

    [Header("Input")]
    [SerializeField] private GameInput gameInput;

    private Rigidbody2D rb;
    private Vector3 initialScale;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        initialScale = transform.localScale; // Store the original scale for reference.
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
        // Flip the entire GameObject based on horizontal input.
        float horizontalInput = gameInput.MoveInput.x;
        if (horizontalInput > 0)
        {
            // Face right.
            transform.localScale = new Vector3(Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
        }
        else if (horizontalInput < 0)
        {
            // Face left.
            transform.localScale = new Vector3(-Mathf.Abs(initialScale.x), initialScale.y, initialScale.z);
        }
    }
}