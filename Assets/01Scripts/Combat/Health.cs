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
    public Action OnDied;
    public Action<Vector2> OnKnockBack;
    
    public UnityEvent OnHitEvent;
    public UnityEvent<Ailment> OnAilmentChanged;

    
    private Entity _owner;
    
    [SerializeField] private AilmentStat _ailmentStat; //질병 및 디버프 관리 스탯
    
    protected void Awake()
    {
        _ailmentStat = new AilmentStat();
        _ailmentStat.EndOFAilmentEvent += HandleEndOfAilment;
        _ailmentStat.AilmentDamageEvent += HandleAilementDamage;
    }

    private void OnDestroy()
    {
        _ailmentStat.EndOFAilmentEvent -= HandleEndOfAilment;
        _ailmentStat.AilmentDamageEvent -= HandleAilementDamage;
    }

    public float GetNormailizedHealth()
    {
        return _currentHealth / (float)maxHealth;
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
        _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, maxHealth);
    }


    protected void Update()
    {
        _ailmentStat.UpdateAilment(); //질병 업데이트
    }

    public void SetOwner(Entity owner)
    {
        _owner = owner;
        _currentHealth = maxHealth = _owner.Stat.maxHealth.GetValue();
    }

    public float GetNormalizedHealth()
    {
        return Mathf.Clamp((float)_currentHealth / maxHealth, 0, 1f);
    }

    public void ApplyDamage(int damage, Vector2 attackDirection, Vector2 knockbackPower, Entity dealer)
    {
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
        }
        
        //아머값에 따른 데미지 보정. 동상시에는 아머 감소.
        damage = _owner.Stat.ArmoredDamage(damage, _ailmentStat.HasAilment(Ailment.Chilled)); 
        _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, maxHealth);
        
        //매직데미지 추가 
        int magicDamage = dealer.Stat.GetMagicDamage();
        CheckAilmentByDamage(damage);

        magicDamage = _owner.Stat.GetMagicDamageAfterResist(magicDamage);
        _currentHealth = Mathf.Clamp(_currentHealth - magicDamage, 0, maxHealth);
        
        
        knockbackPower.x *= attackDirection.x; //y값은 고정으로.
        OnKnockBack?.Invoke(knockbackPower);
        OnHitEvent?.Invoke();
        OnHit?.Invoke();
        
        if (_currentHealth == 0)
        {
            OnDied?.Invoke();
        }
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
            
            Debug.Log($"{gameObject.name} : shocked damage added = {shockDamage}");
        }
    }

    
    
}
