using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkelectonGroundState : EnemyState<SkelectonStateEnum>
{
    protected EnemySkelecton _enemy;
    protected Transform _playerTrm;
    
    public SkelectonGroundState(Enemy enemyBase, EnemyStateMachine<SkelectonStateEnum> stateMachine, string animBoolName, EnemySkelecton enemy) : base(enemyBase, stateMachine, animBoolName)
    {
        _enemy = enemy;
    }

    public override void UpdateState()
    {
        base.UpdateState();

        RaycastHit2D hit = _enemy.IsPlayerDetected();
        //플레이와의 거리 : 뒤쪽에서 접근해도 인지할 수 있도록
        float distance = Vector2.Distance(_enemy.transform.position, _playerTrm.position); 
        if ((hit|| distance < 2) && !_enemy.IsObstacleInLine(hit.distance))
        {
            _stateMachine.ChangeState(SkelectonStateEnum.Battle);
        }
        
    }

    public override void Enter()
    {
        base.Enter();
        _playerTrm = GameManager.Instance.PlayerTrm;
    }

    public override void Exit()
    {
        base.Exit();
    }
}
