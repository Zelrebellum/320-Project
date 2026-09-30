using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem;

public class wallRun : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField]
    private LayerMask wallJumpLayer;

    [SerializeField]
    private float wallJumpDistance = 1.3f;

    [SerializeField]
    private bool wallrunning;

    [SerializeField]
    private float increaseSpeed;

    [SerializeField]
    private float speedMinimum;

    private MovementController movement;

    private Rigidbody rBody;

    private RaycastHit hitInfo;

    void Start()
    {
        movement = gameObject.GetComponent<MovementController>();
        rBody = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // if we're not wall running, skip everything
        if (!wallrunning)
        {
            rBody.useGravity = true;
            return;
        }

        rBody.useGravity = false;

        if (movement.GetCurrentSpeed() < speedMinimum)
        {
            wallrunning = false;
            return;
        }

        Vector3 rotation = transform.rotation.eulerAngles;




    }

    // list for what i need to do to implement this 4 now
    // input check for jump
    // check angles ~45 degrees to the left and right of player hull for a runnable wall layer game object
    // if hit, get player on the wall + at an angle, stop checking for gravity
    // if the player stops holding w, check for gravity and right them
    // at the end of the run (abt ~2 seconds) drop them
    // if at any time during the run player presses space, have them jump
    // increase player speed a lil 2 make them feel awesome and amazing
    // roll when landing? maybe? if i can do that ?

    void WallJump(InputAction.CallbackContext context)
    {
        if (!context.canceled || wallrunning)
        {
            return;
        } 

        Vector3 left = Quaternion.AngleAxis(-45.0f, Vector3.up) * transform.forward;
        Vector3 right = Quaternion.AngleAxis(45.0f, Vector3.up) * transform.forward;

        bool hit = Physics.Raycast(transform.position, left, out hitInfo, wallJumpDistance, wallJumpLayer);

        if (hit)
        {
            wallrunning = true;
            return;
        }
        
        hit = Physics.Raycast(transform.position, right, out hitInfo, wallJumpDistance, wallJumpLayer);

        if (!hit)
        {
            return;
        }

        wallrunning = true;

    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 left = Quaternion.AngleAxis(-45.0f, Vector3.up) * transform.forward;

        Gizmos.DrawRay(transform.position, left);
        
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, Quaternion.AngleAxis(45.0f, Vector3.up) * transform.forward);

    }

}
