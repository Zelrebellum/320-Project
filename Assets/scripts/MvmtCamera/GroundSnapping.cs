using UnityEngine;

public class GroundSnapping : MonoBehaviour
{
    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private float yOffset = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hitInfo;

        if (Physics.Raycast(transform.position, -Vector3.up, out hitInfo, 1.0f, layerMask))
        {
            Vector3 newForward = Vector3.Cross(transform.right, hitInfo.normal);
            transform.LookAt(transform.position + newForward, hitInfo.normal);
            Vector3 snapPoint = transform.position;

            snapPoint.y = hitInfo.point.y + yOffset;

            transform.position = snapPoint;

        }
        else if (Physics.Raycast(transform.position, -Vector3.up, Mathf.Infinity, layerMask))
        {
            Vector3 newForward = Vector3.Cross(transform.right,new Vector3(0, 1, 0));
            transform.LookAt(transform.position + newForward, new Vector3(0, 1, 0));
        }
       



    }
}
