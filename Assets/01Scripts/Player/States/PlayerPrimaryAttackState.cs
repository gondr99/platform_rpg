using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{
    private int _comboCounter;
    private float _lastTimeAttacked;
    private float _comboWindow = 2;
    private readonly int _comboCountHash = Animator.StringToHash("ComboCounter");
    public PlayerPrimaryAttackState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        //마지막 공격으로부터 _comboWindow만큼 시간이 흘렀다면 다시 0번 콤보부터 이어서.
        if (_comboCounter > 2 || Time.time >= _lastTimeAttacked + _comboWindow)
            _comboCounter = 0;
        
        _player.AnimatorCompo.SetInteger(_comboCountHash, _comboCounter);
        _player.currentComboCounter = _comboCounter;
        
        //애니메이션 속도를 조절
        _player.AnimatorCompo.speed = _player.attackSpeed;
        
        float attackDirection = _player.FacingDirection;
        if (_player.PlayerInput.xInput != 0)
        {
            attackDirection = _player.PlayerInput.xInput;
        }

        //약간 상승해주는 느낌을 주기 위해 y속도도 조절
        _player.SetVelocity(_player.attackMovement[_comboCounter].x * attackDirection, _player.attackMovement[_comboCounter].y);
        //이동중에 공격했을 때 0.1초정도의 딜레이를 주고 조금 움직이고 멈추도록 함.
        DelayStop();
    }

    private async void DelayStop()
    {
        await Task.Delay(100);
        _player.StopImmediately(false);
    }

    public override void UpdateState()
    {
        base.UpdateState();
        
        if(_triggerCalled)
            _stateMachine.ChangeState(StateEnum.Idle);
    }

    public override void Exit()
    {
        ++_comboCounter; 
        _lastTimeAttacked = Time.time;
        //애니메이션 속도 원상복귀.
        _player.AnimatorCompo.speed = 1;
        _player.SetIsBusyWhenDelayTime(100); //0.1초동안 비지 상태로 만들어준다.
        base.Exit();
    }
}
