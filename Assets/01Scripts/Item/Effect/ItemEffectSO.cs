using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemEffectSO : ScriptableObject
{
    [Range(0, 100f)]
    public float effectChance;
    public bool usedByMelee;
    public bool usedBySkill;
    public List<PlayerSkill> activeSkillTypeList;
    
    public bool activeByHit; //피격 또는 공격성공시만 발동
    public float effectCooldown;
    protected float _lastMeleeEffectTime;
    protected float _lastSkillEffectTime;
    protected float _lastHitEffectTime;

    protected virtual void OnEnable()
    {
        _lastMeleeEffectTime = -3000f; //시작할때 바로 수행되도록
        _lastSkillEffectTime = -3000f;
        _lastHitEffectTime = -3000f;
    }

    //이펙트의 쿨타임 없이 구동하기 위한 매서드
    public abstract void UseEffect();
    
    public virtual bool ExecuteEffectByMelee(bool hitAttack)
    {
        if (!usedByMelee) return false;
        if (_lastMeleeEffectTime + effectCooldown > Time.time) return false; 
        
        //체크 만들어!!!!
        return true;
    }

    public virtual bool ExecuteEffectBySkill(PlayerSkill skillType)
    {
        if (!usedBySkill) return false;
        if (_lastSkillEffectTime + effectCooldown > Time.time) return false;
        return true;
    }

    //피격시 발동 이펙트
    public virtual bool ExecuteEffectByHit(Health health)
    {
        if (_lastHitEffectTime + effectCooldown > Time.time) return false;
        return true;
    }
}
