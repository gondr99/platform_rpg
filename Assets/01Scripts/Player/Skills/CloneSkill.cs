using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CloneSkill : Skill
{
    [Header("Clone_info")]
    [SerializeField] private CloneSkillController _clonePrefab;
    [SerializeField] private float _cloneDuration;
    [SerializeField] private bool _canAttack;

    [SerializeField] private bool _createCloneOnDashStart;
    [SerializeField] private bool _createCloneOnDashOver;
    [SerializeField] private bool _createCloneOnCounterAttack;
    
    [Header("Duplicate clone")]
    public bool canDuplicateClone; //공격 종료후 다른 클론을 만들어 낼 수 있는가?
    public float duplicatePercent;

    [Header("Crystal instead of clone")]
    [SerializeField] private bool _crystalInsteadOfClone;
    
    public float findEnemyRadius = 5f;
    
    
    [Header("스킬트리셋")] 
    [SerializeField] private SkillTreeSlotUI _unlockCloneSlot;
    [SerializeField] private SkillTreeSlotUI _unlockEndCloneSlot;
    [SerializeField] private SkillTreeSlotUI _unlockCounterCloneSlot;
    [SerializeField] private SkillTreeSlotUI _AdditionalCloneChanceSlot;
    [SerializeField] private SkillTreeSlotUI _crystalInsteadOfCloneSlot;

    public float damageMultiplier = 1f; //증뎀량.

    #region 스킬트리 연결부분
    private void Awake()
    {
        _unlockCloneSlot.UpgradeEvent += HandleUnlockCloneEvent;
        _unlockEndCloneSlot.UpgradeEvent += HandleUnlockEndCloneEvent;
        _unlockCounterCloneSlot.UpgradeEvent += HandleCounterCloneEvent;
        _AdditionalCloneChanceSlot.UpgradeEvent += HandleAdditionalCloneEvent;
        _crystalInsteadOfCloneSlot.UpgradeEvent += HandleCrystalCloneEvent;
    }

    private void OnDestroy()
    {
        _unlockCloneSlot.UpgradeEvent -= HandleUnlockCloneEvent;
        _unlockEndCloneSlot.UpgradeEvent -= HandleUnlockEndCloneEvent;
        _unlockCounterCloneSlot.UpgradeEvent -= HandleCounterCloneEvent;
        _AdditionalCloneChanceSlot.UpgradeEvent -= HandleAdditionalCloneEvent;
        _crystalInsteadOfCloneSlot.UpgradeEvent -= HandleCrystalCloneEvent;
    }

    private void HandleCrystalCloneEvent(int currentCount)
    {
        _crystalInsteadOfClone = true;
    }
    
    private void HandleUnlockCloneEvent(int currentcount)
    {
        skillEnalbed = true;
        _createCloneOnDashStart = true;
        damageMultiplier = 1f + (currentcount - 1) * 0.1f;
    }

    private void HandleUnlockEndCloneEvent(int currentcount)
    {
        _createCloneOnDashOver = true;
    }

    private void HandleCounterCloneEvent(int currentcount)
    {
        _createCloneOnCounterAttack = true;
    }

    private void HandleAdditionalCloneEvent(int currentcount)
    {
        canDuplicateClone = true;
        duplicatePercent = 0.3f + (currentcount - 1) * 0.05f;
    }

    
    #endregion
    
    public void CreateClone(Transform originTrm, Vector3 offset = new Vector3())
    {
        if (_crystalInsteadOfClone)
        {
            _player.skill.GetSkill<CrystalSkill>().CreateCrystal(originTrm.position + offset);
            return;
        }
        
        CloneSkillController newClone = Instantiate(_clonePrefab);
        newClone.SetupClone(this, originTrm, offset, _cloneDuration, _player, _canAttack);
    }


    public void CreateCloneOnDashStart()
    {
        if(_createCloneOnDashStart)
            CreateClone(_player.transform);
    }
    
    public void CreateCloneOnDashOver()
    {
        if(_createCloneOnDashOver)
            CreateClone(_player.transform);
    }

    public async void CreateCloneOnCounterAttack(Transform enemy)
    {
        if (_createCloneOnCounterAttack)
        {
            await Task.Delay(400); //0.4초 딜레이
            CreateClone(enemy.transform, new Vector3(2 * _player.FacingDirection, 0, 0));
        }
    }
}
