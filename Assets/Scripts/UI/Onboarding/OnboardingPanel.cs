using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OnboardingPanel : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text bodyText;
    [SerializeField] private Button continueButton;

    public bool IsConfigured => titleText != null && bodyText != null && continueButton != null;

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
    public void SetContent(string title, string instructions)
    {
        titleText.text = title;
        bodyText.text = instructions;
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
