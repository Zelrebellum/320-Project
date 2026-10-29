
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject pauseMenuContent;
    [SerializeField] private GameObject settingsPanel;

    // Allows other scripts to read the pause state.
    // Only this class can change its value.
    public static bool IsPaused { get; private set; }

    // Tracks the onboarding panel currently displayed.
    private GameObject currentOnboardingPanel;

    /// <summary>
    /// Start with the pause menu hidden and gameplay running.
    /// </summary>
    private void Start()
    {
        ResumeGame();
    }

    /// <summary>
    /// Pause gameplay, display the pause menu,
    /// and release the mouse cursor.
    /// </summary>
    public void PauseGame()
    {
        // Prevent the pause menu from opening over onboarding.
        if (currentOnboardingPanel != null)
        {
            return;
        }

        settingsPanel.SetActive(false);
        pauseMenuContent.SetActive(true);
        pausePanel.SetActive(true);

        SetPausedState(true);
    }

    /// <summary>
    /// Display onboarding, pause gameplay,
    /// and release the mouse cursor.
    /// </summary>
    public void OpenOnboarding(GameObject panel)
    {
        if (panel == null || currentOnboardingPanel != null)
        {
            return;
        }

        currentOnboardingPanel = panel;

        // Hide the pause menu while displaying onboarding.
        settingsPanel.SetActive(false);
        pauseMenuContent.SetActive(true);
        pausePanel.SetActive(false);

        SetPausedState(true);
        currentOnboardingPanel.SetActive(true);
    }

    /// <summary>
    /// Close onboarding and the pause menu,
    /// then restore gameplay and lock the cursor.
    /// </summary>
    public void ResumeGame()
    {
        if (currentOnboardingPanel != null)
        {
            currentOnboardingPanel.SetActive(false);
            currentOnboardingPanel = null;
        }

        // Reset the menu pages for the next pause.
        settingsPanel.SetActive(false);
        pauseMenuContent.SetActive(true);
        pausePanel.SetActive(false);

        SetPausedState(false);
    }

    /// <summary>
    /// Update the pause state, gameplay time,
    /// and mouse cursor together.
    /// </summary>
    private void SetPausedState(bool paused)
    {
        IsPaused = paused;
        Time.timeScale = paused ? 0f : 1f;

        Cursor.lockState = paused
            ? CursorLockMode.None
            : CursorLockMode.Locked;

        Cursor.visible = paused;
    }

    /// <summary>
    /// Show Settings while keeping gameplay paused.
    /// </summary>
    public void OpenSettings()
    {
        // Keep Settings from opening over onboarding.
        if (currentOnboardingPanel != null)
        {
            return;
        }

        pauseMenuContent.SetActive(false);
        settingsPanel.SetActive(true);
    }

    /// <summary>
    /// Return from Settings to the main pause menu.
    /// </summary>
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        pauseMenuContent.SetActive(true);
    }

    /// <summary>
    /// Restore the time scale and return to the main menu.
    /// </summary>
    public void ReturnToMainMenu()
    {
        IsPaused = false;
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Clear the pause state when this object is destroyed,
    /// including when its scene is unloaded.
    /// </summary>
    private void OnDestroy()
    {
        IsPaused = false;
        Time.timeScale = 1f;
    }
}
