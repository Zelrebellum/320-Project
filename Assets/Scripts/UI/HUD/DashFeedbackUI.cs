using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class DashFeedbackUI : MonoBehaviour
{
    [Header("Dash Flash")]
    [SerializeField, Range(0f, 1f)]
    private float flashAlpha = 0.12f;

    [SerializeField, Min(0.01f)]
    private float fadeDuration = 0.2f;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    private void Update()
    {
        if (PauseMenuManager.IsPaused || BlinkTransition.IsPlaying)
        {
            canvasGroup.alpha = 0f;
            return;
        }

        canvasGroup.alpha = Mathf.MoveTowards(
            canvasGroup.alpha,
            0f,
            flashAlpha / Mathf.Max(0.01f, fadeDuration)
                * Time.deltaTime
        );
    }

    // Call once when a dash successfully starts.
    [ContextMenu("Test Dash Flash")]
    public void PlayDashEffect()
    {
        if (canvasGroup == null
            || PauseMenuManager.IsPaused
            || BlinkTransition.IsPlaying)
        {
            return;
        }

        canvasGroup.alpha = flashAlpha;
    }

    private void OnDisable()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }
    }
}