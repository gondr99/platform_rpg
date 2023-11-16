using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class Health : MonoBehaviour, IDamageable
{
    public int maxHealth;
    private int _currentHealth;

    public Action OnHit;
    public Action OnDied;
    public Action<Vector2> OnKnockBack;
    
    public UnityEvent OnHitEvent;

    private Entity _owner;
    
    public void SetOwner(Entity owner)
    {
        _owner = owner;
        _currentHealth = maxHealth = _owner.Stat.maxHP.GetValue();
    }

    public float GetNormalizedHealth()
    {
        return Mathf.Clamp((float)_currentHealth / maxHealth, 0, 1f);
    }

    public void ApplyDamage(int damage, Vector2 attackDirection, Vector2 knockbackPower, Entity owner)
    {
        //완벽 회피 계산.
        if (_owner.Stat.CanEvasion())
        {
            Debug.Log($"{_owner.gameObject.name} is evasion attack!");
            return;
        }
        //크리티컬확률에 따라 크리티컬인지 확인하고 데미지 증뎀
        if (owner.Stat.IsCritical(ref damage))
        {
            Debug.Log($"Critical! : {damage}"); //데미지 증뎀되었음.
        }
        
        //아머값에 따른 데미지 보정.
        damage = _owner.Stat.ArmoredDamage(damage);
        
        _currentHealth = Mathf.Clamp(_currentHealth - damage, 0, maxHealth);

        knockbackPower.x *= attackDirection.x; //y값은 고정으로.
        OnKnockBack?.Invoke(knockbackPower);
        OnHitEvent?.Invoke();
        OnHit?.Invoke();
        
        if (_currentHealth == 0)
        {
            OnDied?.Invoke();
        }
    }
}
