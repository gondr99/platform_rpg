using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SwordSkillController : MonoBehaviour
{
    [SerializeField] private float _disappearDistance = 1f;
    private Animator _animator;
    private Rigidbody2D _rigidbody;
    private CircleCollider2D _circleCollider;
    private Player _player;
    private SwordSkill _swordSkill;
    
    private bool _canRotate = true;
    private bool _isReturning = false;
    private float _returnSpeed;

    //바운스 관련 변수들. 
    private bool _isBouncing;
    private float _bounceSpeed;
    private int _bounceAmount;
    private List<Transform> _targetList = new List<Transform>();
    private int _targetIndex;
    private int _currentBounceCount = 0;

    //피어싱 관련 변수들
    //private bool _isPiercing;
    private int _pierceAmount;
    
    
    private readonly int _rotationHash = Animator.StringToHash("Rotation");
    private void Awake()
    {
        _animator = transform.Find("Visual").GetComponent<Animator>();
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
        
        //피어싱 소드가 아닐경우만 회전.
        if(_pierceAmount <= 0)
            _animator.SetBool(_rotationHash, true);
    }

    public void SetupBounce(int bounceAmount, float bounceSpeed)
    {
        _isBouncing = true;
        _bounceAmount = bounceAmount;
        _bounceSpeed = bounceSpeed;
        _currentBounceCount = 0;
    }

    private void Update()
    {
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
        }

        if(_isBouncing)
            BounceProcess();
    }

    private void BounceProcess()
    {
        //적에게 맞아서 리스트를 뽑았다면.
        if (_targetList.Count > 0)
        {
            Transform currentTarget = _targetList[_targetIndex];

            transform.position = Vector2.MoveTowards(
                transform.position,
                currentTarget.position, _bounceSpeed * Time.deltaTime);

            if (Vector2.Distance(transform.position, currentTarget.position) < 0.1f)
            {
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
        if (other.TryGetComponent<Health>(out Health health))
        {
            DamageToTarget(health);
            
            //바운싱 옵션이 있다면.
            if (_isBouncing && _targetList.Count <= 0)
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 10f, _swordSkill.whatIsEnemy);
                
                //transform 콤포넌트를 뽑아온다.
                _targetList = colliders.Select(x => x.transform).ToList();
            }
        }
        
        //해당 오브젝트에 꼽혀서 정지되도록.
        StuckIntoTarget(other);
    }

    private void DamageToTarget(Health health)
    {
        //데미지 옵션
        Vector2 direction = (health.transform.position - transform.position).normalized;
        health.ApplyDamage(_swordSkill.skillDamage, direction, _swordSkill.knockbackPower);
    }

    private void StuckIntoTarget(Collider2D other)
    {
        if (_pierceAmount > 0 && other.GetComponent<Enemy>() != null)
        {
            //적에게 맞혔으나 _pierce횟수가 더 남아있다면 피어싱
            --_pierceAmount;
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

    public void SetupPierce(int pierceAmount)
    {
        
        _pierceAmount = pierceAmount;
    }
}
