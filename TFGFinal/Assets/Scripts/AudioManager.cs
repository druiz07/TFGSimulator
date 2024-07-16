using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public Sound[] musicSounds, sfxSounds;
    public AudioSource musicSource, sfxsource;
    public static AudioManager Instance;
    public string musicSceneName = "LandingScene";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void Start()
    {
        //PlayMusic("Theme");
    }
    private void OnDestroy()
    {
        // Asegúrate de desuscribirte del evento cuando el objeto se destruya
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Verificar si la escena cargada coincide con el nombre de la escena para la música
        if (scene.name != musicSceneName)
        {
            if (!musicSource.isPlaying)
            {
                PlayMusic("Theme");
            }
        }
        else
        {
            StopMusic("Theme");
        }
    }
    public void PlayMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);
        if (s == null)
        {
            Debug.Log("music sound not found");
        }
        else
        {

            musicSource.clip = s.clip;
            musicSource.Play();

        }
    }
    public void StopMusic(string name)
    {
        Sound s = Array.Find(musicSounds, x => x.name == name);
        if (s == null)
        {
            Debug.Log("music sound not found");
        }
        else
        {


            musicSource.Stop();

        }
    }
    public void PlaySFX(string name)
    {
        Sound s = Array.Find(sfxSounds, x => x.name == name);
        if (s == null)
        {
            Debug.Log("SFX sound not found");
        }
        else
        {

            sfxsource.PlayOneShot(s.clip);
        }
    }

    public void ToggleMusic()
    {
        musicSource.mute = !musicSource.mute;
    }
    public void ToggleSFX()
    {
        sfxsource.mute = !sfxsource.mute;
    }
    public void MusicVolume(float volume)
    {
        musicSource.volume = volume;
    }
    public void SFXVolume(float volume)
    {
        sfxsource.volume = volume;
    }
    public float GetMusicVolume()
    {
        return musicSource.volume;
    }
    public float SFXVolume()
    {
        return sfxsource.volume;
    }
}
