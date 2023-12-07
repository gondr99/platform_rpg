using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    private const float _groundFlyTime = 0.3f;
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
        
        //이건 나중에 공중에서도 쓸 수 있게 해줘야 할듯.
        _player.PlayerInput.UltiSkillEvent += OnUltiSkill;
    }
    
    public override void UpdateState()
    {
        base.UpdateState();
        if (!_player.IsGroundDetected())
        {
            _stateMachine.ChangeState(StateEnum.Fall);
        }
    }

    public override void Exit()
    {
        _player.PlayerInput.JumpEvent -= OnHandleJump;
        _player.PlayerInput.AttackEvent -= OnHandleAttack;
        _player.PlayerInput.CounterAttackEvent -= OnCounterAttack;
        _player.PlayerInput.ThrowAimEvent -= OnThrowAim;
        _player.PlayerInput.UltiSkillEvent -= OnUltiSkill;
        base.Exit();
    }

    private void OnUltiSkill()
    {
        _player.flyTimerOnUlti = _groundFlyTime;
        if (_player.skill.GetSkill<BlackholeSkill>().AttemptUseSkill())
        {
            _stateMachine.ChangeState(StateEnum.Blackhole);
        }
    }

    private void OnThrowAim(bool state)
    {
        //이미 칼을 던진상태면 더이상 진행안함.
        SwordSkill swordSkill = _player.skill.GetSkill<SwordSkill>();
        if (swordSkill == null || swordSkill.skillEnalbed == false) 
            return;
        
        bool hasSwordAlready = swordSkill.generatedSword != null;
        if (state && !hasSwordAlready)
        {
            _stateMachine.ChangeState(StateEnum.AimSword);
        }
        else if (state)
        {
            swordSkill.ReturnGenerateSword();
        }
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
