using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerSkill
{
    Dash,
    Clone,
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

    public Skill GetSkill(PlayerSkill skill)
    {
        if (_skills.TryGetValue(skill, out Skill target))
        {
            return target;
        }

        return null;
    }
}
