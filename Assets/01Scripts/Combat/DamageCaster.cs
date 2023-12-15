using UnityEngine;

public class DamageCaster : MonoBehaviour
{
    public Transform attackChecker;
    public float attackCheckRadius;

    public Vector2 knockbackPower;
    
    [SerializeField] private int _maxHitCount = 5; //최대로 때릴 수 있는 적 갯수
    public LayerMask whatIsEnemy;
    private Collider2D[] _hitResult;

    private Entity _owner;
    private bool _castByCloneSkill;
    private void Awake()
    {
        _hitResult = new Collider2D[_maxHitCount];
    }

    public void SetOwner(Entity owner, bool castByCloneSkill)
    {
        _owner = owner;
        _castByCloneSkill = castByCloneSkill;
    }

    public bool CastDamage()
    {
        int cnt = Physics2D.OverlapCircleNonAlloc(attackChecker.position, attackCheckRadius, _hitResult, whatIsEnemy);

        //이거 쓰면 라이더가 경고 때리는데 위에껀 유니티에서 쓰지말라함..아잇...
        //Physics2D.OverlapCircleAll(attackChecker.position, attackCheckRadius, whatIsEnemy);
        
        for (int i = 0; i < cnt; ++i)
        {
            Vector2 direction = (_hitResult[i].transform.position - transform.position).normalized;
            if (_hitResult[i].TryGetComponent<IDamageable>(out IDamageable health))
            {
                int damage = _owner.Stat.GetDamage();
                if (_castByCloneSkill)
                {
                    damage =  Mathf.RoundToInt(damage * SkillManager.Instance.GetSkill<CloneSkill>().damageMultiplier);
                }
                health.ApplyDamage(damage, direction, knockbackPower, _owner);
                SetAilmentByStat(health);
            }
        }

        return cnt > 0;
    }

    private void SetAilmentByStat(IDamageable targetHealth)
    {
        CharacterStat stat = _owner.Stat; //주인의 스탯참조
        float duration = stat.ailmentTimeMS.GetValue() * 0.001f;
        
        if (stat.canIgniteByMelee && stat.CanAilment(Ailment.Ignited)) //점화 가능
        {
            int damage = stat.GetDotDamage(Ailment.Ignited);
            targetHealth.SetAilment(Ailment.Ignited, duration, damage);
        }

        if (stat.canChillByMelee && stat.CanAilment(Ailment.Chilled))
        {
            int damage = stat.GetDotDamage(Ailment.Chilled);
            targetHealth.SetAilment(Ailment.Chilled, duration, damage);
        }
        
        if (stat.canShockByMelee && stat.CanAilment(Ailment.Shocked))
        {
            int damage = stat.GetDotDamage(Ailment.Shocked);
            targetHealth.SetAilment(Ailment.Shocked, duration, damage);
        }
    }
    
    private void OnDrawGizmos()
    {
        if(attackChecker != null)
            Gizmos.DrawWireSphere(attackChecker.position, attackCheckRadius);
    }
}
