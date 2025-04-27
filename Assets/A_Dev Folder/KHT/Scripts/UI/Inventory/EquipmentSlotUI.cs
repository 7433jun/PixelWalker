using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EquipmentSlotUI : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] Image iconImage;

    private int equipmentSlotIndex;
    private bool isEquiped;

    private InventoryUI Presenter
    {
        get
        {
            return GameManager.UI.InventoryUI;
        }
    }

    public void SetIndex(int index) { equipmentSlotIndex = index; }
    public int GetIndex() { return equipmentSlotIndex; }

    public void SetSlotUI()
    {
        if (Presenter.GetEquipmentItemIndex(equipmentSlotIndex) != 0)
        {
            isEquiped = true;

            iconImage.sprite = Presenter.GetEquipmentItemSprite(equipmentSlotIndex);
            iconImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        }
        else
        {
            isEquiped = false;

            iconImage.sprite = null;
            iconImage.color = new Color(1.0f, 1.0f, 1.0f, 0.0f);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isEquiped)
            return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Presenter.DrawInfo(Presenter.GetEquipmentItemIndex(equipmentSlotIndex));
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Presenter.Unequip(equipmentSlotIndex);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!isEquiped)
            return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Presenter.DrawInfo(Presenter.GetEquipmentItemIndex(equipmentSlotIndex));

            Presenter.DragIcon.GetComponent<Image>().sprite = iconImage.sprite;
            Presenter.DragIcon.SetActive(true);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!isEquiped)
            return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Presenter.DragIcon.transform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!isEquiped)
            return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Presenter.DragIcon.SetActive(false);

            if (eventData.pointerCurrentRaycast.gameObject != null)
            {
                EquipmentSlotUI equipmentSlotUI = eventData.pointerCurrentRaycast.gameObject.GetComponent<EquipmentSlotUI>();
                InventorySlotUI inventorySlotUI = eventData.pointerCurrentRaycast.gameObject.GetComponent<InventorySlotUI>();

                if (equipmentSlotUI)
                {
                    Presenter.ChangeSlot(equipmentSlotIndex, equipmentSlotUI.GetIndex());
                }
                else if (inventorySlotUI)
                {
                    Presenter.Equip(inventorySlotUI.GetIndex(), equipmentSlotIndex);
                }
            }
            else
            {
                Presenter.Unequip(equipmentSlotIndex);
            }

            Presenter.DrawInfo(Presenter.GetEquipmentItemIndex(equipmentSlotIndex));
        }
    }
}
