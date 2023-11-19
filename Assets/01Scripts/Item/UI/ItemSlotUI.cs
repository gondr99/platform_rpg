using System;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class ItemSlotUI : MonoBehaviour, IPointerDownHandler
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
        if (item.data.itemType == ItemType.Equipment)
        {
            Inventory.Instance.EquipItem(item.data);
        }
    }
}
