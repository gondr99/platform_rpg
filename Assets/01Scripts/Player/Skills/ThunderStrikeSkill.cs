using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

//3초에 한번씩 3타마다 썬더 스트라이크가 사거리내 랜덤 적에게 확률적으로 발동
public class ThunderStrikeSkill : Skill
{
    [Header("Skill info")] 
    [SerializeField] private ThunderStrikeController _skillPrefab;
    public float effectRadius = 15f;
    public int activePercent = 30; //30퍼 확률로 발동.

    public int amountOfThunder = 1; //떨어지는 번개 수 
    [Header("Ailment")] 
    public bool isShockable; //감점가능

    private List<Enemy> _targetList = new List<Enemy>();
    
    [Header("스킬트리셋")] 
    [SerializeField] private SkillTreeSlotUI _unlockThunderSlot;
    [SerializeField] private SkillTreeSlotUI _unlockShockAilmentSlot;
    [SerializeField] private SkillTreeSlotUI _increaseThunderCountSlot;
    [SerializeField] private SkillTreeSlotUI _increaseThunderPercentSlot;

    private bool _isActivating; //활성화된 상태에서 또 활성화되지 않도록

    private void Awake()
    {
        _unlockThunderSlot.UpgradeEvent += HandleUnlockThunderEvent;
        _unlockShockAilmentSlot.UpgradeEvent += HandleShockAilmentEvent;
        _increaseThunderCountSlot.UpgradeEvent += HandleIncreaseCountEvent;
        _increaseThunderPercentSlot.UpgradeEvent += HandleIncreasePercentEvent;
    }

    private void OnDestroy()
    {
        _unlockThunderSlot.UpgradeEvent -= HandleUnlockThunderEvent;
        _unlockShockAilmentSlot.UpgradeEvent -= HandleShockAilmentEvent;
        _increaseThunderCountSlot.UpgradeEvent -= HandleIncreaseCountEvent;
        _increaseThunderPercentSlot.UpgradeEvent -= HandleIncreasePercentEvent;
    }


    #region 스킬 트리 연결부분
    private void HandleUnlockThunderEvent(int currentcount)
    {
        skillEnalbed = true;
        activePercent = 50;
    }

    private void HandleShockAilmentEvent(int currentcount)
    {
        isShockable = true;
    }

    private void HandleIncreaseCountEvent(int currentcount)
    {
        amountOfThunder = 1 + currentcount;
    }

    private void HandleIncreasePercentEvent(int currentcount)
    {
        activePercent = 50 + currentcount * 10;
    }

    #endregion


    public override void UseSkill()
    {
        if(!skillEnalbed) return; //비활성화시 작동안함.
        if (_isActivating) return; //이미 사용중이라면 작동안함
        base.UseSkill();

        if (Random.Range(0, 100) > activePercent)
            return;
        
        //확률 통과했다면 시작.
        
        FillTargetList();
        DamageToTargets();
    }

    //확률이니 이펙트 상관없이 발동하는것
    public override void UseSkillWithoutCooltimeAndEffect()
    {
        if (_isActivating) return; //이미 사용중이라면 작동안함
        FillTargetList();
        DamageToTargets();
    }

    private async void DamageToTargets()
    {
        Vector3 offset = new Vector3(0, 3.5f); //머리위에서 부터
        foreach (Enemy enemy in _targetList)
        {
            if(enemy == null || enemy.gameObject == null) continue;
            
            ThunderStrikeController thunderInstance = Instantiate(_skillPrefab, enemy.transform.position + offset, Quaternion.identity);
            thunderInstance.Setup(this, enemy);
            await Task.Delay(300);
        }

        _isActivating = false; //이런 작업을 해줘야 Task.Delay에서 리스트가 안고쳐진다.
    }

    private void FillTargetList()
    {
        _isActivating = true; //활성화시키고 (활성화된 동안 다시 발동 안하도록)
        _targetList.Clear();
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_player.transform.position, effectRadius, whatIsEnemy);

        if (colliders.Length == 0) return; //아무것도 없다면 할게 없다.
        
        for (int i = 0; i < amountOfThunder; ++i)
        {
            Enemy enemy = colliders[Random.Range(0, colliders.Length)].GetComponent<Enemy>();
            _targetList.Add( enemy ); 
        }
        
    }
}
