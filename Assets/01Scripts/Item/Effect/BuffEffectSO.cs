using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;


[CreateAssetMenu(menuName = "SO/Items/Effect/Buff")]
public class BuffEffectSO : ItemEffectSO
{

    public StatType buffStat;
    public int buffAmount;
    public float duration;
    
    private FieldInfo _statField;
    private void OnValidate()
    {
        FindStatField();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        FindStatField();
    }


    private void FindStatField()
    {
        _statField = typeof(CharacterStat).GetField(buffStat.ToString());
    }

    public override void UseEffect()
    {
        CastBuff();
    }
    
    public override bool ExecuteEffectByMelee(bool hitAttack)
    {
        if (!base.ExecuteEffectByMelee(hitAttack)) return false;
        if (activeByHit && !hitAttack) return false; //직접피격이 아니니
        
        CastBuff();
        _lastMeleeEffectTime = Time.time;
        return true;
    }

    private void CastBuff()
    {
        if(Random.Range(0, 100) > effectChance) return;
        
        
        Player player = GameManager.Instance.Player;
        Stat targetStat = _statField.GetValue(player.Stat) as Stat;
        player.Stat.IncreaseStatBy(buffAmount, duration, targetStat);
        
        
    }
}
