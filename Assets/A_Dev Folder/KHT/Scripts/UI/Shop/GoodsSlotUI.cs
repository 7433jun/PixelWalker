using CSVData;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GoodsSlotUI : MonoBehaviour, IPointerClickHandler
{
    private int slotIndex;
    public Image itemIconImage;
    public Image priceImage;
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemPriceText;
    public Button buyButton;
    public TextMeshProUGUI buyButtonText;

    public void SetSlotUI(int index)
    {
        slotIndex = index;
        SetSlotBuyable();
    }

    public void BuyButton()
    {
        GameManager.UI.StoreUI.BuySlotItem(slotIndex);
        GameManager.UI.StoreUI.RefreshStore();
    }

    public void SetSlotBuyable()
    {
        buyButton.interactable = GameManager.UI.StoreUI.GetItemBuyable(slotIndex);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            GameManager.UI.StoreUI.DrawInfo(slotIndex);
        }
    }
}
