using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public enum SwordSkillType
{
    Regular,
    Bounce,
    Pierce,
    Spin
}

public class SwordSkill : Skill
{
    public SwordSkillType swordSkillType = SwordSkillType.Regular;
    
    [Header("Skill info")] 
    [SerializeField] private SwordSkillController _swordPrefab;
    [SerializeField] private Vector2 _launchForce;
    [SerializeField] private float _swordGravity;
    [SerializeField] private float _returnSpeed = 16f;

    public float freezeTime = 0.7f;
    public float damageMultiplier = 1;
    public Vector2 knockbackPower;
    public float returnImpactPower = 8;
    public float destroyTimer = 7f;

    [Header("Pierce info")] 
    [SerializeField] private int _pierceAmount;
    [SerializeField] private float _pierceGravity;
    
    [Header("Bouncing info")]
    [SerializeField] private float _bounceSpeed = 20f;
    [SerializeField] private int _bounceAmount = 4;
    [SerializeField] private float _bounceGravity = 3f;

    [Header("Spin info")]
    [SerializeField] private float _maxTravelDistance = 7;
    [SerializeField] private float _spinDuration = 2;
    [SerializeField] private float _spinGravity = 1;
    [SerializeField] private float _hitCooldown = 0.35f;
    
    [Header("Aiming Dots")] 
    [SerializeField] private int _numberOfDots;
    [SerializeField] private float _spaceBetweenDots;
    [SerializeField] private GameObject _dotPrefab;
    [SerializeField] private Transform _dotsParent;
    private GameObject[] _dots;
    
    private Vector2 _finalDirection;
    private bool _holdKey = false;

    [HideInInspector] public SwordSkillController generatedSword;

    [SerializeField] private SkillTreeSlotUI _enableSkillSlot;
    [SerializeField] private SkillTreeSlotUI _pierceShotSkillSlot;
    [SerializeField] private SkillTreeSlotUI _bounceShotSkillSlot;
    [SerializeField] private SkillTreeSlotUI _spinShotSkillSlot;
    [SerializeField] private SkillTreeSlotUI _freezeSkillSlot;
    [SerializeField] private SkillTreeSlotUI _ailmentSkillSlot;

    public bool canFreeze; //타격시 적을 순간적으로 프리즈 시키는가?
    public bool canAilment;
    public float ailmentTime = 1f;
    
    private void Awake()
    {
        _enableSkillSlot.UpgradeEvent += HandleEnableEvent;
        _pierceShotSkillSlot.UpgradeEvent += HandlePierceShotEvent;
        _bounceShotSkillSlot.UpgradeEvent += HandleBounceShotEvent;
        _spinShotSkillSlot.UpgradeEvent += HandleSpinShotEvent;
        _freezeSkillSlot.UpgradeEvent += HandleFreezeSkillEvent;
        _ailmentSkillSlot.UpgradeEvent += HandleAilmentSkillEvent;
    }

    
    #region 스킬트리 시스템에 반응하는 핸들러 함수들

    private void HandleEnableEvent(int currentcount)
    {
        skillEnalbed = true; //해당 스킬을 활성화해줌.
        Player player = GameManager.Instance.Player;
        player.PlayerInput.ThrowAimEvent += OnThrowAim; //던지는 키
    }

    private void HandlePierceShotEvent(int currentcount)
    {
        if (currentcount == 1)
        {
            swordSkillType = SwordSkillType.Pierce;
        }
        _pierceAmount = 2 + currentcount * 2;
    }
    private void HandleBounceShotEvent(int currentcount)
    {
        if (currentcount == 1)
        {
            swordSkillType = SwordSkillType.Bounce;
        }
        _bounceAmount = 2 + currentcount;
    }
    
    private void HandleSpinShotEvent(int currentcount)
    {
        if (currentcount == 1)
        {
            swordSkillType = SwordSkillType.Spin;
        }

        _hitCooldown = 0.5f - currentcount * 0.05f;
    }
    
    private void HandleFreezeSkillEvent(int currentcount)
    {
        canFreeze = true;
    }
    
    private void HandleAilmentSkillEvent(int currentcount)
    {
        canAilment = true;
        ailmentTime = 1f + currentcount * 0.5f;
    }


    #endregion
    
    


    protected override void Start()
    {
        base.Start();
        if (skillEnalbed)
        {
            _player.PlayerInput.ThrowAimEvent += OnThrowAim; //던지는 키 
        }
        
        GenerateDots(); //점들을 만들어두고.
        SetupGravity(); //현재 스킬의 종류에 맞게 그라비티 셋팅
    }
    
