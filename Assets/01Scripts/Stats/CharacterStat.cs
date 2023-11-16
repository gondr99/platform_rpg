
using System;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public abstract class CharacterStat : ScriptableObject
{
    [Header("Major stat")]
    public Stat strength; // 1포인트당 데미지 증가, 크증뎀 1%
    public Stat agility; // 1포인트당 회피 1%, 크리티컬 1%
    public Stat intelligence; // 1포인트당 마법데미지 1증가, 마법저항 3증가
    public Stat vitality; // 1포인트당 체력 5증가.
    
    
    [Header("Defensive stats")]
    public Stat maxHP; //체력
    public Stat armor; //방어도
    public Stat evasion; //회피도
    
    
    [Header("Offensive stats")]
    public Stat damage;
    public Stat criticalChance;
    public Stat criticalDamage;


    private void OnEnable()
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

    public int ArmoredDamage(int incomingDamage)
    {
        return Mathf.Max(1, incomingDamage - armor.GetValue());
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
}
