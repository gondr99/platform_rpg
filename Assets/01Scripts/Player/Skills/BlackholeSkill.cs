using System;
using UnityEngine;

public class BlackholeSkill : Skill
{
    [SerializeField] private BlackholeSkillController _blackholeSkillPrefab;
    
    [Header("Attack info")] 
    public int amountOfAttack = 4;
    public float cloneAttackCooldown = 0.3f;

    [Header("Effect info")]
    public float maxSize;
    public float growSpeed;

    public float maxRiffleCount = 2;
    public float maxRiffleSpeed = 1f;
    
    //스킬의 이펙트가 모두 종료되었을때 발생하는 이벤트.
    public event Action SkillEffectEnd;
    
    private BlackholeSkillController _blackholeSkill;
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
