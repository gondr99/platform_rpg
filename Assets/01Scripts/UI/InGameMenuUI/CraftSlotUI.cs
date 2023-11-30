using System;
using UnityEngine.EventSystems;

public class CraftSlotUI : ItemSlotUI
{
    public void SetUpCraftSlot(ItemDataEquipment data)
    {
        if (data == null) return;
        
        item.data = data;
        _itemImage.sprite = data.icon;
        _itemText.text = data.itemName;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
         ItemDataEquipment craftData = item.data as ItemDataEquipment;
         UIHelper.Instance.CraftWindow.SetUpCraftWindow(craftData);
    }
}
