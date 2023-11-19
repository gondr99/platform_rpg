using System;
using UnityEngine.EventSystems;

public class CraftSlotUI : ItemSlotUI
{
    private void OnEnable()
    {
        UpdateSlot(item);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
         ItemDataEquipment craftData = item.data as ItemDataEquipment;
         Inventory.Instance.CanCraft(craftData, craftData.craftingMaterials);
    }
}
