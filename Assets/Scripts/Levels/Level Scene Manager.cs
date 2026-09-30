using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Attached to the object that advances the level (e.g. hole). 
/// </summary>
public class LevelSceneManager : MonoBehaviour
{
    /// <summary>
    /// The scene for the next level the player advances to
    /// </summary>
    [SerializeField] string nextLevel;

    /// <summary>
    /// Triggered when the player has dropped (touched) the hole. 
    /// Will progress the player to the next level. 
    /// </summary>
    /// <param name="collision">Enter collision of the player to this</param>
    public void OnCollisionEnter(Collision collision)
    {
        // TODO
        //Check that it is the player 
        //Optimize Scene transitions
        SceneManager.LoadScene(nextLevel);
    }
}
