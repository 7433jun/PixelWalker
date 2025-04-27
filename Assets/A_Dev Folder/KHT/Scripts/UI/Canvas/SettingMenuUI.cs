using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingMenuUI : MonoBehaviour, Iui
{
    [Header("Panels")]
    public GameObject displayPanel;
    public GameObject soundPanel;
    public GameObject gameplayPanel;

    [Header("Display Settings")]
    public TMP_Dropdown windowModeDropdown;
    public TMP_Dropdown resolutionDropdown;
    public TMP_Dropdown maxFPSDropdown;

    [Header("Sound Settings")]
    public Slider bgmSlider;
    public Slider fxSlider;

    [Header("Gameplay Settings")]
    public TMP_Dropdown languagesDropdown;

    [Header("Apply Button")]
    public Button applyButton;

    void Start()
    {
        applyButton.onClick.AddListener(GameManager.Setting.ApplyButton);

        GameManager.UI.SetIui(this);
        UpdateLanguage();
    }

    public void UpdateLanguage()
    {

    }
}
