using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public GameObject optionsPanel;
    public GameObject creditsPanel;

    /// <summary>
    /// Press Start game jump to Game Scene
    /// </summary>
    public void StartGame()
    {
        SceneManager.LoadScene("UITest");
    }

    /// <summary>
    /// Press Setting open the Options panel
    /// </summary>
    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
    }

    /// <summary>
    /// Close the Options panel
    /// </summary>
    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
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
