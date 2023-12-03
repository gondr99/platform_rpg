using System;
using UnityEngine;

public class BlackholeSkill : Skill
{
    [SerializeField] private BlackholeSkillController _blackholeSkillPrefab;
    
    [Header("Attack info")] 
    public int amountOfAttack = 4;
    public float cloneAttackCooldown = 0.3f;
    public float holdBlackholeTime = 2f; //2초간 유지. 2초간 아무것도 안누르면 취소.

    [Header("Effect info")]
    public float maxSize;
    public float growSpeed;

    public float maxRiffleCount = 2;
    public float maxRiffleSpeed = 1f;
    
    //스킬의 이펙트가 모두 종료되었을때 발생하는 이벤트.
    public event Action SkillEffectEnd;
    
    
    [Header("스킬트리셋")] 
    [SerializeField] private SkillTreeSlotUI _unlockBlackholeSlot;
    [SerializeField] private SkillTreeSlotUI _increaseCountSlot;
    [SerializeField] private SkillTreeSlotUI _increaseRadiusSlot;
    
    
    private BlackholeSkillController _blackholeSkill;


    #region 스킬트리 연결부분
    private void Awake()
    {
        _unlockBlackholeSlot.UpgradeEvent += HandleUnlockEvent;
        _increaseCountSlot.UpgradeEvent += HandleIncreaseCountEvent;
        _increaseRadiusSlot.UpgradeEvent += HandleIncreaseRadiusEvent;
    }

    private void OnDestroy()
    {
        _unlockBlackholeSlot.UpgradeEvent -= HandleUnlockEvent;
        _increaseCountSlot.UpgradeEvent -= HandleIncreaseCountEvent;
        _increaseRadiusSlot.UpgradeEvent -= HandleIncreaseRadiusEvent;
    }

    private void HandleUnlockEvent(int currentcount)
    {
        skillEnalbed = true;
    }

    private void HandleIncreaseCountEvent(int currentcount)
    {
        amountOfAttack = 2 + currentcount;
    }

    private void HandleIncreaseRadiusEvent(int currentcount)
    {
        maxSize = 15 + (currentcount - 1) * 0.5f;
    }
    #endregion


    protected override void Start()
    {

        base.Start();
        _blackholeSkill = Instantiate(_blackholeSkillPrefab, transform.position, Quaternion.identity);
        _blackholeSkill.transform.parent = transform;

        _blackholeSkill.SetUpSkill(this);
        
        _blackholeSkill.gameObject.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();
    }

    public override bool AttemptUseSkill()
    {
        return base.AttemptUseSkill();
    }

    public override void UseSkill()
    {
        base.UseSkill();
        
        AudioManager.Instance.PlaySFX(6, null);
    }

    public void BlackholeFieldOpen(Vector3 position)
    {
        _blackholeSkill.transform.position = position;
        _blackholeSkill.InitSkill(); //초기화
        _blackholeSkill.gameObject.SetActive(true);
    }

    public void ReleaseAttack()
    {
        _blackholeSkill.ReleaseCloneAttack();
    }

    public void SkillControllerEnd()
    {
        _blackholeSkill.gameObject.SetActive(false);
        SkillEffectEnd?.Invoke();
    }
}
