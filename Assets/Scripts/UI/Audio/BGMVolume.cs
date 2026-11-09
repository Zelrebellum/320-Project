using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BGMVolume : MonoBehaviour
{
    private AudioSource audioSource;
    private float originalVolume;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        // save original volume
        originalVolume = audioSource.volume;
    }

    private void OnEnable()
    {
        VolumeSettings.BGMVolumeChanged += ApplyVolume;

        // into secne load volume
        ApplyVolume(PlayerPrefs.GetFloat("BGMVolume", 1f));
    }

    private void ApplyVolume(float volume)
    {
        audioSource.volume = originalVolume * Mathf.Clamp01(volume);
    }

    private void OnDisable()
    {
        VolumeSettings.BGMVolumeChanged -= ApplyVolume;
    }
}