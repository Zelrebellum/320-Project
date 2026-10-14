using UnityEngine;

public class PlayerSlowdown : MonoBehaviour
{
    [SerializeField, Range(0.0f, 99.0f)]
    private float slowdownMeter = 0;
    
    [SerializeField]
    private float maxSlowdown = 100.0f;
    [SerializeField]
    private float accumulationRate = 12.5f;
    [SerializeField]
    private float recoveryRate = 10.0f;
    [SerializeField]
    private float recoveryDelay = 3.0f;
    [SerializeField]
    private float maxSpeedPenalty = 0.8f;
    
    private float recoveryTimer = 0.0f;

    private bool inSwamp = false;

    
    // Update is called once per frame
    void Update()
    {
        UpdateSlowdown();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Entered Swamp");
        if(other.CompareTag("Swamp")){
           EnterSwamp(); 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Swamp")){
            ExitSwamp();
        }   
    }
    
    void EnterSwamp()
    {
        inSwamp = true;
        recoveryTimer = 0;
    }

    void ExitSwamp()
    {
        inSwamp = false;
    }

    public void ReduceSlowdown(float amount)
    {
        //if isDead
        if (false)
        {
            return;
        }
        slowdownMeter -= amount;
        slowdownMeter = Mathf.Clamp(slowdownMeter, 0, maxSlowdown);
    }

    void UpdateSlowdown()
    {
        //if isDead
        if (false)
        {
            return;
        }
        if (inSwamp)
        {
            Debug.Log("SWAMPY");
            slowdownMeter += accumulationRate * Time.deltaTime;
            recoveryTimer = 0;
        }
        else
        {
            if(recoveryTimer < recoveryDelay)
            {
                recoveryTimer += Time.deltaTime; 
            }
            else
            {
                slowdownMeter -= recoveryRate * Time.deltaTime;
            }
        }

        slowdownMeter = Mathf.Clamp(slowdownMeter, 0, maxSlowdown);

        if(slowdownMeter >= maxSlowdown)
        {
            //isDead = true;
            //Die()
        }
    }

    public float GetSpeedMultiplier()
    {
        float slowdownPerc = slowdownMeter / maxSlowdown;
        float multiplier = 1 - (slowdownPerc * maxSpeedPenalty);
        return multiplier;
    }
}
