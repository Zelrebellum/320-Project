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

    /// <summary>
    /// Starts with the pause menu hidden and gameplay running.
    /// </summary>
    private void Start()
    {
        ResumeGame();
    }

    /// <summary>
    /// Pauses gameplay, displays the pause menu,
    /// and releases the mouse cursor.
    /// </summary>
    public void PauseGame()
    {
        // Always open the main pause menu instead of Settings.
        settingsPanel.SetActive(false);
        pauseMenuContent.SetActive(true);

        IsPaused = true;
        Time.timeScale = 0f;
        pausePanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Resumes gameplay, hides the pause menu,
    /// and locks the mouse cursor.
    /// </summary>
    public void ResumeGame()
    {
        // Reset the menu pages for the next pause.
        settingsPanel.SetActive(false);
        pauseMenuContent.SetActive(true);

        IsPaused = false;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// Shows Settings while keeping gameplay paused.
    /// </summary>
    public void OpenSettings()
    {
        pauseMenuContent.SetActive(false);
        settingsPanel.SetActive(true);
    }

    /// <summary>
    /// Returns from Settings to the main pause menu.
    /// </summary>
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        pauseMenuContent.SetActive(true);
    }

    /// <summary>
    /// Restores the time scale and returns to the main menu.
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
    /// Clears the pause state when this object is destroyed,
    /// including when its scene is unloaded.
    /// </summary>
    private void OnDestroy()
    {
        IsPaused = false;
        Time.timeScale = 1f;
    }
}