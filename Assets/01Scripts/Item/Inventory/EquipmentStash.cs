using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentStash : Stash
{
    public EquipmentStash(Transform parent) : base(parent)
    {
    }

    public override void AddItem(ItemData item)
    {
        if (stashDictionary.TryGetValue(item, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(item);
            stash.Add(newItem);
            stashDictionary.Add(item, newItem);
        }
    }

    public override void RemoveItem(ItemData item, int count)
    {
        if (stashDictionary.TryGetValue(item, out InventoryItem stashValue))
        {
            if (stashValue.stackSize <= count)
            {
                stash.Remove(stashValue);
                stashDictionary.Remove(item);
            }
            else
            {
                stashValue.RemoveStack(count);
            }
        }
    }

    public override bool CanAddItem()
    {
        if (stash.Count >= _itemSlots.Length)
        {
            Debug.Log("No more space in stash");
            return false;
        }
        return true;
    }
}
