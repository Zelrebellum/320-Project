
using UnityEngine;
using UnityEngine.UI;

public class SlowdownOverlay : MonoBehaviour
{
    [Header("Overlay References")]
    [SerializeField] private Image mudImage;
    [SerializeField] private Image handsImage;
    [SerializeField] private Image darknessImage;

    [Header("Overlay Settings")]
    [SerializeField] private float startingScale = 1.5f;
    [SerializeField] private float endScale = 1.0f;

    [Header("Player Reference")]
    [SerializeField] private GameObject player;

    private PlayerSlowdown slowdown;
    private Image currentImage;

    public bool isActive { get; set; } = true;

    private void Start()
    {
        if (player == null ||
            mudImage == null ||
            handsImage == null ||
            darknessImage == null)
        {
            Debug.LogError(
                "SlowdownOverlay: Required reference is missing!",
                this
            );
            enabled = false;
            return;
        }

        slowdown = player.GetComponent<PlayerSlowdown>();

        if (slowdown == null)
        {
            Debug.LogError(
                "SlowdownOverlay: PlayerSlowdown is missing!",
                this
            );
            enabled = false;
            return;
        }

        // Hide all overlays when the scene starts.
        SetImageAlpha(mudImage, 0f);
        SetImageAlpha(handsImage, 0f);
        SetImageAlpha(darknessImage, 0f);

        // Select the overlay appropriate for the current zone.
        currentImage = slowdown.AtEnd ? handsImage : mudImage;

        UpdateOverlay(slowdown.GetSlowdownPercentage());
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        // Hands replace mud while the player is in the end zone.
        if (slowdown.AtEnd && currentImage != handsImage)
        {
            SwitchOverlay(handsImage);
        }
        else if (!slowdown.AtEnd && currentImage != mudImage)
        {
            SwitchOverlay(mudImage);
        }

        UpdateOverlay(slowdown.GetSlowdownPercentage());
    }

    public void UpdateOverlay(float slowdown)
    {
        float progress;
        float currentAlpha;
        float darknessAlpha;
        float currentScale;

        // 0-25%: Overlay gradually fades in.
        if (slowdown <= 0.25f)
        {
            progress = slowdown / 0.25f;
            currentAlpha = Mathf.Lerp(0f, 1f, progress);
            currentScale = startingScale;
            darknessAlpha = 0f;
        }
        // 25-75%: Overlay grows toward the center.
        else if (slowdown <= 0.75f)
        {
            progress = (slowdown - 0.25f) / 0.5f;
            currentAlpha = 1f;
            currentScale = Mathf.Lerp(startingScale, endScale, progress);
            darknessAlpha = 0f;
        }
        // 75-100%: Darkness gradually increases.
        else
        {
            progress = (slowdown - 0.75f) / 0.25f;
            currentAlpha = 1f;
            currentScale = endScale;
            darknessAlpha = Mathf.Lerp(0f, 1f, progress);
        }

        SetImageAlpha(currentImage, currentAlpha);
        SetImageAlpha(darknessImage, darknessAlpha);

        currentImage.rectTransform.localScale =
            new Vector3(currentScale, currentScale, 1f);
    }

    private void SwitchOverlay(Image newImage)
    {
        // Hide the previous overlay before selecting the next one.
        SetImageAlpha(currentImage, 0f);

        currentImage = newImage;
    }

    private void SetImageAlpha(Image image, float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}
