using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public enum DashState
{
    OnCooldown,
    WrathReady,
    SafeReady
}

public class Dash : MonoBehaviour
{
    [Header("Dash Momentum")]
    [SerializeField, Min(0f)]
    private float dashMomentumDuration = 0.2f;

    private float dashMomentumTimer;

    public bool HasDashMomentum => dashMomentumTimer > 0f;
    [SerializeField] private float impulseForce = 8.0f;
    [SerializeField, Range(0.5f, 10.0f)] private float dashCooldownDuration = 2.5f;
    [Header("FOV")]

    [SerializeField] private float dashFOV = 120;
    [SerializeField] private float FOVfade = 1.5f;
    private bool dashFOVActive;

    [Header("Cooldown")]
    private PlayerHUD playerHUD;
    [SerializeField] private float cooldownRemaining;
    public WrathState CurrentWrathState => wrath.CurrentWrathState; 
    
    private PlayerMovement movement;
    private Rigidbody rBody;
    private CameraController camControl;
    private WallRun wall;
    private WrathManager wrath;
    private SlothManager slowdown;
    public bool IsWrathEnabled => wrath.isActiveAndEnabled;
    
    public float CoolDown
    {
        get
        {
            if (wrath.isActiveAndEnabled && wrath.CurrentWrathState == WrathState.Active)
            {
                return Mathf.Max(
                    0f,
                    cooldownRemaining - (dashCooldownDuration - wrath.WrathDashCooldown)
                );
            }

            return cooldownRemaining;
        }
    }
    
    public DashState DashCooldownState
    {
        get
        {
            if(cooldownRemaining <= 0f && (!wrath.isActiveAndEnabled || wrath.CurrentWrathState != WrathState.Active))
            {
                return DashState.SafeReady;
            }
            if (wrath.isActiveAndEnabled && cooldownRemaining <= dashCooldownDuration - wrath.WrathDashCooldown
                                         && wrath.CurrentWrathState != WrathState.Recovery)
            {
                return DashState.WrathReady;
            }
            return DashState.OnCooldown;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        
        movement = gameObject.GetComponent<PlayerMovement>();
        rBody = gameObject.GetComponent<Rigidbody>();
        camControl = gameObject.GetComponent<CameraController>();
        playerHUD = movement.UI;
        slowdown = movement.Manager.GetComponent<SlothManager>();
        wrath = movement.Manager.GetComponent<WrathManager>();
        wall = gameObject.GetComponent<WallRun>();
    }

    // Update is called once per frame
    private void Update()
    {
        cooldownRemaining = Mathf.Clamp(
            cooldownRemaining - Time.deltaTime,
            0f,
            dashCooldownDuration
        );
        
        float effectiveCooldown = dashCooldownDuration;

        if (wrath.isActiveAndEnabled && wrath.CurrentWrathState == WrathState.Active)
        {
            effectiveCooldown = wrath.WrathDashCooldown;
        }
        
        if (dashFOVActive &&
            cooldownRemaining <= dashCooldownDuration - 0.7f)
        {
            camControl.EnableFOV(camControl.BaseFOV, FOVfade);
            dashFOVActive = false;
        }
        
        float progress = 1f - (CoolDown / effectiveCooldown);

        playerHUD.UpdateDashCooldown(Mathf.Clamp01(progress));
        
    }

    public void FixedUpdate()
    {
        if (dashMomentumTimer > 0f)
        {
            dashMomentumTimer = Mathf.Max(
                0f,
                dashMomentumTimer - Time.deltaTime
            );
        }
    }

    public void ResetDash()
    {
        dashFOVActive = false;
        cooldownRemaining = 0;
        dashMomentumTimer = 0f;
        playerHUD.UpdateDashCooldown(1f);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        
        if (DashCooldownState == DashState.OnCooldown
            || movement.IsCrouching
            || wall.Wallrunning)
        {
            return;
        }
        
        //checks if player dashed while on cooldown (only possible when wrath is in effect)
        bool wrathDash = cooldownRemaining > 0f;
        
        //if wrath state is active, more powerful dash
        float effectiveDashForce = impulseForce;
        if (wrath.isActiveAndEnabled && wrath.CurrentWrathState == WrathState.Active)
        {
            effectiveDashForce *= wrath.DashForceMultiplier;
        }
        
        rBody.AddForce(camControl.Camera.transform.forward * effectiveDashForce, ForceMode.Impulse);
        
        //if wrath is active notify wrathmanager of recent dash and add to wrath meter if on wrath cooldown
        if (wrath.isActiveAndEnabled)
        {
            wrath.NotifyDash();
        
            if (wrathDash)
            {
                wrath.AddWrath(wrath.DashWrathGain);
            }
        }
        
        dashMomentumTimer = dashMomentumDuration;
        if (slowdown.isActiveAndEnabled)
        {
            slowdown.ReduceSlowdown(10.0f);

        }
        cooldownRemaining = dashCooldownDuration;
        dashFOVActive = true;
        camControl.EnableFOV(dashFOV, 75f);
    }
}
