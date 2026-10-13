using UnityEngine;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;

    /// <summary>
    /// Load the saved volume whenever Settings opens
    /// </summary>
    private void OnEnable()
    {
        float volume = PlayerPrefs.GetFloat("Volume", 1f);

        volumeSlider.SetValueWithoutNotify(volume);
        AudioListener.volume = volume;

        volumeSlider.onValueChanged.AddListener(ChangeVolume);
    }

    /// <summary>
    /// Remove the listener and save when Settings closes
    /// </summary>
    private void OnDisable()
    {
        volumeSlider.onValueChanged.RemoveListener(ChangeVolume);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Apply and store the new volume
    /// </summary>
    /// <param name="volume">value of volume</param>
    private void ChangeVolume(float volume)
    {
        AudioListener.volume = volume;
        PlayerPrefs.SetFloat("Volume", volume);
    }
}