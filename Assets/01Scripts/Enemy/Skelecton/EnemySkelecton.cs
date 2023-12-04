using System;
using UnityEngine;

public enum SkelectonStateEnum
{
    Idle,
    Move,
    Battle,
    Attack,
    Stuned,
    Dead
}

public class EnemySkelecton : Enemy
{
    public EnemyStateMachine<SkelectonStateEnum> StateMachine { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        StateMachine = new EnemyStateMachine<SkelectonStateEnum>();
        
        foreach (SkelectonStateEnum state in Enum.GetValues(typeof(SkelectonStateEnum)) )
        {
            string typeName = state.ToString();
            Type t = Type.GetType($"Skelecton{typeName}State");
            //이부분은 나중에 정정 필요. 다 this면 걍 타입캐스트 하면 돼.
            if (t != null)
            {
                var enemyState = Activator.CreateInstance(t, this, StateMachine, typeName) as EnemyState<SkelectonStateEnum>;
                StateMachine.AddState(state, enemyState);
            }
            else
            {
                Debug.LogError($"Enemy skelecton : no state [ {typeName} ]");
            }

        }
    }

    protected override void Start()
    {
        base.Start();
        StateMachine.Initialize(SkelectonStateEnum.Idle);
        //체력에 구독. 만약 구독해제 안되면 이거 나중에 고쳐야함.
        // 이렇게 하면 안되는게 일단 카운터 어택이면 체력을 더 깔 수가 없음.
        //HealthCompo.OnHitEvent.AddListener(()=> CanBeStunned());
    }

    protected override void Update()
    {
        base.Update();
        
        if (_isFrozen) return;
        StateMachine.CurrentState.UpdateState();
    }

    protected override void HandleDie(Vector2 direction)
    {
        StateMachine.ChangeState(SkelectonStateEnum.Dead);
    }

    public override bool CanBeStunned()
    {
        if (base.CanBeStunned())
        {
            StateMachine.ChangeState(SkelectonStateEnum.Stuned);
            return true;
        }

        return false;
    }

    
    protected override void HandleHit()
    {
        base.HandleHit();
        if(!_isFrozenWithoutTimer)
            StateMachine.ChangeState(SkelectonStateEnum.Battle); //공격상태로 넘긴다.
    }

    public override void AnimationFinishTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();
}
