using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        _player.StopImmediately(true);
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void UpdateState()
    {
        base.UpdateState();
        //만약 전방에 벽이 있다면 달리지 말고 멈추도록 하자
        float xInput = _player.PlayerInput.xInput;
        if ( Mathf.Abs( _player.FacingDirection  + xInput) > 1.2f && _player.IsWallDetected())
            return;
        
        if(xInput != 0 && !_player.IsBusy)
            _stateMachine.ChangeState(StateEnum.Move);
    }

}