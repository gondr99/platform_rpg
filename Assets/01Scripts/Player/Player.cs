using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

public class Player: MonoBehaviour
{
    [Header("Setting values")] 
    public float moveSpeed = 12f;
    public float jumpForce = 12f;
    public float dashDuration = 0.4f;
    public float dashSpeed = 20f;
    [SerializeField] private float _dashCoolTime;
    private float _lastDashTime;

    public Vector2[] attackMovement;
    public float attackSpeed = 1f;
    
    [SerializeField] private Transform _groundChecker;
    [SerializeField] private float _groundCheckDistance;
    [SerializeField] private LayerMask _whatIsGround;
    [SerializeField] private Transform _wallCheck;
    [SerializeField] private float _wallCheckDistance;

    public int FacingDirection { get; private set; } = 1;
    private bool _facingRight = true;

    public bool IsBusy { get; private set; } = false;
    
    #region 컴포넌트
    public Animator AnimatorCompo { get; private set; }
    public Rigidbody2D RigidbodyCompo { get; private set; }
    public PlayerStateMachine StateMachine { get; private set; }

    [SerializeField] private InputReader _inputReader;
    public InputReader PlayerInput => _inputReader;
    #endregion
    
    private void Awake()
    {
        AnimatorCompo = transform.Find("Visual").GetComponent<Animator>();
        RigidbodyCompo = GetComponent<Rigidbody2D>();
        StateMachine = new PlayerStateMachine();
        
        foreach (StateEnum state in Enum.GetValues(typeof(StateEnum)) )
        {
            string typeName = state.ToString();
            Type t = Type.GetType($"Player{typeName}State");
            var playerState = Activator.CreateInstance(t, this, StateMachine, typeName) as PlayerState;

            StateMachine.AddState(state, playerState);
            
        }
    }
    private void Start()
    {
        StateMachine.Initialize(StateEnum.Idle);
    }

    private void OnEnable()
    {
        PlayerInput.DashEvent += HandleDashInput;
    }

    private void OnDisable()
    {
        PlayerInput.DashEvent += HandleDashInput;
    }

    private void HandleDashInput()
    {
        //벽에 붙어있는 동안은 대시가 안되도록
        if (IsWallDetected())
            return;
        
        if (_lastDashTime + _dashCoolTime <= Time.time)
        {
            StateMachine.ChangeState(StateEnum.Dash);
            _lastDashTime = Time.time;
        }
    }

    
    private void Update()
    {
        StateMachine.CurrentState.UpdateState();
    }
    
    
    //현재 상태에서 애니메이션이 종료되었음을 트리거 한다.
    public void AnimationTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();

    #region velocity controll

    public void SetVelocity(float x, float y)
    {
        RigidbodyCompo.velocity = new Vector2(x, y);
        FlipController(x);
    }

    public void StopImmediately(bool withYAxis)
    {
        if(withYAxis)
            RigidbodyCompo.velocity = Vector2.zero;
        else
            RigidbodyCompo.velocity = new Vector2(0, RigidbodyCompo.velocity.y);
    }

    #endregion

    #region Check Collision
    public bool IsGroundDetected() =>
        Physics2D.Raycast(_groundChecker.position, Vector2.down, _groundCheckDistance, _whatIsGround);

    public bool IsWallDetected() =>
        Physics2D.Raycast(_wallCheck.position, Vector2.right * FacingDirection, _wallCheckDistance, _whatIsGround);

    #endregion

    #region Flip controlling

    private void Flip()
    {
        FacingDirection = FacingDirection * -1;
        _facingRight = !_facingRight;
        transform.Rotate(0, 180, 0); //180도 회전. 
    }


    public void FlipController(float x)
    {
        bool gotoRight = x > 0 && !_facingRight;
        bool gotoLeft = x < 0 && _facingRight;
        if (gotoLeft || gotoRight)
        {
            Flip();
        }
    }
    
    #endregion
    
    //IsBusy 셋팅함수  (MS단위로 입력)
    public async void SetIsBusyWhenDelayTime(int delayTimeMS)
    {
        IsBusy = true;
        await Task.Delay(delayTimeMS);
        IsBusy = false;
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(_groundChecker.position, _groundChecker.position + new Vector3(0, -_groundCheckDistance, 0));
        Gizmos.DrawLine(_wallCheck.position, _wallCheck.position + new Vector3(_wallCheckDistance, 0, 0));
    }
#endif
}