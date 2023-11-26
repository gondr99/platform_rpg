using System;
using UnityEngine;
using UnityEngine.UI;

public class SkillCooldownIconUI : MonoBehaviour
{
    [SerializeField] private PlayerSkill _skillType;
    [SerializeField] private Image _cooldownImage;
    
    private Skill _targetSkill;
    private void Start()
    {
        _targetSkill = SkillManager.Instance.GetSkill(_skillType);
        _cooldownImage.fillAmount = 0;
        _targetSkill.OnCoolDown += HandleCooldown;
    }

    private void HandleCooldown(float current, float max)
    {
        _cooldownImage.fillAmount = current / max;
    }

    private void OnValidate()
    {
        if (_skillType != 0)
        {
            gameObject.name = $"SkillCooldownUI - [{_skillType.ToString()}]";
        }
    }
}
