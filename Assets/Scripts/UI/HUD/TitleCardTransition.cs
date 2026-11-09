using System.Collections;
using UnityEngine;

public class TitleCardTransition : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CanvasGroup titleCanvasGroup;
    [SerializeField] private BlinkTransition blinkTransition;

    [Header("Timing")]
    [SerializeField] private float holdDuration = 2f;

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

        // Keep the title fully visible.
        yield return new WaitForSecondsRealtime(holdDuration);

        // Start the blink before hiding the title card.
        blinkTransition.PlayBlink();
        gameObject.SetActive(false);
    }
}