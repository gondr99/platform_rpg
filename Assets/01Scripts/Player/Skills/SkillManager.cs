using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerSkill
{
    Dash = 1,
    Clone = 2,
    Sword = 3,
    Blackhole = 4,
    Crystal = 5,
    ThunderStrike = 6
}

public class SkillManager : MonoSingleton<SkillManager>
{
    private Dictionary<PlayerSkill, Skill> _skills = new Dictionary<PlayerSkill, Skill>();

    private void Awake()
    {
        foreach (PlayerSkill skill in Enum.GetValues(typeof(PlayerSkill)))
        {
            Skill skillComponent = GetComponent($"{skill}Skill") as Skill;
            _skills.Add(skill, skillComponent);
        }
    }

    public T GetSkill<T>(PlayerSkill skill) where T : Skill
    {
        
        if (_skills.TryGetValue(skill, out Skill target))
        {
            return target as T;
        }
        return null;
    }

    //Enum타입으로 그냥 스킬 가져오는 방법.
    public Skill GetSkill(PlayerSkill skill)
    {
        if (_skills.TryGetValue(skill, out Skill target))
        {
            return target;
        }
        return null;
    }

    public void UseSkillFeedback(PlayerSkill skillType)
    {
        //스킬을 액티브하게 사용시 발생하는 피드백 이벤트(스킬 사용에 대한 이펙트는 아뮬렛이 담당.
        ItemDataEquipment amulet = Inventory.Instance.GetEquipmentByType(EquipmentType.Amulet);
        if (amulet != null)
        {
            amulet.ItemEffectBySkill(skillType);
        }
    }
}
