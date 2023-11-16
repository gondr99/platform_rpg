using System;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Enemy : Entity
{

    
    [Header("셋팅값들")]
    public float moveSpeed;
    public float idleTime;
    public float battleTime; //전투시간을 초과하면 idle상태로 이동한다.

    private float _defaultMoveSpeed;
    
    [SerializeField] protected LayerMask _whatIsPlayer;
    [SerializeField] protected LayerMask _whatIsObstacle;

    [Header("공격상태설정값")]
    public float runAwayDistance;
    public float attackDistance;
    public float attackCooldown;
    [HideInInspector] public float lastTimeAttacked;

    protected bool _isFrozen = false; //얼어있는 상태
    protected bool _isFrozenWithoutTimer = false; //시간제한 없이 프리즈 시킬때

    protected int _lastAnimationBoolHash; //마지막으로 재생된 애니메이션 해시
    
    protected override void Awake()
    {
        base.Awake();
        _defaultMoveSpeed = moveSpeed;
    }

    protected override void Update()
    {
        base.Update();
        
    }

    public virtual void AssignLastAnimHash(int hashCode)
    {
        _lastAnimationBoolHash = hashCode;
    }
    
    //전방 50에 플레이어가 있는지 검사.
    public virtual RaycastHit2D IsPlayerDetected()
        => Physics2D.Raycast(_wallCheck.position, Vector2.right * FacingDirection, runAwayDistance, _whatIsPlayer);

    public virtual bool IsObstacleInLine(float distance)
    {
        return Physics2D.Raycast(_wallCheck.position, Vector2.right * FacingDirection,  distance, _whatIsObstacle);
    }

    public abstract void AnimationFinishTrigger();

    //만약 타임 프리징에 걸렸다면.
    public virtual void FreezeTime(bool isFreeze, bool isFrozenWithoutTimer = false)
    {
        if (isFrozenWithoutTimer)
        {
            _isFrozenWithoutTimer = isFrozenWithoutTimer; //시간제한없이 얼릴때가 true일때만. 
        }
        
        _isFrozen = isFreeze;
        if (isFreeze)
        {
            moveSpeed = 0;
            AnimatorCompo.speed = 0; //애니메이션 정지. 이동 정지.
        }
        else
        {
            moveSpeed = _defaultMoveSpeed;
            AnimatorCompo.speed = 1;
            _isFrozenWithoutTimer = false;
        }
    }

    public virtual async void FreezeTimerFor(float delaySec)
    {
        FreezeTime(true); //정지
        await Task.Delay(Mathf.FloorToInt(delaySec * 1000));

        if (!_isFrozenWithoutTimer)
        {
            FreezeTime(false); //재생
        }//영구 결빙 상태일때는 타이머가 풀지 못한다.
        
    }
    
    #region counter attack region
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
    #endregion
    
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
