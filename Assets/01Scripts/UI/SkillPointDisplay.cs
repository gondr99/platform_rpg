using System;
using TMPro;
using UnityEngine;

public class SkillPointDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _skillPointText;
    
    private void OnEnable()
    {
        LevelUpManager.Instance.SkillPointChanged += HandleSkillPointChanged;
        HandleSkillPointChanged(LevelUpManager.Instance.SkillPoint); //열릴때 한번 갱신
    }

    private void OnDisable()
    {
        if(LevelUpManager.Instance != null)
            LevelUpManager.Instance.SkillPointChanged -= HandleSkillPointChanged;   
    }

    private void HandleSkillPointChanged(int point)
    {
        _skillPointText.text = point.ToString();
    }

}
