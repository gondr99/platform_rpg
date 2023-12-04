using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkelectonBattleState : EnemyState<SkelectonStateEnum>
{
    private EnemySkelecton _enemy;
    protected Player _player;
    private int _moveDirection;

    private float _timer;
    
    public SkelectonBattleState(Enemy enemyBase, EnemyStateMachine<SkelectonStateEnum> stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        _enemy = enemyBase as EnemySkelecton;
    }

    public override void UpdateState()
    {
        base.UpdateState();
        
        if (_player.HealthCompo.isDead)
        {
            _stateMachine.ChangeState(SkelectonStateEnum.Idle); 
        }
        
        if (_player.transform.position.x > _enemy.transform.position.x)
            _moveDirection = 1;
        else
            _moveDirection = -1;
        _enemy.SetVelocity(_enemy.moveSpeed * _moveDirection, _rigidbody.velocity.y);
        
        RaycastHit2D hit = _enemy.IsPlayerDetected();
        //레이캐스트힛구조체 안에 operator bool 이 재정의 되어 있음. hit.collider == null 체크함.
        
        if (hit && !_enemy.IsObstacleInLine(hit.distance))
        {
            _timer = _enemy.battleTime; //타이머 설정
            
            if (hit.distance < _enemy.attackDistance && CanAttack())
            {
                _stateMachine.ChangeState(SkelectonStateEnum.Attack);
                return;
            }
        }

        float distance = Vector2.Distance(_player.transform.position, _enemy.transform.position);
        
        
        //앞이 절벽이거나 적이 근거리라면.
        if (!_enemy.IsGroundDetected() || (distance <=  _enemy.attackDistance))
        {
            _enemy.StopImmediately(false);  //정지시에 재생할 애니메이션 상태를 하나 만들어야 해.
            return;
        }

        if (_timer >= 0 &&  distance < _enemy.runAwayDistance)
        {
            _timer -= Time.deltaTime;
        }
        else
        {
            _stateMachine.ChangeState(SkelectonStateEnum.Idle); // 전투시간을 초과했다면 idle로 이동.
        }

    }

    public override void Enter()
    {
        base.Enter();
        _player = GameManager.Instance.Player;
        SetDirectionToEnemy();
    }

    //적 위치를 바라보도록 코드
    private void SetDirectionToEnemy()
    {
        _enemy.FlipController(_player.transform.position.x - _enemy.transform.position.x);
    }

    public override void Exit()
    {
        base.Exit();
    }

    private bool CanAttack()
    {
        return Time.time >= _enemy.lastTimeAttacked + _enemy.attackCooldown;
    }
}
