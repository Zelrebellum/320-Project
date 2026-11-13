using UnityEngine;

public class SacredZone : MonoBehaviour
{
    [SerializeField, Min(0f)]
    private float activationDelay = 2f;

    [SerializeField, Min(0f)]
    private float wrathRecoveryRate = 15f;

    private float timeInside;
    private bool playerInside;
    private WrathManager wrath;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        wrath = FindAnyObjectByType<WrathManager>();
    }
    // Update is called once per frame
    void Update()
    {
        if (playerInside)
        {
            timeInside += Time.deltaTime;
        }

        if (playerInside && timeInside >= activationDelay)
        {
            wrath.ReduceWrath(wrathRecoveryRate * Time.deltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
            Debug.Log("Player in sacred zone");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false; 
            timeInside = 0;
            Debug.Log("Player exited sacred zone");
        }
    }
}
