using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkelectonGroundState : EnemyState<SkelectonStateEnum>
{
    protected EnemySkelecton _enemy;
    protected Player _player;
    public SkelectonGroundState(Enemy enemyBase, EnemyStateMachine<SkelectonStateEnum> stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        _enemy = enemyBase as EnemySkelecton;
    }

    public override void UpdateState()
    {
        base.UpdateState();

        RaycastHit2D hit = _enemy.IsPlayerDetected();
        //플레이와의 거리 : 뒤쪽에서 접근해도 인지할 수 있도록

        if (_player.HealthCompo.isDead) return; //죽었으면 걍 이동.
        float distance = Vector2.Distance(_enemy.transform.position, _player.transform.position);
        if (distance < 2 || (hit && !_enemyBase.IsObstacleInLine(hit.distance)))
        {
            _stateMachine.ChangeState(SkelectonStateEnum.Battle);
            return;
        }

        //if ((hit|| distance < 2) && !_enemy.IsObstacleInLine(hit.distance))
        //{
        //    _stateMachine.ChangeState(SkelectonStateEnum.Battle);
        //}
        
    }

    public override void Enter()
    {
        base.Enter();
        _player = GameManager.Instance.Player;
    }

    public override void Exit()
    {
        base.Exit();
    }
}
