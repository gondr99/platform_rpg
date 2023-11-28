using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Object = System.Object;


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

    [Header("item effect")] 
    public ItemEffectSO[] effectList;

    //아이템 효과 적는곳.
    [TextArea]
    public string itemEffectDescription;
    
    [Header("Major stat")]
    public int strength;
    public int agility;
    public int intelligence;
    public int vitality;
    
    [Header("Offensive stats")]
    public int damage;
    public int criticalChance;
    public int criticalDamage;
    
    
    [Header("Defensive stat")]
    public int maxHealth; //체력
    public int armor; //방어도
    public int evasion; //회피도
    public int magicResistance; //마법방어
    
    [Header("Magic stats")] 
    public int fireDamage;
    public int ignitePercent;
    public int iceDamage;
    public int chillPercent;
    public int lightingDamage;
    public int shockPercent;

    [Header("Craft requirements")] public List<InventoryItem> craftingMaterials;

    [Header("Use Equipment")] 
    public float cooldown;
    protected float _lastUseTime;
    private int _descriptionLength;

    //필드 인포를 가지고 있는 딕셔너리.
    protected Dictionary<StatType, FieldInfo> _fieldInfoDictionary = new Dictionary<StatType, FieldInfo>();

    protected virtual void OnEnable()
    {
        _lastUseTime = -1500f;

        _fieldInfoDictionary.Clear();
        Type itemStatType = typeof(ItemDataEquipment); 
        foreach (StatType statType in Enum.GetValues(typeof(StatType)))
        {
            FieldInfo itemStatField = itemStatType.GetField(statType.ToString());
            if (itemStatField == null)
            {
                Debug.LogError($"There are no stat! error : {statType.ToString()}");
            }
            else
            {
                _fieldInfoDictionary.Add(statType, itemStatField);
            }
        }
    }

    //장비 사용
    public void UseEquipment()
    {
        if (_lastUseTime + cooldown > Time.time) return;
        
        foreach (ItemEffectSO effect in effectList)
        {
            effect.UseEffect(); //기본값 실행
        }

        _lastUseTime = Time.time;
    }

    //hitAttack은 적을 피격 성공했을 때만 발동하게 할 것인지를 나타냄.
    public void ItemEffectByMelee(bool hitAttack = false)
    {
        // 각 이펙트는 각자의 확률에 따라 실행됨.
        foreach (ItemEffectSO effect in effectList)
        {
            effect.ExecuteEffectByMelee(hitAttack); //아이템의 각 효과들을 수행함.
        }
    }

    public void ItemEffectBySkill(PlayerSkill skillType)
    {
        foreach (ItemEffectSO effect in effectList)
        {
            effect.ExecuteEffectBySkill(skillType); //아이템의 각 효과들을 수행함.
        }
    }

    public void ItemEffectByHit(Health health)
    {
        foreach (ItemEffectSO effect in effectList)
        {
            effect.ExecuteEffectByHit(health);
        }
    }
    
    public void AddModifiers()
    {
        PlayerStat playerStat = GameManager.Instance.Player.Stat as PlayerStat;
        if (playerStat == null)
            return;

        foreach (var fieldSet in _fieldInfoDictionary)
        {
            Stat stat = playerStat.GetStatByType(fieldSet.Key);
            stat.AddModifier( (int)fieldSet.Value.GetValue(this));
        }
        
    }

    public void RemoveModifiers()
    {
        PlayerStat playerStat = GameManager.Instance.Player.Stat as PlayerStat;
        if (playerStat == null)
            return;
        
        foreach (var fieldSet in _fieldInfoDictionary)
        {
            Stat stat = playerStat.GetStatByType(fieldSet.Key);
            stat.RemoveModifier( (int)fieldSet.Value.GetValue(this));
        }
    }

    public override string GetDescription()
    {
        _stringBuilder.Clear();
        _descriptionLength = 0;
        foreach (var fieldSet in _fieldInfoDictionary)
        {
            AddItemDescription( (int)fieldSet.Value.GetValue(this), fieldSet.Key.ToString() );
        }

        if (_descriptionLength < 5)
        {
            for (int i = _descriptionLength; i < 5; ++i)
            {
                _stringBuilder.AppendLine();
                _stringBuilder.Append("");
            }
        }

        if (!string.IsNullOrEmpty(itemEffectDescription))
        {
            _stringBuilder.AppendLine();
            _stringBuilder.Append(itemEffectDescription);
        }
        
        return _stringBuilder.ToString();
    }

    private void AddItemDescription(int value, string name)
    {
        if (value != 0)
        {
            if (_stringBuilder.Length > 0)
            {
                _stringBuilder.AppendLine();
            }

            ++_descriptionLength;
            _stringBuilder.Append($"{name} : {value.ToString()}");
        }
    }
}


// playerStat.strength.AddModifier(strength);
// playerStat.agility.AddModifier(agility);
// playerStat.intelligence.AddModifier(intelligence);
// playerStat.vitality.AddModifier(vitality);
//         
// playerStat.damage.AddModifier(damage);
// playerStat.criticalChance.AddModifier(criticalChance);
// playerStat.criticalDamage.AddModifier(criticalDamage);
//         
// playerStat.maxHealth.AddModifier(maxHealth);
// playerStat.armor.AddModifier(armor);
// playerStat.evasion.AddModifier(evasion);
// playerStat.magicResistance.AddModifier(magicResistance);
//         
// playerStat.fireDamage.AddModifier(fireDamage);
// playerStat.ignitePercent.AddModifier(ignitePercent);
// playerStat.iceDamage.AddModifier(iceDamage);
// playerStat.chillPercent.AddModifier(chillPercent);
// playerStat.lightingDamage.AddModifier(lightingDamage);
// playerStat.shockPercent.AddModifier(shockPercent);

// stat.strength.RemoveModifier(strength);
// stat.agility.RemoveModifier(agility);
// stat.intelligence.RemoveModifier(intelligence);
// stat.vitality.RemoveModifier(vitality);
//         
// stat.damage.RemoveModifier(damage);
// stat.criticalChance.RemoveModifier(criticalChance);
// stat.criticalDamage.RemoveModifier(criticalDamage);
//         
// stat.maxHealth.RemoveModifier(maxHealth);
// stat.armor.RemoveModifier(armor);
// stat.evasion.RemoveModifier(evasion);
// stat.magicResistance.RemoveModifier(magicResistance);
//         
// stat.fireDamage.RemoveModifier(fireDamage);
// stat.ignitePercent.RemoveModifier(ignitePercent);
// stat.iceDamage.RemoveModifier(iceDamage);
// stat.chillPercent.RemoveModifier(chillPercent);
// stat.lightingDamage.RemoveModifier(lightingDamage);
// stat.shockPercent.RemoveModifier(shockPercent);