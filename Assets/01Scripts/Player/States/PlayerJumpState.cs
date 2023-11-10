
using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _player.jumpForce);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        float xInput = _player.PlayerInput.xInput;
        if (xInput != 0)
        {
            _player.SetVelocity(_player.moveSpeed * 0.8f * xInput, _rigidbody.velocity.y);
        }
        
        if (_rigidbody.velocity.y < 0)
        {
            _stateMachine.ChangeState(StateEnum.Air);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
