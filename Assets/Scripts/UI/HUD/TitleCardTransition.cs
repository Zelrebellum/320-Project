using System.Collections;
using UnityEngine;

/// <summary>
/// Displays the level title card before playing the blink transition
/// Attached to the TitleCard object
/// </summary>
public class TitleCardTransition : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup titleCanvasGroup;
    [SerializeField] private BlinkTransition blinkTransition;

    [Header("Timing")]
    [SerializeField] private float holdDuration = 2f;

    /// <summary>
    /// Shows the title for the configured duration then starts
    /// the blink transition and hides the title card
    /// </summary>
    private IEnumerator Start()
    {
        if (titleCanvasGroup == null || blinkTransition == null)
        {
            Debug.LogError(
                "TitleCardTransition: Assign both references.", this);
            yield break;
        }

        titleCanvasGroup.alpha = 1f;
        titleCanvasGroup.blocksRaycasts = true;

        // Keep the title visible even when the game is paused.
        yield return new WaitForSecondsRealtime(holdDuration);

        // Close the eyelids before removing the title card.
        blinkTransition.PlayBlink();
        gameObject.SetActive(false);
    }
}