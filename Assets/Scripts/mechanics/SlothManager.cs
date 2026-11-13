using UnityEngine;

public class SlothManager : MonoBehaviour
{
    [Header("Slowdown Settings")]
    [SerializeField, Range(0f, 100f)]
    private float onboardingSlowdown = 25f;
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
    
    private PlayerManager playerManager;
    private LevelManager levelManager;
    private Dash dash;
    private PlayerMovement movement;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        levelManager = GetComponent<LevelManager>();
        dash = playerManager.Player.GetComponent<Dash>();
        movement = playerManager.Player.GetComponent<PlayerMovement>();
        slowdownMeter = onboardingSlowdown;
    }

    private void Update()
    {
        UpdateSlowdown();
    }

    public void SetSwampState(bool active)
    {
        if (active && !inSwamp)
        {
            recoveryTimer = 0f;
        }  
        
        inSwamp = active;
    }
    
    public void SetSafeState(bool active)
    {
        isSafe = active;
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
                levelManager.CompleteLevel();
                return;
            }

            playerManager.Die();
        }
    }
    
    public void SetEndZoneState(bool active)
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
        SetEndZoneState(false);
        slowdownMeter = 0f;
        recoveryTimer = 0f;
        inSwamp = false;
        isSafe = false;
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
