using System;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class CloneSkillController : MonoBehaviour
{
    [SerializeField] private int _attackCategoryCount = 3;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private DamageCaster _damageCaster;

    private Transform _closestEnemy;
    
    private readonly int _attackNumberHash = Animator.StringToHash("AttackNumber");
    private int _facingDirection = 1;
    
    private CloneSkill _skill;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _damageCaster = transform.Find("DamageCaster").GetComponent<DamageCaster>();
        
    }
    
    public void SetupClone(CloneSkill skill, Transform originTrm, Vector3 offset, float cloneDuration, Entity owner,  bool canAttack = false)
    {
        if (canAttack)
        {
            _animator.SetInteger(_attackNumberHash, Random.Range(1, _attackCategoryCount + 1));
        }
        _skill = skill;
        _damageCaster.SetOwner(owner, castByCloneSkill:true); //데미지 셋팅
        transform.position = originTrm.position + offset;
        FacingClosetTarget(); //가장 가까운 적 찾아서 바라보고.
        FadeAfterDelay(cloneDuration);
    }

    private async void FadeAfterDelay(float delay)
    {
        await Task.Delay(Mathf.FloorToInt(delay * 1000));
        _spriteRenderer.DOFade(0, 0.7f).OnComplete(() =>
        {
            Destroy(gameObject); //페이딩 끝나면 삭제. 굳이 풀매니징까지는 안해도 된다.
        });
    }
    
    //생성되면 가장 가까운 적을 향하도록 함.
    private void FacingClosetTarget()
    {
        _closestEnemy = _skill.FindClosestEnemy(transform, _damageCaster.whatIsEnemy, _skill.findEnemyRadius);        

        if (_closestEnemy != null)
        {
            if (transform.position.x > _closestEnemy.position.x)
            {
                _facingDirection = -1;
                transform.Rotate(0, 180, 0);
            }
        }
    }
    
    private void AnimationTrigger()
    {
        
    }

    //이건 공격 콤포넌트를 따로 만들어야 해. SOLID에 어긋나.
    private void AttackTrigger()
    {
        bool success = _damageCaster.CastDamage();

        if (success && _skill.canDuplicateClone)
        {
            if (Random.value < _skill.duplicatePercent)
            {
                _skill.CreateClone(transform, new Vector3(1.5f * _facingDirection,0,0));
            }
        }
    }
}
