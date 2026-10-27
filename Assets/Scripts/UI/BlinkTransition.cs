
using System.Collections;
using UnityEngine;

public class BlinkTransition : MonoBehaviour
{
    [Header("Eyelid References")]
    [SerializeField] private RectTransform topEyelid;
    [SerializeField] private RectTransform bottomEyelid;

    [Header("Animation Durations")]
    [SerializeField] private float firstOpenDuration = 0.7f;
    [SerializeField] private float closeDuration = 0.12f;
    [SerializeField] private float finalOpenDuration = 0.35f;

    public static bool IsPlaying { get; private set; }

    private RectTransform panel;
    private float moveDistance;

    private void Awake()
    {
        panel = GetComponent<RectTransform>();

        IsPlaying = false;
        SetEyeOpening(0f);
    }

    private void Start()
    {
        PlayBlink();
    }

    public void PlayBlink()
    {
        if (IsPlaying && gameObject.activeSelf)
        {
            return;
        }

        gameObject.SetActive(true);

        Canvas.ForceUpdateCanvases();

        moveDistance = panel.rect.height / 2f + 2f;

        SetEyeOpening(0f);

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

    // 0 = closed, 1 = fully open.
    private void SetEyeOpening(float opening)
    {
        float offset = moveDistance * opening;

        topEyelid.anchoredPosition = new Vector2(0f, offset);
        bottomEyelid.anchoredPosition = new Vector2(0f, -offset);
    }

    private void OnDisable()
    {
        IsPlaying = false;
    }
}
