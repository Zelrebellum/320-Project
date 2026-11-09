using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Volume))]
public class BrightnessController : MonoBehaviour
{
    private ColorAdjustments colorAdjustments;

    /// <summary>
    /// 
    /// </summary>
    private void OnEnable()
    {
        Volume volume = GetComponent<Volume>();

        if (volume.sharedProfile == null ||
            !volume.profile.TryGet<ColorAdjustments>(out colorAdjustments))
        {
            Debug.LogWarning("Please Volume Profile add Color Adjustments¡£", this);
            return;
        }

        colorAdjustments.active = true;
        colorAdjustments.postExposure.overrideState = true;

        BrightnessSettings.BrightnessChanged += ApplyBrightness;
        ApplyBrightness(BrightnessSettings.SavedBrightness);
    }

    private void ApplyBrightness(float brightness)
    {
        colorAdjustments.postExposure.value = Mathf.Log(brightness, 2f);
    }

    private void OnDisable()
    {
        BrightnessSettings.BrightnessChanged -= ApplyBrightness;
    }
}