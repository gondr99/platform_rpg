using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour
{
    [SerializeField] private int _maxHitCount; //한 공격으로 최대로 때릴 적 객체 수
    [SerializeField] private LayerMask _whatIsEnemy;
    private Player _player;

    private Collider2D[] _hitResult;
    
    private void Awake()
    {
        _player = transform.parent.GetComponent<Player>();
        _hitResult = new Collider2D[_maxHitCount];
    }

    private void AnimationTrigger()
    {
        _player.AnimationTrigger();
    }
    
    private void AttackTrigger()
    {
        _player.Attack();
    }

    //칼을 던지는 이벤트.
    private void ThrowSword()
    {
        SwordSkill skill = _player.skill.GetSkill<SwordSkill>(PlayerSkill.Sword);
        skill.CreateSword();
    }
}
