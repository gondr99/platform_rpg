using System;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Enemy : Entity
{

    
    [Header("셋팅값들")]
    public float moveSpeed;
    public float idleTime;
    public float battleTime; //전투시간을 초과하면 idle상태로 이동한다.
    
    
    [SerializeField] protected LayerMask _whatIsPlayer;
    [SerializeField] protected LayerMask _whatIsObstacle;

    [Header("공격상태설정값")]
    public float runAwayDistance;
    public float attackDistance;
    public float attackCooldown;
    [HideInInspector] public float lastTimeAttacked;
    
    protected override void Awake()
    {
        base.Awake();
        
    }

    protected override void Update()
    {
        base.Update();

    }

    //전방 50에 플레이어가 있는지 검사.
    public virtual RaycastHit2D IsPlayerDetected()
        => Physics2D.Raycast(_wallCheck.position, Vector2.right * FacingDirection, runAwayDistance, _whatIsPlayer);

    public virtual bool IsObstacleInLine(float distance)
    {
        return Physics2D.Raycast(_wallCheck.position, Vector2.right * FacingDirection,  distance, _whatIsObstacle);
    }

    public abstract void AnimationFinishTrigger();

    public virtual void OpenCounterAttackWindow()
    {
        _canBeStuned = true;
    }

    public virtual void CloseCounterAttackWindow()
    {
        _canBeStuned = false;
    }

    public virtual bool CanBeStunned()
    {
        if (_canBeStuned)
        {
            CloseCounterAttackWindow();
            return true;
        }

        return false;
    }
    
    #if UNITY_EDITOR
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + attackDistance * FacingDirection, transform.position.y));
        Gizmos.color = Color.white;
    }
    #endif
}
