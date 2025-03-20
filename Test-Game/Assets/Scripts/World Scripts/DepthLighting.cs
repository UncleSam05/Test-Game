using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DepthLighting : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerTransform;       // The player's (submarine's) transform.
    [SerializeField] private Volume globalVolume;             // Global Volume with Color Adjustments override.
    [SerializeField] private Light2D globalLight;               // Global 2D light that illuminates the scene.

    [Header("Depth Settings")]
    [SerializeField] private float surfaceY = 10f;              // Y-level at the surface.
    [SerializeField] private float deepY = -20f;                // Y-level at which the scene is completely dark.

    [Header("Color & Exposure Settings")]
    [SerializeField] private Color surfaceColor = new Color(0.5f, 0.7f, 1f, 1f); // Light blue tint at the surface.
    [SerializeField] private Color deepColor = Color.black;     // Dark tint at depth.
    [SerializeField] private float surfaceExposure = 1.0f;        // Exposure at the surface.
    [SerializeField] private float deepExposure = -2f;            // Exposure at depth.

    [Header("Global Light Settings")]
    [SerializeField] private float surfaceGlobalLightIntensity = 1.0f; // Global light intensity at the surface.
    [SerializeField] private float deepGlobalLightIntensity = 0.0f;    // Global light intensity at depth.

    private ColorAdjustments colorAdjustments;

    private void Start()
    {
        // Try to retrieve the Color Adjustments override from the global volume.
        if (globalVolume != null && globalVolume.profile.TryGet<ColorAdjustments>(out colorAdjustments))
        {
            // Initialize with surface settings.
            colorAdjustments.colorFilter.value = surfaceColor;
            colorAdjustments.postExposure.value = surfaceExposure;
        }
        else
        {
            Debug.LogError("Global Volume or Color Adjustments override not found!");
        }
    }

    private void Update()
    {
        if (playerTransform == null || colorAdjustments == null || globalLight == null)
            return;

        // Calculate a normalized t value: t = 0 at surfaceY, and t = 1 at deepY (or below).
        float t = Mathf.InverseLerp(surfaceY, deepY, playerTransform.position.y);

        // Lerp the scene tint from surfaceColor (light blue) to deepColor (black).
        Color currentFilter = Color.Lerp(surfaceColor, deepColor, t);
        colorAdjustments.colorFilter.value = currentFilter;

        // Lerp the overall exposure from surfaceExposure to deepExposure.
        float currentExposure = Mathf.Lerp(surfaceExposure, deepExposure, t);
        colorAdjustments.postExposure.value = currentExposure;

        // Lerp the global light's intensity from surfaceGlobalLightIntensity to deepGlobalLightIntensity.
        globalLight.intensity = Mathf.Lerp(surfaceGlobalLightIntensity, deepGlobalLightIntensity, t);
    }
}