using UnityEngine;
using UnityEngine.InputSystem;

public class Dash : MonoBehaviour
{
    [SerializeField] private float impulseForce = 8.0f;
    private bool dashing;
    [SerializeField, Range(0.5f, 10.0f)] private float dashCooldownDuration;
    [Header("FOV")]

    [SerializeField] private float dashFOV = 120;

    [SerializeField] private float FOVfade = 1.5f;

    private PlayerHUD playerHUD;
    private float cooldownRemaining;

    public float CoolDown
    {
        get => cooldownRemaining;
    }
    
    private PlayerMovement movement;
    private PlayerSlowdown slowdown;

    private Rigidbody rBody;

    private CameraController camControl;

    private WallRun wall;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movement = gameObject.GetComponent<PlayerMovement>();
        rBody = gameObject.GetComponent<Rigidbody>();
        camControl = gameObject.GetComponent<CameraController>();
        playerHUD = movement.UI;
        Debug.Log($"Movement HUD: {movement.UI}");
        Debug.Log($"Dash HUD: {playerHUD}");
        slowdown = gameObject.GetComponent<PlayerSlowdown>();
        wall = gameObject.GetComponent<WallRun>();
    }

    // Update is called once per frame
    private void Update()
    {
        cooldownRemaining -= Time.deltaTime;

        if (!dashing)
        {
            return;
        }

        if (cooldownRemaining < dashCooldownDuration - 0.7f)
        {
            camControl.EnableFOV(camControl.StartFOV, FOVfade);
        }

        Debug.Log($"PlayerHUD: {playerHUD}");

        if (cooldownRemaining < 0f)
        {
            dashing = false;
            playerHUD.UpdateDashCooldown(1f);
            return;
        }

        playerHUD.UpdateDashCooldown((dashCooldownDuration - cooldownRemaining) / dashCooldownDuration);
    }

    public void ResetDash()
    {
        dashing = false;
        cooldownRemaining = 0;
        playerHUD.UpdateDashCooldown(1f);
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        // if the button's been canceled, or if we're already dashing, or if the cooldown hasn't been hit, do nothing
        if (!context.performed
         || dashing 
         || cooldownRemaining > 0.0f 
         || movement.IsCrouching
         || wall.Wallrunning
         )
        {
            return;
        }

        rBody.AddForce(camControl.Camera.transform.forward * impulseForce, ForceMode.Impulse);
        slowdown.ReduceSlowdown(10.0f);
        dashing = true;

        cooldownRemaining = dashCooldownDuration;
        camControl.EnableFOV(dashFOV, 75f);
    }
}
