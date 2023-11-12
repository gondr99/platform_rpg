using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

public class Player: Entity
{
    [Header("Setting values")] 
    public float moveSpeed = 12f;
    public float jumpForce = 12f;
    public float dashDuration = 0.4f;
    public float dashSpeed = 20f;
    [SerializeField] private float _dashCoolTime;
    private float _lastDashTime;

    [Header("Attack Settings")]
    public Vector2[] attackMovement;  //앞으로 전진하는 정도.
    public float attackSpeed = 1f;
    public float counterAttackDuration = 0.2f;


    public bool IsBusy { get; private set; } = false;
    
    
    public PlayerStateMachine StateMachine { get; private set; }
    [SerializeField] private InputReader _inputReader;
    public InputReader PlayerInput => _inputReader;
    
    protected override void Awake()
    {
        base.Awake();
        StateMachine = new PlayerStateMachine();
        
        foreach (StateEnum state in Enum.GetValues(typeof(StateEnum)) )
        {
            string typeName = state.ToString();
            Type t = Type.GetType($"Player{typeName}State");
            var playerState = Activator.CreateInstance(t, this, StateMachine, typeName) as PlayerState;

            StateMachine.AddState(state, playerState);
            
        }
    }
    protected override void Start()
    {
        base.Start();
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

    
    protected override void Update()
    {
        base.Update();
        StateMachine.CurrentState.UpdateState();
    }
    
    
    //현재 상태에서 애니메이션이 종료되었음을 트리거 한다.
    public void AnimationTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();
    

    
    //IsBusy 셋팅함수  (MS단위로 입력)
    public async void SetIsBusyWhenDelayTime(int delayTimeMS)
    {
        IsBusy = true;
        await Task.Delay(delayTimeMS);
        IsBusy = false;
    }

}