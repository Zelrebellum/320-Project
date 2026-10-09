using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject optionsPanel;
    public GameObject creditsPanel;

    // Black panel used for the fade transition
    public CanvasGroup fadePanel;
    public float fadeDuration = 0.8f;

    /// <summary>
    /// Press Start game and fade to the Game Scene
    /// </summary>
    public void StartGame()
    {
        StartCoroutine(FadeAndLoadGame());
    }

    /// <summary>
    /// Fade the screen to black, then load the Playground scene
    /// </summary>
    private IEnumerator FadeAndLoadGame()
    {
        float time = 0f;

        // Change the fade panel from transparent to black
        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;

            fadePanel.alpha = time / fadeDuration;

            yield return null;
        }

        // Make sure the screen completely black
        fadePanel.alpha = 1f;

        // Load the game scene
        SceneManager.LoadScene("Playground");
    }

    /// <summary>
    /// Press Setting open the Options panel
    /// </summary>
    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
    }

    /// <summary>
    /// Close the Options panel
    /// </summary>
    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
    }

    /// <summary>
    /// Press Credits open the Credits panel
    /// </summary>
    public void OpenCredits()
    {
        creditsPanel.SetActive(true);
    }

    /// <summary>
    /// Close the Credits panel
    /// </summary>
    public void CloseCredits()
    {
        creditsPanel.SetActive(false);
    }

    /// <summary>
    /// Quit the game
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}