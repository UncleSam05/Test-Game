using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class PlayGame : MonoBehaviour
{
    public Image fadeImage;         // Assign your full-screen black image in the Inspector.
    public float fadeDuration = 1f; // Duration for fade-out.
    public string gameSceneName = "GameScene"; // Name of your game scene.

    // Call this method when the Play button is pressed.
    public void PlayGameButtonPressed()
    {
        StartCoroutine(FadeOutAndLoad());
    }

    IEnumerator FadeOutAndLoad()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            // Lerp alpha from 0 (transparent) to 1 (opaque)
            float alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            SetFadeAlpha(alpha);
            yield return null;
        }
        // Ensure the image is fully opaque.
        SetFadeAlpha(1f);
        // Load the game scene.
        SceneManager.LoadScene(gameSceneName);
    }

    void SetFadeAlpha(float alpha)
    {
        Color newColor = fadeImage.color;
        newColor.a = alpha;
        fadeImage.color = newColor;
    }
}


