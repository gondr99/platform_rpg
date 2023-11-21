using UnityEngine;

[CreateAssetMenu(menuName = "SO/Items/Effect/Heal")]
public class HealEffectSO : ItemEffectSO
{
    //흡혈은 melee로 
    [Range(0f, 1f)] [SerializeField] private float healPercent;
    public override void UseEffect()
    {
        CastHeal();
    }

    public override bool ExecuteEffectByMelee(bool hitAttack)
    {
        if (!base.ExecuteEffectByMelee(hitAttack)) return false;
        if (activeByHit && !hitAttack) return false; //피격시 활성화 조건인데 안맞았으면 발동 안함.
        
        CastHeal();
        _lastMeleeEffectTime = Time.time;
        return true;
    }

    private void CastHeal()
    {
        if (Random.Range(0, 100) > effectChance) return;
        
        Player player = GameManager.Instance.Player;
        PlayerStat stat = player.Stat as PlayerStat;

        //최소 1은 회복되도록
        int healAmount = Mathf.Max(1, Mathf.RoundToInt(stat.GetMaxHealthValue() * healPercent));

        player.HealthCompo.ApplyHeal(healAmount);
    }
}
