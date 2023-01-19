using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private List<AudioClip> musicTracks;
    [SerializeField] private List<AudioClip> soundEffects;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource soundEffectSource;
    [SerializeField] private float defaultMusicVolume = 0.5f;
    [SerializeField] private float defaultSoundEffectVolume = 1f;

    [Header("Settings UI")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider soundEffectsSlider;

    private const string MUSIC_VOLUME_KEY = "music_volume";
    private const string SOUND_EFFECT_VOLUME_KEY = "sound_effect_volume";

    public static AudioManager Instance;

    private void Awake()
    {
        Instance = this;
        LoadSoundSettings();
    }

    private void LoadSoundSettings()
    {
        musicSource.volume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, defaultMusicVolume);
        soundEffectSource.volume = PlayerPrefs.GetFloat(SOUND_EFFECT_VOLUME_KEY, defaultSoundEffectVolume);

        if (musicSlider != null) musicSlider.value = musicSource.volume;
        if (soundEffectsSlider != null) soundEffectsSlider.value = soundEffectSource.volume;
    }

    public void SaveMusicVolume(float volume)
    {
        musicSource.volume = volume;
        PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }

    public void SaveEffectsVolume(float volume)
    {
        soundEffectSource.volume = volume;
        PlayerPrefs.SetFloat(SOUND_EFFECT_VOLUME_KEY, volume);
        PlayerPrefs.Save();
    }

    public void PlayMusic(int index)
    {
        if (index < 0 || index >= musicTracks.Count)
        {
            Debug.LogError("Invalid index for music tracks!");
            return;
        }

        musicSource.clip = musicTracks[index];
        musicSource.Play();
    }

    public void PlaySoundEffect(int index)
    {
        if (index < 0 || index >= soundEffects.Count)
        {
            Debug.LogError("Invalid index for sound effects!");
            return;
        }

        soundEffectSource.PlayOneShot(soundEffects[index]);
    }
}
