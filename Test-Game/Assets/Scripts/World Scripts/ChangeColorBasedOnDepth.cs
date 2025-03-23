using UnityEngine;

public class ChangeColorBasedOnDepth : MonoBehaviour
{
    // Reference to the player's transform.
    public Transform player;

    // Colors for the surface and deep states.
    public Color surfaceColor = Color.white; // Color when the player is at or near the surface.
    public Color deepColor = Color.blue;       // Color when the player is deep down.

    // Y positions corresponding to the surface and deep levels.
    public float surfaceY = 0f;   // The highest Y position.
    public float deepY = -10f;    // The deepest Y position.

    // Reference to the SpriteRenderer component.
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        // Get the player's current Y position.
        float playerY = player.position.y;

        // Convert the player's Y position to a 0–1 range, where 0 corresponds to surfaceY and 1 corresponds to deepY.
        float t = Mathf.InverseLerp(surfaceY, deepY, playerY);

        // Interpolate between the surfaceColor and deepColor based on t.
        spriteRenderer.color = Color.Lerp(surfaceColor, deepColor, t);
    }
}