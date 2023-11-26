using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//제작을 위한 구조체
public struct MaterialPair
{
    public InventoryItem item;
    public int count;
}

public class Inventory : MonoSingleton<Inventory>
{
    public event Action<bool, float, float> OnFlaskCooldownEvent;

    //장비하고 있는 것
    public List<InventoryItem> equipments;
    public Dictionary<ItemDataEquipment, InventoryItem> equipmentDictionary;

    //인벤토리
    public List<InventoryItem> inventory;
    public Dictionary<ItemData, InventoryItem> inventoryDictionary;

    //창고
    public List<InventoryItem> stash;
    public Dictionary<ItemData, InventoryItem> stashDictionary;

    [Header("Inventory UI")] 
    [SerializeField] private Transform _inventorySlotParent;
    [SerializeField] private Transform _stashSlotParent;
    [SerializeField] private Transform _equipmentSlotParent;

    [SerializeField] private Transform _statSlotParent;
    private ItemSlotUI[] _inventoryItemSlots; //인벤토링 아이템 슬롯(장비등)
    private ItemSlotUI[] _stashItemSlots; //창고아이템 슬롯(재료등)
    private EquipmentSlotUI[] _equipmentSlots;
    private StatSlotUI[] _statSlots;
    
    private float _flaskCooldown;
    private float _lastFlaskUseTime;

    //디버그용 
    public ItemDataEquipment[] initEquipList; 
        
    private void Awake()
    {
        equipments = new List<InventoryItem>();
        equipmentDictionary = new Dictionary<ItemDataEquipment, InventoryItem>();
        _equipmentSlots = _equipmentSlotParent.GetComponentsInChildren<EquipmentSlotUI>();

        inventory = new List<InventoryItem>();
        inventoryDictionary = new Dictionary<ItemData, InventoryItem>();
        _inventoryItemSlots = _inventorySlotParent.GetComponentsInChildren<ItemSlotUI>(); //자식에 있는 아이템 슬롯을 전부 가져온다.

        stash = new List<InventoryItem>();
        stashDictionary = new Dictionary<ItemData, InventoryItem>();
        _stashItemSlots = _stashSlotParent.GetComponentsInChildren<ItemSlotUI>();

        _statSlots = _statSlotParent.GetComponentsInChildren<StatSlotUI>();
    }

    //디버그용.
    private void Start()
    {
        foreach (ItemDataEquipment equipment in initEquipList)
        {
            AddItem(equipment);
        }
    }

