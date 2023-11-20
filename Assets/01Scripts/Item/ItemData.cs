using UnityEngine;

public enum ItemType
{
    Material,
    Equipment
}

[CreateAssetMenu(menuName = "SO/Items/Item", fileName = "New Item data")]
public class ItemData : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public Sprite icon;

    [Range(0, 100)]
    public float dropChance;
}
