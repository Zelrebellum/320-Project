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
    private float recoveryRate = 10.0f;
    [SerializeField]
    private float recoveryDelay = 3.0f;
    [SerializeField]
    private float maxSpeedPenalty = 0.8f;
    
    private float recoveryTimer = 0.0f;

    private bool inSwamp = false;
    private bool isSafe = false;
    private bool atEndZone = false;
    public bool AtEnd { get { return atEndZone; } }


    [Header("References")]
    [SerializeField] PlayerManager playerManager;
    
    private Dash dash;

    private void Awake()
    {
        dash = GetComponent<Dash>();
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
            Debug.LogWarning("You did it!");
            dash.enabled = false;
            atEndZone = true;
            return;
        }

        isSafe = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("End"))
        {
            atEndZone = false;
            dash.enabled = true;
            return;
        }

        isSafe = false;
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
            if (atEndZone)
            {
                accumulationRate = 30f;
            }
            else
            {
                accumulationRate = 12.5f;
            }

            slowdownMeter += accumulationRate * Time.deltaTime;
            recoveryTimer = 0f;
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
            }

            playerManager.Die();
        }
    }

    public void ResetSlowdown()
    {
        slowdownMeter = 0f;
        recoveryTimer = 0f;
        inSwamp = false;
    }

    public float GetSpeedMultiplier()
    {
        float slowdownPercentage =
            slowdownMeter / maxSlowdown;

        float multiplier =
            1f - (slowdownPercentage * maxSpeedPenalty);

        return multiplier;
    }

    public float GetSlowdownPercentage()
    {
        return slowdownMeter / maxSlowdown;
    }
}
