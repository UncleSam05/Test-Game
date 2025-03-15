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
            // Normalize the direction vector.
            Vector3 direction = dirToMouse.normalized;

            // Rotate the player to face the mouse.
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // Calculate the target position for the player's center so that the tip reaches the mouse.
            // When transform.position equals targetPos, the tip (offset by tipOffset) will be at the mouse.
            Vector3 targetPos = mousePos - direction * tipOffset;

            // Calculate the effective distance from the player's current position to the target position.
            float effectiveDistance = Vector3.Distance(transform.position, targetPos);

            // Determine the desired speed based on effectiveDistance (slowing down near the target).
            float desiredSpeed = maxSpeed;
            if (effectiveDistance < slowDownDistance)
            {
                desiredSpeed = maxSpeed * (effectiveDistance / slowDownDistance);
            }

            // If Q is held down, override desired speed with zero (braking).
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

            // Smoothly transition currentSpeed towards the desiredSpeed.
            currentSpeed = Mathf.MoveTowards(currentSpeed, desiredSpeed, acceleration * Time.deltaTime);

            // Move the player towards the target position.
            transform.position = Vector3.MoveTowards(transform.position, targetPos, currentSpeed * Time.deltaTime);
        }
        else
        {
            // If very close, decelerate smoothly.
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, acceleration * Time.deltaTime);
        }
    }

    // When colliding with a boost power-up (set as a trigger with the tag "BoostPowerUp"),
    // grant three boosts and destroy the power-up object.
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("BoostPowerUp"))
        {
            boostCount = 3;
            Destroy(other.gameObject);
        }
    }
}
