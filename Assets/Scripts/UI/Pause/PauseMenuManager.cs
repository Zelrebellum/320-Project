using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject pauseMenuContent;
    [SerializeField] private GameObject settingsPanel;

    public static bool IsPaused { get; private set; }

    /// <summary>
    /// Start with the pause menu hidden
    /// </summary>
    private void Start()
    {
        ResumeGame();
    }

    /// <summary>
    /// Press Escape to pause or resume the game
    /// </summary>
    private void Update()
    {
        if (BlinkTransition.IsPlaying)
        {
            return;
        }
    }

    /// <summary>
    /// Pause the game and show the mouse cursor
    /// </summary>
    public void PauseGame()
    {
        settingsPanel.SetActive(false);
        pauseMenuContent.SetActive(true);

        IsPaused = true;
        Time.timeScale = 0f;
        pausePanel.SetActive(true);

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Resume the game and hide the mouse cursor
    /// </summary>
    public void ResumeGame()
    {
        settingsPanel.SetActive(false);
        pauseMenuContent.SetActive(true);

        IsPaused = false;
        Time.timeScale = 1f;
        pausePanel.SetActive(false);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Menu(InputAction.CallbackContext context)
    {
        if (!context.performed ||
            BlinkTransition.IsPlaying)
        {
            return;
        }
        
        if (IsPaused)
            ResumeGame();
        else
            PauseGame();
    }

    /// <summary>
    /// Restore the time scale when this object is destroyed
    /// </summary>
    private void OnDestroy()
    {
        Time.timeScale = 1f;
        IsPaused = false;
    }

    /// <summary>
    /// Show settings and hide the pause menu buttons
    /// </summary>
    public void OpenSettings()
    {
        pauseMenuContent.SetActive(false);
        settingsPanel.SetActive(true);
    }

    /// <summary>
    /// Close settings and show the pause menu buttons
    /// </summary>
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        pauseMenuContent.SetActive(true);
    }

    /// <summary>
    /// Restore the time scale and return to the main menu
    /// </summary>
    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        IsPaused = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("MainMenu");
    }
}