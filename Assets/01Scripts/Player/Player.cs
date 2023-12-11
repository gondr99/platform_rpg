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

    private float _defaultMoveSpeed;
    private float _defaultJumpForce;
    private float _defaultDashSpeed;

    [Header("Attack Settings")]
    public Vector2[] attackMovement;  //앞으로 전진하는 정도.
    public float attackSpeed = 1f;
    public float counterAttackDuration = 0.2f;
    public Transform backTrm; //후면에 따라다니는 오브젝트들을 위한
    
    public bool IsBusy { get; private set; } = false;

    #region Player components

    public PlayerFXPlayer FxPlayer { get; private set; }

    #endregion
    
    [HideInInspector] public SkillManager skill;
    
    public PlayerStateMachine StateMachine { get; private set; }
    [SerializeField] private InputReader _inputReader;
    public InputReader PlayerInput => _inputReader;

    //궁극기 썼을 때 하늘로 올라가는 시간 설정. 경우에 따라 다르게 설정됨.
    [HideInInspector] public float flyTimerOnUlti = 0.3f;
    
    //궁극기 쓰면 피격이나 
    public bool canStateChangeable = true;
    protected bool _isDead = false;
    
    //현재 콤보상태 저장
    [HideInInspector] public int currentComboCounter = 0;
    
    protected override void Awake()
    {
        base.Awake();
        StateMachine = new PlayerStateMachine();
        
        foreach (StateEnum state in Enum.GetValues(typeof(StateEnum)) )
        {
            string typeName = state.ToString();
            try
            {
                Type t = Type.GetType($"Player{typeName}State");
                var playerState = Activator.CreateInstance(t, this, StateMachine, typeName) as PlayerState;

                StateMachine.AddState(state, playerState);
            }
            catch (Exception ex)
            {
                Debug.LogError($"{typeName} is loading error, Message : {ex.Message}");
            }
        }

        //FX재생을 위한 플레이어.
        FxPlayer = transform.Find("PlayerFX").GetComponent<PlayerFXPlayer>();
    }
    
    protected override void Start()
    {
        base.Start();
        skill = SkillManager.Instance; //스킬매니저 캐싱
        StateMachine.Initialize(StateEnum.Idle, this);
        
        //기본 값 셋팅
        _defaultMoveSpeed = moveSpeed;
        _defaultJumpForce = jumpForce;
        _defaultDashSpeed = dashSpeed;
    }

    private void OnEnable()
    {
        PlayerInput.DashEvent += HandleDashInput;
        PlayerInput.CrystalSkillEvent += HandleCrystalInput;
        PlayerInput.HealFlaskEvent += HandleFlaskInput;
    }

    private void OnDisable()
    {
        PlayerInput.DashEvent -= HandleDashInput;
        PlayerInput.CrystalSkillEvent -= HandleCrystalInput;
        PlayerInput.HealFlaskEvent -= HandleFlaskInput;
    }

    protected override void HandleDie(Vector2 direction)
    {
        //사망처리
        HandleHit(); //체력바 초기화시켜주고
        _isDead = true;
        StateMachine.ChangeState(StateEnum.Dead);
        base.HandleKnockback(direction);
    }

    protected override void HandleKnockback(Vector2 direction)
    {
        if(!_isDead)
            base.HandleKnockback(direction);  //베이스 넉백은 실행할지 말지 사망에 따라 결정.
    }

    private void HandleDashInput()
    {
        //벽에 붙어있는 동안은 대시가 안되도록
        if (IsWallDetected())
            return;
        
        //대시 스킬 사용 성공시.
        if (skill.GetSkill<DashSkill>().AttemptUseSkill())
        {
            StateMachine.ChangeState(StateEnum.Dash);
        }
    }

    private void HandleCrystalInput()
    {
        CrystalSkill crystalSkill = skill.GetSkill<CrystalSkill>();
        crystalSkill.AttemptUseSkill(); //사용시도.
    }

    private void HandleFlaskInput()
    {
        Inventory.Instance.UseFlask(); //플라스크 사용.
    }

    //플레이어의 공격관련 코드들.
    public override void Attack()
    {
        //공격 사운드 재생
        AudioManager.Instance.PlaySFX(2, sourceTrm: null, withRandomPitch: true);
        
        bool hitAttack = false; //공격에 적이 맞았는가?
        if (DamageCasterCompo.CastDamage())
        {
            //공격성공시 현재 플레이어 콤보상태 계산.
            if (currentComboCounter == 2)
            {
                ThunderStrikeSkill thunder = skill.GetSkill<ThunderStrikeSkill>();
                thunder.AttemptUseSkill(); //3타 공격 성공시 시도.
            }

            hitAttack = true;
        }
        
        ItemDataEquipment equip = Inventory.Instance.GetEquipmentByType(EquipmentType.Weapon);
        if(equip != null)
            equip.ItemEffectByMelee(hitAttack);// 무기 이펙트 실행
    }

    protected override void Update()
    {
        base.Update();
        StateMachine.CurrentState.UpdateState();
    }

    public override void SlowEntityBy(float percent)
    {
        if (moveSpeed < _defaultMoveSpeed) return;
        moveSpeed *= 1 - percent;
        jumpForce *= 1 - percent;
        dashSpeed *= 1 - percent;
        AnimatorCompo.speed *= 1 - percent;
    }

    protected override void ReturnDefaultSpeed()
    {
        base.ReturnDefaultSpeed();
        moveSpeed = _defaultMoveSpeed;
        jumpForce = _defaultJumpForce;
        dashSpeed = _defaultDashSpeed;
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