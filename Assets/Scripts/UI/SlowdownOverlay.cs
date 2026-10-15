using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;

public class SlowdownOverlay : MonoBehaviour
{
    Image handsImage;
    RectTransform handsRectTransform;
    Image darknessImage;
    float startingScale = 1.5f;
    float endScale = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateOverlay(float slowdown)
    {
        float progress;
        float handsAlpha;
        float darknessAlpha;

        if(slowdown <= 0.25)
        {
            progress = slowdown/0.25f;
            handsAlpha = Mathf.Lerp(0, 1, progress);
        }
    }
}
