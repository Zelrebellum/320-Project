using UnityEngine;

public class DeathwallManager : MonoBehaviour
{
    [SerializeField] GameObject manager;
    [SerializeField] Transform startPos;
    private Vector3 startPosTrue;
    [SerializeField] Transform endPos;
    [SerializeField, Range(0.0f, 1.0f)] float intensity;
    [SerializeField] bool isRepeating;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.transform.position = startPos.position;
        startPosTrue = startPos.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        movement();
    }

    private void movement()
    {
        if(this.gameObject.transform.position.z < endPos.position.z)
        {
            this.gameObject.transform.position += (this.gameObject.transform.forward*intensity);
        }
        else if (isRepeating)
        {
            this.gameObject.transform.position = startPosTrue;
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == manager.GetComponent<PlayerManager>().player)
        {
            manager.GetComponent<PlayerManager>().Respawn();
        }
    }
}
