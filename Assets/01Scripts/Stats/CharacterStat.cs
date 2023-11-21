using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;

public enum StatType
{
    strength,
    agility,
    intelligence,
    vitality,
    maxHealth,
    armor,
    evasion,
    magicResistance,
    damage,
    criticalChance,
    criticalDamage,
    fireDamage,
    ignitePercent,
    iceDamage,
    chillPercent,
    lightingDamage,
    shockPercent
}

public abstract class CharacterStat : ScriptableObject
{
    [Header("Major stat")]
    public Stat strength; // 1포인트당 데미지 증가, 크증뎀 1%
    public Stat agility; // 1포인트당 회피 1%, 크리티컬 1%
    public Stat intelligence; // 1포인트당 마법데미지 1증가, 마법저항 3증가, 도트 데미지에 지능의 10% 증뎀(지능10당 도트뎀 10퍼 증가)
    public Stat vitality; // 1포인트당 체력 5증가.
    
    
    [Header("Defensive stats")]
    public Stat maxHealth; //체력
    public Stat armor; //방어도
    public Stat evasion; //회피도
    public Stat magicResistance; //마법방어
    
    [Header("Offensive stats")]
    public Stat damage;
    public Stat criticalChance;
    public Stat criticalDamage;


    [Header("Magic stats")] 
    public Stat fireDamage;
    public Stat ignitePercent;
    public Stat iceDamage;
    public Stat chillPercent;
    public Stat lightingDamage;
    public Stat shockPercent;

    public Stat ailmentTimeMS; //밀리세컨드 단위의 질병 확률

    public bool canIgniteByMelee;
    public bool canChillByMelee;
    public bool canShockByMelee;
    
    //평타로 거는 상태이상은 데미지 캐스터에서,
    //일반 스킬들은 전부 Skill에서 진행.
    protected Entity _owner;

    protected Dictionary<StatType, FieldInfo> _filedInfoDictionary = new Dictionary<StatType, FieldInfo>();

    public virtual void SetOwner(Entity owner)
    {
        _owner = owner;
    }
    public virtual void IncreaseStatBy(int modifyValue, float duration, Stat statToModify)
    {
        _owner.StartCoroutine(StatModifyCoroutine(modifyValue, duration, statToModify));
    }
    

    //얘는 Task로 하면 게임 정지시에도 끝나버림.
    private IEnumerator StatModifyCoroutine(int modifyValue, float duration, Stat statToModify)
    {
        statToModify.AddModifier(modifyValue);
        yield return new WaitForSeconds(duration);
        statToModify.RemoveModifier(modifyValue);
    }
    
    protected virtual void OnEnable()
    {
        criticalDamage.SetDefaultValue(150); //처음 시작시 150% 증뎀으로 설정.
        
    }

    public int GetDamage()
    {
        return damage.GetValue() + strength.GetValue();
    }

    public bool CanEvasion()
    {
        int total = evasion.GetValue() + agility.GetValue();
        if (Random.Range(0, 100) < total)
        {
            return true; //회피 성공. 차후 메시지 띄워주기.
        }
        return false;
    }

    public int ArmoredDamage(int incomingDamage, bool isChilled)
    {
        float multiplier = isChilled ? 0.8f : 1f; //동상일때는 20% 아머 피어싱.
        return Mathf.Max(1, Mathf.RoundToInt(incomingDamage - armor.GetValue() * multiplier) );
    }

    public bool IsCritical(ref int incomingDamage)
    {
        int totalCritical = criticalChance.GetValue() + agility.GetValue();

        if (Random.Range(0, 100) <= totalCritical)
        {
            //크리티컬 증뎀 시키고.
            incomingDamage = CalculateCriticalDamage(incomingDamage);
            return true;
        }
        return false;
    }

    protected int CalculateCriticalDamage(int incomingDamage)
    {
        int percent = criticalDamage.GetValue() + strength.GetValue();
        return Mathf.RoundToInt(incomingDamage * percent * 0.01f); //0.01f 곱하면 백분율이다.
    }

    public virtual int GetMagicDamage()
    {
        int fire = fireDamage.GetValue();
        int ice = iceDamage.GetValue();
        int lighting = lightingDamage.GetValue();
        
        return fire + ice + lighting + intelligence.GetValue();
    }

    public int GetMaxHealthValue()
    {
        return maxHealth.GetValue() + vitality.GetValue() * 5;
    }

    public virtual int GetMagicDamageAfterResist(int incomingDamage)
    {
        int resistTotal = magicResistance.GetValue() + intelligence.GetValue() * 3;
        
        return Mathf.Max(1, incomingDamage - resistTotal); 
    }

    public virtual int GetDotDamage(Ailment ailment)
    {
        switch (ailment)
        {
            case Ailment.Ignited:
                float dotAmplifier = 0.1f + intelligence.GetValue() * 0.01f;
                return Mathf.Max(1, Mathf.RoundToInt(fireDamage.GetValue() * dotAmplifier )); //화염데미지의 10% 
            case Ailment.Chilled:
                return 0;
            case Ailment.Shocked:
                return 0;
            default:
                return 0;
        }
    }
    

    //상태이상 가능성.
    public bool CanAilment(Ailment ailment)
    {
        switch (ailment)
        {
            case Ailment.Ignited:
                return Random.Range(0, 100) < ignitePercent.GetValue(); 
            case Ailment.Chilled:
                return Random.Range(0, 100) < chillPercent.GetValue();
            case Ailment.Shocked:
                return Random.Range(0, 100) < shockPercent.GetValue();
            default:
                return false;
        }
    }
}
