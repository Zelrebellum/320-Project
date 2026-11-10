using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class TitleSequence : MonoBehaviour
{
    [SerializeField] private float delay = 0.5f;
    [SerializeField] private float fadeDuration = 3f;

    private CanvasGroup titleGroup;

    /// <summary>
    /// Set up title state
    /// </summary>
    private void Awake()
    {
        titleGroup = GetComponent<CanvasGroup>();
        titleGroup.alpha = 0f;
        titleGroup.interactable = false;
        titleGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// Title fade
    /// </summary>
    /// <returns></returns>
    private IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(delay);

        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(
                elapsed / Mathf.Max(0.01f, fadeDuration)
            );

            titleGroup.alpha = Mathf.SmoothStep(0f, 1f, progress);
            yield return null;
        }

        titleGroup.alpha = 1f;
    }
}