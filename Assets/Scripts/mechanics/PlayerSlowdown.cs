using UnityEngine;
using UnityEngine.SceneManagement;

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
    private bool isSafe = false;
    private bool atEnd = false;
    [SerializeField] PlayerManager playerManager;
    
    private dash dash;

    void Awake()
    {
        dash = gameObject.GetComponent<dash>();
    }
    
    // Update is called once per frame
    void Update()
    {
        UpdateSlowdown();
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Entered Swamp");
        if(other.gameObject.CompareTag("Swamp")){
           EnterSwamp(); 
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if(other.gameObject.CompareTag("Swamp")){
            ExitSwamp();
        }   
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("End"))
        {
            Debug.LogWarning("You did it!");
            dash.enabled = false;
            atEnd = true;
            return;
        }
        isSafe = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("End"))
        {
            atEnd = false;
            dash.enabled = true;
            return;
        }
        isSafe = false;
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

    //reduces slowdown meter automatically by given amount
    public void ReduceSlowdown(float amount)
    {
        //if isDead
        if (playerManager.IsDead)
        {
            return;
        }
        slowdownMeter -= amount;
        slowdownMeter = Mathf.Clamp(slowdownMeter, 0, maxSlowdown);
    }

    //updates current slowdown meter based on player status
    void UpdateSlowdown()
    {
        //if isDead
        if (playerManager.IsDead)
        {
            return;
        }
        //while in swamp the slowdown meter builds over time
        if (inSwamp && !isSafe)
        {
            Debug.Log("SWAMPY");
            if (atEnd)
            {
                accumulationRate = 30f;
            }
            else
            {
                accumulationRate = 12.5f;
            }
            slowdownMeter += accumulationRate * Time.deltaTime;
            recoveryTimer = 0;
        }
        else
        {
            //while out of swamp, recovery timer starts if it hasn't already
            if(recoveryTimer < recoveryDelay)
            {
                recoveryTimer += Time.deltaTime; 
            }
            //once delay is finished, slow will start decaying over time
            else
            {
                slowdownMeter -= recoveryRate * Time.deltaTime;
            }
        }

        slowdownMeter = Mathf.Clamp(slowdownMeter, 0, maxSlowdown);

        //if slowdown exceeds max value, player will die
        if(slowdownMeter >= maxSlowdown)
        {
            if (atEnd)
            {
                SceneManager.LoadScene("EndingCredits");
            }
            playerManager.Die();
        }
    }

    //resets slowdown values to default
    public void ResetSlowdown()
    {
       slowdownMeter = 0;
       recoveryTimer = 0;
       inSwamp = false; 
    }

    public float GetSpeedMultiplier()
    {
        float slowdownPerc = slowdownMeter / maxSlowdown;
        float multiplier = 1 - (slowdownPerc * maxSpeedPenalty);
        return multiplier;
    }

    public float GetSlowdownPercentage()
    {
        return slowdownMeter/maxSlowdown;
    }
}
