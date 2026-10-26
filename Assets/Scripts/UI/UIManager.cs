using UnityEngine;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    [Header("UI References")]

    // Gameplay HUD containing cooldown indicators, icons, and overlays.
    // SerializeField allows this private field to appear in the Inspector.
    [SerializeField] private GameObject gameplayHUD;

    // Handles the pause menu, game time, and cursor state.
    [SerializeField] private PauseMenuManager pauseMenuManager;

    // Handles the eye blink transition animation.
    [SerializeField] private BlinkTransition blinkTransition;

    /// <summary>
    /// Shows the gameplay HUD.
    /// </summary>
    public void ShowGameplayHUD()
    {
        // Check that the reference is assigned before using it.
        if (gameplayHUD != null)
        {
            gameplayHUD.SetActive(true);
        }
    }

    /// <summary>
    /// Hides the gameplay HUD.
    /// </summary>
    public void HideGameplayHUD()
    {
        if (gameplayHUD != null)
        {
            gameplayHUD.SetActive(false);
        }
    }

    /// <summary>
    /// Pauses the game through the existing pause menu manager.
    /// </summary>
    // Use this component menu option during Play Mode for testing.
    [ContextMenu("Test/Pause Game")]
    public void PauseGame()
    {
        if (pauseMenuManager != null)
        {
            pauseMenuManager.PauseGame();
        }
    }

    /// <summary>
    /// Resumes the game through the existing pause menu manager.
    /// </summary>
    [ContextMenu("Test/Resume Game")]
    public void ResumeGame()
    {
        if (pauseMenuManager != null)
        {
            pauseMenuManager.ResumeGame();
        }
    }

    /// <summary>
    /// Plays the eye blink transition.
    /// </summary>
    public void PlayBlink()
    {
        if (blinkTransition != null)
        {
            blinkTransition.PlayBlink();
        }
    }

    /// <summary>
    /// Receives the menu input from PlayerInput, such as Escape,
    /// and toggles between pausing and resuming the game.
    /// </summary>
    public void Menu(InputAction.CallbackContext context)
    {
        // Only handle the performed phase to avoid repeated toggles.
        // Ignore menu input while the blink animation is playing.
        if (!context.performed || BlinkTransition.IsPlaying)
        {
            return;
        }

        // Access the static property through the class name.
        if (PauseMenuManager.IsPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }
}