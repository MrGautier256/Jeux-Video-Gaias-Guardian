using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MusicController : MonoBehaviour
{
    public static MusicController Instance;

    [System.Serializable]
    public class SceneMusic
    {
        public string sceneName;
        public AudioClip musicClip;
    }

    public List<SceneMusic> sceneMusicList = new List<SceneMusic>();

    private Dictionary<string, AudioClip> musicByScene;
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
            audioSource.volume = AudioSettingsManager.Instance != null ? AudioSettingsManager.Instance.Volume : 1f;
            SceneManager.sceneLoaded += OnSceneLoaded;

            // Convert list to dictionary
            musicByScene = new Dictionary<string, AudioClip>();
            foreach (var item in sceneMusicList)
            {
                musicByScene[item.sceneName] = item.musicClip;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (musicByScene.TryGetValue(scene.name, out AudioClip newClip))
        {
            if (audioSource.clip != newClip)
            {
                audioSource.clip = newClip;
                audioSource.Play();
            }
        }
    }
}
