using UnityEngine;

public class Player : MonoBehaviour
{
    // Underwater movement settings.
    public float maxSpeed = 5f;
    public float slowDownDistance = 3f;
    public float acceleration = 10f;
    // Offset so the tip of the submarine (front) reaches the cursor.
    public float tipOffset = 1f;

    // Boost settings (underwater only).
    public float boostSpeed = 15f;
    public float boostDuration = 0.2f;
    // When boosting, if within this distance the speed scales down.
    public float boostSlowDownDistance = 1f;
    private float boostTimer = 0f;
    private bool isBoosting = false;
    public int boostCount = 0;
    private float currentSpeed = 0f;

    // Surfaced mode settings.
    // Y position at which the submarine is considered surfaced.
    public float waterSurfaceY = 10f;
    // Fixed rotation when surfaced (e.g. horizontal facing).
    public Quaternion surfacedRotation = Quaternion.Euler(0, 0, 0);
    // Velocity used for smooth acceleration in surfaced mode.
    private Vector3 surfacedVelocity = Vector3.zero;

    // Transition blending between underwater (0) and surfaced (1).
    public float transitionTime = 0.5f;
    private float transition = 0f;

    void Update()
    {
        // Update the transition parameter toward 1 if above waterSurfaceY, else toward 0.
        float targetTransition = (transform.position.y >= waterSurfaceY) ? 1f : 0f;
        transition = Mathf.MoveTowards(transition, targetTransition, Time.deltaTime / transitionTime);

        // --------------------
        // UNDERWATER LOGIC
        // --------------------
        Vector3 underwaterVelocity = Vector3.zero;
        Quaternion underwaterRotation = transform.rotation; // fallback
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
                // If the cursor is inside the submarine, only rotate and decelerate.
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, acceleration * Time.deltaTime);
                underwaterVelocity = Vector3.zero;
            }
            else
            {
                // Calculate the target position so that the tip (offset by tipOffset) reaches the mouse.
                Vector3 targetPos = mousePos - direction * tipOffset;
                float effectiveDistance = Vector3.Distance(transform.position, targetPos);

                // Determine the desired speed (smooth slowdown when nearing the target).
                float desiredSpeed = maxSpeed;
                if (effectiveDistance < slowDownDistance)
                    desiredSpeed = maxSpeed * (effectiveDistance / slowDownDistance);

                // Boost logic applies only when fully underwater.
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

                // Allow braking via Q key.
                if (Input.GetKey(KeyCode.Q))
                    desiredSpeed = 0f;

                // Smoothly transition current speed.
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
        // Only allow forward/backward and downward movement when surfaced.
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        if (vertical > 0)
            vertical = 0;
        Vector3 desiredSurfaceVelocity = new Vector3(horizontal, vertical, 0f) * maxSpeed;
        surfacedVelocity = Vector3.MoveTowards(surfacedVelocity, desiredSurfaceVelocity, acceleration * Time.deltaTime);

        // --------------------
        // BLEND MOVEMENT & ROTATION
        // --------------------
        // Blend the underwater (mouse-controlled) and surfaced (keyboard-controlled) velocities.
        Vector3 blendedVelocity = Vector3.Lerp(underwaterVelocity, surfacedVelocity, transition);
        transform.position += blendedVelocity * Time.deltaTime;

        // Blend the rotation between underwaterRotation (free aiming) and the fixed surfacedRotation.
        Quaternion blendedRotation = Quaternion.Slerp(underwaterRotation, surfacedRotation, transition);
        transform.rotation = blendedRotation;
    }

    // On collision with a boost power-up (with trigger collider and tag "BoostPowerUp"),
    // grant three boosts and destroy the power-up.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BoostPowerUp"))
        {
            boostCount = 3;
            Destroy(other.gameObject);
        }
    }
}
