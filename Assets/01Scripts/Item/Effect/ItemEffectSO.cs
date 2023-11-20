using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemEffectSO : ScriptableObject
{
    [Range(0, 100f)]
    public float effectChance;

    public bool activeByHit; //피격 또는 공격성공시만 발동
    public abstract void ExecuteEffect(bool hitAttack);
}
