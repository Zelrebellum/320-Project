using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class UITestManager : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject endPanel;

    /// <summary>
    /// Start default
    /// </summary>
    void Start()
    {
        pausePanel.SetActive(false);
        endPanel.SetActive(false);
    }

    /// <summary>
    /// ESC controls pause panel E to test end screen
    /// </summary>
    void Update()
    {
        // Press ESC to open / close Pause Menu
        if (Keyboard.current != null &&
            Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            pausePanel.SetActive(!pausePanel.activeSelf);
        }

        // Press E to test End Screen
        if (Keyboard.current != null &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            pausePanel.SetActive(false);
            endPanel.SetActive(true);
        }
    }

    /// <summary>
    /// Back to game
    /// </summary>
    public void Resume()
    {
        pausePanel.SetActive(false);
    }

    /// <summary>
    /// Load to main menu
    /// </summary>
    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void TestEndingCredits()
    {
        SceneManager.LoadScene("Credits");
    }
}