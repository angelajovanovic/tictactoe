using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThemeApplier : MonoBehaviour
{
    public Camera mainCamera;

    [Header("Colors")]
    public Color classicColor;
    public Color darkColor;

    [Header("UI Visuals")]
    public Image backgroundImage;
    public Sprite classicSprite;
    public Sprite darkSprite;

    [Header("UI Elements to Tint")]
    public Graphic[] themedGraphics;
    public Color classicTextColor = Color.black;
    public Color darkTextColor = Color.white;

    void Start()
    {
        ApplyTheme();
    }

    public void ApplyTheme()
    {
        bool isDark = (MainMenuManager.SelectedTheme == "Dark");

        if (mainCamera == null) mainCamera = Camera.main;

        if (mainCamera != null)
        {
            mainCamera.clearFlags = CameraClearFlags.SolidColor;
            mainCamera.backgroundColor = isDark ? darkColor : classicColor;
        }

        // 1. Sredjivanje pozadinske slike i njene BOJE
        if (backgroundImage != null)
        {
            
            if (isDark && darkSprite != null) backgroundImage.sprite = darkSprite;
            else if (!isDark && classicSprite != null) backgroundImage.sprite = classicSprite;

            
            backgroundImage.color = isDark ? darkColor : classicColor;
        }

        // 2. Tintovanje ostalih elemenata (tekstovi, ikonice)
        if (themedGraphics != null && themedGraphics.Length > 0)
        {
            Color textColor = isDark ? darkTextColor : classicTextColor;
            for (int i = 0; i < themedGraphics.Length; i++)
            {
                if (themedGraphics[i] != null)
                {
                    // Ovde idu samo tekstovi i stvari koje treba da budu crne/bele
                    themedGraphics[i].color = textColor;
                }
            }
        }
    }
}