using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingCreditsScroll : MonoBehaviour
{
    public RectTransform creditsContent;

    public float scrollSpeed = 80f;
    public float startY = -700f;
    public float endY = 1200f;

    private bool isScrolling = true;

    /// <summary>
    /// Set Text at bottom
    /// </summary>
    void Start()
    {
        // Make sure the game is not still paused
        Time.timeScale = 1f;

        Vector2 position = creditsContent.anchoredPosition;

        position.y = startY;

        creditsContent.anchoredPosition = position;
    }

    /// <summary>
    /// Textcontent scrolling
    /// </summary>
    void Update()
    {
        if (isScrolling == true)
        {
            creditsContent.anchoredPosition +=
                Vector2.up * scrollSpeed * Time.unscaledDeltaTime;

            if (creditsContent.anchoredPosition.y >= endY)
            {
                isScrolling = false;

                StartCoroutine(ReturnToMainMenu());
            }
        }
    }

    /// <summary>
    /// Back after 2 second
    /// </summary>
    /// <returns></returns>
    IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSeconds(2f);

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