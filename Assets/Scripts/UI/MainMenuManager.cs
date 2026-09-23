using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject settingsPanel;
    public GameObject creditsPanel;

    /// <summary>
    /// Press Start game jump to Game Scene
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    /// <summary>
    /// Press Setting open the setting panel
    /// </summary>
    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }

    /// <summary>
    /// Close the setting panel
    /// </summary>
    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
    }

    /// <summary>
    /// Press Credits open the credits panel
    /// </summary>
    public void OpenCredits()
    {
        creditsPanel.SetActive(true);
    }

    /// <summary>
    /// Close the credits panel
    /// </summary>
    public void CloseCredits()
    {
        creditsPanel.SetActive(false);
    }

    /// <summary>
    /// Quit the game
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
