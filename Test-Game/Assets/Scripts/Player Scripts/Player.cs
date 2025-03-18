using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    // Underwater movement settings.
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float slowDownDistance = 3f;
    [SerializeField] private float acceleration = 10f;
    // Offset so that the tip of the submarine (front) reaches the cursor.
    [SerializeField] private float tipOffset = 1f;

    // Boost settings (underwater only).
    [SerializeField] private float boostSpeed = 15f;
    [SerializeField] private float boostDuration = 0.2f;
    [SerializeField] private float boostSlowDownDistance = 1f;
    [SerializeField] private int boostCount = 0;

    // Surfaced mode settings.
    // Y position at which the submarine is considered surfaced (and clamped as max).
    [SerializeField] private float waterSurfaceY = 10f;
    // Fixed rotation when surfaced (e.g., horizontal facing).
    [SerializeField] private Quaternion surfacedRotation = Quaternion.Euler(0, 0, 0);
    // Transition time between underwater and surfaced behavior.
    [SerializeField] private float transitionTime = 0.5f;

    // Negative input delay: how long negative (dive) input must be sustained to trigger underwater mode.
    [SerializeField] private float diveThresholdTime = 0.2f;

    // Reference to the GameInput component (assign via Inspector).
    [SerializeField] private GameInput gameInput;

    // Private runtime variables.
    private float currentSpeed = 0f;
    private float boostTimer = 0f;
    private bool isBoosting = false;
    private Vector3 surfacedVelocity = Vector3.zero;
    private float transition = 0f;
    private float negativeInputTimer = 0f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        // Get input values from GameInput.
        Vector2 moveInput = gameInput.MoveInput;
        float horizontalInput = moveInput.x;
        float verticalInput = moveInput.y; // verticalInput < 0 means "S" is pressed

        // Update negative input timer.
        if (verticalInput < 0)
            negativeInputTimer += Time.fixedDeltaTime;
        else
            negativeInputTimer = 0f;

        // Update the transition parameter.
        // Remain in surfaced mode (transition -> 1) if the submarine's y is at or above waterSurfaceY 
        // and negative input hasn't been sustained longer than diveThresholdTime.
        float targetTransition = (rb.position.y >= waterSurfaceY && negativeInputTimer < diveThresholdTime) ? 1f : 0f;
        transition = Mathf.MoveTowards(transition, targetTransition, Time.fixedDeltaTime / transitionTime);

        // --------------------
        // UNDERWATER LOGIC (mouse-controlled)
        // --------------------
        Vector3 underwaterVelocity = Vector3.zero;
        Quaternion underwaterRotation = transform.rotation;

        // Get cursor position from GameInput and convert to world space.
        Vector2 cursorScreenPos = gameInput.CursorPosition;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(cursorScreenPos.x, cursorScreenPos.y, Camera.main.nearClipPlane));
        mousePos.z = transform.position.z; // maintain same z

        Vector3 dirToMouse = mousePos - (Vector3)rb.position;
        float distanceToMouse = dirToMouse.magnitude;

        if (distanceToMouse > 0.001f)
        {
            Vector3 direction = dirToMouse.normalized;
            underwaterRotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

            // Check if the mouse is within the submarine’s 2x1 capsule bounds.
            Vector3 localMousePos = transform.InverseTransformPoint(mousePos);
            if (Mathf.Abs(localMousePos.x) <= 1f && Mathf.Abs(localMousePos.y) <= 0.5f)
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, acceleration * Time.fixedDeltaTime);
                underwaterVelocity = Vector3.zero;
            }
            else
            {
                // Calculate target position so that the tip (offset by tipOffset) reaches the mouse.
                Vector3 targetPos = mousePos - direction * tipOffset;
                float effectiveDistance = Vector3.Distance(rb.position, targetPos);

                // Determine desired speed based on effective distance (slow down near target).
                float desiredSpeed = maxSpeed;
                if (effectiveDistance < slowDownDistance)
                    desiredSpeed = maxSpeed * (effectiveDistance / slowDownDistance);

                // Apply braking if Brake input is active.
                if (gameInput.BrakePressed)
                    desiredSpeed = 0f;

                // Boost logic.
                if (isBoosting)
                {
                    if (effectiveDistance < boostSlowDownDistance)
                        desiredSpeed = boostSpeed * (effectiveDistance / boostSlowDownDistance);
                    else
                        desiredSpeed = boostSpeed;
                    boostTimer -= Time.fixedDeltaTime;
                    if (boostTimer <= 0)
                        isBoosting = false;
                }
                else if (gameInput.BoostPressed && boostCount > 0)
                {
                    isBoosting = true;
                    boostTimer = boostDuration;
                    boostCount--;
                    if (effectiveDistance < boostSlowDownDistance)
                        desiredSpeed = boostSpeed * (effectiveDistance / boostSlowDownDistance);
                    else
                        desiredSpeed = boostSpeed;
                }

                currentSpeed = Mathf.MoveTowards(currentSpeed, desiredSpeed, acceleration * Time.fixedDeltaTime);
                underwaterVelocity = direction * currentSpeed;
            }
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, acceleration * Time.fixedDeltaTime);
        }

        // --------------------
        // SURFACED LOGIC (keyboard-controlled)
        // --------------------
        // While surfaced, allow only horizontal movement and downward diving.
        // Disallow upward movement when surfaced.
        float clampedVerticalInput = verticalInput > 0 ? 0 : verticalInput;
        Vector3 desiredSurfaceVelocity = new Vector3(horizontalInput, clampedVerticalInput, 0f) * maxSpeed;
        surfacedVelocity = Vector3.MoveTowards(surfacedVelocity, desiredSurfaceVelocity, acceleration * Time.fixedDeltaTime);

        // --------------------
        // BLEND MOVEMENT & ROTATION
        // --------------------
        Vector3 blendedVelocity = Vector3.Lerp(underwaterVelocity, surfacedVelocity, transition);
        Vector2 newPos = rb.position + (Vector2)(blendedVelocity * Time.fixedDeltaTime);

        // Clamp the y position so that the submarine cannot go above waterSurfaceY.
        newPos.y = Mathf.Min(newPos.y, waterSurfaceY);

        rb.MovePosition(newPos);

        Quaternion blendedRotation = Quaternion.Slerp(underwaterRotation, surfacedRotation, transition);
        rb.MoveRotation(blendedRotation);

        // Consume the boost input.
        gameInput.ConsumeBoost();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BoostPowerUp"))
        {
            boostCount = 3;
            Destroy(other.gameObject);
        }
    }
}
