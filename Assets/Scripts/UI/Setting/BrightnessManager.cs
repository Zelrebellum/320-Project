using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Volume))]
public class BrightnessManager : MonoBehaviour
{
    public static BrightnessManager Instance { get; private set; }

    private VolumeProfile runtimeProfile;
    private ColorAdjustments colorAdjustments;

    private void Awake()
    {
        // if have manager destroy
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Volume volume = GetComponent<Volume>();

        if (volume.sharedProfile == null)
        {
            Debug.LogError("BrightnessManager no setting Volume Profile¡£", this);
            enabled = false;
            return;
        }

        runtimeProfile = volume.profile;

        if (!runtimeProfile.TryGet<ColorAdjustments>(out colorAdjustments))
        {
            Debug.LogError("Profile in miss Color Adjustments¡£", this);
            enabled = false;
            return;
        }

        colorAdjustments.active = true;
        colorAdjustments.postExposure.overrideState = true;
    }

    private void OnEnable()
    {
        if (Instance != this || colorAdjustments == null)
            return;

        BrightnessSettings.BrightnessChanged += ApplyBrightness;

        // restore saved brightness
        ApplyBrightness(BrightnessSettings.SavedBrightness);
    }

    private void ApplyBrightness(float brightness)
    {
        brightness = Mathf.Clamp(brightness, 0.25f, 3f);

        // slider 1 is 0 
        colorAdjustments.postExposure.value = Mathf.Log(brightness, 2f);
    }

    private void OnDisable()
    {
        BrightnessSettings.BrightnessChanged -= ApplyBrightness;
    }

    private void OnDestroy()
    {
        if (Instance != this)
            return;

        Instance = null;

        if (runtimeProfile != null)
            Destroy(runtimeProfile);
    }
}