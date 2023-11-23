using System;
using UnityEngine.EventSystems;

public class CraftSlotUI : ItemSlotUI
{
    public void SetUpCraftSlot(ItemDataEquipment data)
    {
        item.data = data;
        _itemImage.sprite = data.icon;
        _itemText.text = data.itemName;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
         ItemDataEquipment craftData = item.data as ItemDataEquipment;
         Inventory.Instance.CanCraft(craftData, craftData.craftingMaterials);
    }
}
