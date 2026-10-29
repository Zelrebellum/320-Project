
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject mainMenuContent;

    [Header("Scene Transition")]
    [Tooltip("CanvasGroup used to fade the screen to black.")]
    [SerializeField] private CanvasGroup fadePanel;

    [SerializeField] private float fadeDuration = 0.8f;

    private bool isTransitioning;

    private void Start()
    {
        fadePanel.alpha = 0f;
        fadePanel.blocksRaycasts = false;
        fadePanel.interactable = false;
    }

    public void StartGame()
    {
        if (isTransitioning)
        {
            return;
        }

        isTransitioning = true;

        // Block menu interaction during the scene transition.
        fadePanel.blocksRaycasts = true;

        StartCoroutine(FadeAndLoadGame());
    }

    private IEnumerator FadeAndLoadGame()
    {
        float time = 0f;

        while (time < fadeDuration)
        {
            time += Time.unscaledDeltaTime;

            fadePanel.alpha = Mathf.Clamp01(time / fadeDuration);

            yield return null;
        }

        fadePanel.alpha = 1f;

        SceneManager.LoadScene("Sloth");
    }

    public void OpenOptions()
    {
        mainMenuContent.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        mainMenuContent.SetActive(true);
    }

    public void OpenCredits()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("EndingCredits");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
