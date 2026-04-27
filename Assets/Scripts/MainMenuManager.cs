using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [Header("UI Popups")]
    public GameObject themePopup;
    public GameObject statsPopup;
    public GameObject settingsPopup;
    public GameObject exitPopup;
    
    public Button startButton;

    [Header("Audio Settings")]
    public AudioSource audioSource;
    public AudioClip clickSound;

    public static string SelectedTheme = "";

    [Header("Scene Settings")]
    public string gameSceneName = "GameScene";

    // cached reference
    private StatisticsManager cachedStatsManager;

    private void Start()
    {
        // Ensure popups are closed at start
        if (themePopup != null) themePopup.SetActive(false);
        if (statsPopup != null) statsPopup.SetActive(false);
        if (settingsPopup != null) settingsPopup.SetActive(false);
        if (exitPopup != null) exitPopup.SetActive(false);

        // Load previously selected theme
        SelectedTheme = PlayerPrefs.GetString("SelectedTheme", "");

        if (startButton != null)
        {
            startButton.interactable = !string.IsNullOrEmpty(SelectedTheme);
        }

        // apply theme visuals via ThemeApplier if present
        var applier = FindObjectOfType<ThemeApplier>();
        if (applier != null) applier.ApplyTheme();
    }

    public void OpenThemePopup()
    {
        if (themePopup == null) return;
        themePopup.SetActive(true);
    }

    // metoda za stats dugme
    public void OpenStatsPopup()
    {
        if (statsPopup == null) return;
        statsPopup.SetActive(true);

        if (cachedStatsManager == null)
            cachedStatsManager = statsPopup.GetComponent<StatisticsManager>();

        if (cachedStatsManager != null)
            cachedStatsManager.DisplayStats();
    }

    public void OpenSettingsPopup()
    {
        if (settingsPopup == null) return;
        settingsPopup.SetActive(true);
    }

    public void OpenExitPopup()
    {
        if (exitPopup == null) return;
        exitPopup.SetActive(true);
    }

    public void SetThemeClassic()
    {
        PlayClickSound();
        SetTheme("Classic");
    }

    public void SetThemeDark()
    {
        PlayClickSound();
        SetTheme("Dark");
    }

    // unified theme setter
    public void SetTheme(string theme)
    {
        if (string.IsNullOrEmpty(theme)) return;

        SelectedTheme = theme;
        PlayerPrefs.SetString("SelectedTheme", SelectedTheme);
        PlayerPrefs.Save();

        Debug.Log("Selected Theme: " + SelectedTheme);

        // notify ThemeApplier in this scene to update visuals
        var applier = FindObjectOfType<ThemeApplier>();
        if (applier != null)
            applier.ApplyTheme();

        if (startButton != null) startButton.interactable = true;
    }
   

    // start dugme - open game scene
    public void StartGame()
    {
        if (string.IsNullOrEmpty(gameSceneName))
        {
            Debug.LogWarning("Game scene name is not set.");
            return;
        }

        SceneManager.LoadScene(gameSceneName);
    }

    // exit dugme - game exit
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

    public void PlayClickSound()
    {
        if (audioSource == null || clickSound == null) return;
        audioSource.PlayOneShot(clickSound);
    }
}
