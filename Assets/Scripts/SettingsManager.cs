using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public AudioMixer mainMixer;

    [Header("UI")]
    public Toggle musicToggle;
    public Toggle sfxToggle;

    private const string BgmKey = "BGM";
    private const string SfxKey = "SFX";

    private void Start()
    {
        int musicOn = PlayerPrefs.GetInt(BgmKey, 1);
        int sfxOn = PlayerPrefs.GetInt(SfxKey, 1);

        SetMusic(musicOn == 1);
        SetSFX(sfxOn == 1);

        if (musicToggle != null) musicToggle.isOn = (musicOn == 1);
        if (sfxToggle != null) sfxToggle.isOn = (sfxOn == 1);
    }

    public void SetMusic(bool isOn)
    {
        if (isOn)
            mainMixer.SetFloat("MusicVol", 0);
        else
            mainMixer.SetFloat("MusicVol", -80);

        PlayerPrefs.SetInt(BgmKey, isOn ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    public void SetSFX(bool isOn)
    {
        if (isOn)
            mainMixer.SetFloat("SFXVol", 0);
        else
            mainMixer.SetFloat("SFXVol", -80);

        PlayerPrefs.SetInt(SfxKey, isOn ? 1 : 0);
        PlayerPrefs.Save();
    }
}
