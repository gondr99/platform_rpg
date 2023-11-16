using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class CrystalSkill : Skill
{
    [SerializeField] private CrystalController _crystalPrefab;
    [SerializeField] private float _timeOut = 5f;
    private CrystalController _currentCrystal;
    
    public int damage = 5;
    public Vector2 knockPower;
    
    [Header("Explosion Crystal")] 
    public bool canExplode;
    public float explosionRadius = 3f;

    [Header("Moving Crystal")] 
    public bool canMoveToEnemy;
    public float moveSpeed;
    public float findEnemyRadius = 10f;

    [Header("Multiple Crystal")]
    public bool isMultipleCrystal; 
    public int amountOfCrystal;
    public float multiCrystalCooldown;
    [HideInInspector] public List<CrystalController> crystalList;
    private bool _readyToLaunch = false;

    private Vector2[] offsets =
    {
        new Vector2(0.3f, 0.3f),
        new Vector2(-0.3f, 0.3f),
        new Vector2(0, -0.3f),
    };

        public override void UseSkill()
    {
        base.UseSkill();

        if (isMultipleCrystal)  //다중발사 시스템일 경우 좀 다르게 동작.
        {
            _cooldownTimer = 0; //연사가 가능하게 하고 3발 다 쏘면 쿨타임 적용.
            if (crystalList.Count == 0)
            {
                MakeMultipleCrystal();
            }else if (_readyToLaunch) //갯수가 있고 발사가능이면 한발씩 발사.
            {
                
                Transform target = FindClosestEnemy(_player.transform, _player.DamageCasterCompo.whatIsEnemy, findEnemyRadius);
                if (target != null)
                {
                    crystalList[crystalList.Count - 1].LaunchToTarget(target);
                    crystalList.RemoveAt(crystalList.Count - 1);

                    if (crystalList.Count == 0)
                    {
                        _cooldownTimer = multiCrystalCooldown;
                    }
                }
                else
                {
                    Debug.Log("There are no enemy in range");
                }
            }

            return;
        }
        
        

        if (_currentCrystal == null)
        {
            _currentCrystal = Instantiate(_crystalPrefab, _player.transform.position, Quaternion.identity);
            _currentCrystal.SetupCrystal(this,_timeOut, _player.DamageCasterCompo.whatIsEnemy);
        }
        else if(!canMoveToEnemy)
        {
            //플레이어와 위치 바꾸기
            Vector2 playerPos = _player.transform.position;
            _player.transform.position = _currentCrystal.transform.position;
            _currentCrystal.transform.position = playerPos;
            
            _currentCrystal?.EndOfCrystal();
        }
    }

    //등뒤에 다중 크리스털 생성
    private async void MakeMultipleCrystal()
    {
        //지정된 갯수만큼 크리스탈을 만든다.
        for (int i = 0; i < amountOfCrystal; ++i)
        {
            //Vector2 offset = Random.insideUnitCircle * 0.5f;
            CrystalController instance = Instantiate(_crystalPrefab, _player.backTrm.position + (Vector3)offsets[i], Quaternion.identity);
            instance.transform.localScale = Vector3.one * 0.5f; //절반 크기
            instance.SetupCrystal(this,_timeOut*5, _player.DamageCasterCompo.whatIsEnemy, false); //5배시간.
            crystalList.Add(instance);
            instance.transform.parent = _player.backTrm; //부모 지정.
            
            instance.StartPulseMove();
            await Task.Delay(100); //0.1초 간격으로
        }

        _readyToLaunch = true; //발사 준비 완료.
    }

    public void UnlinkThisCrystal()
    {
        _currentCrystal = null;
    }
}
