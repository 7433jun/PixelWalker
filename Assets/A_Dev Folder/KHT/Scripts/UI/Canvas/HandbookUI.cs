using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HandbookUI : MonoBehaviour, Iui
{
    [Header("Canvases")]
    [SerializeField] Canvas mapCanvas;
    [SerializeField] Canvas inventoryCanvas;
    [SerializeField] Canvas achievementCanvas;

    [Header("Button Images")]
    [SerializeField] Image mapButtonImage;
    [SerializeField] Image inventoryButtonImage;
    [SerializeField] Image achievementButtonImage;

    [Header("Button Texts")]
    [SerializeField] TextMeshProUGUI mapButtonText;
    [SerializeField] TextMeshProUGUI inventoryButtonText;
    [SerializeField] TextMeshProUGUI achievementButtonText;

    [Header("Title Text")]
    [SerializeField] TextMeshProUGUI titleText;

    [Header("Colors")]
    [SerializeField] Color normalImageColor;
    [SerializeField] Color normalTextColor;
    [SerializeField] Color selectedImageColor;
    [SerializeField] Color selectedTextColor;

    private float mapButtonFontSize;
    private float inventoryButtonFontSize;
    private float achievementButtonFontSize;
    private float titleFontSize;

    private Canvas canvas;
    public Canvas Canvas => canvas;

    public enum HandbookUIType
    {
        Default,
        Map,
        Inventory,
        Achievement
    }

    private HandbookUIType currentUI;

    void Start()
    {
        GameManager.UI.SetIui(this);

        canvas = GetComponent<Canvas>();

        mapButtonFontSize = mapButtonText.fontSize;
        inventoryButtonFontSize = inventoryButtonText.fontSize;
        achievementButtonFontSize = achievementButtonText.fontSize;
        titleFontSize = titleText.fontSize;

        SelectUIButton(2);
        UpdateLanguage();
    }

    private void ResetUI()
    {
        mapCanvas.enabled = false;
        inventoryCanvas.enabled = false;
        achievementCanvas.enabled = false;

        mapButtonImage.color = normalImageColor;
        inventoryButtonImage.color = normalImageColor;
        achievementButtonImage.color = normalImageColor;
        
        mapButtonText.color = normalTextColor;
        inventoryButtonText.color = normalTextColor;
        achievementButtonText.color = normalTextColor;
    }

    public void SelectUIButton(int typeindex)
    {
        HandbookUIType type = (HandbookUIType)typeindex;

        if (type == currentUI)
            return;

        ResetUI();

        switch (type)
        {
            case HandbookUIType.Map:
                mapButtonImage.color = selectedImageColor;
                mapButtonText.color = selectedTextColor;
                mapCanvas.enabled = true;
                break;
            case HandbookUIType.Inventory:
                inventoryButtonImage.color = selectedImageColor;
                inventoryButtonText.color = selectedTextColor;

                //GameManager.UI.InventoryUI.DrawInfo(0);

                inventoryCanvas.enabled = true;
                break;
            case HandbookUIType.Achievement:
                achievementButtonImage.color = selectedImageColor;
                achievementButtonText.color = selectedTextColor;
                achievementCanvas.enabled = true;
                break;
        }

        currentUI = type;
        titleText.text = GameManager.Data.GetSystemText("HandbookUI", currentUI.ToString());
    }

    public void UpdateLanguage()
    {
        mapButtonText.font = GameManager.Data.GetFont();
        mapButtonText.fontSize = GameManager.Data.GetFontSize(mapButtonFontSize);
        mapButtonText.text = GameManager.Data.GetSystemText("HandbookUI", "Map");

        inventoryButtonText.font = GameManager.Data.GetFont();
        inventoryButtonText.fontSize = GameManager.Data.GetFontSize(inventoryButtonFontSize);
        inventoryButtonText.text = GameManager.Data.GetSystemText("HandbookUI", "Inventory");

        achievementButtonText.font = GameManager.Data.GetFont();
        achievementButtonText.fontSize = GameManager.Data.GetFontSize(achievementButtonFontSize);
        achievementButtonText.text = GameManager.Data.GetSystemText("HandbookUI", "Achievement");

        titleText.font = GameManager.Data.GetFont();
        titleText.fontSize = GameManager.Data.GetFontSize(titleFontSize);
        titleText.text = GameManager.Data.GetSystemText("HandbookUI", currentUI.ToString());
    }
}
