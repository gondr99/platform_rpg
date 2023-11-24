using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class SwordSkillController : MonoBehaviour
{
    [SerializeField] private float _disappearDistance = 1f;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    private Rigidbody2D _rigidbody;
    private CircleCollider2D _circleCollider;
    private Player _player;
    private SwordSkill _swordSkill;
    
    private bool _canRotate = true;
    private bool _isReturning = false;
    private float _returnSpeed;
    private float _lifeTime = 0f;
    private bool _isDestroyed = false;
    
    //바운스 관련 변수들. 
    private bool _isBouncing;
    private float _bounceSpeed;
    private int _bounceAmount;
    private List<Enemy> _targetList = new List<Enemy>();
    private int _targetIndex;
    private int _currentBounceCount = 0;

    //피어싱 관련 변수들
    private int _pierceAmount;
    
    
    //spinner관련 변수들
    private float _maxTravelDistance;
    private float _spinDuration;
    private float _spinTimer;
    private bool _wasStopped;
    private bool _isSpining;
    private float _hitTimer;
    private float _hitCooldown;
    private float _spinXDirection; //스피너가 나가는 X방향.
    
    private readonly int _rotationHash = Animator.StringToHash("Rotation");
    private void Awake()
    {
        Transform visualTrm = transform.Find("Visual");
        _animator = visualTrm.GetComponent<Animator>();
        _spriteRenderer = visualTrm.GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _circleCollider = GetComponent<CircleCollider2D>();
    }

    public void SetupSword(Vector2 dir, float gravityScale, Player player, SwordSkill swordSkill, float returnSpeed = 12f)
    {
        _rigidbody.velocity = dir;
        _rigidbody.gravityScale = gravityScale;
        _player = player;
        _swordSkill = swordSkill;
        _returnSpeed = returnSpeed;

        _lifeTime = _swordSkill.destroyTimer; //시간제한 설정
        _isDestroyed = false;
        
        _spinXDirection = Mathf.Clamp(_rigidbody.velocity.x, -1, 1);//노멀라이즈된 방향값.
        //피어싱 소드가 아닐경우만 회전.
        if(_pierceAmount <= 0)
            _animator.SetBool(_rotationHash, true);
    }
    
    private void Update()
    {
        if (_isDestroyed) return; //이미 파괴된 칼이면 
        
        if(_canRotate)
            transform.right = _rigidbody.velocity;

        if (_isReturning)
        {
            transform.position = Vector2.MoveTowards(transform.position, _player.transform.position,
                _returnSpeed * Time.deltaTime);

            //플레이어와 가까워졌다면 삭제.
            float distanceToPlayer = Vector2.Distance(transform.position, _player.transform.position);
            if (distanceToPlayer < _disappearDistance)
            {
                _swordSkill.CatchSword();
            }

            return;
        }

        _lifeTime -= Time.deltaTime;
        if (_lifeTime <= 0) //라이프타임이 끝났다면
        {
            _isDestroyed = true;
            _swordSkill.DestroyGenerateSword(); //만들어진 소드를 널로 만들고
            _spriteRenderer.DOFade(0, 0.8f).OnComplete(() => Destroy(gameObject));
        }

        if (_isBouncing)
        {
            BounceProcess();
        }

        if (_isSpining)
        {
            SpinProcess();
        }
            
    }

    private void SpinProcess()
    {
        float distance = Vector2.Distance(_player.transform.position, transform.position);
        if (distance > _maxTravelDistance && !_wasStopped)
        {
            StopSpinning();
        }

        //이미 정지된 상태라면 갈갈이 시작.
        if (_wasStopped)
        {
            _spinTimer -= Time.deltaTime;

            //천천히 앞으로 전진
            transform.position = Vector2.MoveTowards(
                transform.position,
                new Vector2(transform.position.x + _spinXDirection, transform.position.y), 1.5f * Time.deltaTime);

            if (_spinTimer < 0)
            {
                _isReturning = true;
                _isSpining = true;
            }

            _hitTimer -= Time.deltaTime;
            if (_hitTimer < 0)
            {
                _hitTimer = _hitCooldown;
                //여기서 데미지 주는 식.
                Collider2D[] colliders = Physics2D.OverlapCircleAll(
                    transform.position, _circleCollider.radius + 0.5f, _swordSkill.whatIsEnemy);
                foreach (Collider2D collider in colliders)
                {
                    if (collider.TryGetComponent<Enemy>(out Enemy enemy))
                    {
                        DamageToTarget(enemy);
                    }
                }
            }
        }
    }

    //스피너를 멈춰야 할 때.
    private void StopSpinning()
    {
        _wasStopped = true;
        _rigidbody.constraints = RigidbodyConstraints2D.FreezePosition;
        _spinTimer = _spinDuration;
    }

    private void BounceProcess()
    {
        //적에게 맞아서 리스트를 뽑았다면.
        if (_targetList.Count > 0)
        {
            Enemy currentTarget = _targetList[_targetIndex];

            transform.position = Vector2.MoveTowards(
                transform.position,
                currentTarget.transform.position, _bounceSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, currentTarget.transform.position) < 0.1f)
            {
                DamageToTarget(currentTarget);
                _targetIndex = (_targetIndex + 1) % _targetList.Count;
                ++_currentBounceCount;
                //한계만큼 다 튕겼다면.
                if (_currentBounceCount >= _bounceAmount)
                {
                    _isBouncing = false;
                    _isReturning = true;
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isReturning)
            return; //만약 복귀중에도 데미지를 주기 원하면 여기에 추가코드 작성가능


        //적에게 꽂혔다면.
        if (other.TryGetComponent<Enemy>(out Enemy enemy))
        {
            
            //바운싱 옵션이 있다면.
            if (_isBouncing && _targetList.Count <= 0)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10f, _swordSkill.whatIsEnemy);
                
                //Health 콤포넌트를 뽑아온다.
                _targetList = colliders.Select(x => x.GetComponent<Enemy>()).ToList();
            }
            else
            {
                DamageToTarget(enemy);    
            }
        }
        
        //해당 오브젝트에 꼽혀서 정지되도록.
        StuckIntoTarget(other);
    }

    private void DamageToTarget(Enemy enemy)
    {
        //프리징 옵션
        if(_swordSkill.canFreeze) //프리징활성화시에만
            enemy.FreezeTimerFor(_swordSkill.freezeTime);
        
        //데미지 옵션
        Vector2 direction = (enemy.transform.position - transform.position).normalized;
        int damage = Mathf.RoundToInt( _player.Stat.GetDamage() * _swordSkill.damageMultiplier ); //배율에 따라 증뎀.
        enemy.HealthCompo.ApplyDamage(damage, direction, _swordSkill.knockbackPower, GameManager.Instance.Player);

        if (_swordSkill.canAilment)
        {
            enemy.HealthCompo.SetAilment(Ailment.Chilled, _swordSkill.ailmentTime, 0 );
        }
        
        //데미지 줄때마다 소드 스킬 피드백 발동시키기.(소드는 UseSkill을 안써)
        SkillManager.Instance.UseSkillFeedback(PlayerSkill.Sword);
    }

    private void StuckIntoTarget(Collider2D other)
    {
        bool isEnemy = other.GetComponent<Enemy>() != null;
        if (_pierceAmount > 0 && isEnemy)
        {
            //적에게 맞혔으나 _pierce횟수가 더 남아있다면 피어싱
            --_pierceAmount;
            return;
        }

        //스핀 검은 박히지 않는다. 다만 땅에 꼴아밖으면 밖힌다.
        if (_isSpining && isEnemy)
        {
            if(!_wasStopped)
                StopSpinning();
            return;
        }
        
        _canRotate = false; //더이상 회전하지 않게 
        _circleCollider.enabled = false;

        _rigidbody.isKinematic = true;
        _rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;

        if (_isBouncing && _targetList.Count > 0)
        {
            return;
        }
        
        _animator.SetBool(_rotationHash, false);
        transform.parent = other.transform;
    }

    public void ReturnSword()
    {
        // 모든 제약조건을 걸어서 물리 영향을 안받게하고 transform으로 땡긴다.
        _rigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
        //_rigidbody.isKinematic = false;
        transform.parent = null;
        //컬라이더는 키면 안돼. 그럼 또다시 흡수돼.
        _isReturning = true; //돌아오도록 설정
    }

    //종류별 셋업
    public void SetupBounce(int bounceAmount, float bounceSpeed)
    {
        _isBouncing = true;
        _bounceAmount = bounceAmount;
        _bounceSpeed = bounceSpeed;
        _currentBounceCount = 0;
    }
    
    public void SetupPierce(int pierceAmount)
    {
        _pierceAmount = pierceAmount;
    }

    public void SetupSpin(float maxTravalDistance, float spinDuration, float hitCooldown)
    {
        _isSpining = true;
        _maxTravelDistance = maxTravalDistance;
        _spinDuration = spinDuration;
        _hitCooldown = hitCooldown;
    }
}
