using UnityEngine;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// Attached to the object that advances the level (e.g. hole). 
/// </summary>
public class LevelSceneManager : MonoBehaviour
{
    /// <summary>
    /// The scene for the next level the player advances to
    /// </summary>

    public static void NextLevel(string LevelName, Action timer)
    {
        timer();
        SceneManager.LoadScene(LevelName);

    }
}
