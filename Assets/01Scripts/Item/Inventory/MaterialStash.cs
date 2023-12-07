using System.Collections.Generic;
using UnityEngine;

public class MaterialStash : Stash
{
    public MaterialStash(Transform parent) : base(parent)
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
    
    public bool CanCraft(ItemDataEquipment itemToCraft, List<InventoryItem> requiredMaterials)
    {
        List<MaterialPair> materialsToRemove = new List<MaterialPair>();
        
        for (int i = 0; i < requiredMaterials.Count; ++i)
        {
            if (stashDictionary.TryGetValue(requiredMaterials[i].data, out InventoryItem stashItem))
            {
                if (stashItem.stackSize < requiredMaterials[i].stackSize)
                {
                    return false;
                }
                
                materialsToRemove.Add(new MaterialPair{ item = stashItem, count = requiredMaterials[i].stackSize});
            }
            else
            {
                return false;
            }
        }

        for (int i = 0; i < materialsToRemove.Count; ++i)
        {
            RemoveItem(materialsToRemove[i].item.data, materialsToRemove[i].count); //갯수만큼 제거
        }
        
        //AddItem(itemToCraft); //이건 밖에서 할 일임.
        //Debug.Log($"crafting success : {itemToCraft.name}");
        return true;
    }
}
