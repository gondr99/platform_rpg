using System;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ItemSlotUI : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] protected Image _itemImage;
    [SerializeField] protected TextMeshProUGUI _itemText;

    public InventoryItem item;

    public void UpdateSlot(InventoryItem newItem)
    {
        item = newItem;
        _itemImage.color = Color.white;
        
        if (item != null)
        {
            _itemImage.sprite = item.data.icon;

            if (item.stackSize > 1)
            {
                _itemText.text = item.stackSize.ToString();
            }
            else
            {
                _itemText.text = String.Empty;
            }
        }
    }

    public void CleanUpSlot()
    {
        item = null;
        _itemImage.sprite = null;
        _itemImage.color = Color.clear;
        
        _itemText.text = String.Empty;
    }

    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (item == null) return;
        
        if (item.data.itemType == ItemType.Equipment)
        {
            //여기서 컨텍스트 메뉴 띄우고 장착할지 버릴지 결정하게 함.
            //Inventory.Instance.EquipItem(item.data);
            RectTransform trm = transform as RectTransform;
            
            UIHelper.Instance.OpenEquipContextMenu(trm.position, this);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item == null || item.data == null) return;
        UIHelper.Instance.ItemTooltip.ShowTooltip(item.data as ItemDataEquipment);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (item == null || item.data == null) return;
        UIHelper.Instance.ItemTooltip.HideTooltip();
    }
}
