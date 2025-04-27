using TMPro;
using UnityEngine;

public class MainMenuUI : MonoBehaviour, Iui
{
    [Header("Localization List")]
    [SerializeField] TextMeshProUGUI startButtonText;
    [SerializeField] TextMeshProUGUI loadButtonText;
    [SerializeField] TextMeshProUGUI settingButtonText;
    [SerializeField] TextMeshProUGUI exitButtonText;

    private float startButtonFontSize;
    private float loadButtonFontSize;
    private float settingButtonFontSize;
    private float exitButtonFontSize;

    void Start()
    {
        GameManager.UI.SetIui(this);

        startButtonFontSize = startButtonText.fontSize;
        loadButtonFontSize = loadButtonText.fontSize;
        settingButtonFontSize = settingButtonText.fontSize;
        exitButtonFontSize = exitButtonText.fontSize;

        UpdateLanguage();
    }

    public void UpdateLanguage()
    {
        startButtonText.font = GameManager.Data.GetFont();
        startButtonText.fontSize = GameManager.Data.GetFontSize(startButtonFontSize);
        startButtonText.text = GameManager.Data.GetSystemText("MainMenuUI", "StartButton");

        loadButtonText.font = GameManager.Data.GetFont();
        loadButtonText.fontSize = GameManager.Data.GetFontSize(loadButtonFontSize);
        loadButtonText.text = GameManager.Data.GetSystemText("MainMenuUI", "LoadButton");

        settingButtonText.font = GameManager.Data.GetFont();
        settingButtonText.fontSize = GameManager.Data.GetFontSize(settingButtonFontSize);
        settingButtonText.text = GameManager.Data.GetSystemText("MainMenuUI", "SettingButton");

        exitButtonText.font = GameManager.Data.GetFont();
        exitButtonText.fontSize = GameManager.Data.GetFontSize(exitButtonFontSize);
        exitButtonText.text = GameManager.Data.GetSystemText("MainMenuUI", "ExitButton");
    }
}
