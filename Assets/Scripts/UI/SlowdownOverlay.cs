using UnityEngine.UI;
using UnityEngine;

public class SlowdownOverlay : MonoBehaviour
{
    [SerializeField] Image handsImage;
    [SerializeField] RectTransform handsRectTransform;
    [SerializeField] Image darknessImage;
    [SerializeField] float startingScale = 1.5f;
    [SerializeField] float endScale = 1.0f;
    public bool isActive { get; set; } = true;

    [SerializeField] private GameObject player;
    private PlayerSlowdown slowdown;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
       slowdown = player.GetComponent<PlayerSlowdown>(); 
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive)
        {
            return;
        }

        UpdateOverlay(slowdown.GetSlowdownPercentage());
    }

    //updates overlay depending on the current slowdown value
    public void UpdateOverlay(float slowdown)
    {
        float progress;
        float handsAlpha;
        float darknessAlpha;
        float handsScale;

        //0-25% slowdown, hands slowly start appearing at edges of screen
        if(slowdown <= 0.25)
        {
            progress = slowdown/0.25f;
            handsAlpha = Mathf.Lerp(0, 1, progress);
            handsScale = startingScale;
            darknessAlpha = 0;
        }
        //25-75% hands slowly grow onto screen, taking up more space
        else if (slowdown <= 0.75)
        {
            progress = (slowdown - 0.25f) / 0.5f;
            handsAlpha = 1;
            handsScale = Mathf.Lerp(startingScale, endScale, progress);
            darknessAlpha = 0;
        }
        //75-100% screen starts to darken until player is dead
        else
        {
            progress = (slowdown - 0.75f) / 0.25f;
            handsAlpha = 1;
            handsScale = endScale;
            darknessAlpha = Mathf.Lerp(0, 1, progress);
        }

        Color color = handsImage.color;
        color.a = handsAlpha;
        handsImage.color = color;

        color = darknessImage.color;
        color.a = darknessAlpha;
        darknessImage.color = color;

        handsRectTransform.localScale = new Vector3(handsScale, handsScale, 1.0f);
    }
}
