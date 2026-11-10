using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject pauseMenuContent;
    [SerializeField] private GameObject settingsPanel;

    public static bool IsPaused { get; private set; }

    private GameObject currentOnboardingPanel;

    /// <summary>
    /// Start with the pause menu hidden and gameplay running
    /// </summary>
    private void Start()
    {
        ResumeGame();
    }

    /// <summary>
    /// Clear the button selection left
    /// </summary>
    private void ClearUISelection()
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    /// <summary>
    /// Pause gameplay, display the pause menu
    /// </summary>
    public void PauseGame()
    {
        // Prevent the pause menu from opening over onboarding
        if (currentOnboardingPanel != null)
        {
            return;
        }

        ClearUISelection();

        settingsPanel.SetActive(false);
        pauseMenuContent.SetActive(true);
        pausePanel.SetActive(true);

        SetPausedState(true);
    }

    /// <summary>
    /// Display onboarding and pause gameplay
    /// </summary>
    public void OpenOnboarding(GameObject panel)
    {
        if (panel == null || currentOnboardingPanel != null)
        {
            return;
        }

        ClearUISelection();

        currentOnboardingPanel = panel;

        settingsPanel.SetActive(false);
        pauseMenuContent.SetActive(true);
        pausePanel.SetActive(false);

        SetPausedState(true);
        currentOnboardingPanel.SetActive(true);
    }

    /// <summary>
    /// Close onboarding and the pause menu
    /// </summary>
    public void ResumeGame()
    {
        ClearUISelection();

        if (currentOnboardingPanel != null)
        {
            currentOnboardingPanel.SetActive(false);
            currentOnboardingPanel = null;
        }

        settingsPanel.SetActive(false);
        pauseMenuContent.SetActive(true);
        pausePanel.SetActive(false);

        SetPausedState(false);
    }

    /// <summary>
    /// Update the pause state
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
    /// Show Settings while keeping gameplay paused
    /// </summary>
    public void OpenSettings()
    {
        if (currentOnboardingPanel != null)
        {
            return;
        }

        ClearUISelection();

        pauseMenuContent.SetActive(false);
        settingsPanel.SetActive(true);
    }

    /// <summary>
    /// Return from Settings to the main pause
    /// </summary>
    public void CloseSettings()
    {
        ClearUISelection();

        settingsPanel.SetActive(false);
        pauseMenuContent.SetActive(true);
    }

    /// <summary>
    /// Restore gameplay time and return
    /// </summary>
    public void ReturnToMainMenu()
    {
        ClearUISelection();

        IsPaused = false;
        Time.timeScale = 1f;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("MainMenu");
    }

    /// <summary>
    /// Clear the pause state
    /// </summary>
    private void OnDestroy()
    {
        IsPaused = false;
        Time.timeScale = 1f;
    }
}