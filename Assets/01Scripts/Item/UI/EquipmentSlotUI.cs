
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class EquipmentSlotUI : ItemSlotUI
{
    public EquipmentType slotType;

    private void OnValidate()
    {
        gameObject.name = $"Equip Slot [ {slotType.ToString()} ]";
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        RectTransform trm = transform as RectTransform;

        Inventory.Instance.UnEquipItem(item.data as ItemDataEquipment);
        CleanUpSlot();
    }
}
