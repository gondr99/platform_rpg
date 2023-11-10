using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAirState : PlayerState
{
    public PlayerAirState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
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

        if (_player.IsWallDetected())
        {
            _stateMachine.ChangeState(StateEnum.WallSlide);
        }
        
        float xInput = _player.PlayerInput.xInput;
        if (xInput != 0)
        {
            _player.SetVelocity(_player.moveSpeed * 0.8f * xInput, _rigidbody.velocity.y);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
