using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class UITestManager : MonoBehaviour
{
    public GameObject pausePanel;
    public GameObject endPanel;

    void Start()
    {
        pausePanel.SetActive(false);
        endPanel.SetActive(false);
    }

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

    public void Resume()
    {
        pausePanel.SetActive(false);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}