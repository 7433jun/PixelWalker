using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] Image iconImage;

    private int inventorySlotIndex;
    private bool isUnlocked;

    private InventoryUI Presenter
    {
        get
        {
            return GameManager.UI.InventoryUI;
        }
    }

    public void SetIndex(int index) { inventorySlotIndex = index; }
    public int GetIndex() { return inventorySlotIndex; }

    public void SetSlotUI()
    {
        isUnlocked = Presenter.GetIsInventoryUnlocked(inventorySlotIndex);

        if (isUnlocked)
        {
            iconImage.sprite = Presenter.GetInventoryItemSprite(inventorySlotIndex);
        }
        else
        {
            iconImage.sprite = Presenter.LockSprite;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isUnlocked)
            return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Presenter.DrawInfo(Presenter.GetInventoryItemIndex(inventorySlotIndex));
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Presenter.Equip(inventorySlotIndex);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isUnlocked)
            return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Presenter.DrawInfo(Presenter.GetInventoryItemIndex(inventorySlotIndex));

            Presenter.DragIcon.GetComponent<Image>().sprite = iconImage.sprite;
            Presenter.DragIcon.SetActive(true);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isUnlocked)
            return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Presenter.DragIcon.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isUnlocked)
            return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Presenter.DragIcon.SetActive(false);

            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                EquipmentSlotUI equipmentSlotUI = eventData.pointerCurrentRaycast.gameObject.GetComponent<EquipmentSlotUI>();

                if (equipmentSlotUI)
                {
                    Presenter.Equip(inventorySlotIndex, equipmentSlotUI.GetIndex());

                    Presenter.DrawInfo(Presenter.GetInventoryItemIndex(inventorySlotIndex));
                }
            }
        }
    }
}
