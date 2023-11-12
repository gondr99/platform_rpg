using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    public PlayerGroundedState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        
    }

    public override void Enter()
    {
        base.Enter();
        _player.PlayerInput.JumpEvent += OnHandleJump;
        _player.PlayerInput.AttackEvent += OnHandleAttack;
        _player.PlayerInput.CounterAttackEvent += OnCounterAttack;
        _player.PlayerInput.ThrowAimEvent += OnThrowAim;
    }
    
    public override void UpdateState()
    {
        base.UpdateState();
        if (!_player.IsGroundDetected())
        {
            _stateMachine.ChangeState(StateEnum.Air);
        }
    }

    public override void Exit()
    {
        _player.PlayerInput.JumpEvent -= OnHandleJump;
        _player.PlayerInput.AttackEvent -= OnHandleAttack;
        _player.PlayerInput.CounterAttackEvent -= OnCounterAttack;
        _player.PlayerInput.ThrowAimEvent -= OnThrowAim;
        base.Exit();
    }

    private void OnThrowAim(bool state)
    {
        if(state)
            _stateMachine.ChangeState(StateEnum.AimSword);
    }

    private void OnHandleAttack()
    {
        _stateMachine.ChangeState(StateEnum.PrimaryAttack);        
    }

    private void OnHandleJump()
    {
        //땅위에 있을 때만 점프를 한다.
        if (_player.IsGroundDetected())
        {
            _stateMachine.ChangeState(StateEnum.Jump);
        }
    }
    
    private void OnCounterAttack()
    {
        _stateMachine.ChangeState(StateEnum.CounterAttack);        
    }

}
