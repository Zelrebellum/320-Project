
using UnityEngine;

public class SettingsTabs : MonoBehaviour
{
    [Header("Settings Pages")]
    [SerializeField] private GameObject controlsPage;
    [SerializeField] private GameObject instructionsPage;
    [SerializeField] private GameObject audioPage;

    // Show Controls by default whenever Settings is opened.
    private void OnEnable()
    {
        ShowControls();
    }

    public void ShowControls()
    {
        controlsPage.SetActive(true);
        instructionsPage.SetActive(false);
        audioPage.SetActive(false);
    }

    public void ShowInstructions()
    {
        controlsPage.SetActive(false);
        instructionsPage.SetActive(true);
        audioPage.SetActive(false);
    }

    public void ShowAudio()
    {
        controlsPage.SetActive(false);
        instructionsPage.SetActive(false);
        audioPage.SetActive(true);
    }
}
