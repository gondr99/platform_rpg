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

    //이건 공격 콤포넌트를 따로 만들어야 해. SOLID에 어긋나.
    private void AttackTrigger()
    {
        _player.Attack();
    }
}
