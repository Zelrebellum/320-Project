using UnityEngine;

public class SettingsTabs : MonoBehaviour
{
    [SerializeField] private GameObject controlsPage;
    [SerializeField] private GameObject instructionsPage;
    [SerializeField] private GameObject audioPage;

    // Show Controls as default
    private void OnEnable()
    {
        ShowControls();
    }

    /// <summary>
    /// Show controls close aduio, instruction
    /// </summary>
    public void ShowControls()
    {
        controlsPage.SetActive(true);
        instructionsPage.SetActive(false);
        audioPage.SetActive(false);
    }

    /// <summary>
    /// Show instruction close controls, audio
    /// </summary>
    public void ShowInstructions()
    {
        controlsPage.SetActive(false);
        instructionsPage.SetActive(true);
        audioPage.SetActive(false);
    }

    /// <summary>
    /// Show audio close controls, instruction
    /// </summary>
    public void ShowAudio()
    {
        controlsPage.SetActive(false);
        instructionsPage.SetActive(false);
        audioPage.SetActive(true);
    }
}