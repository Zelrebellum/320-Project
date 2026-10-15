using UnityEngine;

public class UIComponents : MonoBehaviour
{
    [SerializeField] private GameObject hudCooldown;
    public GameObject HudCooldown
    {
        get => hudCooldown;
    }

    [SerializeField] private GameObject crouchIcon;
    public GameObject CrouchIcon
    {
        get => crouchIcon;
    }

    [SerializeField] private GameObject postureIcon;

    public GameObject PostureIcon
    {
        get => postureIcon;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

}
