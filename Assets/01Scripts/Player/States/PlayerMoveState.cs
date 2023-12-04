
using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    
    public PlayerMoveState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        AudioManager.Instance.PlaySFX(14, null, true);
    }

    public override void Exit()
    {
        AudioManager.Instance.StopSFX(14);
        base.Exit();
    }

    public override void UpdateState()
    {
        base.UpdateState();
        float xInput = _player.PlayerInput.xInput;

        _player.SetVelocity(xInput * _player.moveSpeed, _rigidbody.velocity.y);
        if(Mathf.Abs(xInput) < 0.05f || _player.IsWallDetected())
            _stateMachine.ChangeState(StateEnum.Idle);
    }
    
}
