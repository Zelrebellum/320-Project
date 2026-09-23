using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.Rendering;
using TMPro;

public class MovementController : MonoBehaviour
{
    [SerializeField]
    public float speed;

    // public for debug purposes
    public Vector2 moveDirection = Vector2.zero;
    public Vector3 velocity = Vector3.zero;

    [SerializeField]
    private Rigidbody rBody;
    [SerializeField]
    public float jumpForce;
    [SerializeField]
    private LayerMask groundLayer;

    // Update is called once per frame
    void Update()
    {

        if (moveDirection != Vector2.zero)
        {
            // Determine the velocity based on player direction & speed
            // axis by axis

            // left/right -- along player's right vector
            velocity = transform.right * moveDirection.x;

            // fwd/back
            velocity += transform.forward * moveDirection.y;

            transform.position += velocity.normalized * speed * Time.deltaTime;
        }
        else
        {
            velocity = Vector3.zero;
        }

    }



    public void OnMove(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
    }


    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.canceled)
        {
            return;
        }
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(transform.position, -transform.up, out hitInfo, 1.3f, groundLayer);
        if (!hit)
        {
            return;
        }
        Debug.Log($"Jumped");
        rBody.AddForce(transform.up * jumpForce, ForceMode.Impulse);

    }

  
}
