
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Camera Reference")]
    [SerializeField] private Camera playerCam;

    [Header("Look Settings")]
    [SerializeField] private bool upDownInversion = true;
    [SerializeField] private bool limitUpDownRange = false;
    [SerializeField] private Vector2 lookSensitivity = Vector2.one;
    [SerializeField] private float maxLookUpDownAngle = 25f;

    // FOV state
    private float startFOV;
    private float currentFOV;
    private float targetFOV;
    private float fovAcceleration;

    private bool changingFOV;
    private bool decreaseFOV;

    public Camera Camera => playerCam;
    public float StartFOV => startFOV;
    public bool FOVStatus => changingFOV;

    private void Awake()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        playerCam.transform.rotation = Quaternion.Euler(0f, 0f, 0f);

        startFOV = playerCam.fieldOfView;
        currentFOV = startFOV;

        // get sensitivity from saved
        ChangeSensitivity(PlayerPrefs.GetFloat("MouseSensitivity", 1f));
    }

    private void Update()
    {
        playerCam.transform.localRotation = Quaternion.Euler(
            playerCam.transform.localRotation.eulerAngles.x,
            0f,
            0f
        );

        playerCam.fieldOfView = currentFOV;

        if (!changingFOV)
        {
            return;
        }

        if ((!decreaseFOV && currentFOV > targetFOV) ||
            (decreaseFOV && currentFOV < targetFOV))
        {
            changingFOV = false;
            currentFOV = targetFOV;
            return;
        }

        currentFOV += fovAcceleration * Time.deltaTime *
                      (decreaseFOV ? -1f : 1f);
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

        Vector2 lookInput = context.ReadValue<Vector2>();

        if (lookInput.x != 0f)
        {
            transform.Rotate(
                0f,
                lookInput.x * lookSensitivity.x * Time.deltaTime,
                0f
            );
        }

        if (lookInput.y != 0f)
        {
            lookInput.y *= upDownInversion ? 1f : -1f;

            Quaternion newLook =
                playerCam.transform.rotation *
                Quaternion.Euler(
                    lookInput.y * lookSensitivity.y * Time.deltaTime,
                    0f,
                    0f
                );

            if (limitUpDownRange)
            {
                // Limit downward pitch.
                if (newLook.eulerAngles.x > maxLookUpDownAngle &&
                    newLook.eulerAngles.x <= 180f)
                {
                    newLook = Quaternion.Euler(
                        maxLookUpDownAngle,
                        newLook.eulerAngles.y,
                        0f
                    );
                }

                // Limit upward pitch.
                if (newLook.eulerAngles.x < 360f - maxLookUpDownAngle &&
                    newLook.eulerAngles.x >= 180f)
                {
                    newLook = Quaternion.Euler(
                        360f - maxLookUpDownAngle,
                        newLook.eulerAngles.y,
                        0f
                    );
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
        sensitivity *= 5f;

        if (sensitivity < 5f)
        {
            sensitivity = 5f;
        }

        lookSensitivity = new Vector2(sensitivity, sensitivity);
    }

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
