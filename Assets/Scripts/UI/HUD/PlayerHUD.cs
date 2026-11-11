using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PlayerHUD : MonoBehaviour
{
    [Header("HUD References")]
    [SerializeField] private GameObject hudCooldown;
    [SerializeField] private GameObject crouchIcon;
    [SerializeField] private GameObject standIcon;

    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Icon Settings")]
    [SerializeField] private float fadeIconTime = 2f;

    private float fadeTimer;

    private float timer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        timer += Time.deltaTime;
        System.TimeSpan t = System.TimeSpan.FromSeconds(timer);

        timerText.text = string.Format("{0:D2}m:{1:D2}s:{2:D3}ms", t.Minutes, t.Seconds, t.Milliseconds);
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

// fade the stand/crounch Icon
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

//dash CD change the bar
    public void UpdateDashCooldown(float progress)
    {
        Vector3 scale = hudCooldown.transform.localScale;
        scale.x = Mathf.Clamp01(progress);
        hudCooldown.transform.localScale = scale;
    }


}
