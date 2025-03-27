using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // This method is called when the Play button is clicked.
    public void PlayGame()
    {
        // Make sure "GameScene" matches the name of your game scene.
        SceneManager.LoadScene("SampleScene");
    }
}

