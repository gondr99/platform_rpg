using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Skill : MonoBehaviour
{
    [SerializeField] protected float _cooldown;
    protected float _cooldownTimer;
    protected Player _player;

    protected virtual void Start()
    {
        _player = GameManager.Instance.Player;
    }

    protected virtual void Update()
    {
        _cooldownTimer -= Time.deltaTime;
    }

    public virtual bool AttemptUseSkill()
    {
        if (_cooldownTimer < 0)
        {
            UseSkill(); //스킬을 사용하고
            _cooldownTimer = _cooldown;
            return true;
        }
        Debug.Log("Skill cooldown");
        return false;
    }

    public virtual void UseSkill()
    {
        
    }
}
