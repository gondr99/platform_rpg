using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkelectonAttackState : EnemyState<SkelectonStateEnum>
{
    private EnemySkelecton _enemy;
    
    public SkelectonAttackState(Enemy enemyBase, EnemyStateMachine<SkelectonStateEnum> stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        _enemy = enemyBase as EnemySkelecton;
    }

    public override void UpdateState()
    {
        base.UpdateState();
        _enemy.StopImmediately(false);

        if (_triggerCalled) //애니메이션이 끝났다면
        {
            _stateMachine.ChangeState(SkelectonStateEnum.Battle); //추적상태로 다시 전환.
        }
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
        _enemy.lastTimeAttacked = Time.time; //마지막으로 공격한 시간을 기록함.
    }
}
