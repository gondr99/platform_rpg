using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DamageCaster : MonoBehaviour
{
    public Transform attackChecker;
    public float attackCheckRadius;
    public int damage = 1;

    public Vector2 knockbackPower;
    
    [SerializeField] private int _maxHitCount = 5; //최대로 때릴 수 있는 적 갯수
    public LayerMask whatIsEnemy;
    private Collider2D[] _hitResult;

    private void Awake()
    {
        _hitResult = new Collider2D[_maxHitCount];
    }

    public void CastDamage()
    {
        int cnt = Physics2D.OverlapCircleNonAlloc(attackChecker.position, attackCheckRadius, _hitResult, whatIsEnemy);

        
        for (int i = 0; i < cnt; ++i)
        {
            Vector2 direction = (_hitResult[i].transform.position - transform.position).normalized;
            if (_hitResult[i].TryGetComponent<IDamageable>(out IDamageable health))
            {
                health.ApplyDamage(damage, direction, knockbackPower);
            }
        }
    }

    private void OnDrawGizmos()
    {
        if(attackChecker != null)
            Gizmos.DrawWireSphere(attackChecker.position, attackCheckRadius);
    }
}
