using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

public class BlackholeSkillController : MonoBehaviour
{
    [Header("Hotkey info")] 
    [SerializeField] private HotKeyController _hotKeyPrefab;
    [SerializeField] private HoyKeyIconSO _hoyKeyIcon;
    private List<HotKeyController> _makedHotKeyList = new List<HotKeyController>();
    
    private float _cloneAttackTimer;
    
    [SerializeField] private bool _canGrow;

    private readonly int _HashRiffleSpeed = Shader.PropertyToID("_RiffleSpeed");
    private readonly int _HashRiffleCount = Shader.PropertyToID("_RiffleCount");

    private List<Enemy> _hitTargets = new List<Enemy>();
    private List<Enemy> _freezedTarget = new List<Enemy>();
    
    private Material _riffleMat;
    private SpriteRenderer _spriteRenderer;
    private List<Key> _keyCodeList; //랜덤으로 중복되지 않은 키를 뽑기 위한 리스트

    private bool _cloneAttackReleased; //공격시작을 알리는 불리언 변수.
    private int _remainAttackAmount = 0; //공격횟수를 저장하는 내부 변수
    private bool _skillEnd = false;

    private float _blackholeSkillTimer;

    private BlackholeSkill _skill;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _riffleMat = _spriteRenderer.material;
    }

    //스킬 초기화
    public void InitSkill()
    {
        _canGrow = true;
        _keyCodeList = _hoyKeyIcon.GetAllKeyFromList();
        _cloneAttackReleased = false;
        _skillEnd = false;
        _remainAttackAmount = _skill.amountOfAttack;
        
        _hitTargets.Clear();
        _freezedTarget.Clear();
        _makedHotKeyList.Clear();
        _blackholeSkillTimer = 0;
    }

    public void SetUpSkill(BlackholeSkill blackholeSkill)
    {
        _skill = blackholeSkill;
        _riffleMat.SetFloat(_HashRiffleSpeed, _skill.maxRiffleSpeed);
        _riffleMat.SetFloat(_HashRiffleCount, _skill.maxRiffleCount);
    }

    private void Update()
    {
        _blackholeSkillTimer += Time.deltaTime;
        //스킬이 아직 안끝났는데 시간이 다되었다면 강제로 R키 발동.
        if (_blackholeSkillTimer >= _skill.holdBlackholeTime && _skillEnd == false)
        {
            ReleaseCloneAttack();
        }
        
        _cloneAttackTimer -= Time.deltaTime;
        
        if (_cloneAttackTimer < 0 && _cloneAttackReleased )
        {
            CloneAttackProcess();
        }
        
        if (_canGrow)
        {
            transform.localScale =
                Vector3.Lerp(transform.localScale, Vector3.one * _skill.maxSize, _skill.growSpeed * Time.deltaTime);
            
        }
    }

    //공격을 종료해주는 것.
    public void ReleaseCloneAttack()
    {
        _skillEnd = true; //발동
        
        if (_hitTargets.Count <= 0)
        {
            ShrinkBlackhole(false); //공격종료
        }
        else
        {
            GameManager.Instance.Player.FadePlayer(true, 0.3f); //플레이어를 페이드 아웃 시키고
            _cloneAttackReleased = true;
        }

    }

    //클론 공격 
    private void CloneAttackProcess()
    {
        _cloneAttackTimer = _skill.cloneAttackCooldown;

        CloneSkill cloneSkill = SkillManager.Instance.GetSkill<CloneSkill>();

        float xOffset = Mathf.Sign(Random.value - 0.5f) * 2f; //50%확률로 왼쪽 오른쪽
        int randomIdx = Random.Range(0, _hitTargets.Count);
        cloneSkill.CreateClone(_hitTargets[randomIdx].transform, new Vector3(xOffset, 0, 0));

        --_remainAttackAmount;
        if (_remainAttackAmount <= 0)
        {
            ShrinkBlackhole(true); //공격종료
            _cloneAttackReleased = false;
        }
    }

    //블랙홀 접기
    private async void ShrinkBlackhole(bool isDelayed)
    {
        _canGrow = false;

        if(isDelayed)
            await Task.Delay(1000); //1초 정도 대기하고 (분신들이 사라질 시간.
        
        
        transform.DOScale(Vector3.zero, 0.4f).OnComplete(()=>
        {
            foreach (Enemy enemy in _freezedTarget)
            {
                enemy.FreezeTime(false); //프리즈 다 풀어준다.
            }

            //만들었던 핫키들 정리
            foreach (HotKeyController hotkey in _makedHotKeyList)
            {
                Destroy(hotkey.gameObject);
            }
            
            if(_hitTargets.Count > 0)
                GameManager.Instance.Player.FadePlayer(false, 0.3f); //공격이 끝나면 페이드인
            //콘트롤러 엔딩
            _skill.SkillControllerEnd();
        });
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<Enemy>(out Enemy enemy) && !_skillEnd)
        {
            enemy.FreezeTime(true, isFrozenWithoutTimer:true); //영구결빙
            _freezedTarget.Add(enemy); //프리징 시킨 적들은 나중에 풀어줘야 해.
            
            CreateHoyKeyOnEnemyHead(enemy);
        }
    }

    private void CreateHoyKeyOnEnemyHead(Enemy enemy)
    {
        if (_keyCodeList.Count == 0) return;

        Vector3 spawnPostion = enemy.transform.position + new Vector3(0, 1.5f);
        
        //Key key = _hoyKeyIcon.GetRandomKeyFromList(); 중복 허용할꺼면 이렇게
        Key key = _keyCodeList[Random.Range(0, _keyCodeList.Count)];
        _keyCodeList.Remove(key); //뽑은건 없애버려.
        
        HotKeyController hotKeyInstance = Instantiate(_hotKeyPrefab, spawnPostion, Quaternion.identity);
        hotKeyInstance.SetupHotKey(key, enemy, this);
        
        _makedHotKeyList.Add(hotKeyInstance);
    }

    public void AddEnemyToTargetList(Enemy enemy)
    {
        _hitTargets.Add(enemy);
    }
}
