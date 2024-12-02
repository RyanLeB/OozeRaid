using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    // ---- References to the audio sources ----
    
    public AudioSource musicSource;
    public AudioSource sfxSource;

    // ---- Lists of music and SFX clips ----
    
    public List<AudioClip> musicClips;
    public List<AudioClip> sfxClips;

    // ---- Sliders for music and SFX volume ----
    
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    // ---- Dictionaries to store the music and SFX clips ----
    
    private Dictionary<string, AudioClip> musicDictionary;
    private Dictionary<string, AudioClip> sfxDictionary;

    // ---- Keys for PlayerPrefs ----
    
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

        // ---- Set the MUSIC sliders to the saved volume values ----
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = savedMusicVolume;
            musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        }
        SetMusicVolume(savedMusicVolume);
        
        // ---- Set the SOUNDFX sliders to the saved volume values ----
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = savedSFXVolume;
            sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
        }
        SetSFXVolume(savedSFXVolume);
    }

    private void InitializeDictionaries()
    {
        // ---- Initialize the music and SFX dictionaries ----
        
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
        // ---- Set the volume of the music source ----
        
        if (musicSource != null)
        {
            musicSource.volume = volume;
        }
    }

    public void SetSFXVolume(float volume)
    {
        // ---- Set the volume of the SFX source ----
        
        if (sfxSource != null)
        {
            sfxSource.volume = volume;
        }
    }

    public void PlayMusic(string name)
    {
        // ---- Checks if dictionary is null ----
        if (musicDictionary == null)
        {
            Debug.LogError("Music dictionary is not initialized.");
            return;
        }

        // ---- Checks if the music clip exists in the dictionary ----
        if (musicDictionary.TryGetValue(name, out var clip))
        {
            //Debug.Log($"Playing music: {name}");
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
        // ---- Checks if dictionary is null ----
        if (sfxDictionary == null)
        {
            Debug.LogError("SFX dictionary is not initialized.");
            return;
        }
        
        // ---- Checks if the SFX clip exists in the dictionary ----
        if (sfxDictionary.TryGetValue(name, out var clip))
        {
            //Debug.Log($"Playing SFX: {name}");
            sfxSource.PlayOneShot(clip);
        }
        else
        {
            Debug.LogWarning($"SFX clip with name {name} not found!");
        }
    }

    // ---- Random pitch method to play SFX like collecting currency ----
    public void PlaySFXWithRandomPitch(string name, float minPitch, float maxPitch)
    {
        if (sfxDictionary.TryGetValue(name, out var clip))
        {
            float randomPitch = Random.Range(minPitch, maxPitch);
            //Debug.Log($"Playing SFX with random pitch: {name}, Pitch: {randomPitch}");
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