using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightPulse : MonoBehaviour
{
    [SerializeField] private float pulseDuration = 2f;  // Total duration of one pulse cycle.
    [SerializeField] private float maxIntensity = 1f;     // Maximum intensity during the pulse.
    [SerializeField] private float minIntensity = 0f;     // Minimum intensity during the pulse.

    private Light2D light2D;
    private Coroutine pulseRoutine;

    public bool IsPulsing { get { return pulseRoutine != null; } }

    private void Awake()
    {
        light2D = GetComponent<Light2D>();
    }

    public void StartPulsing()
    {
        if (pulseRoutine == null)
            pulseRoutine = StartCoroutine(Pulse());
    }

    public void StopPulsing()
    {
        if (pulseRoutine != null)
        {
            StopCoroutine(pulseRoutine);
            pulseRoutine = null;
            // Optionally, reset intensity to a default value.
            light2D.intensity = minIntensity;
        }
    }

    private IEnumerator Pulse()
    {
        while (true)
        {
            float timer = 0f;
            // Ramp up.
            while (timer < pulseDuration / 2f)
            {
                timer += Time.deltaTime;
                light2D.intensity = Mathf.Lerp(minIntensity, maxIntensity, timer / (pulseDuration / 2f));
                yield return null;
            }
            timer = 0f;
            // Ramp down.
            while (timer < pulseDuration / 2f)
            {
                timer += Time.deltaTime;
                light2D.intensity = Mathf.Lerp(maxIntensity, minIntensity, timer / (pulseDuration / 2f));
                yield return null;
            }
        }
    }
}