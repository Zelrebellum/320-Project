using UnityEngine;

public enum WrathState
{
    Building,
    Active,
    Recovery
}
public class WrathManager : MonoBehaviour
{
    [Header("Wrath Meter")]
    [SerializeField, Min(0.01f)] private float maxWrath = 100f;
    [SerializeField] private float currentWrath;
    [SerializeField] private float wrathDashCooldown = 1.0f;
    
    [Header("Wrath Dash")]

    public float WrathDashCooldown => wrathDashCooldown;
    [SerializeField] private float dashWrathGain = 15f;
    public float DashWrathGain => dashWrathGain;
    
    [Header("Wrath Fall")]
    [SerializeField, Min(0)] private float minimumFallDistance = 8f;
    [SerializeField, Min(0)] private float wrathPerUnitFell = 2f;
    private float highestAirborneY;
    private bool wasAirborne;    
    
    [Header("State Management")]
    [Tooltip("How long wrath state lasts")]
    
    [SerializeField, Min(0.01f)] private float wrathDuration = 10f;
    [Tooltip("How long recovery state after wrath lasts")]
    [SerializeField, Min(0)] private float recoveryDuration = 2f;
    
    [SerializeField] private float stateTimer;
    private WrathState wrathState;
    public WrathState CurrentWrathState => wrathState;
    
    [Header("Passive Wrath Recovery")]
    [SerializeField, Min(0f)] private float dashRecoveryDelay = 4f;
    [SerializeField, Min(0f)] private float passiveRecoveryRate = 5f;

    private float timeSinceLastDash;
    
    [Header("Wrath Bonuses")]
    [SerializeField, Min(1f)]
    private float speedMultiplier = 1.25f;
    [SerializeField, Min(1f)]
    private float dashForceMultiplier = 1.5f;
    public float DashForceMultiplier => dashForceMultiplier;
    
    [SerializeField, Min(1f)]
    private float jumpMultiplier = 1.25f;
    [SerializeField, Range(30f, 120f)]
    private float wrathFOV = 75f;

    [Header("Enemy Proximity")]
    [SerializeField, Min(0.0f)]
    private float enemyDetectionRange = 20f;
    [SerializeField, Min(0f)]
    private float closestEnemyRange = 5f;
    [SerializeField, Min(0f)]
    private float maxEnemyWrathGain = 20f;
    
    [Header("References")] 
    [SerializeField] private RisingEnemy enemy;
    private PlayerManager playerManager;
    private PlayerMovement movement;
    public PlayerMovement PlayerMovement => movement;
    
    private CameraController camControl;
    private PlayerHUD playerHUD;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerManager = gameObject.GetComponent<PlayerManager>();
        movement = playerManager.Player.GetComponent<PlayerMovement>();
        camControl = playerManager.Player.GetComponent<CameraController>();
        playerHUD = FindAnyObjectByType<PlayerHUD>();
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastDash += Time.deltaTime;
        if (timeSinceLastDash >= dashRecoveryDelay && CurrentWrathState == WrathState.Building)
        {
            ReduceWrath(passiveRecoveryRate * Time.deltaTime);
        }
        
        TrackFall();
        TrackEnemyRange();
        
        switch(wrathState)
        {
            case WrathState.Building:
                break;
            case WrathState.Active:
                stateTimer += Time.deltaTime;
                if (stateTimer >= wrathDuration)
                {
                    EnterRecoveryState();
                }
                break;
            case WrathState.Recovery:
                stateTimer += Time.deltaTime;
                if (stateTimer >= recoveryDuration)
                {
                    EnterBuildingState();
                }
                break;
        }
        
        playerHUD.UpdateWrathMeter(GetWrathPercentage());

    }

    void EnterWrathState()
    {
        stateTimer = 0;
        wrathState = WrathState.Active;
        movement.SetWrathBonuses(speedMultiplier, jumpMultiplier);
        camControl.SetBaseFOV(wrathFOV);
    }

    void EnterRecoveryState()
    {
        currentWrath = 0f;
        stateTimer = 0;
        wrathState = WrathState.Recovery;
        movement.SetWrathBonuses(1.0f, 1.0f);
        camControl.SetBaseFOV(camControl.StartFOV);
    }

    void EnterBuildingState()
    {
        stateTimer = 0;
        wrathState = WrathState.Building;
    }

    void TrackFall()
    {
        if (movement.Airborne && !wasAirborne)
        {
            highestAirborneY = movement.transform.position.y;
        }

        if (movement.Airborne)
        {
            highestAirborneY = Mathf.Max(movement.transform.position.y,  highestAirborneY);
        }

        if (wasAirborne && !movement.Airborne)
        {
            float distanceFell;
            distanceFell = highestAirborneY - movement.transform.position.y;
            if (distanceFell > minimumFallDistance)
            {
                float wrathGain = (distanceFell - minimumFallDistance) * wrathPerUnitFell;
                AddWrath(wrathGain);
            }
        }
        
        wasAirborne = movement.Airborne;
    }

    void TrackEnemyRange()
    {
        float distance = movement.transform.position.y - enemy.EnemySurface;
        float proximity = Mathf.InverseLerp(
            enemyDetectionRange,
            closestEnemyRange,
            distance
        );
        AddWrath(proximity * maxEnemyWrathGain * Time.deltaTime);
    }
    
    public void AddWrath(float amount)
    {
        if (wrathState == WrathState.Building)
        {
            currentWrath += amount;
            currentWrath = Mathf.Clamp(
                currentWrath, 0f, maxWrath);
            if (currentWrath >= maxWrath)
            {
                EnterWrathState();
            }
        }
    }

    public void ReduceWrath(float amount)
    {
        if (wrathState == WrathState.Building)
        {
            currentWrath -= amount;
            currentWrath = Mathf.Clamp(
                currentWrath, 0f, maxWrath);
        }
    }
    
    public void NotifyDash()
    {
        timeSinceLastDash = 0f;
    }

    public void ResetWrath()
    {
        currentWrath = 0f;
        wrathState = WrathState.Building;
        stateTimer = 0;
        highestAirborneY = movement.transform.position.y;
        wasAirborne = false;
        timeSinceLastDash = 0f;
        movement.SetWrathBonuses(1.0f, 1.0f);
    }
        
    public float GetWrathPercentage()
    {
        float currentPercentage = 0;
        
        switch (wrathState)
        {
            case WrathState.Building:
                currentPercentage = currentWrath / maxWrath;
                break;
            case WrathState.Active:
                currentPercentage =  1.0f - (stateTimer / wrathDuration);
                break;
            case WrathState.Recovery:
                break;
        }

        return currentPercentage;
    }
}
