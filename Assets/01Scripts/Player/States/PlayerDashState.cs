
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

        //지나간 자리에 클론 생성.
        CloneSkill cloneSkill = _player.skill.GetSkill<CloneSkill>();
        cloneSkill.CreateCloneOnDashStart();
        
        //대시중 무적으로 만들어줌
        _player.HealthCompo.MakeInvincible(true);
        _player.FxPlayer.SetAfterImageMode(true);
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
        //지나간 자리에 클론 생성.
        CloneSkill cloneSkill = _player.skill.GetSkill<CloneSkill>();
        cloneSkill.CreateCloneOnDashOver();
        _player.StopImmediately(false);
        
        //대시중 무적으로 만들어줌
        _player.HealthCompo.MakeInvincible(false);
        _player.FxPlayer.SetAfterImageMode(false);
        base.Exit();
    }
}