    //플라스크 쿨타임은 플라스크가 아니라 인벤토리에서 관리하는게 맞다.
    public void UseFlask()
    {
        ItemDataEquipment flask = GetEquipmentByType(EquipmentType.Flask);
        
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

    private void UpdateSlotUI()
    {
        //기존 창고 클린하고
        for (int i = 0; i < _equipmentSlots.Length; ++i)
        {
            //해당 슬롯에 들어갈 수 있는 아이템을 보유중이라면
            ItemDataEquipment slotEquipment = equipmentDictionary.Keys.ToList().Find(x => x.equipmentType == _equipmentSlots[i].slotType);
            if (slotEquipment != null)
            {
                _equipmentSlots[i].UpdateSlot(equipmentDictionary[slotEquipment]);
            }
        }
        
        for (int i = 0; i < _inventoryItemSlots.Length; ++i)
        {
            _inventoryItemSlots[i].CleanUpSlot();
        }
        
        for (int i = 0; i < _stashItemSlots.Length; ++i)
        {
            _stashItemSlots[i].CleanUpSlot();
        }
        
        
        //새로 그리기.
        for (int i = 0; i < inventory.Count; ++i)
        {
            _inventoryItemSlots[i].UpdateSlot(inventory[i]);
        }
        
        for (int i = 0; i < stash.Count; ++i)
        {
            _stashItemSlots[i].UpdateSlot(stash[i]);
        }
        
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
        InventoryItem newItem = new InventoryItem(newEquipment);


        ItemDataEquipment oldEquipment = null; 
        foreach (KeyValuePair<ItemDataEquipment, InventoryItem> equipKeyValue in equipmentDictionary)
        {
            //같은 장비 타입이면. 기존 장비 해제하고 
            if (equipKeyValue.Key.equipmentType == newEquipment.equipmentType)
            {
                oldEquipment = equipKeyValue.Key; //삭제할 아이템 저장.
                break;
            }
        }

        if (oldEquipment != null)
        {
            UnEquipItem(oldEquipment);
        }

        equipments.Add(newItem);
        equipmentDictionary.Add(newEquipment, newItem);
        newEquipment.AddModifiers(); //장착한 아이템의 효과 적용.


        if (newEquipment.equipmentType == EquipmentType.Flask)
        {
            OnFlaskCooldownEvent?.Invoke(true, _lastFlaskUseTime, _flaskCooldown);
        }
        
        RemoveItem(item);//장착한 아이템은 인벤토리에서 삭제한다. (장비칸으로 넘어갔으니까)
        UpdateSlotUI();
    }

    //장비 장착 해제.
    public void UnEquipItem(ItemDataEquipment oldEquipment, bool backToInventory = true)
    {
        if (oldEquipment == null) return;
        if (equipmentDictionary.TryGetValue(oldEquipment, out InventoryItem value))
        {
            equipments.Remove(value);
            equipmentDictionary.Remove(oldEquipment); //리스트와 딕셔너리에서 모두 빼준다.
            oldEquipment.RemoveModifiers(); //빼준 아이템의 효과 빼준다.
            
            //인벤토리로 돌아가야 하면.
            if(backToInventory)
                AddItem(oldEquipment); //삭제하고 인벤토리에 다시 넣어준다.
            
            //플라스크면 이벤트 발행
            if (oldEquipment.equipmentType == EquipmentType.Flask)
            {
                OnFlaskCooldownEvent?.Invoke(false, _lastFlaskUseTime, _flaskCooldown);
            }
        }
    }


    public bool CanAddItem()
    {
        if (inventory.Count >= _inventoryItemSlots.Length)
        {
            Debug.Log("No more space in inventory");
            return false;
        }
        return true;
    }
    
    public void AddItem(ItemData item)
    {
        if (item.itemType == ItemType.Equipment && CanAddItem())
        {
            AddToInventory(item);
        }else if (item.itemType == ItemType.Material)
        {
            AddToStash(item);
        }

        UpdateSlotUI();
    }

    private void AddToInventory(ItemData item)
    {
        //장비 인벤토리는 스택개념을 없앤다.
        if (inventoryDictionary.TryGetValue(item, out InventoryItem value))
        {
            value.AddStack();
        }
        else
        {
            InventoryItem newItem = new InventoryItem(item);
            inventory.Add(newItem);
            inventoryDictionary.Add(item, newItem);
        }
    }

    private void AddToStash(ItemData item)
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

    public void RemoveItem(ItemData item, int count = 1)
    {
        if (inventoryDictionary.TryGetValue(item, out InventoryItem value))
        {
            if (value.stackSize <= count)
            {
                inventory.Remove(value);
                inventoryDictionary.Remove(item);
            }
            else
            {
                value.RemoveStack(count);
            }
        }
        
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
        
        UpdateSlotUI();
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
                    Debug.Log("not enough materials");
                    return false;
                }
                
                materialsToRemove.Add(new MaterialPair{ item = stashItem, count = requiredMaterials[i].stackSize});
            }
            else
            {
                Debug.Log("not enough materials");
                return false;
            }
        }

        for (int i = 0; i < materialsToRemove.Count; ++i)
        {
            RemoveItem(materialsToRemove[i].item.data, materialsToRemove[i].count); //갯수만큼 제거
        }
        
        AddItem(itemToCraft);
        Debug.Log($"crafting success : {itemToCraft.name}");
        return true;
    }


    public ItemDataEquipment GetEquipmentByType(EquipmentType type)
    {
        ItemDataEquipment equipItem = null;
    
        foreach (KeyValuePair<ItemDataEquipment, InventoryItem> equipKeyValue in equipmentDictionary)
        {
            //지정된 장비 타입을 장착하고 있다면 가져온다. 
            if (equipKeyValue.Key.equipmentType == type)
            {
                equipItem = equipKeyValue.Key;
                break;
            }
        }
        
        return equipItem;
    }
    
}
