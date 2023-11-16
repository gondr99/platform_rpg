using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class Player: Entity
{
    [Header("Setting values")] 
    public float moveSpeed = 12f;
    public float jumpForce = 12f;
    public float dashDuration = 0.4f;
    public float dashSpeed = 20f;

    [Header("Attack Settings")]
    public Vector2[] attackMovement;  //앞으로 전진하는 정도.
    public float attackSpeed = 1f;
    public float counterAttackDuration = 0.2f;
    public Transform backTrm; //후면에 따라다니는 오브젝트들을 위한
    
    public bool IsBusy { get; private set; } = false;

    public SkillManager skill;
    
    public PlayerStateMachine StateMachine { get; private set; }
    [SerializeField] private InputReader _inputReader;
    public InputReader PlayerInput => _inputReader;

    //궁극기 썼을 때 하늘로 올라가는 시간 설정. 경우에 따라 다르게 설정됨.
    [HideInInspector] public float flyTimerOnUlti = 0.3f;
    
    //궁극기 쓰면 피격이나 
    public bool canStateChangeable = true;
    
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
        skill = SkillManager.Instance; //스킬매니저 캐싱
        StateMachine.Initialize(StateEnum.Idle, this);
    }

    private void OnEnable()
    {
        PlayerInput.DashEvent += HandleDashInput;
        PlayerInput.CrystalSkillEvent += HandleCrystalInput;
    }

    private void OnDisable()
    {
        PlayerInput.DashEvent -= HandleDashInput;
        PlayerInput.CrystalSkillEvent -= HandleCrystalInput;
    }
    
    private void HandleDashInput()
    {
        //벽에 붙어있는 동안은 대시가 안되도록
        if (IsWallDetected())
            return;
        
        //대시 스킬 사용 성공시.
        if (skill.GetSkill<DashSkill>(PlayerSkill.Dash).AttemptUseSkill())
        {
            StateMachine.ChangeState(StateEnum.Dash);
        }
    }

    private void HandleCrystalInput()
    {
        CrystalSkill crystalSkill = skill.GetSkill<CrystalSkill>(PlayerSkill.Crystal);
        crystalSkill.AttemptUseSkill(); //사용시도.
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


    public void FadePlayer(bool fadeOut, float sec)
    {
        float endValue = fadeOut ? 0 : 1f;
        SpriteRendererCompo.DOFade(endValue, sec);
    }
    
}