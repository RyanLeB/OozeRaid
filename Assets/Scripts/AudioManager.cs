using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public AudioSource musicSource;
    public AudioSource sfxSource;

    public List<AudioClip> musicClips;
    public List<AudioClip> sfxClips;

    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    private Dictionary<string, AudioClip> musicDictionary;
    private Dictionary<string, AudioClip> sfxDictionary;

    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";

    private void Awake()
    {
        InitializeDictionaries();
    }
    
    private void Start()
    {
        

        float savedMusicVolume = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        float savedSFXVolume = PlayerPrefs.GetFloat(SFXVolumeKey, 1f);

        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = savedMusicVolume;
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }
        SetMusicVolume(savedMusicVolume);

        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = savedSFXVolume;
            sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        }
        SetSFXVolume(savedSFXVolume);
    }

    private void InitializeDictionaries()
    {
        musicDictionary = new Dictionary<string, AudioClip>();
        foreach (var clip in musicClips)
        {
            musicDictionary[clip.name] = clip;
        }

        sfxDictionary = new Dictionary<string, AudioClip>();
        foreach (var clip in sfxClips)
        {
            sfxDictionary[clip.name] = clip;
        }
    }

    public void SetMusicVolume(float volume)
    {
        if (musicSource != null)
        {
            musicSource.volume = volume;
        }
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = volume;
        }
    }

    public void PlayMusic(string name)
    {
        if (musicDictionary == null)
        {
            Debug.LogError("Music dictionary is not initialized.");
            return;
        }

        if (musicDictionary.TryGetValue(name, out var clip))
        {
            Debug.Log($"Playing music: {name}");
            musicSource.clip = clip;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning($"Music clip with name {name} not found in the dictionary.");
        }
    }

    public void PlaySFX(string name)
    {
        if (sfxDictionary.TryGetValue(name, out var clip))
        {
            Debug.Log($"Playing SFX: {name}");
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"SFX clip with name {name} not found!");
        }
    }

    public void PlaySFXWithRandomPitch(string name, float minPitch, float maxPitch)
    {
        if (sfxDictionary.TryGetValue(name, out var clip))
        {
            float randomPitch = Random.Range(minPitch, maxPitch);
            Debug.Log($"Playing SFX with random pitch: {name}, Pitch: {randomPitch}");
            sfxSource.pitch = randomPitch;
            sfxSource.PlayOneShot(clip);
            sfxSource.pitch = 1f;
        }
        else
        {
            Debug.LogWarning($"SFX clip with name {name} not found!");
        }
    }
}