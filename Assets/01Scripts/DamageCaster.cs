using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class DamageCaster : MonoBehaviour
{
    public Transform attackChecker;
    public float attackCheckRadius;

    public Vector2 knockbackPower;
    
    [SerializeField] private int _maxHitCount = 5; //최대로 때릴 수 있는 적 갯수
    public LayerMask whatIsEnemy;
    private Collider2D[] _hitResult;

    private Entity _owner;
    private void Awake()
    {
        _hitResult = new Collider2D[_maxHitCount];
    }

    public void SetOwner(Entity owner)
    {
        _owner = owner;
    }

    public bool CastDamage()
    {
        int cnt = Physics2D.OverlapCircleNonAlloc(attackChecker.position, attackCheckRadius, _hitResult, whatIsEnemy);

        //이거 쓰면 라이더가 경고 때리는데 위에껀 유니티에서 쓰지말라함..아잇...
        //Physics2D.OverlapCircleAll(attackChecker.position, attackCheckRadius, whatIsEnemy);
        
        for (int i = 0; i < cnt; ++i)
        {
            Vector2 direction = (_hitResult[i].transform.position - transform.position).normalized;
            if (_hitResult[i].TryGetComponent<IDamageable>(out IDamageable health))
            {
                health.ApplyDamage(_owner.Stat.GetDamage(), direction, knockbackPower, _owner);
            }
        }

        return cnt > 0;
    }

    private void OnDrawGizmos()
    {
        if(attackChecker != null)
            Gizmos.DrawWireSphere(attackChecker.position, attackCheckRadius);
    }
}
