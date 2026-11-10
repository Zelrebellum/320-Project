using System;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider uiSlider;

    public static event Action<float> BGMVolumeChanged;

    /// <summary>
    /// Load saved volumes and connect slider listeners
    /// </summary>
    private void OnEnable()
    {
        if (volumeSlider == null || bgmSlider == null || uiSlider == null)
        {
            Debug.LogError(
                "VolumeSettings: Assign the Master, BGM, and UI sliders.", this);
            return;
        }

        float masterVolume = LoadVolume("MasterVolume");
        float bgmVolume = LoadVolume("BGMVolume");
        float uiVolume = LoadVolume("UIVolume");

        SetupSlider(volumeSlider, masterVolume);
        SetupSlider(bgmSlider, bgmVolume);
        SetupSlider(uiSlider, uiVolume);

        AudioListener.volume = masterVolume;

        volumeSlider.onValueChanged.AddListener(ChangeVolume);
        bgmSlider.onValueChanged.AddListener(ChangeBGMVolume);
        uiSlider.onValueChanged.AddListener(ChangeUIVolume);
    }

    /// <summary>
    /// Load a saved volume, using full volume as the default
    /// </summary>
    private float LoadVolume(string key)
    {
        return Mathf.Clamp01(PlayerPrefs.GetFloat(key, 1f));
    }

    /// <summary>
    /// Configure the slider without triggering its listeners
    /// </summary>
    private void SetupSlider(Slider slider, float volume)
    {
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.wholeNumbers = false;

        slider.SetValueWithoutNotify(volume);
    }

    /// <summary>
    /// Apply and store the master volume immediately
    /// </summary>
    private void ChangeVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);

        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    /// <summary>
    /// Store the music volume and notify active music sources
    /// </summary>
    private void ChangeBGMVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);

        PlayerPrefs.SetFloat("BGMVolume", volume);
        BGMVolumeChanged?.Invoke(volume);
    }

    /// <summary>
    /// Store the volume used by UI button sounds
    /// </summary>
    private void ChangeUIVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);

        PlayerPrefs.SetFloat("UIVolume", volume);
    }

    /// <summary>
    /// Save settings and disconnect listeners when closed
    /// </summary>
    private void OnDisable()
    {
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(ChangeVolume);
        }

        if (bgmSlider != null)
        {
            bgmSlider.onValueChanged.RemoveListener(ChangeBGMVolume);
        }

        if (uiSlider != null)
        {
            uiSlider.onValueChanged.RemoveListener(ChangeUIVolume);
        }

        PlayerPrefs.Save();
    }
}