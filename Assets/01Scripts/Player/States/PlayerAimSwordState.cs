using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAimSwordState : PlayerState
{
    private bool _releaseKey;
    public PlayerAimSwordState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        _releaseKey = false;
        _player.PlayerInput.ThrowAimEvent += OnThrowAim;
        
        (_player.skill.GetSkill(PlayerSkill.Sword) as SwordSkill)?.DotsActive(true);
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (_releaseKey && _triggerCalled)
        {
            //키도 눌렸고 애니메이션도 종료되었다면 Idle상태로 전환
            _stateMachine.ChangeState(StateEnum.Idle);
        }
    }

    public override void Exit()
    {
        (_player.skill.GetSkill(PlayerSkill.Sword) as SwordSkill)?.DotsActive(false);
        _player.PlayerInput.ThrowAimEvent -= OnThrowAim;
        base.Exit();
    }

    private void OnThrowAim(bool state)
    {
        //키를 놓는것만 체크한다.
        if (!state)
        {
            _releaseKey = true;
            _player.AnimatorCompo.SetBool(_animBoolHash, false); //여기서 꺼줘야 던지는 애니메이션으로 감.
        }
    }
}
