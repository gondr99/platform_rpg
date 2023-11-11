using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkelectonStateEnum
{
    Idle,
    Move,
    Battle,
    Attack
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
            var enemyState = Activator.CreateInstance(t, this, StateMachine, typeName, this) as EnemyState<SkelectonStateEnum>;

            StateMachine.AddState(state, enemyState);
            
        }
    }

    protected override void Start()
    {
        base.Start();
        StateMachine.Initialize(SkelectonStateEnum.Idle);
    }

    protected override void Update()
    {
        base.Update();
        StateMachine.CurrentState.UpdateState();
    }
    
    public override void AnimationFinishTrigger() => StateMachine.CurrentState.AnimationFinishTrigger();
}
