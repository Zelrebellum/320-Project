using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RectTransform), typeof(CanvasGroup))]
public class TitleFragment : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float initialDelay = 3.5f;
    [SerializeField] private float fallDuration = 3f;
    [SerializeField] private float repeatDelay = 2f;

    [Header("Movement")]
    [SerializeField] private float fallDistance = 100f;
    [SerializeField] private float horizontalDrift = 20f;
    [SerializeField] private float rotationAmount = 80f;

    private RectTransform fragmentRect;
    private CanvasGroup fragmentGroup;

    // Store the original postion so each fall start
    private Vector2 startingPosition;
    private Quaternion startingRotation;

    private void Awake()
    {
        fragmentRect = GetComponent<RectTransform>();
        fragmentGroup = GetComponent<CanvasGroup>();

        startingPosition = fragmentRect.anchoredPosition;
        startingRotation = fragmentRect.localRotation;

        // Hide the fragment
        fragmentGroup.alpha = 0f;
        fragmentGroup.interactable = false;
        fragmentGroup.blocksRaycasts = false;
    }

    private IEnumerator Start()
    {
        // Wait for the title's
        yield return new WaitForSecondsRealtime(initialDelay);

        while (true)
        {
            // Reset the fragment
            fragmentRect.anchoredPosition = startingPosition;
            fragmentRect.localRotation = startingRotation;

            float elapsed = 0f;
            float duration = Mathf.Max(0.01f, fallDuration);

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float progress = Mathf.Clamp01(elapsed / duration);

                // Ease into the fall to create a gentle acceleration
                float fallProgress = progress * progress;

                Vector2 offset = new Vector2(
                    horizontalDrift * progress,
                    -fallDistance * fallProgress
                );

                fragmentRect.anchoredPosition = startingPosition + offset;

                // Slowly rotate the fragment as it falls
                fragmentRect.localRotation =
                    startingRotation *
                    Quaternion.Euler(0f, 0f, rotationAmount * progress);

                // Fade in briefly, then fade out during the second half
                float fadeIn = Mathf.Clamp01(progress / 0.1f);
                float fadeOut = 1f - Mathf.SmoothStep(
                    0f, 1f,
                    Mathf.InverseLerp(0.5f, 1f, progress)
                );

                fragmentGroup.alpha = fadeIn * fadeOut;

                // Continue on the next frame, even when timeScale is zero
                yield return null;
            }

            // Keep the fragment hidden between falls
            fragmentGroup.alpha = 0f;
            yield return new WaitForSecondsRealtime(repeatDelay);
        }
    }
}