    //각 이벤트 해제
    private void OnDestroy()
    {
        _enableSkillSlot.UpgradeEvent -= HandleEnableEvent;
        _pierceShotSkillSlot.UpgradeEvent -= HandlePierceShotEvent;
        _bounceShotSkillSlot.UpgradeEvent -= HandleBounceShotEvent;
        _spinShotSkillSlot.UpgradeEvent -= HandleSpinShotEvent;
        _freezeSkillSlot.UpgradeEvent -= HandleFreezeSkillEvent;
        _ailmentSkillSlot.UpgradeEvent -= HandleAilmentSkillEvent;
        if (skillEnalbed)
        {
            _player.PlayerInput.ThrowAimEvent -= OnThrowAim; //던지는 키
        }
    }

    //각 스킬에 맞게 중력 설정.
    private void SetupGravity()
    {
        if (swordSkillType == SwordSkillType.Pierce)
        {
            _swordGravity = _pierceGravity;
        }

        if (swordSkillType == SwordSkillType.Bounce)
        {
            _swordGravity = _bounceGravity;
        }

        if (swordSkillType == SwordSkillType.Spin)
        {
            _swordGravity = _spinGravity;
        }
    }

    protected override void Update()
    {
        base.Update();
        if (_holdKey && generatedSword == null)
        {
            for (int i = 0; i < _dots.Length; ++i)
            {
                _dots[i].transform.position = DotPositionOnT(i * _spaceBetweenDots);
            }
        }
    }


    private void OnThrowAim(bool state)
    {
        _holdKey = state; //눌리면 true로 셋팅됨.
        if (!_holdKey) //키가 떼지는 순간.
        {
            _finalDirection = AimDirection().normalized * _launchForce;
        }
    }
    
    public void CreateSword()
    {
        SwordSkillController newSword = Instantiate(_swordPrefab, _player.transform.position, Quaternion.identity);

        if (swordSkillType == SwordSkillType.Bounce)
        {
            newSword.SetupBounce(_bounceAmount, _bounceSpeed);
        }else if (swordSkillType == SwordSkillType.Pierce)
        {
            newSword.SetupPierce(_pierceAmount); //관통 검 셋팅
        }else if (swordSkillType == SwordSkillType.Spin)
        {
            newSword.SetupSpin(_maxTravelDistance, _spinDuration, _hitCooldown); //회전검 셋팅
        }
        //최종적으로 셋업을 해서 칼이 결정하도록 한다.
        newSword.SetupSword(_finalDirection, _swordGravity, _player, this, _returnSpeed);
        
        generatedSword = newSword; //만들어진 검을 할당함.
    }

    public Vector2 AimDirection()
    {
        Vector2 playerPosition = _player.transform.position;
        Vector2 mousePosition = GameManager.Instance.MainCam.ScreenToWorldPoint(_player.PlayerInput.AimPosition);

        Vector2 direction = mousePosition - playerPosition;
        
        return direction;
    }

    public void CatchSword()
    {
        //칼을 잡았다면 플레이어를 캐치소드로 변경.
        _player.StateMachine.ChangeState(StateEnum.CatchSword);
        Destroy(generatedSword.gameObject);
        generatedSword = null;
    }

    //생성된 칼이 너무 멀리 갔을 때 파괴하는것.
    public void DestroyGenerateSword()
    {
        generatedSword = null;
    }

    public void ReturnGenerateSword()
    {
        if (generatedSword != null)
        {
            generatedSword.ReturnSword();
        }
    }
    
    #region Guide Dots region

    //가이드 점을 껐다 켰다.
    public void DotsActive(bool state)
    {
        for (int i = 0; i < _dots.Length; ++i)
        {
            _dots[i].SetActive(state);
        }
    }
    
    //가이드라인 그려주는 함수.
    private void GenerateDots()
    {
        _dots = new GameObject[_numberOfDots]; //포인터만 만든거임.
        for (int i = 0; i < _numberOfDots; ++i)
        {
            //만들어서 부모밑에 넣어둔다.(비활성화 상태)
            _dots[i] = Instantiate(_dotPrefab, _player.transform.position, Quaternion.identity, _dotsParent);
            _dots[i].SetActive(false);
        }
    }

    private Vector2 DotPositionOnT(float t)
    {
        Vector2 normalizedAim = AimDirection().normalized;
        
        Vector2 position = (Vector2)_player.transform.position 
                           //+ new Vector2(normalizedAim.x * _launchForce.x, normalizedAim.y * _launchForce.y ) * t
                           + normalizedAim * _launchForce * t
                           + Physics2D.gravity * (_swordGravity * (t*t) * 0.5f);
        // x축은 힘에 시간을 곱하면 되고
        // y축은 힘에 시간을 곱한거에서 중력 * 시간 제곱을 반으로 나눈걸 빼주면 돼. 이때 gravity가 이미 음수이기 때문에 더해준거.
        //만약 mass가 있다면 질량으로 나눠주면 돼.
        return position;
    }
    
    #endregion
    
}
