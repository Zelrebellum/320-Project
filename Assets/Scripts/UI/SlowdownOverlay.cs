using UnityEngine.UI;
using UnityEngine;

public class SlowdownOverlay : MonoBehaviour
{
    OverlayManager overlayManager;
    Image currentImage;
    Image mudImage;
    Image handsImage;
    Image darknessImage;
    [SerializeField] float startingScale = 1.5f;
    [SerializeField] float endScale = 1.0f;
    public bool isActive { get; set; } = true;
    private PlayerSlowdown slowdown;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        overlayManager = gameObject.GetComponentInParent<OverlayManager>();
        slowdown = overlayManager.GetPlayer().GetComponent<PlayerSlowdown>();
        mudImage = overlayManager.GetMudImage();
        handsImage = overlayManager.GetHandsImage();
        darknessImage = overlayManager.GetDarknessImage();

        currentImage = mudImage;

        //Color color = handsImage.color;
        //color.a = 0;
        //handsImage.color = color;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive)
        {
            return;
        }

        if (slowdown.AtEnd && currentImage != handsImage) { SwitchOverlay(handsImage);}
        if (!slowdown.AtEnd && currentImage != mudImage) { SwitchOverlay(mudImage); }

        UpdateOverlay(slowdown.GetSlowdownPercentage());
    }

    //updates overlay depending on the current slowdown value
    public void UpdateOverlay(float slowdown)
    {
        float progress;
        float currentAlpha;
        float darknessAlpha;
        float currentScale;

        //0-25% slowdown, hands slowly start appearing at edges of screen
        if(slowdown <= 0.25)
        {
            progress = slowdown/0.25f;
            currentAlpha = Mathf.Lerp(0, 1, progress);
            currentScale = startingScale;
            darknessAlpha = 0;
        }
        //25-75% hands slowly grow onto screen, taking up more space
        else if (slowdown <= 0.75)
        {
            progress = (slowdown - 0.25f) / 0.5f;
            currentAlpha = 1;
            currentScale = Mathf.Lerp(startingScale, endScale, progress);
            darknessAlpha = 0;
        }
        //75-100% screen starts to darken until player is dead
        else
        {
            progress = (slowdown - 0.75f) / 0.25f;
            currentAlpha = 1;
            currentScale = endScale;
            darknessAlpha = Mathf.Lerp(0, 1, progress);
        }

        Color color = currentImage.color;
        color.a = currentAlpha;
        currentImage.color = color;

        color = darknessImage.color;
        color.a = darknessAlpha;
        darknessImage.color = color;

        currentImage.rectTransform.localScale = new Vector3(currentScale, currentScale, 1.0f);
    }

    void SwitchOverlay(Image newImage)
    {
        Color color = currentImage.color;
        color.a = 0;
        currentImage .color = color;

        currentImage = newImage;
    }
}
