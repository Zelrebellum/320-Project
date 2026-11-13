using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OnboardingPanel : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private Button continueButton;
    [Header("Keyboard Settings")]
    private Key closeKey;

    public bool IsConfigured => titleText != null && bodyText != null && continueButton != null;

    // press key to go next
    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard != null && keyboard[closeKey].wasPressedThisFrame)
        {
            Close();
        }
    }

    /// <summary>
    /// Connect the button locally so this prefab needs no scene button events.
    /// </summary>
    private void OnEnable()
    {
        if (continueButton != null)
        {
            continueButton.onClick.AddListener(Close);
        }
    }

    private void OnDisable()
    {
        if (continueButton != null)
        {
            continueButton.onClick.RemoveListener(Close);
        }
    }

    /// <summary>
    /// Update the shared panel with the current trigger's instructions.
    /// </summary>
    public void SetContent(string title, string instructions, Key tutorialCloseKey)
    {
        titleText.text = title;
        bodyText.text = instructions;
        closeKey = tutorialCloseKey;
    }

    /// <summary>
    /// Find the owning UI locally and restore gameplay through its manager.
    /// </summary>
    public void Close()
    {
        UIManager manager = GetComponentInParent<UIManager>();
        if (manager != null)
        {
            manager.ResumeGame();
        }
    }
}
