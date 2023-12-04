using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerWallJumpState : PlayerState
{
    public PlayerWallJumpState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        _player.SetVelocity(5 * - _player.FacingDirection, _player.jumpForce);
        DelayToAir();
    }

    private async void DelayToAir()
    {
        await Task.Delay(400); //0.4초 대기후
        _stateMachine.ChangeState(StateEnum.Fall);
    }

    public override void UpdateState()
    {
        base.UpdateState();
    }

    public override void Exit()
    {
        base.Exit();
    }
}
