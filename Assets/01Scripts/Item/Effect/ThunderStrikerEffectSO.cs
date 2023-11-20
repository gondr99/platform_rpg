using UnityEngine;

[CreateAssetMenu(menuName = "SO/Items/Effect/ThunderStrike")]
public class ThunderStrikerEffectSO : ItemEffectSO
{
    public override void ExecuteEffect(bool hitAttack)
    {
        if (activeByHit && !hitAttack) return;
        
        if (Random.Range(0, 100f) < effectChance )
        {
            ThunderStrikeSkill skill = SkillManager.Instance.GetSkill<ThunderStrikeSkill>(PlayerSkill.ThunderStrike);
            skill.UserSkillWithoutPercent();
        }
    }
}
