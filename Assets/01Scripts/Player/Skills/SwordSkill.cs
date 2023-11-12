
using System;
using UnityEngine;
using UnityEngine.Serialization;

public class SwordSkill : Skill
{
    [Header("Skill info")] 
    [SerializeField] private SwordSkillController _swordPrefab;
    [SerializeField] private Vector2 _launchForce;
    [SerializeField] private float _swordGravity;

    [Header("Aiming Dots")] 
    [SerializeField] private int _numberOfDots;
    [SerializeField] private float _spaceBetweenDots;
    [SerializeField] private GameObject _dotPrefab;
    [SerializeField] private Transform _dotsParent;
    private GameObject[] _dots;
    
    private Vector2 _finalDirection;
    private bool _holdKey = false;

    protected override void Start()
    {
        base.Start();
        _player.PlayerInput.ThrowAimEvent += OnThrowAim; //던지는 키 
        GenerateDots(); //점들을 만들어두고.
    }

    protected override void Update()
    {
        base.Update();
        if (_holdKey)
        {
            for (int i = 0; i < _dots.Length; ++i)
            {
                _dots[i].transform.position = DotPositionOnT(i * _spaceBetweenDots);
            }
        }
    }

    private void OnDestroy()
    {
        _player.PlayerInput.ThrowAimEvent -= OnThrowAim; //던지는 키
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
        newSword.SetupSword(_finalDirection, _swordGravity, _player);
    }

    public Vector2 AimDirection()
    {
        Vector2 playerPosition = _player.transform.position;
        Vector2 mousePosition = GameManager.Instance.MainCam.ScreenToWorldPoint(_player.PlayerInput.AimPosition);

        Vector2 direction = mousePosition - playerPosition;
        
        return direction;
    }

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
}
