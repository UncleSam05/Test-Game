using UnityEngine;
using UnityEngine.Rendering.Universal;

public class SubmarineLocalLightActivator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerTransform; // Reference to the submarine's transform.
    [SerializeField] private Light2D localLight;          // The Light2D component for the submarine's local light.

    [Header("Activation Settings")]
    [SerializeField] private float activationDepth = 0f;    // The y-value below which the light activates (e.g., 0 or lower).

    [Header("Optional Smoothing")]
    [SerializeField] private bool smoothTransition = false; // Toggle for smooth intensity change.
    [SerializeField] private float targetIntensity = 1f;    // The desired intensity when active.
    [SerializeField] private float intensityLerpSpeed = 2f;   // Lerp speed for smooth intensity transitions.

    private void Update()
    {
        // Check if the player is below the activation depth.
        if (playerTransform.position.y < activationDepth)
        {
            // Enable the local light if it is not already enabled.
            if (!localLight.gameObject.activeSelf)
            {
                localLight.gameObject.SetActive(true);
                // Optionally, if not smoothing, set intensity immediately.
                if (!smoothTransition)
                {
                    localLight.intensity = targetIntensity;
                }
            }

            // If smoothing is enabled, smoothly interpolate intensity to targetIntensity.
            if (smoothTransition)
            {
                localLight.intensity = Mathf.Lerp(localLight.intensity, targetIntensity, intensityLerpSpeed * Time.deltaTime);
            }
        }
        else
        {
            // If the player is above the activation depth, disable the local light.
            if (localLight.gameObject.activeSelf)
            {
                localLight.gameObject.SetActive(false);
            }
        }
    }
}

