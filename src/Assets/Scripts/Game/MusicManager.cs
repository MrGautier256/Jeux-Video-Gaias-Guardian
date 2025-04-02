using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource.Play();
        }
        else
        {
            if (audioSource.clip != instance.audioSource.clip)
            {
                Destroy(instance.gameObject); 
                instance = this;
                DontDestroyOnLoad(gameObject);
                audioSource.Play();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
