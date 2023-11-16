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

    [SerializeField]private CharacterStat _characterStat;
    public CharacterStat Stat => _characterStat; 
    #endregion
    
    public int FacingDirection { get; private set; } = 1;
    protected bool _facingRight = true;
    
    protected virtual void Awake()
    {
        Transform visualTrm = transform.Find("Visual");
        AnimatorCompo = visualTrm.GetComponent<Animator>();
        RigidbodyCompo = GetComponent<Rigidbody2D>();
        HealthCompo = GetComponent<Health>();
        DamageCasterCompo = transform.Find("DamageCaster").GetComponent<DamageCaster>();
        SpriteRendererCompo = visualTrm.GetComponent<SpriteRenderer>();

        
        DamageCasterCompo.SetOwner(this); //자신의 스탯상 데미지를 넣어줌.

        HealthCompo.maxHealth = Stat.maxHP.GetValue(); //최대체력 Awake에서 설정하면 Start에서 현재 체력으로도 셋팅됨.
        HealthCompo.OnKnockBack += HandleKnockback;
        HealthCompo.OnHit += HandleHit;
        HealthCompo.OnDied += HandleDie;
    }

    private void OnDestroy()
    {
        HealthCompo.OnKnockBack -= HandleKnockback;
        HealthCompo.OnHit -= HandleHit;
        HealthCompo.OnDied -= HandleDie;
    }

    protected virtual void HandleHit()
    {
        //나중에 UI 갱신관련 로직이 여기 들어가야 한다.
    }

    protected virtual void HandleKnockback(Vector2 direction)
    {
        StartCoroutine(HitKnockback(direction));
    }

    protected abstract void HandleDie();


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
