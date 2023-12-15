using UnityEngine;

public interface IDamageable
{
    public void ApplyDamage(int damage, Vector2 attackDirection, Vector2 knockbackPower, Entity dealer);

    //상태이상 걸기
    public void SetAilment(Ailment ailment, float duration, int damage);
}
