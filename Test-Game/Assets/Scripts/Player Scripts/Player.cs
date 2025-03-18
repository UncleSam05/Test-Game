using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    // Underwater movement settings.
    [SerializeField] private float maxSpeed = 5f;
    [SerializeField] private float slowDownDistance = 3f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float tipOffset = 1f;

    // Boost settings (underwater only).
    [SerializeField] private float boostSpeed = 15f;
    [SerializeField] private float boostDuration = 0.2f;
    [SerializeField] private float boostSlowDownDistance = 1f;
    [SerializeField] private int boostCount = 0;

    // Surfaced mode settings.
    [SerializeField] private float waterSurfaceY = 10f;
    [SerializeField] private Quaternion surfacedRotation = Quaternion.Euler(0, 0, 0);
    [SerializeField] private float transitionTime = 0.5f;

    // Negative input delay.
    [SerializeField] private float diveThresholdTime = 0.2f;

    // Reference to GameInput.
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
        // Get input from GameInput.
        Vector2 moveInput = gameInput.MoveInput;
        float horizontalInput = moveInput.x;
        float verticalInput = moveInput.y; // vertical < 0 means "S" pressed

        if (verticalInput < 0)
            negativeInputTimer += Time.fixedDeltaTime;
        else
            negativeInputTimer = 0f;

        float targetTransition = (rb.position.y >= waterSurfaceY && negativeInputTimer < diveThresholdTime) ? 1f : 0f;
        transition = Mathf.MoveTowards(transition, targetTransition, Time.fixedDeltaTime / transitionTime);

        // --------------------
        // UNDERWATER LOGIC (mouse-controlled)
        // --------------------
        Vector3 underwaterVelocity = Vector3.zero;
        Quaternion underwaterRotation = transform.rotation;

        Vector2 cursorScreenPos = gameInput.CursorPosition;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(cursorScreenPos.x, cursorScreenPos.y, Camera.main.nearClipPlane));
        mousePos.z = transform.position.z;

        Vector3 dirToMouse = mousePos - (Vector3)rb.position;
        float distanceToMouse = dirToMouse.magnitude;

        if (distanceToMouse > 0.001f)
        {
            Vector3 direction = dirToMouse.normalized;
            underwaterRotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

            Vector3 localMousePos = transform.InverseTransformPoint(mousePos);
            if (Mathf.Abs(localMousePos.x) <= 1f && Mathf.Abs(localMousePos.y) <= 0.5f)
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, acceleration * Time.fixedDeltaTime);
                underwaterVelocity = Vector3.zero;
            }
            else
            {
                Vector3 targetPos = mousePos - direction * tipOffset;
                float effectiveDistance = Vector3.Distance(rb.position, targetPos);

                float desiredSpeed = maxSpeed;
                if (effectiveDistance < slowDownDistance)
                    desiredSpeed = maxSpeed * (effectiveDistance / slowDownDistance);

                if (gameInput.BrakePressed)
                    desiredSpeed = 0f;

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
        float clampedVerticalInput = verticalInput > 0 ? 0 : verticalInput;
        Vector3 desiredSurfaceVelocity = new Vector3(horizontalInput, clampedVerticalInput, 0f) * maxSpeed;
        surfacedVelocity = Vector3.MoveTowards(surfacedVelocity, desiredSurfaceVelocity, acceleration * Time.fixedDeltaTime);

        // --------------------
        // BLEND MOVEMENT & ROTATION
        // --------------------
        Vector3 blendedVelocity = Vector3.Lerp(underwaterVelocity, surfacedVelocity, transition);
        Vector2 newPos = rb.position + (Vector2)(blendedVelocity * Time.fixedDeltaTime);

        if (transition > 0 && verticalInput >= 0)
            newPos.y = Mathf.Lerp(newPos.y, waterSurfaceY, transition);
        newPos.y = Mathf.Min(newPos.y, waterSurfaceY);

        rb.MovePosition(newPos);

        Quaternion blendedRotation = Quaternion.Slerp(underwaterRotation, surfacedRotation, transition);
        rb.MoveRotation(blendedRotation);

        // Consume the boost input so that it's only used once.
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
