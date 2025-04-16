using UnityEngine;
using UnityEngine.Audio;

public class AudioSettingsManager : MonoBehaviour
{
    public static AudioSettingsManager Instance;

    public AudioMixer audioMixer;

    public float Volume { get; private set; } = 1f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadVolume();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void SetVolume(float sliderValue)
    {
        audioMixer.SetFloat("volume", sliderValue);
        PlayerPrefs.SetFloat("GameVolume", sliderValue);
    }

    private void LoadVolume()
    {
        Volume = PlayerPrefs.GetFloat("GameVolume", 0f);
        audioMixer.SetFloat("volume", Volume);
    }
}
