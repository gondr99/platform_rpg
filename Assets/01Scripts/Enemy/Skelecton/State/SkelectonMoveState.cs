using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkelectonMoveState : SkelectonGroundState
{
    public SkelectonMoveState(Enemy enemyBase, EnemyStateMachine<SkelectonStateEnum> stateMachine, string animBoolName) 
        : base(enemyBase, stateMachine, animBoolName)
    {
    }

    public override void UpdateState()
    {
        base.UpdateState();
        
        _enemy.SetVelocity(_enemy.moveSpeed * _enemy.FacingDirection, _rigidbody.velocity.y);

        if (_enemy.IsWallDetected() || !_enemy.IsGroundDetected())
        {
            _enemy.Flip();
            _stateMachine.ChangeState(SkelectonStateEnum.Idle);
        }
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
