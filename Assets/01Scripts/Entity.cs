using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public abstract class Entity : MonoBehaviour
{
    [Header("Collision info")]
    [SerializeField] protected Transform _groundChecker;
    [SerializeField] protected float _groundCheckDistance;
    [SerializeField] protected LayerMask _whatIsGround;
    [SerializeField] protected Transform _wallCheck;
    [SerializeField] protected float _wallCheckDistance;

    [Header("Knockback info")]
    [SerializeField] protected float _knockbackDuration;
    protected bool _isKnocked;
    
    [Header("Stun Info")] 
    public float stunDuration;
    public Vector2 stunDirection;
    protected bool _canBeStuned;
    
    #region 컴포넌트
    public Animator AnimatorCompo { get; private set; }
    public Rigidbody2D RigidbodyCompo { get; private set; }
    public Health HealthCompo { get; private set; }
    public DamageCaster DamageCasterCompo { get; private set; }
    public SpriteRenderer SpriteRendererCompo { get; private set; }
    public CapsuleCollider2D Collider { get; private set; }

    [SerializeField]protected CharacterStat _characterStat;
    public CharacterStat Stat => _characterStat; 
    #endregion
    
    public int FacingDirection { get; private set; } = 1;
    protected bool _facingRight = true;
    public UnityEvent OnFliped;
    public UnityEvent<float> OnHealthBarChanged;
    
    protected virtual void Awake()
    {
        Transform visualTrm = transform.Find("Visual");
        AnimatorCompo = visualTrm.GetComponent<Animator>();
        RigidbodyCompo = GetComponent<Rigidbody2D>();
        HealthCompo = GetComponent<Health>();
        DamageCasterCompo = transform.Find("DamageCaster").GetComponent<DamageCaster>();
        SpriteRendererCompo = visualTrm.GetComponent<SpriteRenderer>();
        Collider = GetComponent<CapsuleCollider2D>();
        
        DamageCasterCompo.SetOwner(this, castByCloneSkill:false); //자신의 스탯상 데미지를 넣어줌.
        HealthCompo.SetOwner(this);
        
        HealthCompo.OnKnockBack += HandleKnockback;
        HealthCompo.OnHit += HandleHit;
        HealthCompo.OnDeathEvent.AddListener(HandleDie);
        HealthCompo.OnAilmentChanged.AddListener(HandleAilmentChanged);
        OnHealthBarChanged?.Invoke(HealthCompo.GetNormalizedHealth()); //최대치로 UI변경.
        
        _characterStat = Instantiate(_characterStat); //복제본으로 탑재.
        _characterStat.SetOwner(this); //자기를 오너로 설정
    }

    private void OnDestroy()
    {
        HealthCompo.OnKnockBack -= HandleKnockback;
        HealthCompo.OnHit -= HandleHit;
        HealthCompo.OnDeathEvent.RemoveListener(HandleDie);
        HealthCompo.OnAilmentChanged.RemoveListener( HandleAilmentChanged);
    }

    //동결에 따른 처리.
    private void HandleAilmentChanged(Ailment ailment)
    {
        if ((ailment & Ailment.Chilled) > 0) //동결상태면 스피드 느리게
        {
            //마법 저항에 따라 적게
            float resistance = (100 - Stat.magicResistance.GetValue()) * 0.01f;
            SlowEntityBy(0.5f *  resistance );
        }
        else
        {
            ReturnDefaultSpeed();
        }
    }

    protected virtual void HandleHit()
    {
        //UI갱신
        OnHealthBarChanged?.Invoke(HealthCompo.GetNormalizedHealth());
    }

    protected virtual void HandleKnockback(Vector2 direction)
    {
        StartCoroutine(HitKnockback(direction));
    }

    protected abstract void HandleDie(Vector2 direction);


    protected virtual void Start()
    {
        
    }
    
    protected virtual void Update()
    {
        
    }

    public virtual void Attack()
    {
        DamageCasterCompo?.CastDamage();
    }
    
    protected virtual IEnumerator HitKnockback(Vector2 direction)
    {
        _isKnocked = true;
        RigidbodyCompo.velocity = direction;
        yield return new WaitForSeconds(_knockbackDuration);
        _isKnocked = false;
    }

    public abstract void SlowEntityBy(float percent); //슬로우는 자식들이 구현.

    protected virtual void ReturnDefaultSpeed()
    {
        AnimatorCompo.speed = 1; //원래 스피드로 되돌리기.
    }

    #region Delay Callback Coroutine

    public Coroutine StartDelayCallback(float delayTime, Action Callback)
    {
        return StartCoroutine(DelayCoroutine(delayTime, Callback));
    }

    protected IEnumerator DelayCoroutine(float delayTime, Action Callback)
    {
        yield return new WaitForSeconds(delayTime);
        Callback?.Invoke();
    }

    #endregion
    
    #region Check Collision
    public virtual bool IsGroundDetected() =>
        Physics2D.Raycast(_groundChecker.position, Vector2.down, _groundCheckDistance, _whatIsGround);

    public virtual bool IsWallDetected() =>
        Physics2D.Raycast(_wallCheck.position, Vector2.right * FacingDirection, _wallCheckDistance, _whatIsGround);

    #endregion

    #region Flip controlling

    public virtual void Flip()
    {
        FacingDirection = FacingDirection * -1;
        _facingRight = !_facingRight;
        transform.Rotate(0, 180, 0); //180도 회전. 
        OnFliped?.Invoke();
    }


    public virtual void FlipController(float x)
    {
        bool gotoRight = x > 0 && !_facingRight;
        bool gotoLeft = x < 0 && _facingRight;
        if (gotoLeft || gotoRight)
        {
            Flip();
        }
    }
    
    #endregion

    #region velocity control

    public void SetVelocity(float x, float y, bool doNotFlip = false)
    {
        if (_isKnocked) return; //나중에 추가함. 
        
        RigidbodyCompo.velocity = new Vector2(x, y);
        if(!doNotFlip)
            FlipController(x);
    }

    public void StopImmediately(bool withYAxis)
    {
        if (_isKnocked) return; //나중에 추가함.
        
        if(withYAxis)
            RigidbodyCompo.velocity = Vector2.zero;
        else
            RigidbodyCompo.velocity = new Vector2(0, RigidbodyCompo.velocity.y);
    }

    #endregion

#if UNITY_EDITOR
    protected virtual void OnDrawGizmos()
    {
        if(_groundChecker != null)
            Gizmos.DrawLine(_groundChecker.position, _groundChecker.position + new Vector3(0, -_groundCheckDistance, 0));
        if(_wallCheck != null)
            Gizmos.DrawLine(_wallCheck.position, _wallCheck.position + new Vector3(_wallCheckDistance, 0, 0));
    }
#endif
}
