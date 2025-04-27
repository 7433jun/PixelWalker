using CSVData;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoreUI : MonoBehaviour, Iui
{
    [Header("Title")]
    [SerializeField] TextMeshProUGUI titleText;

    [Header("Label")]
    [SerializeField] TextMeshProUGUI infoLabelText;
    [SerializeField] TextMeshProUGUI infoNameLabelText;
    [SerializeField] TextMeshProUGUI infoCostLabelText;
    [SerializeField] TextMeshProUGUI infoDescriptionLabelText;

    [Header("Info")]
    [SerializeField] Image infoIconImage;
    [SerializeField] TextMeshProUGUI infoNameText;
    [SerializeField] TextMeshProUGUI infoCostText;
    [SerializeField] TextMeshProUGUI infoDescriptionText;

    [Header("Store")]
    [SerializeField] GameObject storePanel;
    [SerializeField] GameObject goodsSlotPrefab;
    [SerializeField] List<Sprite> priceTypeSprite;

    private float titleFontSize;
    private float infoLabelFontSize;
    private float infoNameLabelFontSize;
    private float infoCostLabelFontSize;
    private float infoDescriptionLabelFontSize;

    private float infoNameFontSize;
    private float infoCostFontSize;
    private float infoDescriptionFontSize;

    private float goodsNameFontSize;

    private List<GoodsSlotUI> goodsSlotUIList;

    void Start()
    {
        GameManager.UI.SetIui(this);

        goodsSlotUIList = new List<GoodsSlotUI>();

        titleFontSize = titleText.fontSize;
        infoLabelFontSize = infoLabelText.fontSize;
        infoNameLabelFontSize = infoNameLabelText.fontSize;
        infoCostLabelFontSize = infoCostLabelText.fontSize;
        infoDescriptionLabelFontSize = infoDescriptionLabelText.fontSize;

        infoNameFontSize = infoNameText.fontSize;
        infoCostFontSize = infoCostText.fontSize;
        infoDescriptionFontSize = infoDescriptionText.fontSize;

        GoodsSlotUI tempGoodsSlotUI = goodsSlotPrefab.GetComponent<GoodsSlotUI>();
        goodsNameFontSize = tempGoodsSlotUI.itemNameText.fontSize;

        UpdateLanguage();
    }

    public void DrawStore(int storeIndex)
    {
        InitInfo();

        goodsSlotUIList.Clear();

        foreach (Transform child in storePanel.transform)
        {
            Destroy(child.gameObject);
        }

        if (storeIndex == 0) return;

        for (int i = 0; i < GameManager.Data.StoreDataTable[storeIndex].ItemList.Count; i++)
        {
            ItemData itemData = GameManager.Data.ItemDataTable[GameManager.Data.StoreDataTable[storeIndex].ItemList[i]];

            GameObject goodsSlot = Instantiate(goodsSlotPrefab, storePanel.transform);
            GoodsSlotUI goodsSlotUI = goodsSlot.GetComponent<GoodsSlotUI>();
            goodsSlotUI.itemIconImage.sprite = Resources.Load<Sprite>(itemData.IconResourceName);
            goodsSlotUI.priceImage.sprite = priceTypeSprite[(int)itemData.PriceType];
            goodsSlotUI.itemNameText.font = GameManager.Data.GetFont();
            goodsSlotUI.itemNameText.fontSize = GameManager.Data.GetFontSize(goodsNameFontSize);
            goodsSlotUI.itemNameText.text = GameManager.Data.GetContentsString(itemData.NameStringIndex);
            goodsSlotUI.itemPriceText.text = itemData.BuyPrice.ToString();
            goodsSlotUI.SetSlotUI(i);
        }
    }

    public void DrawInfo(int slotIndex)
    {
        int itemIndex = GameManager.Store.GetItemIndex(slotIndex);

        if (itemIndex != 0)
        {
            ItemData itemData = GameManager.Data.ItemDataTable[itemIndex];

            infoIconImage.sprite = Resources.Load<Sprite>(itemData.IconResourceName);
            infoIconImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
            infoNameText.text = GameManager.Data.GetContentsString(itemData.NameStringIndex);
            infoCostText.text = itemData.Cost.ToString();
            infoDescriptionText.text = GameManager.Data.GetContentsString(itemData.DescriptionStringIndex, itemData.EffectValue.ToString());
        }
        else
        {
            InitInfo();
        }
    }

    private void InitInfo()
    {
        infoIconImage.sprite = null;
        infoIconImage.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        infoNameText.text = string.Empty;
        infoCostText.text = string.Empty;
        infoDescriptionText.text = string.Empty;
    }

    public void RefreshStore()
    {
        foreach (var slotUI in goodsSlotUIList)
        {
            slotUI.SetSlotBuyable();
        }
    }

    public void BuySlotItem(int slotIndex)
    {
        GameManager.Store.Buy(GameManager.Store.GetItemIndex(slotIndex));
    }

    public bool GetItemBuyable(int slotIndex)
    {
        return GameManager.Store.CheckBuyable(GameManager.Store.GetItemIndex(slotIndex));
    }

    public void UpdateLanguage()
    {
        titleText.font = GameManager.Data.GetFont();
        titleText.fontSize = GameManager.Data.GetFontSize(titleFontSize);
        titleText.text = GameManager.Data.GetSystemText("StoreUI", "Store");

        infoLabelText.font = GameManager.Data.GetFont();
        infoLabelText.fontSize = GameManager.Data.GetFontSize(infoLabelFontSize);
        infoLabelText.text = GameManager.Data.GetSystemText("StoreUI", "Info");

        infoNameLabelText.font = GameManager.Data.GetFont();
        infoNameLabelText.fontSize = GameManager.Data.GetFontSize(infoNameLabelFontSize);
        infoNameLabelText.text = GameManager.Data.GetSystemText("StoreUI", "Name");

        infoCostLabelText.font = GameManager.Data.GetFont();
        infoCostLabelText.fontSize = GameManager.Data.GetFontSize(infoCostLabelFontSize);
        infoCostLabelText.text = GameManager.Data.GetSystemText("StoreUI", "Cost");

        infoDescriptionLabelText.font = GameManager.Data.GetFont();
        infoDescriptionLabelText.fontSize = GameManager.Data.GetFontSize(infoDescriptionLabelFontSize);
        infoDescriptionLabelText.text = GameManager.Data.GetSystemText("StoreUI", "Description");


        infoNameText.font = GameManager.Data.GetFont();
        infoNameText.fontSize = GameManager.Data.GetFontSize(infoNameFontSize);

        infoCostText.font = GameManager.Data.GetFont();
        infoCostText.fontSize = GameManager.Data.GetFontSize(infoCostFontSize);

        infoDescriptionText.font = GameManager.Data.GetFont();
        infoDescriptionText.fontSize = GameManager.Data.GetFontSize(infoDescriptionFontSize);


        DrawStore(0);
    }
}
