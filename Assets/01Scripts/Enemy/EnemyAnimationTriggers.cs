using UnityEngine;

public class EnemyAnimationTriggers : MonoBehaviour
{
    [SerializeField] private LayerMask _whatIsEnemy;
    private Enemy _enemy;

    private Collider2D[] _hitResult = new Collider2D[1];
    private void Awake() 
    {
        _enemy = transform.parent.GetComponent<Enemy>();
    }

    private void AnimationTrigger()
    {
        _enemy.AnimationFinishTrigger();
    }
    
    private void AttackTrigger()
    {
        _enemy.Attack();
    }

    private void CounterAttackTrigger() => _enemy.OpenCounterAttackWindow();
    private void CounterAttackEndTrigger() => _enemy.CloseCounterAttackWindow();
}
