using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class UIButtonAudio : MonoBehaviour
{
    [Header("Button Audio")]
    [SerializeField] private AudioClip clickSound;

    private AudioSource audioSource;
    private Button[] buttons;

    /// <summary>
    /// Configure a shared audio source outside individual panels.
    /// </summary>
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = false;
        audioSource.spatialBlend = 0f;
        audioSource.volume = 1f;
        audioSource.ignoreListenerPause = true;

        AudioListener.volume = Mathf.Clamp01(
            PlayerPrefs.GetFloat("MasterVolume", 1f));
    }

    /// <summary>
    /// Connect all existing child buttons, including inactive ones.
    /// </summary>
    private void OnEnable()
    {
        buttons = GetComponentsInChildren<Button>(true);

        foreach (Button button in buttons)
        {
            button.onClick.AddListener(PlayClick);
        }
    }

    /// <summary>
    /// Remove listeners when this UI is disabled.
    /// </summary>
    private void OnDisable()
    {
        if (buttons == null)
        {
            return;
        }

        foreach (Button button in buttons)
        {
            if (button != null)
            {
                button.onClick.RemoveListener(PlayClick);
            }
        }
    }

    /// <summary>
    /// Play through the shared source so closing a panel
    /// does not stop its button sound.
    /// </summary>
    private void PlayClick()
    {
        if (clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}