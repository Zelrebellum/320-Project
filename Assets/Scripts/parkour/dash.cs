using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class dash : MonoBehaviour
{
    [SerializeField]
    private float increaseSpeed;
    private float startSpeed;
    [SerializeField]
    private bool dashing = false;

    [SerializeField, Range(0.5f, 10.0f)]
    private float dashTime;
    private float waitTimer = 0;
    [SerializeField]
    private float dashFOV;
    private float coolDown;
    private PlayerMovement movement;

    private Rigidbody rBody;

    private CameraController camControl;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movement = gameObject.GetComponent<PlayerMovement>();
        startSpeed = movement.speed;
        rBody = gameObject.GetComponent<Rigidbody>();
        camControl = gameObject.GetComponent<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {

        coolDown -= Time.deltaTime;
        
        if (dashing && coolDown < dashTime - 0.7f)
        {
            camControl.EnableFOV(camControl.GetStartFOV(), 10f);
        }
        if (coolDown < 0f)
        {
            dashing = false;
        }
        /*if (waitTimer >= dashTime)
        {
            dashing = false;
            movement.speed = startSpeed;
            waitTimer = 0;
        }

        waitTimer += Time.deltaTime;*/

    }

    public void OnDash(InputAction.CallbackContext context)
    {
        // if the button's been canceled, or if we're already dashing, or if the cooldown hasn't been hit, do nothing
        if (!context.canceled
         || dashing 
         || coolDown > 0.0f)
        {
            return;
        }

        Vector3 position = transform.position;
        position += camControl.Camera.transform.forward * 3f;
        transform.position = position;
        dashing = true;
        coolDown = dashTime;
        camControl.EnableFOV(dashFOV, 75f);
    }
}
