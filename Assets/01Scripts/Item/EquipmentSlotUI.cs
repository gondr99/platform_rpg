
using System;

public class EquipmentSlotUI : ItemSlotUI
{
    public EquipmentType slotType;

    private void OnValidate()
    {
        gameObject.name = $"Equip Slot [ {slotType.ToString()} ]";
    }
}
