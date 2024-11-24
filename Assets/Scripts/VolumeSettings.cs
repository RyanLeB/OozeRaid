using UnityEngine;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    private const string MusicVolumeKey = "MusicVolume";
    private const string SFXVolumeKey = "SFXVolume";

    void Start()
    {
        // ---- Load saved volume values ----
        musicVolumeSlider.value = PlayerPrefs.GetFloat(MusicVolumeKey, 1f);
        sfxVolumeSlider.value = PlayerPrefs.GetFloat(SFXVolumeKey, 1f);

        // ---- Add listeners to handle volume changes ----
        musicVolumeSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMusicVolume(float volume)
    {
        // ---- Set the music volume in the audio manager ----
        GameManager.Instance.audioManager.SetMusicVolume(volume);
        // ---- Save the volume value ----
        PlayerPrefs.SetFloat(MusicVolumeKey, volume);
    }

    public void SetSFXVolume(float volume)
    {
        // ---- Set the SFX volume in the audio manager ----
        GameManager.Instance.audioManager.SetSFXVolume(volume);
        // ---- Save the volume value ----
        PlayerPrefs.SetFloat(SFXVolumeKey, volume);
    }
}