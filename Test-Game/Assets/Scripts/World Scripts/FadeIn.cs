using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeIn : MonoBehaviour
{
    public Image fadeImage;           // Assign the full-screen black image in the Inspector.
    public float fadeDuration = 2f;     // Duration for the fade-in effect.
    public float delayBeforeFade = 1f;
    [SerializeField] private GameObject globalLight;
    [SerializeField] private GameObject playerGlobalLight;

    void Start()
    {
        // Ensure the image starts fully opaque.
        SetFadeAlpha(1f);
        // Start the fade-in process after the specified delay.
        StartCoroutine(DelayedFadeIn());
    }

    IEnumerator DelayedFadeIn()
    {
        // Wait for the specified delay.
        yield return new WaitForSeconds(delayBeforeFade);

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            // Lerp alpha from 1 (opaque) to 0 (transparent) over fadeDuration.
            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            SetFadeAlpha(alpha);
            yield return null;
        }
        // Ensure the image is completely transparent after the fade.
        SetFadeAlpha(0f);
    }

    void SetFadeAlpha(float alpha)
    {
        Color c = fadeImage.color;
        c.a = alpha;
        fadeImage.color = c;
    }
    private void Awake()
    {
        globalLight.SetActive(false);
        playerGlobalLight.SetActive(true);
        fadeImage.enabled = true;
    }
}

