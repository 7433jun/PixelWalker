using CSVData;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour, Iui
{
    [Header("Label Texts")]
    [SerializeField] TextMeshProUGUI equipmentLabelText;
    [SerializeField] TextMeshProUGUI coreLabelText;
    [SerializeField] TextMeshProUGUI costLabelText;
    [SerializeField] TextMeshProUGUI infoLabelText;
    [SerializeField] TextMeshProUGUI infoNameLabelText;
    [SerializeField] TextMeshProUGUI infoCostLabelText;
    [SerializeField] TextMeshProUGUI infoDescriptionLabelText;

    [Header("Cost")]
    [SerializeField] TextMeshProUGUI costText;
    [SerializeField] Slider costSlider;
    [SerializeField] GameObject spentCostBar;
    [SerializeField] GameObject expectedCostBar;
    [SerializeField] Color costAvailableColor;
    [SerializeField] Color costWarningColor;

    [Header("Info")]
    [SerializeField] Image infoIconImage;
    [SerializeField] TextMeshProUGUI infoNameText;
    [SerializeField] TextMeshProUGUI infoCostText;
    [SerializeField] TextMeshProUGUI infoDescriptionText;

    [Header("Panels")]
    [SerializeField] GameObject equipmentSlotPanel;
    [SerializeField] GameObject inventorySlotPanel;
    [SerializeField] GameObject effectPanel;

    [Header("Prefabs")]
    [SerializeField] GameObject equipmentSlotPrefab;
    [SerializeField] GameObject inventorySlotPrefab;
    [SerializeField] GameObject effectPrefab;
    [SerializeField] GameObject dragIconPrefab;

    [Header("Sprite")]
    [SerializeField] Sprite lockSprite;

    private float equipmentLabelFontSize;
    private float coreLabelFontSize;
    private float costLabelFontSize;
    private float infoLabelFontSize;
    private float infoNameLabelFontSize;
    private float infoCostLabelFontSize;
    private float infoDescriptionLabelFontSize;

    private float infoNameFontSize;
    private float infoCostFontSize;
    private float infoDescriptionFontSize;

    private float effectNameFontSize;
    private float effectDescriptionFontSize;

    private List<EquipmentSlotUI> equipmentSlotUIList;
    private List<InventorySlotUI> inventorySlotUIList;
    private GameObject dragIcon;
    private float sliderWidth;
    private int currentInfoItemIndex;
    private Canvas canvas;

    public GameObject DragIcon => dragIcon;
    public Sprite LockSprite => lockSprite;
    public Canvas Canvas => canvas;

    void Start()
    {
        GameManager.UI.SetIui(this);

        equipmentSlotUIList = new List<EquipmentSlotUI>();
        inventorySlotUIList = new List<InventorySlotUI>();

        for (int i = 0; i < GameManager.Inventory.equipmentSize; i++)
        {
            GameObject equipmentSlot = Instantiate(equipmentSlotPrefab, equipmentSlotPanel.transform);
            EquipmentSlotUI equipmentSlotUI = equipmentSlot.GetComponent<EquipmentSlotUI>();
            equipmentSlotUI.SetIndex(i);
            equipmentSlotUIList.Add(equipmentSlotUI);
        }

        for (int i = 0; i < GameManager.Inventory.inventorySize; i++)
        {
            GameObject inventorySlot = Instantiate(inventorySlotPrefab, inventorySlotPanel.transform);
            InventorySlotUI inventorySlotUI = inventorySlot.GetComponent<InventorySlotUI>();
            inventorySlotUI.SetIndex(i);
            inventorySlotUIList.Add(inventorySlotUI);
        }

        dragIcon = Instantiate(dragIconPrefab, transform);
        sliderWidth = costSlider.GetComponent<RectTransform>().rect.width;
        canvas = GetComponent<Canvas>();

        equipmentLabelFontSize = equipmentLabelText.fontSize;
        coreLabelFontSize = coreLabelText.fontSize;
        costLabelFontSize = costLabelText.fontSize;
        infoLabelFontSize = infoLabelText.fontSize;
        infoNameLabelFontSize = infoNameLabelText.fontSize;
        infoCostLabelFontSize = infoCostLabelText.fontSize;
        infoDescriptionLabelFontSize = infoDescriptionLabelText.fontSize;

        infoNameFontSize = infoNameText.fontSize;
        infoCostFontSize = infoCostText.fontSize;
        infoDescriptionFontSize = infoDescriptionText.fontSize;

        EffectSlotUI tempEffectSlotUI = effectPrefab.GetComponent<EffectSlotUI>();
        effectNameFontSize = tempEffectSlotUI.nameText.fontSize;
        effectDescriptionFontSize = tempEffectSlotUI.descriptionText.fontSize;

        DrawInventory();
        DrawInfo(0);

        UpdateLanguage();
    }

    public void DrawInventory()
    {
        // Equipment
        foreach (var slot in equipmentSlotUIList)
        {
            slot.SetSlotUI();
        }

        // Item List
        foreach (var slot in inventorySlotUIList)
        {
            slot.SetSlotUI();
        }

        // Core
        foreach (Transform child in effectPanel.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < GameManager.Inventory.equipmentSize; i++)
        {
            int itemIndex = GameManager.Inventory.equipment[i];

            if (itemIndex != 0)
            {
                ItemData itemData = GameManager.Data.ItemDataTable[itemIndex];

                GameObject effectSlot = Instantiate(effectPrefab, effectPanel.transform);
                EffectSlotUI effectSlotUI = effectSlot.GetComponent<EffectSlotUI>();

                effectSlotUI.indexText.text = $"NO_{i + 1}";

                effectSlotUI.nameText.font = GameManager.Data.GetFont();
                effectSlotUI.nameText.fontSize = GameManager.Data.GetFontSize(effectNameFontSize);
                effectSlotUI.nameText.text = GameManager.Data.GetContentsString(itemData.NameStringIndex);

                effectSlotUI.descriptionText.font = GameManager.Data.GetFont();
                effectSlotUI.descriptionText.fontSize = GameManager.Data.GetFontSize(effectDescriptionFontSize);
                effectSlotUI.descriptionText.text = GameManager.Data.GetContentsString(itemData.DescriptionStringIndex, itemData.EffectValue.ToString());
            }
        }

        // Cost
        costText.text = $"{GameManager.Inventory.currentCost}/{GameManager.Inventory.maxCost}";

        if (GameManager.Inventory.currentCost == 0)
        {
            costSlider.value = 0;
        }
        else
        {
            costSlider.value = (float)GameManager.Inventory.currentCost / GameManager.Inventory.maxCost;
        }

        // Info
        DrawInfo(currentInfoItemIndex);
    }

    public void DrawInfo(int itemIndex)
    {
        currentInfoItemIndex = itemIndex;

        RectTransform spentCostBarRect = spentCostBar.GetComponent<RectTransform>();
        RectTransform expectedCostBarRect = expectedCostBar.GetComponent<RectTransform>();
        spentCostBarRect.sizeDelta = Vector2.zero;
        expectedCostBarRect.sizeDelta = Vector2.zero;

        if (itemIndex != 0)
        {
            ItemData itemData = GameManager.Data.ItemDataTable[itemIndex];

            infoIconImage.sprite = Resources.Load<Sprite>(itemData.IconResourceName);
            infoIconImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            infoNameText.text = GameManager.Data.GetContentsString(itemData.NameStringIndex);
            infoCostText.text = itemData.Cost.ToString();
            infoDescriptionText.text = GameManager.Data.GetContentsString(itemData.DescriptionStringIndex, itemData.EffectValue.ToString());

            if (GameManager.Inventory.equipment.Contains(itemIndex))
            {
                spentCostBarRect.sizeDelta = new Vector2((float)itemData.Cost / GameManager.Inventory.maxCost * sliderWidth, 0.0f);
            }
            else
            {
                if (GameManager.Inventory.currentCost + itemData.Cost < GameManager.Inventory.maxCost)
                {
                    expectedCostBar.GetComponent<Image>().color = costAvailableColor;
                    expectedCostBarRect.sizeDelta = new Vector2((float)itemData.Cost / GameManager.Inventory.maxCost * sliderWidth, 0.0f);
                }
                else
                {
                    expectedCostBar.GetComponent<Image>().color = costWarningColor;
                    expectedCostBarRect.sizeDelta = new Vector2(((float)GameManager.Inventory.maxCost - GameManager.Inventory.currentCost) / GameManager.Inventory.maxCost * sliderWidth, 0.0f);
                }
            }
        }
        else
        {
            infoIconImage.sprite = null;
            infoIconImage.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
            infoNameText.text = string.Empty;
            infoCostText.text = string.Empty;
            infoDescriptionText.text = string.Empty;
        }
    }

    public int GetEquipmentItemIndex(int equipmentSlotIndex)
    {
        return GameManager.Inventory.equipment[equipmentSlotIndex];
    }

    public int GetInventoryItemIndex(int inventorySlotIndex)
    {
        return GameManager.Inventory.inventory[inventorySlotIndex];
    }

    public Sprite GetEquipmentItemSprite(int equipmentSlotIndex)
    {
        return Resources.Load<Sprite>(GameManager.Data.ItemDataTable[GetEquipmentItemIndex(equipmentSlotIndex)].IconResourceName);
    }

    public Sprite GetInventoryItemSprite(int inventorySlotIndex)
    {
        return Resources.Load<Sprite>(GameManager.Data.ItemDataTable[GetInventoryItemIndex(inventorySlotIndex)].IconResourceName);
    }

    public bool GetIsInventoryUnlocked(int inventorySlotIndex)
    {
        return GameManager.Inventory.isInventoryUnlocked[inventorySlotIndex];
    }

    public void Equip(int inventorySlotIndex)
    {
        GameManager.Inventory.Equip(inventorySlotIndex);
    }

    public void Equip(int inventorySlotIndex, int equipmentSlotIndex)
    {
        GameManager.Inventory.Equip(inventorySlotIndex, equipmentSlotIndex);
    }

    public void Unequip(int equipmentSlotIndex)
    {
        GameManager.Inventory.Unequip(equipmentSlotIndex);
    }

    public void ChangeSlot(int equipmentSlotIndex1, int equipmentSlotIndex2)
    {
        GameManager.Inventory.ChangeSlot(equipmentSlotIndex1, equipmentSlotIndex2);
    }

    public void UpdateLanguage()
    {
        equipmentLabelText.font = GameManager.Data.GetFont();
        equipmentLabelText.fontSize = GameManager.Data.GetFontSize(equipmentLabelFontSize);
        equipmentLabelText.text = GameManager.Data.GetSystemText("InventoryUI", "Equipment");

        coreLabelText.font = GameManager.Data.GetFont();
        coreLabelText.fontSize = GameManager.Data.GetFontSize(coreLabelFontSize);
        coreLabelText.text = GameManager.Data.GetSystemText("InventoryUI", "Core");

        costLabelText.font = GameManager.Data.GetFont();
        costLabelText.fontSize = GameManager.Data.GetFontSize(costLabelFontSize);
        costLabelText.text = GameManager.Data.GetSystemText("InventoryUI", "Cost");

        infoLabelText.font = GameManager.Data.GetFont();
        infoLabelText.fontSize = GameManager.Data.GetFontSize(infoLabelFontSize);
        infoLabelText.text = GameManager.Data.GetSystemText("InventoryUI", "Info");

        infoNameLabelText.font = GameManager.Data.GetFont();
        infoNameLabelText.fontSize = GameManager.Data.GetFontSize(infoNameLabelFontSize);
        infoNameLabelText.text = GameManager.Data.GetSystemText("InventoryUI", "Name");

        infoCostLabelText.font = GameManager.Data.GetFont();
        infoCostLabelText.fontSize = GameManager.Data.GetFontSize(infoCostLabelFontSize);
        infoCostLabelText.text = GameManager.Data.GetSystemText("InventoryUI", "Cost");

        infoDescriptionLabelText.font = GameManager.Data.GetFont();
        infoDescriptionLabelText.fontSize = GameManager.Data.GetFontSize(infoDescriptionLabelFontSize);
        infoDescriptionLabelText.text = GameManager.Data.GetSystemText("InventoryUI", "Description");


        infoNameText.font = GameManager.Data.GetFont();
        infoNameText.fontSize = GameManager.Data.GetFontSize(infoNameFontSize);

        infoCostText.font = GameManager.Data.GetFont();
        infoCostText.fontSize = GameManager.Data.GetFontSize(infoCostFontSize);

        infoDescriptionText.font = GameManager.Data.GetFont();
        infoDescriptionText.fontSize = GameManager.Data.GetFontSize(infoDescriptionFontSize);

        DrawInventory();
    }
}
