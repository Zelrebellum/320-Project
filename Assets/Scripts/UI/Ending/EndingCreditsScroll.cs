using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingCreditsScroll : MonoBehaviour
{
    [Header("Credits References")]
    [SerializeField] private RectTransform creditsContent;

    [Header("Scrolling Settings")]
    [SerializeField] private float scrollSpeed = 80f;
    [SerializeField] private float startY = -700f;
    [SerializeField] private float endY = 1200f;
    private bool isScrolling = true;

    /// <summary>
    /// Set Text at bottom
    /// </summary>
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        // Make sure the game is not still paused
        Time.timeScale = 1f;

        Vector2 position = creditsContent.anchoredPosition;

        position.y = startY;

        creditsContent.anchoredPosition = position;
    }

    /// <summary>
    /// Textcontent scrolling
    /// </summary>
    private void Update()
    {
        if (!isScrolling)
        {
            return;
        }

        creditsContent.anchoredPosition +=
            Vector2.up * scrollSpeed * Time.unscaledDeltaTime;

        if (creditsContent.anchoredPosition.y >= endY)
        {
            isScrolling = false;
            StartCoroutine(ReturnToMainMenu());
        }
    }

    /// <summary>
    /// Back to main menu after 2 second
    /// </summary>
    /// <returns></returns>
    IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSecondsRealtime(2f);

        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Skip scrolling text and back to mainmenu
    /// </summary>
    public void SkipCredits()
    {
        StopAllCoroutines();

        Time.timeScale = 1f;

        SceneManager.LoadScene("MainMenu");
    }
}