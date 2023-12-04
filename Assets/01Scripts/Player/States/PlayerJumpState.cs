
using UnityEngine;

public class PlayerJumpState : PlayerAirState
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

        if (_rigidbody.velocity.y < 0)
        {
            _stateMachine.ChangeState(StateEnum.Fall);
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
