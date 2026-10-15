using System.Collections;
using UnityEngine;

public class BlinkTransition : MonoBehaviour
{
    [SerializeField] private RectTransform topEyelid;
    [SerializeField] private RectTransform bottomEyelid;

    [SerializeField] private float firstOpenDuration = 0.7f;
    [SerializeField] private float closeDuration = 0.12f;
    [SerializeField] private float finalOpenDuration = 0.35f;
    public static bool IsPlaying { get; private set; }

    private float moveDistance;

    /// <summary>
    /// Start with both eyelids closed
    /// </summary>
    private void Awake()
    {
        IsPlaying = false;
        SetEyeOpening(0f);
    }

    /// <summary>
    /// Play the animation when the level starts
    /// </summary>
    private void Start()
    {
        PlayBlink();
        //Canvas.ForceUpdateCanvases();

        //RectTransform panel = GetComponent<RectTransform>();
        //moveDistance = panel.rect.height / 2f + 2f;

        //yield return new WaitForSecondsRealtime(0.2f);
        //yield return MoveEyelids(0f, 1f, firstOpenDuration);

        //yield return new WaitForSecondsRealtime(0.15f);
        //yield return MoveEyelids(1f, 0f, closeDuration);

        //yield return new WaitForSecondsRealtime(0.08f);
        //yield return MoveEyelids(0f, 1f, finalOpenDuration);

        //IsPlaying = false;
        //gameObject.SetActive(false);
    }

    /// <summary>
    /// Move both eyelids smoothly
    /// </summary>
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
    /// Zero is closed and one is fully open
    /// </summary>
    private void SetEyeOpening(float opening)
    {
        float offset = moveDistance * opening;

        topEyelid.anchoredPosition = new Vector2(0f, offset);
        bottomEyelid.anchoredPosition = new Vector2(0f, -offset);
    }

    /// <summary>
    /// Clear the transition state when disabled
    /// </summary>
    private void OnDisable()
    {
        IsPlaying = false;
    }

    public void PlayBlink()
    {
        if(IsPlaying && gameObject.activeSelf)
        {
            return; 
        }

        gameObject.SetActive(true);

        Canvas.ForceUpdateCanvases();

        RectTransform panel = GetComponent<RectTransform>();
        moveDistance = panel.rect.height / 2f + 2f;

        SetEyeOpening(0);

        IsPlaying = true;
        StartCoroutine(BlinkSequence());
    }

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

}