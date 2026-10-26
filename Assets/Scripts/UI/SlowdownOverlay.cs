using UnityEngine;
using UnityEngine.UI;

public class SlowdownOverlay : MonoBehaviour
{
    [Header("Overlay Scale")]
    [SerializeField] private float startingScale = 1.5f;
    [SerializeField] private float endScale = 1f;

    private OverlayManager overlayManager;
    private PlayerSlowdown slowdown;

    private Image currentImage;
    private Image mudImage;
    private Image handsImage;
    private Image darknessImage;

    private bool imagesInitialized;
    private float nextPlayerSearch;

    // PlayerManager temporarily disables automatic updates during death.
    public bool isActive { get; set; } = true;

    /// <summary>
    /// Loads the overlay references and connects the player.
    /// </summary>
    private void Start()
    {
        if (!InitializeImages())
        {
            return;
        }

        BindPlayer(overlayManager.GetPlayer());

        // Hide both images before selecting the correct overlay.
        SetAlpha(mudImage, 0f);
        SetAlpha(handsImage, 0f);

        SelectOverlay();

        float amount =
            slowdown != null ? slowdown.GetSlowdownPercentage() : 0f;

        UpdateOverlay(amount);
    }

    /// <summary>
    /// Reads image references from the parent OverlayManager.
    /// Also supports calls made before Start.
    /// </summary>
    private bool InitializeImages()
    {
        if (imagesInitialized)
        {
            return true;
        }

        overlayManager = GetComponentInParent<OverlayManager>();

        if (overlayManager == null)
        {
            Debug.LogError(
                "SlowdownOverlay requires a parent OverlayManager.", this);

            enabled = false;
            return false;
        }

        mudImage = overlayManager.GetMudImage();
        handsImage = overlayManager.GetHandsImage();
        darknessImage = overlayManager.GetDarknessImage();

        imagesInitialized = true;
        return true;
    }

    /// <summary>
    /// Connects the component that provides the player's slowdown state.
    /// </summary>
    public void BindPlayer(GameObject player)
    {
        slowdown = player != null
            ? player.GetComponent<PlayerSlowdown>()
            : null;
    }

    /// <summary>
    /// Updates the overlay from the player's current slowdown.
    /// </summary>
    private void Update()
    {
        if (!isActive || !imagesInitialized)
        {
            return;
        }

        // Retry occasionally if the player was not available at startup.
        if (slowdown == null)
        {
            if (Time.unscaledTime >= nextPlayerSearch)
            {
                nextPlayerSearch = Time.unscaledTime + 1f;
                BindPlayer(overlayManager.GetPlayer());
            }

            if (slowdown == null)
            {
                return;
            }
        }

        SelectOverlay();
        UpdateOverlay(slowdown.GetSlowdownPercentage());
    }

    /// <summary>
    /// Uses mud during normal gameplay and hands in the end zone.
    /// </summary>
    private void SelectOverlay()
    {
        Image nextImage =
            slowdown != null && slowdown.AtEnd ? handsImage : mudImage;

        // Keep an available overlay working while references are incomplete.
        if (nextImage == null)
        {
            nextImage = mudImage != null ? mudImage : handsImage;
        }

        if (currentImage == nextImage)
        {
            return;
        }

        SetAlpha(currentImage, 0f);
        currentImage = nextImage;
    }

    /// <summary>
    /// Applies a normalized slowdown value.
    /// PlayerManager can also call this directly during death and respawn.
    /// </summary>
    public void UpdateOverlay(float amount)
    {
        if (!InitializeImages())
        {
            return;
        }

        amount = Mathf.Clamp01(amount);
        SelectOverlay();

        float overlayAlpha;
        float darknessAlpha;
        float overlayScale;

        if (amount <= 0.25f)
        {
            // 0-25%: fade the overlay into view.
            float progress = amount / 0.25f;

            overlayAlpha = Mathf.Lerp(0f, 1f, progress);
            overlayScale = startingScale;
            darknessAlpha = 0f;
        }
        else if (amount <= 0.75f)
        {
            // 25-75%: move the overlay further onto the screen.
            float progress = (amount - 0.25f) / 0.5f;

            overlayAlpha = 1f;
            overlayScale = Mathf.Lerp(startingScale, endScale, progress);
            darknessAlpha = 0f;
        }
        else
        {
            // 75-100%: darken the screen as the player approaches death.
            float progress = (amount - 0.75f) / 0.25f;

            overlayAlpha = 1f;
            overlayScale = endScale;
            darknessAlpha = Mathf.Lerp(0f, 1f, progress);
        }

        SetAlpha(currentImage, overlayAlpha);
        SetAlpha(darknessImage, darknessAlpha);

        if (currentImage != null)
        {
            currentImage.rectTransform.localScale =
                new Vector3(overlayScale, overlayScale, 1f);
        }
    }

    /// <summary>
    /// Changes transparency without changing the image's RGB color.
    /// </summary>
    private static void SetAlpha(Image image, float alpha)
    {
        if (image == null)
        {
            return;
        }

        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}