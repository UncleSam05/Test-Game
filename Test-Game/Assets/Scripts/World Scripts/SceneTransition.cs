using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransition : MonoBehaviour
{
    public Image fadeImage;           // Assign your full-screen black image in the Inspector.
    public float fadeDuration = 1f;   // Duration of the fade effect.

    // Optionally, start with a fade-in when the scene loads.
    void Start()
    {
        StartCoroutine(FadeIn());
    }

    // Call this method from your Play button to start fading out and then load the scene.
    public void FadeToScene(string sceneName)
    {
        StartCoroutine(FadeOut(sceneName));
    }

    IEnumerator FadeOut(string sceneName)
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            SetImageAlpha(alpha);
            yield return null;
        }
        // Ensure the image is fully opaque.
        SetImageAlpha(1);
        // Load the target scene.
        SceneManager.LoadScene(sceneName);
    }

    IEnumerator FadeIn()
    {
        float timer = fadeDuration;
        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            float alpha = Mathf.Lerp(0, 1, timer / fadeDuration);
            SetImageAlpha(alpha);
            yield return null;
        }
        // Ensure the image is fully transparent.
        SetImageAlpha(0);
    }

    // Helper method to update the image's alpha.
    void SetImageAlpha(float alpha)
    {
        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }
}

