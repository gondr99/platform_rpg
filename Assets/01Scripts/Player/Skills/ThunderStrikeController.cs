using System.Threading.Tasks;
using UnityEngine;

public class ThunderStrikeController : MonoBehaviour
{
    [SerializeField] private float _speed;
    private Enemy _target;

    private Animator _animator;

    private bool _isHit = false;
    private readonly int _hashHitTrigger = Animator.StringToHash("Hit");
    private Transform _visualTrm;
    private ThunderStrikeSkill _skill;
    private void Awake()
    {
        _visualTrm = transform.Find("Visual");
        _animator = _visualTrm.GetComponent<Animator>();
    }

    public void Setup(ThunderStrikeSkill skill, Enemy target)
    {
        _target = target;
        _skill = skill;
    }
    
    private void Update()
    {
        if (!_target)
        {
            Destroy(gameObject);
            return;
        }
        
        transform.position =
            Vector2.MoveTowards(transform.position,
                _target.transform.position,
                _speed * Time.deltaTime);
        Vector2 direction = _target.transform.position - transform.position;
        
        if (Vector2.Distance(transform.position, _target.transform.position) < 0.1f && !_isHit)
        {
            _isHit = true;
            HitProcess(direction);
        }
        else if(!_isHit)
        {
            _visualTrm.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg + 90f );
        }
    }

    private async void HitProcess(Vector2 direction)
    {
        _visualTrm.rotation = Quaternion.identity;
        transform.localScale = Vector3.one * 1.2f;
        
        Player player = GameManager.Instance.Player;
        
        //마법공격은 
        _target.HealthCompo.ApplyMagicDamage(
            player.Stat.lightingDamage.GetValue(), 
            direction.normalized, 
            new Vector2(1.5f, 3f), 
            player);
        
        _animator.SetTrigger(_hashHitTrigger);
        
        if (_skill.isShockable && player.Stat.CanAilment(Ailment.Shocked)) //쇼크 공격이 가능하고 확률도 통과하면
        {
            float duration = player.Stat.ailmentTimeMS.GetValue() * 0.001f;
            _target.HealthCompo.SetAilment(Ailment.Shocked, duration, 0); //감전은 그 자체로 데미지는 없다.
        }

        await Task.Delay(400); //이녀석은 시간과 상관없이 가서 Time.scale건드리면 큰일난다.
        Destroy(gameObject);
    }
    
}