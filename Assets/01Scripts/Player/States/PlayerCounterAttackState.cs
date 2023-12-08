using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCounterAttackState : PlayerState
{
    private readonly int _successCounterHash = Animator.StringToHash("SuccessCounter");

    private float _counterTimer;
    private Collider2D[] _hitResult;

    private bool _cloneCreated = false;
    
    public PlayerCounterAttackState(Player player, PlayerStateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
        _hitResult = new Collider2D[1]; //카운터는 한명만.
    }

    public override void Enter()
    {
        base.Enter();
        _cloneCreated = false;
        _player.StopImmediately(false);
        _counterTimer = _player.counterAttackDuration; //카운터어택 시간 초기화.
        _player.AnimatorCompo.SetBool(_successCounterHash, false);
    }

    public override void UpdateState()
    {
        base.UpdateState();
        _counterTimer -= Time.deltaTime;

        CheckCounter();
        
        if (_counterTimer < 0 || _triggerCalled)
        {
            _stateMachine.ChangeState(StateEnum.Idle);
        }
    }

    private void CheckCounter()
    {
        DamageCaster caster = _player.DamageCasterCompo;
        int cnt = Physics2D.OverlapCircleNonAlloc(caster.attackChecker.position, caster.attackCheckRadius, _hitResult, caster.whatIsEnemy);

        
        for (int i = 0; i < cnt; ++i)
        {
            if (_hitResult[i].TryGetComponent<Enemy>(out Enemy enemy))
            {
                if (enemy.CanBeStunned())
                {
                    _counterTimer = 5f; //일단 크게 넣어주면 애니메이터에 의해서 알아서 종료된다.
                    _player.AnimatorCompo.SetBool(_successCounterHash, true);
                    
                    //카메라 쉐이크
                    _player.FxPlayer.ShakeCamera(new Vector2(0, 0.5f));
                    
                    //카운터 성공시에 분신 생성.
                    if (_cloneCreated == false)
                    {
                        _cloneCreated = true;
                        _player.skill.GetSkill<CloneSkill>().CreateCloneOnCounterAttack(enemy.transform);
                    }
                }
            }
        }
    }

    public override void Exit()
    {
        base.Exit();
    }
}
