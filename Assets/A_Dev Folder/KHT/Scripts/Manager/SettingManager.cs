using JsonData;
using System.IO;
using UnityEngine;
using Project.Enums;

public class SettingManager
{
    private static SettingManager instance;
    public static SettingManager Instance
    {
        get
        {
            if (instance == null)
                instance = new SettingManager();
            return instance;
        }
    }

    private SettingData settingData;

    public static SettingData SettingData => Instance.settingData;

    private SettingManager()
    {
        LoadSettings();
    }

    private void LoadSettings()
    {
        if (File.Exists(Path.Combine(Application.persistentDataPath, "Settings.json")))
        {
            settingData = PWFile.LoadJson<SettingData>(Application.persistentDataPath, "Settings");
        }
        else
        {
            settingData = new SettingData
            {
                windowModeValue = 1, //FullScreenWindow
                resolutionValue = 1, //1920*1080
                maxFPSValue = 2, //60
                bgmValue = 0.8f,
                fxValue = 0.8f,
                languageValue = Language.EN
            };
            SaveSettings();
        }
    }

    private void SaveSettings()
    {
        PWFile.SaveJson(settingData, Application.persistentDataPath, "Settings");
    }

    public void ApplyButton()
    {
        if (GameManager.UI.SettingMenu.displayPanel.activeSelf)
        {
            ApplyDisplaySettings();
        }
        else if (GameManager.UI.SettingMenu.soundPanel.activeSelf)
        {
            ApplySoundSettings();
        }
        else if (GameManager.UI.SettingMenu.gameplayPanel.activeSelf)
        {
            ApplyGameplaySettings();
        }

        SaveSettings();
    }

    private void ApplyDisplaySettings()
    {
        // window mode setting
        switch (GameManager.UI.SettingMenu.windowModeDropdown.value)
        {
            case 0:
                Screen.fullScreen = false;
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
            case 1:
                Screen.fullScreen = true;
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            default:
                break;
        }

        // resolution setting
        if (Screen.fullScreenMode == FullScreenMode.Windowed)
        {
            int width = 1280;
            int height = 720;
            switch (GameManager.UI.SettingMenu.resolutionDropdown.value)
            {
                case 0:
                    width = 1280; height = 720;
                    break;
                case 1:
                    width = 1920; height = 1080;
                    break;
                case 2:
                    width = 3840; height = 2160;
                    break;
            }
            Screen.SetResolution(width, height, false);
        }

        // FPS setting
        int[] fpsOptions = { -1, 30, 60, 144 };
        Application.targetFrameRate = fpsOptions[GameManager.UI.SettingMenu.maxFPSDropdown.value];
    }

    private void ApplySoundSettings()
    {

    }

    private void ApplyGameplaySettings()
    {
        switch (GameManager.UI.SettingMenu.languagesDropdown.value)
        {
            case 0:
                settingData.languageValue = Language.EN;
                break;
            case 1:
                settingData.languageValue = Language.KO;
                break;
        }

        GameManager.UI.UpdateLanguageUIList();
    }
}
