using UnityEngine;
using UnityEngine.UI;


public class PlayerHUD : MonoBehaviour
{
    [Header("HUD References")]
    [SerializeField] private GameObject hudCooldown;
    [SerializeField] private GameObject crouchIcon;
    [SerializeField] private GameObject standIcon;

    [Header("Icon Settings")]
    [SerializeField] private float fadeIconTime = 2f;

    private float fadeTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        FadeIcon();
    }

    public void ShowCrouch()
    {
        crouchIcon.SetActive(true);
        standIcon.SetActive(false);
    }

    public void ShowStanding()
    {
        crouchIcon.SetActive(false);
        standIcon.SetActive(true);

        fadeTimer = fadeIconTime;

        Image image = standIcon.GetComponent<Image>();
        Color color = image.color;
        color.a = 1f;
        image.color = color;
    }

    private void FadeIcon()
    {
        if (!standIcon.activeSelf)
        {
            return;
        }

        if(fadeTimer <= 0)
        {
            standIcon.SetActive(false);
            return;
        }

        fadeTimer -= Time.deltaTime;

        Vector4 color = standIcon.GetComponent<Image>().color;
        color.w = fadeTimer / fadeIconTime;
        standIcon.GetComponent<Image>().color = color;
    }

    public void UpdateDashCooldown(float progress)
    {
        Vector3 scale = hudCooldown.transform.localScale;
        scale.x = Mathf.Clamp01(progress);
        hudCooldown.transform.localScale = scale;
    }


}
