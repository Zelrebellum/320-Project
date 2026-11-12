using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Attached to the object that advances the level (e.g. hole). 
/// </summary>
public class LevelSceneManager : MonoBehaviour
{
    
    /// <summary>
    /// The scene for the next level the player advances to
    /// Action timer is a function pointer to the track timer in PlayerHUD, keeping the time according to the scene.
    /// </summary>
    public static void NextLevel(string LevelName, Action timer)
    {
        timer();
        SceneManager.LoadScene(LevelName);
    }
}
