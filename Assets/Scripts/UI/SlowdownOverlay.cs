
using UnityEngine;
using UnityEngine.UI;

public class SlowdownOverlay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image mudImage;
    [SerializeField] private Image handsImage;
    [SerializeField] private Image darknessImage;
    [SerializeField] private GameObject manager;

    [Header("Overlay Settings")]
    [SerializeField] private float startingScale = 1.5f;
    [SerializeField] private float endScale = 1.0f;

    private SlothManager slothManager;

    private Image currentImage;

    public bool isActive { get; set; } = true;

    private void Start()
    {
        if (manager == null)
        {
            Debug.LogError("SlowdownOverlay: Manager reference is missing!", this);
            enabled = false;
            return;
        }

        if (mudImage == null || handsImage == null || darknessImage == null)
        {
            Debug.LogError("SlowdownOverlay: Overlay is missing an image reference! (check children of object)", this);
            enabled = false;
            return;
        }
        
        slothManager = manager.GetComponent<SlothManager>();

        // Hide all overlays when the scene starts.
        SetImageAlpha(mudImage, 0f);
        SetImageAlpha(handsImage, 0f);
        SetImageAlpha(darknessImage, 0f);

        // Select the overlay appropriate for the current zone.
        currentImage = slothManager.AtEnd ? handsImage : mudImage;

        UpdateOverlay(slothManager.GetSlowdownPercentage());
    }

    private void Update()
    {
        if (!isActive)
        {
            return;
        }

        // Hands replace mud while the player is in the end zone.
        if (slothManager.AtEnd && currentImage != handsImage)
        {
            SwitchOverlay(handsImage);
        }
        else if (!slothManager.AtEnd && currentImage != mudImage)
        {
            SwitchOverlay(mudImage);
        }

        UpdateOverlay(slothManager.GetSlowdownPercentage());
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
