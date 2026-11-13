using UnityEngine;

public class SlothDetector : MonoBehaviour
{
    private SlothManager slothManager;
    private int swampContacts;

    private void Awake()
    {
        PlayerMovement movement = GetComponent<PlayerMovement>();
        slothManager = movement.Manager.GetComponent<SlothManager>();
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Swamp"))
        {
            swampContacts++;
            slothManager.SetSwampState(swampContacts > 0);
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Swamp"))
        {
            swampContacts = Mathf.Max(0, swampContacts - 1);
            slothManager.SetSwampState(swampContacts > 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Safe"))
        {
            slothManager.SetSafeState(true);
        }

        if (other.gameObject.CompareTag("End"))
        {
            slothManager.SetEndZoneState(true);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Safe"))
        {
            slothManager.SetSafeState(false);
        }
        
        if (other.gameObject.CompareTag("End"))
        {
            slothManager.SetEndZoneState(false);
        }
    }
    
    public void ResetContacts()
    {
        swampContacts = 0;
    }
}
