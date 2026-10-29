using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSlowdown : MonoBehaviour
{
    [Header("Slowdown Settings")]
    [SerializeField, Range(0f, 100f)]
    private float slowdownMeter = 0;
    
    [SerializeField]
    private float maxSlowdown = 100.0f;
    [SerializeField]
    private float accumulationRate = 12.5f;
    [SerializeField]
    private float endZoneAccumulationRate = 30f;
    [SerializeField]
    private float recoveryRate = 10.0f;
    [SerializeField]
    private float recoveryDelay = 3.0f;
    [SerializeField]
    private float maxSpeedPenalty = 0.8f;
    [SerializeField, Range(0f, 1f)]
    private float swampSpeedPenalty = 0.10f;
    [SerializeField, Min(1f)]
    private float crouchAccumulationMultiplier = 1.25f;
    
    private float recoveryTimer = 0.0f;

    private bool inSwamp = false;
    private bool isSafe = false;
    private bool atEndZone = false;
    public bool AtEnd { get { return atEndZone; } }


    [Header("References")]
    [SerializeField] PlayerManager playerManager;
    
    private Dash dash;
    private PlayerMovement movement;

    private void Awake()
    {
        dash = GetComponent<Dash>();
        movement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        UpdateSlowdown();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Swamp"))
        {
            EnterSwamp();
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Swamp"))
        {
            ExitSwamp();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("End"))
        {
            SetEndZoneState(true);
            return;
        }

        if (other.CompareTag("Safe"))
        {
            isSafe = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("End"))
        {
            SetEndZoneState(false);
            return;
        }

        if (other.CompareTag("Safe"))
        {
            isSafe = false;
        }
    }

    private void EnterSwamp()
    {
        inSwamp = true;
        recoveryTimer = 0f;
    }

    private void ExitSwamp()
    {
        inSwamp = false;
    }

    public void ReduceSlowdown(float amount)
    {
        if (playerManager.IsDead)
        {
            return;
        }

        slowdownMeter -= amount;
        slowdownMeter = Mathf.Clamp(
            slowdownMeter, 0f, maxSlowdown);
    }

    private void UpdateSlowdown()
    {
        if (playerManager.IsDead)
        {
            return;
        }

        if (inSwamp && !isSafe)
        {
            float currentAccumulationRate = atEndZone
                ? endZoneAccumulationRate
                : accumulationRate;

            if (movement.IsCrouching)
            {
                currentAccumulationRate *= crouchAccumulationMultiplier;
            }

            slowdownMeter += currentAccumulationRate * Time.deltaTime;
        }
        
        else
        {
            if (recoveryTimer < recoveryDelay)
            {
                recoveryTimer += Time.deltaTime;
            }
            else
            {
                slowdownMeter -= recoveryRate * Time.deltaTime;
            }
        }

        slowdownMeter = Mathf.Clamp(
            slowdownMeter, 0f, maxSlowdown);

        if (slowdownMeter >= maxSlowdown)
        {
            if (atEndZone)
            {
                SceneManager.LoadScene("EndingCredits");
                return;
            }

            playerManager.Die();
        }
    }
    
    private void SetEndZoneState(bool active)
    {
        if (atEndZone == active)
        {
            return;
        }

        atEndZone = active;

        slowdownMeter = 0f;
        recoveryTimer = 0f;

        dash.enabled = !active;
        movement.SetEndZoneJump(active);
    }

    public void ResetSlowdown()
    {
        slowdownMeter = 0f;
        recoveryTimer = 0f;
        inSwamp = false;
    }

    public float GetSpeedMultiplier()
    {
        float slowdownPerc = slowdownMeter / maxSlowdown;

        float meterPenalty = slowdownPerc * maxSpeedPenalty;
        float swampPenalty;

        if (inSwamp && !isSafe)
        {
            swampPenalty = swampSpeedPenalty;
        }
        else
        {
            swampPenalty = 0f;
        }
        
        return Mathf.Clamp01(1f - meterPenalty - swampPenalty);
    }

    public float GetSlowdownPercentage()
    {
        return slowdownMeter / maxSlowdown;
    }
}
