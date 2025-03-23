using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SubmarineLightingController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerTransform;   // The submarine/player's transform.
    [SerializeField] private Light2D globalLight;         // The big, spot Light2D covering the whole screen.
    [SerializeField] private Light2D localLight;          // The small, circular Light2D around the player.

    [Header("Global Light Settings")]
    [SerializeField] private float surfaceY = 10f;          // At or above this y-level, global light is full intensity.
    [SerializeField] private float deepY = -10f;            // At or below this y-level, global light intensity is 0.

    [Header("Local Light Settings")]
    [SerializeField] private float localActivationY = -5f;  // When player's y is below this, local light activates.

    private void Update()
    {
        if (playerTransform == null || globalLight == null || localLight == null)
            return;

        float playerY = playerTransform.position.y;

        // Global Light:
        // Use InverseLerp with deepY as 0 intensity and surfaceY as full intensity.
        // This produces a value of 0 when playerY <= deepY, 1 when playerY >= surfaceY, and a linear interpolation in between.
        float globalIntensity = Mathf.Clamp01(Mathf.InverseLerp(deepY, surfaceY, playerY));
        globalLight.intensity = globalIntensity;

        // Local Light:
        // Activate the local light (set intensity to 1 and enable the light)
        // only when the player is below localActivationY; otherwise, disable it.
        if (playerY < localActivationY)
        {
            if (!localLight.enabled)
                localLight.enabled = true;
            localLight.intensity = 1f;
        }
        else
        {
            localLight.intensity = 0f;
            if (localLight.enabled)
                localLight.enabled = false;
        }
    }
}
