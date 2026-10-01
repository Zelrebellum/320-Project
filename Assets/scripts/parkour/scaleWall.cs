using System;
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
    private Vector3 scaleDistance;

    [SerializeField, Range(0, 20)]
    private float climbSpeed;

    private MovementController movement;

    private Rigidbody rBody;
    private bool climbing = false;
    private RaycastHit hitInfo;

    //private Vector3 prevDistance;

    [SerializeField, Range(1, 0)]
    private float scaleTime;

    private float waitTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // get the movement controller on our current object. useful so we dont have to keep checking if we're airborne multiple times.
        movement = gameObject.GetComponent<MovementController>();
        rBody = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // if we're climbing, ignore everything else and just climb 
        if (climbing)
        {
            Vector3 position = transform.position;

            float bottom = (transform.position.y);
            float top = transform.position.y + transform.localScale.y;

            /*Vector3 distanceToLedge = hitInfo.transform.position - transform.position;

            distanceToLedge = new Vector3(
                Mathf.Abs(distanceToLedge.x), 
                Mathf.Abs(distanceToLedge.y), 
                Mathf.Abs(distanceToLedge.z));

            Vector3 trend = distanceToLedge - prevDistance;*/


            if (bottom < hitInfo.transform.position.y + hitInfo.transform.localScale.y)
            {
                position.y += climbSpeed * Time.deltaTime;
                transform.position = position;
            }
            else
            {
                rBody.AddForce(transform.forward * 2, ForceMode.Impulse);
                waitTime = scaleTime;
                climbing = false;
            }


            //prevDistance = distanceToLedge;
            return;
        }

        if (waitTime > 0)
        {
            waitTime -= Time.deltaTime;
            return;
        }

        // if we're not airborne, go to next update
        if (!movement.airborne)
        {
            return;
        }
        
        Vector3 topPosition = transform.position;
        topPosition.y += transform.localScale.y;

        bool hit = Physics.Raycast(topPosition, transform.forward, out hitInfo, scaleDistance.x, groundLayer);

        // is there something on the ground layer close enough to us in the local forward position?
        if (!hit)
        {
            return;
        }

        float ledge = Mathf.Abs((hitInfo.transform.position.y + hitInfo.transform.localScale.y) - (topPosition.y));

        // if the ledge's height is close enough, but not *too* close, mantle it. prevents the wibblewobblies on jumping when tuned well.
        if (ledge <= scaleDistance.y && ledge > scaleDistance.z)
        {
            climbing = true;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Vector3 position = transform.position;

        position.y += transform.localScale.y;

        Gizmos.DrawRay(position, transform.forward);
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

    }


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


    }*/


}
