using UnityEngine;

public class Player : MonoBehaviour
{
    // Regular movement variables.
    public float maxSpeed = 5f;
    public float slowDownDistance = 3f;
    public float acceleration = 10f;
    // The offset so that the tip of the capsule (front) reaches the cursor.
    public float tipOffset = 1f;

    // Boost settings.
    public float boostSpeed = 15f;
    public float boostDuration = 0.2f;
    private float boostTimer = 0f;
    private bool isBoosting = false;
    public int boostCount = 0;

    // Current movement speed for smooth transitions.
    private float currentSpeed = 0f;

    void Update()
    {
        // Convert the mouse position from screen space to world space.
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = transform.position.z; // Keep z coordinate consistent.

        // Calculate the vector from the player's center to the mouse.
        Vector3 dirToMouse = mousePos - transform.position;
        float distanceToMouse = dirToMouse.magnitude;

        if (distanceToMouse > 0.001f)
        {
            // Determine normalized direction and rotate the player to face the mouse.
            Vector3 direction = dirToMouse.normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // Convert the mouse position to local space relative to the player.
            Vector3 localMousePos = transform.InverseTransformPoint(mousePos);
            // Check if the mouse is inside the player's 2x1 capsule bounds.
            // (Assuming half extents of 1 on x and 0.5 on y)
            if (Mathf.Abs(localMousePos.x) <= 1f && Mathf.Abs(localMousePos.y) <= 0.5f)
            {
                // The cursor is within the player's capsule.
                // Only rotate and smoothly decelerate movement to zero.
                currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, acceleration * Time.deltaTime);
                return;
            }

            // Calculate target position so the tip of the capsule (offset by tipOffset) reaches the mouse.
            Vector3 targetPos = mousePos - direction * tipOffset;
            float effectiveDistance = Vector3.Distance(transform.position, targetPos);

            // Determine the desired speed, slowing down when close to the target.
            float desiredSpeed = maxSpeed;
            if (effectiveDistance < slowDownDistance)
            {
                desiredSpeed = maxSpeed * (effectiveDistance / slowDownDistance);
            }

            // Apply braking if the Q key is held.
            if (Input.GetKey(KeyCode.Q))
            {
                desiredSpeed = 0f;
            }

            // Boost logic.
            if (isBoosting)
            {
                desiredSpeed = boostSpeed;
                boostTimer -= Time.deltaTime;
                if (boostTimer <= 0)
                {
                    isBoosting = false;
                }
            }
            else if (Input.GetKeyDown(KeyCode.Space) && boostCount > 0)
            {
                isBoosting = true;
                boostTimer = boostDuration;
                boostCount--;
                desiredSpeed = boostSpeed;
            }

            // Smoothly transition currentSpeed towards desiredSpeed.
            currentSpeed = Mathf.MoveTowards(currentSpeed, desiredSpeed, acceleration * Time.deltaTime);
            // Move the player towards the target position.
            transform.position = Vector3.MoveTowards(transform.position, targetPos, currentSpeed * Time.deltaTime);
        }
        else
        {
            // When extremely close to the target, decelerate smoothly.
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, acceleration * Time.deltaTime);
        }
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
