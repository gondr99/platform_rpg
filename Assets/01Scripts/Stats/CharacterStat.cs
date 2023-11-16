
using UnityEngine;

public abstract class CharacterStat : ScriptableObject
{
    public Stat damage;
    public Stat maxHP;
    public Stat Strength;

    public int GetDamage()
    {
        return damage.GetValue() + Strength.GetValue();
    }
}
