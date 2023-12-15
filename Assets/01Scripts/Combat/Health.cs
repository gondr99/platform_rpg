using System;
using UnityEngine;
using UnityEngine.Events;

[Flags]
public enum Ailment : int
{
    None = 0,
    Ignited = 1, // 도트데미지 주는 효과 3초에 걸쳐 0.3초당 3씩 
    Chilled = 2, // 4초간 아머 -20 감소
    Shocked = 4  // 피격시마다 쇼크 데미지 추가.(받는 데미지의 10%, 최소 3데미지)
}
public class Health : MonoBehaviour, IDamageable
{
    public int maxHealth;
    private int _currentHealth;

    public Action OnHit;
    //public Action OnDied;
    public Action<Vector2> OnKnockBack;
    public Action<Color, int> OnDamageText; //데미지 텍스트를 띄워야 할때.

    public UnityEvent<Vector2> OnDeathEvent;
    public UnityEvent OnHitEvent;
    public UnityEvent<Ailment> OnAilmentChanged;

    private Entity _owner;
    public bool isDead = false;
    private bool _isInvincible = false; //무적상태
    [SerializeField] private AilmentStat _ailmentStat; //질병 및 디버프 관리 스탯

    public bool isLastHitCritical = false; //마지막 공격이 크리티컬로 적중했냐?
    public Vector2 lastAttackDirection;
    public bool isHitByMelee;
    
    protected void Awake()
    {
        _ailmentStat = new AilmentStat();
        _ailmentStat.EndOFAilmentEvent += HandleEndOfAilment;
        _ailmentStat.AilmentDamageEvent += HandleAilementDamage;
        isDead = false;
    }

    private void OnDestroy()
    {
        _ailmentStat.EndOFAilmentEvent -= HandleEndOfAilment;
        _ailmentStat.AilmentDamageEvent -= HandleAilementDamage;
    }

    private void HandleEndOfAilment(Ailment ailment)
    {
        Debug.Log($"{gameObject.name} : cure from {ailment.ToString()}");
        //여기서 아이콘 제거등의 일들이 일어나야 한다.
        OnAilmentChanged?.Invoke(_ailmentStat.currentAilment);
        
    }

    private void HandleAilementDamage(Ailment ailment, int damage)
    {
        //종류에 맞춰 글자가 뜨도록 해야한다.
        Debug.Log($"{ailment.ToString()} dot damaged : {damage}");
        OnHit?.Invoke();
        _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, maxHealth);
    }


    protected void Update()
    {
        _ailmentStat.UpdateAilment(); //질병 업데이트
    }

    public void SetOwner(Entity owner)
    {
        _owner = owner;
        _currentHealth = maxHealth = _owner.Stat.GetMaxHealthValue();
    }

    public float GetNormalizedHealth()
    {
        if (maxHealth <= 0) return 0;
        return Mathf.Clamp((float)_currentHealth / maxHealth, 0, 1f);
    }

    public void ApplyHeal(int amount)
    {
        _currentHealth = Mathf.Min(_currentHealth + amount, maxHealth);
        //체력증가에 따른 UI필요.
        Debug.Log($"{_owner.gameObject.name} is healed!! : {amount}");
    }

    public void ApplyDamage(int damage, Vector2 attackDirection, Vector2 knockbackPower, Entity dealer)
    {
        if(isDead || _isInvincible) return; //사망하거나 무적상태면 더이상 데미지 없음.
        
        //완벽 회피 계산.
        if (_owner.Stat.CanEvasion())
        {
            Debug.Log($"{_owner.gameObject.name} is evasion attack!");
            return;
        }
        //크리티컬확률에 따라 크리티컬인지 확인하고 데미지 증뎀
        if (dealer.Stat.IsCritical(ref damage))
        {
            Debug.Log($"Critical! : {damage}"); //데미지 증뎀되었음.
            isLastHitCritical = true;
        }
        else
        {
            isLastHitCritical = false;
        }
        
        //아머값에 따른 데미지 보정. 동상시에는 아머 감소.
        damage = _owner.Stat.ArmoredDamage(damage, _ailmentStat.HasAilment(Ailment.Chilled)); 
        _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, maxHealth);
        
        isHitByMelee = true;
        lastAttackDirection = (transform.position - dealer.transform.position).normalized;
        
        //여기서 데미지 띄워주기
        DamageTextManager.Instance.PopupDamageText(_owner.transform.position, damage, isLastHitCritical ? DamageCategory.Critical : DamageCategory.Noraml);
        
        //감전데미지 체크
        CheckAilmentByDamage(damage);
        
        knockbackPower.x *= attackDirection.x; //y값은 고정으로.
        AfterHitFeedbacks(knockbackPower);
    }

    public void ApplyMagicDamage(int damage, Vector2 attackDirection, Vector2 knockbackPower, Entity dealer)
    {
        int magicDamage = _owner.Stat.GetMagicDamageAfterResist(damage);
        _currentHealth = Mathf.Clamp(_currentHealth - magicDamage, 0, maxHealth);
        Debug.Log($"apply magic damage to {_owner.gameObject.name}! : {damage}");
        
        knockbackPower.x *= attackDirection.x; //y값은 고정으로.
        
        //데미지 띄우기
        DamageTextManager.Instance.PopupDamageText(_owner.transform.position, magicDamage, DamageCategory.Noraml);
        isHitByMelee = false;
        AfterHitFeedbacks(knockbackPower);
    }

    private void AfterHitFeedbacks(Vector2 knockbackPower)
    {
        
        if (_currentHealth == 0)
        {
            isDead = true;
            //OnDied?.Invoke();
            OnDeathEvent?.Invoke(knockbackPower);
            return;
        }

        OnKnockBack?.Invoke(knockbackPower);
        OnHitEvent?.Invoke();
        OnHit?.Invoke();
    }

    //상태이상 걸기.
    public void SetAilment(Ailment ailment, float duration, int damage)
    {
        _ailmentStat.ApplyAilments(ailment, duration, damage);
        OnAilmentChanged?.Invoke(_ailmentStat.currentAilment);
    }

    //데미지를 받았을 때 질병 체크하는 함수 (쇼크 데미지 같은 타격당 데미지에 적용.
    private void CheckAilmentByDamage(int damage)
    {
        //쇼크데미지 추가 부분.
        if (_ailmentStat.HasAilment(Ailment.Shocked)) //쇼크 상태이상이 있다면 데미지의 10% 추뎀 
        {
            int shockDamage = Mathf.Min( 3, Mathf.RoundToInt( damage * 0.1f));
            _currentHealth = Mathf.Clamp(_currentHealth - shockDamage, 0, maxHealth);
            
            //디버프용 데미지 텍스트 추가
            DamageTextManager.Instance.PopupDamageText(_owner.transform.position, shockDamage, DamageCategory.Debuff);
            //Debug.Log($"{gameObject.name} : shocked damage added = {shockDamage}");
        }
    }


    public void MakeInvincible(bool value)
    {
        _isInvincible = value;
    }
    
    
}
