using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

//제작을 위한 구조체
public struct MaterialPair
{
    public InventoryItem item;
    public int count;
}

public class Inventory : MonoSingleton<Inventory>, ISaveManager
{
    //장착여부, 현재쿨, 플라스크 쿨
    public event Action<bool, float, float> OnFlaskCooldownEvent;

    //장비하고 있는 것
    
    public EquipSlots equipSlots; //장착 칸들
    public EquipmentStash equipmentStash;//장비 창고
    public MaterialStash materialStash;//창고

    [Header("Inventory UI")] 
    [SerializeField] private Transform _equipStashSlotParent;
    [SerializeField] private Transform _stashSlotParent;
    [SerializeField] private Transform _equipmentSlotParent;

    [SerializeField] private Transform _statSlotParent;
    private StatSlotUI[] _statSlots;

    [Header("Database")]
    public List<InventoryItem> loadedItems;
    public List<ItemDataEquipment> loadedEquipment;


    private float _flaskCooldown;
    private float _lastFlaskUseTime;

    //디버그용 
    public ItemDataEquipment[] initEquipList; 
        
    private void Awake()
    {
        equipSlots = new EquipSlots(_equipmentSlotParent, this);//장비 장착칸 제작
        equipmentStash = new EquipmentStash(_equipStashSlotParent);//장비 창고 제작
        materialStash = new MaterialStash(_stashSlotParent);//재료 창고 제작

        _statSlots = _statSlotParent.GetComponentsInChildren<StatSlotUI>();
    }
    
    private void Start()
    {
        if (loadedEquipment.Count > 0)
        {
            foreach (ItemDataEquipment equip in loadedEquipment)
            {
                equipSlots.EquipItem(equip);
            }
        }
        
        if (loadedItems.Count > 0)
        {
            foreach (InventoryItem item in loadedItems)
            {
                for(int i = 0; i < item.stackSize; ++i)
                {
                    AddItem(item.data);
                }
            }
            return;
        }
        
        //여긴 디버그
        foreach (ItemDataEquipment equipment in initEquipList)
        {
            AddItem(equipment);
        }
    }

    //플라스크 쿨타임은 플라스크가 아니라 인벤토리에서 관리하는게 맞다.
    public void UseFlask()
    {
        ItemDataEquipment flask = equipSlots.GetEquipmentByType(EquipmentType.Flask);
        
        if (flask != null)
        {
            bool canUseFlask = Time.time > _lastFlaskUseTime + _flaskCooldown;;
            if (canUseFlask)
            {
                _flaskCooldown = flask.cooldown; //이렇게 하면 시작시에도 바로 사용 가능. ///굳이?
                flask.UseEquipment();
                _lastFlaskUseTime = Time.time;
                OnFlaskCooldownEvent?.Invoke(true, _lastFlaskUseTime, _flaskCooldown);
            }
            else
            {
                Debug.Log("Flask cooldown");
            }
        }
        else
        {
            Debug.Log("no flask");
        }
    }

    public void FlaskUnEquipped()
    {
        OnFlaskCooldownEvent?.Invoke(false, _lastFlaskUseTime, _flaskCooldown);
    }

    public void FlaskEquipped()
    {
        OnFlaskCooldownEvent?.Invoke(true, _lastFlaskUseTime, _flaskCooldown);
    }
    private void UpdateSlotUI()
    {
        equipSlots.UpdateSlotUI(); //장착 장비 새로그리기
        equipmentStash.UpdateSlotUI(); //장비 창고 다시 그리기.
        materialStash.UpdateSlotUI(); //재료창고 다시그리기.

        //스테이터스 갱신하기
        for (int i = 0; i < _statSlots.Length; ++i)
        {
            _statSlots[i].UpdateStatValueUI();
        }
    }

