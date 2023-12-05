using System.Threading.Tasks;
using Cinemachine;
using UnityEngine;

public class SkelectonDeadState : EnemyState<SkelectonStateEnum>
{
    private EnemySkelecton _enemy;
    public SkelectonDeadState(Enemy enemyBase, EnemyStateMachine<SkelectonStateEnum> stateMachine, string animBoolName) : base(enemyBase, stateMachine, animBoolName)
    {
        _enemy = enemyBase as EnemySkelecton;
    }

    public override void Enter()
    {
        base.Enter();
        //이건 나중에 좀 제대로 만들어보자.
        _enemy.AnimatorCompo.SetBool(_enemy.GetLastAnimHash(), true); //마지막 재생 애니메이션을 실행하고.
        _enemy.AnimatorCompo.speed = 0;
        _enemy.Collider.enabled = false;

        _enemy.StartDelayCallback(2f, () =>
        {
            GameObject.Destroy(_enemy.gameObject);
        });

    }

    public override void UpdateState()
    {
        base.UpdateState();
    }

}
