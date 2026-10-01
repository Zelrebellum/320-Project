using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashScreenManager : MonoBehaviour
{
    public float displayTime = 2.5f;

    /// <summary>
    /// Show 2.5 second then in Mainmenu
    /// </summary>
    /// <returns></returns>
    IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(displayTime);

        SceneManager.LoadScene("MainMenu");
    }
}