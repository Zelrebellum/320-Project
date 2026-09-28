using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class climbScript : MonoBehaviour
{

    [SerializeField]
    private LayerMask groundLayer;

    [SerializeField]
    private Vector3 scaleWallLimit;

    [SerializeField]
    private Vector2 scaleDistance;

    [SerializeField, Range(0, 20)]
    private float climbSpeed;

    private MovementController movement;
    private bool climbing = false;
    private RaycastHit hitInfo;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        movement = gameObject.GetComponent<MovementController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (climbing)
        {
            Vector3 position = transform.position;

            float bottom = (transform.position.y - transform.localScale.y);
            
            if (bottom < hitInfo.transform.position.y + hitInfo.transform.localScale.y)
            {
                position.y += climbSpeed * Time.deltaTime;
                transform.position = position;
            }
            else
            {
                climbing = false;
            }
            return;
        }

        
        if (!movement.airborne)
        {
            return;
        }

        bool hit = Physics.Raycast(transform.position, transform.forward, out hitInfo, scaleDistance.x, groundLayer);
        if (!hit)
        {
            return;
        }

        float ledge = Mathf.Abs((hitInfo.transform.position.y + hitInfo.transform.localScale.y) - (transform.position.y + transform.localScale.y));

        if (ledge <= scaleDistance.y)
        {
            climbing = true;
        }
    }

    /*private void OnTriggerEnter(Collider other)
    {

        if (scaleLayer != (scaleLayer | (1 << other.gameObject.layer)))
        {
            return;
        }

        Vector3 position = new Vector3(
            Mathf.Abs(other.transform.position.x - transform.position.x),
            Mathf.Abs(other.transform.position.y - transform.position.y),
            Mathf.Abs(other.transform.position.z - transform.position.z));

        
        if (position.x > scaleWallLimit.x &&
            position.y > scaleWallLimit.y &&
            position.z > scaleWallLimit.z)
        {
            return;
        }

        position = transform.position;
        position.y = other.transform.position.y;

        transform.position = position;

    }*/

    private void ScaleLedge(InputAction.CallbackContext context)
    {
        if (!context.canceled)
        {
            return;
        }
        RaycastHit hitInfo;
        bool hit = Physics.Raycast(transform.position, transform.forward, out hitInfo, scaleDistance.x, groundLayer);
        if (!hit)
        {
            return;
        }

        float ledge = Mathf.Abs((hitInfo.transform.position.y * hitInfo.transform.localScale.y) - (transform.position.y * transform.localScale.y));

        if (ledge < scaleDistance.y)
        {
            
        }


    }


}
