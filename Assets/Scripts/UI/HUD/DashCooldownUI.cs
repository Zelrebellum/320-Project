using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class DashCooldownUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Dash dashSource;

    [Header("Visibility")]
    [SerializeField, Min(0f)] private float readyHoldTime = 0.4f;
    [SerializeField, Min(0.01f)] private float fadeDuration = 0.3f;
    
    [Header("Cooldown Bar")]
    [SerializeField] private Image cooldownBar;
    [SerializeField] private Color onCooldownBarColor;
    [SerializeField] private Color wrathCooldownBarColor;
    private Color safeCooldownBarColor;

    private CanvasGroup canvasGroup;
    private bool wasCoolingDown;
    private float holdTimer;

    /// <summary>
    /// Starts with the cooldown bar hidden.
    /// </summary>
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        
        safeCooldownBarColor = cooldownBar.color;
    }

    /// <summary>
    /// Shows the bar during cooldown, then holds and fades it out.
    /// </summary>
    private void Update()
    {
        if (dashSource == null || !dashSource.isActiveAndEnabled)
        {
            canvasGroup.alpha = 0f;
            wasCoolingDown = false;
            holdTimer = 0f;
            return;
        }
        
        UpdateDashColor();

        // Keep the bar visible while the gameplay cooldown is active.
        if (dashSource.CoolDown > 0f)
        {
            canvasGroup.alpha = 1f;
            wasCoolingDown = true;
            holdTimer = readyHoldTime;
            return;
        }

        // Detect the moment the cooldown finishes.
        if (wasCoolingDown)
        {
            wasCoolingDown = false;
            holdTimer = readyHoldTime;
        }

        // Briefly display the completed bar.
        if (holdTimer > 0f)
        {
            holdTimer -= Time.deltaTime;
            return;
        }

        // Fade out without disabling the bar's GameObject.
        // Scaled time keeps the fade paused when gameplay is paused.
        float fadeStep =
            Time.deltaTime / Mathf.Max(0.01f, fadeDuration);

        canvasGroup.alpha = Mathf.MoveTowards(
            canvasGroup.alpha,
            0f,
            fadeStep);
    }

    /// <summary>
    /// Clears the visual state when this component is disabled.
    /// </summary>
    private void OnDisable()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        wasCoolingDown = false;
        holdTimer = 0f;
    }

    void UpdateDashColor()
    {
        if (dashSource.CurrentWrathState == WrathState.Active)
        {
            cooldownBar.color = wrathCooldownBarColor;
        }
        else if (dashSource.CurrentWrathState == WrathState.Recovery)
        {
            cooldownBar.color = safeCooldownBarColor;

        }
        else if (dashSource.CurrentWrathState == WrathState.Building)
        {
            Color currentCoolDownBarColor = cooldownBar.color;
            
            switch (dashSource.DashCooldownState)
            {
                case DashState.OnCooldown:
                    currentCoolDownBarColor = onCooldownBarColor;
                    break;
                case DashState.WrathReady:
                    currentCoolDownBarColor = wrathCooldownBarColor;
                    break;
                case DashState.SafeReady:
                    currentCoolDownBarColor = safeCooldownBarColor;
                    break;
            }

            cooldownBar.color = currentCoolDownBarColor;
        }
    }
}