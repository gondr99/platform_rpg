
using UnityEngine;

public abstract class PlayerAirState : PlayerState
{
    protected PlayerAirState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void UpdateState()
    {
        base.UpdateState();
        float xInput = _player.PlayerInput.xInput;
        if (Mathf.Abs( xInput) > 0.05f)
        {
            _player.SetVelocity(_player.moveSpeed * 0.8f * xInput, _rigidbody.velocity.y);
        }
        
        if (_player.IsWallDetected())
        {
            _stateMachine.ChangeState(StateEnum.WallSlide);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
