using UnityEngine;

public class PlayerSlowdown : MonoBehaviour
{
    [SerializeField, Range(0.0f, 80.0f)]
    private float slowdownMeter = 0;
    
    [SerializeField]
    private float maxSlowdown = 100.0f;
    private float accumulationRate = 3.0f;
    private float recoveryRate = 5.0f;
    private float recoveryDelay = 3.0f;
    private float maxSpeedPenalty = 0.8f;
    
    private float recoveryTimer = 0.0f;

    private bool inSwamp = false;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
        float multiplier = 1 - (slowdownPerc * (1 - maxSpeedPenalty));
        return multiplier;
    }
}
