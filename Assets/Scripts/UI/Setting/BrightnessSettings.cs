using System;
using UnityEngine;
using UnityEngine.UI;

public class BrightnessSettings : MonoBehaviour
{
    [SerializeField] private Slider brightnessSlider;

    public static event Action<float> BrightnessChanged;

    public static float SavedBrightness =>
        Mathf.Clamp(PlayerPrefs.GetFloat("Brightness", 1f), 0.25f, 3f);

    private void OnEnable()
    {
        if (brightnessSlider == null)
        {
            Debug.LogWarning("Give Brightness Slider value", this);
            return;
        }

        brightnessSlider.minValue = 0.25f;
        brightnessSlider.maxValue = 3f;
        brightnessSlider.wholeNumbers = false;

        brightnessSlider.SetValueWithoutNotify(SavedBrightness);
        brightnessSlider.onValueChanged.AddListener(ChangeBrightness);
    }

    private void ChangeBrightness(float brightness)
    {
        brightness = Mathf.Clamp(brightness, 0.25f, 3f);

        PlayerPrefs.SetFloat("Brightness", brightness);
        BrightnessChanged?.Invoke(brightness);
    }

    private void OnDisable()
    {
        if (brightnessSlider != null)
        {
            brightnessSlider.onValueChanged.RemoveListener(ChangeBrightness);
        }

        PlayerPrefs.Save();
    }
}