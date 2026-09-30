using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class dash : MonoBehaviour
{
    [SerializeField]
    private float increaseSpeed;
    private float startSpeed;
    [SerializeField]
    private bool dashing;

    [SerializeField, Range(0.5f, 10.0f)]
    private float dashTime;
    private float waitTimer = 0;
    
    private float coolDown;
    private MovementController movement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movement = gameObject.GetComponent<MovementController>();
        startSpeed = movement.speed;
    }

    // Update is called once per frame
    void Update()
    {

        if (!dashing)
        {
            coolDown -= Time.deltaTime;
            return;
        }

        if (waitTimer >= dashTime)
        {
            dashing = false;
            movement.speed = startSpeed;
            waitTimer = 0;
        }

        waitTimer += Time.deltaTime;

    }

    public void OnDash(InputAction.CallbackContext context)
    {
        // if the button's been canceled, or if we're already dashing, or if the cooldown hasn't been hit, do nothing
        if (context.canceled
         || dashing 
         || coolDown > 0.0f)
        {
            return;
        }

        dashing = true;
        movement.speed += increaseSpeed;
        coolDown = dashTime;

    }
}
