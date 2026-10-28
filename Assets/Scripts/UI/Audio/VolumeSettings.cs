using UnityEngine;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;

    /// <summary>
    /// Load the saved master volume when this control opens.
    /// </summary>
    private void OnEnable()
    {
        if (volumeSlider == null)
        {
            Debug.LogError(
                "VolumeSettings: Volume Slider is not assigned.", this);
            return;
        }

        float volume = Mathf.Clamp01(
            PlayerPrefs.GetFloat("MasterVolume", 1f));

        volumeSlider.SetValueWithoutNotify(volume);
        AudioListener.volume = volume;

        volumeSlider.onValueChanged.AddListener(ChangeVolume);
    }

    /// <summary>
    /// Apply and store changes immediately.
    /// </summary>
    private void ChangeVolume(float volume)
    {
        volume = Mathf.Clamp01(volume);

        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("MasterVolume", volume);
    }

    /// <summary>
    /// Save settings and disconnect the listener when closed.
    /// </summary>
    private void OnDisable()
    {
        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(ChangeVolume);
        }

        PlayerPrefs.Save();
    }
}