    //장비 장착
    public void EquipItem(ItemData item)
    {
        ItemDataEquipment newEquipment = item as ItemDataEquipment;
        if (newEquipment == null)
        {
            Debug.LogError("This is can not equip");
            return; //장비 아이템이 아닌경우 장착 불가.
        }
        
        equipSlots.EquipItem(newEquipment);
        equipmentStash.RemoveItem(item, 1);//장착한 아이템은 인벤토리에서 삭제한다. (장비칸으로 넘어갔으니까)
        UpdateSlotUI();
    }

    //장비 장착 해제.
    public void UnEquipItem(ItemDataEquipment oldEquipment)
    {
        if (oldEquipment == null) return;
        equipSlots.UnEquipItem(oldEquipment);
    }

    //이건 나중에 좀 변경해야 할듯.
    public bool CanAddItem()
    {
        return equipmentStash.CanAddItem();
    }
    
    public void AddItem(ItemData item)
    {
        if (item.itemType == ItemType.Equipment && CanAddItem())
        {
            equipmentStash.AddItem(item);
        }else if (item.itemType == ItemType.Material)
        {
            materialStash.AddItem(item);
        }
        UpdateSlotUI();
    }

    public void RemoveItem(ItemData item, int count = 1)
    {
        switch (item.itemType)
        {
            case ItemType.Material:
                if (equipmentStash.HasItem(item))
                {
                    equipmentStash.RemoveItem(item, count);
                }
                break;
            case ItemType.Equipment:
                if (materialStash.HasItem(item))
                {
                    materialStash.RemoveItem(item, count);
                }
                break;
        }
        UpdateSlotUI();
    }

    public bool CanCraft(ItemDataEquipment itemToCraft, List<InventoryItem> requiredMaterials)
    {
        if (materialStash.CanCraft(itemToCraft, requiredMaterials))
        {
            AddItem(itemToCraft);
            Debug.Log($"crafting success : {itemToCraft.name}");
            return true;
        }
        
        Debug.Log("not enough materials");
        return false;
    }


    public ItemDataEquipment GetEquipmentByType(EquipmentType type)
    {
        return equipSlots.GetEquipmentByType(type);
    }

    public void LoadData(GameData data)
    {
        //이거 빌드시 작동안할 확률이 매우 높음.
        List<ItemData> itemDataBase = GetItemDataBaseFromAssetDatabase();
        
        foreach (var pair in data.inventory)
        {
            ItemData item = itemDataBase.Find(x => x.itemID == pair.Key);
            if (item != null)
            {
                InventoryItem itemToLoad = new InventoryItem(item);
                itemToLoad.stackSize = pair.Value;
                
                loadedItems.Add(itemToLoad);
            }
        }

        //장착 장비들 복원
        foreach (string itemID in data.equipmentIDList)
        {
            ItemData item = itemDataBase.Find(x => x.itemID == itemID);
            if (item != null)
            {
                loadedEquipment.Add(item as ItemDataEquipment);
            }
        }
        Debug.Log("Items loaded");
    }

    public void SaveData(ref GameData data)
    {
        data.inventory.Clear();
        data.equipmentIDList.Clear();

        foreach (var pair in equipmentStash.stashDictionary)
        {
            data.inventory.Add(pair.Key.itemID, pair.Value.stackSize);   
        }

        foreach (var pair in materialStash.stashDictionary)
        {
            data.inventory.Add(pair.Key.itemID, pair.Value.stackSize);
        }
        
        foreach (var pair in equipSlots.equipmentDictionary)
        {
            data.equipmentIDList.Add(pair.Key.itemID);
        }
    }


    private List<ItemData> GetItemDataBaseFromAssetDatabase()
    {
        List<ItemData> itemDataBase = new List<ItemData>();
        string[] assetNames = AssetDatabase.FindAssets("", 
            new[] { "Assets/08SO/ItemData" });

        foreach (string soName in assetNames)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(soName);
            ItemData itemData = AssetDatabase.LoadAssetAtPath<ItemData>(assetPath);
            if(itemData != null)
                itemDataBase.Add(itemData);
        }

        return itemDataBase;
    }
}
