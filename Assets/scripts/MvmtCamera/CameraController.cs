using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Camera playerCam;

    [SerializeField]
    bool upDownInversion = true;

    [SerializeField]
    bool limitUpDownRange = false;

    [SerializeField]
    Vector2 lookSensitivity = Vector2.one;

    [SerializeField]
    float maxLookUpDownAngle = 25;

    private void Awake()
    {
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        playerCam.transform.rotation = Quaternion.Euler(0f, 0f, 0f);
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        if (Application.isFocused)
        {
            OnFocus();
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
                    newLook = Quaternion.Euler(maxLookUpDownAngle, newLook.eulerAngles.y, newLook.eulerAngles.z);
                }

                // otherwise, if we're past the max angle looking up AND looking up
                if (newLook.eulerAngles.x < 360 - maxLookUpDownAngle && newLook.eulerAngles.x >= 180f)
                {
                    newLook = Quaternion.Euler(360 - maxLookUpDownAngle, newLook.eulerAngles.y, newLook.eulerAngles.z);
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
        CameraReset();
    }

    public void OnFocus()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }
}