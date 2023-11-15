using System;
using DG.Tweening;
using UnityEngine;

public class CrystalController : MonoBehaviour
{
    private float _crystalExistTimer;
    private CrystalSkill _skill;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    private bool _isDestroyed = false;
    private Transform _closestTarget = null;
    
    private readonly int _hashExplodeTrigger = Animator.StringToHash("Explode");
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    public void SetupCrystal(CrystalSkill skill, float timer, LayerMask whatIsEnemy)
    {
        _skill = skill;
        _crystalExistTimer = timer;
        _closestTarget = _skill.FindClosestEnemy(transform, whatIsEnemy, _skill.findEnemyRadius); //가장 가까운 적을 찾는다.
    }


    private void Update()
    {
        _crystalExistTimer -= Time.deltaTime;
        if (_crystalExistTimer <= 0 && !_isDestroyed)
        {
            EndOfCrystal();
            return;
        }

        if (_skill.canMoveToEnemy && _closestTarget != null && !_isDestroyed)
        {
            transform.position = Vector2.MoveTowards(transform.position, _closestTarget.position,
                _skill.moveSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, _closestTarget.position) < 1f)
            {
                EndOfCrystal();
            }
        }
    }

    private void AnimationExplodeEvent()
    {
        LayerMask whatIsEnemy = GameManager.Instance.Player.DamageCasterCompo.whatIsEnemy;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, _skill.explosionRadius, whatIsEnemy);

        foreach (Collider2D collider in colliders) 
        {
            if (collider.TryGetComponent<Enemy>(out Enemy enemy))
            {
                Vector2 dir = enemy.transform.position - transform.position;
                enemy.HealthCompo.ApplyDamage(_skill.damage, dir.normalized, _skill.knockPower);
            }
        }
    }

    private void EndOfExplosionAnimation()
    {
        transform.DOKill();
        DestroySelf(0.1f);
    }
    
    
    //크리스탈 스킬 종료
    public void EndOfCrystal()
    {
        _isDestroyed = true;

        if (_skill.canExplode) //폭발성이면 터지도록
        {
            Vector3 endValue = Vector3.one * 2.5f;
            transform.DOScale(endValue, 0.03f);
            _animator.SetTrigger(_hashExplodeTrigger);
        }
        else
        {
            DestroySelf();
        }
    }


    private void DestroySelf(float time = 0.5f)
    {
        _skill.UnlinkThisCrystal();
        _spriteRenderer.DOFade(0f, time).OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
    
    
#if UNITY_EDITOR
    protected virtual void OnDrawGizmos()
    {
        
        if (_skill != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _skill.explosionRadius);
            Gizmos.color = Color.white;
        }
    }
#endif
}
