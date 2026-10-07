using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem;

public class wallRun : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private LayerMask wallJumpLayer;

    [SerializeField] private float wallJumpDistance = 1.3f;

    [SerializeField] private bool wallrunning;

    [SerializeField] private float increaseSpeed;

    [SerializeField] private float speedMinimum;

    [SerializeField] private float wallRunTime;
    
    [SerializeField] private float timer;
    private PlayerMovement movement;
    private Rigidbody rBody;
    private RaycastHit hitInfo;
    private float currentAngle = 0;

    private bool leftWall = false;
    private bool rightWall = false;

    void Start()
    {
        movement = gameObject.GetComponent<PlayerMovement>();
        rBody = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // if we're not wall running, skip everything
        if (!wallrunning)
        {
            currentAngle = 0;
            rBody.useGravity = true;
            return;
        }

        if (timer < 0 || movement.GetCurrentSpeed() < speedMinimum)
        {
            timer = wallRunTime;
            wallrunning = false;
            Quaternion transf = Quaternion.Euler(
            new Vector3(0f,
                        transform.rotation.eulerAngles.y,
                        0f));
            transform.rotation = transf;
            return;
        }
        timer -= Time.deltaTime;
        movement.airborne = false;
        rBody.useGravity = false;
        // use hitInfo.normal to do what u nee
        Quaternion trans = Quaternion.Euler(
            new Vector3(transform.rotation.eulerAngles.x,
                        transform.rotation.eulerAngles.y,
                        currentAngle));
        
        transform.rotation = trans;

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

    public void WallJump(InputAction.CallbackContext context)
    {
        if (!context.canceled || wallrunning)
        {
            return;
        } 

        Vector3 left = Quaternion.AngleAxis(-45.0f, Vector3.up) * transform.forward;
        Vector3 right = Quaternion.AngleAxis(45.0f, Vector3.up) * transform.forward;

        leftWall = Physics.Raycast(transform.position, left, out hitInfo, wallJumpDistance, wallJumpLayer);
        rightWall = Physics.Raycast(transform.position, right, out hitInfo, wallJumpDistance, wallJumpLayer);

        if (leftWall)
        {
            currentAngle = -15f;
        }
        else if (rightWall)
        {
            currentAngle = 15f;
        }
        else
        {
            return;
        }

        wallrunning = true;
        rBody.useGravity = false;
        timer = wallRunTime;

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
