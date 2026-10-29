using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class OnboardingTrigger : MonoBehaviour
{
    [Header("Tutorial Content")]
    [SerializeField] private string title = "TUTORIAL TITLE";
    [SerializeField, TextArea(3, 10)]
    private string instructions = "Tutorial instructions go here.";

    [Header("Trigger Settings")]
    [SerializeField] private bool triggerOnce = true;

    private bool hasTriggered;

    /// <summary>
    /// Configure the collider when this component is added.
    /// </summary>
    private void Reset()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        TryActivate(other.GetComponentInParent<PlayerMovement>());
    }

    /// <summary>
    /// Find UI in this scene and consume the trigger only after it opens.
    /// </summary>
    public bool TryActivate(PlayerMovement player)
    {
        if (player == null || (triggerOnce && hasTriggered))
        {
            return false;
        }

        foreach (UIManager manager in FindObjectsByType<UIManager>())
        {
            if (manager.gameObject.scene != gameObject.scene)
            {
                continue;
            }

            // Failed or blocked requests remain available for another entry.
            if (!manager.TryShowOnboarding(title, instructions))
            {
                return false;
            }

            hasTriggered = true;
            return true;
        }

        Debug.LogWarning("OnboardingTrigger: Add a configured UIHud prefab to this scene.", this);
        return false;
    }

    /// <summary>
    /// Allow this tutorial to activate again.
    /// </summary>
    public void ResetTrigger()
    {
        hasTriggered = false;
    }
}
