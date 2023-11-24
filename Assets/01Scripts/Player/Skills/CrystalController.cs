using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class CrystalController : MonoBehaviour
{
    private float _crystalExistTimer;
    private CrystalSkill _skill;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    private bool _isDestroyed = false;
    private Transform _closestTarget = null;
    
    private readonly int _hashExplodeTrigger = Animator.StringToHash("Explode");

    private bool _isLauched = false;
    private bool _autoFindCloset = false;
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    public void SetupCrystal(CrystalSkill skill, float timer, LayerMask whatIsEnemy, bool findClosest = true)
    {
        _skill = skill;
        _crystalExistTimer = timer;
        _autoFindCloset = findClosest;
        if(findClosest)
            _closestTarget = _skill.FindClosestEnemy(transform, whatIsEnemy, _skill.findEnemyRadius); //가장 가까운 적을 찾는다.
        _isLauched = false;
    }


    private void Update()
    {
        if (_autoFindCloset && _closestTarget == null)
        {
            _closestTarget = _skill.FindClosestEnemy(transform, _skill.whatIsEnemy, _skill.findEnemyRadius);
        }
        
        _crystalExistTimer -= Time.deltaTime;
        if (_crystalExistTimer <= 0 && !_isDestroyed)
        {
            EndOfCrystal();
            return;
        }

        if (_skill.canMoveToEnemy && _closestTarget != null)
        {
            ChaseToTarget();
        }

        //다중 크리스탈 시스템에서 발사시에 해야할 일.
        if (_skill.isMultipleCrystal && _isLauched)
        {
            ChaseToTarget(3f);
        }
    }

    public void StartPulseMove()
    {
        Vector3 pos = transform.localPosition;
        transform.DOLocalMoveY(pos.y + 0.5f, 0.5f).SetLoops(-1, LoopType.Yoyo);
    }

    private void ChaseToTarget(float speedMultiplier = 1f)
    {
        transform.position = Vector2.MoveTowards(transform.position, _closestTarget.position,
            _skill.moveSpeed * speedMultiplier * Time.deltaTime);

        if (_isDestroyed) return;
        
        if (Vector2.Distance(transform.position, _closestTarget.position) < 1f)
        {
            EndOfCrystal();
        }
    }

    public void LaunchToTarget(Transform targetTrm)
    {
        _closestTarget = targetTrm;
        transform.DOKill(); //모든 트윈 제거.
        transform.DOLocalMoveY(2f, 0.3f).OnComplete(() =>
        {
            _isLauched = true;
            transform.parent = null;
        });
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
                Player player = GameManager.Instance.Player;
                //배율에 따라 증가된 값으로 데미지
                int damage = Mathf.RoundToInt(
                        (_skill.damage + player.Stat.intelligence.GetValue()) * _skill.damageMultiplier );
                enemy.HealthCompo.ApplyMagicDamage(damage, dir.normalized, _skill.knockPower, player);
                
                if (_skill.isShockable && Random.value <= _skill.shockPercent) //쇼크 공격이 가능하고 확률도 통과하면
                {
                    float duration = player.Stat.ailmentTimeMS.GetValue() * 0.001f;
                    enemy.HealthCompo.SetAilment(Ailment.Shocked, duration, 0); //감전은 그 자체로 데미지는 없다.
                }
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
