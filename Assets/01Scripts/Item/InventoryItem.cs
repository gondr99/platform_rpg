using System;

[Serializable]
public class InventoryItem
{
    public ItemData data;
    public int stackSize; //몇개 있냐?
    
    public InventoryItem(ItemData newItemData)
    {
        data = newItemData;
        AddStack();
    }

    public void AddStack()
    {
        ++stackSize;
    }

    public void RemoveStack()
    {
        --stackSize;
    }
}
