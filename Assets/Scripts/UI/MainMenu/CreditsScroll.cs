using UnityEngine;

public class CreditsScroll : MonoBehaviour
{
    public RectTransform creditsContent;

    public float scrollSpeed = 80f;
    public float startY = -700f;
    public float endY = 1200f;

    private bool isScrolling = true;

    /// <summary>
    /// Reset credits position when Credits opens
    /// </summary>
    void OnEnable()
    {
        Vector2 position = creditsContent.anchoredPosition;

        position.y = startY;

        creditsContent.anchoredPosition = position;

        isScrolling = true;
    }

    /// <summary>
    /// Scroll the credits upward
    /// </summary>
    void Update()
    {
        if (isScrolling == true)
        {
            creditsContent.anchoredPosition +=
                Vector2.up * scrollSpeed * Time.deltaTime;

            if (creditsContent.anchoredPosition.y >= endY)
            {
                isScrolling = false;
            }
        }
    }
}