using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallState : PlayerAirState
{
    public PlayerFallState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void UpdateState()
    {
        base.UpdateState();
        if (_player.IsGroundDetected())
        {
            _stateMachine.ChangeState(StateEnum.Idle);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
