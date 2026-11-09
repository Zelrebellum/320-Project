using System.Collections;
using UnityEngine;

/// <summary>
/// Plays an eyelid animation when called by the title card transition
/// Attached to the BlinkTransition UI panel
/// </summary>
public class BlinkTransition : MonoBehaviour
{
    [Header("Eyelid References")]
    [SerializeField] private RectTransform topEyelid;
    [SerializeField] private RectTransform bottomEyelid;

    [Header("Animation Durations")]
    [SerializeField] private float firstOpenDuration = 0.7f;
    [SerializeField] private float closeDuration = 0.12f;
    [SerializeField] private float finalOpenDuration = 0.35f;

    /// <summary>
    /// Indicates whether the blink animation is currently playing
    /// </summary>
    public static bool IsPlaying { get; private set; }

    private RectTransform panel;
    private float moveDistance;

    /// <summary>
    /// Stores the panel reference and initializes the eyelids as closed
    /// </summary>
    private void Awake()
    {
        panel = GetComponent<RectTransform>();

        IsPlaying = false;
        SetEyeOpening(0f);
    }

    /// <summary>
    /// Activates the panel and starts the blink animation
    /// Repeated calls are ignored while the animation is playing
    /// </summary>
    public void PlayBlink()
    {
        if (IsPlaying && gameObject.activeSelf)
        {
            return;
        }

        gameObject.SetActive(true);

        // Update the UI layout before measuring the panel
        Canvas.ForceUpdateCanvases();

        // Add a small margin to move the eyelids beyond the center
        moveDistance = panel.rect.height / 2f + 2f;

        SetEyeOpening(0f);

        IsPlaying = true;
        StartCoroutine(BlinkSequence());
    }

    /// <summary>
    /// Opens the eyes, briefly closes them, then opens them again
    /// Hides the panel when the sequence finishes
    /// </summary>
    private IEnumerator BlinkSequence()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        yield return MoveEyelids(0f, 1f, firstOpenDuration);

        yield return new WaitForSecondsRealtime(0.15f);
        yield return MoveEyelids(1f, 0f, closeDuration);

        yield return new WaitForSecondsRealtime(0.08f);
        yield return MoveEyelids(0f, 1f, finalOpenDuration);

        IsPlaying = false;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// Moves both eyelids smoothly between two opening amounts
    /// Uses unscaled time so the animation can play while paused
    /// </summary>
    /// <param name="from">The starting opening amount</param>
    /// <param name="to">The target opening amount</param>
    /// <param name="duration">The movement duration in seconds</param>
    private IEnumerator MoveEyelids(
        float from, float to, float duration)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsed / duration);
            progress = Mathf.SmoothStep(0f, 1f, progress);

            SetEyeOpening(Mathf.Lerp(from, to, progress));

            yield return null;
        }

        SetEyeOpening(to);
    }

    /// <summary>
    /// Positions the top and bottom eyelids in opposite directions
    /// </summary>
    /// <param name="opening">
    /// The opening amount: 0 is closed and 1 is fully open
    /// </param>
    private void SetEyeOpening(float opening)
    {
        float offset = moveDistance * opening;

        topEyelid.anchoredPosition = new Vector2(0f, offset);
        bottomEyelid.anchoredPosition = new Vector2(0f, -offset);
    }

    /// <summary>
    /// Clears the playing state when the panel is disabled
    /// </summary>
    private void OnDisable()
    {
        IsPlaying = false;
    }
}