using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
public class ChangeMusic : MonoBehaviour
{

    private AudioSource audio;

    [SerializeField]
    private AudioClip LoopingAudio;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audio = GetComponent<AudioSource>();
        
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Safe"))
        {
            Debug.Log("here");
            audio.loop = false;
            StartCoroutine(AudioEnd());
        }
    }

    private IEnumerator AudioEnd()
    {
        yield return new WaitUntil(() => !audio.isPlaying);
        audio.clip = LoopingAudio;
        audio.loop = true;
        audio.Play();
    }
}
