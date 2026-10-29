using UnityEngine;

public class CreditsScroll : MonoBehaviour
{
    [Header("Credits References")]
    [SerializeField] private RectTransform creditsContent;

    [Header("Scrolling Settings")]
    [SerializeField] private float scrollSpeed = 80f;
    [SerializeField] private float startY = -700f;
    [SerializeField] private float endY = 1200f;

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
        if (!isScrolling)
        {
            return;
        }

        creditsContent.anchoredPosition +=
            Vector2.up * scrollSpeed * Time.unscaledDeltaTime;

        if (creditsContent.anchoredPosition.y >= endY)
        {
            isScrolling = false;
        }
    }
}