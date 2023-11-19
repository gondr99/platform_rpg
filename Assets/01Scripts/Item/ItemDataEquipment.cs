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
    
    public void AddModifiers()
    {
        PlayerStat stat = GameManager.Instance.Player.Stat as PlayerStat;
        if (stat == null)
            return;
        
        stat.strength.AddModifier(strength);
        stat.agility.AddModifier(agility);
        stat.intelligence.AddModifier(intelligence);
        stat.vitality.AddModifier(vitality);
        
        stat.damage.AddModifier(damage);
        stat.criticalChance.AddModifier(criticalChance);
        stat.criticalDamage.AddModifier(criticalDamage);
        
        stat.maxHealth.AddModifier(maxHealth);
        stat.armor.AddModifier(armor);
        stat.evasion.AddModifier(evasion);
        stat.magicResistance.AddModifier(magicResistance);
        
        stat.fireDamage.AddModifier(fireDamage);
        stat.ignitePercent.AddModifier(ignitePercent);
        stat.iceDamage.AddModifier(iceDamage);
        stat.chillPercent.AddModifier(chillPercent);
        stat.lightingDamage.AddModifier(lightingDamage);
        stat.shockPercent.AddModifier(shockPercent);
    }

    public void RemoveModifiers()
    {
        PlayerStat stat = GameManager.Instance.Player.Stat as PlayerStat;
        if (stat == null)
            return;
        
        stat.strength.RemoveModifier(strength);
        stat.agility.RemoveModifier(agility);
        stat.intelligence.RemoveModifier(intelligence);
        stat.vitality.RemoveModifier(vitality);
        
        stat.damage.RemoveModifier(damage);
        stat.criticalChance.RemoveModifier(criticalChance);
        stat.criticalDamage.RemoveModifier(criticalDamage);
        
        stat.maxHealth.RemoveModifier(maxHealth);
        stat.armor.RemoveModifier(armor);
        stat.evasion.RemoveModifier(evasion);
        stat.magicResistance.RemoveModifier(magicResistance);
        
        stat.fireDamage.RemoveModifier(fireDamage);
        stat.ignitePercent.RemoveModifier(ignitePercent);
        stat.iceDamage.RemoveModifier(iceDamage);
        stat.chillPercent.RemoveModifier(chillPercent);
        stat.lightingDamage.RemoveModifier(lightingDamage);
        stat.shockPercent.RemoveModifier(shockPercent);
    }
}
