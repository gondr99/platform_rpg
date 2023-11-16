using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public void ApplyDamage(int damage, Vector2 attackDirection, Vector2 knockbackPower, Entity owner);
}
