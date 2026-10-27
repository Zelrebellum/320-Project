
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    [Header("Pause Menu")]
    [Tooltip("The parent panel containing the pause menu UI.")]
    [SerializeField] private GameObject pausePanel;

    [Tooltip("The main pause menu buttons and content.")]
    [SerializeField] private GameObject pauseMenuContent;

    [Header("Settings")]
    [SerializeField] private GameObject settingsPanel;

    public static bool IsPaused { get; private set; }

    private void Start()
    {
        ResumeGame();
    }

    public void Menu(InputAction.CallbackContext context)
    {
        if (!context.performed || BlinkTransition.IsPlaying)
        {
            return;
        }

        if (IsPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

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

    public void OpenSettings()
    {
        pauseMenuContent.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        pauseMenuContent.SetActive(true);
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;
        IsPaused = false;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        SceneManager.LoadScene("MainMenu");
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
        IsPaused = false;
    }
}
