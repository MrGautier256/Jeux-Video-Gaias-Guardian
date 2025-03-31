using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    private static bool musicPlaying = false;

    void Awake()
    {
        if (!musicPlaying)
        {
            DontDestroyOnLoad(gameObject);
            GetComponent<AudioSource>().Play();
            musicPlaying = true;
        }
        else
        {
            Destroy(gameObject); 
        }
    }
}