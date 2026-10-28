using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Camera playerCam;

    public Camera Camera
    {
        get => playerCam;
    }

    [SerializeField]
    bool upDownInversion = true;

    [SerializeField]
    bool limitUpDownRange = false;

    [SerializeField]
    Vector2 lookSensitivity = Vector2.one;

    [SerializeField]
    float maxLookUpDownAngle = 25;

    private float startFOV;

    public float StartFOV
    {
        get => startFOV;
    }

    private float currentFOV;

    private float targetFOV;

    private float fovAcceleration;


    private bool changingFOV = false;
    
    public bool FOVStatus
    {
        get => changingFOV;
    }
    private bool decreaseFOV = false;

    private void Awake()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        playerCam.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        startFOV = playerCam.fieldOfView;
        currentFOV = startFOV;

        // get sensitivity from saved
        ChangeSensitivity(PlayerPrefs.GetFloat("MouseSensitivity", 5f));
    }

    public void Update()
    {
        playerCam.transform.localRotation = Quaternion.Euler(playerCam.transform.localRotation.eulerAngles.x, 0f, 0f );
        playerCam.fieldOfView = currentFOV;
        if (!changingFOV)
        {
            return;
        }

        if ((!decreaseFOV && currentFOV > targetFOV)
        || (decreaseFOV && currentFOV < targetFOV))
        {
            changingFOV = false;
            currentFOV = targetFOV;
            return;
        }
        currentFOV += fovAcceleration * Time.deltaTime * (decreaseFOV ? -1f : 1f);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (Application.isFocused)
        {
            OnFocus();
        }

        if (PauseMenuManager.IsPaused || BlinkTransition.IsPlaying)
        {
            return;
        }
        // read the mouse displacement from the center of the screen
        Vector2 lookInput = context.ReadValue<Vector2>();

        // if we've turned left or right
        if (lookInput.x != 0f)
        {
            // use that to rotate the entire player object
            transform.Rotate(0f, lookInput.x * lookSensitivity.x * Time.deltaTime, 0f);
            //transform.Rotate(0f, lookInput.x, 0f);
        }

        // if we've turned up or down
        if (lookInput.y != 0f)
        {
            // invert the y axis if needed
            lookInput.y *= upDownInversion ? 1f : -1f;

            // Find the new camera rotation
            Quaternion newLook = playerCam.transform.rotation * Quaternion.Euler(lookInput.y * lookSensitivity.y * Time.deltaTime, 0f, 0f);

            // See if it's gone too far

            // 0 == the horizon
            // 0 -> maxAngle == looking down
            // 360 --> 360-maxAngle == looking up

            if (limitUpDownRange)
            {
                // If we're past the max angle looking down and looking down or behind us
                if (newLook.eulerAngles.x > maxLookUpDownAngle && newLook.eulerAngles.x <= 180f)
                {
                    newLook = Quaternion.Euler(maxLookUpDownAngle, newLook.eulerAngles.y, 0f);
                }

                // otherwise, if we're past the max angle looking up AND looking up
                if (newLook.eulerAngles.x < 360 - maxLookUpDownAngle && newLook.eulerAngles.x >= 180f)
                {
                    newLook = Quaternion.Euler(360 - maxLookUpDownAngle, newLook.eulerAngles.y, 0f);
                }
            }
            playerCam.transform.rotation = newLook;
        }
    }

    public void OnReset(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            CameraReset();
        }
    }

    private void CameraReset()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        playerCam.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    public void OnFocus()
    {
        if (PauseMenuManager.IsPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void ChangeSensitivity(float sensitivity)
    {
        sensitivity *= 20;

        if (sensitivity < 5)
        {
            sensitivity = 5;
        }
        lookSensitivity = new Vector2(sensitivity, sensitivity);
    }

    /// <summary>
    /// Enables FOV change over time
    /// </summary>
    /// <param name="target">The final FOV</param>
    /// <param name="acceleration">How quick the FOV should go</param>
    public void EnableFOV(float target, float acceleration)
    {
        changingFOV = true;
        targetFOV = target;
        if (targetFOV < currentFOV)
        {
            decreaseFOV = true;
        }
        else
        {
            decreaseFOV = false;
        }
        fovAcceleration = acceleration;
    }

}