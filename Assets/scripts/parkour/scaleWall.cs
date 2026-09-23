using Unity.VisualScripting;
using UnityEngine;

public class climbScript : MonoBehaviour
{

    [SerializeField]
    private LayerMask scaleLayer;

    [SerializeField]
    private Vector3 scaleWallLimit;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
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


}
