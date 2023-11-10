
using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    
    public PlayerMoveState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void UpdateState()
    {
        base.UpdateState();
        float xInput = _player.PlayerInput.xInput;

        _player.SetVelocity(xInput * _player.moveSpeed, _rigidbody.velocity.y);
        if(xInput == 0 || _player.IsWallDetected())
            _stateMachine.ChangeState(StateEnum.Idle);
    }
    
    
    
}
