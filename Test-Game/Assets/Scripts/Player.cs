using UnityEngine;

public class Player : MonoBehaviour
{
    // Underwater movement settings.
    public float maxSpeed = 5f;
    public float slowDownDistance = 3f;
    public float acceleration = 10f;
    // Offset so that the tip of the submarine (front) reaches the cursor.
    public float tipOffset = 1f;

    // Boost settings (underwater only).
    public float boostSpeed = 15f;
    public float boostDuration = 0.2f;
    public float boostSlowDownDistance = 1f;
    private float boostTimer = 0f;
    private bool isBoosting = false;
    public int boostCount = 0;
    private float currentSpeed = 0f;

    // Surfaced mode settings.
    // Y position at which the submarine is considered surfaced.
    public float waterSurfaceY = 10f;
    // Fixed rotation when surfaced (e.g., horizontal facing).
    public Quaternion surfacedRotation = Quaternion.Euler(0, 0, 0);
    // Velocity used for smooth acceleration in surfaced mode.
    private Vector3 surfacedVelocity = Vector3.zero;

    // Transition blending between underwater (0) and surfaced (1).
    // transitionTime controls how fast the submarine blends between modes.
    public float transitionTime = 0.5f;
    private float transition = 0f;

    void Update()
    {
        // Read vertical input.
        float verticalInput = Input.GetAxis("Vertical");

        // Update the transition parameter.
        // Only transition to surfaced mode if the submarine's y is at or above waterSurfaceY 
        // and the player is not actively diving (vertical input >= 0).
        float targetTransition = (transform.position.y >= waterSurfaceY && verticalInput >= 0) ? 1f : 0f;
        transition = Mathf.MoveTowards(transition, targetTransition, Time.deltaTime / transitionTime);

        // --------------------
        // UNDERWATER LOGIC
        // --------------------
        Vector3 underwaterVelocity = Vector3.zero;
        Quaternion underwaterRotation = transform.rotation;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = transform.position.z; // keep z consistent

        Vector3 dirToMouse = mousePos - transform.position;
        float distanceToMouse = dirToMouse.magnitude;

        if (distanceToMouse > 0.001f)
        {
            Vector3 direction = dirToMouse.normalized;
            underwaterRotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);

            // Check if the mouse is within the submarine’s 2x1 capsule bounds.
            Vector3 localMousePos = transform.InverseTransformPoint(mousePos);
            if (Mathf.Abs(localMousePos.x) <= 1f && Mathf.Abs(localMousePos.y) <= 0.5f)
            {
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, acceleration * Time.deltaTime);
                underwaterVelocity = Vector3.zero;
            }
            else
            {
                // Calculate target position so that the tip (offset by tipOffset) reaches the mouse.
                Vector3 targetPos = mousePos - direction * tipOffset;
                float effectiveDistance = Vector3.Distance(transform.position, targetPos);

                // Determine desired speed (with smooth slowdown as it nears the target).
                float desiredSpeed = maxSpeed;
                if (effectiveDistance < slowDownDistance)
                    desiredSpeed = maxSpeed * (effectiveDistance / slowDownDistance);

                // Boost logic applies only when underwater.
                if (transition < 0.01f)
                {
                    if (isBoosting)
                    {
                        if (effectiveDistance < boostSlowDownDistance)
                            desiredSpeed = boostSpeed * (effectiveDistance / boostSlowDownDistance);
                        else
                            desiredSpeed = boostSpeed;

                        boostTimer -= Time.deltaTime;
                        if (boostTimer <= 0)
                            isBoosting = false;
                    }
                    else if (Input.GetKeyDown(KeyCode.Space) && boostCount > 0)
                    {
                        isBoosting = true;
                        boostTimer = boostDuration;
                        boostCount--;
                        if (effectiveDistance < boostSlowDownDistance)
                            desiredSpeed = boostSpeed * (effectiveDistance / boostSlowDownDistance);
                        else
                            desiredSpeed = boostSpeed;
                    }
                }

                // Braking with Q key.
                if (Input.GetKey(KeyCode.Q))
                    desiredSpeed = 0f;

                currentSpeed = Mathf.MoveTowards(currentSpeed, desiredSpeed, acceleration * Time.deltaTime);
                underwaterVelocity = direction * currentSpeed;
            }
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, acceleration * Time.deltaTime);
        }

        // --------------------
        // SURFACED LOGIC
        // --------------------
        // While surfaced, only allow forward/backward movement and downward diving.
        float horizontal = Input.GetAxis("Horizontal");
        // Disallow upward movement when surfaced.
        if (verticalInput > 0)
            verticalInput = 0;
        Vector3 desiredSurfaceVelocity = new Vector3(horizontal, verticalInput, 0f) * maxSpeed;
        surfacedVelocity = Vector3.MoveTowards(surfacedVelocity, desiredSurfaceVelocity, acceleration * Time.deltaTime);

        // --------------------
        // BLEND MOVEMENT & ROTATION
        // --------------------
        Vector3 blendedVelocity = Vector3.Lerp(underwaterVelocity, surfacedVelocity, transition);
        Vector3 newPos = transform.position + blendedVelocity * Time.deltaTime;

        // Only force the vertical position to waterSurfaceY if not diving.
        if (transition > 0 && verticalInput >= 0)
        {
            newPos.y = Mathf.Lerp(newPos.y, waterSurfaceY, transition);
        }
        transform.position = newPos;

        Quaternion blendedRotation = Quaternion.Slerp(underwaterRotation, surfacedRotation, transition);
        transform.rotation = blendedRotation;
    }

    // On collision with a boost power-up trigger, grant three boosts and destroy the power-up.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BoostPowerUp"))
        {
            boostCount = 3;
            Destroy(other.gameObject);
        }
    }
}
