using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Items/Effect/ThunderStrike")]
public class ThunderStrikerEffectSO : ItemEffectSO
{
    public override void UseEffect()
    {
        CastThunderStrike();
    }

    public override bool ExecuteEffectByMelee(bool hitAttack)
    {
        if (!base.ExecuteEffectByMelee(hitAttack)) return false;
        
        if (activeByHit && !hitAttack) return false;

        CastThunderStrike();
        _lastMeleeEffectTime = Time.time;
        return true;
    }

    public override bool ExecuteEffectBySkill(PlayerSkill skillType)
    {
        if (!base.ExecuteEffectBySkill(skillType)) return false;

        PlayerSkill skill = activeSkillTypeList.FirstOrDefault(x => x == skillType);
        if (skill != 0) //못찾은경우가 아니라면 
        {
            CastThunderStrike();
            _lastSkillEffectTime = Time.time;
            return true;
        }
        return false;
    }

    private void CastThunderStrike()
    {
        if (Random.Range(0, 100f) < effectChance )
        {
            ThunderStrikeSkill skill = SkillManager.Instance.GetSkill<ThunderStrikeSkill>();
            skill.UseSkillWithoutCooltimeAndEffect();
        }
    }
}
