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
    
    private void Start()
    {
        _currentHealth = maxHealth;
    }

    public float GetNormalizedHealth()
    {
        return Mathf.Clamp((float)_currentHealth / maxHealth, 0, 1f);
    }

    public void ApplyDamage(int damage, Vector2 attackDirection, Vector2 knockbackPower)
    {
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
