using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "SO/Items/Effect/ThunderStrike")]
public class ThunderStrikerEffectSO : ItemEffectSO
{
    public bool usedByMelee;
    public bool usedBySkill;
    public List<PlayerSkill> activeSkillTypeList;
    
    public override void ExecuteEffectByMelee(bool hitAttack)
    {
        if (activeByHit && !hitAttack) return;
        if (!usedByMelee) return;
        
        CastThunderStrike();
    }

    public override void ExecuteEffectBySkill(PlayerSkill skillType)
    {
        Debug.Log(skillType);
        if (!usedBySkill) return;

        PlayerSkill skill = activeSkillTypeList.FirstOrDefault(x => x == skillType);
        if (skill != 0) //못찾은경우가 아니라면 
        {
            CastThunderStrike();
        }
    }

    private void CastThunderStrike()
    {
        if (Random.Range(0, 100f) < effectChance )
        {
            ThunderStrikeSkill skill = SkillManager.Instance.GetSkill<ThunderStrikeSkill>(PlayerSkill.ThunderStrike);
            skill.UseSkillWithoutCooltimeAndEffect();
        }
    }
}
