using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class SkelectonIdleState : SkelectonGroundState
{
    private bool _isAlreadyChanged = false;
    public SkelectonIdleState(Enemy enemyBase, EnemyStateMachine<SkelectonStateEnum> stateMachine, string animBoolName) 
        : base(enemyBase, stateMachine, animBoolName)
    {
    }

    public override void UpdateState()
    {
        base.UpdateState();

        // if (_enemy.IsPlayerDetected())
        // {
        //     _stateMachine.ChangeState(SkelectonStateEnum.Battle);
        // }
    }

    public override void Enter()
    {
        base.Enter();
        _isAlreadyChanged = false; //이동 딜레이중에 피격당해서 이동시 차후 딜레이가 끝나고 중복 실행되는것을 막기 위해
        ChangeToMoveWithDelay(Mathf.FloorToInt( _enemy.idleTime * 1000)); //ms로 변경해서 보냄.
    }

    private async void ChangeToMoveWithDelay(int delayMS)
    {
        await Task.Delay(delayMS);
        if(!_isAlreadyChanged)
            _stateMachine.ChangeState(SkelectonStateEnum.Move);
    }
    public override void Exit()
    {
        _isAlreadyChanged = true;
        AudioManager.Instance.PlaySFX(24, _enemy.transform);
        base.Exit();
    }
}
