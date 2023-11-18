using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    Weapon,
    Armor,
    Amulet,
    Flask
}

[CreateAssetMenu(menuName = "SO/Items/Equipment", fileName = "New Item data")]
public class ItemDataEquipment : ItemData
{
    public EquipmentType equipmentType;
}
