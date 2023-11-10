
using UnityEngine;

public class PlayerDashState : PlayerState
{
    private float _dashStartTime;
    private float _dashDirection;
    public PlayerDashState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        _dashDirection = _player.PlayerInput.xInput != 0 ? _player.PlayerInput.xInput : _player.FacingDirection; 
        _dashStartTime = Time.time;
    }

    public override void UpdateState()
    {
        base.UpdateState();
        _player.SetVelocity(_player.dashSpeed * _dashDirection, 0);
        if (_dashStartTime + _player.dashDuration <= Time.time)
        {
            _stateMachine.ChangeState(StateEnum.Idle);
        }
    }

    public override void Exit()
    {
        _player.StopImmediately(false);
        base.Exit();
    }
}
