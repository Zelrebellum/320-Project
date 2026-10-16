using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class dash : MonoBehaviour
{
    [SerializeField] private float increaseSpeed;
    private float startSpeed;
    [SerializeField] private bool dashing = false;

    [SerializeField, Range(0.5f, 10.0f)] private float dashTime;
    private float waitTimer = 0;
    [SerializeField] private float dashFOV;

    [SerializeField] private float FOVfade;

    private RectTransform hudCooldown;

    private float coolDown;

    public float CoolDown
    {
        get => coolDown;
    }
    private PlayerMovement movement;
    private PlayerSlowdown slowdown;

    private Rigidbody rBody;

    private CameraController camControl;

    private wallRun wall;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movement = gameObject.GetComponent<PlayerMovement>();
        startSpeed = movement.targetSpeed;
        rBody = gameObject.GetComponent<Rigidbody>();
        camControl = gameObject.GetComponent<CameraController>();
        hudCooldown = movement.UI.HudCooldown.GetComponent<RectTransform>();
        slowdown = gameObject.GetComponent<PlayerSlowdown>();
        wall = gameObject.GetComponent<wallRun>();
        startSpeed = movement.targetSpeed;
    }

    // Update is called once per frame
    void Update()
    {

        coolDown -= Time.deltaTime;

        if (!dashing)
        {
            return;
        }
        
        if (coolDown < dashTime - 0.7f)
        {
            camControl.EnableFOV(camControl.StartFOV, FOVfade);
        }
        Vector3 scale = hudCooldown.localScale;
        scale.x =  (dashTime - coolDown) / dashTime;
    
        if (coolDown < 0f)
        {
            scale.x = 1;
            dashing = false;
            //movement.targetSpeed = startSpeed;
        }
        hudCooldown.localScale = scale;
        /*if (waitTimer >= dashTime)
        {
            dashing = false;
            movement.targetSpeed = startSpeed;
            waitTimer = 0;
        }

        waitTimer += Time.deltaTime;*/

    }

    public void ResetDash()
    {
        dashing = false;
        coolDown = 0;
    }

    public void OnDash(InputAction.CallbackContext context)
    {
        // if the button's been canceled, or if we're already dashing, or if the cooldown hasn't been hit, do nothing
        if (!context.performed
         || dashing 
         || coolDown > 0.0f 
         || movement.IsCrouching
         || wall.Wallrunning
         )
        {
            return;
        }

        Vector3 position = transform.position;
        position += camControl.Camera.transform.forward * 3f;
        if (!movement.LevelBoundary.Contains(position))
        {
            return;
        }
        rBody.MovePosition(position);
        Debug.Log("DASHED!");
        slowdown.ReduceSlowdown(20);
        dashing = true;
        //startSpeed = movement.targetSpeed;
      //  movement.targetSpeed = increaseSpeed;
        coolDown = dashTime;
        camControl.EnableFOV(dashFOV, 75f);
    }
}
