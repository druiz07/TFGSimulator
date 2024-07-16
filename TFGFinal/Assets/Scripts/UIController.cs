using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Slider musicslider;
    public Slider sfxslider;
    public Text musicText;
    public Text sfxText;
    float previousMusicValue;
    float previousSFXValue;

    public void Start()
    {
        previousMusicValue = AudioManager.Instance.GetMusicVolume();
        previousSFXValue = AudioManager.Instance.SFXVolume();
        float value = previousMusicValue * 100;
        int valueint = (int)value;
        musicText.text = valueint + "%";

        value = (float)previousSFXValue * 100;
        valueint = (int)value;
        sfxText.text = valueint + "%";

        musicslider.value = previousMusicValue;
        sfxslider.value = previousSFXValue;
    }


    public void MusicVolume()
    {
        float value = (float)musicslider.value * 100;
        int valueint = (int)value;
        musicText.text = valueint + "%";
        AudioManager.Instance.MusicVolume(musicslider.value);
    }
    public void SFXVolume()
    {
        float value = (float)sfxslider.value * 100;
        int valueint = (int)value;
        sfxText.text = valueint + "%";
        AudioManager.Instance.SFXVolume(sfxslider.value);
    }
}
